#region Information
/*
 * Author : Y R'kaina
 * Created on : 01/09/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpressI.Date.Exception;

namespace TNS.AdExpressI.Date {

    public class Date : IDate {

        #region enum
        /// <summary>
        /// Period Type : n last year, current year ...
        /// </summary>
        private enum periodType {
            nLastYears,
            nLastMonths,
            nLastWeeks,
            nLastDays,
            previousYear,
            previousMonth,
            previousWeek,
            previousDay,
            currentYear
        }
        #endregion

        #region IDate Membres

        #region SetDate
        /// <summary>
        /// This method sets the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
        /// to sets this attributes, it use the informations provided by globalCalendarWebControl
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="FirstDayNotEnable">First day for which we don't have data</param>
        /// <param name="periodCalendarDisponibilityType">Disponibility type : currentDay or lastCompletePeriod</param>
        /// <param name="comparativePeriodCalendarType">Comparative period type : comparativeWeekDate, dateToDate or manual</param>
        /// <param name="periodSelected">Period type selected</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        public void SetDate(ref WebSession webSession, DateTime FirstDayNotEnable, globalCalendar.periodDisponibilityType periodCalendarDisponibilityType, globalCalendar.comparativePeriodType comparativePeriodCalendarType, int selectedPeriod, int selectedValue)
        {
            DateTime compareDate;
            DateTime lastDayEnable = DateTime.Now;
            DateTime tempDate = DateTime.Now;
            bool isLastCompletePeriod = false;

            if (webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE &&
                periodCalendarDisponibilityType == globalCalendar.periodDisponibilityType.lastCompletePeriod) {

                lastDayEnable = FirstDayNotEnable.AddDays(-1);
                isLastCompletePeriod = true;
            }
            
            switch (selectedPeriod) {

                case (int)periodType.nLastYears:
                    SetNLastYears(ref webSession, selectedValue);
                    break;
                case (int)periodType.nLastMonths:
                    SetNLastMonths(ref webSession, selectedValue, isLastCompletePeriod, lastDayEnable);
                    break;
                case (int)periodType.nLastWeeks:
                    SetNLastWeeks(ref webSession, selectedValue, isLastCompletePeriod, lastDayEnable);
                    break;
                case (int)periodType.nLastDays:
                    SetNLastDays(ref webSession, selectedValue, lastDayEnable);
                    break;
                case (int)periodType.previousYear:
                    SetPreviousYear(ref webSession);
                    break;
                case (int)periodType.previousMonth:
                    SetPreviousMonth(ref webSession);
                    break;
                case (int)periodType.previousWeek:
                    SetPreviousWeek(ref webSession);
                    break;
                case (int)periodType.previousDay:
                    SetPreviousDay(ref webSession);
                    break;
                case (int)periodType.currentYear:
                    SetCurrentYear(ref webSession);
                    break;
                default:
                    throw (new NotImplementedException());
            }

            compareDate = new DateTime(Convert.ToInt32(webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(webSession.PeriodEndDate.Substring(6, 2)));

            if (webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) {

                if (CompareDateEnd(lastDayEnable, compareDate))
                    webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                else {
                    switch (periodCalendarDisponibilityType) {
                        case globalCalendar.periodDisponibilityType.currentDay:
                            webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                            break;
                        case globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                            break;
                    }
                }
            }
            else {
                if (CompareDateEnd(DateTime.Now, compareDate))
                    webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
                else
                    webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
            }
        }
        #endregion

        #region UpdateDate
        /// <summary>
        /// This method update the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
        /// to update this attributes, it use the informations provided by the saved websession
        /// </summary>
        /// <param name="type">Type of period (dateToDate, nLastDays ...)</param>
        /// <param name="webSession">The customer session</param>
        /// <param name="webSessionSave">The customer session from DB</param>
        /// <param name="FirstDayNotEnable">First day for which we don't have data</param>
        public void UpdateDate(CustomerSessions.Period.Type type, ref WebSession webSession, TNS.AdExpress.Web.Core.Sessions.WebSession webSessionSave, DateTime FirstDayNotEnable) {
            
            DateTime compareDate;
            DateTime lastDayEnable = DateTime.Now;
            DateTime tempDate = DateTime.Now;
            bool isLastCompletePeriod = false;
            globalCalendar.comparativePeriodType comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
            globalCalendar.periodDisponibilityType periodDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;

            if (webSessionSave.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) {

                try {

                    if (webSessionSave.CustomerPeriodSelected != null) {

                        comparativePeriodType = webSessionSave.CustomerPeriodSelected.ComparativePeriodType;
                        periodDisponibilityType = webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType;
                    }
                    else {

                        switch (type) {

                            case CustomerSessions.Period.Type.currentYear:
                            case CustomerSessions.Period.Type.previousYear:
                            case CustomerSessions.Period.Type.previousMonth:
                            case CustomerSessions.Period.Type.previousDay:
                            case CustomerSessions.Period.Type.nLastDays:
                                comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
                                periodDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                                break;
                            case CustomerSessions.Period.Type.previousWeek:
                                comparativePeriodType = globalCalendar.comparativePeriodType.comparativeWeekDate;
                                periodDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                                break;
                            case CustomerSessions.Period.Type.nLastMonth:
                                comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
                                periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                break;
                            case CustomerSessions.Period.Type.nLastWeek:
                                comparativePeriodType = globalCalendar.comparativePeriodType.comparativeWeekDate;
                                periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                break;
                        }
                    }
                }
                catch (System.Exception) {

                    switch (type) {

                        case CustomerSessions.Period.Type.currentYear:
                        case CustomerSessions.Period.Type.previousYear:
                        case CustomerSessions.Period.Type.previousMonth:
                        case CustomerSessions.Period.Type.previousDay:
                        case CustomerSessions.Period.Type.nLastDays:
                            comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
                            periodDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                            break;
                        case CustomerSessions.Period.Type.previousWeek:
                            comparativePeriodType = globalCalendar.comparativePeriodType.comparativeWeekDate;
                            periodDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                            break;
                        case CustomerSessions.Period.Type.nLastMonth:
                            comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
                            periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                            break;
                        case CustomerSessions.Period.Type.nLastWeek:
                            comparativePeriodType = globalCalendar.comparativePeriodType.comparativeWeekDate;
                            periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                            break;
                    }
                }

                switch (periodDisponibilityType) {

                    case globalCalendar.periodDisponibilityType.currentDay:
                        lastDayEnable = DateTime.Now;
                        break;
                    case globalCalendar.periodDisponibilityType.lastCompletePeriod:
                        lastDayEnable = FirstDayNotEnable.AddDays(-1);
                        isLastCompletePeriod = true;
                        break;
                }
            }

            switch (type) {

                case CustomerSessions.Period.Type.nLastYear:
                    SetNLastYears(ref webSession, webSessionSave.PeriodLength);
                    break;
                case CustomerSessions.Period.Type.previousYear:
                    SetPreviousYear(ref webSession);
                    break;
                case CustomerSessions.Period.Type.nLastMonth:
                    SetNLastMonths(ref webSession, webSessionSave.PeriodLength, isLastCompletePeriod, lastDayEnable);
                    break;
                case CustomerSessions.Period.Type.previousMonth:
                    SetPreviousMonth(ref webSession);
                    break;
                case CustomerSessions.Period.Type.nLastWeek:
                    SetNLastWeeks(ref webSession, webSessionSave.PeriodLength, isLastCompletePeriod, lastDayEnable);
                    break;
                case CustomerSessions.Period.Type.previousWeek:
                    SetPreviousWeek(ref webSession);
                    break;
                case CustomerSessions.Period.Type.nLastDays:
                    SetNLastDays(ref webSession, webSessionSave.PeriodLength, lastDayEnable);
                    break;
                case CustomerSessions.Period.Type.previousDay:
                    SetPreviousDay(ref webSession);
                    break;
                case CustomerSessions.Period.Type.currentYear:
                    SetCurrentYear(ref webSession);
                    break;
            }

            compareDate = new DateTime(Convert.ToInt32(webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(webSession.PeriodEndDate.Substring(6, 2)));

            if (webSessionSave.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) {

                if (CompareDateEnd(lastDayEnable, compareDate))
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate, true, comparativePeriodType, periodDisponibilityType);
                else
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
            }
            else {
                if (CompareDateEnd(DateTime.Now, compareDate))
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
                else
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
            }
        }
        #endregion

        #region GetLastAvailableDate
        /// <summary>
        /// Get the last available date for which we have data in the data base (for a list of vehicle)
        /// </summary>
        /// <returns>dictionary with the list of pairs vehicleId:lastAvailableDate</returns>
        public virtual Dictionary<Vehicles.names, DateTime> GetLastAvailableDate() {

            string[] vehicleIdsList = VehiclesInformation.GetDatabaseIds().Split(',');
            VehicleInformation vehicle;
            string lastAvailableDate = string.Empty;
            DataSet ds;
            Dictionary<Vehicles.names, DateTime> lastAvailableDateList = new Dictionary<Vehicles.names, DateTime>();
            DateTime dateTime;

            try {
                foreach (string currentVehicle in vehicleIdsList) {

                    vehicle = VehiclesInformation.Get(Int64.Parse(currentVehicle));

                    if (vehicle.NeedLastAvailableDate) {

                        ds = DataAccess.LastAvailableDateDataAccess.GetLastAvailableDate(VehiclesInformation.EnumToDatabaseId(vehicle.Id));

                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
                            lastAvailableDate = ds.Tables[0].Rows[0]["availableDate"].ToString();
                            dateTime = new DateTime(int.Parse(lastAvailableDate.Substring(0, 4)), int.Parse(lastAvailableDate.Substring(4, 2)), 1);
                            dateTime = dateTime.AddMonths(1);
                            lastAvailableDateList.Add(vehicle.Id, dateTime.AddDays(-1));
                        }
                    }
                }
            }
            catch (System.Exception e) {
                throw (new LastAvailableDateException("Impossible to create a dictionary with the list of pairs vehicleId:lastAvailableDate for the GetLastAvailableDate() method ", e));
            }

            if (lastAvailableDateList.Count > 0)
                return lastAvailableDateList;
            else
                return null;
        }
        #endregion

        #region SetNLastYears
        /// <summary>
        /// Set date attributes for the n last years
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        public virtual void SetNLastYears(ref WebSession webSession, int selectedValue) {
            
            webSession.PeriodType = CustomerSessions.Period.Type.nLastYear;
            webSession.PeriodLength = selectedValue;
            webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSession.PeriodLength).ToString("yyyy0101");
            webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #region SetNLastMonths
        /// <summary>
        /// Set date attributes for the n last months
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="isLastCompletePeriod">True if it's verify the last complete period</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        public virtual void SetNLastMonths(ref WebSession webSession, int selectedValue, bool isLastCompletePeriod, DateTime lastDayEnable) {
            
            DateTime firstDayOfMonth;
            DateTime lastDayOfMonth;
            Int32 lastDayOfMonthInt;
            DateTime previousMonth;

            webSession.PeriodType = CustomerSessions.Period.Type.nLastMonth;
            webSession.PeriodLength = selectedValue;
            if (isLastCompletePeriod) {

                firstDayOfMonth = new DateTime(lastDayEnable.Year, lastDayEnable.Month, 1);
                lastDayOfMonth = (firstDayOfMonth.AddMonths(1)).AddDays(-1);

                if (lastDayEnable == lastDayOfMonth) {

                    webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - webSession.PeriodLength).ToString("yyyyMM01"); ;
                    webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                }
                else {

                    webSession.PeriodBeginningDate = lastDayEnable.AddMonths(0 - webSession.PeriodLength).ToString("yyyyMM01"); ;
                    previousMonth = lastDayEnable.AddMonths(-1);
                    firstDayOfMonth = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                    lastDayOfMonthInt = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                    webSession.PeriodEndDate = firstDayOfMonth.ToString("yyyyMM") + lastDayOfMonthInt;
                }
            }
            else {

                webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - webSession.PeriodLength).ToString("yyyyMM01"); ;
                webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
            }
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #region SetNLastWeeks
        /// <summary>
        /// Set date attributes for the n last weeks
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="isLastCompletePeriod">True if it's verify the last complete period</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        public virtual void SetNLastWeeks(ref WebSession webSession, int selectedValue, bool isLastCompletePeriod, DateTime lastDayEnable) {

            AtomicPeriodWeek startWeek;
            AtomicPeriodWeek endWeek;
            DateTime dateBegin;
            DateTime dateEnd;
            DateTime lastDayOfWeek;

            webSession.PeriodType = CustomerSessions.Period.Type.nLastWeek;
            webSession.PeriodLength = selectedValue;
            startWeek = new AtomicPeriodWeek(lastDayEnable);
            endWeek = new AtomicPeriodWeek(lastDayEnable);

            if (isLastCompletePeriod) {

                lastDayOfWeek = endWeek.FirstDay.AddDays(6);

                if (lastDayOfWeek == lastDayEnable) {

                    dateEnd = lastDayEnable;
                }
                else {

                    startWeek.SubWeek(1);
                    endWeek.SubWeek(1);
                    lastDayOfWeek = endWeek.FirstDay.AddDays(6);
                    dateEnd = lastDayOfWeek;
                }
            }
            else {

                dateEnd = lastDayEnable;
            }

            webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
            startWeek.SubWeek(webSession.PeriodLength - 1);
            dateBegin = startWeek.FirstDay;
            webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");

            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.weekly;
        }
        #endregion

        #region SetNLastDays
        /// <summary>
        /// Set date attributes for the n last days
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        public virtual void SetNLastDays(ref WebSession webSession, int selectedValue, DateTime lastDayEnable) {
            
            DateTime tempDate = DateTime.Now;

            webSession.PeriodType = CustomerSessions.Period.Type.nLastDays;
            webSession.PeriodLength = selectedValue;
            tempDate = lastDayEnable;
            webSession.PeriodBeginningDate = tempDate.AddDays(1 - webSession.PeriodLength).ToString("yyyyMMdd"); ;
            webSession.PeriodEndDate = tempDate.ToString("yyyyMMdd");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.dayly;
        }
        #endregion

        #region SetPreviousYear
        /// <summary>
        /// Set date attributes for the previous year
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public virtual void SetPreviousYear(ref WebSession webSession) {

            webSession.PeriodType = CustomerSessions.Period.Type.previousYear;
            webSession.PeriodLength = 1;
            webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy0101");
            webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy1231");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #region SetPreviousMonth
        /// <summary>
        /// Set date attributes for the previous month
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public virtual void SetPreviousMonth(ref WebSession webSession) {

            DateTime firstDayOfMonth;
            Int32 lastDayOfMonthInt;

            webSession.PeriodType = CustomerSessions.Period.Type.previousMonth;
            webSession.PeriodLength = 1;
            firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lastDayOfMonthInt = (firstDayOfMonth.AddDays(-1)).Day;
            webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM01");
            webSession.PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM") + lastDayOfMonthInt;
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #region SetPreviousWeek
        /// <summary>
        /// Set date attributes for the previous week
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public virtual void SetPreviousWeek(ref WebSession webSession) {
            
            AtomicPeriodWeek tmp;
            DateTime dateBegin;
            DateTime dateEnd;

            webSession.PeriodType = CustomerSessions.Period.Type.previousWeek;
            webSession.PeriodLength = 1;
            tmp = new AtomicPeriodWeek(DateTime.Now);
            tmp.SubWeek(1);
            dateBegin = tmp.FirstDay;
            webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
            dateEnd = tmp.FirstDay.AddDays(6);
            webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.weekly;
        }
        #endregion

        #region SetPreviousDay
        /// <summary>
        /// Set date attributes for the previous day
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public virtual void SetPreviousDay(ref WebSession webSession) {

            webSession.PeriodType = CustomerSessions.Period.Type.previousDay;
            webSession.PeriodLength = 2;
            webSession.PeriodBeginningDate = webSession.PeriodEndDate = DateTime.Now.AddDays(1 - webSession.PeriodLength).ToString("yyyyMMdd");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.dayly;
        }
        #endregion

        #region SetCurrentYear
        /// <summary>
        /// Set date attributes for the current year
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public virtual void SetCurrentYear(ref WebSession webSession) {

            webSession.PeriodType = CustomerSessions.Period.Type.currentYear;
            webSession.PeriodLength = 1;
            webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSession.PeriodLength).ToString("yyyy0101");
            webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #endregion

        #region comparaison entre la date de fin et la date d'aujourd'hui
        /// <summary>
        /// Verifie si la date de fin est inférieur ou non à la date de début
        /// </summary>
        /// <returns>vrai si la date de fin et inférieur à la date de début</returns>
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd)
        {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }
        #endregion
    }
}
