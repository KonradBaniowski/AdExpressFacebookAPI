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

				//select
				sql.Append(" select ");
				sql.Append(" sum("+DBTables.TOP_OPTION_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");
				sql.Append(" ,"+DBTables.RESULT_PREFIXE+".id_result,"+DBTables.RESULT_PREFIXE+".result ");
				sql.Append(" ,"+DBTables.RESULT_PREFIXE+".ID_MODULE,"+DBTables.MODULE_PREFIXE+".MODULE");

				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_OPTION "+DBTables.TOP_OPTION_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".RESULT "+DBTables.RESULT_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".MODULE "+DBTables.MODULE_PREFIXE);
			
				//Where
				sql.Append(" where "+DBTables.TOP_OPTION_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.TOP_OPTION_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.TOP_OPTION_PREFIXE+".id_option = "+DBTables.RESULT_PREFIXE+".id_result ");
				sql.Append(" and "+DBTables.MODULE_PREFIXE+".id_module="+DBTables.RESULT_PREFIXE+".id_module ");		
			
				//Gourp by
				sql.Append(" group by  ");		
				sql.Append("  "+DBTables.RESULT_PREFIXE+".id_result,"+DBTables.RESULT_PREFIXE+".result");
				sql.Append("  ,"+DBTables.RESULT_PREFIXE+".ID_MODULE,"+DBTables.MODULE_PREFIXE+".MODULE");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible d'obtenir la liste des options (onglets) les plus utilisés ", err));
				
			}
			
		}
	}
}
