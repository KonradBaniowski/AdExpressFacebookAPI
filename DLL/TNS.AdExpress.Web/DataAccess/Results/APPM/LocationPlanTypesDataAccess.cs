#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 9/08/2005 
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
using TNS.AdExpress.Domain.Units;
namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Obtient les donn�es des types d'emplacements du plan
	/// </summary>
	public class LocationPlanTypesDataAccess
	{
		/// <summary>
		/// Obtient les donn�es des types d'emplacements du plan
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de donn�es</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible suppl�mentaire</param>
		/// <returns>donn�es des types d'emplacements du plan</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
				
			StringBuilder sql = new StringBuilder(1000);
			sql.Append(" select  id_media,date_media_num,id_advertisement,id_location,location,id_target,target ");

            sql.AppendFormat(", {0}", WebFunctions.SQLGenerator.GetUnitAliasSum(webSession));

			sql.Append("  from (  ");
			//Sous requ�te type d'emplacements
			sql.Append("  select  "+GetFields(webSession));						
			sql.Append("  from  "+GetTables());	
			sql.Append("  where  "+GetJoint(webSession));
			sql.Append("  "+GetCustomerSelection(webSession,dateBegin,dateEnd,baseTarget,additionalTarget));
			sql.Append("  group by  "+GetGroupBy(webSession));
			sql.Append("  order by "+GetOrderBy(webSession));

			sql.Append("  ) group by id_media,date_media_num,id_advertisement,id_location,location,id_target,target ");
			sql.Append(" order by location,id_location asc ");
			

			#region execution de la requ�te
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.LocationPlanTypesDataAccessException(" Impossible de charger les donn�es des types d'emplacements du plan ",err));
			}		
			#endregion
		}

		#region M�thodes internes
		
		/// <summary>
		/// Champs de la requ�tes
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>Champs de la requ�tes</returns>
		private static string GetFields(WebSession webSession){
			StringBuilder sql=new StringBuilder();
            sql.AppendFormat("{0}.id_media, {0}.DATE_MEDIA_NUM,{1}.id_advertisement, {2}.id_target, {2}.target, {3}.id_location, {3}.location"
                , DBTables.DATA_PRESS_APPM_PREFIXE
                , DBTables.DATA_LOCATION_PREFIXE
                , DBTables.TARGET_PREFIXE
                , DBTables.LOCATION_PREFIXE);

            if(webSession.Unit != WebConstantes.CustomerSessions.Unit.grp)
                sql.AppendFormat(", {0}",WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, DBCst.TableType.Type.dataVehicle4M));
            else
                sql.AppendFormat(", sum({0})*{1}.{2} as {3} "
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                    , DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
			
			return sql.ToString();
		}
		
		/// <summary>
		/// Libell�s Tables 
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
				//+" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_language_data_i="+webSession.DataLanguage
				+" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED
				+" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_location (+)= " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_location "
				+" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_language (+)= " + webSession.DataLanguage
				+" and " + DBCst.Tables.LOCATION_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_media (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media "
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_advertisement (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_advertisement "
				+" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".date_media_num (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num "				
			    +" and " + DBCst.Tables.DATA_LOCATION_PREFIXE +".activation < "+  DBCst.ActivationValues.UNACTIVATED;
		}

		/// <summary>
		/// S�lections du client
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible suppl�mentaire</param>
		/// <returns>s�lections du client</returns>
		private static string GetCustomerSelection(WebSession webSession,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){

            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
				
            return " and " + DBCst.Tables.TARGET_PREFIXE + ".id_target in (" + baseTarget + ","+additionalTarget+")"
				+" and "+ DBTables.TARGET_PREFIXE+".id_language="+webSession.DataLanguage.ToString()
				+" and "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".date_media_num >= "+dateBegin.ToString()+"  and  "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".date_media_num <= "+dateEnd.ToString()
				+" and "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".id_inset is null "
				//S�lection client
				+"  "+WebFunctions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true)
				//Droits client
				+"  "+WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBCst.Tables.DATA_PRESS_APPM_PREFIXE,true)
                + "  " + WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true, module.ProductRightBranches);
		}
		
		/// <summary>
		/// Regroupement des donn�es
		/// </summary>
		/// <param name="webSession">session</param>
		/// <returns>Regroupement des donn�es</returns>
		private static string GetGroupBy(WebSession webSession){
			string 
				sql= DBTables.DATA_PRESS_APPM_PREFIXE+".id_media,"+DBTables.DATA_PRESS_APPM_PREFIXE+".DATE_MEDIA_NUM,"+DBTables.DATA_LOCATION_PREFIXE+".id_advertisement,";
				sql+=DBCst.Tables.TARGET_PREFIXE + ".id_target,"+DBCst.Tables.TARGET_PREFIXE + ".target ";
                if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp) sql += "," + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField;
				sql+=","+DBCst.Tables.LOCATION_PREFIXE + ".id_location,"+DBCst.Tables.LOCATION_PREFIXE + ".location";
			return sql;
		}
		
		/// <summary>
		/// Tri des donn�es
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>requete Tri des donn�es</returns>
		private static string GetOrderBy(WebSession webSession){
			return DBCst.Tables.TARGET_PREFIXE + ".id_target,"+DBCst.Tables.TARGET_PREFIXE + ".target "				
				+","+DBCst.Tables.LOCATION_PREFIXE + ".id_location,"+DBCst.Tables.LOCATION_PREFIXE + ".location";
		}
		#endregion
	}
}