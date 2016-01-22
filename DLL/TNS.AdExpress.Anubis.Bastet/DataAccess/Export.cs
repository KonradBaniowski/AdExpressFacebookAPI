#region Informations
///////////////////////////////////////////////////////////
//  Export.cs
//  Created on:      25-nov.-2005 16:51:11
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;



namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Obtient les données du top export de fichiers
	/// </summary>
	public class Export
	{
		#region Top 
		/// <summary>
		///  Obtient les données du top export de fichiers
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données du top export de fichiers</returns>
		public static DataTable Top(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table topExcelTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopExcelExport);

				//select
				sql.Append(" select sum("+topExcelTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER ");
                sql.Append("," + companyTable.Prefix + ".id_company ," + companyTable.Prefix + ".company," + topExcelTable.Prefix + ".id_login," + loginTable.Prefix + ".login, " + contactTable.Prefix + ".first_name, " + contactTable.Prefix + ".name ");
				//From				
				sql.Append(" from " + topExcelTable.SqlWithPrefix);
					sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
						+ "," + addressTable.SqlWithPrefix + "," + companyTable.SqlWithPrefix);
				//Where
                    sql.Append(" where " + topExcelTable.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+topExcelTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+topExcelTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
				//Gourp by
                sql.Append(" group by  " + companyTable.Prefix + ".id_company,cpn.company," + topExcelTable.Prefix + ".id_login," + loginTable.Prefix + ".login, " + contactTable.Prefix + ".first_name, " + contactTable.Prefix + ".name ");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,"+loginTable.Prefix+".login ");
				#endregion

				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" Top : Impossible to export clients with most excel exports ", err));
			}
			
		}
		#endregion

	}
}
