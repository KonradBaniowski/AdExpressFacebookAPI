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
				Table periodTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingPeriod);
				Table topPeriod = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopPeriod);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+topPeriod.Prefix+".id_periode,"+periodTable.Prefix+".periode");
				sql.Append(",sum("+topPeriod.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+topPeriod.SqlWithPrefix);
				sql.Append(" ,"+periodTable.SqlWithPrefix);
			
				//Where
				sql.Append(" where "+topPeriod.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+topPeriod.Prefix+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+periodTable.Prefix+".id_periode="+topPeriod.Prefix+".id_periode");
			
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+topPeriod.Prefix+".ID_PERIODE ,"+periodTable.Prefix+".periode");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible get most used period list ", err));
				
			}
		
		}

		
	}
}
