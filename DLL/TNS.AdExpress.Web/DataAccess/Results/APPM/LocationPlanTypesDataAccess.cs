#region Informations
// Auteur: D. V. Mussuma
// Date de création: 9/08/2005 
// Modified by: K.Shehzad
// Date of Modification: 12/08/2005  (changing the Exception usage)
#endregion
using System;
using System.Text;
using System.Data;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using  DBCst=TNS.AdExpress.Constantes.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Obtient les données des types d'emplacements du plan
	/// </summary>
	public class LocationPlanTypesDataAccess
	{
		/// <summary>
		/// Obtient les données des types d'emplacements du plan
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplémentaire</param>
		/// <returns>données des types d'emplacements du plan</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
				
			StringBuilder sql = new StringBuilder(1000);
			sql.Append(" select  id_media,date_media_num,id_advertisement,id_location,location,id_target,target ");
			switch(webSession.Unit){				
				case WebConstantes.CustomerSessions.Unit.euro:
					sql.Append(" ,sum(euro) as "+webSession.Unit+ "");
					break;
				case WebConstantes.CustomerSessions.Unit.insertion:				
					 sql.Append(" ,sum(insertion) as "+webSession.Unit+ "");
					break;
				case WebConstantes.CustomerSessions.Unit.pages:
					 sql.Append(" ,sum(pages) as  "+webSession.Unit+ "");
					break;
				case WebConstantes.CustomerSessions.Unit.grp:
					sql.Append(" ,sum(totalgrp) as "+webSession.Unit+ "");
					break;
				default:
					throw new WebExceptions.LocationPlanTypesDataAccessException("GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget)-->Le cas de cette unité n'est pas gérer. Pas de champ correspondante.");
					
			}	
			sql.Append("  from (  ");
			//Sous requête type d'emplacements
			sql.Append("  select  "+GetFields(webSession));						
			sql.Append("  from  "+GetTables());	
			sql.Append("  where  "+GetJoint(webSession));
			sql.Append("  "+GetCustomerSelection(webSession,dateBegin,dateEnd,baseTarget,additionalTarget));
			sql.Append("  group by  "+GetGroupBy(webSession));
			sql.Append("  order by "+GetOrderBy(webSession));

			sql.Append("  ) group by id_media,date_media_num,id_advertisement,id_location,location,id_target,target ");
			sql.Append(" order by location,id_location asc ");
			

			#region execution de la requête
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.LocationPlanTypesDataAccessException(" Impossible de charger les données des types d'emplacements du plan ",err));
			}		
			#endregion
		}

		#region Méthodes internes
		
		/// <summary>
		/// Champs de la requêtes
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>Champs de la requêtes</returns>
		private static string GetFields(WebSession webSession){
			string sql="";
 			sql+= DBTables.DATA_PRESS_APPM_PREFIXE+".id_media,"+DBTables.DATA_PRESS_APPM_PREFIXE+".DATE_MEDIA_NUM,"+DBTables.DATA_LOCATION_PREFIXE+".id_advertisement,";
				sql+=  DBTables.TARGET_PREFIXE+".id_target,"+DBTables.TARGET_PREFIXE+".target"
				+","+DBTables.LOCATION_PREFIXE+".id_location,"+DBTables.LOCATION_PREFIXE+".location";
			switch(webSession.Unit){				
				case WebConstantes.CustomerSessions.Unit.euro:
					sql+= " ,sum("+DBCst.Fields.EXPENDITURE_EURO+") as euro ";
					break;
				case WebConstantes.CustomerSessions.Unit.insertion:				
					sql+= ", sum("+DBCst.Fields.INSERTION+") as insertion ";
					break;
				case WebConstantes.CustomerSessions.Unit.pages:
					sql+= ", sum("+DBCst.Fields.AREA_PAGE+") as pages ";
					break;
				case WebConstantes.CustomerSessions.Unit.grp:
					sql+= " ,sum("+DBCst.Fields.INSERTION+")*"+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".grp as totalgrp ";
					break;
				default:
					throw new WebExceptions.LocationPlanTypesDataAccessException("GetFields(WebSession webSession)-->Le cas de cette unité n'est pas gérer. Pas de champ correspondante.");					
			}	
			return sql;
		}
		
		/// <summary>
		/// Libellés Tables 
		/// </summary>
		/// <returns> Tables</returns>
		private static string GetTables(){
			return DBCst.Schema.APPM_SCHEMA+"."+DBCst.Tables.DATA_PRESS_APPM+ " " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE 
				+ " , "+DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.LOCATION + " " + DBCst.Tables.LOCATION_PREFIXE 
				+" , "+DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.DATA_LOCATION + " " + DBCst.Tables.DATA_LOCATION_PREFIXE
				+ " , "+DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET + " " + DBCst.Tables.TARGET_PREFIXE 
				+ " , " + DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT + " " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE;
		}

		/// <summary>
		/// Jointures tables
		/// </summary>
		/// <param name="webSession">sesion du client</param>
		/// <returns>jointures</returns>
		private static string GetJoint(WebSession webSession){

			return DBCst.Tables.TARGET_PREFIXE + ".id_target = " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target "
				+" and " + DBCst.Tables.TARGET_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED
				+" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media "
				+" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_language_data_i="+webSession.SiteLanguage
				+" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED
				+" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_location (+)= " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_location "
				+" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_language (+)= " + webSession.SiteLanguage
				+" and " + DBCst.Tables.LOCATION_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_media (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media "
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_advertisement (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_advertisement "
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".date_media_num (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num "				
			    +" and " + DBCst.Tables.DATA_LOCATION_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED;
		}

		/// <summary>
		/// Sélections du client
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplémentaire</param>
		/// <returns>sélections du client</returns>
		private static string GetCustomerSelection(WebSession webSession,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			return " and " + DBCst.Tables.TARGET_PREFIXE + ".id_target in (" + baseTarget + ","+additionalTarget+")"
				+" and "+ DBTables.TARGET_PREFIXE+".id_language="+webSession.SiteLanguage.ToString()
				+" and "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".date_media_num >= "+dateBegin.ToString()+"  and  "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".date_media_num <= "+dateEnd.ToString()
				+" and "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".id_inset is null "
				//Sélection client
				+"  "+WebFunctions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true)
				//Droits client
				+"  "+WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBCst.Tables.DATA_PRESS_APPM_PREFIXE,true)
				+"  "+WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DBCst.Tables.DATA_PRESS_APPM_PREFIXE,true);
		}
		
		/// <summary>
		/// Regroupement des données
		/// </summary>
		/// <param name="webSession">session</param>
		/// <returns>Regroupement des données</returns>
		private static string GetGroupBy(WebSession webSession){
			string 
				sql= DBTables.DATA_PRESS_APPM_PREFIXE+".id_media,"+DBTables.DATA_PRESS_APPM_PREFIXE+".DATE_MEDIA_NUM,"+DBTables.DATA_LOCATION_PREFIXE+".id_advertisement,";
				sql+=DBCst.Tables.TARGET_PREFIXE + ".id_target,"+DBCst.Tables.TARGET_PREFIXE + ".target ";
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp )sql+=","+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".grp";
				sql+=","+DBCst.Tables.LOCATION_PREFIXE + ".id_location,"+DBCst.Tables.LOCATION_PREFIXE + ".location";
			return sql;
		}
		
		/// <summary>
		/// Tri des données
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>requete Tri des données</returns>
		private static string GetOrderBy(WebSession webSession){
			return DBCst.Tables.TARGET_PREFIXE + ".id_target,"+DBCst.Tables.TARGET_PREFIXE + ".target "				
				+","+DBCst.Tables.LOCATION_PREFIXE + ".id_location,"+DBCst.Tables.LOCATION_PREFIXE + ".location";
		}
		#endregion
	}
}
