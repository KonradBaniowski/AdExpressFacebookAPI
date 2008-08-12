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
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL.Exceptions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines
{

    /// <summary>
    /// Provide data for Evolution Indicators
    /// </summary>
    public class DALEngineEvolution:DALEngine
    {

        #region Attributes
        /// <summary>
        /// Year to get
        /// </summary>
        protected CstResult.MotherRecap.ElementType _classifLevel = CstResult.MotherRecap.ElementType.advertiser;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="classifLevel">Detail of the indicator</param>
        public DALEngineEvolution(WebSession session, CstResult.MotherRecap.ElementType classifLevel)
            : base(session)
        {
            _classifLevel = classifLevel;
        }
        #endregion

        #region Get Data
        /// <summary>
        /// Get data for Evolution indicators
        /// </summary>
        /// <returns>Tops data</returns>
        public DataSet GetData()
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();

            #region Request building
            Table dataTable = this.GetDataTable(_classifLevel == CstResult.MotherRecap.ElementType.product);
            sql.AppendFormat(" select {0}.id_advertiser, {0}.advertiser, ", _recapAdvertiser.Prefix);
            if (_classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(" {0}.id_product, {0}.product,  ", _recapProduct.Prefix);
            }
            sql.Append(this.GetExpenditureClause());


            sql.Append(" from ");
            sql.AppendFormat(" {0}, ", dataTable.SqlWithPrefix);
            sql.AppendFormat(" {0} ", _recapAdvertiser.SqlWithPrefix);
            if (_classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(", {0}, ", _recapProduct.SqlWithPrefix);
            }

            sql.Append(" where  ");
            sql.AppendFormat(" {0}.id_advertiser={1}.id_advertiser", dataTable.Prefix, _recapAdvertiser.Prefix);
            sql.AppendFormat(" and {0}.id_language={1}", _recapAdvertiser.Prefix, _session.SiteLanguage);
            if (_classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                sql.AppendFormat(" and {0}.id_product={1}.id_product", dataTable.Prefix, _recapProduct.Prefix);
                sql.AppendFormat(" and {0}.id_language={1}", _recapProduct.Prefix, _session.SiteLanguage);
            }

            //Media Selection
            sql.AppendFormat(" and {0}", this.GetMediaSelection(dataTable.Prefix));

            #region Product classification
            // Product selection
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
            // Product rights
            sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
            #endregion


            if (_classifLevel == CstResult.PalmaresRecap.ElementType.product)
            {
                sql.AppendFormat( " group by {0}.id_advertiser, {0}.advertiser, {1}.id_product, {1}.product ", _recapAdvertiser.Prefix, _recapProduct.Prefix);
            }
            else
            {
                sql.AppendFormat( " group by {0}.id_advertiser, {0}.advertiser ", _recapAdvertiser.Prefix);
            }

            sql.Append(" order by  Ecart desc ");
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

        #region Get Expenditure Clause
        /// <summary>
        /// Build expenditure clause
        /// </summary>
        /// <returns>SQL code for expenditure clause</returns>
        protected override string GetExpenditureClause()
        {

            StringBuilder sql = new StringBuilder();
            StringBuilder sqlEvol = new StringBuilder();
            StringBuilder sqlN1 = new StringBuilder();

            #region dates (mensuels) des investissements
            int iStart = _periodBegin.Month;
            int iEnd = _periodEnd.Month;

            if (!_periodEnd.Equals(null) && !_periodBegin.Equals(null))
            {
                for (int i = iStart; i <= iEnd; i++)
                {
                    if (i == iEnd && iStart != iEnd)
                    {
                        sql.AppendFormat("exp_euro_N{0}_{1}) as total_N, ", _strYearId, i);
                        sqlEvol.AppendFormat("exp_euro_N{0}_{1} ", _strYearId, i);
                        sqlN1.AppendFormat("exp_euro_N{0}_{1})) as Ecart", _strYearN1Id, i);
                    }
                    else if (i == iStart && iStart != iEnd)
                    {
                        sql.AppendFormat("sum(exp_euro_N{0}_{1} + ", _strYearId, i);
                        sqlEvol.AppendFormat("sum(exp_euro_N{0}_{1} + ", _strYearId, i);
                        sqlN1.AppendFormat(" - (exp_euro_N{0}_{1} + ", _strYearN1Id, i);
                    }
                    else if (iStart == iEnd)
                    {
                        sql.AppendFormat("sum(exp_euro_N{0}_{1} ", _strYearId, i);
                        sqlEvol.AppendFormat("sum(exp_euro_N{0}_{1} ", _strYearId, i);
                        sqlN1.AppendFormat("-(exp_euro_N{0}_{1})) as Ecart", _strYearN1Id, i);
                    }
                    else
                    {
                        sql.AppendFormat("exp_euro_N{0}_{1} + ", _strYearId, i);
                        sqlEvol.AppendFormat("exp_euro_N{0}_{1} + ", _strYearId, i);
                        sqlN1.AppendFormat("exp_euro_N{0}_{1} + ", _strYearN1Id, i);
                    }

                }
                sql.Append(sqlEvol.ToString()).Append(sqlN1.ToString());
            }
            #endregion

            return sql.ToString();

        }
        #endregion
    }
}
