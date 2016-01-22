using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// This class contains constantes for the service
    /// </summary>
    public class Constantes {

        #region Application
        public class Application {
            /// <summary>
            /// Application configuration directory
            /// </summary>
            public const string APPLICATION_CONFIGURATION_DIRECTORY = "Configuration";
            /// <summary>
            /// Media Configuration File Name
            /// </summary>
            public const string MEDIA_CONFIGURATION = "MediaInformations.xml";
            /// <summary>
            ///Database configuration file name
            /// </summary>
            public const string DATABASE_CONFIGURATION_FILE = "Database.xml";
            /// <summary>
            ///Filter configuration file name
            /// </summary>
            public const string FILTER_CONFIGURATION_FILE = "FilterInformations.xml";
        }
        #endregion

        #region Top Diffusion
        /// <summary>
        /// Top Diffusion Slots
        /// </summary>
        public enum TopDiffusion { 
            _03H_04H,
            _04H_05H,
            _05H_06H,
            _06H_07H,
            _07H_08H,
            _08H_09H,
            _09H_10H,
            _10H_11H,
            _11H_12H,
            _12H_13H,
            _13H_14H,
            _14H_15H,
            _15H_16H,
            _16H_17H,
            _17H_18H,
            _18H_19H,
            _19H_20H,
            _20H_21H,
            _21H_22H,
            _22H_23H,
            _23H_24H,
            _24H_01H,
            _01H_02H,
            _02H_03H
        }
        #endregion

        #region ClassificationLevel
        public enum ClassificationLevelId { 
            Advertiser,
            Product,
            Version
        }
        #endregion

    }
}
