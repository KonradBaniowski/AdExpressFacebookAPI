using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Web.Core.DataAccess;

namespace TNS.AdExpress.Web.Core {
    /// <summary>
    /// Class used to load the active banners format presented in the database tabels correponding to Internet Evaliant and Mobile Evaliant
    /// </summary>
    public class ActiveBannersFormatList {

        #region Variables
        /// <summary>
        /// List of the active banners format
        /// the key corresponds to the vehicle id
        /// the value corresponds to the list of banner format items
        /// </summary>
        private static Dictionary<Int64, List<FilterItem>> _list;
        /// <summary>
        /// Vehicles list having banners format
        /// </summary>
        private static List<VehicleInformation> _vehicleList;
        #endregion

        #region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        static ActiveBannersFormatList(){
            _list = new Dictionary<Int64, List<FilterItem>>();
            _vehicleList = new List<VehicleInformation>();
            _vehicleList.Add(VehiclesInformation.Get(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack));
            _vehicleList.Add(VehiclesInformation.Get(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile));
		}
		#endregion

        #region Init
        /// <summary>
        /// Init the active banners format list
        /// </summary>
        public static void Init(int siteLanguage) {

            DataSet ds;
            DataTable dt;
            List<FilterItem> filterItemsList;

            foreach (VehicleInformation vehicle in _vehicleList) {

                filterItemsList = new List<FilterItem>();

                ds = ActiveBannersFormatListDataAccess.GetActiveBannersFormatData(vehicle.DatabaseId, siteLanguage);

                if (ds != null && ds.Tables.Count > 0) {
                    dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        filterItemsList.Add(new FilterItem(Convert.ToInt64(row["id_format_banners"].ToString()), row["format_banners"].ToString()));
                    }

                    _list.Add(vehicle.DatabaseId, filterItemsList);
                }
            }
        }
        #endregion

        #region Get Active Banners Format List
        /// <summary>
        /// Method used to get the list of active banners format
        /// </summary>
        /// <returns>The list of active banners format</returns>
        public static List<FilterItem> GetActiveBannersFormatList(Int64 vehicleId) {

            if (_list != null && _list[vehicleId] != null)
                return _list[vehicleId];
            else
                return null;

        }
        #endregion

    }
}
