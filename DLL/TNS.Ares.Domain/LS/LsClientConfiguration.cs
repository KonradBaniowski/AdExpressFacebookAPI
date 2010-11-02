using System;
using System.Collections.Generic;
using System.Text;
using TNS.LinkSystem.LinkKernel;

namespace TNS.Ares.Domain.LS {
    /// <summary>
    /// Represents all the properties necessary to configure an LS client
    /// </summary>
    public class LsClientConfiguration {

        #region Variables
        /// <summary>
        /// Id used to assemble a group of modules tasks
        /// </summary>
        private int _familyId;
        /// <summary>
        /// Name used to assemble a group of modules tasks
        /// </summary>
        private string _familyName = string.Empty;
        /// <summary>
        /// Port used by the link watcher to monitor the clients
        /// </summary>
        private int _monitorPort;
        /// <summary>
        /// Name that appears in the link console
        /// </summary>
        private string _productName = string.Empty;
        /// <summary>
        /// The list of modules that the client can treat
        /// </summary>
        private List<ModuleDescription> _moduleDescription = null;
        /// <summary>
        /// Used to locate the configuration files
        /// </summary>
        private string _directoryName = "";
        /// <summary>
        /// Max available slots
        /// </summary>
        private int _maxAvailableSlots;
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
        /// <param name="moduleDescriptionList">The list of modules that the client can treat</param>
        /// <param name="maxAvailableSlots">Max available slots</param>
        public LsClientConfiguration(int familyId, string familyName, int monitorPort, string productName, string directoryName, List<ModuleDescription> moduleDescriptionList, int maxAvailableSlots) {
            _familyId = familyId;
            _familyName = familyName;
            _monitorPort = monitorPort;
            _productName = productName;
            _directoryName = directoryName;
            _moduleDescription = moduleDescriptionList;
            _maxAvailableSlots = maxAvailableSlots;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Family Id (used to assemble a group of modules tasks)
        /// </summary>
        public Int32 FamilyId {
            get { return (_familyId); }
        }
        /// <summary>
        /// Get Name used to assemble a group of modules tasks
        /// </summary>
        public string FamilyName {
            get { return (_familyName); }
        }
        /// <summary>
        /// Gets Monitor Port (used by the link watcher to monitor the clients)
        /// </summary>
        public Int32 MonitorPort {
            get { return (_monitorPort); }
        }
        /// <summary>
        /// Gets Product Name (Name that appears in the link console)
        /// </summary>
        public string ProductName {
            get { return (_productName); }
        }
        /// <summary>
        /// Gets Module Description List (The list of modules that the client can treat)
        /// </summary>
        public List<ModuleDescription> ModuleDescriptionList {
            get { return (_moduleDescription); }
        }
        /// <summary>
        /// Gets Directory Name (Used to locate the configuration files)
        /// </summary>
        public string DirectoryName {
            get { return (_directoryName); }
        }
        /// <summary>
        /// Gets Max available slots
        /// </summary>
        public Int32 MaxAvailableSlots {
            get { return (_maxAvailableSlots); }
        }
        #endregion

    }
}
