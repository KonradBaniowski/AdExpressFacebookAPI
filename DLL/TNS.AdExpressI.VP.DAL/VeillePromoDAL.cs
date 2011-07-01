using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using System.Collections;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.VP.DAL
{

    public abstract class VeillePromoDAL : IVeillePromoDAL
    {
        #region Variables
        /// <summary>
        /// Client's session
        /// </summary>
        protected WebSession _session = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VeillePromoDAL(WebSession session)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
        }
        #endregion


        #region IVeillePromoDAL Membres

        /// <summary>
        /// Retreive the data for Veille promo schedule result
        /// </summary>
        /// <returns>
        /// DataSet      
        public DataSet GetData()
        {
            DataSet ds = null;
            string classifTableName = string.Empty, classifFieldName = string.Empty, classifOrderFieldName = string.Empty
                , classifJoinCondition = string.Empty, universFilter = string.Empty;

            try
            {
                StringBuilder sql = new StringBuilder(5000);

                string prefix = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;

                //Get Data base schema descritpion
                Schema schPromo = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.promo03);

                //Get data promo table
                Table dataPromo = WebApplicationParameters.GetDataTable(TableIds.dataPromotion, false);

                //Get classification tables name
                classifTableName = _session.GenericMediaDetailLevel.GetSqlTables(schPromo.Label);

                // Get the SQL  fields corresponding to the classification's items                 
                classifFieldName = _session.GenericMediaDetailLevel.GetSqlFields();
             

                // Get the SQL fields corresponding to the classification's items  
                classifOrderFieldName = _session.GenericMediaDetailLevel.GetSqlOrderFields();


                /* Get the classif SQL joins  code*/
                classifJoinCondition = GetSqlJoins(prefix);

                //Get  classification levels selected
                GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

                //get universe filters
                universFilter = GetUniversFilter(prefix);


                //SELECT
                //sql.AppendFormat(" select ID_DATA_PROMOTION,{0}, date_begin_num, date_end_num  ", classifFieldName);
                //sql.Append(" ,promotion_content, condition_visual, condition_text, promotion_brand, promotion_visual ");
                sql.AppendFormat(" select * ");
             

                //FROM
                sql.AppendFormat(" from  {0} , {1}  ", classifTableName, dataPromo.SqlWithPrefix);
                
                //WHERE
                sql.Append(" where  0=0 ");

                //Adding universe filters
                sql.AppendFormat(" {0} ", universFilter);

                //Adding claasification joins
                sql.AppendFormat(" {0}", classifJoinCondition);
                
                //ORDER BY
                sql.AppendFormat(" order by {0}, date_begin_num, date_end_num ", classifOrderFieldName);

                IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.vPromo);
                
                ds = dataSource.Fill(sql.ToString()); 
            }
            catch (Exception ex)
            {

                throw new Exceptions.VeillePromoDALException(" Impossible to get Veille Promo Data ", ex);
            }


            return ds;
        }

        #endregion


        /// <summary>
        /// Obtient le code SQL des jointures correspondant aux éléments du niveau de détail
        /// </summary>
        /// <remarks>Début par And</remarks>
        /// <param name="languageId">Langue à afficher</param>
        /// <param name="dataTablePrefix">Préfixe de la table de données sur laquelle on fait la jointure</param>
        /// <returns>Code SQL</returns>
        protected virtual string GetSqlJoins(string dataTablePrefix)
        {
            if (dataTablePrefix == null || dataTablePrefix.Length == 0) throw (new ArgumentNullException("Parameter dataTablePrefix is invalid"));
            string sql = "";
            ArrayList levels = _session.GenericMediaDetailLevel.Levels;
            foreach (DetailLevelItemInformation currentLevel in levels)
            {
                if (currentLevel.DataBaseTableName != null)
                {
                    sql += " and " + currentLevel.DataBaseTableNamePrefix + "." + currentLevel.DataBaseIdField + "=" + dataTablePrefix + "." + currentLevel.DataBaseIdField;
                    sql += " and " + currentLevel.DataBaseTableNamePrefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

                }
            }
            return (sql);
        }

        /// <summary>
        /// Get differents universes filters options fr SQL r the query. 
        /// Filters can be users rigths, classification items selected or period selected.
        /// </summary>
        /// <returns>SQl string for universes filter</returns>
        protected virtual string GetUniversFilter(string dataTablePrefix)
        {
            StringBuilder sql = new StringBuilder();          

            //Customer period selected
            string periodBeginningDate = _session.PeriodBeginningDate;
            string periodEndDate = _session.PeriodEndDate;

              //Filtering period
            sql.Append("  and (");
            sql.AppendFormat(" (( DATE_BEGIN_NUM >= {0} and DATE_BEGIN_NUM <= {1}) ", periodBeginningDate, periodEndDate);
            sql.AppendFormat(" or (DATE_END_NUM >= {0} and DATE_END_NUM <= {1})) ", periodBeginningDate, periodEndDate);
            sql.Append("  or ");
            sql.AppendFormat("  (( DATE_BEGIN_NUM >= {0} and  {0} <= DATE_END_NUM ) ", periodBeginningDate);
            sql.AppendFormat(" or (DATE_BEGIN_NUM >= {0} and {0} <= DATE_END_NUM )) ", periodEndDate);
            sql.Append("  ) ");

            // Product classification Selection
            sql.Append(GetProductClassifFilters(dataTablePrefix, true));

            // Brand  Classification Selection
            sql.Append(GetBrandClassifFilters(dataTablePrefix, true));


            return sql.ToString();
        }

        /// <summary>
        /// Get Product Classificaton  Filters
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="beginByAnd">begin By And</param>
        /// <returns>SQL string Product Classificaton  Filters</returns>
        protected virtual string GetProductClassifFilters( string prefix,bool beginByAnd)
        {
            StringBuilder sql = new StringBuilder();
            bool first = true;          
            string segmentActivityAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.groupAccess);
            string segmentActivityException = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.groupException);
            string categoryAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.segmentAccess);
            string categoryException = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.segmentException);
            string productAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.productAccess);
            string productException = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.productException);

            //Add Classification items in Access
            //Segment Activity
            if (!string.IsNullOrEmpty(segmentActivityAccess))
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" (({0}.id_segment in ({1}) ", prefix, segmentActivityAccess);
                first = false;
            }
            // Product Category
            if (!string.IsNullOrEmpty(categoryAccess))
            {
                if (!first) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" (( ");
                }
                sql.AppendFormat("  {0}.id_category in ({1}) ", prefix, categoryAccess);
                first = false;
            }
            // Product
            if (!string.IsNullOrEmpty(productAccess))
            {
                if (!first) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" (( ");
                }
                sql.AppendFormat("  {0}.id_product in ({1}) ", prefix, productAccess);
                first = false;
            }
            if (!first) sql.Append(" )");

            //Add Classification items in exception
            // Segment Activity
            if (!string.IsNullOrEmpty(segmentActivityException))
            {
                if (!first) sql.Append(" and ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat(" {0}.id_sector not in ({1}) ",prefix,segmentActivityException);
                first = false;
            }
            // Product Category
            if (!string.IsNullOrEmpty(categoryException))
            {
                if (!first) sql.Append(" and ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat(" {0}.id_category not in ({1}) ", prefix, categoryException);
                first = false;
            }
            // Product
            if (!string.IsNullOrEmpty(productException))
            {
                if (!first) sql.Append(" and ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat(" {0}.id_product not in ({1}) ", prefix, productException);
                first = false;
            }
            if (!first) sql.Append(" )");

            return sql.ToString();
        }

        /// <summary>
        /// Get Brand Classificaton  Filters
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="beginByAnd">begin By And</param>
        /// <returns>SQL string Brand Classificaton  Filters</returns>
        protected virtual string GetBrandClassifFilters(string prefix, bool beginByAnd)
        {
            StringBuilder sql = new StringBuilder();
            bool first = true;          
            string circuitAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.circuitAccess);
            string circuitException = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.circuitException);
            string brandAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.brandAccess);
            string brandException = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.brandException);

            //Add Classification items in Access

            //Circuit
            if (!string.IsNullOrEmpty(circuitAccess))
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" (({0}.id_circuit in ({1}) ", prefix, circuitAccess);
                first = false;
            }
            // Brand
            if (!string.IsNullOrEmpty(brandAccess))
            {
                if (!first) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" (( ");
                }
                sql.AppendFormat("  {0}.id_brand in ({1}) ", prefix, brandAccess);
                first = false;
            }          
            if (!first) sql.Append(" ) ");

            //Add Classification items in exception

            // Circuit
            if (!string.IsNullOrEmpty(circuitException))
            {
                if (!first) sql.Append(" and ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat(" {0}.id_circuit not in ({1}) ", prefix, circuitException);
                first = false;
            }
            // Brand
            if (!string.IsNullOrEmpty(brandException))
            {
                if (!first) sql.Append(" and ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat(" {0}.id_brand not in ({1}) ", prefix, brandException);
                first = false;
            }           
            if (!first) sql.Append(" ) ");

            return sql.ToString();
        }
    }
}
