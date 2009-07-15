using System;
using System.Collections.Generic;
using System.Text;

/* TNS */
using TNS.AdExpress.Domain.Classification;
using System.Xml;
using TNS.AdExpress.Domain.Layers;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.XmlLoader
{
    public class NyxConfigurationXL
    {
        /// <summary>
        /// Load the configuration from an XmlTextReader
        /// </summary>
        /// <param name="nyxConfiguration">Configuration source</param>
        /// <param name="defaultFilePath">Out parameter where default file path should be stored</param>
        /// <param name="defaultLongevity">Out parameter where the default longevity should be stored</param>
        /// <param name="dataAccessLayers">DataAccessLayer dictionary</param>
        /// <param name="plugins">Plugin dictionary</param>
        public static bool Load(IDataSource nyxConfiguration, out string defaultFilePath, out int defaultLongevity, out string defaultThemePath,
                                out Dictionary<NyxDataAccessLayer, DataAccessLayer> dataAccessLayers,
                                out Dictionary<PluginType, PluginInformation> plugins, out string targetHost)
        {
            bool isActivated = false;

            // Default values
            defaultFilePath = "";
            defaultThemePath = "";
            targetHost = "http://tnsadexpress.com/";
            defaultLongevity = 30;
            dataAccessLayers = new Dictionary<NyxDataAccessLayer, DataAccessLayer>();
            plugins = new Dictionary<PluginType, PluginInformation>();

            // Checking if configuration source is defined
            if (nyxConfiguration != null)
            {
                nyxConfiguration.Open();
                XmlTextReader reader = (XmlTextReader)nyxConfiguration.GetSource();

                // Reading configuration
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case "NyxConfiguration":
                                if (reader.GetAttribute("isAlertsActivated") != null)
                                    bool.TryParse(reader.GetAttribute("isAlertsActivated"), out isActivated);
                                if (reader.GetAttribute("targetHost") != null)
                                {
                                    targetHost = reader.GetAttribute("targetHost");
                                    if (targetHost.EndsWith("/") == false)
                                        targetHost += "/";
                                }
                                break;
                            case "dataAccessLayers":
                                break;
                            case "dataAccessLayer":
                                // Retrieving DataAccessLayer information
                                string layerClass = reader.GetAttribute("class");
                                string layerName = reader.GetAttribute("enum");
                                string layerAsssembly = reader.GetAttribute("assemblyName");

                                // Checking if we have all the required parameters
                                // and adding a new DataAccessLayer object
                                if (layerName != null && layerAsssembly != null && layerClass != null &&
                                    layerName.Length > 0 && layerAsssembly.Length > 0 && layerClass.Length > 0)
                                    dataAccessLayers.Add((NyxDataAccessLayer)Enum.Parse(typeof(NyxDataAccessLayer), layerName),
                                                              new DataAccessLayer(layerName, layerAsssembly, layerClass));
                                break;
                            case "plugins":
                                // Reading attributes and setting application
                                // default values
                                if (reader.GetAttribute("defaultLongevity") != null)
                                    defaultLongevity = int.Parse(reader.GetAttribute("defaultLongevity"));
                                if (reader.GetAttribute("defaultFilePath") != null)
                                    defaultFilePath = reader.GetAttribute("defaultFilePath");
                                if (reader.GetAttribute("defaultThemePath") != null)
                                    defaultThemePath = reader.GetAttribute("defaultThemePath");
                                break;
                            case "plugin":

                                // Reading plugin information
                                try
                                {
                                    #region Loading plugin attributes
                                    int resultType = int.Parse(reader.GetAttribute("resultType"));
                                    string name = reader.GetAttribute("name");

                                    // Loading plugin file path attribute
                                    string filePath = defaultFilePath;
                                    if (reader.GetAttribute("filePath") != null)
                                        filePath = reader.GetAttribute("filePath");

                                    // Loading longevity attribute
                                    int longevity = defaultLongevity;
                                    if (reader.GetAttribute("longevity") != null)
                                        longevity = int.Parse(reader.GetAttribute("longevity"));

                                    // Loading deleteExpired attribute
                                    bool deleteExpired = false;
                                    if (reader.GetAttribute("deleteExpired") != null)
                                        deleteExpired = bool.Parse(reader.GetAttribute("deleteExpired"));

                                    // Loading theme path
                                    string themePath = defaultThemePath;
                                    if (reader.GetAttribute("themePath") != null)
                                        themePath = reader.GetAttribute("themePath");

                                    // Loading file extension
                                    string extension = ".pdf";
                                    if (reader.GetAttribute("extension") != null)
                                        extension = reader.GetAttribute("extension");

                                    // Adding plugin configuration
                                    PluginInformation info = new PluginInformation(filePath, longevity, deleteExpired, resultType, themePath, name, extension);
                                    plugins.Add((PluginType)Enum.Parse(typeof(PluginType), reader.GetAttribute("enumId")), info);
                                    #endregion
                                }
                                catch
                                {
                                    // The plugin entrie is not valid and won't be
                                    // added to the plugin list
                                }
                                break;

                        }
                    }
                }
            }
            else
                // The configuration source is not defined.
                // Impossible to load the configuration
                throw new ArgumentNullException("nyxConfiguration");
            return (isActivated);
        }
    }
}
