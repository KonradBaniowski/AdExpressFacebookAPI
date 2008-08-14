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
    /// Provide data for Tops Indicators
    /// </summary>
    public class DALEngineSeasonality : DALEngine
    {

        #region Attributes
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        public DALEngineSeasonality(WebSession session)
            : base(session)
        {
        }
        #endregion

        #region Get Table Data
        /// <summary>
        /// Get data for Seasonality indicators presented as table
        /// </summary>
        /// <returns>Seasonality data</returns>
        public DataSet GetTableData(bool withAdvertisers, bool withRights)
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            Table dataTable = GetDataTable(true);

            #region Request Building

            sql.Append("select");

            #region Select Clause
            sql.AppendFormat(" {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            //Sector selection
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
            {
                sql.AppendFormat(", {0}.id_sector, {1}.sector", dataTable.Prefix, _recapSector.Prefix);
            }
            //Product
            sql.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
            sql.AppendFormat(", {0}", this.GetExpenditureClause(true));
            #endregion

            sql.Append(" from");

            #region From
            sql.AppendFormat(" {0}", dataTable.SqlWithPrefix);
            sql.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
            {
                sql.AppendFormat(", {0}", _recapSector.SqlWithPrefix);
            }
            sql.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
            #endregion

            sql.Append(" where");

            #region Where

            #region  Joins
            sql.AppendFormat(" {0}.id_advertiser={1}.id_advertiser and {1}.id_language={2}", dataTable.Prefix, _recapAdvertiser.Prefix, _session.SiteLanguage);
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
            {
                sql.AppendFormat(" and {0}.id_sector={1}.id_sector and {1}.id_language={2}", dataTable.Prefix, _recapSector.Prefix, _session.SiteLanguage);
            }
            sql.AppendFormat(" and {0}.id_product={1}.id_product and {1}.id_language={2}", dataTable.Prefix, _recapProduct.Prefix, _session.SiteLanguage);
            #endregion

            #region Media selection
            sql.AppendFormat(" and {0}", this.GetMediaSelection(dataTable.Prefix));
            #endregion

            #region Product selection
            if (withRights)
            {
                if (withAdvertisers)
                {
                    string l = string.Empty;
                    if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].GetGroup(0).Count() > 0)
                    {
                        l = _session.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                    }
                    string l2 = string.Empty;
                    if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].GetGroup(0).Count() > 0)
                    {
                        l2 = _session.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                    }
                    if ((l.Length + l2.Length) > 0)
                    {
                        sql.AppendFormat(" and {0}.id_advertiser in (", dataTable.Prefix);
                        if (l.Length > 0)
                        {
                            sql.Append(l);
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

                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                // Product rights
                sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));

            }
            //Total famille
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
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

            #endregion

            sql.Append(" group by ");

            #region group by
            sql.AppendFormat(" {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
            {
                sql.AppendFormat(", {0}.id_sector, {1}.sector", dataTable.Prefix, _recapSector.Prefix);
            }
            sql.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
            #endregion

            sql.Append(" order by ");

            #region order by
            sql.AppendFormat(" {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
            {
                sql.AppendFormat(", {0}.id_sector, {1}.sector", dataTable.Prefix, _recapSector.Prefix);
            }
            sql.AppendFormat(", {0}.id_product, {0}.product", _recapProduct.Prefix);
            #endregion

            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
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

        #region Get Chart Data
        /// <summary>
        /// Get list of advertisers and investments (N and N-1) for the seasonality chart indicator.
        /// Get univers, market or sector total
        /// </summary>
        /// <remarks>Used to present data as a chart</remarks>
        /// <param name="withAdvertisers">Include advertisers data?</param>	
        /// <param name="withRights">Apply product righst?</param>
        /// <returns>Seasonality data</returns>
        /// <history>[D. V. Mussuma]  Modifié le 30/11/04</history>
        public DataSet GetChartData(bool withAdvertisers, bool withRights)
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            Table dataTable = GetDataTable(true);

            #region variables
            ////Libéllé de la table "recap" cible
            //string recapTableName = "";
            ////bolléen pour construire la requête sql
            //bool premier = true;
            //bool firstfield = true;
            ////bool firstgrouby=true;
            ////bool firstorderby=true;
            //bool beginbyand = true;
            ////Liste des annonceurs concurrents sélectionnés
            //string CompetitorAdvertiserAccessList = "";
            ////Liste des familles possédant au moins un groupe ou une variété sélectionnée
            //string listVehicle = "";
            ////Pour stocker les données générés par la requête
            //DataSet ds = null;
            ////Indique si la requête sql doit être construite et exécitée
            //bool buildSqlStatement = true;
            #endregion

            #region Query building

            sql.Append("select");

            #region Select
            if (withAdvertisers)
            {
                sql.AppendFormat(" {0}.id_advertiser, {1}.advertiser,", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            sql.AppendFormat(" {0}", this.GetExpenditureClause(true));
            #endregion

            sql.Append(" from");

            #region From
            sql.AppendFormat(" {0}", dataTable.SqlWithPrefix);
            if (withAdvertisers)
            {
                sql.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
            }
            #endregion

            sql.Append(" Where");

            #region Where

            #region Joins
            if (withAdvertisers)
            {
                sql.AppendFormat(" {0}.id_advertiser={1}.id_advertiser and {1}.id_language={2} and", dataTable.Prefix, _recapAdvertiser.Prefix, _session.SiteLanguage);
            }
            #endregion

            #region Selection
            sql.AppendFormat(" {0}", this.GetMediaSelection(dataTable.Prefix));

            //Advertiser
            if (withAdvertisers)
            {
                string l = string.Empty;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].GetGroup(0).Count() > 0)
                {
                    l = _session.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                }
                string l2 = string.Empty;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].GetGroup(0).Count() > 0)
                {
                    l2 = _session.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                }
                if ((l.Length + l2.Length) > 0)
                {
                    sql.AppendFormat(" and {0}.id_advertiser in (", dataTable.Prefix);
                    if (l.Length > 0)
                    {
                        sql.Append(l);
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
            //Not sector or market total
            if (withRights)
            {
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                // Product rights
                sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
            }
            //Sector total
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && !withAdvertisers && !withRights)
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

            #endregion

            #region Group by 
            if (withAdvertisers)
            {
                sql.AppendFormat(" group by {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            #endregion

            #region Sort
            if (withAdvertisers)
            {
                sql.AppendFormat(" order by {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            #endregion

            #endregion

            #region Query exec
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
            try
            {
                ds = dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw new ProductClassIndicatorsDALException(string.Format("Unable to lad data for graphics seasonality : {0}", sql), err);
            }
            #endregion

            return (ds);
        }
        #endregion

        #region Get total for seasonality

        /// <summary>
        /// Fournit le total marché ou le total famille ou univers
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="comparisonCriterion">Critère de commparaison pour le calcul des totaux (famille ou marché ou univers)</param>
        /// <returns></returns>
        public DataSet GetSeasonalityTotal()
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            Table dataTable = GetDataTable(true);

            #region Construction de la requête
            //Foreach month of the selected period
            for (int i = _periodBegin.Month; i <= _periodEnd.Month; i++)
            {

                if (i != _periodBegin.Month) sql.Append(" UNION ALL");

                #region Select
                sql.Append(" select request1.*");
                //Evolution N/N-1
                if (_session.ComparativeStudy)
                {
                    sql.Append(", decode(request1.Total_N1,0,-1,ROUND(((request1.Total_N/request1.Total_N1)*100)-100,0)) as evol");
                }
                sql.Append(", round((request1.Total_N/request1.nbref)) as budget_moyen");
                sql.AppendFormat(", {0}.product", _recapProduct.Prefix);
                sql.AppendFormat(", {0}.advertiser", _recapAdvertiser.Prefix);
                sql.AppendFormat(", pkg_recap_test.FIRST_PRODUCT_INVEST(request1.id_product,'exp_euro_N{0}_{1}','{2}') as INVESTMENT_PRODUCT", _strYearId, i, dataTable.Label);
                sql.AppendFormat(", pkg_recap_test.FIRST_ADVERTISER_INVEST(request1.id_advertiser,'exp_euro_N{0}_{1}','{2}') as INVESTMENT_ADVERTISER", _strYearId, i, dataTable.Label);
                sql.AppendFormat(", ROUND((pkg_recap_test.FIRST_PRODUCT_INVEST(request1.id_product,'exp_euro_N{0}_{1}','{2}')/request1.Total_N)*100,3) as SOV_FIRST_PRODUCT", _strYearId, i, dataTable.Label);
                sql.AppendFormat(", ROUND((pkg_recap_test.FIRST_ADVERTISER_INVEST(request1.id_advertiser,'exp_euro_N{0}_{1}','{2}')/request1.Total_N)*100,3) as SOV_FIRST_ADVERTISER", _strYearId, i, dataTable.Label);
                #endregion

                sql.Append(" from (");

                #region From

                #region "in" request

                sql.Append(" select");

                #region Select
                //Investissement année N												
                sql.AppendFormat(" sum(exp_euro_N{0}_{1}) as Total_N{0}", _strYearId, i);
                //Investissement année N-1						 
                if (_session.ComparativeStudy)
                {
                    sql.AppendFormat(", sum(exp_euro_N{0}_{1}) as Total_N{0}", _strYearN1Id, i);
                }
                try
                {
                    //Nombre de références par mois	
                    if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal)
                    {
                        string cat = _session.GetSelection(_session.CurrentUniversMedia, CstRight.type.categoryAccess);
                        string med = _session.GetSelection(_session.CurrentUniversMedia, CstRight.type.mediaAccess);
                        if (cat.Length > 0) cat = string.Format("'{0}'", cat); else cat = "null";
                        if (med.Length > 0) med = string.Format("'{0}'", med); else med = "null";
                        sql.AppendFormat(", pkg_recap_test.NB_REF_BY_MONTH_TOTAL_MARKET('exp_euro_N{0}_{1}','{2}',{3},{4}) as nbref",_strYearId, i, dataTable.Label, cat, med);
                        sql.AppendFormat(", pkg_recap_test.FIRST_PRODUCT_TOTAL_MARKET('exp_euro_N{0}_{1}','{2}',{3},{4}) as id_product",_strYearId, i, dataTable.Label, cat, med);
                        sql.AppendFormat(", pkg_recap_test.FIRST_ADVERTISER_TOTAL_MARKET('exp_euro_N{0}_{1}','{2}',{3},{4}) as id_advertiser", _strYearId, i, dataTable.Label, cat, med);
                        sql.AppendFormat(", {0} as MOIS", i);
                    }

                }
                catch (Exception firtsErr)
                {
                    throw new ProductClassIndicatorsDALException("Unable to spot the first advertiser, referenceand number of reference for the total : " + firtsErr.Message, firtsErr);
                }
                #endregion

                sql.Append(" from");

                #region From
                sql.AppendFormat(" {0}", dataTable.SqlWithPrefix);
                #endregion

                sql.Append(" where");

                #region where
                sql.AppendFormat(" {0}", this.GetMediaSelection(dataTable.Prefix));

                if (_session.ComparaisonCriterion == CstComparaisonCriterion.universTotal)
                {
                    // Product selection
                    if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                        sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                    // Product rights
                    sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
                }

                //Si la requete porte sur le total famille
                if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal)
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

                #endregion
                
                sql.Append(" ) request1");
                sql.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
                sql.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
                #endregion

                sql.Append(" where");

                #region Where

                sql.AppendFormat(" {0}.id_advertiser=request1.id_advertiser and {0}.id_language={1}", _recapAdvertiser.Prefix, _session.SiteLanguage);
                sql.AppendFormat(" and {0}.id_product=request1.id_product and {0}.id_language={1}", _recapProduct.Prefix, _session.SiteLanguage);


                #endregion

            }
            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
            try
            {
                ds = dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw new ProductClassIndicatorsDALException(string.Format("Unable to load data : {0}", sql), err);
            }
            #endregion

            return ds;
        }
        #endregion

        #region Internal methods

        #region GetExpenditureClause
        /// <summary>
        /// Get expenditureClause 
        /// </summary>
        /// <param name="withN1">True if N-1 monthes required</param>	
        /// <returns>SQL code of expenditure clause</returns>
        protected override string GetExpenditureClause(bool withN1)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlN1 = new StringBuilder();
            int iFstMonth = _periodBegin.Month;
            int iLstMonth = _periodEnd.Month;

            if (!_periodBegin.Equals(null) && !_periodEnd.Equals(null))
            {
                for (int i = iFstMonth; i <= iLstMonth; i++)
                {
                    if (i != iLstMonth)
                    {
                        sql.AppendFormat(" sum(exp_euro_N{0}_{1}) {2},", _strYearId, i, FctUtilities.Dates.GetMonthAlias(i, _iYearId, 3, _session));
                    }
                    else if (i == iLstMonth)
                    {
                        sql.AppendFormat(" sum(exp_euro_N{0}_{1}) {2}", _strYearId, i, FctUtilities.Dates.GetMonthAlias(i, _iYearId, 3, _session));
                    }
                    //N-1
                    if (withN1 && _session.ComparativeStudy && _iYearId < 2)
                    {
                        if (i != iLstMonth)
                        {
                            sqlN1.AppendFormat(" sum(exp_euro_N{0}_{1}) {2},", _strYearN1Id, i, FctUtilities.Dates.GetMonthAlias(i, _iYearN1Id, 3, _session));
                        }
                        else if (i == iLstMonth)
                        {
                            sqlN1.AppendFormat(" sum(exp_euro_N{0}_{1}) {2}", _strYearN1Id, i, FctUtilities.Dates.GetMonthAlias(i, _iYearN1Id, 3, _session));
                        }
                    }
                }
                if (sqlN1.Length > 0)
                {
                    sql.AppendFormat(", {0}", sqlN1);
                }
            }

            return sql.ToString(); ;
        }
        #endregion

        #endregion

    }
}
