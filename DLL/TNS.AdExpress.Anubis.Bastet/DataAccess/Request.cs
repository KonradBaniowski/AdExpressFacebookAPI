#region Informations
///////////////////////////////////////////////////////////
//  Request.cs
//  Created on:      05-déc.-2005 16:51:11
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
	///Obtient les données du top des clients utilisant les requ^tes sauvegardées
	/// </summary>
	public class Request
	{
		#region Top 
		/// <summary>
		///  Obtient les données du top des clients utilisant les requ^tes sauvegardées
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données du top export de fichiers</returns>
		public static DataTable TopClient(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table topMyAdExpress = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopMyAdExpress);

				//select
				sql.Append(" select sum("+topMyAdExpress.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER ");			
				sql.Append(","+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+topMyAdExpress.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//From
				sql.Append(" from " + topMyAdExpress.SqlWithPrefix);
				sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
					+ "," + addressTable.SqlWithPrefix + "," + companyTable.SqlWithPrefix);
				//Where
                sql.Append(" where " + topMyAdExpress.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+topMyAdExpress.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+topMyAdExpress.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
				//Gourp by
				sql.Append(" group by  "+companyTable.Prefix+".id_company,cpn.company,"+topMyAdExpress.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,"+loginTable.Prefix+".login ");
				#endregion

				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopClient : Impossible to get clients using most saved request ", err));
			}
			
		}
		#endregion
	}
}
