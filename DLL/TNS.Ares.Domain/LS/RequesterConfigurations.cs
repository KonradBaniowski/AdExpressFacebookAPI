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
    /// Ares configuration class
    /// </summary>
    public class RequesterConfigurations
    {
        #region Variables
        private static Dictionary<LsClientName, RequesterConfiguration> _aresConfigurationList = null;
        #endregion

        #region Methods
        /// <summary>
        /// Gets AresConfiguration
        /// </summary>
        public static RequesterConfiguration GetAresConfiguration(LsClientName requesterName) {
            if (_aresConfigurationList != null) {
                if (_aresConfigurationList.ContainsKey(requesterName)) {
                    return _aresConfigurationList[requesterName];
                }
                else {
                    throw new ArgumentException("AresConfigurations hasen't family Id '" + requesterName + "'");
                }
            }
            else {
                throw new Exception("AresConfigurations has don't initialize");
            }
        }
        #endregion

        #region Load
        /// <summary>
        /// Load configuration from the given datasource
        /// </summary>
        /// <param name="source">Datasource containing Nyx configuration</param>
        public static void Load(IDataSource source) {
            _aresConfigurationList = RequesterConfigurationXL.Load(source);
        }
        #endregion
    }
}
