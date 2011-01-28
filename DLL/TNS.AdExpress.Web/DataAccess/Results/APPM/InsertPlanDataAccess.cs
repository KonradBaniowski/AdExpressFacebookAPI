#region Information
/*
Author : G.RAGNEAU
Creation : 01/08/2005
Last Modifications : 
 Modified by: K.Shehzad
 Date of Modification: 12/08/2005  (changing the Exception usage)
 K.Shehzad: 05/09/2005 Table/Field names changed
*/
#endregion

using System;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;

using DBCst = TNS.AdExpress.Constantes.DB;
using Cst = TNS.AdExpress.Constantes;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Manage the requests to the database for the module "Valorisation and efficiency by insertion"
	/// !!! Require to load the advertiser univers to study in the Current Univres Advertiser of the webSession
	/// </summary>
	public class InsertPlanDataAccess
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Base target</param>
		/// <param name="idMedia">Media to study</param>
		/// <param name="idWave">Wave ID</param>
		/// <returns>DataSet containing data for the "Valorisation and efficiency by insertion plan"</returns>
		public static DataSet GetData(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idMedia, Int64 idWave){
		
			StringBuilder sql = new StringBuilder(3000);

			#region select
			sql.Append("select distinct " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media," + DBCst.Tables.MEDIA_PREFIXE + ".media,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_advertisement,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_product," + DBCst.Tables.PRODUCT_PREFIXE + ".product,");
 			//sql.Append("sum(" + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".expenditure_euro) as euro,");
            sql.AppendFormat("{0},",SQLGenerator.GetUnitFieldNameSumWithAlias(webSession,DBCst.TableType.Type.dataVehicle,DBCst.Tables.DATA_PRESS_APPM_PREFIXE));
			sql.Append(DBCst.Tables.FORMAT_PREFIXE+ ".format,");
			sql.Append(DBCst.Tables.LOCATION_PREFIXE + ".location,");
			//sql.Append("'toto' as location,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num," + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_cover_num,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE+ ".media_paging");
			#endregion

			#region from
			sql.Append(" from " + DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.DATA_PRESS_APPM + " " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.PRODUCT + " " + DBCst.Tables.PRODUCT_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.FORMAT + " " + DBCst.Tables.FORMAT_PREFIXE+ ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.DATA_LOCATION + " " + DBCst.Tables.DATA_LOCATION_PREFIXE+ ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT + " " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.TARGET + " " + DBCst.Tables.TARGET_PREFIXE + ",");
			sql.Append(DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.WAVE + " " + DBCst.Tables.WAVE_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.LOCATION + " " + DBCst.Tables.LOCATION_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.MEDIA + " " + DBCst.Tables.MEDIA_PREFIXE);
			#endregion

			#region where

			sql.Append(" where ");

			sql.Append(DBCst.Tables.TARGET_PREFIXE + ".id_target = " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target ");
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media ");

			sql.Append(" and " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media=" + idMedia.ToString());
			sql.Append(" and " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num" + " between " + dateBegin + " and " + dateEnd);//date_parution_num

			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".id_media = " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media ");
			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".id_product = " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_product ");
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.FORMAT_PREFIXE + ".id_format (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_format" );
			sql.Append(" and " + DBCst.Tables.FORMAT_PREFIXE + ".id_language (+)= " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.FORMAT_PREFIXE + ".activation (+)< " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_location (+)= " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_location" );
			sql.Append(" and " + DBCst.Tables.LOCATION_PREFIXE + ".id_language (+)= " + webSession.DataLanguage);
			sql.Append(" and " + DBCst.Tables.LOCATION_PREFIXE + ".activation (+)< " + DBCst.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_media (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media ");
			sql.Append(" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".id_advertisement (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_advertisement ");
			sql.Append(" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".date_media_num (+)= " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num ");
			sql.Append(" and " + DBCst.Tables.DATA_LOCATION_PREFIXE + ".activation (+)< " + DBCst.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".id_wave = " + idWave);
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".id_wave = " + DBCst.Tables.WAVE_PREFIXE + ".id_wave");
			sql.Append(" and " + DBCst.Tables.WAVE_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			
			sql.Append(" and " + DBCst.Tables.TARGET_PREFIXE + ".id_target in (" + idBaseTarget + ")");
			//all results without inset
			sql.Append(" and " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_inset is null");

			//sql.Append(SQLGenerator.GetAnalyseCustomerProductSelection(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));


			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));

			//Media Universe
			sql.Append(WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE));

			//Media Rights
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));
			//Product rights
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true, module.ProductRightBranches));

			#endregion
            
			#region Group By
			sql.Append(" group by " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_product," + DBCst.Tables.PRODUCT_PREFIXE + ".product,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_media," + DBCst.Tables.MEDIA_PREFIXE + ".media,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".id_advertisement,");
            sql.AppendFormat(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".{0},", webSession.GetSelectedUnit().DatabaseField);
			sql.Append(DBCst.Tables.FORMAT_PREFIXE + ".format,");
			sql.Append(DBCst.Tables.LOCATION_PREFIXE + ".location,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_cover_num,");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE+ ".media_paging");
			#endregion

			sql.Append(" order by " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_media_num asc, format asc ");//" + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ".date_parution_num asc,

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new InsertPlanDataAccessException("GetData::An error occured when accessing to the database ",exc);
			}
		}
	}
}
