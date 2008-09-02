#region Informations
// Author: Y. R'kaina 
// Date of creation: 17/01/2007 
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
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM{
	/// <summary>
	/// This class is used to generate the datasets which are used in Sector Data Average.
	/// </summary>
	public class SectorDataAverageDataAccess{
	
		#region get general data
		/// <summary>
		///  Calculates and returns the dataset for the Sector Data Synthesis	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetData(WebSession webSession,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region construction of the query
			
			#region variables
			StringBuilder sql = new StringBuilder(1000);
			string sectorPrefixe=DBTables.WEB_PLAN_PREFIXE, subsectorPrefixe=DBTables.WEB_PLAN_PREFIXE, groupPrefixe=DBTables.WEB_PLAN_PREFIXE;
			string productDetail = "";
			#endregion

			#region Initialisation du productDetail
			if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser)
				productDetail="id_advertiser";
			else if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand)
				productDetail="id_brand";
			else if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product)
				productDetail="id_product";
			#endregion

			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion

			#region from
//			sql.Append(" from ( ");
			#endregion

				#region select
				sql.Append("select "+productDetail+", count(id_media) as nbMedia,id_target,target, ");
                sql.AppendFormat("sum({0}) as {0},sum({1}) as {1},sum({2}) as {2}, "
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());
				sql.Append("sum(totalgrp) as totalgrp");
				#endregion

				#region from
				sql.Append(" from ( ");
				#endregion

					#region select
					sql.Append("select "+productDetail+", id_media, id_target,target, ");
					sql.AppendFormat("sum({0}) as {0},sum({1}) as {1},sum({2}) as {2}, "
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                        , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());
					sql.Append("sum(totalgrp) as totalgrp ");
					#endregion

					#region from
					sql.Append(" from ( ");
					#endregion
		
						#region select
						sql.Append("select "+productDetail+", id_media, "+ DBTables.TARGET_PREFIXE+".id_target,target, ");
                        sql.AppendFormat("sum({0}) as {1},sum({2}) as {3},sum({4}) as {5}, sum({2})*{6} as totalgrp "
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseMultimediaField
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseMultimediaField
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
						#endregion

						#region from
						sql.Append(" from ");
						sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
						sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+", ");
						sql.Append(DBSchema.APPM_SCHEMA+".TARGET "+ DBTables.TARGET_PREFIXE+" ");
						#endregion

						#region where
						sql.Append(" where ");
						//media
						sql.Append(DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
						//target
						sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
						sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_target in("+baseTarget.ToString()+","+ additionalTarget.ToString()+")");		
						sql.Append(" and "+ DBTables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
						sql.Append(" and " + DBTables.TARGET_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

						//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
						sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
						//products
						// Sélection de Produits
						if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
							sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
										
						sql.Append(" and "+dateField+" >="+dateBegin);
						sql.Append(" and "+dateField+" <="+dateEnd);
						//all results without inset
						sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
						
						//media rights
						sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
						//product rights
						sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
						#endregion

						#region group by
						sql.Append(" group by "+ DBTables.TARGET_PREFIXE+".id_target, target, "+DBConstantes.Tables.WEB_PLAN_PREFIXE+"."+productDetail+", id_media, " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
						#endregion

					#region group by
					sql.Append(" ) ");
					sql.Append(" group by id_target, target, "+productDetail+", id_media");
					#endregion

				#region group by
				sql.Append(" ) ");
				sql.Append(" group by "+productDetail+", id_target, target");
				#endregion

			#endregion

			#region Execution of the query
			try{
				return(webSession.Source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataAverageDataAccessException("GetData: Error while executing the query for the Sector Data Average",err));
			}		
			#endregion
		}
		#endregion

		#region get active week number
		/// <summary>
		///  Calculates and returns the dataset for the Sector Data Synthesis	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetPresenceDuration(WebSession webSession,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region construction of the query

			#region variables
			StringBuilder sql = new StringBuilder(1000);
			string sectorPrefixe=DBTables.WEB_PLAN_PREFIXE, subsectorPrefixe=DBTables.WEB_PLAN_PREFIXE, groupPrefixe=DBTables.WEB_PLAN_PREFIXE;
			string productDetail = "";
			#endregion

			#region Initialisation du productDetail
			if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser)
				productDetail="id_advertiser";
			else if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand)
				productDetail="id_brand";
			else if(webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product)
				productDetail="id_product";
			#endregion

			#region Table and field names
			string dateField="week_media_num";
			string dataTableName = DBConstantes.Tables.WEB_PLAN_APPM_WEEK;
			#endregion

			#region select
			sql.Append("select round(avg(nbWeek),3) as avgNbWeek, round(min(nbWeek),3) as minNbWeek, round(max(nbWeek),3) as maxNbWeek, ");
			sql.Append("round(avg(GRP_Week),3) as avgGRP_Week, round(min(GRP_Week),3) as minGRP_Week, round(max(GRP_Week),3) as maxGRP_Week");
			#endregion

			#region from
			sql.Append(" from ( ");
			#endregion

				#region select
				sql.Append("select "+productDetail+", count("+dateField+") as nbWeek,");
				sql.Append("sum(totalgrp) as totalgrp, round(sum(totalgrp)/count("+dateField+"),3) as GRP_Week ");
				#endregion

				#region from
				sql.Append(" from ( ");
				#endregion

					#region select
					sql.Append("select "+productDetail +", "+dateField+", ");
					sql.Append("sum(totalgrp) as totalgrp ");
					#endregion

					#region from
					sql.Append(" from ( ");
					#endregion

						#region select
						sql.Append("select "+productDetail +", "+dateField+", ");
						sql.AppendFormat("sum({0})*{1} as totalgrp "
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                            , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
						#endregion

						#region from
						sql.Append(" from ");
						sql.Append(DBSchema.APPM_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE+", ");
						sql.Append(DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
						#endregion

						#region where
						sql.Append(" where ");
						//media
						sql.Append(DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
						//target
						sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+ additionalTarget.ToString()+")");		
						
						//sql.Append(" and "+ DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.DataLanguage);
						sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
						// Sélection de Produits
						if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
							sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
											
						sql.Append(" and "+dateField+" >="+dateBegin);
						sql.Append(" and "+dateField+" <="+dateEnd);
						//all results without inset
						sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
								
						//media rights
						sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
						//product rights
						sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
						#endregion

						#region group by
                        sql.Append(" group by " + DBConstantes.Tables.WEB_PLAN_PREFIXE + "." + productDetail + ", " + dateField + ", " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
						#endregion

					#region group by
					sql.Append(" ) ");
					sql.Append(" group by "+productDetail+", "+dateField);
					#endregion

				#region group by
				sql.Append(" ) ");
				sql.Append(" group by "+productDetail+")");
				#endregion

			#endregion

			#region Execution of the query
			try{
				return(webSession.Source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataAverageDataAccessException("GetData: Error while executing the query for the Sector Data presence duration",err));
			}		
			#endregion

		}
		#endregion

	}
}
