using System;
using System.Collections.Generic;
using System.Text;

/* TNS */
using TNS.Ares.Domain.LS;
using System.Xml;
using TNS.Ares.Domain.Layers;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;

namespace TNS.Ares.Domain.XmlLoader
{
    public class PluginConfigurationXL
    {
        /// <summary>
        /// Load the configuration from an XmlTextReader
        /// </summary>
        /// <param name="nyxConfiguration">Configuration source</param>
        /// <param name="defaultFilePath">Out parameter where default file path should be stored</param>
        /// <param name="defaultLongevity">Out parameter where the default longevity should be stored</param>
        /// <param name="dataAccessLayers">DataAccessLayer dictionary</param>
        /// <param name="plugins">Plugin dictionary</param>
        public static void Load(IDataSource dataSource, out string defaultFilePath, out string defaultVirtualPath, 
                                out int defaultLongevity, out string defaultThemePath,
                                out Dictionary<PluginDataAccessLayerName, DataAccessLayer> dataAccessLayers,
                                out Dictionary<PluginType, PluginInformation> plugins,
                                out DefaultConnectionIds defaultConnectionId, out int defaultFamilyId) {

            // Default values
            defaultFilePath = "";
            defaultVirtualPath = "";
            defaultThemePath = "";
            defaultLongevity = 30;
            dataAccessLayers = new Dictionary<PluginDataAccessLayerName, DataAccessLayer>();
            plugins = new Dictionary<PluginType, PluginInformation>();
            defaultConnectionId = DefaultConnectionIds.music;
            defaultFamilyId = -1;

            // Checking if configuration source is defined
            try {
                dataSource.Open();
                XmlTextReader reader = (XmlTextReader)dataSource.GetSource();
                XmlReader subReader = null;
                // Reading configuration
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "pluginConfiguration":
                                if (reader.GetAttribute("defaultConnectionId") != null)
                                    defaultConnectionId = (DefaultConnectionIds)Enum.Parse(typeof(DefaultConnectionIds), reader.GetAttribute("defaultConnectionId"));
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
                                    dataAccessLayers.Add((PluginDataAccessLayerName)Enum.Parse(typeof(PluginDataAccessLayerName), layerName),
                                                              new DataAccessLayer(layerName, layerAsssembly, layerClass));
                                break;
                            case "plugins":
                                // Reading attributes and setting application
                                // default values
                                if (reader.GetAttribute("defaultLongevity") != null)
                                    defaultLongevity = int.Parse(reader.GetAttribute("defaultLongevity"));
                                if (reader.GetAttribute("defaultFilePath") != null)
                                    defaultFilePath = reader.GetAttribute("defaultFilePath");
                                if (reader.GetAttribute("defaultVirtualPath") != null)
                                    defaultVirtualPath = reader.GetAttribute("defaultVirtualPath");
                                if (reader.GetAttribute("defaultThemePath") != null)
                                    defaultThemePath = reader.GetAttribute("defaultThemePath");
                                if (reader.GetAttribute("defaultFamilyId") != null)
                                    defaultFamilyId = Int32.Parse(reader.GetAttribute("defaultFamilyId"));
                                
                                break;
                            case "plugin":

                                // Reading plugin information
                                try {
                                    #region Loading plugin attributes
                                    int resultType = int.Parse(reader.GetAttribute("resultType"));
                                    string name = reader.GetAttribute("name");

                                    // Loading plugin file path attribute
                                    string filePath = defaultFilePath;
                                    if (reader.GetAttribute("filePath") != null)
                                        filePath = reader.GetAttribute("filePath");

                                    // Loading plugin file path attribute
                                    string virtualPath = defaultVirtualPath;
                                    if (reader.GetAttribute("virtualPath") != null)
                                        virtualPath = reader.GetAttribute("virtualPath");

                                    // Loading longevity attribute
                                    int longevity = defaultLongevity;
                                    if (reader.GetAttribute("longevity") != null)
                                        longevity = int.Parse(reader.GetAttribute("longevity"));

                                    // Loading default Family Id attribute
                                    int familyId = defaultFamilyId;
                                    if (reader.GetAttribute("familyId") != null)
                                        familyId = int.Parse(reader.GetAttribute("familyId"));
                                    

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

                                    //Loading path file configuration
                                    string pathFileConfiguration = string.Empty;
                                    if (reader.GetAttribute("pathFileConfiguration") != null && reader.GetAttribute("pathFileConfiguration").Length > 0)
                                        pathFileConfiguration = reader.GetAttribute("pathFileConfiguration");

                                    // Loading use Exec
                                    bool exec = false;
                                    if (reader.GetAttribute("useExec") != null)
                                        bool.TryParse(reader.GetAttribute("useExec"), out exec);

                                    bool deleteRowSuccess = false;
                                    if (reader.GetAttribute("deleteRowSuccess") != null)
                                        bool.TryParse(reader.GetAttribute("deleteRowSuccess"), out deleteRowSuccess);

                                    // Loading assembly name
                                    string assemblyname = "";
                                    if (reader.GetAttribute("assemblyName") != null)
                                        assemblyname = reader.GetAttribute("assemblyName");

                                    // Loading assembly class
                                    string assemblynameClass = "";
                                    if (reader.GetAttribute("class") != null)
                                        assemblynameClass = reader.GetAttribute("class");

                                    // Adding plugin configuration
                                    PluginType pluginType = (PluginType)Enum.Parse(typeof(PluginType), reader.GetAttribute("enumId"));
                                    PluginInformation info = null;

                                    Dictionary<DayOfWeek, PluginExec> pluginExecList = new Dictionary<DayOfWeek, PluginExec>();
                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "exec":
                                                    DayOfWeek dayOfWeekFrom = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),subReader.GetAttribute("dayOfWeekFrom"));
                                                    DayOfWeek dayOfWeekTo = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),subReader.GetAttribute("dayOfWeekTo"));
                                                    TimeSpan dayOfWeekHourTo = new TimeSpan(Int32.Parse(subReader.GetAttribute("dayOfWeekHourTo").Split(':')[0]),Int32.Parse(subReader.GetAttribute("dayOfWeekHourTo").Split(':')[1]),Int32.Parse(subReader.GetAttribute("dayOfWeekHourTo").Split(':')[2]));
                                                    pluginExecList.Add(dayOfWeekFrom, new PluginExec(dayOfWeekFrom, dayOfWeekTo, dayOfWeekHourTo));
                                                    break;
                                            }
                                        }
                                    }
                                    info = new PluginInformation(filePath, virtualPath, longevity, resultType, themePath, pluginType, familyId, exec, pluginExecList, deleteRowSuccess, deleteExpired, name, extension, pathFileConfiguration,assemblynameClass,assemblyname);
                                    plugins.Add(pluginType, info);
                                    #endregion
                                }
                                catch {
                                    // The plugin entrie is not valid and won't be
                                    // added to the plugin list
                                }
                                break;

                        }
                    }
                }
            }
            catch(System.Exception err) {

                #region Close the file
                if (dataSource.GetSource() != null) dataSource.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }

            dataSource.Close();
        }
    }
}
