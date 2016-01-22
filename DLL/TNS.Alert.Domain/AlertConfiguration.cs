using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.Alert.Domain.XmlLoader;

namespace TNS.Alert.Domain {

    public class AlertConfiguration {

        #region Variables
        /// <summary>
        /// Alerte Alert Configuration
        /// </summary>
        private static AlertInformation _alertInformation = null;
        /// <summary>
        /// Alerte Occurrence Configuration
        /// </summary>
        private static OccurrenceInformation _occurrenceInformation = null;
        /// <summary>
        /// Alerte Mail Configuration
        /// </summary>
        private static MailInformation _mailInformation = null;
        /// <summary>
        /// Alerte Activated or not
        /// </summary>
        private static bool _isActivated = false;
        #endregion
       
        #region Assessor
        /// <summary>
        /// Get Alert Configuration
        /// </summary>
        public static AlertInformation AlertInformation {
            get { return (_alertInformation); }
        }
        /// <summary>
        /// Get Occurrence Configuration
        /// </summary>
        public static OccurrenceInformation OccurrenceInformation {
            get { return (_occurrenceInformation); }
        }
        /// <summary>
        /// Get Alerte Mail Configuration
        /// </summary>
        public static MailInformation MailInformation {
            get { return (_mailInformation); }
        }
        /// <summary>
        /// Get Alerte Activated or not
        /// </summary>
        public static bool IsActivated {
            get { return (_isActivated); }
        }
        #endregion

        #region Load
        /// <summary>
        /// Load configuration from the given datasource
        /// </summary>
        /// <param name="source">Datasource containing Nyx configuration</param>
        public static void Load(IDataSource source) {
            _occurrenceInformation = null;
            _mailInformation = null;
            _isActivated = false;
            AlertConfigurationXL.Load(source,out _alertInformation, out _occurrenceInformation, out _mailInformation, out _isActivated);
        }
        #endregion

    }
}
