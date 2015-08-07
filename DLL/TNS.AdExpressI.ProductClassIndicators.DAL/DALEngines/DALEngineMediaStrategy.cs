#region Info
/*
 * Author :     G Ragneau
 * Created on : 30/07/2008
 * History:
 *      Date - Author - Description
 *      29/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDb = TNS.AdExpress.Constantes.DB;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL.Exceptions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines
{

    /// <summary>
    /// Provide data for Media strategy Indicators
    /// </summary>
    public class DALEngineMediaStrategy : DALEngine
    {

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        public DALEngineMediaStrategy(WebSession session, DALUtilities dalUtilities)
            : base(session, dalUtilities)
        {
        }
        #endregion

        #region Get Table Data
        /// <summary>
        /// Get data for media strategy indicators presented as table
        /// </summary>
        /// <returns>Seasonality data</returns>
        public DataSet GetTableData(CstResult.MotherRecap.ElementType classifLevel, CstComparaisonCriterion compareasonCriterion, bool applyAdvFilter)
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlJoin = new StringBuilder();
            StringBuilder sqlGroup = new StringBuilder();
            StringBuilder sqlOrder = new StringBuilder();
            Table dataTable = GetDataTable(true);

			#region Build request
																
			sql.Append(" select");

			#region Select
			//Media Field
            sql.AppendFormat(" {0}.id_vehicle, {0}.vehicle", _recapVehicle.Prefix);
            sqlFrom.AppendFormat("{0}, {1}", dataTable.SqlWithPrefix, _recapVehicle.SqlWithPrefix);
            sqlJoin.AppendFormat(" {0}.id_vehicle = {1}.id_vehicle and {1}.id_language = {2}", dataTable.Prefix, _recapVehicle.Prefix, _session.DataLanguage);
            sqlGroup.AppendFormat(" {0}.id_vehicle, {0}.vehicle", _recapVehicle.Prefix);
            sqlOrder.AppendFormat(" {0}.id_vehicle, {0}.vehicle", _recapVehicle.Prefix);
            if (_session.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory)
            {
                sql.AppendFormat(", {0}.id_category, {0}.category", _recapCategory.Prefix);
                sqlFrom.AppendFormat(", {0}", _recapCategory.SqlWithPrefix);
                sqlJoin.AppendFormat(" and {0}.id_category = {1}.id_category and {1}.id_language = {2}", dataTable.Prefix, _recapCategory.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat(", {0}.id_category, {0}.category", _recapCategory.Prefix);
                sqlOrder.AppendFormat(", {0}.id_category, {0}.category", _recapCategory.Prefix);
            }
            else if (_session.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia)
            {
                sql.AppendFormat(", {0}.id_category, {0}.category, {1}.id_media, {1}.media", _recapCategory.Prefix, _recapMedia.Prefix);
                sqlFrom.AppendFormat(", {0}, {1}", _recapCategory.SqlWithPrefix, _recapMedia.SqlWithPrefix);
                sqlJoin.AppendFormat(" and {0}.id_media = {1}.id_media and {1}.id_language = {2}", dataTable.Prefix, _recapMedia.Prefix, _session.DataLanguage);
                sqlJoin.AppendFormat(" and {0}.id_category = {1}.id_category and {1}.id_language = {2}", _recapMedia.Prefix, _recapCategory.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat(", {0}.id_category, {0}.category", _recapCategory.Prefix);
                sqlOrder.AppendFormat(", {0}.id_category, {0}.category", _recapCategory.Prefix);
                sqlGroup.AppendFormat(", {0}.id_media, {0}.media", _recapMedia.Prefix);
                sqlOrder.AppendFormat(", {0}.id_media, {0}.media", _recapMedia.Prefix);
            }

			//Product Field
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
                sqlFrom.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
                sqlJoin.AppendFormat(" and {0}.id_product = {1}.id_product and {1}.id_language = {2}", dataTable.Prefix, _recapProduct.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
                sqlOrder.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
            }
            else
            {
                sql.AppendFormat(", {0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
                sqlFrom.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
                sqlJoin.AppendFormat(" and {0}.id_advertiser = {1}.id_advertiser and {1}.id_language = {2}", dataTable.Prefix, _recapAdvertiser.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat(", {0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
                sqlOrder.AppendFormat(", {0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
            }
            sql.AppendFormat(", {0}", this.GetExpenditureClause());
            #endregion

            sql.AppendFormat(" from {0}", sqlFrom);

            sql.Append(" where ");

            #region where
            sql.Append(sqlJoin);

            #region Advertiser filter
            if (applyAdvFilter)
            {
                string l = string.Empty;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].GetGroup(0).Count() > 0 && _session.SecondaryProductUniverses[0].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
                {
                    l = _session.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                }
                string l2 = string.Empty;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].GetGroup(0).Count() > 0 && _session.SecondaryProductUniverses[1].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
                {
                    l2 = _session.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                }
                if ((l.Length + l2.Length) > 0)
                {
                    sql.AppendFormat(" and {0}.id_advertiser in (", dataTable.Prefix);
                    if (l.Length > 0)
                    {
                        sql.Append(l);
                        if (l2.Length < 1)
                        {
                            sql.Append(")");
                        }
                    }
                    if (l2.Length > 0)
                    {
                        if (l.Length > 0)
                        {
                            sql.AppendFormat(",{0})", l2);
                        }
                        else
                        {
                            sql.AppendFormat("{0})", l2);
                        }
                    }

                }

            }
            #endregion

            #region Product selection
            if (compareasonCriterion == CstComparaisonCriterion.universTotal)
            {
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                // Product rights
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
                string productRightsBranches = (module != null) ? module.ProductRightBranches : "";
                sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix,  true, productRightsBranches));
            }


            //Total famille
            if (compareasonCriterion == CstComparaisonCriterion.sectorTotal)
            {
                sql.AppendFormat(" and {0}.id_sector in ( ", dataTable.Prefix);
                sql.Append(" select distinct id_sector ");
                sql.AppendFormat(" from {0} where", dataTable.SqlWithPrefix);
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false, TNS.Classification.Universe.AccessType.includes));

                sql.Append(" ) ");
            }
            #endregion

            sql.AppendFormat(" and {0}", _utilities.GetMediaSelection(dataTable.Prefix));

            #endregion

            #region Group by
            sql.AppendFormat(" group by {0}", sqlGroup);
            #endregion

            #region Order by
            sql.AppendFormat(" order by {0}", sqlOrder);
            #endregion

			#endregion											

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
            try
            {
                ds = dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw new ProductClassIndicatorsDALException(string.Format("Error while loading data: {0}", sql), err);
            }
            #endregion

            return (ds);
        }
        #endregion

        #region Get Top
        /// <summary>
        /// Get fisrt elements
        /// </summary>
        /// <returns>First elements on the required deteil level</returns>
        public DataSet GetTopElements(CstResult.MotherRecap.ElementType classifLevel, CstComparaisonCriterion compareasonCriterion, CstResult.MediaStrategy.MediaLevel mediaLevel, bool pluri)
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlJoin = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlPartition = new StringBuilder();
            StringBuilder sqlGroup = new StringBuilder();
            StringBuilder sqlOrder = new StringBuilder();
            Table dataTable = GetDataTable(true);


            #region Build request

            sql.Append(" select * from ( select");

            #region Select
            if (!pluri)
            {
                switch (mediaLevel)
                {
                    case CstResult.MediaStrategy.MediaLevel.vehicleLevel:
                        sql.AppendFormat(" {0}.id_vehicle, ", dataTable.Prefix);
                        sqlGroup.AppendFormat(" {0}.id_vehicle, ", dataTable.Prefix);
                        sqlOrder.AppendFormat(" {0}.id_vehicle, ", dataTable.Prefix);
                        sqlPartition.AppendFormat(" partition by {0}.id_vehicle", dataTable.Prefix);
                        break;
                    case CstResult.MediaStrategy.MediaLevel.categoryLevel:
                        sql.AppendFormat(" {0}.id_category, ", dataTable.Prefix);
                        sqlGroup.AppendFormat(" {0}.id_category, ", dataTable.Prefix);
                        sqlOrder.AppendFormat(" {0}.id_category, ", dataTable.Prefix);
                        sqlPartition.AppendFormat(" partition by {0}.id_category", dataTable.Prefix);
                        break;
                    case CstResult.MediaStrategy.MediaLevel.mediaLevel:
                        sql.AppendFormat(" {0}.id_media, ", dataTable.Prefix);
                        sqlGroup.AppendFormat(" {0}.id_media, ", dataTable.Prefix);
                        sqlOrder.AppendFormat(" {0}.id_media, ", dataTable.Prefix);
                        sqlPartition.AppendFormat(" partition by {0}.id_media", dataTable.Prefix);
                        break;
                }
            }


            //Product Field
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(" {0}.id_product, {0}.product", _recapProduct.Prefix);
                sqlFrom.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
                sqlJoin.AppendFormat(" {0}.id_product = {1}.id_product and {1}.id_language = {2}", dataTable.Prefix, _recapProduct.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat("{0}.id_product, {0}.product", _recapProduct.Prefix);
                sqlOrder.AppendFormat("{0}.id_product, {0}.product", _recapProduct.Prefix);
            }
            else
            {
                sql.AppendFormat(" {0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
                sqlFrom.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
                sqlJoin.AppendFormat(" {0}.id_advertiser = {1}.id_advertiser and {1}.id_language = {2}", dataTable.Prefix, _recapAdvertiser.Prefix, _session.DataLanguage);
                sqlGroup.AppendFormat("{0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
                sqlOrder.AppendFormat("{0}.id_advertiser, {0}.advertiser", _recapAdvertiser.Prefix);
            }
            sql.AppendFormat(", {0}", this.GetExpenditureClause());
            sql.AppendFormat(", rank() over({0} order by {1} desc) rk", sqlPartition, this.GetExpenditureClause(CstResult.PalmaresRecap.typeYearSelected.currentYear));
            #endregion

            sql.AppendFormat(" from {0}", dataTable.SqlWithPrefix);
            sql.Append(sqlFrom);

            sql.Append(" where ");

            #region where
            sql.Append(sqlJoin);

            #region Product selection
            if (compareasonCriterion == CstComparaisonCriterion.universTotal)
            {
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                // Product rights
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
                string productRightsBranches = (module != null) ? module.ProductRightBranches : "";
                sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix,  true, productRightsBranches));
            }


            //Total famille
            if (compareasonCriterion == CstComparaisonCriterion.sectorTotal)
            {
                sql.AppendFormat(" and {0}.id_sector in ( ", dataTable.Prefix);
                sql.Append(" select distinct id_sector ");
                sql.AppendFormat(" from {0} where", dataTable.SqlWithPrefix);
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false, TNS.Classification.Universe.AccessType.includes));

                sql.Append(" ) ");
            }
            #endregion

            sql.AppendFormat(" and {0}", _utilities.GetMediaSelection(dataTable.Prefix));
            sql.AppendFormat(" and TOTAL_YEAR_N{0} > 0", _strYearId);

            #endregion

            #region Group by
            sql.AppendFormat(" group by {0}", sqlGroup);
            #endregion

            #region Order by
            sql.AppendFormat(" order by {0}", sqlOrder);
            #endregion

            sql.Append(") where rk = 1");
            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
            try
            {
                ds = dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw new ProductClassIndicatorsDALException(string.Format("Error while loading data: {0}", sql), err);
            }
            #endregion

            return (ds);
        }
        #endregion
    }
}
