#region Information
//Authors:  D.Mussuma
//Date of Creation: 29/08/2005
//Date of modification:
#endregion
using System;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using  DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// Données zoom plan média
	/// </summary>
	public class ZoomMediaPlanDataAccess
	{
		#region General Data for APPM Media Plan 
		/// <summary>
		///  Calculates and returns the dataset for the Media Plan 	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget) {
			#region variables
			StringBuilder sql = new StringBuilder(1000);
			#endregion
		
			#region Construction of the query
			
			#region Select
			//Vehicle
			sql.Append(" select ");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_vehicle, vehicle, ");
			//Category
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_category, category, ");
			//Media and its periodicity
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_media, media,"+DBTables.DATA_PRESS_APPM_PREFIXE+".id_periodicity,");
			//Units
			sql.AppendFormat("sum({0}) as {1},sum({2}) as {3},(sum({4})*{5}) as totalGRP"
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			//Date of publication
			sql.Append(", date_parution_num as publication_date");
			#endregion

			#region From
			sql.Append(" from ");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.VEHICLE+" "+DBTables.VEHICLE_PREFIXE+",");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.CATEGORY+" "+ DBTables.CATEGORY_PREFIXE+",");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.MEDIA+" "+ DBTables.MEDIA_PREFIXE+",");
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+DBTables.DATA_PRESS_APPM+ " "+DBTables.DATA_PRESS_APPM_PREFIXE+", ");
			sql.Append(DBSchema.APPM_SCHEMA+"."+DBTables.TARGET_MEDIA_ASSIGNEMNT+" "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+",");
			sql.Append(DBSchema.APPM_SCHEMA+"."+DBTables.TARGET+" "+DBTables.TARGET_PREFIXE);
			#endregion

			#region Where
			sql.Append(" where ");
			sql.Append(DBTables.VEHICLE_PREFIXE+".id_vehicle="+DBTables.DATA_PRESS_APPM_PREFIXE+".id_vehicle ");
			sql.Append(" and "+DBTables.CATEGORY_PREFIXE+".id_category="+DBTables.DATA_PRESS_APPM_PREFIXE+".id_category ");
			sql.Append(" and "+DBTables.MEDIA_PREFIXE+".id_media="+DBTables.DATA_PRESS_APPM_PREFIXE+".id_media");
			sql.Append(" and "+DBTables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage);
			sql.Append(" and "+DBTables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage);
			sql.Append(" and "+DBTables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage);
			sql.Append(" and "+DBTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and date_parution_num >="+dateBegin+" and date_parution_num <="+dateEnd);
			sql.Append(SQLGenerator.GetAnalyseCustomerProductSelection(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, DBTables.DATA_PRESS_APPM_PREFIXE, DBTables.DATA_PRESS_APPM_PREFIXE, true));			
			sql.Append(" and "+DBTables.TARGET_PREFIXE+".id_target in("+additionalTarget+")");
			sql.Append(" and "+DBTables.TARGET_PREFIXE + ".id_target = " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target ");
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBTables.WEB_PLAN_PREFIXE + ".id_media ");
			//Media Right
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));
			//Product rights
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBTables.DATA_PRESS_APPM_PREFIXE, true, module.ProductRightBranches));
			#endregion

			#region Group by
			sql.Append(" group by ");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_vehicle, vehicle, ");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_category, category,");
            sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE + ".id_media, media, " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_periodicity , date_parution_num, " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].DatabaseField);
			#endregion

			#region Order by
			sql.Append(" order by ");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_vehicle, vehicle, ");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_category, category,");
			sql.Append(DBTables.DATA_PRESS_APPM_PREFIXE+".id_media, media, date_parution_num");
			#endregion

			#endregion

			#region Execution of the query
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err) {
				throw(new WebExceptions.APPMMediaPlanDataAccessException("GetData:: Error while executing the query for the Media Plan APPM ",err));
			}		
			#endregion			
			
		}
		#endregion
	}
}
