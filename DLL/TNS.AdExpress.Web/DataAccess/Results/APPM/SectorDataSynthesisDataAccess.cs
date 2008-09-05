#region Informations
// Author: Y. R'kaina 
// Date of creation: 15/01/2007 
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
	/// This class is used to generate the datasets which are used in Sector Data Synthesis.
	/// </summary>
	public class SectorDataSynthesisDataAccess{

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
			#endregion

			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string dataTableName=Functions.GetAPPMWebPlanTable(webSession);
			#endregion
	
			#region select
			sql.Append("select id_media, id_advertiser, id_brand, id_product, "+ DBTables.TARGET_PREFIXE+".id_target,target, ");
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
			//sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession,"","","","",sectorPrefixe,subsectorPrefixe,groupPrefixe,true));
			
			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));						
						
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE));

			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBTables.WEB_PLAN_PREFIXE, true));
			#endregion

			#region group by
            sql.Append(" group by " + DBTables.TARGET_PREFIXE + ".id_target, target, " + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_media, " + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_advertiser, " + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_brand, " + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_product, " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			#endregion

			#endregion

			#region Execution of the query
			try{
				return(webSession.Source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataSynthesisDataAccessException("GetData: Error while executing the query for the Sector Data Synthesis",err));
			}		
			#endregion
		}
		#endregion
	}
}
