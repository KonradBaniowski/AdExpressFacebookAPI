using System;
using System.Collections.Generic;
using System.Data;
using TNS.AdExpress.Web.Core.Selection;

namespace TNS.AdExpress.Web.Core
{
    public class SpotSubTypes
    {

        /// <summary>
        /// Spot sub types Items
        /// </summary>
        private static Dictionary<int, List<FilterItem>> _list;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static SpotSubTypes()
        {
            _list = new Dictionary<int, List<FilterItem>>();
        }
        #endregion
        #region Init
        /// <summary>
        /// Init the active banners format list
        /// </summary>
        public static void Init()
        {
         
            var ds = DataAccess.SpotSubTypesDAL.GetData();

            if (ds != null && ds.Tables.Count > 0)
            {
                _list = new Dictionary<int, List<FilterItem>>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int idLanguage = Convert.ToInt32(row["id_language"].ToString());

                    if (_list.ContainsKey(idLanguage))
                    {
                        _list[idLanguage].Add(
                            new FilterItem(Convert.ToInt64(row["id_spot_sub_type"].ToString()),
                                row["spot_sub_type"].ToString()));
                    }
                    else
                    {
                        var filterItem = new FilterItem(Convert.ToInt64(row["id_spot_sub_type"].ToString()),
                            row["spot_sub_type"].ToString());
                        _list.Add(idLanguage, new List<FilterItem>{ filterItem });
                    }
                }
            }

        }
        #endregion

        #region Get Items
        /// <summary>
        /// Method used to get the list of spot sub types
        /// </summary>
        /// <returns>The list of purchase mode</returns>
        public static Dictionary<int, List<FilterItem>> GetItems()
        {
            return _list;
        }
        #endregion
    }
}