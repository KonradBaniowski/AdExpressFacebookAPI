using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.Aphrodite.Constantes {
    
    public class Application {
        /// <summary>
        /// Application configuration directory
        /// </summary>
        public const string APPLICATION_CONFIGURATION_DIRECTORY="Configuration";
        /// <summary>
        /// Application configuration file name
        /// </summary>
        public const string APPLICATION_CONFIGURATION_FILE="ApplicationParameters.xml";
        /// <summary>
        /// Media types configuration file name
        /// </summary>
        public const string MEDIA_TYPES_CONFIGURATION_FILE="MediaTypes.xml";
        /// <summary>
        ///Database configuration file name
        /// </summary>
        public const string DATABASE_CONFIGURATION_FILE="Database.xml";
        /// <summary>
        /// DAL configuration file name
        /// </summary>
        public const string DAL_CONFIGURATION_FILE = "DataAccessLayer.xml";

        #region Treatment Type
        /// <summary>
        /// Treatment Type for data to load : week or month
        /// </summary>
        public enum TreatmentType {
            /// <summary>
            /// Comparative Date Week
            /// </summary>
            comparativeDateWeek = 0,
            /// <summary>
            /// Date To Date Week
            /// </summary>
            dateToDateWeek = 1,
            /// <summary>
            /// Month
            /// </summary>
            month = 2
        }
        #endregion

    }
}
