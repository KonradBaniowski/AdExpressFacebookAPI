#region Information
/*
 * Author : Y R'kaina
 * Created on : 09/10/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using System.Reflection;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Exceptions;
using CstFrequency = TNS.AdExpress.Constantes.Customer.DB.Frequency;

namespace TNS.AdExpressI.Date.DAL {
    /// <summary>
    /// This class provides all the methods to determine specific dates for specific modules.
    /// The specific dates can be the last day in which we have data in the database or the last loaded year. 
    /// </summary>
    public class DateDAL : IDateDAL {

        #region Variables
        /// <summary>
        /// Client session
        /// </summary>
        protected TNS.AdExpress.Web.Core.Sessions.WebSession _session = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DateDAL() {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session</param>
        public DateDAL(WebSession session) {
            _session = session; 
        }
        #endregion


        

        #region IDateDAL Membres

        #region GetFirstDayNotEnabled
        /// <summary>
        /// Calculate the first day from which we don't have data in the database
        /// We check for the vehicles list selected (associated to the media type selected) the last publication date in the data table.
        /// if we don't have data, we calculate the previous year according to the startYear variable and the datetime returned is the last day of the previous year.
        /// <remarks>
        /// This method is used in the date selection page to show periods that have not yet been completely uploaded
        /// and this information is represented by using dark gray color in the calendar 
        /// </remarks>
        /// <example>
        /// The sub media selected : press
        /// startYear : 2008
        /// case 1 :
        ///     last publication date : 15/06/2008
        ///     value returned : last publication date + periodicity (daily : periodicity = 1 day, weekly : periodicity = 7 days, monthly : periodicity = 1 month)
        /// case 2 :
        ///     last publication date : not found = no data
        ///     value returned : 31/12/2007
        /// </example>
        /// </summary>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <param name="startYear">Used to calculate the fist day not enable : date format yyyy, example : 2008</param>
        public virtual DateTime GetFirstDayNotEnabled(List<Int64> selectedVehicleList, int startYear) {

            if (selectedVehicleList == null) throw new Exception.DateDALException("The methods is not implemented for 0 media type");
            if (selectedVehicleList.Count != 1) throw new Exception.DateDALException("The methods is not implemented for " + selectedVehicleList.Count + " media type");


            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;
            Int64 selectedVehicle = selectedVehicleList[0];
            IDataSource dataSource = GetDataSource();

            /* Get last publication date
             * */
            lastDate = GetLatestPublication(selectedVehicleList);
            /* Calculate the previous year
             * */
            startYear--;
            /* If there is no data, we return the last day of the previous year
             * */
            if (lastDate.Length == 0) lastDate = startYear + "1231";
            /* instantiate the last publication date 
             * */
            publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));

            /* Calculate the date from which we don't have data (we do this by adding the periodicity to the last publication date)
             * */
            switch (VehiclesInformation.DatabaseIdToEnum(selectedVehicle)) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.indoor:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                case DBClassificationConstantes.Vehicles.names.mms:
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.internet:
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

        #region GetLastLoadedYear
        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// </summary>      
        /// <returns>Year</returns>
        public virtual int GetLastLoadedYear() {

            #region Tables initilization
            Table recapInfo;

            try {
                /* Get the recap data table name
                 * */
                recapInfo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInfo);
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Query
            /* At first we get the max of the id_recap_info and after that we select the current_year which correspond to the last year which has data.
             * */
            string sql = "select current_year from " + recapInfo.Sql + " where id_recap_info = (select max(id_recap_info) from " + recapInfo.Sql + ")";
            #endregion

            #region Execution of the query
            try {
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
                /* Execution of the SQL request
                 * */
                DataSet ds = source.Fill(sql.ToString());
                return (int.Parse(ds.Tables[0].Rows[0]["current_year"].ToString()));
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Impossible to get the last loaded year in the database for the recap tables", err));
            }
            #endregion

        }
        #endregion

        #region GetLatestPublication
        /// <summary>
        /// Get the last publication date for a list of vehicles according to the media type passed to the method
        /// At first we select the max date for every vehicle, after that we select the min date from the list built in the first step 
        /// </summary>
        /// <example>
        /// The media type selected is press 
        /// The max date of the first vehicle is 01/05/2008
        /// The max date of the second vehicle is 15/04/2008
        /// The max date of the third vehicle is 21/01/2008
        /// The date returned will be the min of the three previous date so : 21/01/2008
        /// </example>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <returns>Last date publication : date format yyyyMMdd, example 20081015</returns>
        public virtual string GetLatestPublication(List<Int64> selectedVehicleList) {

            if (selectedVehicleList == null) throw new Exception.DateDALException("The methods is not implemented for 0 media type");
            if (selectedVehicleList.Count != 1) throw new Exception.DateDALException("The methods is not implemented for " + selectedVehicleList.Count + " media type");

            string sql = string.Empty;
            int positionUnivers = 1;
            string mediaList = string.Empty;
            string tableName = string.Empty;
            Int64 idVehicle = selectedVehicleList[0];
            IDataSource dataSource = GetDataSource();

            #region Building the query
            /* Get the data table name, example : DATA_PRESS, DATA_TV, DATA_RADIO ... 
             * */
            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInternet, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMail, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataPress, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMagazine, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataTv, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataRadio, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.indoor:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInDoor, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.instore:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInStore, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.cinema:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataCinema, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, _session.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.mms:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMms, _session.IsSelectRetailerDisplay).Sql;
                    break;
            }

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                /* For internet media type, we select the max date from DATA_INTERNET, for all the vehicles
                 * */
                /* For directMarketing media type, we select the max date from DATA_MARKETING_DIRECT for all the vehicles
            * */
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                case DBClassificationConstantes.Vehicles.names.mailValo:
                case DBClassificationConstantes.Vehicles.names.internet:
                    sql += string.Format(" select max({0}) last_date ", DBConstantes.Fields.DATE_MEDIA_NUM);
                    sql += string.Format(" from {0}", tableName);
                    break;
                               
                /* For the media types below, we select the max date for every vehicle and after that we select the min date from the list of max date
                 * */
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.indoor:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                case DBClassificationConstantes.Vehicles.names.mms:
                    /* Select min date from the list of max date
                     * */
                    sql += " select min(last_date) as last_date ";
                    sql += " from (";

                    #region Media selection
                    Dictionary<CustormerConstantes.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;

                    /* Get the list of vehicles selected
                     * */
                    if (selectedVehicles != null && selectedVehicles.ContainsKey(CustormerConstantes.Right.type.mediaAccess)) {
                        mediaList = selectedVehicles[CustormerConstantes.Right.type.mediaAccess];
                    }
                    #endregion

                    /* Get the max date for every vehicle
                     * */
                    if (mediaList.Length > 0) {

                        string[] strs = mediaList.Substring(0, mediaList.Length).Split(',');
                        int i = 0;

                        while (i < strs.Length) {
                            if (i > 0) {
                                sql += " UNION ";
                            }

                            sql += " select id_media, max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                            sql += " from " + tableName;
                            sql += " where id_media = " + strs[i] + "";
                            sql += " group by id_media ";

                            i += 1;
                        }
                    }

                    sql += " )";
                    break;
            }
            #endregion

            #region Execution of the query
            try {
                /* Execute the SQL request
                 * */
                DataSet ds = dataSource.Fill(sql);
                /* Return the last date
                 * */
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Error while trying to get the last publication date", err));
            }
            #endregion

        }
        #endregion

        #region GetLastAvailableDate
        /// <summary>
        /// Get the last available date for which we have data in the data base (for the media type selected)
        /// </summary>
        /// <param name="vehicleId">Media Type Identifier</param>
        /// <returns>DataSet containing the result of the SQL request</returns>
        public virtual DataSet GetLastAvailableDate(Int64 vehicleId) {

            StringBuilder sql = new StringBuilder(500);

            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            /* Get the table name of the media type selected : DATA_PRESS, DATA_TV ...
             * */
            string tableName = TNS.AdExpress.Web.Core.Utilities.SQLGenerator.GetDataTableName(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.PeriodBreakdownType.data, vehicleId,_session.IsRetailerDisplay);
            /* Get the max date available
             * */
            sql.Append("Select max( date_media_num ) as availableDate ");
            sql.Append(" from " + tableName);

            #region Execution of query
            try {
                /* Execution of the SQL request
                 * */
                return dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new Exception.LastAvailableDateException("Impossible to get the last available date : " + sql, err));
            }
            #endregion

        }
        #endregion

         #region Get Media type's last available date  which has data
        /// <summary>
        /// Get Media type's last available date  which has data 
        /// </summary>
        /// <remarks>Use particularly in  Product class analysis modules</remarks>
        /// <param name="idVehicle">Media type ID</param>
        /// <param name="webSession">Session</param>
        /// <returns>Last available month which has data : YYYYMM</returns>
        public virtual string CheckAvailableDateForMedia(long idVehicle)
        {

            int nbYears = WebApplicationParameters.DataNumberOfYear;
            string lastAvailableDate = (DateTime.Now.Year - (nbYears - 1)).ToString();

            string sql = "";


            DataSet ds;

            #region Construction de la requete
            sql = "select ";
            // Cas ou l'année en cours est différente de la dernière année chargée
            if (DateTime.Now.Year > _session.DownLoadDate)
            {
                for (int i = nbYears; i > 0; i--)
                {
                    for (int j = 1; j <= 12; j++)
                    {
                        sql += " max(exp_euro_" + ((i - 1 != 0) ? "N" + (i - 1) : "N") + "_" + j + ") as N"
                            + (DateTime.Now.Year - i) + j.ToString("0#") + ",";
                    }
                }
            }
            else
            {
                nbYears = nbYears - 1;
                for (int i = nbYears; i >= 0; i--)
                {
                    for (int j = 1; j <= 12; j++)
                    {
                        sql += " max(exp_euro_" + ((i != 0) ? "N" + i : "N") + "_" + j + ") as N"
                            + (DateTime.Now.Year - i) + j.ToString("0#") + ",";
                    }
                }
            }

            sql = sql.Remove(sql.Length - 1, 1);

            sql += " from " + TNS.AdExpress.Web.Core.Utilities.SQLGenerator.getVehicleTableNameForSectorAnalysisResult(VehiclesInformation.DatabaseIdToEnum(idVehicle), _session.IsSelectRetailerDisplay);
            #endregion

            #region Exécution de la requete
            IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
            ds = source.Fill(sql);

            #endregion

            #region Extraction du dernier mois disponible

            for (int i = ds.Tables[0].Columns.Count - 1; i >= 0; i--)
            {
                if (ds.Tables[0].Rows[0][i].ToString() != "0")
                {
                    lastAvailableDate = ds.Tables[0].Columns[i].ColumnName.Remove(0, 1);
                    break;
                }
            }
            #endregion

            return lastAvailableDate;

        }
        #endregion

        #region Get Data Source
        /// <summary>
        /// Get Data Source
        /// </summary>
        /// <returns>Data source</returns>
        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource() {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.
                WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _session;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = (TNS.AdExpress.Web.Core.SourceProvider)AppDomain.CurrentDomain.
                CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class,
                false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            return sourceProvider.GetSource();

        }
        #endregion

        #region GetCalendarStartDate
        /// <summary>
        /// Get the calendar starting date
        /// </summary>
        /// <returns>The year corresponding to the starting date : date format yyyy example 2008</returns>
        public virtual int GetCalendarStartDate()
        {
            int nbYears = WebApplicationParameters.DataNumberOfYear;
            if (_session.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
            {
                nbYears = nbYears - 2;
                return DateTime.Now.AddYears(-nbYears).Year;
            }
            else
            {
                nbYears = nbYears - 1;
                return DateTime.Now.AddYears(-nbYears).Year;
            }

        }
        #endregion

        #region Check validity of the period depending on data delivering frequency
        /// <summary>
        /// Check period depending on data delivering frequency
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="EndPeriod">End period</param>
        /// <returns>Period End</returns>
        public virtual string CheckPeriodValidity(WebSession _session, string EndPeriod)
        {
            Int64 frequency = _session.CustomerLogin.GetIdFrequency(_session.CurrentModule);
            switch (frequency)
            {
                case CstFrequency.ANNUAL:
                    //if the studied year is not entirely loaded (== current year or december not loaded)
                    if (int.Parse(EndPeriod.Substring(0, 4)) >= int.Parse(_session.LastAvailableRecapMonth.Substring(0, 4)))
                        throw new NoDataException();
                    break;
                case CstFrequency.MONTHLY:
                    return GetAbsoluteEndPeriod(_session, EndPeriod, 1);
                case CstFrequency.TWO_MONTHLY:
                    return GetAbsoluteEndPeriod(_session, EndPeriod, 2);
                case CstFrequency.QUATERLY:
                    return GetAbsoluteEndPeriod(_session, EndPeriod, 3);
                case CstFrequency.SEMI_ANNUAL:
                    return GetAbsoluteEndPeriod(_session, EndPeriod, 6);
                case CstFrequency.DAILY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.DAILY.ToString() + ")");
                case CstFrequency.SEMI_MONTHLY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.SEMI_MONTHLY.ToString() + ")");
                case CstFrequency.WEEKLY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.WEEKLY.ToString() + ")");
            }

            return EndPeriod;

        }
        /// <summary>
        /// Get the end of the period depending on data loading
        /// </summary>
        /// <param name="webSession">User session</param>
        /// <param name="EndPeriod">End of the period</param>
        /// <param name="divisor">diviseur</param>
        /// <returns>End of the period</returns>
        protected string GetAbsoluteEndPeriod(WebSession _session, string EndPeriod, int divisor)
        {

            string absoluteEndPeriod = "0";

            //If selected year is lower or equal to data loadin year, then get last loaded month of the last complete trimester
            if (_session.LastAvailableRecapMonth != null && _session.LastAvailableRecapMonth.Length >= 6
                && int.Parse(EndPeriod.Substring(0, 4)) <= int.Parse(_session.LastAvailableRecapMonth.Substring(0, 4))
                )
            {
                //if selected year is equal to last loaded year, get last complete trimester
                //Else get last selected month of the previous year
                if (int.Parse(EndPeriod.Substring(0, 4)) == int.Parse(_session.LastAvailableRecapMonth.Substring(0, 4)))
                {
                    absoluteEndPeriod = _session.LastAvailableRecapMonth.Substring(0, 4) + (int.Parse(_session.LastAvailableRecapMonth.Substring(4, 2)) - int.Parse(_session.LastAvailableRecapMonth.Substring(4, 2)) % divisor).ToString("00");
                    //if study month is greather than the last loaded month, get back to the last loaded month
                    if (int.Parse(EndPeriod) > int.Parse(absoluteEndPeriod))
                    {
                        EndPeriod = absoluteEndPeriod;
                    }
                }
            }
            else
            {
                EndPeriod = EndPeriod.Substring(0, 4) + "00";
            }

            return EndPeriod;
        }
        #endregion

        #region GetTendenciesLastAvailableDate
        /// <summary>
        /// Get Tendencies Last Available Date
        /// </summary>
        /// <returns>Last Available Date</returns>
        public virtual DateTime GetTendenciesLastAvailableDate() {

            DateTime date = new DateTime(1,1,1);
            StringBuilder sql = new StringBuilder();
            //Table tableName = WebApplicationParameters.GetDataTable(TableIds.tendencyWeek, false);
            Table tableName = WebApplicationParameters.GetDataTable(TableIds.tendencyMonth, false);
            IDataSource dataSource = GetDataSource();

            sql.Append(" SELECT max(date_period) as lastAvailableDate ");
            sql.AppendFormat(" FROM {0} ", tableName.Sql);

            #region Execution of the query
            try {
                /* Execute the SQL request
                 * */
                DataSet ds = dataSource.Fill(sql.ToString());
                /* Return the last date
                 * */
                if (ds != null && ds.Tables[0].Rows.Count > 0){
                    string dateStr = ds.Tables[0].Rows[0]["lastAvailableDate"].ToString();
                    AtomicPeriodWeek week = new AtomicPeriodWeek(int.Parse(dateStr.Substring(0, 4)), int.Parse(dateStr.Substring(4, 2)));
                    return (week.LastDay);
                }
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Error while trying to get the last publication date", err));
            }
            #endregion

            return date;
        }
        #endregion

        #endregion

    }
}
