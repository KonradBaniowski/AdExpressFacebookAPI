using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.Alert.Domain.Parameters
{
    public class GarbageParameters
    {
        #region Variables

        private DataAccessLayer _dataAccessLayer;
        private DataBase _database;
        private string _filePath;
        private Dictionary<int, int> _pluginList;
        private int _defaultLongevity = 30;

        #endregion

        #region Properties

        public DataAccessLayer DataAccessLayer
        {
            get { return (this._dataAccessLayer); }
        }

        public IDataSource DataSource
        {
            get { return (this._database.GetDefaultConnection(DefaultConnectionIds.webAdministration)); }
        }

        public IDataSource AlertDataSource
        {
            get { return (this._database.GetDefaultConnection(DefaultConnectionIds.alert)); }
        }

        public string FilePath
        {
            get { return (this._filePath); }
        }

        public Dictionary<int, int> Plugins
        {
            get { return (this._pluginList); }
        }

        #endregion


        public GarbageParameters(IDataSource generalConfiguration)
        {
            if (generalConfiguration != null)
            {
                generalConfiguration.Open();
                XmlTextReader reader = (XmlTextReader)generalConfiguration.GetSource();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case "DataAccessLayer":
                                string layerClass = reader.GetAttribute("class");
                                string layerName = reader.GetAttribute("name");
                                string layerAsssembly = reader.GetAttribute("assemblyName");

                                if (layerName != null && layerAsssembly != null && layerClass != null &&
                                    layerName.Length > 0 && layerAsssembly.Length > 0 && layerClass.Length > 0)
                                    this._dataAccessLayer = new DataAccessLayer(layerName, layerAsssembly, layerClass);
                                break;
                            case "FilePath":
                                this._filePath = reader.GetAttribute("path");
                                break;
                            case "plugins":
                                this._pluginList = new Dictionary<int, int>();
                                break;
                            case "plugin":
                                // A null should be impossible, as the "plugins"
                                // element should come before and initialize
                                // the dictionary, but... who knows?
                                if (this._pluginList == null)
                                    this._pluginList = new Dictionary<int,int>();

                                // Setting record type longevity
                                int longevity = this._defaultLongevity;
                                if (reader.GetAttribute("resultLongevity") != null)
                                    int.TryParse(reader.GetAttribute("resultLongevity"), out longevity);

                                try
                                {
                                    int resultType = int.Parse(reader.GetAttribute("resultType"));
                                    this._pluginList.Add(resultType, longevity);
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

                // Loading database informations
                this._database = new DataBase(generalConfiguration);
            }
            else
                throw new ArgumentNullException("src");
        }
    }
}
