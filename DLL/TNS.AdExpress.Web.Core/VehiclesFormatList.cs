using System;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Web.Core.DataAccess;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core {
    /// <summary>
    /// Class used to load the active banners format presented in the database tabels correponding to Internet Evaliant and Mobile Evaliant
    /// </summary>
    public class VehiclesFormatList {

        #region Variables
        /// <summary>
        /// List of the active banners format
        /// the key corresponds to the vehicle id
        /// the value corresponds to the list of banner format items
        /// </summary>
        private static Dictionary<Int64, VehicleFormatList> _list;
        #endregion

        #region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        static VehiclesFormatList(){
            _list = new Dictionary<Int64, VehicleFormatList>();
		}
		#endregion

        #region Init
        /// <summary>
        /// Init the active banners format list
        /// </summary>
        public static void Init(Dictionary<Int64, VehicleFormatInformation> vehicleFormatInformationList, int defaultDataLanguage) {

            if (vehicleFormatInformationList != null)
            {
                foreach (VehicleFormatInformation vehicleFormatInformation in vehicleFormatInformationList.Values)
                {

                    _list.Add(vehicleFormatInformation.VehicleId
                              , new VehicleFormatList(
                                    vehicleFormatInformation.VehicleId
                                    , GetFormatBannerList(vehicleFormatInformation.DataTableName
                                                          , vehicleFormatInformation.FormatTableName
                                                          , defaultDataLanguage
                                          )
                                    )
                        );
                }
            }
        }
        #endregion

        #region Get List
        /// <summary>
        /// Method used to get the list of banners format
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier</param>
        /// <returns>The list of banners format</returns>
        protected static Dictionary<Int64, FilterItem> GetList(Int64 vehicleId)
        {
            var formatList = new Dictionary<Int64, FilterItem>();
            if (_list.Count > 0 && _list.ContainsKey(vehicleId)
                && _list[vehicleId].FormatList.Count > 0)
            {
                
                foreach (var cFilterItem in _list[vehicleId].FormatList.Values)
                {
                    formatList.Add(cFilterItem.Id
                        , new FilterItem(
                            cFilterItem.Id
                            , cFilterItem.Label
                            , true
                            )
                        );
                }
            }

            return formatList;
        }

        /// <summary>
        /// Method used to get the list of banners format
        /// </summary>
        /// <param name="vehicleIdList">Vehicle identifier List</param>
        /// <returns>The list of banners format</returns>
        public static Dictionary<Int64, FilterItem> GetList(List<Int64> vehicleIdList) {
            var filterItems = new Dictionary<Int64, FilterItem>();
            bool first = true;
            foreach (var cVehicleId in vehicleIdList) {
                if (_list.ContainsKey(cVehicleId)) {
                    if (first) {
                        filterItems = GetList(cVehicleId);
                        first = false;
                    }
                    else {
                        var cFilterItems = GetList(cVehicleId);

                        foreach (var cFilterItem in new List<FilterItem>(filterItems.Values)) {
                            if (!cFilterItems.ContainsKey(cFilterItem.Id))
                                filterItems.Remove(cFilterItem.Id);
                        }
                    }
                }
            }
            return filterItems;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get Format Banner List
        /// </summary>
        /// <param name="formatTable">Format Table</param>
        /// <param name="dataLanguageId">Data Language Identifier</param>
        /// <param name="dataTable">Data Table</param>
        /// <returns>Format Banner List</returns>
        private static Dictionary<Int64, FilterItem> GetFormatBannerList(TableIds dataTable, TableIds formatTable, int dataLanguageId)
        {
            var filterItemsList = new Dictionary<Int64, FilterItem>();
            var ds = VehicleFormatListDataAccess.GetActiveData(dataTable, formatTable, dataLanguageId);

            if (ds != null && ds.Tables.Count > 0) {
                foreach (DataRow row in ds.Tables[0].Rows) {
                    filterItemsList.Add(Convert.ToInt64(row[0].ToString()), new FilterItem(Convert.ToInt64(row[0].ToString()), row[1].ToString()));
                }
            }
            return filterItemsList;
        }
        #endregion

    }
}
