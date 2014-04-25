using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Database Information
    /// </summary>
    public class DataBaseInformation {

        #region Variables
        /// <summary>
        /// Connection String
        /// </summary>
        private static string _connectionString = string.Empty;
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source">XML File</param>
        public static void Init(IDataSource source) {
            _connectionString = DataBaseInformationXL.Load(source);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Connection String
        /// </summary>
        public static string ConnectionString {
            get { return _connectionString; }
        }
        #endregion


    }
}
