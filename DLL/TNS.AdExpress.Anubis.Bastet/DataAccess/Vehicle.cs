#region Informations
// Auteur: D. V. Mussuma
// Date de création: 5/12/2005
// Date de modification:
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Constantes.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Description résumée de Vehicle.
	/// </summary>
	public class Vehicle
	{
		/// <summary>
		/// Obtient les données des médias les plus utilisés
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données des médias les plus utilisés</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+DBTables.TOP_VEHICLE_PREFIXE+".id_vehicle,"+DBTables.VEHICLE_PREFIXE+".vehicle");
				sql.Append(",sum("+DBTables.TOP_VEHICLE_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_VEHICLE "+DBTables.TOP_VEHICLE_PREFIXE);
				sql.Append(" ,"+DBSchema.ADEXPRESS_SCHEMA+".VEHICLE "+DBTables.VEHICLE_PREFIXE);
			
				//Where
				sql.Append(" where "+DBTables.TOP_VEHICLE_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+DBTables.TOP_VEHICLE_PREFIXE+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+DBTables.TOP_VEHICLE_PREFIXE+".id_vehicle="+DBTables.VEHICLE_PREFIXE+".id_vehicle");
				sql.Append(" and "+DBTables.VEHICLE_PREFIXE+".id_language="+Language.FRENCH.GetHashCode());
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+DBTables.TOP_VEHICLE_PREFIXE+".ID_vehicle ,"+DBTables.VEHICLE_PREFIXE+".vehicle");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible d'obtenir la liste des médias les plus utilisés ", err));
				
			}
			
		}

			/// <summary>
			/// Obtient les données des médias les plus utilisés par module
			/// </summary>
			/// <param name="parameters">parametres des statistiques</param>
			/// <returns>données des médias les plus utilisés par module</returns>
			public static DataTable TopByModule(BastetCommon.Parameters parameters) {
				try{
					#region Requête
					StringBuilder sql = new StringBuilder(3000);

					//select
					sql.Append(" select ");	
					sql.Append(" "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_vehicle,vehicle");
					sql.Append(", "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_module,module");
					sql.Append(",sum("+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

					//From
					sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_VEHICLE_BY_MODULE "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE);
					sql.Append(" ,"+DBSchema.ADEXPRESS_SCHEMA+".VEHICLE "+DBTables.VEHICLE_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".MODULE "+DBTables.MODULE_PREFIXE);
			
					//Where
					sql.Append(" where "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
					if(parameters!=null && parameters.Logins.Length>0){
						sql.Append(" and "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_login in ("+parameters.Logins+") ");				
					}
					sql.Append(" and "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_vehicle="+DBTables.VEHICLE_PREFIXE+".id_vehicle");
					sql.Append(" and "+DBTables.VEHICLE_PREFIXE+".id_language="+Language.FRENCH.GetHashCode());
					sql.Append(" and "+DBTables.VEHICLE_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
					sql.Append(" and "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_module="+DBTables.MODULE_PREFIXE+".id_module");					
					sql.Append(" and "+DBTables.MODULE_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
					//Gourp by
					sql.Append(" group by  ");
					sql.Append("  "+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_module,module,"+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_vehicle ,vehicle ");
					//Order by
					sql.Append(" order by  module asc,"+DBTables.TOP_VEHICLE_BY_MODULE_PREFIXE+".id_vehicle ,vehicle");
					#endregion
				
					#region Execution
			
					return(parameters.Source.Fill(sql.ToString()).Tables[0]);
					#endregion
				}
				catch(System.Exception err){
					throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopByModule : Impossible d'obtenir la liste des médias les plus utilisés par module", err));
				
				}
			
			}

	}
}
