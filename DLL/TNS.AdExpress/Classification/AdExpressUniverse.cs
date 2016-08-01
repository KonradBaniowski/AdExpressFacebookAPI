#region Informations
// Auteur: D. Mussuma
// Cr�ation: 19/11/2007
// Modification:
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using TNS.Classification.Universe;


namespace TNS.AdExpress.Classification  {

	/// <summary>
	/// AdExpress Universe Class
	/// </summary>
	[System.Serializable]
	public class AdExpressUniverse : TNS.Classification.Universe.Universe {

		#region Constructor
		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">Universe dimension (product or media)</param>
		public AdExpressUniverse(TNS.Classification.Universe.Dimension dimension)
			: base(dimension, TNS.Classification.Universe.Security.full) {

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dimension">Universe dimension (product or media)</param>
		/// <param name="universeLabel">universe label</param>
		public AdExpressUniverse(string universeLabel,TNS.Classification.Universe.Dimension dimension)
			: base(universeLabel, dimension, TNS.Classification.Universe.Security.full) {

		}
		#endregion

		#region Sql Conditions
       

        /// <summary>
        /// Get sql conditions corresponding to the selected universe items
        /// </summary>
        /// <param name="dataTablePrefix">Current dataTable Prefix</param>
        /// <param name="beginByAnd">Indicates if condition begin by and</param>
        ///<param name="replaceIds">replace classificaion items ids</param>
        /// <returns>String sql conditions</returns>
        public string GetSqlConditions(string dataTablePrefix, bool beginByAnd, Func<long, string, string> replaceIds=null)
        {

            string sql = "";

            //Get include elements conditions
            sql = GetSqlConditions(dataTablePrefix, beginByAnd, AccessType.includes, replaceIds);

            //Get excludes elements conditions
            if (sql.Length > 0) beginByAnd = true;
            sql += GetSqlConditions(dataTablePrefix, beginByAnd, AccessType.excludes, replaceIds);

            return sql;
        }

       
	    /// <summary>
		/// Get sql conditions corresponding to the selected universe items according access type (include or exclude)
		/// </summary>
		/// <param name="dataTablePrefix">Current dataTable Prefix</param>
		/// <param name="beginByAnd">Indicates if condition begin by and</param>
		/// <returns>String sql conditions</returns>
		/// <param name="accessType">access type (include or exclude)</param>
		/// <param name="replaceIds">replace classificaion items ids</param>
		/// <returns></returns>
		public string GetSqlConditions(string dataTablePrefix, bool beginByAnd, AccessType accessType
            ,Func<long,string,string> replaceIds=null) {
		    var sql = new StringBuilder(3000);
			bool first = true;

		    //Get include or exlucdes elements conditions
			List<NomenclatureElementsGroup> listGroup = (accessType == AccessType.includes) 
                ? GetIncludes() : GetExludes();
			if (listGroup != null && listGroup.Count > 0) {

				for (int i = 0; i < listGroup.Count; i++) {
					if (listGroup[i] != null && listGroup[i].Count() > 0) {
						List<long> levelList = listGroup[i].GetLevelIdsList();
						for (int j = 0; j < levelList.Count; j++) {
							//For each level collection generate the sql condition
							string listIds = listGroup[i].GetAsString(levelList[j]);
							if (accessType == AccessType.includes) {
								if (first) {
									if (beginByAnd) sql.Append(" and ");
									sql.Append(" ( ");
								}
								else sql.Append(" or ");
							}
							else {
								if (first) {
									if (beginByAnd) sql.Append(" and ");
									sql.Append(" ( ");
								}
								else sql.Append(" and ");
							}
						    if (replaceIds != null) listIds = replaceIds(levelList[j], listIds);
						    sql.AppendFormat(accessType == AccessType.includes ? " {0}.{1} in ({2}) " 
                                : " {0}.{1} not in ({2}) ",dataTablePrefix
						                     , UniverseLevels.Get(levelList[j]).DataBaseIdField, listIds);
						    first = false;
						}
						if (!first) sql.Append(" )");
					}
					//Change group
					first = true;
					beginByAnd = true;
				}
			}

			return sql.ToString();
		}
		#endregion

		
	}
}