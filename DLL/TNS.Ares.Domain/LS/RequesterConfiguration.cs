using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.LinkSystem.LinkKernel;

namespace TNS.Ares.Domain.LS
{
    public enum LsClientName {
        AdExpress,
        EasyMusic,
        PixPalace,
        PixPalaceRequester,
        WcbRequester
    }

    /// <summary>
    /// Ares configuration class
    /// </summary>
    public class RequesterConfiguration : LsClientConfiguration {
        
        #region Variables
        /// <summary>
        /// Client name
        /// </summary>
        private LsClientName _name;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="familyId">Id used to assemble a group of modules tasks</param>
        /// <param name="familyName">Name used to assemble a group of modules tasks</param>
        /// <param name="monitorPort">Port used by the link watcher to monitor the clients</param>
        /// <param name="productName">Name that appears in the link console</param>
        /// <param name="directoryName">Used to locate the configuration files</param>
        /// <param name="name">Client name</param>
        /// <param name="moduleDescriptionList">The list of modules that the client can treat</param>
        /// <param name="maxAvailableSlots">Max available slots</param>
        public RequesterConfiguration(int familyId, string familyName, int monitorPort, string productName, string directoryName, LsClientName name, List<ModuleDescription> moduleDescriptionList, int maxAvailableSlots)
            : base(familyId, familyName, monitorPort, productName, directoryName, moduleDescriptionList, maxAvailableSlots) {
            _name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Client Name
        /// </summary>
        public LsClientName Name {
            get { return (_name); }
        }
        #endregion
    }
}
