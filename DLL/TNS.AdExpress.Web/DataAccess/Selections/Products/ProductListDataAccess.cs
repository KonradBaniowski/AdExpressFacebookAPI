#region information
// Author: Khurram Shehzad
// Date of Creation: 18/07/2005
// 29/11/2005 Par B.Masson > Mise en place de IDataSource pour Execution de la requête
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Data;
using System.Text;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using Cst = TNS.AdExpress.Constantes;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// This class is used to construct the list of the selected products whose results can be seen in the APPM synthesis
	/// </summary>
	public class ProductListDataAccess{

		/// <summary>
		/// This method constructs the list of the products to be shown in the synthesis product dropdownlist
		/// </summary>
		/// <param name="webSession">webSession of the client</param>	
		/// <returns>returns the dataset containing the selected products</returns>		
		public static DataSet getProductList(TNS.AdExpress.Web.Core.Sessions.WebSession webSession){

			#region Variables
			DataSet dsListProduct=null;
			StringBuilder sql=new StringBuilder(1500);		
			#endregion

			#region Dates
			int dateBegin = int.Parse(webSession.PeriodBeginningDate);
			int dateEnd = int.Parse(webSession.PeriodEndDate);				
			#endregion

			#region Targets
			//base target
			//Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerRightConstante.type.aepmBaseTargetAccess));
			//additional target
			Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerRightConstante.type.aepmTargetAccess));									
			#endregion

			#region Tables and fields
			string dateField=DBConstantes.Tables.WEB_PLAN_PREFIXE+"."+Functions.GetDateFieldWebPlanTable(webSession);
			string webPlanMonthWeek=Functions.GetAPPMWebPlanTable(webSession);
			#endregion	
			
			#region Construction of the query

			#region select
			//select
			sql.Append("select distinct "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,product");
			#endregion

			#region from
			//from
			sql.Append(" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".product "+ DBConstantes.Tables.PRODUCT_PREFIXE);
			sql.Append(", "+TNS.AdExpress.Constantes.DB.Schema.APPM_SCHEMA+"."+ webPlanMonthWeek+" "+DBConstantes.Tables.WEB_PLAN_PREFIXE);
			sql.Append(", "+DBSchema.APPM_SCHEMA+".TARGET_MEDIA_ASSIGNMENT "+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE);
			//sql.Append(", "+DBSchema.APPM_SCHEMA+".TARGET "+ DBConstantes.Tables.TARGET_PREFIXE+" ");
			#endregion

			#region where
			//where
			sql.Append(" where "+DBConstantes.Tables.PRODUCT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.PRODUCT_PREFIXE+".id_product="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".ID_PRODUCT ");
			sql.Append(" and "+dateField+" >="+dateBegin);
			sql.Append(" and "+dateField+" <="+dateEnd);
			//media
			sql.Append(" and "+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_media_secodip="+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_media");
			//target
			//sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_target="+DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target");
			sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+ idAdditionalTarget.ToString()+")");		
			//sql.Append(" and "+ DBConstantes.Tables.TARGET_PREFIXE+".id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and "+ DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_language_data_i="+webSession.SiteLanguage);
			sql.Append(" and " + DBConstantes.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			
			//sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,DBConstantes.Tables.WEB_PLAN_PREFIXE,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.WEB_PLAN_PREFIXE, true));						
		
			//all results without inset
			sql.Append(" and " + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			//Rights
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));	
			sql.Append(TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true));											

			#endregion

			#region order by
			sql.Append(" order by product ");
			#endregion	
			
			#endregion		

			#region Execution de la requête
			try{
				dsListProduct=webSession.Source.Fill(sql.ToString());
				dsListProduct.Tables[0].TableName="dsListProduct";
				return(dsListProduct);
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des régies avec les supports d'un utilisateur",err));
			}
			#endregion			

		}

	}
}