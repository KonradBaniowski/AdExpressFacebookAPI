using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Rolex.DAL
{
    public class RolexDAL : IRolexDAL
    {
        const string DATE_BEGIN_NUM = "DATE_BEGIN_NUM", DATE_END_NUM = "DATE_END_NUM";

        #region Session
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// User Session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion

        /// <summary>
        /// Period beginning date
        /// </summary>
        protected string _periodBeginningDate = "";
        /// <summary>
        /// Period end date
        /// </summary>
        protected string _periodEndDate = "";

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        public RolexDAL(WebSession session)
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
        public RolexDAL(WebSession session, string periodBeginningDate, string periodEndDate)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
            _periodBeginningDate = periodBeginningDate;
            _periodEndDate = periodEndDate;
        }

        #endregion

        #region GetMinMaxPeriod
        /// <summary>
        /// Get Min Period
        /// </summary>
        /// <returns></returns>
        public virtual DataSet GetMinMaxPeriod()
        {
            DataSet ds = null;
            var sql = new StringBuilder(5000);

            sql.Append(" select min(DATE_BEGIN_NUM) DATE_BEGIN_NUM,max(DATE_END_NUM) DATE_END_NUM from DATA_ROLEX ");            
            return  _session.Source.Fill(sql.ToString());
        }
        #endregion

        #region GetData
        /// <summary>
        /// Retreive the data for Rolex schedule result
        /// </summary>
        /// <returns>
        /// DataSet  
        /// </returns>
        public virtual DataSet GetData(GenericDetailLevel detailLevel)
        {

            var sql = new StringBuilder();

            //Get Data base schema descritpion
            var schRolex = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.rolex03);

            //Get data rolex table
            var dataRolex = WebApplicationParameters.GetDataTable(TableIds.dataRolex, false);

            // Get the classification table
            var mediaTableName = detailLevel.GetSqlTables(schRolex.Label);

            // Get the classification fields
            var mediaFieldName = detailLevel.GetSqlFields();

            // Get the gourp by fields
            var groupByFieldName = detailLevel.GetSqlGroupByFields();

            // Get the gourp by fields
            var orderByFieldName = detailLevel.GetSqlOrderFields();

            // Get joins for classification

            var mediaJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, dataRolex.Prefix);

            // Select : Media classificaion selection
            sql.AppendFormat("select {0}, ", mediaFieldName);

            //Select periods
            sql.AppendFormat(" to_number(to_char(to_date({0}, 'YYYYMMDD'), 'IYYYIW')) as date_num ", DATE_BEGIN_NUM);

            // From : Tables
            sql.AppendFormat(" from {0},{1} ", mediaTableName, dataRolex.SqlWithPrefix);

            // Where : Conditions media
            sql.AppendFormat("where 0=0 {0}", mediaJoinCondition);

            //Filtering period
            if (_session.PeriodType != CstWeb.CustomerSessions.Period.Type.allHistoric)
            {
                sql.AppendFormat("and  DATE_BEGIN_NUM >= {0} and DATE_BEGIN_NUM <= {1} ", _periodBeginningDate, _periodEndDate);
            }
            //Filtering on  sites
            sql.Append(GetSiteClassifFilters(dataRolex.Prefix, true));

            //Filtering on location
            sql.Append(GetClassifFilters(dataRolex.Prefix, true, _session.SelectedLocations, "id_location"));

            //Filtering on presence types
            sql.Append(GetClassifFilters(dataRolex.Prefix, true, _session.SelectedPresenceTypes, "id_presence_type"));

            // Group by
            sql.AppendFormat(" Group by {0} , {1}", groupByFieldName, DATE_BEGIN_NUM);

            // Order by
            sql.AppendFormat(" Order by {0} , {1}", orderByFieldName, DATE_BEGIN_NUM);


            return _session.Source.Fill(sql.ToString());
        }
        #endregion

        /// <summary>
        /// Get site without visibility
        /// </summary>
        /// <param name="detailLevelInformation">detail Level Information</param>
        /// <returns></returns>
        public virtual DataSet GetSitesWithoutVisibility(DetailLevelItemInformation detailLevelInformation)
        {
            var sql = new StringBuilder();


            //Get Data base schema descritpion
            var schRolex = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.rolex03);

            //Get data rolex table
            var dataRolex = WebApplicationParameters.GetDataTable(TableIds.dataRolex, false);


            // Select : Append site lfileds
            sql.AppendFormat("select {0}, {1} ", detailLevelInformation.GetSqlFieldId(), detailLevelInformation.GetSqlField());

            // From : Tables
            sql.AppendFormat(" from {0}.{1},{2} ", schRolex.Label, detailLevelInformation.GetTableNameWithPrefix(), dataRolex.SqlWithPrefix);

            // Where 
            sql.AppendFormat("where 0=0 ");

            sql.AppendFormat(" and {0}.{1}={2}.{1}(+)", detailLevelInformation.DataBaseTableNamePrefix, detailLevelInformation.DataBaseIdField, dataRolex.Prefix);

            sql.AppendFormat(" and {0}.id_language={1}", detailLevelInformation.DataBaseTableNamePrefix, _session.DataLanguage);
            sql.AppendFormat(" and {0}.activation<{1} ", detailLevelInformation.DataBaseTableNamePrefix , ActivationValues.UNACTIVATED);

            //Filtering period
            if (_session.PeriodType != CstWeb.CustomerSessions.Period.Type.allHistoric)
            {
                sql.AppendFormat("and  DATE_BEGIN_NUM (+)>= {0} and DATE_BEGIN_NUM (+)<= {1} ", _periodBeginningDate, _periodEndDate);
            }

            sql.Append(" and DATE_BEGIN_NUM is null ");

            //Only site with URL
            sql.Append(" and URL is not null ");

            sql.AppendFormat(" order by {0},{1}", detailLevelInformation.GetSqlField(),
                             detailLevelInformation.GetSqlFieldId());

            return _session.Source.Fill(sql.ToString());
        }


        #region GetFileData
        /// <summary>
        /// Retreive the data for Rolex schedule result
        /// </summary>
        /// <param name="selectedDetailLevel">selected Detail Level</param>
        /// <param name="selectedLevelValues">selected Level Values</param>
        /// <param name="detailLevel">detail Level</param>
        /// <returns>
        /// DataSet  
        /// </returns>
        public DataSet GetFileData(GenericDetailLevel selectedDetailLevel, List<long> selectedLevelValues, GenericDetailLevel detailLevel)
        {
            var sql = new StringBuilder();
            //Get Data base schema descritpion
            var schRolex = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.rolex03);

            //Get data rolex table
            var dataRolex = WebApplicationParameters.GetDataTable(TableIds.dataRolex, false);

            // Get the classification table
            var mediaTableName = detailLevel.GetSqlTables(schRolex.Label);

            // Get the classification fields
            var mediaFieldName = detailLevel.GetSqlFields();

            // Get the gourp by fields
            var groupByFieldName = detailLevel.GetSqlGroupByFields();

            // Get joins for classification
            var mediaJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, dataRolex.Prefix);



            // Select : Media classificaion selection
            sql.AppendFormat("select {0} ", mediaFieldName);

            //Select visuals
            sql.AppendFormat(",ID_PAGE, {0}.COMMENTARY, URL, VISUAL ,{1},{2}", dataRolex.Prefix, DATE_BEGIN_NUM, DATE_END_NUM);

            // From : Tables
            sql.AppendFormat(" from {0},{1} ", mediaTableName, dataRolex.SqlWithPrefix);

            // Where : Conditions media
            sql.AppendFormat("where 0=0 {0}", mediaJoinCondition);

            sql.AppendFormat("and  DATE_BEGIN_NUM >= {0} and DATE_BEGIN_NUM <= {1} ", _periodBeginningDate, _periodEndDate);

            //Filtering with selected classification levels
            for (int i = 1; i <= selectedDetailLevel.GetNbLevels; i++)
            {
                sql.AppendFormat(" and {0} in ({1})", selectedDetailLevel[i].GetSqlFieldId(), selectedLevelValues[i - 1]);
            }

            // Group by
            sql.AppendFormat("Group by {0} , {1}, {2}.COMMENTARY, URL, VISUAL,{3},{4} ", groupByFieldName, "ID_PAGE", dataRolex.Prefix, DATE_BEGIN_NUM, DATE_END_NUM);
            sql.Append(" ORDER BY SITE,ID_LOCATION asc,LOCATION,ID_PAGE,PRESENCE_TYPE ");

            return _session.Source.Fill(sql.ToString());
        }
        #endregion

        #region GetSiteClassifFilters
        /// <summary>
        /// Get site Classificaton  Filters
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="beginByAnd">begin By And</param>
        /// <returns>SQL string site Classificaton  Filters</returns>
        protected virtual string GetSiteClassifFilters(string prefix, bool beginByAnd)
        {
            var sql = new StringBuilder();
            string siteAccess = _session.GetSelection(_session.SelectionUniversMedia, Right.type.siteAccess);


            //Add Classification items in Access
            if (!string.IsNullOrEmpty(siteAccess))
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" {0}.id_site in ({1}) ", prefix, siteAccess);
            }
            return sql.ToString();
        }
        #endregion

        #region GetClassifFilters
        /// <summary>
        /// Get  Classificaton  Filters
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="beginByAnd">begin By And</param>
        /// <returns>SQL string  Classificaton  Filters</returns>
        protected virtual string GetClassifFilters(string prefix, bool beginByAnd, List<long> classifItems, string dbField)
        {
            var sql = new StringBuilder();

            //Add Classification items in Access
            if (classifItems != null && classifItems.Count > 0)
            {
                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" {0}.{2} in ({1}) ", prefix, string.Join(",", classifItems), dbField);
            }
            return sql.ToString();
        }
        #endregion
    }
}
