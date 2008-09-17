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
	/// Description résumée de Period.
	/// </summary>
	public class Period
	{
		/// <summary>
		/// Obtient les données des périodes les plus utilisés
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données des périodes les plus utilisés</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+DBTables.TOP_PERIODE_PREFIXE+".id_periode,"+DBTables.PERIODE_PREFIXE+".periode");
				sql.Append(",sum("+DBTables.TOP_PERIODE_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_PERIODE "+DBTables.TOP_PERIODE_PREFIXE);
				sql.Append(" ,"+DBSchema.UNIVERS_SCHEMA+".PERIODE "+DBTables.PERIODE_PREFIXE);
			
				//Where
				sql.Append(" where "+DBTables.TOP_PERIODE_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+DBTables.TOP_PERIODE_PREFIXE+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+DBTables.PERIODE_PREFIXE+".id_periode="+DBTables.TOP_PERIODE_PREFIXE+".id_periode");
			
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+DBTables.TOP_PERIODE_PREFIXE+".ID_PERIODE ,"+DBTables.PERIODE_PREFIXE+".periode");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible d'obtenir la liste des périodes les plus utilisés ", err));
				
			}
		
		}

		
	}
}
