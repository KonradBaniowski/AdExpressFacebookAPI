#region Informations
// Author: K. Shehzad 
// Date of creation: 01/08/2005 
// K.Shehzad: 05/09/2005 Table/Field names changed
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using Cst = TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.DataAccess;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// This class calculates and returns the dateset for the Analysis of PDV of the Plan.
	/// </summary>
	public class PDVPlanDataAccess{

		#region General Data for PDV Plan Analysis table
		/// <summary>
		///  Calculates and returns the dataset for the APPM PDV Plan	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool forCompetitorUniverse)
		{
			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			#endregion

			#region variables
			StringBuilder sql = new StringBuilder(1000);
			#endregion

			#region Construction of the query
			 
			#region select
			sql.Append(" select distinct id_target,target");
			if (showProduct) sql.Append(" ,id_product,product ");
            sql.AppendFormat(" ,id_group_,sum({0}) as {0},sum({1}) as {1},sum({2}) as {2},sum(totalgrp) as {3}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
			#endregion

			#region from
			sql.Append(" from (");

			#region select
			sql.Append("select distinct ");
			if (showProduct) sql.Append(DBTables.WEB_PLAN_PREFIXE + ".id_product,product,");
			sql.Append("  "+DBTables.WEB_PLAN_PREFIXE+".id_group_,");
			sql.Append(DBTables.TARGET_PREFIXE+".id_target,target");
            sql.AppendFormat(",sum({0}) as {1},sum({2}) as {3},sum({4}) as {5},sum({4})*{6} as totalgrp"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
	     	#endregion

			#region from
			sql.Append(" from ");			
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
			if (showProduct) sql.Append(DBSchema.ADEXPRESS_SCHEMA + "." + DBTables.PRODUCT + " " + DBTables.PRODUCT_PREFIXE + ", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			#endregion

			#region where
			sql.Append(" where ");
			if (showProduct){
				sql.Append(DBTables.PRODUCT_PREFIXE + ".id_product=" + DBTables.WEB_PLAN_PREFIXE + ".id_product ");			
				sql.Append(" and " + DBTables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage);
				sql.Append(" and " + DBTables.PRODUCT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			}
			if (showProduct) sql.Append(" and ");
			sql.Append("  "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and " + DBTables.TARGET_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+additionalTarget.ToString()+")");			
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			
			// Sélection de Produits
			if (forCompetitorUniverse && webSession.PrincipalProductUniverses.Count > 1) {
				sql.Append(webSession.PrincipalProductUniverses[1].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
			}
			else if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
			
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.WEB_PLAN_PREFIXE));

			//Rights
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBTables.WEB_PLAN_PREFIXE,true));
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE, true, module.ProductRightBranches));
			
			#endregion

			#region groupby
			sql.Append(" group by ");
			if (showProduct) sql.Append(DBTables.WEB_PLAN_PREFIXE + ".id_product,product,");
			sql.Append(" grp,id_group_,"+DBTables.TARGET_PREFIXE+".id_target,target)");
			
			#endregion

			#endregion

			#region group by
			sql.Append("group by  ");
			if (showProduct) sql.Append("  id_product,product,");
			sql.Append("  id_group_,id_target,target");
			#endregion

			#region order by
			if (showProduct) sql.Append(" order by product");
			#endregion

			#endregion

			#region Execution of the Query
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.PDVPlanDataAccessExcpetion("GetData:: Error while executing the query for PDV PLan  ",err));
			}		
			#endregion


		}
		#endregion

		#region Group investment for calculating pdv
		/// <summary>
		/// calculates the investment of the group for calculating PDV
		/// </summary>
		/// <param name="webSession">Clients Session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">start Date</param>
		/// <param name="dateEnd">End Date</param>
		/// <param name="baseTarget">Target of the base</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="idGroup">id of the group for which pdv is to be calculated</param>
		/// <returns>Dataset containing investment of the group or competitor univers</returns>
		public static DataSet UniversgroupInvestment(WebSession webSession,IDataSource dataSource, int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget,string idGroup)
		{
			
			#region variables
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion

			#region construction of the query

			StringBuilder sql = new StringBuilder(1000);

            #region select
            sql.Append(" select distinct id_target,target");          
            sql.AppendFormat(" ,sum({0}) as {0},sum({1}) as {1},sum({2}) as {2},sum(totalgrp) as {3}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());

            sql.Append(" from (");
            #endregion

			#region select
			sql.Append(" select "+DBTables.TARGET_PREFIXE+".id_target,target,");
            sql.AppendFormat(" sum({0}) as {1},sum({2}) as {3},sum({4}) as {5},sum({4})*{6} as totalgrp "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
			#endregion
			
			#region from
			sql.Append(" from ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			#endregion

			#region where

			sql.Append(" where ");
			//get joins and other conditions for synthesis
			sql.Append(DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			//target
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+additionalTarget.ToString()+")");		
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBTables.TARGET_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+ DBTables.WEB_PLAN_PREFIXE+".id_group_ in ("+idGroup+")");	
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.WEB_PLAN_PREFIXE));

			//Rights
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBTables.WEB_PLAN_PREFIXE,true));
            //TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            //sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE, true, module.ProductRightBranches));

			#endregion

			#region groupby
			sql.Append(" group by "+DBTables.TARGET_PREFIXE+".id_target,target,grp");
			#endregion

            #region group by
            sql.Append("  ) group by  id_target,target ");           
            #endregion

			#endregion

			#region Execution of the query

			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisDataAccessException("UniversgroupInvestment:: Error while executing the query for PDV PLan ",err));
			}

			#endregion						

		}
		
		#endregion

		#region Data for Graphics
		/// <summary>
		///  Calculates and returns the dataset for the APPM PDV Plan Graphics	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="additionalTarget">additional Target</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetGraphicsData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 additionalTarget)
		{
			#region variables
			StringBuilder sql = new StringBuilder(1000);
			//string advertisersSelected=string.Empty;
			//string productsSelected=string.Empty;
			//string brandsSelected=string.Empty;
			string tables=string.Empty;
			string fields=string.Empty;
			string joins=string.Empty;
			string groupby=string.Empty;
			#endregion

			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion
			
			#region tables, fields etc. selected by client
			fields=GetFields(webSession,false);
			tables = GetTables(webSession);
			joins=GetJoins(webSession);
			#endregion

			#region Construction of the query

			#region select
			sql.Append("select "+fields);
            sql.AppendFormat(" ,sum({0}) as {0},sum({1}) as {1},sum({2}) as {2},sum(totalgrp) as {3}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString());
			#endregion

			#region from
			sql.Append(" from (");

			#region select
			fields = GetFields(webSession,true);
			sql.Append(" select "+fields);
            sql.AppendFormat(",sum({0}) as {1},sum({2}) as {3},sum({4}) as {5},sum({4})*{6} as totalgrp"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			#endregion

			#region from
			sql.Append(" from "+tables);			
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			#endregion

			#region where
			sql.Append(" where "+ joins);
			sql.Append(" and "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+additionalTarget.ToString()+")");			
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBTables.TARGET_PREFIXE +".activation < "+  DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
		
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.WEB_PLAN_PREFIXE));

			//Rights
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBTables.WEB_PLAN_PREFIXE,true));
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE, true, module.ProductRightBranches));
            #endregion

			#region group by
			sql.Append(" group by "+fields);
            sql.AppendFormat(", {0})", UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			#endregion
            
			#endregion

			#region group by
			fields=GetFields(webSession,false);
			sql.Append("group by  "+fields);
			#endregion

			#region order by
			//sql.Append(" order by ");
			#endregion


			#endregion

			#region Execution of the query
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.PDVPlanDataAccessExcpetion("GetGraphicsData:: Error while executing the query for PDV PLan ",err));
			}		
			#endregion
		}

	
		#endregion

		#region Private methods

		#region get fields
		/// <summary>
		/// This method returns the fields required for the selected univers
		/// </summary>
		/// <param name="webSession">Client session</param>	
		/// <param name="innerSelect">boolean that indicates that whether the fields are to be selected for
		/// inner select or the outer one</param>
		/// <returns>string of required fields</returns>
		private static string GetFields(WebSession webSession, bool innerSelect)
		{
			string fields=string.Empty;
			string webPlanPrefixe=string.Empty;
			List<NomenclatureElementsGroup> nomenclatureGroups = null;
			int counter = 0;
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			if(innerSelect)
				webPlanPrefixe=DBTables.WEB_PLAN_PREFIXE+".";
			
			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)) {
				fields = webPlanPrefixe + "id_advertiser,advertiser";
				counter++;
			}
			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes)) {			
				fields = webPlanPrefixe + "id_brand,brand";
				counter++;
			}
			if (showProduct && (counter>1 || (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes)))) {
				fields = webPlanPrefixe+"id_product,product";
			}
			
			return fields;

		}
		#endregion

		#region get tables
		/// <summary>
		/// This method returns the tables required for the selected univers
		/// </summary>
		/// <param name="webSession">Client session</param>
		/// <returns>string of required tables</returns>
		private static string GetTables(WebSession webSession)
		{
			string tables=string.Empty;
			int counter = 0;
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			 if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)) {
				tables = DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.ADVERTISER+" "+DBTables.ADVERTISER_PREFIXE+",";
				counter++;
			}
			
			 if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes)) {
				tables = DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.BRAND+" "+DBTables.BRAND_PREFIXE+",";
				counter++;
			}
			if (showProduct && (counter > 1 || (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes)))) {
				tables = DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.PRODUCT+" "+DBTables.PRODUCT_PREFIXE+", ";
			}
			
			return tables;

		}
		#endregion

		#region get joins
		/// <summary>
		/// This method returns the join conditions required for the selected univers
		/// </summary>		
		/// <param name="webSession">Client Session</param>
		/// <returns>string of required joins</returns>
		private static string GetJoins(WebSession webSession)
		{
			string joins=string.Empty;
			int counter = 0;
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)) {
				joins = DBTables.ADVERTISER_PREFIXE + ".id_advertiser=" + DBTables.WEB_PLAN_PREFIXE + ".id_advertiser ";
				joins += " and " + DBTables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage;
				counter++;
			}
			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes)) {
				joins = DBTables.BRAND_PREFIXE + ".id_brand=" + DBTables.WEB_PLAN_PREFIXE + ".id_brand ";
				joins += " and " + DBTables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage;
				counter++;
			}
			if (showProduct && (counter > 1 ||  (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes)))) {
				joins =DBTables.PRODUCT_PREFIXE+".id_product="+DBTables.WEB_PLAN_PREFIXE+".id_product ";
				joins+=" and "+DBTables.PRODUCT_PREFIXE+".id_language="+webSession.DataLanguage;
				
			}
			
			return joins;

		}
		#endregion

		#region groupby
		/// <summary>
		/// Fields method is used instead of this method for the time being as the same fields are required for the groupby condition
		/// This method returns the groupby fields required for the selected univers
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <returns>string of required group by fields</returns>
		private static string GetGroupBy(WebSession webSession)
		{
			string groupBy=string.Empty;
			int counter = 0;
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)) {
				groupBy = DBTables.WEB_PLAN_PREFIXE + ".id_advertiser,advertiser";
				counter++;
			}
			if (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes)) {
				groupBy = DBTables.WEB_PLAN_PREFIXE + ".id_brand,brand";
				counter++;
			}
			if (showProduct && (counter > 1 || (webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes)))) {
				groupBy = DBTables.WEB_PLAN_PREFIXE+".id_product,product";
			}
			
			return groupBy;

		}
		#endregion


		#endregion
	}
}
