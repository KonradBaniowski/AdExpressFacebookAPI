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
    /// Provide data for Summary Indicators
    /// </summary>
    public class DALEngineSummary:DALEngine
    {

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="typeYear">Type of study</param>
        /// <param name="classifLevel">Detail of the indicator</param>
        public DALEngineSummary(WebSession session, DALUtilities dalUtilities)
            : base(session, dalUtilities)
        {
        }
        #endregion

        #region Get Data
        /// <summary>
        /// Get data for summary indicators
        /// </summary>
        /// <param name="type">Type of total : univers, market, sector</param>
        /// <returns>INvestments of the type of total</returns>
        public DataSet GetInvestments(CstComparaisonCriterion type)
        {
			
            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
			Table dataTable = this.GetDataTable(true);

            #region Request building

            sql.Append(" select total_N ");
            if (_session.ComparativeStudy)
            {
                sql.Append(" ,total_N1");
                //Evolution K€ (year N) / K€ (year N-1).
                sql.Append(",decode(total_N1,0,null,ROUND(((total_N/total_N1)*100)-100,0)) as evol");
                //Difference : 	K€ (N) - K€ (N-1).
                sql.Append(" ,total_N-total_N1 as ecart ");
            }
            sql.Append(" from ( ");

            #region "In" request
            sql.AppendFormat(" select {0}", this.GetExpenditureClause());
            sql.AppendFormat(" from {0}", dataTable.SqlWithPrefix);
            sql.Append(" where");

            #region Product classification
            bool and = false;
            if (type == CstComparaisonCriterion.universTotal)
            {
                //Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                {
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false));
					and = true;
                }
                //product rights
                //sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, false));
				sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, and));
                and = true;
            }
            if (type == CstComparaisonCriterion.sectorTotal)
            {
                sql.AppendFormat(" {0}.id_sector in ( ", dataTable.Prefix);
                sql.Append(" select distinct id_sector ");
                sql.AppendFormat(" from {0} where", dataTable.SqlWithPrefix);
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false, TNS.Classification.Universe.AccessType.includes));

                sql.Append(" ) ");
                and = true;
            }
            #endregion

            #region sélection des médias
            if (and) sql.Append(" and ");
            sql.AppendFormat(" {0}", _utilities.GetMediaSelection(dataTable.Prefix));
            #endregion

            #endregion

            sql.Append(" ) ");
            sql.Append(" where total_N is not null");
            if (_session.ComparativeStudy)
            {
                sql.Append(" or total_N1 is not null");
            }

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

        /// <summary>
        /// Get data for summary indicators
        /// </summary>
        /// <param name="type">Type of total : univers, market, sector</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <returns>INvestments of the type of total</returns>
        public DataSet GetVolumes(CstComparaisonCriterion type, CstResult.MotherRecap.ElementType classifLevel)
        {

            DataSet ds = null;
            StringBuilder sql = new StringBuilder();
            StringBuilder sql2 = new StringBuilder();
            Table dataTable = this.GetDataTable(true);

            #region Request Model building
            string idElement = (classifLevel == CstResult.MotherRecap.ElementType.product) ? "id_product" : "id_advertiser";
            sql.AppendFormat(" select count({0}) as nbElt ", idElement);//Debut requete principal					
            sql.Append(" from ( ");

            #region "In" request
            sql.AppendFormat(" select distinct {0}", idElement);
            sql.Append(", {0} as {1}");
            sql.AppendFormat(" from {0}", dataTable.SqlWithPrefix);

            sql.Append(" where");

            #region Product classification
            if (type == CstComparaisonCriterion.universTotal)
            {
                //Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false));
                //product rights
				//sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
				sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix,dataTable.Prefix, true));
                sql.Append(" and ");
            }
            if (type == CstComparaisonCriterion.sectorTotal)
            {
                sql.AppendFormat(" {0}.id_sector in ( ", dataTable.Prefix);
                sql.Append(" select distinct id_sector ");
                sql.AppendFormat(" from {0} where", dataTable.SqlWithPrefix);
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false, TNS.Classification.Universe.AccessType.includes));

                sql.Append(" ) and ");

            }
            #endregion

            #region sélection des médias
            sql.AppendFormat(" {0}", _utilities.GetMediaSelection(dataTable.Prefix));
            #endregion

            sql.AppendFormat(" group by {0}", idElement);
            #endregion

            sql.Append(" ) where {1} > 0 ");
            
            #endregion

            #region Request building
            string model = sql.ToString();
            sql2.AppendFormat(model, this.GetExpenditureClause(CstResult.PalmaresRecap.typeYearSelected.currentYear), "total_n");
            if (_session.ComparativeStudy)
            {
                sql2.Append(" UNION ALL ");
                sql2.AppendFormat(model, this.GetExpenditureClause(CstResult.PalmaresRecap.typeYearSelected.previousYear), "total_n1");
            }
            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
            try
            {
                ds = dataSource.Fill(sql2.ToString());
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
