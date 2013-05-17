using System;
using System.Collections.Generic;
using AdExpressConstantes = TNS.AdExpress.Constantes.Classification.DB;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common.Oracle;
using System.Diagnostics;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.DAL;

namespace KMI.AdExpress.Aphrodite {
    /// <summary>
    /// Compute Data for Trebds reports
    /// </summary>
    public class ComputeDataEngine {

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
        public void Load(EventLog eventLogAphrodite) {
            eventLogAphrodite.WriteEntry("Trends Data Loading Start");

            try {

                #region Database connection
                IDataSource source = new OracleDataSource(_dataBaseConfiguration.ConnectionString);
                #endregion


                foreach (MediaTypeInformation currentMediaType in _mediaTypesList.Values) {

                    List<string> monthList = Period.DownLoadListMonthPeriod(_currentDay);

                    #region Remove data
                    eventLogAphrodite.WriteEntry("Start removing data for " + currentMediaType.VehicleId.ToString());

                    try {
                        TrendsDAL.Remove(currentMediaType, source);
                    }
                    catch (System.Exception err) {
                        eventLogAphrodite.WriteEntry("Error while removing data for " + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                        throw (err);
                    }
                    eventLogAphrodite.WriteEntry("End removing data for " + currentMediaType.VehicleId.ToString());
                    #endregion

                    #region Load Data
                    if (currentMediaType.MonthTrendsTable.Length > 0) {
                        foreach (string currentMonth in monthList) {
                            eventLogAphrodite.WriteEntry("Start computing Month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());

                            try {
                                InsertMonth(currentMonth, currentMediaType, source);
                            }
                            catch (System.Exception err) {
                                eventLogAphrodite.WriteEntry("Error while computing Month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                throw (err);
                            }
                            eventLogAphrodite.WriteEntry("End computing Month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());
                        }
                    }
                    #endregion

                    #region Compute SubTotals
                    if (currentMediaType.MonthTrendsTable.Length > 0) {
                        foreach (string currentMonth in monthList) {
                            eventLogAphrodite.WriteEntry("Start computing Subtotal - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());

                            try {
                                TrendsDAL.InsertSubTotal(currentMonth, source, currentMediaType, false, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL);
                            }
                            catch (System.Exception err) {
                                eventLogAphrodite.WriteEntry("Error while computing Subtotal - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                throw (err);
                            }
                            eventLogAphrodite.WriteEntry("End computing Subtotal - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());
                        }
                    }
                    #endregion

                    #region Compute Totals
                    if (currentMediaType.MonthTrendsTable.Length > 0) {
                        foreach (string currentMonth in monthList) {
                            eventLogAphrodite.WriteEntry("Start computing total - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());

                            try {
                                TrendsDAL.InsertSubTotal(currentMonth, source, currentMediaType, true, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL);
                            }
                            catch (System.Exception err) {
                                eventLogAphrodite.WriteEntry("Error while computing total - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                                throw (err);
                            }
                            eventLogAphrodite.WriteEntry("End computing total - month: " + currentMonth + " for Media type :" + currentMediaType.VehicleId.ToString());
                        }
                    }
                    #endregion

                    #region Cumul à date
                    eventLogAphrodite.WriteEntry("Start computing Cumul for Media type :" + currentMediaType.VehicleId.ToString());
                    try {
                        InsertCumul(currentMediaType, _currentDay, source);
                        TrendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE, source, currentMediaType, false, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL);
                        TrendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE, source, currentMediaType, true, TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE, TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL);

                    }
                    catch (System.Exception err) {
                        eventLogAphrodite.WriteEntry("Error while computing Cumul for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                        throw (err);
                    }
                    eventLogAphrodite.WriteEntry("End computing Cumul for Media type :" + currentMediaType.VehicleId.ToString());
                    #endregion

                    #region Calcul des Pdm
                    eventLogAphrodite.WriteEntry("Start computing Pdm for Media type :" + currentMediaType.VehicleId.ToString());

                    try {
                        // Pdm 
                        TrendsDAL.InsertPDM(currentMediaType, source, false);
                        // Pdm total
                        TrendsDAL.InsertPDM(currentMediaType, source, true);
                    }
                    catch (System.Exception err) {
                        eventLogAphrodite.WriteEntry("Error while computing Pdm for Media type :" + currentMediaType.VehicleId.ToString() + " : " + err.Message);
                        throw (err);
                    }
                    eventLogAphrodite.WriteEntry("End computing Pdm for Media type :" + currentMediaType.VehicleId.ToString());
                    #endregion

                }
            }
            catch (System.Exception err) {
                eventLogAphrodite.WriteEntry("Error while treating tendency data for Media type : " + err.Message);
                return;
            }

            eventLogAphrodite.WriteEntry("Trends Data Loading End");
        }
        #endregion

        #region Private Methods

        #region Insert Month
        /// <summary>
        /// Compute periods and insert data in table month
        /// </summary>
        /// <param name="period">Month period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        private void InsertMonth(string period, MediaTypeInformation mediaTypeInformation, IDataSource source) {

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

            TrendsDAL.InsertMonth(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, mediaTypeInformation, cumul, source);
        }
        #endregion

        #region Insert Cumul
        /// <summary>
        /// Compute periods and insert data in table month
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        private void InsertCumul(MediaTypeInformation mediaTypeInformation, DateTime currentDay, IDataSource source) {

            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE;
            string periodId = TNS.AdExpress.Constantes.DB.Hathor.ID_PERIOD_CUMULATIVE;
            string year = TNS.AdExpress.Constantes.DB.Hathor.YEAR_PERIOD_CUMULATIVE;

            DateTime dT = currentDay.AddMonths(-1);
            string studyYear = dT.Year.ToString();

            DateTime LastDate = new DateTime(currentDay.Year, currentDay.Month, 1).AddDays(-1);
            periodEnding = LastDate.Year.ToString();
            if (LastDate.Month.ToString().Length < 2) periodEnding += "0";
            periodEnding += LastDate.Month.ToString() + LastDate.Day.ToString();

            DateTime LastDatePreviousPeriod = new DateTime(currentDay.AddYears(-1).Year, currentDay.Month, 1).AddDays(-1);
            periodEndingPrev = LastDatePreviousPeriod.Year.ToString();
            if (LastDatePreviousPeriod.Month.ToString().Length < 2) periodEndingPrev += "0";
            periodEndingPrev += LastDatePreviousPeriod.Month.ToString() + LastDatePreviousPeriod.Day.ToString();

            periodBeginning = studyYear + "0101";
            string previousStudyYear = dT.AddYears(-1).Year.ToString();
            periodBeginningPrev = previousStudyYear + "0101";

            string period = studyYear;
            if (dT.Month.ToString().Length < 2) period += "0";
            period += dT.Month.ToString();

            TrendsDAL.InsertMonth(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, mediaTypeInformation, cumul, source);
        }
        #endregion

        #endregion

    }
}
