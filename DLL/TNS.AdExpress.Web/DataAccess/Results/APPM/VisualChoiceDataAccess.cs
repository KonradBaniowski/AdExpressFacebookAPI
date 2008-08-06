#region Informations
// Auteur: D. V. Mussuma
// Date de création: 28/08/2005 
// Date of Modification: 
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using Oracle.DataAccess.Client;
using TNS.FrameWork.DB.Common;
using System.Collections;
using System.Data;
using System.Text;
using DBCst=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using Cst = TNS.AdExpress.Constantes;
using WebExceptions=TNS.AdExpress.Web.Exceptions;


namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Obtient les données nécessaires au choix des visuels.
	/// </summary>
	public class VisualChoiceDataAccess
	{
		#region Liste des Media
		/// <summary>
		/// Obtient la liste des supports actifs pour la sélection produit.
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>		
		/// <param name="additionalTarget">cible supplémentaire</param>
		/// <returns>données des types d'emplacements du plan</returns>
		public static DataSet GetMediaListData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 additionalTarget){
			
			#region Variables		
			StringBuilder sql=new StringBuilder(1500);		
			#endregion						

			#region tables et champs
			string dateField=DBCst.Tables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string webPlanMonthWeek=Functions.GetAPPMWebPlanTable(webSession);
			#endregion
									
			#region Construction de la requête			

			#region select
			//select
			sql.Append("select distinct "+DBCst.Tables.MEDIA_PREFIXE+".id_media,media");
			#endregion

			#region from
			//from
			sql.Append(" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".media "+ DBCst.Tables.MEDIA_PREFIXE);
			sql.Append(", "+TNS.AdExpress.Constantes.DB.Schema.APPM_SCHEMA+"."+ webPlanMonthWeek+" "+DBCst.Tables.WEB_PLAN_PREFIXE);
			sql.Append(", "+DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
			#endregion

			#region where
			//where
			sql.Append(" where "+DBCst.Tables.MEDIA_PREFIXE+".activation<"+DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBCst.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.MEDIA_PREFIXE+".id_media="+DBCst.Tables.WEB_PLAN_PREFIXE+".id_media ");
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			//media
			sql.Append(" and "+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBCst.Tables.WEB_PLAN_PREFIXE+".id_media");
			//target
			//sql.Append(" and "+ DBCst.Tables.TARGET_PREFIXE+".id_target="+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+ additionalTarget.ToString()+")");		
			//sql.Append(" and "+ DBCst.Tables.TARGET_PREFIXE+".id_language="+DBCst.Language.FRENCH);
			//sql.Append(" and "+ DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBCst.Tables.WEB_PLAN_PREFIXE, true));						
			
			//all results without inset
			sql.Append(" and " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			//Rights
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBCst.Tables.WEB_PLAN_PREFIXE,true));	
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DBCst.Tables.WEB_PLAN_PREFIXE,true));				
						

			#endregion

			#region order by
			sql.Append(" order by media ");
			#endregion	
			
			#endregion		

			#region execution de la requête
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.VisualChoiceDataAccessException(" Impossible de charger les données des types de la sélection du support.",err));
			}		
			#endregion

		}
		#endregion

		#region Liste des visuels
		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère les données de la fiche justificative
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		///<param name="additionalTarget">cible supplémentaire</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetVisualListData(IDataSource dataSource, WebSession webSession, Int64 idMedia, int dateBegin, int dateEnd,Int64 additionalTarget){
			
			StringBuilder sql = new StringBuilder(3000);

			#region Select
			sql.Append("select "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media, "+ DBCst.Tables.MEDIA_PREFIXE +".media, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num,"+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_cover_num,");			 
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".media_paging, "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".rank_media,");								
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".visual ");
			#endregion

			#region From
			sql.Append(" from " + DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.DATA_PRESS_APPM + " " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.MEDIA + " " + DBCst.Tables.MEDIA_PREFIXE );
			sql.Append(", "+DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
			#endregion

			#region Where
			sql.Append(" where "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media = "+ idMedia);
					
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_media_num  >="+dateBegin);
			sql.Append(" and " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num <=" + dateEnd);
			
			//sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_language_data_i = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.MEDIA_PREFIXE +".id_language = "+ webSession.DataLanguage);			
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media = "+ DBCst.Tables.MEDIA_PREFIXE +".id_media ");

			sql.Append(" and "+ DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+ additionalTarget.ToString()+")");		
			//media
			sql.Append(" and "+DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".id_media");			
			//sql.Append(" and "+ DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));						
			
			//all results without inset
			sql.Append(" and " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_inset is null ");
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));

			#endregion

			#region execution de la requête
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.VisualChoiceDataAccessException(" Impossible de charger les données de la sélection du visuel. ",err));
			}		
			#endregion
		}
		#endregion
	}
}
