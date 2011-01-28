#region Informations
// Author: K. Shehzad 
// Date of creation: 27/07/2005 
// K.Shehzad: 05/09/2005 Table/Field names changed
#endregion

using System;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using Cst = TNS.AdExpress.Constantes;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// This class is used to get the data for GRP and CGRP of all the 
	/// targets  as well as their affinities with respect to the base target.
	/// </summary>
	public class AffinitiesDataAccess
	{
		/// <summary>
		/// This method calculates and returns the dateset containing the required values like GRP, CGRP,
		/// etc.
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Base target</param>
		/// <param name="idWave">Wave ID</param>
		/// <returns>DataSet containing data like GRP, CGRP of all the targets in a specified wave</returns>
		public static DataSet GetData( WebSession webSession , IDataSource dataSource,int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idWave)
		{
			StringBuilder sql = new StringBuilder(3000);

			#region construction of the query
			
			#region Select
			sql.AppendFormat(" select distinct id_target,target,sum(totalGRP) as {0},sum({1}) as {1}"
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.grp].DatabaseField
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.euro].Id.ToString() );
			#endregion

			#region from
			sql.Append(" from (");
			
			#region select
            sql.AppendFormat("select distinct {0}.id_target,target,(sum({1})*{2}) totalGRP,sum({3}) as {4}"
                , DBTables.TARGET_PREFIXE
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.insertion].DatabaseMultimediaField
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.grp].DatabaseField
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.euro].DatabaseMultimediaField
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.euro].Id.ToString());
			#endregion

			#region from
			sql.Append(" from " + DBSchema.APPM_SCHEMA + "." + DBTables.TARGET_MEDIA_ASSIGNEMNT + " " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ",");
			sql.Append(DBSchema.APPM_SCHEMA + "." + Functions.GetAPPMWebPlanTable(webSession) + " " + DBTables.WEB_PLAN_PREFIXE + ",");
			sql.Append(DBSchema.APPM_SCHEMA + "." + DBTables.TARGET + " " + DBTables.TARGET_PREFIXE );
			#endregion

			#region where
			sql.Append(" where ");
			sql.Append(DBTables.TARGET_PREFIXE + ".id_target = " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target ");
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBTables.TARGET_PREFIXE+ ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBTables.WEB_PLAN_PREFIXE + ".id_media ");
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + "." + Functions.GetDateFieldWebPlanTable(webSession) + " between " + dateBegin + " and " + dateEnd);
			sql.Append(" and " + DBTables.TARGET_PREFIXE + ".id_wave = " + idWave.ToString());
			//all results without inset
			sql.Append(" and " + DBTables.WEB_PLAN_PREFIXE + ".id_inset = " + Cst.Classification.DB.insertType.EXCEPT_INSERT.GetHashCode());
			
			//sql.Append(SQLGenerator.GetAnalyseCustomerProductSelection(webSession,"",DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,DBTables.WEB_PLAN_PREFIXE,true));	

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.WEB_PLAN_PREFIXE, true));

			//Media Universe
			sql.Append(SQLGenerator.GetResultMediaUniverse(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE));

			//Rights
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBTables.WEB_PLAN_PREFIXE,true));
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(webSession, DBConstantes.Tables.WEB_PLAN_PREFIXE, true, module.ProductRightBranches));
			
            #endregion

			#region Group By
			sql.Append(" group by ");
			sql.AppendFormat("{0}.id_target,target,{1}"
                , DBTables.TARGET_PREFIXE
                , UnitsInformation.List[Cst.Web.CustomerSessions.Unit.grp].DatabaseField);
			#endregion

			#region order by
			sql.Append(" order by ");
			sql.Append(DBTables.TARGET_PREFIXE + ".id_target");
			#endregion

			sql.Append(")");
			#endregion

			#region group by
			sql.Append(" group by ");
			sql.Append("id_target,target");
			#endregion

			#region order by
			sql.Append(" order by ");
			sql.Append(" grp desc ");		
			#endregion

			#endregion

			#region Excection of the query
			try{
				return dataSource.Fill(sql.ToString());
			}
			catch( System.Exception exc){
				throw new AffinitiesDataAccessExeption("GetData::An error occured while executing the request for Affinties ",exc);
			}
			#endregion

		}
	}
}
