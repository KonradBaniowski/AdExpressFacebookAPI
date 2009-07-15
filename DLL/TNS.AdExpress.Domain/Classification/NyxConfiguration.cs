using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Classification
{
    /// <summary>
    /// Data access layer names
    /// </summary>
    public enum NyxDataAccessLayer
    {
        Alert = 1,
        Session = 2
    }

    /// <summary>
    /// Nyx configuration class
    /// </summary>
    public class NyxConfiguration
    {
        #region Variables
        private static Dictionary<NyxDataAccessLayer, DataAccessLayer> _dataAccessLayers = null;
        private static Dictionary<PluginType, PluginInformation> _plugins = null;
        private static string _defaultFilePath = "";
        private static string _defaultThemePath = "";
        private static int _defaultLongevity = -1;
        private static bool _isActivated = false;
        private static string _targetHost = "";
        #endregion

        #region Methods

        /// <summary>
        /// Returns a DataAccessLayer corresponding to the
        /// given parameter. If no layer matches, it will return
        /// null
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <returns>The DataAccessLayer object or null</returns>
        public static DataAccessLayer GetDataAccessLayer(NyxDataAccessLayer layer)
        {
            if (_dataAccessLayers.ContainsKey(layer))
                return (_dataAccessLayers[layer]);
            return (null);
        }

        // Gets a copy of the plugin list
        public static Dictionary<PluginType, PluginInformation> Plugins
        {
            get { return (_plugins); }
        }

        /// <summary>
        /// Returns a PluginInformation corresponding to the
        /// given parameter. If no plugin matches, it will return
        /// null
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>The PluginInformation object or null</returns>
        public static PluginInformation GetPluginInformation(PluginType plugin)
        {
            if (_plugins.ContainsKey(plugin))
                return (_plugins[plugin]);
            return (null);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets target host, used for creating links in emails
        /// </summary>
        public static string TargetHost
        {
            get { return (_targetHost); }
        }

        /// <summary>
        /// Gets the default file path, where file will be stored
        /// </summary>
        public static string DefaultFilePath
        {
            get { return (_defaultFilePath); }
        }

        /// <summary>
        /// Specifies whether the alert creation is enabled
        /// or not
        /// </summary>
        public static bool IsAlertsActivated
        {
            get { return (_isActivated); }
        }

        /// <summary>
        /// Returns the default result longevity, which defines how
        /// long a customer request should be kept
        /// </summary>
        public static int DefaultLongevity
        {
            get { return (_defaultLongevity); }
        }
        #endregion

        #region Load
        /// <summary>
        /// Load configuration from the given datasource
        /// </summary>
        /// <param name="source">Datasource containing Nyx configuration</param>
        public static void Load(IDataSource source)
        {
            _isActivated = NyxConfigurationXL.Load(source, out _defaultFilePath, out _defaultLongevity, out _defaultThemePath, out _dataAccessLayers, out _plugins, out _targetHost);
        }
        #endregion
    }
}
