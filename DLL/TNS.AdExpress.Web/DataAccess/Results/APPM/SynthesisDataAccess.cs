#region Informations
// Author: K. Shehzad 
// Date of creation: 13/07/2005 
// K.Shehzad: 05/09/2005 Table/Field names changed
#endregion

using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.DataAccess;
using Cst = TNS.AdExpress.Constantes;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM{
	/// <summary>
	/// This class is used to generate the datasets which are used in APPM Synthesis.
	/// </summary>
	public class SynthesisDataAccess{
		
		#region get general data
		/// <summary>
		///  Calculates and returns the dataset for the APPM Synthesis	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <param name="idProduct">id of the product selected from the products dropdownList 
		/// if ID product is 0 it indicates that no particular prodcuts was seleced from the products dropdownlist and the result is shown
		/// for all.
		/// </param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget, Int64 idProduct,bool mediaAgencyAccess){
			
			#region construction of the query
			
			#region variables
			StringBuilder sql = new StringBuilder(1000);
			string fieldProduct="";
			string tableProduct="";
			string joinProduct="";
			string groupByProduct="";
			//string products="";
			#endregion

			#region fields,tables and joins for single Product
			if(idProduct>1)
			{
				fieldProduct=GetFiedlsForProduct(mediaAgencyAccess);
				tableProduct=GetTablesForProduct(webSession,mediaAgencyAccess);
				joinProduct=GetJoinForProduct(webSession.DataLanguage,false,mediaAgencyAccess);
				groupByProduct=GetGroupByForProduct(mediaAgencyAccess);				
			}
			#endregion

			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion
	
			#region select

			sql.Append("select "+fieldProduct);
			sql.Append(" id_media, group_,"+ DBTables.TARGET_PREFIXE+".id_target,target, ");
			sql.Append(DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_group_,");
			sql.AppendFormat("sum({0}) as {1},sum({2}) as {3},sum({4}) as {5}, "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());
			//sql.Append("count(distinct id_media) medias, ");

			//sql.Append("tm.GRP as totalgrp, ");
			sql.AppendFormat("sum({0})*{1} as totalgrp "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			//sql.Append("ROUND(sum((TOTALUNITE)/tm.GRP),2) as grpcost");
			//sql.Append("ROUND(sum(TOTALUNITE)/(sum(TOTALINSERT)*tm.GRP),2) as grpcost");
			
			#endregion

			#region from

			sql.Append(" from "+tableProduct+" ");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.GROUP_+" "+DBTables.GROUP_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			
			#endregion

			#region where
			sql.Append(" where "+joinProduct);
			//group
			sql.Append(DBTables.GROUP_PREFIXE+".id_group_="+DBTables.WEB_PLAN_PREFIXE+".id_group_ ");
			sql.Append(" and "+DBTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and " + DBTables.GROUP_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			//media
			sql.Append(" and "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			//target
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+ additionalTarget.ToString()+")");		
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and " + DBTables.TARGET_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			//in case of multiple products
			if(idProduct<1){
				//sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,true));

				// Sélection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
					sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						


			}
			//in case of single product
			else
				sql.Append(" and "+ DBTables.WEB_PLAN_PREFIXE+".id_product in ("+idProduct.ToString()+")");
			
			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.WEB_PLAN_PREFIXE));

			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));

			#endregion

			#region groupby			
			sql.Append(" group by "+groupByProduct+ DBTables.TARGET_PREFIXE+".id_target,group_,target,GRP,"+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_group_,"+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			#endregion

			#endregion

			#region Execution of the query
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisDataAccessException("GetData:: Error while executing the query for the Synthesis APPM ",err));
			}		
			#endregion

		}

		#endregion

		#region Get Data by Version
		/// <summary>
		///  Calculates and returns the dataset for the APPM Synthesis by Vezrsion	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>		
		/// <param name="idVersion">Version ID</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>dataset for synthesis by version of APPM</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget, bool mediaAgencyAccess,string idVersion){
		
			#region construction of the query
			
			#region Variables
			StringBuilder sql = new StringBuilder(1000);
			string fieldProduct="";
			string tableProduct="";
			string joinProduct="";
			string groupByProduct="";			
			#endregion

			#region fields,tables and joins for single Version
			if(idVersion!=null && idVersion.Length>0) {
				fieldProduct=GetFiedls(mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);
				tableProduct=GetTables(webSession,mediaAgencyAccess);
				joinProduct=GetJoin(webSession.DataLanguage,false,mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);
				groupByProduct=GetGroupBy(mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);				
			}
			#endregion

			#region Table and field names
			string dateField=DBTables.DATA_PRESS_APPM_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=DBTables.DATA_PRESS_APPM;
			#endregion
	
			#region select

			sql.Append("select "+fieldProduct);
			sql.Append(" id_media, group_,"+ DBTables.TARGET_PREFIXE+".id_target,target, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_group_,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_slogan,");
			sql.AppendFormat("sum({0}) as {1},sum({2}) as {3},sum({4}) as {5}, "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());					
			sql.AppendFormat("sum({0})*{1}.{2} as totalgrp "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			
			#endregion

			#region from

			sql.Append(" from "+tableProduct+" ");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.GROUP_+" "+DBTables.GROUP_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.DATA_PRESS_APPM_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			
			#endregion

			#region where
			sql.Append(" where "+joinProduct);

			sql.Append(" "+DBTables.DATA_PRESS_APPM_PREFIXE+".id_slogan is not null  ");

			//group
			sql.Append(" and "+DBTables.GROUP_PREFIXE+".id_group_="+DBTables.DATA_PRESS_APPM_PREFIXE+".id_group_ ");
			sql.Append(" and "+DBTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and " + DBTables.GROUP_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			//media
			sql.Append(" and "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_media");
			//target
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+ additionalTarget.ToString()+")");		
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and " + DBTables.TARGET_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
		

			sql.Append(" and date_media_num >="+dateBegin);
			sql.Append(" and date_media_num <=" + dateEnd);

			//Version
			sql.Append(" and " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_slogan in ("+idVersion+") ");

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.DATA_PRESS_APPM_PREFIXE));

			//all results without inset
			sql.Append(" and " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_inset is null ");
			
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));

			#endregion

			#region groupby			
			sql.Append(" group by "+groupByProduct+ DBTables.TARGET_PREFIXE+".id_target,group_,target,GRP,"+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_group_,"+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_media,"+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_slogan");
			#endregion

			#endregion

			#region Execution of the query
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisDataAccessException("GetData:: Error while executing the query for the Synthesis by Version for APPM ",err));
			}		
			#endregion
		}

		#endregion

		#region Get Data by a list of Versions
		/// <summary>
		///  Calculates and returns the dataset for the APPM Synthesis by a list of Versions	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>			
		/// <param name="versions">List of Versions ID</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>dataset for synthesis by version of APPM</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget, bool mediaAgencyAccess,ICollection versions) {
		
			#region construction of the query
			
			#region Variables
			StringBuilder sql = new StringBuilder(1000);
			string fieldProduct="";
			string tableProduct="";
			string joinProduct="";
			string groupByProduct="";		
			string versionCondition="";
			int nbVersion=0;
			bool first=true;
			#endregion

			string ids = string.Empty;			

			#region nouvelle version avec max 999 éléments par requete
			foreach(Object o in versions) {

				ids += o.ToString() + ",";
				nbVersion++;

				if(nbVersion==999)
				{//Limitation à 999 versions par condition
					if (ids.Length > 0) {
						ids = ids.Substring(0, ids.Length-1);
					}
					if(!first)versionCondition+= " or ";
					versionCondition+= DBTables.DATA_PRESS_APPM_PREFIXE + ".id_slogan in ( "+ids+" ) ";
					first=false;
					ids=string.Empty;
					nbVersion=0;
				}
			}

			if (ids.Length > 0 || versionCondition.Length>0) {
				if (ids.Length > 0 ) {
					ids = ids.Substring(0, ids.Length-1);
					if(!first)versionCondition+= " or ";
					versionCondition+= DBTables.DATA_PRESS_APPM_PREFIXE + ".id_slogan in ( "+ids+" ) ";
				}
			}
			else {
				return null;
			}
			#endregion

			#region fields,tables and joins for single Version
			//if(idVersion!=null && idVersion.Length>0) {
				fieldProduct=GetFiedls(mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);
				tableProduct=GetTables(webSession,mediaAgencyAccess);
				joinProduct=GetJoin(webSession.DataLanguage,false,mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);
				groupByProduct=GetGroupBy(mediaAgencyAccess,webSession,DBTables.DATA_PRESS_APPM_PREFIXE);				
			//}
			#endregion

			#region Table and field names
			string dateField=DBTables.DATA_PRESS_APPM_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=DBTables.DATA_PRESS_APPM;
			#endregion
	
			#region select

			sql.Append("select "+fieldProduct);
			sql.Append(" id_media, group_,"+ DBTables.TARGET_PREFIXE+".id_target,target, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_group_,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_slogan,");
            sql.AppendFormat("sum({0}) as {1},sum({2}) as {3},sum({4}) as {5}, "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());					
			sql.AppendFormat("sum({0})*{1}.{2} as totalgrp "
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			
			#endregion

			#region from

			sql.Append(" from "+tableProduct+" ");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.GROUP_+" "+DBTables.GROUP_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.DATA_PRESS_APPM_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
			
			#endregion

			#region where
			sql.Append(" where "+joinProduct);

			sql.Append(" "+DBTables.DATA_PRESS_APPM_PREFIXE+".id_slogan is not null  ");

			//group
			sql.Append(" and "+DBTables.GROUP_PREFIXE+".id_group_="+DBTables.DATA_PRESS_APPM_PREFIXE+".id_group_ ");
			sql.Append(" and "+DBTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and " + DBTables.GROUP_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			//media
			sql.Append(" and "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_media");
			//target
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+ additionalTarget.ToString()+")");		
			sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and " + DBTables.TARGET_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);


			sql.Append(" and date_media_num >=" + dateBegin);
			sql.Append(" and date_media_num <=" + dateEnd);

			//Version
			

			sql.Append(" and (" + versionCondition + ")");


			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.DATA_PRESS_APPM_PREFIXE));


			//all results without inset
			sql.Append(" and " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_inset is null ");
			
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));

			#endregion

			#region groupby			
			sql.AppendFormat(" group by {0}{1}.id_target,group_,target,{2},{3}.id_group_,{3}.id_media,{3}.id_slogan"
                , groupByProduct
                , DBTables.TARGET_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField
                , DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE);
			#endregion

			#endregion

			#region Execution of the query
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err) {
				throw(new WebExceptions.SynthesisDataAccessException("GetData:: Error while executing the query for the Synthesis by Version for APPM ",err));
			}		
			#endregion
		}

		#endregion
		
		#region Group or competitor product investment for calculating pdv

		

		/// <summary>
		/// calculates the investment of the group for calculating PDV by Version
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">start Date</param>
		/// <param name="dateEnd">End Date</param>
		/// <param name="baseTarget">Target of the base</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="idGroup">id of the group for which pdv is to be calculated</param>		
		/// <returns>Dataset containing investment of the group or competitor univers</returns>
		public static DataSet UniversGroupInvestment(WebSession webSession,IDataSource dataSource, int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget,string idGroup)
		{
			
			#region variables

			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=null;			
			dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion

			#region construction of the query

			StringBuilder sql = new StringBuilder(1000);

			#region select
			sql.AppendFormat("select sum({0}) as {1}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());
			#endregion
			
			#region from

			sql.Append(" from ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+"  ");
			//sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");

			#endregion

			#region where

			sql.Append(" where ");
			//get joins and other conditions for synthesis
			sql.Append(DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			//target
			//sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+baseTarget.ToString()+")");		
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			//sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			//with respect to comptetitor univers
			//if(webSession.CompetitorUniversAdvertiser.Count>1){
			//    webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[2]).TreeCompetitorAdvertiser;
			//    sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,true));			
			//}
			// Sélection product of comptetitor universe
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 1)
				sql.Append(webSession.PrincipalProductUniverses[1].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						


			//with respect to groups
			else{
				sql.Append(" and "+ DBTables.WEB_PLAN_PREFIXE+".id_group_ in ("+idGroup+")");	
			}



			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBTables.WEB_PLAN_PREFIXE));

			
				sql.Append(" and "+dateField+" >="+dateBegin);
				sql.Append(" and "+dateField+" <="+dateEnd);
		

			//all results without inset			
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));

			
			#endregion

			#endregion

			#region Execution of the query

			try
			{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisDataAccessException("UniversGroupInvestment:: Error while executing the query for the Synthesis APPM ",err));
			}

			#endregion						

		}		
		
		#endregion

		#region Investment by Version or  product
		
		/// <summary>
		/// Obtient l'investissement en fonction d'un produit.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">Source de données</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="baseTarget">Cible de base</param>	
		/// <param name="idProduct">ID produit</param>
		/// <returns>Investissement en euros</returns>
		public static DataSet GetProductInvestment(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,string idProduct){
			
			#region variables

			string dateField=DBTables.DATA_PRESS_APPM_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
	
			#endregion

			#region construction of the query

			StringBuilder sql = new StringBuilder(1000);

			#region select
            sql.AppendFormat("select sum({0}) as {1}"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString());
			#endregion
			
			#region from

			sql.Append(" from ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.DATA_PRESS_APPM_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+"  ");
			//sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");

			#endregion

			#region where

			sql.Append(" where ");

			//get joins and other conditions for synthesis
			sql.Append(DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE+".id_media");
			
			//target		
			sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+baseTarget.ToString()+")");		
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);			
			//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
			
			// product
			if(idProduct!=null && idProduct.Length>0)
			sql.Append(" and "+ DBTables.DATA_PRESS_APPM_PREFIXE+".id_product in ("+idProduct+")");	
						
			//Period
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBConstantes.Tables.DATA_PRESS_APPM_PREFIXE));			

			//all results without inset
			sql.Append(" and " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));

			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true));
			
			#endregion

			#endregion

			#region Execution of the query

			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisDataAccessException("GetInvestment:: Error while executing the query for the Version Synthesis APPM ",err));
			}

			#endregion						
		}
		#endregion

		#region Private methods

		/// <summary>
		/// Obtient les champs nécessaires à l'affichage d'un produit
		/// </summary>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>Champs nécessaires à l'affichage d'un produit</returns>
		private static string GetFiedlsForProduct(bool mediaAgencyAccess){
			string fields=string.Empty;
			fields+="product,advertiser,";
			if(mediaAgencyAccess)
				fields+="advertising_agency,";
			return fields;
		}

		/// <summary>
		/// Obtient les champs nécessaires à l'affichage d'une synthèse de version
		/// </summary>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="tablePrefixe">Prefixe Table </param>
		/// <returns>Champs nécessaires à l'affichage d'un produit</returns>
		private static string GetFiedls(bool mediaAgencyAccess,WebSession webSession,string tablePrefixe){
			string fields=string.Empty;
			fields+=" distinct "+tablePrefixe+".id_product,product,advertiser,";
			if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				fields+="brand,";
			if(mediaAgencyAccess)
				fields+="advertising_agency,";
			return fields;
		}

		/// <summary>
		/// Obtient les tables pour le détail du produit
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>Les tables pour le détail du produit</returns>
		private static string GetTablesForProduct(WebSession webSession,bool mediaAgencyAccess){
			string sql="";
			sql+=DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.ADVERTISER+" "+DBTables.ADVERTISER_PREFIXE+", ";
			sql+=DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.PRODUCT+" "+DBTables.PRODUCT_PREFIXE+", ";
			if(mediaAgencyAccess){
				//sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+", ";
				sql += DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBTables.ADVERTISING_AGENCY + " " + DBTables.ADVERTISING_AGENCY_PREFIXE + ", ";

			}
			//sql+=DBSchema.ADEXPRESS_SCHEMA+"."+", ";
					
			return(sql);
		
		}

		/// <summary>
		/// Obtient les tables pour le détail du produit par version
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>Les tables pour le détail du produit</returns>
		private static string GetTables(WebSession webSession,bool mediaAgencyAccess){
			string sql="";
			sql+=DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.ADVERTISER+" "+DBTables.ADVERTISER_PREFIXE+", ";
			sql+=DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.PRODUCT+" "+DBTables.PRODUCT_PREFIXE+", ";
			if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
			sql+=DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.BRAND+" "+DBTables.BRAND_PREFIXE+", ";
			if(mediaAgencyAccess){
				//sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+", ";
				sql += DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBTables.ADVERTISING_AGENCY + " " + DBTables.ADVERTISING_AGENCY_PREFIXE + ", ";

			}			
					
			return(sql);
		
		}

		/// <summary>
		/// Obtient les joins pour le détail du produit
		/// </summary>
		/// <param name="languageId">language id</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <param name="beginByAnd">booleand to indicate whether to start by and or not</param>
		/// <returns></returns>
		private static string GetJoinForProduct(int languageId,bool beginByAnd,bool mediaAgencyAccess){
			string sql="";
			if(beginByAnd)sql+="and ";
			sql+=DBTables.PRODUCT_PREFIXE+".id_product="+DBTables.WEB_PLAN_PREFIXE+".id_product ";
			sql+=" and "+DBTables.PRODUCT_PREFIXE+".id_language="+languageId.ToString();
			sql+=" and "+DBTables.ADVERTISER_PREFIXE+".id_advertiser="+DBTables.WEB_PLAN_PREFIXE+".id_advertiser ";
			sql+=" and "+DBTables.ADVERTISER_PREFIXE+".id_language="+languageId.ToString();
			if(mediaAgencyAccess){
				sql += " and " + DBConstantes.Tables.ADVERTISING_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY(+)=" + DBTables.WEB_PLAN_PREFIXE + ".ID_ADVERTISING_AGENCY";
				sql += " and " + DBConstantes.Tables.ADVERTISING_AGENCY_PREFIXE + ".id_language(+)=" + DBConstantes.Language.FRENCH;
			}
			sql+=" and ";
			return(sql);
		
		}

		/// <summary>
		/// Obtient les joins pour le détail du produit par version
		/// </summary>
		/// <param name="languageId">language id</param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <param name="beginByAnd">booleand to indicate whether to start by and or not</param>
		/// <param name="webSession">Session of the client</param>
		/// <param name="tablePrefixe">Prefixe Table </param>
		/// <returns></returns>
		private static string GetJoin(int languageId,bool beginByAnd,bool mediaAgencyAccess,WebSession webSession,string tablePrefixe){
			string sql="";
			if(beginByAnd)sql+="and ";
			sql+=DBTables.PRODUCT_PREFIXE+".id_product="+tablePrefixe+".id_product ";
			sql+=" and "+DBTables.PRODUCT_PREFIXE+".id_language="+languageId.ToString();
			sql+=" and "+DBTables.ADVERTISER_PREFIXE+".id_advertiser="+tablePrefixe+".id_advertiser ";
			sql+=" and "+DBTables.ADVERTISER_PREFIXE+".id_language="+languageId.ToString();
			if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE)){
				sql+=" and "+DBTables.BRAND_PREFIXE+".id_brand="+tablePrefixe+".id_brand ";
				sql+=" and "+DBTables.BRAND_PREFIXE+".id_language="+languageId.ToString();
			}
			if(mediaAgencyAccess){
				sql += " and " + DBConstantes.Tables.ADVERTISING_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY(+)=" + tablePrefixe + ".ID_ADVERTISING_AGENCY";
				sql += " and " + DBConstantes.Tables.ADVERTISING_AGENCY_PREFIXE + ".id_language(+)=" + DBConstantes.Language.FRENCH;
			}
			sql+=" and ";
			return(sql);
		
		}

		/// <summary>
		/// Obtient les champs nécessaires à l'affichage d'un produit
		/// </summary>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>Champs nécessaires à l'affichage d'un produit</returns>
		private static string GetGroupByForProduct(bool mediaAgencyAccess){
			string groupBy=string.Empty;
			groupBy+="product,advertiser,";
			if(mediaAgencyAccess)
				groupBy+="advertising_agency,";
			return groupBy;
		}

		/// <summary>
		/// Obtient les champs nécessaires à l'affichage d'un produit par version
		/// </summary>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <param name="tablePrefixe"> Prefixe Table</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Champs nécessaires à l'affichage d'un produit</returns>
		private static string GetGroupBy(bool mediaAgencyAccess,WebSession webSession,string tablePrefixe){
			string groupBy=string.Empty;
			groupBy+="product,"+tablePrefixe+".id_product,advertiser,";
			if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				groupBy+="brand,";
			if(mediaAgencyAccess)
				groupBy+="advertising_agency,";
			return groupBy;
		}

		#endregion		
		
	}
}
