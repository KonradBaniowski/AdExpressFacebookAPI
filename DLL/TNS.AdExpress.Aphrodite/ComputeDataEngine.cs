using System;
using System.Collections.Generic;
using AdExpressConstantes = TNS.AdExpress.Constantes.Classification.DB;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common.Oracle;
using System.Diagnostics;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.DAL;
using KMI.AdExpress.Aphrodite.Constantes;
using KMI.AdExpress.Aphrodite.Domain.Layers;
using TNS.FrameWork.Date;
using TNS.LinkSystem.LinkKernel;

namespace KMI.AdExpress.Aphrodite {
    /// <summary>
    /// Compute Data for Trebds reports
    /// </summary>
    public class ComputeDataEngine {

        #region Events
        // The delegate named EventChangeHandler, which will encapsulate
        // any method that takes a message and a log category as the parameters and returns no value. 
        // It's the delegate the subscribers must implement.
        public delegate void EventChangeHandler(
           string message,
           eLogCategories logCategory
        );

        // The event we publish
        public event EventChangeHandler EventChange;

        // The method which fires the Event
        protected void OnEventChange(string message, eLogCategories logCategory) {
            // Check if there are any Subscribers
            if (message != null) {
                // Call the Event
                EventChange(message, logCategory);
            }
        }
        // The delegate named ErrorEventChangeHandler, which will encapsulate
        // any method that takes a message, an exception and a log category as the parameters and returns no value. 
        // It's the delegate the subscribers must implement.
        public delegate void ErrorEventChangeHandler(
           string message,
           Exception ex,
           eLogCategories logCategory
        );

        // The event we publish
        public event ErrorEventChangeHandler ErrorEventChange;

        // The method which fires the Event
        protected void OnErrorEventChange(string message, Exception ex, eLogCategories logCategory) {
            // Check if there are any Subscribers
            if (message != null) {
                // Call the Event
                ErrorEventChange(message, ex, logCategory);
            }
        }
        #endregion

        #region Variales
        /// <summary>
        /// Media types description
        /// </summary>
        Dictionary<AdExpressConstantes.Vehicles.names, MediaTypeInformation> _mediaTypesList;
        /// <summary>
        /// Database configuration
        /// </summary>
        private DataBaseConfiguration _dataBaseConfiguration;
        /// <summary>
        /// Override Date.Now
        /// </summary>
        private DateTime _currentDay = DateTime.Now;
        /// <summary>
        /// Event Log Aphrodite
        /// </summary>
        private EventLog _eventLogAphrodite;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mediaTypesList">Media types description</param>
        /// <param name="currentDay">Day of treatment</param>
        /// <param name="dataBaseConfiguration">Database configuration</param>
        public ComputeDataEngine(Dictionary<AdExpressConstantes.Vehicles.names, MediaTypeInformation> mediaTypesList, DateTime currentDay, DataBaseConfiguration dataBaseConfiguration) {
            _mediaTypesList=mediaTypesList;
            _dataBaseConfiguration=dataBaseConfiguration;
            _currentDay=currentDay;
        }
        #endregion

        #region Compute Data
        /// <summary>
        /// Compute data for trends report
        /// </summary>
        public void Load(EventLog eventLogAphrodite, DataAccessLayer dal) {

            _eventLogAphrodite = eventLogAphrodite;

            OnEventChange("Trends Data Loading Start", eLogCategories.Information);
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Trends Data Loading Start");

            try {

                #region Database connection
                IDataSource source = new OracleDataSource(_dataBaseConfiguration.ConnectionString);
                #endregion

                TrendsDAL trendsDAL = (TrendsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"bin\" + dal.AssemblyName, dal.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, null, null, null, null);

                foreach (MediaTypeInformation currentMediaType in _mediaTypesList.Values) {

                    foreach (Application.TreatmentType treatmentType in currentMediaType.ListTreatmentType) {

                        List<string> periodList = Period.DownloadListPeriod(_currentDay, treatmentType);

                        #region Remove data
                        OnEventChange("Start removing data for " + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start removing data for " + currentMediaType.VehicleId.ToString());

                        try {
                            trendsDAL.Remove(currentMediaType, treatmentType, source);
                        }
                        catch (System.Exception err) {
                            OnErrorEventChange("Error while removing data for " + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while removing data for " + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                            throw (err);
                        }
                        OnEventChange("End removing data for " + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End removing data for " + currentMediaType.VehicleId.ToString());
                        #endregion

                        #region Insert Data
                        if (((treatmentType == Application.TreatmentType.month) && (currentMediaType.MonthTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.comparativeDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.dateToDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))) {
                            foreach (string currentPeriod in periodList) {
                                OnEventChange("Start computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());

                                try {
                                    InsertPeriod(currentPeriod, currentMediaType, treatmentType, trendsDAL, source);
                                }
                                catch (System.Exception err) {
                                    OnErrorEventChange("Error while computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                                    if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                    throw (err);
                                }
                                OnEventChange("End computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End computing Period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());
                            }
                        }
                        #endregion

                        #region Compute SubTotals
                        if (((treatmentType == Application.TreatmentType.month) && (currentMediaType.MonthTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.comparativeDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.dateToDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))) {
                            foreach (string currentPeriod in periodList) {
                                OnEventChange("Start computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());

                                try {
                                    trendsDAL.InsertSubTotal(currentPeriod, source, currentMediaType, false, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL, treatmentType);
                                }
                                catch (System.Exception err) {
                                    OnErrorEventChange("Error while computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                                    if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                    throw (err);
                                }
                                OnEventChange("End computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End computing Subtotal - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());
                            }
                        }
                        #endregion

                        #region Compute Totals
                        if (((treatmentType == Application.TreatmentType.month) && (currentMediaType.MonthTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.comparativeDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))
                                || ((treatmentType == Application.TreatmentType.dateToDateWeek) && (currentMediaType.WeekTrendsTable.Length > 0))) {
                            foreach (string currentPeriod in periodList) {
                                OnEventChange("Start computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());

                                try {
                                    trendsDAL.InsertSubTotal(currentPeriod, source, currentMediaType, true, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL, treatmentType);
                                }
                                catch (System.Exception err) {
                                    OnErrorEventChange("Error while computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                                    if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                    throw (err);
                                }
                                OnEventChange("End computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End computing total - period: " + currentPeriod + " for Media type :" + currentMediaType.VehicleId.ToString());
                            }
                        }
                        #endregion

                        #region Cumul à date
                        OnEventChange("Start computing Cumul for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start computing Cumul for Media type :" + currentMediaType.VehicleId.ToString());
                        try {
                            trendsDAL.InsertCumul(currentMediaType, _currentDay, treatmentType, source);
                            trendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE, source, currentMediaType, false, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL, treatmentType);
                            trendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE, source, currentMediaType, true, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL, treatmentType);
                        }
                        catch (System.Exception err) {
                            OnErrorEventChange("Error while computing Cumul for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while computing Cumul for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                            throw (err);
                        }
                        OnEventChange("End computing Cumul for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End computing Cumul for Media type :" + currentMediaType.VehicleId.ToString());
                        #endregion

                        #region Calcul des Pdm
                        OnEventChange("Start computing Pdm for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start computing Pdm for Media type :" + currentMediaType.VehicleId.ToString());

                        try {
                            // Pdm 
                            trendsDAL.InsertPDM(currentMediaType, source, false, treatmentType);
                            // Pdm total
                            trendsDAL.InsertPDM(currentMediaType, source, true, treatmentType);
                        }
                        catch (System.Exception err) {
                            OnErrorEventChange("Error while computing Pdm for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message, err, eLogCategories.Problem);
                            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while computing Pdm for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                            throw (err);
                        }
                        OnEventChange("End computing Pdm for Media type :" + currentMediaType.VehicleId.ToString(), eLogCategories.Information);
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("End computing Pdm for Media type :" + currentMediaType.VehicleId.ToString());
                        #endregion

                    }
                }
            }
            catch (System.Exception err) {
                OnErrorEventChange("Error while treating tendency data for Media type : " + err.Message, err, eLogCategories.Problem);
                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while treating tendency data for Media type : " + err.Message);
                throw err;
            }

            OnEventChange("Trends Data Loading End", eLogCategories.Information);
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Trends Data Loading End");
        }
        #endregion

        #region Private Methods

        #region Insert Period
        /// <summary>
        /// Compute periods and insert data in table month or Week
        /// </summary>
        /// <param name="period">Period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="treatmentType">Treatment type : Month or Week</param>
        /// <param name="trendsDAL">Trends DAL to use</param>
        /// <param name="source">Data source</param>
        private void InsertPeriod(string period, MediaTypeInformation mediaTypeInformation, Application.TreatmentType treatmentType, TrendsDAL trendsDAL, IDataSource source) {

            switch (treatmentType) { 
                case Application.TreatmentType.month:
                    InsertMonth(period, mediaTypeInformation, trendsDAL, source);
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                    InsertComparativeDateWeek(period, mediaTypeInformation, trendsDAL, source);
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    InsertDateToDateWeek(period, mediaTypeInformation, trendsDAL, source);
                    break;
                default:
                    throw new Exception("Treatment type not defined");
            }

        }
        #endregion

        #region Insert Month
        /// <summary>
        /// Compute periods and insert data in table month
        /// </summary>
        /// <param name="period">Month period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="trendsDAL">Trends DAL</param>
        /// <param name="source">Data source</param>
        private void InsertMonth(string period, MediaTypeInformation mediaTypeInformation, TrendsDAL trendsDAL, IDataSource source) {

            #region Periods
            string year = "";
            // Format MM
            string periodId = "";
            // Format YYYYMMDD
            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE;

            //Recherche des différentes date pour les lignes de la table tendency_month;
            year = period.Substring(0, 4);
            //Mois
            periodId = period.Substring(4, 2);
            periodBeginning = year + periodId + "01";
            DateTime dT = new DateTime(int.Parse(year), int.Parse(periodId), 1);
            DateTime dtPrev = new DateTime(int.Parse(year) - 1, int.Parse(periodId), 1);

            dT = dT.AddMonths(1);
            dT = dT.AddDays(-1);
            periodEnding = year + periodId + dT.Day.ToString();

            periodBeginningPrev = dtPrev.Year.ToString() + periodId + "01";
            dtPrev = dtPrev.AddMonths(1);
            dtPrev = dtPrev.AddDays(-1);

            periodEndingPrev = dtPrev.Year.ToString() + periodId + dtPrev.Day.ToString();
            #endregion

            OnEventChange("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: " + periodEndingPrev, eLogCategories.Information);
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: ");
            trendsDAL.InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT, mediaTypeInformation, cumul, Application.TreatmentType.month, source);
        }
        #endregion

        #region Insertion current comparative date week
        /// <summary>
        /// Compute periods and insert data in table week
        /// </summary>
        /// <param name="period">Week period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="trendsDAL">Trends DAL</param>
        /// <param name="source">Data source</param>
        private void InsertComparativeDateWeek(string period, MediaTypeInformation mediaTypeInformation, TrendsDAL trendsDAL, IDataSource source) {

            #region Variables
            string year = "";
            // Format MM
            string periodId = "";
            // Format YYYYMMDD
            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            #endregion

            year = period.Substring(0, 4);
            periodId = period.Substring(4, 2);

            TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent = new AtomicPeriodWeek(int.Parse(year), int.Parse(periodId));
            TNS.FrameWork.Date.AtomicPeriodWeek weekPrev = new AtomicPeriodWeek(int.Parse(year) - 1, int.Parse(periodId));

            periodBeginning = weekCurrent.FirstDay.ToString("yyyyMMdd");
            periodEnding = weekCurrent.LastDay.ToString("yyyyMMdd");
            periodBeginningPrev = weekPrev.FirstDay.ToString("yyyyMMdd");
            periodEndingPrev = weekPrev.LastDay.ToString("yyyyMMdd");
            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE;

            OnEventChange("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: " + periodEndingPrev, eLogCategories.Information);
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: ");
            trendsDAL.InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT, mediaTypeInformation, cumul, Application.TreatmentType.comparativeDateWeek, source);
        }
        #endregion

        #region Insertion current date to date week
        /// <summary>
        /// Compute periods and insert data in table week
        /// </summary>
        /// <param name="period">Week period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="trendsDAL">Trends DAL</param>
        /// <param name="source">Data source</param>
        private void InsertDateToDateWeek(string period, MediaTypeInformation mediaTypeInformation, TrendsDAL trendsDAL, IDataSource source) {

            #region Variables
            string year = "";
            // Format MM
            string periodId = "";
            // Format YYYYMMDD
            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            DateTime dateBeginningPrev;
            DateTime dateEndingPrev;
            #endregion

            year = period.Substring(0, 4);
            periodId = period.Substring(4, 2);

            TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent = new AtomicPeriodWeek(int.Parse(year), int.Parse(periodId));

            periodBeginning = weekCurrent.FirstDay.ToString("yyyyMMdd");
            periodEnding = weekCurrent.LastDay.ToString("yyyyMMdd");

            if (weekCurrent.FirstDay.Month == 2 && weekCurrent.FirstDay.Day == 29)
                dateBeginningPrev = new DateTime(weekCurrent.FirstDay.Year - 1, 3, 1);
            else
                dateBeginningPrev = new DateTime(weekCurrent.FirstDay.Year - 1, weekCurrent.FirstDay.Month, weekCurrent.FirstDay.Day);

            periodBeginningPrev = dateBeginningPrev.ToString("yyyyMMdd");

            dateEndingPrev = SetPreviousDateToDateWeek(dateBeginningPrev);
            periodEndingPrev = dateEndingPrev.ToString("yyyyMMdd");
            
            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE;

            OnEventChange("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: " + periodEndingPrev, eLogCategories.Information);
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Period Beginning: " + periodBeginning + " -- Period Ending:" + periodEnding + " vs Period Beginning Prev: " + periodBeginningPrev + " -- Period Ending Prev: ");
            trendsDAL.InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE, mediaTypeInformation, cumul, Application.TreatmentType.comparativeDateWeek, source);
        }
        #endregion

        #region Set previous date to date week
        /// <summary>
        /// Set previous date to date week
        /// </summary>
        /// <returns>Last day</returns>
        private DateTime SetPreviousDateToDateWeek(DateTime firstDay) {

            DateTime lastDate = firstDay;

            for (int i = 1; i <= 6; i++)
                lastDate = lastDate.AddDays(1);

            return lastDate;
        }
        #endregion

        #endregion

    }
}
