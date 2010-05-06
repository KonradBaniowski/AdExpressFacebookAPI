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
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;


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
		public static DataTable TopUsed(BastetCommon.Parameters parameters, int language) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
				Table topVehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopVehicle);

				//select
				sql.Append(" select ");
		
				sql.Append(" "+topVehicleTable.Prefix+".id_vehicle,"+vehicleTable.Prefix+".vehicle");
				sql.Append(",sum("+topVehicleTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from " + topVehicleTable.SqlWithPrefix);
				sql.Append(" ," + vehicleTable.SqlWithPrefix);
			
				//Where
                sql.Append(" where " + topVehicleTable.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
				if(parameters!=null && parameters.Logins.Length>0){
					sql.Append(" and "+topVehicleTable.Prefix+".id_login in ("+parameters.Logins+") ");				
				}
				sql.Append(" and "+topVehicleTable.Prefix+".id_vehicle="+vehicleTable.Prefix+".id_vehicle");
				sql.Append(" and "+vehicleTable.Prefix+".id_language="+language);
				//Gourp by
				sql.Append(" group by  ");
				sql.Append("  "+topVehicleTable.Prefix+".ID_vehicle ,"+vehicleTable.Prefix+".vehicle");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible to get most used media list ", err));
				
			}
			
		}

			/// <summary>
			/// Obtient les données des médias les plus utilisés par module
			/// </summary>
			/// <param name="parameters">parametres des statistiques</param>
			/// <returns>données des médias les plus utilisés par module</returns>
			public static DataTable TopByModule(BastetCommon.Parameters parameters,int language) {
				try{
					#region Requête
					StringBuilder sql = new StringBuilder(3000);
					Table vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
					Table topVehicleByModuleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopVehicleByModule);
					Table moduleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);

					//select
					sql.Append(" select ");	
					sql.Append(" "+topVehicleByModuleTable.Prefix+".id_vehicle,vehicle");
					sql.Append(", "+topVehicleByModuleTable.Prefix+".id_module,module");
					sql.Append(",sum("+topVehicleByModuleTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

					//From
					sql.Append(" from " + topVehicleByModuleTable.SqlWithPrefix);
					sql.Append(" ,"+vehicleTable.SqlWithPrefix+","+moduleTable.SqlWithPrefix);
			
					//Where
                    sql.Append(" where " + topVehicleByModuleTable.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
					if(parameters!=null && parameters.Logins.Length>0){
						sql.Append(" and "+topVehicleByModuleTable.Prefix+".id_login in ("+parameters.Logins+") ");				
					}
					sql.Append(" and "+topVehicleByModuleTable.Prefix+".id_vehicle="+vehicleTable.Prefix+".id_vehicle");
					sql.Append(" and "+vehicleTable.Prefix+".id_language="+ language);
					sql.Append(" and "+vehicleTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
					sql.Append(" and "+topVehicleByModuleTable.Prefix+".id_module="+moduleTable.Prefix+".id_module");					
					sql.Append(" and "+moduleTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
					//Gourp by
					sql.Append(" group by  ");
					sql.Append("  "+topVehicleByModuleTable.Prefix+".id_module,module,"+topVehicleByModuleTable.Prefix+".id_vehicle ,vehicle ");
					//Order by
					sql.Append(" order by  module asc,"+topVehicleByModuleTable.Prefix+".id_vehicle ,vehicle");
					#endregion
				
					#region Execution
			
					return(parameters.Source.Fill(sql.ToString()).Tables[0]);
					#endregion
				}
				catch(System.Exception err){
					throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopByModule : Impossible t oget most used media by module", err));
				
				}
			
			}

	}
}
