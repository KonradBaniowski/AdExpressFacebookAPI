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

namespace TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines
{

    /// <summary>
    /// Provide data for Tops Indicators
    /// </summary>
    public class DALEngineTop:DALEngine
    {

        #region Attributes
        /// <summary>
        /// Year to get
        /// </summary>
        protected CstResult.PalmaresRecap.typeYearSelected _typeYear = CstResult.PalmaresRecap.typeYearSelected.currentYear;
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
        /// <param name="typeYear">Type of study</param>
        /// <param name="classifLevel">Detail of the indicator</param>
        public DALEngineTop(WebSession session, CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel):base(session){
            _typeYear = typeYear;
            _classifLevel = classifLevel;
        }
        #endregion

        #region Get Data
        /// <summary>
        /// Get data for Top indicators
        /// </summary>
        /// <returns>Tops data</returns>
        public DataSet GetData()
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();

            #region Request building
            Table dataTable = this.GetDataTable(true);
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
                sql.AppendFormat(", {0}", _recapProduct.SqlWithPrefix);
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
            if (_typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear){
                sql.Append(" order by total_N desc ");
            }
            else
            {
                sql.Append(" order by total_N1 desc ");
            }

            if (_session.ComparativeStudy && _typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear)
            {
                sql.Append(", total_N1 desc");
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
                throw new ProductClassIndicatorsDALException(string.Format("Error while loading data: {0}", sql), err);
            }
            #endregion

            return (ds);
        }
        #endregion
    }
}
