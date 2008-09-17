#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 28/11/2005
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
	/// Description r�sum�e de Unit.
	/// </summary>
	public class Unit
	{
		/// <summary>
		/// Obtient les donn�es des unit�s les plus utilis�es
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>unit�s les plus utilis�es</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters) {
			try{
				#region Requ�te
				StringBuilder sql = new StringBuilder(3000);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+DBTables.TOP_UNIT_PREFIXE+".id_unit,"+DBTables.UNIT_PREFIXE+".unit");
				sql.Append(",sum("+DBTables.TOP_UNIT_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_UNIT "+DBTables.TOP_UNIT_PREFIXE);
				sql.Append(" ,"+DBSchema.UNIVERS_SCHEMA+".UNIT "+DBTables.UNIT_PREFIXE);
			
				//Where
				sql.Append(" where "+DBTables.TOP_UNIT_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+DBTables.TOP_UNIT_PREFIXE+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+DBTables.UNIT_PREFIXE+".id_unit="+DBTables.TOP_UNIT_PREFIXE+".id_unit");
			
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+DBTables.TOP_UNIT_PREFIXE+".id_unit ,"+DBTables.UNIT_PREFIXE+".unit");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible d'obtenir la liste des unit�s les plus utilis�s ", err));
				
			}
			
		}	
		
	}
}
