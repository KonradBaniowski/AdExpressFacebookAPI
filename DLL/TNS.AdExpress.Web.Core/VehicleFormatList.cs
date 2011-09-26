using System;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Web.Core.DataAccess;

namespace TNS.AdExpress.Web.Core {
    /// <summary>
    /// Class used to load the active banners format presented in the database tabels correponding to Internet Evaliant and Mobile Evaliant
    /// </summary>
    public class VehicleFormatList {

        #region Variables
        /// <summary>
        /// Vehicle Identifier
        /// </summary>
        protected Int64 _vehicleId;
        /// <summary>
        /// List of banner format items
        /// Key: Format Id
        /// Value: Format List 
        /// </summary>
        private Dictionary<Int64, FilterItem> _formatList;
        #endregion

        #region Constructor
		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="vehicleId">Vehicle Identifier</param>
		/// <param name="formatList">
        /// List of banner format items
        /// Key: Format Id
        /// Value: Format List 
		/// </param>
        public VehicleFormatList(Int64 vehicleId, Dictionary<Int64, FilterItem> formatList)
		{
            _vehicleId = vehicleId;
            _formatList = formatList;
		}
		#endregion

        #region Assessor

        /// <summary>
        /// Get Vehicle Identifier
        /// </summary>
        public Int64 VehicleId
        {
            get { return _vehicleId; }
        }

        /// <summary>
        /// Get List of banner format items
        /// Key: Format Id
        /// Value: Format List 
        /// </summary>
        public Dictionary<Int64, FilterItem> FormatList
        {
            get { return _formatList; }
        }
        #endregion
    }
}
