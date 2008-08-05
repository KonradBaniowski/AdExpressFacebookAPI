#region Informations
// Auteur: Y. R'kaina
// Date de création: 25/01/2007 
#endregion

using System;
using System.Data; 
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using System.Text;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using Cst = TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.DataAccess;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM{
	/// <summary>
	/// Description résumée du DataAccess de Sector Data Seasonality
	/// </summary>
	public class SectorDataSeasonalityDataAccess{

		#region Get données

		/// <summary>
		/// Calcul et retourne Dataset pour Analyse des familles d'intérêts du plan des APPM 
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		/// <param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>Dataset pour les familles d'intérêts du plan des APPM </returns>
		public static DataSet GetSeasonalityData(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){

			#region construction de la requête
			StringBuilder sql = new StringBuilder(2000);
			
			#region Table and field names
			string dateField = Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName = Functions.GetAPPMWebPlanTable(webSession);
			#endregion

			#region Select
			sql.Append("select ");
			sql.Append(""+dateField+", id_target , target,  sum(totalgrp) as totalgrp, sum(euro) as euros , ");
			
			#region sélection par rappot à l'unité choisit
			switch (webSession.Unit){
				case WebConstantes.CustomerSessions.Unit.euro:  
				case WebConstantes.CustomerSessions.Unit.kEuro:  
					sql.Append("   sum(budget) as unit  " );
					break;
				case WebConstantes.CustomerSessions.Unit.grp:  
					sql.Append(" sum(grp) as unit " );					
					break;
				case WebConstantes.CustomerSessions.Unit.insertion:  
					sql.Append("   sum(insertion) as unit  " );
					break;
				case WebConstantes.CustomerSessions.Unit.pages:  
					//					sql.Append("   sum(page)/1000 as unit " );
					sql.Append("   sum(page) as unit " );
					break;
				default : break;
			}
			#endregion

			#endregion

			#region From
			sql.Append(" from ");
			sql.Append("(");
			#endregion

				#region select
				sql.Append("select ");
				sql.Append(dateField+", " );
				sql.Append(DBConstantes.Tables.TARGET_PREFIXE+".id_target, " );
				sql.Append(DBConstantes.Tables.TARGET_PREFIXE+".target," );
				sql.Append("sum(Totalinsert)* GRP as totalgrp , " );
				sql.Append(" sum(Totalunite )as euro , " );
	
				#region sélection par rappot à l'unité choisit
				switch (webSession.Unit){
					case WebConstantes.CustomerSessions.Unit.euro: 
					case WebConstantes.CustomerSessions.Unit.kEuro:
						sql.Append("sum(Totalunite)as budget  " );
						break;
					case WebConstantes.CustomerSessions.Unit.grp: 
						sql.Append(DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".grp as grp" );					
						break;
					case WebConstantes.CustomerSessions.Unit.insertion: 
						sql.Append("sum(Totalinsert) as insertion  " );
						break;
					case WebConstantes.CustomerSessions.Unit.pages: 
						sql.Append("sum(Totalpages) as page  " );
						break;
					default : break;
				}
				#endregion
				#endregion

				#region From
				sql.Append(" from ");
				sql.Append(DBConstantes.Schema.APPM_SCHEMA+"."+dataTableName+"  "+DBConstantes.Tables.WEB_PLAN_PREFIXE+",  ");
				sql.Append(DBConstantes.Schema.APPM_SCHEMA+".WAVE "+ DBConstantes.Tables.WAVE_PREFIXE+", ");
				sql.Append(DBConstantes.Schema.APPM_SCHEMA+".TARGET " +DBConstantes.Tables.TARGET_PREFIXE+", ");
				sql.Append(DBConstantes.Schema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT " +DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
				#endregion

				#region Where
				sql.Append(" where ");
				// Jointures
				sql.Append(DBConstantes.Tables.WAVE_PREFIXE+".id_wave="+DBConstantes.Tables.TARGET_PREFIXE+".id_wave" );
				sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target" );
				sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".ID_MEDIA_SECODIP="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media" );
				
				//media
				sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage );
				sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".activation < "+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
				
				//target
				sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_language="+webSession.DataLanguage);
				sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+ ".activation < " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
				
				sql.Append(" and "+ dateField+">="+dateBegin.ToString());
				sql.Append(" and "+dateField+"<="+dateEnd.ToString());//date sélectionné
		
				//vague Sélectionnée
				sql.Append(" and "+ DBConstantes.Tables.WAVE_PREFIXE+".id_wave="+idWave );
				sql.Append(" and "+ DBConstantes.Tables.WAVE_PREFIXE+".activation < " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
				//Cibles
				sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_target in("+idBaseTarget.ToString()+","+ idAdditionalTarget.ToString()+")");	
				//product + univers			
				// Sélection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
					sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.WEB_PLAN_PREFIXE, true));						
				//Droits clients
				sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));	
				sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));	
				//tous les resultats sans inset
				sql.Append(" and " + DBConstantes.Tables.WEB_PLAN_PREFIXE+ ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
				#endregion

				#region group by
				sql.Append(" group by "+dateField+", "+DBConstantes.Tables.TARGET_PREFIXE+".id_target ,"+DBConstantes.Tables.TARGET_PREFIXE+".target ,"+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".grp ");
				#endregion

			#region group by
			sql.Append(")");
			sql.Append(" group by  "+dateField+", id_target, target");
			sql.Append(" order by  "+dateField+"");
			#endregion
			
			#endregion

			#region execution
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataSeasonalityDataAccessException("Impossible de charger les données de Sector Data Seasonality",err));
			}
			#endregion
		}

		#endregion
	}
}
