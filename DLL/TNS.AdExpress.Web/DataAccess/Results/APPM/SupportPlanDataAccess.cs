#region Information
/*
Author : G.RAGNEAU
Creation : 26/07/2005
Last Modifications : 

 -  K.Shehzad Date of Modification: 12/08/2005  (changing the Exception usage)
 -  D. V. Mussuma of Modification: 23/08/2005  correction date sélectionnée.
 -	G .Ragneau : 25/08/2005 : ajout close hors encarts dans la requete
 -  K.Shehzad: 05/09/2005 Table/Field names changed
 -  K.Shehzad: 06/09/2005 Use of decode function to avoid division by zero while calculating CGRP
*/
#endregion

using System;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;

using DBCst = TNS.AdExpress.Constantes.DB;
using Cst = TNS.AdExpress.Constantes;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Manage the reuqests to the database for the module "Valorisation and efficiency by media"
	/// !!! Require to load the advertiser univers to study in the Current Univres Advertiser of the webSession
	/// </summary>
	public class SupportPlanDataAccess
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Base target</param>
		/// <param name="idAdditionaleTarget">Addictionale target</param>
		/// <param name="idWave">Wave ID</param>
		/// <returns>DataSet containing data for the "Valorisation and efficiency by support plan"</returns>
		public static DataSet GetData(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idAdditionaleTarget, Int64 idWave){
		
			StringBuilder sql = new StringBuilder(3000);

			#region select
			sql.Append("select " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_vehicle," + DBCst.Tables.VEHICLE_PREFIXE + ".vehicle,");
			sql.Append(DBCst.Tables.WEB_PLAN_PREFIXE + ".id_category," + DBCst.Tables.CATEGORY_PREFIXE + ".category,");
			sql.Append(DBCst.Tables.WEB_PLAN_PREFIXE + ".id_media," + DBCst.Tables.MEDIA_PREFIXE + ".media,");
			sql.Append(DBCst.Tables.TARGET_PREFIXE + ".id_target," + DBCst.Tables.TARGET_PREFIXE + ".target,");
			sql.Append("sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalunite) as euro,");
			sql.Append("(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalinsert)*" + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp) as totalgrp,");
			sql.Append(DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp as grp,");
			//modifications to avoid division by zero while calculating CGRP by using decode function
			sql.Append("decode(sum(totalinsert)*" + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp,0,0,round(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalunite)/(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalinsert)*" + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp),3))as cgrp ");
			//sql.Append("round(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalunite)/(sum(" + DBCst.Tables.WEB_PLAN_PREFIXE + ".totalinsert)*" + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp),3)as cgrp ");
			#endregion

			#region from
			sql.Append(" from " + DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT + " " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + Functions.GetAPPMWebPlanTable(webSession) + " " + DBCst.Tables.WEB_PLAN_PREFIXE + ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.WAVE + " " + DBCst.Tables.WAVE_PREFIXE + ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET + " " + DBCst.Tables.TARGET_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.MEDIA + " " + DBCst.Tables.MEDIA_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.CATEGORY + " " + DBCst.Tables.CATEGORY_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.VEHICLE + " " + DBCst.Tables.VEHICLE_PREFIXE + " ");
			#endregion

			#region where

			sql.Append(" where ");

			sql.Append(DBCst.Tables.WAVE_PREFIXE + ".id_wave = " + DBCst.Tables.TARGET_PREFIXE + ".id_wave ");
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".id_target = " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target ");
			sql.Append(" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_media ");

			sql.Append(" and " + DBCst.Tables.WEB_PLAN_PREFIXE + "." + Functions.GetDateFieldWebPlanTable(webSession) + " between " + dateBegin.ToString() + " and " + dateEnd.ToString());

			sql.Append(" and " + DBCst.Tables.VEHICLE_PREFIXE + ".id_vehicle = " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_vehicle ");
			sql.Append(" and " + DBCst.Tables.VEHICLE_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.VEHICLE_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.CATEGORY_PREFIXE + ".id_category = " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_category ");
			sql.Append(" and " + DBCst.Tables.CATEGORY_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.CATEGORY_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".id_media = " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_media ");
			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBCst.Tables.WAVE_PREFIXE + ".id_wave = " + idWave.ToString());
			sql.Append(" and " + DBCst.Tables.WAVE_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".id_target in (" + idBaseTarget.ToString() + "," + idAdditionaleTarget.ToString() + ")");
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			//all results without inset
			sql.Append(" and " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBCst.Tables.WEB_PLAN_PREFIXE, true));						
			
			//Media Rights
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.WEB_PLAN_PREFIXE, true));
			//Product rights
			sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession, DBCst.Tables.WEB_PLAN_PREFIXE, true));

			#endregion

			#region Group By
			sql.Append(" group by " + DBCst.Tables.WEB_PLAN_PREFIXE + ".id_vehicle," + DBCst.Tables.VEHICLE_PREFIXE + ".vehicle,");
			sql.Append(DBCst.Tables.WEB_PLAN_PREFIXE + ".id_category," + DBCst.Tables.CATEGORY_PREFIXE + ".category,");
			sql.Append(DBCst.Tables.WEB_PLAN_PREFIXE + ".id_media," + DBCst.Tables.MEDIA_PREFIXE + ".media,");
			sql.Append(DBCst.Tables.TARGET_PREFIXE + ".id_target," + DBCst.Tables.TARGET_PREFIXE + ".target,");
			sql.Append(DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".grp");
			#endregion

			sql.Append(" order by category asc ,id_category , euro desc, media, id_media");

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new SupportPlanDataAccessException("GetData::An error occured when accessing to the database ",exc);
			}
		}
	}
}
