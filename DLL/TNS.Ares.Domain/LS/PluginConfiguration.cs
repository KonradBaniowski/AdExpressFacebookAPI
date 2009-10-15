using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;

namespace TNS.Ares.Domain.LS
{
    /// <summary>
    /// Data access layer names
    /// </summary>
    public enum PluginDataAccessLayerName
    {
        Alert = 1,
        Session = 2
    }

    /// <summary>
    /// Nyx configuration class
    /// </summary>
    public class PluginConfiguration
    {
        #region Variables
        /// <summary>
        /// Data access Layer
        /// </summary>
        private static Dictionary<PluginDataAccessLayerName, DataAccessLayer> _dataAccessLayers = null;
        /// <summary>
        /// Plugin information Name List
        /// </summary>
        private static Dictionary<PluginType, PluginInformation> _pluginsInformationName = null;
        /// <summary>
        /// Plugins Information Id
        /// </summary>
        private static Dictionary<int, PluginInformation> _pluginsInformationId = new Dictionary<int, PluginInformation>();
        /// <summary>
        /// Default File Path
        /// </summary>
        private static string _defaultFilePath = "";
        /// <summary>
        /// Default Virtual Path
        /// </summary>
        private static string _defaultVirtualPath = "";
        /// <summary>
        /// Default theme Path
        /// </summary>
        private static string _defaultThemePath = "";
        /// <summary>
        /// default Longevity
        /// </summary>
        private static int _defaultLongevity = -1;
        /// <summary>
        /// Default Connection Id
        /// </summary>
        private static DefaultConnectionIds _defaultConnectionId;
        /// <summary>
        /// Default Family Id
        /// </summary>
        private static int _defaultFamilyId = -1;
        #endregion

        #region Methods

        /// <summary>
        /// Returns a DataAccessLayer corresponding to the
        /// given parameter. If no layer matches, it will return
        /// null
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <returns>The DataAccessLayer object or null</returns>
        public static DataAccessLayer GetDataAccessLayer(PluginDataAccessLayerName layer) {
            if (_dataAccessLayers.ContainsKey(layer))
                return (_dataAccessLayers[layer]);
            return (null);
        }

        // Gets a copy of the plugin list
        public static Dictionary<PluginType, PluginInformation> PluginsName {
            get { return (_pluginsInformationName); }
        }

        /// <summary>
        /// Returns a PluginInformation corresponding to the
        /// given parameter. If no plugin matches, it will return
        /// null
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>The PluginInformation object or null</returns>
        public static PluginInformation GetPluginInformation(PluginType plugin) {
            if (_pluginsInformationName.ContainsKey(plugin))
                return (_pluginsInformationName[plugin]);
            return (null);
        }
        /// <summary>
        /// Returns a PluginInformation corresponding to the
        /// given parameter. If no plugin matches, it will return
        /// null
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>The PluginInformation object or null</returns>
        public static PluginInformation GetPluginInformation(int resultType) {
            if (_pluginsInformationId.ContainsKey(resultType))
                return (_pluginsInformationId[resultType]);
            return (null);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets Default Connection Id
        /// </summary>
        public static DefaultConnectionIds DefaultConnectionId {
            get { return (_defaultConnectionId); }
        }

        /// <summary>
        /// Gets the default file path, where file will be stored
        /// </summary>
        public static string DefaultFilePath {
            get { return (_defaultFilePath); }
        }

        /// <summary>
        /// Gets the default file path, where file will be stored
        /// </summary>
        public static string DefaultVirtualPath {
            get { return (_defaultVirtualPath); }
        }

        /// <summary>
        /// Returns the default result longevity, which defines how
        /// long a customer request should be kept
        /// </summary>
        public static int DefaultLongevity {
            get { return (_defaultLongevity); }
        }

        /// <summary>
        /// Returns the default Family Id
        /// </summary>
        public static int DefaultFamilyId {
            get { return (_defaultFamilyId); }
        }
        #endregion

        #region Load
        /// <summary>
        /// Load configuration from the given datasource
        /// </summary>
        /// <param name="source">Datasource containing Nyx configuration</param>
        public static void Load(IDataSource source) {
            _pluginsInformationId.Clear();
            PluginConfigurationXL.Load(source, out _defaultFilePath, out _defaultVirtualPath, out _defaultLongevity, out _defaultThemePath, out _dataAccessLayers, out _pluginsInformationName, out _defaultConnectionId, out _defaultFamilyId);
            foreach (PluginInformation currentPluginInformation in _pluginsInformationName.Values) {
                _pluginsInformationId.Add(currentPluginInformation.ResultType, currentPluginInformation);
            }
        }
        #endregion
    }
}
