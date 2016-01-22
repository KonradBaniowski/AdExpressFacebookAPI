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
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;



namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Description résumée de Unit.
	/// </summary>
	public class Unit
	{
		/// <summary>
		/// Obtient les données des unités les plus utilisées
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>unités les plus utilisées</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table unitTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingUnit);				
				Table topUnit = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopUnit);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+topUnit.Prefix+".id_unit,"+unitTable.Prefix+".unit");
				sql.Append(",sum("+topUnit.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+topUnit.SqlWithPrefix);
				sql.Append(" ," + unitTable.SqlWithPrefix);
			
				//Where
                sql.Append(" where " + topUnit.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+topUnit.Prefix+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+unitTable.Prefix+".id_unit="+topUnit.Prefix+".id_unit");
			
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+topUnit.Prefix+".id_unit ,"+unitTable.Prefix+".unit");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible to get most used units list ", err));
				
			}
			
		}	
		
	}
}
