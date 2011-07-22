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
using CstWeb = TNS.AdExpress.Constantes.Web;
namespace TNS.AdExpressI.VP.DAL
{

    public abstract class VeillePromoDAL : IVeillePromoDAL
    {
        #region Variables
        /// <summary>
        /// Client's session
        /// </summary>
        protected WebSession _session = null;

        /// <summary>
        /// Period beginning date
        /// </summary>
        protected string _periodBeginningDate = "";
        /// <summary>
        /// Period end date
        /// </summary>
        protected string _periodEndDate = "";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        ///  <param name="session">session</param>
        public VeillePromoDAL(WebSession session)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
        }


        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="periodBeginningDate">period Beginning Date</param>
        /// <param name="periodEndDate">period End Date</param>
        public VeillePromoDAL(WebSession session, string periodBeginningDate, string periodEndDate)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
            _periodBeginningDate = periodBeginningDate;
            _periodEndDate = periodEndDate;
        }
        #endregion


        #region IVeillePromoDAL Membres
        /// <summary>
        /// Get Min Period
        /// </summary>
        /// <returns></returns>
        public virtual DataSet GetMinMaxPeriod()
        {
            DataSet ds = null;
            StringBuilder sql = new StringBuilder(5000);
            try
            {
                sql.Append(" select min(DATE_BEGIN_NUM) DATE_BEGIN_NUM,max(DATE_END_NUM) DATE_END_NUM from DATA_PROMOTION ");
                IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.vPromo);
                ds = dataSource.Fill(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exceptions.VeillePromoDALException(" Impossible to get min max Promo period ", ex);
            }
            return ds;

        }
        /// <summary>
        /// Retreive the data for Veille promo schedule result
        /// </summary>
        /// <returns>
        /// DataSet      
        public virtual DataSet GetBenchMarkData()
        {
            DataSet ds = null;
            string classifTableName = string.Empty, classifFieldName = string.Empty, classifOrderFieldName = string.Empty
                , classifJoinCondition = string.Empty, universFilter = string.Empty, persoTableName = string.Empty
                , persoJoins = string.Empty, segmtFields = string.Empty;

            try
            {
                StringBuilder sql = new StringBuilder(5000);


                string prefix = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;

                //Get Data base schema descritpion
                Schema schPromo = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.promo03);

                //Get data promo table
                Table dataPromo = WebApplicationParameters.GetDataTable(TableIds.dataPromotion, false);


                // Get the SQL fields corresponding to the classification's items  
                classifOrderFieldName ="";


                /* Get the classif SQL joins  code*/
                classifJoinCondition = "";//GetSqlJoins(prefix);



                //Segement fields                
                DetailLevelItemInformation segemntLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpSegment);
                segmtFields = segemntLevelInformation.GetSqlFieldId() + "," + segemntLevelInformation.GetSqlField();
                classifOrderFieldName = segemntLevelInformation.GetSqlField() + "," + segemntLevelInformation.GetSqlFieldId();
                classifFieldName = segmtFields;

                persoTableName = segemntLevelInformation.GetTableNameWithPrefix();
                classifTableName +=  schPromo.Label + "." + persoTableName;

                persoJoins = " and " + segemntLevelInformation.GetSqlFieldId() + "=" + prefix + "." + segemntLevelInformation.GetSqlFieldIdWithoutTablePrefix()
                + " and " + segemntLevelInformation.DataBaseTableNamePrefix + ".activation< " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                classifJoinCondition += persoJoins;

                //product fileds
                DetailLevelItemInformation productLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpProduct);
                classifFieldName += ","+productLevelInformation.GetSqlFieldId() + "," + productLevelInformation.GetSqlField();
                classifOrderFieldName += ","+ productLevelInformation.GetSqlField() + "," + productLevelInformation.GetSqlFieldId();

                classifTableName += "," + schPromo.Label + "." + productLevelInformation.GetTableNameWithPrefix();

                classifJoinCondition += " and " + productLevelInformation.GetSqlFieldId() + "=" + prefix + "." + productLevelInformation.GetSqlFieldIdWithoutTablePrefix()
                 + " and " + productLevelInformation.DataBaseTableNamePrefix + ".activation< " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;


                //Force brand field

                DetailLevelItemInformation brandLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpBrand);
                classifFieldName += "," + brandLevelInformation.GetSqlFieldId() + "," + brandLevelInformation.GetSqlField();
                classifOrderFieldName += "," + brandLevelInformation.GetSqlField() + "," + brandLevelInformation.GetSqlFieldId();

                classifTableName += "," + schPromo.Label + "." + brandLevelInformation.GetTableNameWithPrefix();

                classifJoinCondition += " and " + brandLevelInformation.GetSqlFieldId() + "=" + prefix + "." + brandLevelInformation.GetSqlFieldIdWithoutTablePrefix()
                 + " and " + brandLevelInformation.DataBaseTableNamePrefix + ".activation< " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;


                //get universe filters
                universFilter = GetUniversFilter(prefix);


                //SELECT

                sql.AppendFormat(" select ID_DATA_PROMOTION,{0}, date_begin_num, date_end_num  ", classifFieldName);
                sql.Append(" ,promotion_content, condition_visual, condition_text, promotion_brand, promotion_visual ");


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


                ds = _session.Source.Fill(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exceptions.VeillePromoDALException(" Impossible to get Promotion Data ", ex);
            }


            return ds;
        }
        /// <summary>
        /// Retreive the data for Veille promo schedule result
        /// </summary>
        /// <returns>
        /// DataSet      
        public virtual DataSet GetData()
        {
            DataSet ds = null;
            string classifTableName = string.Empty, classifFieldName = string.Empty, classifOrderFieldName = string.Empty
                , classifJoinCondition = string.Empty, universFilter = string.Empty, persoTableName = string.Empty
                , persoJoins = string.Empty, persoFields = string.Empty;

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

                //perso fields                
                if (!detailLevel.ContainDetailLevelItem(_session.PersonnalizedLevel))
                {
                    DetailLevelItemInformation persoLevelInformation = DetailLevelItemsInformation.Get(_session.PersonnalizedLevel);
                    persoFields = persoLevelInformation.GetSqlFieldId() + "," + persoLevelInformation.GetSqlField();
                    classifFieldName += "," + persoFields;

                    persoTableName = persoLevelInformation.GetTableNameWithPrefix();
                    classifTableName += "," + schPromo.Label + "." + persoTableName;

                    persoJoins = " and " + persoLevelInformation.GetSqlFieldId() + "=" + prefix + "." + persoLevelInformation.GetSqlFieldIdWithoutTablePrefix()
                    + " and " + persoLevelInformation.DataBaseTableNamePrefix + ".activation< " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                    classifJoinCondition += persoJoins;
                }
                //Force brand field
                if (!detailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.vpBrand) && _session.PersonnalizedLevel != DetailLevelItemInformation.Levels.vpBrand)
                {
                    DetailLevelItemInformation brandLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpBrand);
                    classifFieldName += "," + brandLevelInformation.GetSqlField();

                    classifTableName += "," + schPromo.Label + "." + brandLevelInformation.GetTableNameWithPrefix();

                    classifJoinCondition += " and " + brandLevelInformation.GetSqlFieldId() + "=" + prefix + "." + brandLevelInformation.GetSqlFieldIdWithoutTablePrefix()
                     + " and " + brandLevelInformation.DataBaseTableNamePrefix + ".activation< " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

                }

                //get universe filters
                universFilter = GetUniversFilter(prefix);


                //SELECT

                sql.AppendFormat(" select ID_DATA_PROMOTION,{0}, date_begin_num, date_end_num  ", classifFieldName);
                sql.Append(" ,promotion_content, condition_visual, condition_text, promotion_brand, promotion_visual ");


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


                ds = _session.Source.Fill(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exceptions.VeillePromoDALException(" Impossible to get Promotion Data ", ex);
            }


            return ds;
        }
        /// <summary>
        /// Retreive the data for Veille promo file result
        /// </summary>
        ///<param name="idDataPromotion">id Data Promotion</param>
        public virtual DataSet GetData(long idDataPromotion)
        {
            DataSet ds = null;
            try
            {
                StringBuilder sql = new StringBuilder(5000);

                //Get data promo table
                Table dataPromo = WebApplicationParameters.GetDataTable(TableIds.dataPromotion, false);
                Table promoProduct = WebApplicationParameters.GetDataTable(TableIds.promoProduct, false);
                Table promoCategory = WebApplicationParameters.GetDataTable(TableIds.promoCategory, false);
                Table promoSegment = WebApplicationParameters.GetDataTable(TableIds.promoSegment, false);
                Table promoCircuit = WebApplicationParameters.GetDataTable(TableIds.promoCircuit, false);
                Table promoBrand = WebApplicationParameters.GetDataTable(TableIds.promoBrand, false);

                //get universe filters
                string universFilter = (idDataPromotion<0) ? GetUniversFilter(dataPromo.Prefix) :"";

                sql.AppendFormat("select ID_DATA_PROMOTION, {0}.ID_PRODUCT , PRODUCT, {0}.ID_BRAND,BRAND, {0}.ID_SEGMENT, SEGMENT ", dataPromo.Prefix);
                sql.AppendFormat(", {0}.ID_CATEGORY, CATEGORY,{0}.ID_CIRCUIT ,CIRCUIT  ", dataPromo.Prefix);
                sql.AppendFormat(", CONDITION_VISUAL, CONDITION_TEXT, PROMOTION_BRAND, PROMOTION_VISUAL, PROMOTION_CONTENT, DATE_BEGIN_NUM, DATE_END_NUM", dataPromo.Prefix);

                //FROM
                sql.AppendFormat(" from  {0} ,{1} ,{2} ", dataPromo.SqlWithPrefix, promoCircuit.SqlWithPrefix, promoBrand.SqlWithPrefix);
                sql.AppendFormat(" ,{0} ,{1} ,{2} ", promoSegment.SqlWithPrefix, promoCategory.SqlWithPrefix, promoProduct.SqlWithPrefix);

                //WHERE
                sql.Append(" where 0=0 ");

                //Adding universe filters
                if (idDataPromotion>-1) sql.AppendFormat(" and ID_DATA_PROMOTION={0} ", idDataPromotion);
                sql.AppendFormat(" and {0}.ID_PRODUCT = {1}.ID_PRODUCT and  {1}.activation<{2}", dataPromo.Prefix, promoProduct.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                sql.AppendFormat(" and {0}.ID_CATEGORY =  {1}.ID_CATEGORY and  {1}.activation<{2}", dataPromo.Prefix, promoCategory.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                sql.AppendFormat(" and {0}.ID_SEGMENT =  {1}.ID_SEGMENT and  {1}.activation<{2}", dataPromo.Prefix, promoSegment.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                sql.AppendFormat(" and {0}.ID_BRAND =  {1}.ID_BRAND and  {1}.activation<{2}", dataPromo.Prefix, promoBrand.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                sql.AppendFormat(" and {0}.ID_CIRCUIT =  {1}.ID_CIRCUIT and  {1}.activation<{2}", dataPromo.Prefix, promoCircuit.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

                //universe filter to get all files for current selection
                sql.Append(universFilter);

                //ORDER BY
                sql.AppendFormat(" order by CIRCUIT,BRAND,SEGMENT,CATEGORY,PRODUCT, date_begin_num, date_end_num ");


                ds = _session.Source.Fill(sql.ToString());

            }
            catch (Exception ex)
            {

                throw new Exceptions.VeillePromoDALException(" Impossible to get one  Promotion File Data ", ex);
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


            //Filtering period
            if (_session.PeriodType != CstWeb.CustomerSessions.Period.Type.allHistoric)
            {
                sql.Append("  and (");
                sql.AppendFormat(" (( DATE_BEGIN_NUM >= {0} and DATE_BEGIN_NUM <= {1}) ", _periodBeginningDate, _periodEndDate);
                sql.AppendFormat(" or (DATE_END_NUM >= {0} and DATE_END_NUM <= {1})) ", _periodBeginningDate, _periodEndDate);
                sql.Append("  or ");
                sql.AppendFormat("  (( DATE_BEGIN_NUM <= {0} and  {0} <= DATE_END_NUM ) ", _periodBeginningDate);
                sql.AppendFormat(" or (DATE_BEGIN_NUM <= {0} and {0} <= DATE_END_NUM ))     ", _periodEndDate);
                sql.Append("  ) ");
            }
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
        protected virtual string GetProductClassifFilters(string prefix, bool beginByAnd)
        {
            StringBuilder sql = new StringBuilder();
            bool first = true;
            string segmentActivityAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.vpSegmentAccess);
            string categoryAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.vpSubSegmentAccess);
            string productAccess = _session.GetSelection(_session.SelectionUniversProduct, CustomerRightConstante.type.vpProductAccess);

            //Add Classification items in Access
            //Segment Activity
            if (!string.IsNullOrEmpty(segmentActivityAccess))
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" ({0}.id_segment in ({1}) ", prefix, segmentActivityAccess);
                first = false;
            }
            // Product Category
            if (!string.IsNullOrEmpty(categoryAccess))
            {
                if (!first) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
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
                    sql.Append(" ( ");
                }
                sql.AppendFormat("  {0}.id_product in ({1}) ", prefix, productAccess);
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
            string circuitAccess = _session.GetSelection(_session.SelectionUniversMedia, CustomerRightConstante.type.circuitAccess);
            string brandAccess = _session.GetSelection(_session.SelectionUniversMedia, CustomerRightConstante.type.vpBrandAccess);

            //Add Classification items in Access

            //Circuit
            if (!string.IsNullOrEmpty(circuitAccess))
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" ( {0}.id_circuit in ({1}) ", prefix, circuitAccess);
                first = false;
            }
            // Brand
            if (!string.IsNullOrEmpty(brandAccess))
            {
                if (!first) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and ");
                    sql.Append(" ( ");
                }
                sql.AppendFormat("  {0}.id_brand in ({1}) ", prefix, brandAccess);
                first = false;
            }
            if (!first) sql.Append(" ) ");

            return sql.ToString();
        }
    }
}
