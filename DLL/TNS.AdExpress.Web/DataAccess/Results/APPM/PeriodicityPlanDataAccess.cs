#region Informations
// Auteur: A.DADOUCH
// Date de cr�ation: 15/07/2005 
// Modified by: 
// K.Shehzad : 12/08/2005  (changing the Exception usage)
// D. V. Mussuma : 01/09/2005  (Adding the customer media and products rights)
// K.Shehzad: 05/09/2005 Table/Field names changed
// D. V. Mussuma : 21/10/2005  (Adding the unit Keuro management)

#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using System.Text;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using Cst = TNS.AdExpress.Constantes;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM {
	/// <summary>
	///  Analyse par p�riodicit�
	/// </summary>
	public class PeriodicityPlanDataAccess{

		#region Get donn�es

		/// <summary>
		/// Calcul et retourne Dataset pour les p�riodicit�s du plan des APPM 
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		/// <param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible s�lectionn�e</param>
		/// <param name="products">produit selectionn�es</param>
		/// <returns>Dataset pour les p�riodicit�s du plan des APPM </returns>
		public static DataSet GetPeriodicityData(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget,string products){

			#region construction de la requ�te
			StringBuilder sql = new StringBuilder(2000);

			sql.Append("select ");
            sql.AppendFormat("vehicle, id_periodicity, periodicity, id_target , target,  sum(totalgrp) as totalgrp, sum({0}) as {0} "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());

			#region s�lection par rappot � l'unit� choisit
            if (webSession.Unit != WebConstantes.CustomerSessions.Unit.euro)
                sql.AppendFormat(",{0} ",WebFunctions.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(webSession));
			#endregion
			sql.Append(" from ");
			sql.Append("(");

			sql.Append("select ");
            sql.AppendFormat("vehicle, id_periodicity, periodicity, id_target , target, totalgrp , {0} "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());

            #region s�lection par rappot � l'unit� choisit
            if (webSession.Unit != WebConstantes.CustomerSessions.Unit.grp
                && webSession.Unit != WebConstantes.CustomerSessions.Unit.euro)
                sql.AppendFormat(",{0} ", WebFunctions.SQLGenerator.GetUnitAlias(webSession));
            else if (webSession.Unit != WebConstantes.CustomerSessions.Unit.euro)
                sql.AppendFormat(",sum(TOTALGRP) as {0} ", WebFunctions.SQLGenerator.GetUnitAlias(webSession));
            #endregion
  
			// construction de la table pour from
			sql.Append(" from ");
			sql.Append("(");
			sql.Append("select ");
			sql.Append(GetPeriodicitySelection(webSession));
			sql.Append(" from ");
			sql.Append(GetPeriodicityTables(webSession));
			sql.Append(" where ");
			sql.Append(GetPeriodicityConditions(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,products));

			// group by dt.id_vehicle,vh.vehicle,prc.id_periodicity,prc.periodicity,tg.id_target,tg.target,tma.grp
			sql.AppendFormat(" group by {0}.id_vehicle, {0}.vehicle , {1}.id_periodicity,{1}.periodicity , {2}.id_target , {2}.target ,{3}.{4} "
                , DBConstantes.Tables.VEHICLE_PREFIXE
                , DBConstantes.Tables.PERIODICITY_PREFIXE
                , DBConstantes.Tables.TARGET_PREFIXE
                , DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
			sql.Append(")");
			// group by id_vehicle,vehicle,id_periodicity,periodicity,id_target,target,totalgrp,euro
            sql.AppendFormat(" group by id_vehicle, vehicle ,id_periodicity, periodicity, id_target, target, totalgrp, {0}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());

            #region GroupeBy par rappot � l'unit� choisit
            if (webSession.Unit != WebConstantes.CustomerSessions.Unit.grp)
                sql.AppendFormat(" ,{0} ", WebFunctions.SQLGenerator.GetUnitAlias(webSession));
            #endregion

			sql.Append(")");
			//			 group by id_vehicle,vehicle,id_periodicity,periodicity,id_target,target
			sql.Append(" group by  vehicle ,id_periodicity, periodicity, id_target, target ");
            sql.Append("   order by  periodicity, id_target ");

			
			#endregion

			#region execution
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.PeriodicityPlanDataAccessException("Impossible de charger les donn�es de la Periodicity Plan ",err));
			}
			#endregion

		}
		#endregion

		#region m�thodes internes

		#region select
		/// <summary>
		/// retourne la s�lection des p�riodicit�
		/// </summary>
		/// <param name="webSession">Session client</param>
		private static StringBuilder GetPeriodicitySelection(WebSession webSession){

			#region construction de la requ�te
			StringBuilder sql = new StringBuilder(1000);
			sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle," );
			sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle," );
			sql.Append(DBConstantes.Tables.PERIODICITY_PREFIXE+".id_periodicity," );
			sql.Append(DBConstantes.Tables.PERIODICITY_PREFIXE+".periodicity, " );
			sql.Append(DBConstantes.Tables.TARGET_PREFIXE+".id_target, " );
			sql.Append(DBConstantes.Tables.TARGET_PREFIXE+".target," );
            sql.AppendFormat("sum({0})* {1} as totalgrp, "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
            sql.AppendFormat("{0}.{1} as {2}, "
                , DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
            sql.AppendFormat(" sum({0}) as {1} "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());

		//	sql.Append("round(sum(Totalunite) / (sum(Totalinsert)*grp),3) as CGRP, ");
		//	sql.Append("round(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalunite)/(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalinsert)*" + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp),3)as cgrp ");

            #region s�lection par rappot � l'unit� choisit
            if (webSession.Unit != WebConstantes.CustomerSessions.Unit.grp
                && webSession.Unit != WebConstantes.CustomerSessions.Unit.euro)
                sql.AppendFormat(",{0} ", WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, DBConstantes.TableType.Type.webPlan));
            #endregion

				
			#endregion

			return sql;
		}
		#endregion

		#region from
		/// <summary>
		/// retourne la s�lection des p�riodicit�
		/// </summary>
		/// <param name="websession">Session client</param>
		/// <returns>requete</returns>
		private static StringBuilder  GetPeriodicityTables(WebSession websession){

			#region construction de la requete
			StringBuilder sql = new StringBuilder(1000);
			string dataTableName=Functions.GetAPPMWebPlanTable(websession);
			sql.Append(DBConstantes.Schema.APPM_SCHEMA+"."+dataTableName+"  "+DBConstantes.Tables.WEB_PLAN_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.APPM_SCHEMA+".WAVE "+ DBConstantes.Tables.WAVE_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.APPM_SCHEMA+".TARGET " +DBConstantes.Tables.TARGET_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".MEDIA "+DBConstantes.Tables.MEDIA_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".PERIODICITY " +DBConstantes.Tables.PERIODICITY_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".VEHICLE "+DBConstantes.Tables.VEHICLE_PREFIXE+", ");
			sql.Append(DBConstantes.Schema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT " +DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
			#endregion

			return sql;
		}
		#endregion

		#region where
		/// <summary>
		///  conditions de la requete
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		/// <param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible selectionn�e</param>
		/// <param name="products">produit selectionn�es</param>
		/// <returns>string des conditions </returns>
		private static StringBuilder GetPeriodicityConditions(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget,string products){
		
			StringBuilder sql = new StringBuilder(1000);		
			
			#region Conditions
			// Jointures
			sql.Append(DBConstantes.Tables.WAVE_PREFIXE+".id_wave="+DBConstantes.Tables.TARGET_PREFIXE+".id_wave" );
			sql.Append(" and "+ DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+VehiclesInformation.EnumToDatabaseId(ClassificationCst.DB.Vehicles.names.press));  /*"+ClassificationCst.DB.Vehicles.names.press)*/
			sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target" );
			sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".ID_MEDIA_SECODIP="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media" );
			sql.Append(" and "+ DBConstantes.Tables.MEDIA_PREFIXE+".id_media="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media" );			
			sql.Append(" and "+ DBConstantes.Tables.PERIODICITY_PREFIXE+".id_periodicity="+DBConstantes.Tables.MEDIA_PREFIXE+".id_periodicity" );			
			//sql.Append(" and "+ DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_vehicle" );				
			//langues
			sql.Append(" and "+ DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage );
			sql.Append(" and "+ DBConstantes.Tables.VEHICLE_PREFIXE+".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage );
			sql.Append(" and "+ DBConstantes.Tables.MEDIA_PREFIXE+".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBConstantes.Tables.PERIODICITY_PREFIXE+".id_language="+webSession.DataLanguage );
			sql.Append(" and "+ DBConstantes.Tables.PERIODICITY_PREFIXE+".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBConstantes.Tables.WAVE_PREFIXE+".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			
			//date s�lectionn�
			#region Table and field names
			string dateField=DBConstantes.Tables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			#endregion

			sql.Append(" and "+ dateField+">="+dateBegin.ToString());
			sql.Append(" and "+dateField+"<="+dateEnd.ToString());//date s�lectionn�
			//vague S�lectionn�e
			sql.Append(" and "+ DBConstantes.Tables.WAVE_PREFIXE+".id_wave="+idWave );
			//	Ne pas oublier de s�letionner uniquement les nomenclatures produits ou m�dia activ�s
			//Cibles
			sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_target in("+idBaseTarget.ToString()+","+ idAdditionalTarget.ToString()+")");	
			//product + univers			
			// S�lection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.WEB_PLAN_PREFIXE, true));

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE));

			//Droits clients
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE, true, module.ProductRightBranches));
            //tous les resultats sans inset
			sql.Append(" and " + DBConstantes.Tables.WEB_PLAN_PREFIXE+ ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			#endregion
			return sql;
		}
		#endregion
																																		  
		#endregion

	}
}

