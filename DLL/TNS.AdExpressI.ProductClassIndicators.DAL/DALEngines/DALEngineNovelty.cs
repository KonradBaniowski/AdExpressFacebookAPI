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
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines
{

    /// <summary>
    /// Provide data for novelty Indicators
    /// </summary>
    public class DALEngineNovelty : DALEngine
    {

        #region Attributes
        /// <summary>
        /// Classification detail
        /// </summary>
        protected CstResult.MotherRecap.ElementType _classifLevel = CstResult.MotherRecap.ElementType.advertiser;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="classifLevel">Detail of the indicator</param>
        public DALEngineNovelty(WebSession session, CstResult.MotherRecap.ElementType classifLevel)
            : base(session)
        {
            _classifLevel = classifLevel;
        }
        #endregion

        #region Get Data
        /// <summary>
        /// Get data for Novelty indicators
        /// </summary>
        /// <returns>Novelty data</returns>
        public DataSet GetData()
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();

            #region Request building
            Table dataTable = this.GetDataTable(true);

            #region Check Period
            if (!HasData(_periodEnd))
            {
                return null;
            }
            #endregion

            #region Global select
            //To get new elements, check investments is null from the beginning of the year except for the current month
            sql.Append("select");
            if (this._classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                sql.Append(" id_advertiser, advertiser");
            }
            else
            {
                sql.Append(" id_product,product, id_advertiser");
            }
            //Investments of current month and from the beginning of the year
            string monthAlias = FctUtilities.Dates.CurrentActiveMonth(_periodEnd, _session);
            sql.AppendFormat(", {0}", monthAlias);
            if (_session.ComparativeStudy)
            {
                sql.AppendFormat(", {0}", GetN1Monthes());
            }
            //Sum of investments of the previous monthes from the beginning of the year
            //If sum is null, then element is new!
            string previousMonthes = this.GetPreviousMonthes();
            sql.Append(", isInactive");

            sql.Append(" from ( ");
            #endregion

            #region internal select

            #region select
            sql.Append("select");
            //classif
            if (this._classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                sql.AppendFormat(" {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            else
            {
                sql.AppendFormat(" {0}.id_product, {1}.product, {0}.id_advertiser", dataTable.Prefix, _recapProduct.Prefix);
            }
            //novelty month
            if (_periodEnd.Month == DateTime.Now.Month)
            {
                sql.AppendFormat(", sum(exp_euro_N_{0}) {1}", _periodEnd.AddMonths(-1).Month, FctUtilities.Dates.GetMonthAlias(_periodEnd.Month - 1, FctUtilities.Dates.yearID(_periodEnd.AddMonths(-1).Date, _session), 3, _session));
            }
            else
            {
                sql.AppendFormat(", sum(exp_euro_N{0}_{1}) {2}", _strYearId, _periodEnd.Month, FctUtilities.Dates.GetMonthAlias(_periodEnd.Month, FctUtilities.Dates.yearID(_periodEnd.Date, _session), 3, _session));
            }
            //previous monthes
            sql.AppendFormat(", {0}", previousMonthes);
            #endregion

            #region From
            sql.Append(" from");
            sql.AppendFormat(" {0}", dataTable.SqlWithPrefix);
            if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                sql.AppendFormat(", {0}", _recapAdvertiser.SqlWithPrefix);
            }
            else{
                sql.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
            }
            #endregion

            #region Joins
            sql.Append(" where");
            if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                sql.AppendFormat(" {0}.id_advertiser={1}.id_advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
                sql.AppendFormat(" and {0}.id_language={1}", _recapAdvertiser.Prefix, _session.SiteLanguage);
            }
            else{
                sql.AppendFormat(" {0}.id_product={1}.id_product", dataTable.Prefix, _recapProduct.Prefix);
                sql.AppendFormat(" and {0}.id_language={1}", _recapProduct.Prefix, _session.SiteLanguage);
            }
            #endregion

            #region Product Classification
            sql.Append(" ");
            // Product selection
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
            // Product rights
            sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
            #endregion

            #region sélection des médias
            sql.AppendFormat(" and {0}", this.GetMediaSelection(dataTable.Prefix));
            #endregion

            #region group by
            if (_classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(" group by {0}.id_product, {1}.product, {0}.id_advertiser", dataTable.Prefix, _recapProduct.Prefix);
            }
            else
            {
                sql.AppendFormat(" group by {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            #endregion

            #region sort
            if (_classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(" order by {0}.id_product, {1}.product, {0}.id_advertiser", dataTable.Prefix, _recapProduct.Prefix);
            }
            else
            {
                sql.AppendFormat(" order by {0}.id_advertiser, {1}.advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            }
            #endregion

            #endregion

            #region End of global select
            sql.AppendFormat(" ) where isInactive = 0 and {0} > 0", monthAlias);
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

        #region Period validate
        /// <summary>
        /// New products or advertisers are available after march, 20th if the period is the current year
        /// </summary>
        /// <param name="PeriodEndDate">End of the Period</param>
        /// <returns>True if the period is correct</returns>
        protected bool HasData(DateTime periodEndDate)
        {
            if (periodEndDate == null)
                return false;

            DateTime oldDate = new DateTime(DateTime.Now.Year, 3, 20);
            TimeSpan ts = periodEndDate - oldDate;
            int differenceInDays = 1;
            if (periodEndDate.Year.Equals(DateTime.Now.Year) || periodEndDate.Month == 1 || periodEndDate.Month == 2 || periodEndDate.Month == 3)
            {
                differenceInDays = ts.Days;
            }
            if (differenceInDays <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Get Monthes for N-1
        /// <summary>
        /// Get select clause for N-1 monthes
        /// </summary>
        /// <returns>Select clause for N-1 monthes</returns>
        protected string GetN1Monthes()
        {
            StringBuilder sql = new StringBuilder();


            if (_session.ComparativeStudy && (_iYearId == 0 || _iYearId == 1))
            {
                for (int j = 1; j <= 12; j++)
                {
                    if (j != 12)
                    {
                        sql.AppendFormat(" {0},", FctUtilities.Dates.GetMonthAlias(j, _iYearN1Id, 3, _session));
                    }
                    else
                    {
                        sql.AppendFormat(" {0}", FctUtilities.Dates.GetMonthAlias(j, _iYearN1Id, 3, _session));
                    }
                }
            }
            return sql.ToString();
        }
        #endregion

        #region Get Previous Monthes of studied Year
        /// <summary>
        /// Build sql code required to determine is an element is new or not
        /// </summary>
        /// <returns>Sql Code</returns>
        protected string GetPreviousMonthes()
        {
            StringBuilder sql = new StringBuilder();

            int iLastMonth = _periodEnd.Month;

            //N
            for (int i = 1; i <= iLastMonth - 1; i++)
            {
                if (iLastMonth != 1 && i != iLastMonth - 1)
                {
                    sql.AppendFormat(" sum(exp_euro_N{0}_{1}) +", _strYearId, i);
                }
                else if ((iLastMonth - 1 == i) || iLastMonth == 1)
                {
                    sql.AppendFormat(" sum(exp_euro_N{0}_{1}) isInactive", _strYearId, i);
                }
            }
            //N-1
            if (_session.ComparativeStudy && _iYearId < 2)
            {
                sql.Append(",");
                for (int j = 1; j <= 12; j++)
                {
                    if (j != 12)
                    {
                        sql.AppendFormat(" sum(exp_euro_N{0}_{1}) {2},", _strYearN1Id, j, FctUtilities.Dates.GetMonthAlias(j, _iYearN1Id, 3, _session));
                    }
                    else
                    {
                        sql.AppendFormat(" sum(exp_euro_N{0}_{1}) {2}", _strYearN1Id, j, FctUtilities.Dates.GetMonthAlias(j, _iYearN1Id, 3, _session));
                    }
                }
            }

            return sql.ToString(); ;

        }
        #endregion
    }
}
