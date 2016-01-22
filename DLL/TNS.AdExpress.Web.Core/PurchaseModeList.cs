using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.Selection;

namespace TNS.AdExpress.Web.Core {
    /// <summary>
    /// Purchase Mode List
    /// </summary>
    public class PurchaseModeList {

        #region Variables
        /// <summary>
        /// Purchase Mode Items
        /// </summary>
        private static Dictionary<Int64, FilterItem> _list;
        #endregion

        #region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        static PurchaseModeList() {
            _list = new Dictionary<Int64, FilterItem>();
		}
		#endregion

        #region Init
        /// <summary>
        /// Init the active banners format list
        /// </summary>
        public static void Init() {

            var filterItemsList = new Dictionary<Int64, FilterItem>();
            DataSet ds;
            ds = TNS.AdExpress.Web.Core.DataAccess.PurchaseModeListDataAccess.GetData();

            if (ds != null && ds.Tables.Count > 0) {
                foreach (DataRow row in ds.Tables[0].Rows) {
                    _list.Add(Convert.ToInt64(row[0].ToString()), new FilterItem(Convert.ToInt64(row[0].ToString()), row[1].ToString()));
                }
            }
          
        }
        #endregion

        #region Get List
        /// <summary>
        /// Method used to get the list of banners format
        /// </summary>
        /// <returns>The list of purchase mode</returns>
        public static Dictionary<Int64, FilterItem> GetList() {
            return _list;
        }
        #endregion

    }
}
