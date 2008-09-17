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

				//select
				sql.Append(" select sum("+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER ");			
				sql.Append(","+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,"+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".id_login,"+DBTables.LOGIN_PREFIXE+".login ");
				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_EXPORT_EXCEL "+DBTables.TOP_EXPORT_EXCEL_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".LOGIN "+DBTables.LOGIN_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".CONTACT "+DBTables.CONTACT_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".ADDRESS "+DBTables.ADDRESS_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".COMPANY "+DBTables.COMPANY_PREFIXE);
				//Where
				sql.Append(" where "+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_login="+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".id_login ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_contact="+DBTables.CONTACT_PREFIXE+".id_contact ");
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_address = "+DBTables.ADDRESS_PREFIXE+".id_address ");
				sql.Append(" and "+DBTables.ADDRESS_PREFIXE+".id_company="+DBTables.COMPANY_PREFIXE+".id_company ");
				//Gourp by
				sql.Append(" group by  "+DBTables.COMPANY_PREFIXE+".id_company,cpn.company,"+DBTables.TOP_EXPORT_EXCEL_PREFIXE+".id_login,"+DBTables.LOGIN_PREFIXE+".login ");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,"+DBTables.LOGIN_PREFIXE+".login ");
				#endregion

				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" Top : Impossible d'obtenir des clients effectuant le plus d'export excel ", err));
			}
			
		}
		#endregion

	}
}
