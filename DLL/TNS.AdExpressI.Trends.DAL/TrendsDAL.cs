#region Information
/*
 * Author : G Facon
 * Creation : 09/10/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Units;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using TNSExceptions = TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using WebFunctions = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web;
#endregion


namespace TNS.AdExpressI.Trends.DAL
{

    /// <summary>
    /// Trends Report DAL
    /// </summary>
    public abstract class TrendsDAL : ITrendsDAL
    {

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session which contains user configuration parameters and universe selection</param>
        public TrendsDAL(WebSession session)
        {
            this._session = session;

            #region Selection of the media type
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new TNSExceptions.VehicleException("Selection of media type is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

        }
        #endregion

        #region ITrendsDAL Membres
        /// <summary>
        /// Retreive the data for the trends report result
        /// </summary>
        /// <returns>
        /// DataSet Containing the following tables
        /// 
        /// Tables["TOTAL"] -> DataTable for the Total  (Media Type level) 
        /// Tables["Levels"] -> DataTable for the levels  (Media Category level\Media Vehicle level)
        /// </returns>
        public virtual DataSet GetData()
        {

            #region Variable
            DataSet ds = new DataSet();
            #endregion

            // Get the levels requestedName
            // Used to known how many levels can be shown
            // For example in Finland AdExpress cannot display media vehicle. So it only display:
            // Media type\Media Category
            TNS.AdExpress.Domain.Level.GenericDetailLevel levelRequested = _session.DetailLevel;

            #region Build SQL Level 1 = TOTAL

            try
            {
                DataSet ds1 = _session.Source.Fill(GetTotalTendencies());
                ds1.Tables[0].TableName = "TOTAL";
                ds.Tables.Add(ds1.Tables[0].Copy());
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Request Error for Trends report Level 1: ", err));
            }



            #endregion

            #region Build SQL Levels 2 to N


            try
            {
                View allMediaView = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);
                DataSet ds2 = _session.Source.Fill(GetLevelsRequest(levelRequested, DBConstantes.Hathor.TYPE_TENDENCY_SUBTOTAL, allMediaView));
                ds2.Tables[0].TableName = "Levels";
                ds.Tables.Add(ds2.Tables[0].Copy());
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Request Error for Trends report Level 1: ", err));
            }

            #endregion


            return (ds);
        }
        #endregion



        /// <summary>
        /// Retrieve the data in levels for the trends report
        /// </summary>
        /// <param name="trendsTable">Trends table to use</param>
        /// <param name="levelInformation">Level information needed</param>
        /// <param name="trendsTypeId">Type of data to retrieve (Total or subtotal)</param>
        /// <param name="levels">Classification levels</param>
        /// <param name="allMediaView">All media View </param>
        /// <returns>Levels SQL string</returns>
        protected virtual string GetLevelsRequest(TNS.AdExpress.Domain.Level.GenericDetailLevel levels, string trendsTypeId, View allMediaView)
        {

            #region Variables
            StringBuilder sqlRequest = new StringBuilder(1000);
            string trendsTotalTableName = "";
            string unitsConditions = "";
            Table trendsTable = WebFunctions.SQLGenerator.GetTrendTableInformation(_session.DetailPeriod);
            Table totalTable = WebFunctions.SQLGenerator.GetTrendTotalTableInformtation(_session.DetailPeriod);
            string trendPrefix = (levels.GetNbLevels > 2) ? trendsTable.Prefix : totalTable.Prefix;
            #endregion

            // Get valid units that AdExpress has to display in the trend report according to the media
            List<UnitInformation> units = _session.GetValidUnitForResult();

            // In the trends report, the First level is the total (Media type)

            //Get the table description
            trendsTotalTableName = totalTable.SqlWithPrefix;

            sqlRequest.Append("Select " + trendPrefix + ".DATE_PERIOD,");

            //Get media classification fields
            sqlRequest.Append(GetSelectClauseFields(levels, trendsTable, totalTable, allMediaView));

            //Get Unit items for select
            unitsConditions = GetUnitFieldsForTendency(levels, units, trendsTable, totalTable, allMediaView);
            if (unitsConditions.Length == 0) throw (new UnitException("Trends Request build: No unit available"));
            sqlRequest.Append("," + unitsConditions.Substring(0, unitsConditions.Length - 2) + " ");

            // From SQL instruction
            sqlRequest.Append(GetTables(levels, trendsTable, totalTable, allMediaView));

            // Where SQL instruction
            sqlRequest.Append(" where 0=0 ");

            //Build Media classification join
            sqlRequest.Append(GetLevelsJoins(levels, trendsTable, totalTable, allMediaView));

            //Filters conditions
            sqlRequest.Append(GetLevelsFilters(levels, trendsTable, totalTable));

            // Group by
            sqlRequest.Append(" group by " + GetLevelsGroupBy(levels, trendsTable, totalTable, units, allMediaView));

            // Order by
            sqlRequest.Append(" order by " + GetLevelsOrderBy(levels, trendsTable, totalTable, allMediaView));

            return (sqlRequest.ToString());
        }

        /// <summary>
        /// Get query for line total
        /// </summary>        
        /// <returns>Sql query string</returns>
        protected virtual string GetTotalTendencies()
        {

            #region Variables
            string dataTotalTableName = "";
            StringBuilder sqlRequest = new StringBuilder(2000);
            #endregion

            #region Building Query
            Table dataTotalTable = WebFunctions.SQLGenerator.GetTrendTotalTableInformtation(_session.DetailPeriod);

            sqlRequest.Append(" select * from " + dataTotalTable.Sql);
            sqlRequest.Append(" where id_vehicle=" + _vehicleInformation.DatabaseId.ToString());

            if (_session.PDM)
            {
                sqlRequest.Append(" and ID_PDM = " + DBConstantes.Hathor.PDM_TRUE);
            }
            else
            {
                sqlRequest.Append(" and ID_PDM = " + DBConstantes.Hathor.PDM_FALSE);
            }
            sqlRequest.Append(" and id_type_tendency=" + DBConstantes.Hathor.TYPE_TENDENCY_TOTAL);
            if (_session.PeriodType == CustomerSessions.Period.Type.cumlDate)
            {
                sqlRequest.Append(" and ID_CUMULATIVE =" + DBConstantes.Hathor.CUMULATIVE_TRUE);
            }
            else
            {
                sqlRequest.Append(" and DATE_PERIOD between " + _session.PeriodBeginningDate + " and " + _session.PeriodEndDate);
                sqlRequest.Append(" and ID_CUMULATIVE =" + DBConstantes.Hathor.CUMULATIVE_FALSE);
            }
            #endregion

            return sqlRequest.ToString();



        }

        /// <summary>
        /// Get unit fields
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="units">Unit list</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>
        /// <param name="allMediaView">all media view</param>
        /// <returns>unit fields sql string</returns>
        protected virtual string GetUnitFieldsForTendency(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, List<UnitInformation> units, Table trendsTable, Table totalTable, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            bool withSum = false;
            DetailLevelItemInformation levelInformation = null;
            string vehicleField = "";

            //Get Unit items for select
            foreach (UnitInformation currentUnit in units)
            {
                switch (_vehicleInformation.Id)
                {
                    case DBClassificationConstantes.Vehicles.names.tv:
                    case DBClassificationConstantes.Vehicles.names.tvGeneral:
                    case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                    case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                    case DBClassificationConstantes.Vehicles.names.radio:
                    case DBClassificationConstantes.Vehicles.names.radioGeneral:
                    case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                    case DBClassificationConstantes.Vehicles.names.radioMusic:
                    case DBClassificationConstantes.Vehicles.names.others:

                        //Adding Level sub total
                        sqlRequest.AppendFormat(" {0}.{1}{2}{3}{1}{2},", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_cur ", " as SUB_");
                        sqlRequest.AppendFormat(" {0}.{1}{2}{3}{1}{2},", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_prev ", " as SUB_");
                        sqlRequest.AppendFormat(" {0}.{1}{2}{3}{1}{2},", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_evol ", " as SUB_");

                        if (levelsRequested.GetNbLevels > 2)
                        {
                            //Adding Lowest level
                            sqlRequest.AppendFormat(" {0}.{1}{2}", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_cur , ");
                            sqlRequest.AppendFormat(" {0}.{1}{2}", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_prev , ");
                            sqlRequest.AppendFormat(" {0}.{1}{2}", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_evol , ");
                        }
                        break;

                    case DBClassificationConstantes.Vehicles.names.press:
                    case DBClassificationConstantes.Vehicles.names.internationalPress:
                    case DBClassificationConstantes.Vehicles.names.magazine:
                    case DBClassificationConstantes.Vehicles.names.newspaper:
                        levelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);
                        vehicleField = allMediaView.Prefix + "." + levelInformation.DataBaseField;

                        //Adding Level sub total
                        sqlRequest.AppendFormat(" (sum({0}.{1}{2})/count({4})) {3}{1}{2}, ", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_cur", " as SUB_", vehicleField);
                        sqlRequest.AppendFormat(" (sum({0}.{1}{2})/count({4})) {3}{1}{2}, ", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_prev", " as SUB_", vehicleField);
                        sqlRequest.AppendFormat(" (sum({0}.{1}{2})/count({4})) {3}{1}{2}, ", totalTable.Prefix, currentUnit.DatabaseTrendsField, "_evol", " as SUB_", vehicleField);

                        if (levelsRequested.GetNbLevels > 2)
                        {
                            //Adding Lowest level
                            sqlRequest.AppendFormat(" sum({0}.{1}{2}) as {1}{2}, ", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_cur");
                            sqlRequest.AppendFormat(" sum({0}.{1}{2}) as {1}{2}, ", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_prev");
                            sqlRequest.AppendFormat(" decode(sum({0}.{1}{4}),0,100,round(((sum({0}.{1}{3})-sum({0}.{1}{4}))/sum({0}.{1}{4}))*100,'2')) as {1}{2}, ", trendsTable.Prefix, currentUnit.DatabaseTrendsField, "_evol", "_cur", "_prev");
                        }
                        break;
                    default:
                        throw new DataBaseException("Unknown Media type identifier.");
                }
            }

            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>
        /// <param name="allMediaView">all media view</param>
        /// <returns>tables string</returns>
        protected virtual string GetTables(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            DetailLevelItemInformation levelInformation = null;
            string levelsTablesName = " ," + allMediaView.Sql + _session.DataLanguage + " " + allMediaView.Prefix;

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.others:
                    // From SQL instruction
                    sqlRequest.AppendFormat(" from {0}", totalTable.SqlWithPrefix);
                    if (levelsRequested.GetNbLevels > 2)
                    {
                        sqlRequest.AppendFormat(", {0}", trendsTable.SqlWithPrefix);
                    }
                    sqlRequest.AppendFormat(levelsTablesName);
                    break;

                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    // From SQL instruction
                    sqlRequest.AppendFormat(" from {0}", totalTable.SqlWithPrefix);
                    sqlRequest.AppendFormat(", {0}", trendsTable.SqlWithPrefix);
                    sqlRequest.AppendFormat(levelsTablesName);
                    break;
                default:
                    throw new DataBaseException("Unknown Media type identifier.");
            }
            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get classification levels joins
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>
        /// <param name="allMediaView">all media view</param>
        /// <returns>SQl joins string</returns>
        protected virtual string GetLevelsJoins(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            DetailLevelItemInformation levelInformation = null;
            string trendPrefix = "";
            string allMediaPrefix = allMediaView.Prefix;

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.others:

                    if (levelsRequested.GetNbLevels > 2) trendPrefix = trendsTable.Prefix;
                    else trendPrefix = totalTable.Prefix;
                    break;

                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    trendPrefix = trendsTable.Prefix;
                    if (!levelsRequested.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media)
                    && levelsRequested.ContainDetailLevelItem(DetailLevelItemInformation.Levels.title))
                    {
                        DetailLevelItemInformation mediaLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);
                        sqlRequest.Append(" and " + allMediaPrefix + "." + mediaLevelInformation.GetSqlFieldIdWithoutTablePrefix() + "=" + trendPrefix + "." + mediaLevelInformation.GetSqlFieldIdWithoutTablePrefix());
                    }

                    break;
                default:
                    throw new DataBaseException(" Unknown Media type identifier. ");
            }
            for (int i = 1; i <= levelsRequested.GetNbLevels; i++)
            {
                levelInformation = levelsRequested[i];
                if (levelInformation.Id != DetailLevelItemInformation.Levels.vehicle
                    && levelInformation.Id != DetailLevelItemInformation.Levels.title)
                {
                    sqlRequest.Append(" and " + trendPrefix + "." + levelInformation.GetSqlFieldIdWithoutTablePrefix() + "=" + allMediaPrefix + "." + levelInformation.GetSqlFieldIdWithoutTablePrefix() + " ");
                }
            }
            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get Levels Filtering options
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>       
        /// <returns></returns>
        protected virtual string GetLevelsFilters(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            string trendPrefix = "";

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.others:
                    trendPrefix = totalTable.Prefix;
                    if (levelsRequested.GetNbLevels > 2)
                    {
                        trendPrefix = trendsTable.Prefix;
                        sqlRequest.Append(" and " + trendPrefix + ".ID_PDM = " + totalTable.Prefix + ".ID_PDM ");
                        sqlRequest.Append(" and " + trendPrefix + ".ID_CATEGORY = " + totalTable.Prefix + ".ID_CATEGORY ");
                        sqlRequest.Append(" and " + trendPrefix + ".ID_CUMULATIVE = " + totalTable.Prefix + ".ID_CUMULATIVE ");
                        sqlRequest.Append(" and " + trendPrefix + ".DATE_PERIOD = " + totalTable.Prefix + ".DATE_PERIOD ");

                    }
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    trendPrefix = trendsTable.Prefix;
                    sqlRequest.Append(" and " + trendPrefix + ".ID_PDM = " + totalTable.Prefix + ".ID_PDM ");
                    sqlRequest.Append(" and " + trendPrefix + ".ID_CATEGORY = " + totalTable.Prefix + ".ID_CATEGORY ");
                    sqlRequest.Append(" and " + trendPrefix + ".ID_CUMULATIVE = " + totalTable.Prefix + ".ID_CUMULATIVE ");
                    sqlRequest.Append(" and " + trendPrefix + ".DATE_PERIOD = " + totalTable.Prefix + ".DATE_PERIOD ");
                    break;
                default:
                    throw new DataBaseException("Unknown Media type identifier.");
            }

            // Media type working set
            sqlRequest.Append("and " + trendPrefix + ".id_vehicle=" + _vehicleInformation.DatabaseId.ToString());

            //Report in PDM
            if (_session.PDM)
            {
                sqlRequest.Append("and " + trendPrefix + ".id_pdm = " + DBConstantes.Hathor.PDM_TRUE + " ");
            }
            else
            {
                sqlRequest.Append("and " + trendPrefix + ".id_pdm = " + DBConstantes.Hathor.PDM_FALSE + " ");
            }

            // Period
            if (_session.PeriodType == CustomerSessions.Period.Type.cumlDate)
            {
                sqlRequest.Append("and " + trendPrefix + ".id_cumulative =" + DBConstantes.Hathor.CUMULATIVE_TRUE + " ");
            }
            else
            {
                sqlRequest.Append("and " + trendPrefix + ".date_period between " + _session.PeriodBeginningDate + " and " + _session.PeriodEndDate + " ");
                sqlRequest.Append("and " + trendPrefix + ".id_cumulative =" + DBConstantes.Hathor.CUMULATIVE_FALSE + " ");
            }

            // Level 0=Total
            sqlRequest.Append("and id_type_tendency=" + DBConstantes.Hathor.TYPE_TENDENCY_SUBTOTAL + " ");

            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get Levels Group By clause
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param> 
        /// <param name="units">Unit list</param>
        /// <param name="allMediaView">all media view</param>
        /// <returns>SQl group by string</returns>
        protected virtual string GetLevelsGroupBy(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable, List<UnitInformation> units, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            DetailLevelItemInformation levelInformation = null;
            bool first = true;
            string trendPrefix = "";
            // string unitFields = "";
            string allMediaPrefix = allMediaView.Prefix;

            for (int i = 1; i <= levelsRequested.GetNbLevels; i++)
            {
                levelInformation = levelsRequested[i];
                if (levelInformation.Id != DetailLevelItemInformation.Levels.vehicle)
                {
                    if (!first) sqlRequest.Append(" , ");
                    sqlRequest.Append(allMediaPrefix + "." + levelInformation.GetSqlIdFieldForGroupByWithoutTablePrefix() + "," + allMediaPrefix + "." + levelInformation.GetSqlFieldForGroupByWithoutTablePrefix());
                    first = false;
                }
            }
            //Get Unit items for select
            foreach (UnitInformation currentUnit in units)
            {
                switch (_vehicleInformation.Id)
                {
                    case DBClassificationConstantes.Vehicles.names.tv:
                    case DBClassificationConstantes.Vehicles.names.tvGeneral:
                    case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                    case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                    case DBClassificationConstantes.Vehicles.names.radio:
                    case DBClassificationConstantes.Vehicles.names.radioGeneral:
                    case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                    case DBClassificationConstantes.Vehicles.names.radioMusic:
                    case DBClassificationConstantes.Vehicles.names.others:
                        trendPrefix = totalTable.Prefix;
                        //Adding Level sub total
                        sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_cur ");
                        sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_prev ");
                        sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_evol ");

                        if (levelsRequested.GetNbLevels > 2)
                        {
                            trendPrefix = trendsTable.Prefix;
                            //Adding Lowest level
                            sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_cur ");
                            sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_prev ");
                            sqlRequest.AppendFormat(" ,{0}.{1}{2}", trendPrefix, currentUnit.DatabaseTrendsField, "_evol ");
                        }
                        break;

                    case DBClassificationConstantes.Vehicles.names.press:
                    case DBClassificationConstantes.Vehicles.names.internationalPress:
                    case DBClassificationConstantes.Vehicles.names.newspaper:
                    case DBClassificationConstantes.Vehicles.names.magazine:
                        trendPrefix = (levelsRequested.GetNbLevels > 2) ? trendsTable.Prefix : totalTable.Prefix;
                        break;
                    default:
                        throw new DataBaseException("Unknown Media type identifier.");
                }
            }

            sqlRequest.Append(", " + trendPrefix + ".DATE_PERIOD ");

            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get Levels Order By clause
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>         
        /// <param name="allMediaView">all media view</param>
        /// <returns>SQl Order by string</returns>
        protected virtual string GetLevelsOrderBy(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            DetailLevelItemInformation levelInformation = null;
            bool first = true;
            string allMediaPrefix = allMediaView.Prefix;

            for (int i = 1; i <= levelsRequested.GetNbLevels; i++)
            {
                levelInformation = levelsRequested[i];
                if (levelInformation.Id != DetailLevelItemInformation.Levels.vehicle)
                {
                    if (!first) sqlRequest.Append(" , ");
                    sqlRequest.Append(allMediaPrefix + "." + levelInformation.GetSqlFieldForOrderWithoutTablePrefix());
                    first = false;
                }
            }

            return sqlRequest.ToString();
        }

        /// <summary>
        /// Get Levels SELECT clause
        /// </summary>
        /// <param name="levelsRequested">Classification levels</param>
        /// <param name="trendsTable">Trend table</param>
        /// <param name="totalTable">total table</param>         
        /// <param name="allMediaView">all media view</param>
        /// <returns>SQl SELECT clause string</returns>
        protected virtual string GetSelectClauseFields(TNS.AdExpress.Domain.Level.GenericDetailLevel levelsRequested, Table trendsTable, Table totalTable, View allMediaView)
        {
            StringBuilder sqlRequest = new StringBuilder(2000);
            DetailLevelItemInformation levelInformation = null;
            string allMediaPrefix = allMediaView.Prefix;

            bool first = true;
            for (int i = 1; i <= levelsRequested.GetNbLevels; i++)
            {
                levelInformation = levelsRequested[i];
                if (levelInformation.Id != DetailLevelItemInformation.Levels.vehicle)
                {
                    if (!first) sqlRequest.Append(" , ");
                    sqlRequest.Append(allMediaPrefix + "." + levelInformation.GetSqlFieldIdWithoutTablePrefix() + "," + allMediaPrefix + "." + levelInformation.GetSqlFieldWithoutTablePrefix());
                    first = false;
                }
            }
            return sqlRequest.ToString();
        }
    }
}
