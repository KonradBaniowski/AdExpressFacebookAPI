#region Information
//  Author : Y. R'kaina
//  Creation  date: 06/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.WebService.Domain.Configuration {
    /// <summary>
    /// Vehicle description
    /// </summary>
    public class VehicleCreativesInformation {

        #region Variables
        /// <summary>
        /// Data base id
        /// </summary>
        private Int64 _databaseId;
        /// <summary>
        /// Creative Information
        /// </summary>
        private CreativeInformation _creativeInformation = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseId">Data base id</param>
        /// <param name="creativeInformation">Creative Information</param>
        public VehicleCreativesInformation(Int64 databaseId, CreativeInformation creativeInformation) {
            _databaseId = databaseId;
            _creativeInformation = creativeInformation;
        }	
        #endregion

        #region Accessors
        /// <summary>
        /// Get database id
        /// </summary>
        public Int64 DatabaseId {
            get { return _databaseId; }
        }
        /// <summary>
        /// Get Creative Information
        /// </summary>
        public CreativeInformation CreativeInfo {
            get { return _creativeInformation; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Open Impersonation
        /// </summary>
        public void Open() {
            _creativeInformation.Open();
        }
        /// <summary>
        /// Close Impersonation
        /// </summary>
        public void Close(){
            _creativeInformation.Close();
        }
        #endregion

    }
}
