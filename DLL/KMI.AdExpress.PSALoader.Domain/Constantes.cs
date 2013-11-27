using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.PSALoader.Domain {
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
            ///Database configuration file name
            /// </summary>
            public const string DATABASE_CONFIGURATION_FILE = "Database.xml";
            /// <summary>
            ///Data File Name
            /// </summary>
            public const string DATA_FILE_DIRECTORY = @"\\frmitch-fs04\PromoPsa\";
            /// <summary>
            ///Visual Informations File Name
            /// </summary>
            public const string VISUAL_INFORMATION = "VisualInformations.xml";
            /// <summary>
            /// PSA Loader Configuration File Name
            /// </summary>
            public const string PSA_LOADER_CONFIGURATION = "PSALoaderConfiguration.xml";
        }
        #endregion

        #region Vehicles
        /// <summary>
        /// Media type List
        /// </summary>
        public class Vehicles {
            /// <summary>
            /// Media type
            /// </summary>
            public enum names {
                /// <summary>
                /// No Media
                /// </summary>
                DEFAULT = 0,
                /// <summary>
                /// Press
                /// </summary>
                PRESSE = 1,
                /// <summary>
                /// Radio
                /// </summary>
                RADIO = 2,
                /// <summary>
                /// Television
                /// </summary>
                TELEVISION = 3,
                /// <summary>
                /// Media tactique
                /// </summary>
                MEDIASTACTICS = 4,
                /// <summary>
                /// Others media types
                /// </summary>
                OTHERS = 5,
                /// <summary>
                /// AdNetTrack
                /// </summary>
                ADNETTRACK = 6,
                /// <summary>
                /// Internet
                /// </summary>
                INTERNET = 7,
                /// <summary>
                /// Outdoor
                /// </summary>
                PUBLICITE_EXTERIEURE = 8,
                /// <summary>
                /// COURRIER CREATIONS
                /// </summary>
                COURRIER_CREATIONS = 10,
                /// <summary>
                /// Evaliant mobilr
                /// </summary>
                EVALIANT_MOBILE = 15
            }
        }
        #endregion

    }
}
