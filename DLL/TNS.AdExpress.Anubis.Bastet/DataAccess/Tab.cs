#region Informations
// Auteur: D. V. Mussuma
// Date de création: 28/11/2005
// Date de modification:
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Description résumée de Tab.
	/// </summary>
	public class Tab
	{
		/// <summary>
		/// Obtient les données des resultats (onglets) les plus utilisés dans AdExpress 
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données des périodes les plus utilisés</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table moduleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);
				Table resultTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightResult);
				Table topOption = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopOption);

				//select
				sql.Append(" select ");
				sql.Append(" sum("+topOption.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");
				sql.Append(" ,"+resultTable.Prefix+".id_result,"+resultTable.Prefix+".result ");
				sql.Append(" ,"+resultTable.Prefix+".ID_MODULE,"+moduleTable.Prefix+".MODULE");

				//From
				sql.Append(" from "+topOption.SqlWithPrefix);
				sql.Append(" ,"+resultTable.SqlWithPrefix);
				sql.Append(" ,"+moduleTable.SqlWithPrefix);
			
				//Where
				sql.Append(" where "+topOption.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+topOption.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+topOption.Prefix+".id_option = "+resultTable.Prefix+".id_result ");
				sql.Append(" and "+moduleTable.Prefix+".id_module="+resultTable.Prefix+".id_module ");		
			
				//Gourp by
				sql.Append(" group by  ");		
				sql.Append("  "+resultTable.Prefix+".id_result,"+resultTable.Prefix+".result");
				sql.Append("  ,"+resultTable.Prefix+".ID_MODULE,"+moduleTable.Prefix+".MODULE");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible get most used tab result ", err));
				
			}
			
		}
	}
}
