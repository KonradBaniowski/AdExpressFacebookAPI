#region Information
/*
 * Author : G. Ragneau
 * Creation : 24/08/2005
 * Modification : 
 *		
 * */
#endregion

using System;
using System.Data;

using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products
{
	/// <summary>
	/// Request managing groups
	/// </summary>
	public class GroupDataAccess
	{
		/// <summary>
		/// Get list of groups matcing with the CurrentAdvertiserSelection in the user session
		/// </summary>
		/// <param name="webSession">User session</param>
		/// <param name="dataSource">Data Source</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.GroupDataAccessException">Thrown whenever an error occured while retrieving data from database</exception>
		/// <returns>List of groups</returns>
		internal static DataSet ListFromSelection(IDataSource dataSource, WebSession webSession){
			string sql = string.Empty;
			try{
				sql += "select distinct group_ ";
                sql += " from " + Schema.APPM_SCHEMA + "." + Tables.DATA_PRESS_APPM + " " + Tables.DATA_PRESS_APPM_PREFIXE;
				sql += "," + Schema.ADEXPRESS_SCHEMA + ".group_ " + Tables.GROUP_PREFIXE;
				sql += " where " + Tables.DATA_PRESS_APPM_PREFIXE + ".id_group_=" + Tables.GROUP_PREFIXE + ".id_group_ ";
				sql += " and " + Tables.GROUP_PREFIXE + ".id_language = " + webSession.DataLanguage;
				// Sélection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
					sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(Tables.DATA_PRESS_APPM_PREFIXE, true);

				//sql += SQLGenerator.GetAnalyseCustomerProductSelection(webSession,"",Tables.DATA_PRESS_APPM_PREFIXE,Tables.DATA_PRESS_APPM_PREFIXE,Tables.DATA_PRESS_APPM_PREFIXE,Tables.DATA_PRESS_APPM_PREFIXE,Tables.DATA_PRESS_APPM_PREFIXE,Tables.DATA_PRESS_APPM_PREFIXE,true);
				sql += SQLGenerator.getAnalyseCustomerProductRight(webSession,Tables.DATA_PRESS_APPM_PREFIXE,true);
				sql += " order by group_";

				return dataSource.Fill(sql);

			}
			catch(System.Exception e){
				throw(new GroupDataAccessException("Unable to retrieve groups list from the database : " + sql,e));
			}
		}

	}
}
