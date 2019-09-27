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
using TNS.AdExpress.Web.Core.Utilities;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using MediaSelection = TNS.AdExpress.Web.Core.DataAccess.Selections.Medias;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.FrameWork;
using TNS.AdExpressI.Date.DAL;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using System.Globalization;
using System.Reflection;

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

        #region Variables
        /// <summary>
        /// Determines if dates shoukd be upadted
        /// </summary>
        protected bool _isUpdateDates = false;
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

            if (webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE &&
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

            if (webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE) {

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
            _isUpdateDates = true;

            if (webSessionSave.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE) {

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

            if (webSessionSave.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE) {

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
            if (!_isUpdateDates)              
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
            }
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

            if (!_isUpdateDates || (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly
                && (Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                    < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)))
                )
            {                
                    webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;                
            }
           
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


            if (!_isUpdateDates || (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly
                && (Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                    < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)))
                )
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.weekly;
            }
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
            }
           
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
            }
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
            }
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.weekly;
            }
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
            }
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
            if (!_isUpdateDates)
            {
                webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
            }
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

        #region GetFirstDayNotEnable
        /// <summary>
        /// Renvoie le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        /// <returns>Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées</returns>
		public static DateTime GetFirstDayNotEnabled(WebSession webSession, long selectedVehicle, int startYear, IDataSource dataSource) {
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;

            switch(VehiclesInformation.DatabaseIdToEnum(selectedVehicle)) {
                case Vehicles.names.press:
                case Vehicles.names.newspaper:
                case Vehicles.names.magazine:
                case Vehicles.names.internationalPress:
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.others:
                case Vehicles.names.outdoor:
                case Vehicles.names.dooh:
                case Vehicles.names.instore:
                case Vehicles.names.indoor:
                case Vehicles.names.cinema:
                case Vehicles.names.adnettrack:
                case Vehicles.names.evaliantMobile:
                case Vehicles.names.mms:
                case Vehicles.names.search:
                case Vehicles.names.social:
                case Vehicles.names.audioDigital:
                case Vehicles.names.paidSocial:
                    lastDate = MediaSelection.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle, dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                case Vehicles.names.directMarketing:
					lastDate = MediaSelection.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle, dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(7);
                    return firstDayOfWeek;
                case Vehicles.names.czinternet:
                case Vehicles.names.internet:
                case Vehicles.names.mailValo:
                    lastDate = MediaSelection.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle,dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

        /// <summary>
        ///  Get Period label in product class analysis depending on selected year
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="period">period</param>
        /// <returns>Label describing period in Product Class Analysis depending on selected year</returns>
        public  string GetPeriodLabel(WebSession _session, CstCustomerSession.Period.Type period)
        {

            string beginPeriod = "";
            string endPeriod = "";
            string year = "";

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            switch (period)
            {
                case CstWeb.CustomerSessions.Period.Type.currentYear:
                    beginPeriod = _session.PeriodBeginningDate;
                    endPeriod = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

                    break;
                case CstWeb.CustomerSessions.Period.Type.previousYear:
                    year = (int.Parse(_session.PeriodBeginningDate.Substring(0, 4)) - 1).ToString();
                    beginPeriod = year + _session.PeriodBeginningDate.Substring(4);
                    endPeriod = year + dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate).Substring(4);

                    break;
                default:
                    throw new ArgumentException(string.Format("Unable to treat this type of period ({0}) .", period.ToString()));
            }

            return Convertion.ToHtmlString(SwitchPeriod(_session, beginPeriod, endPeriod));
        }

        /// <summary>
        /// Display of period in product classs analysis
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="beginPeriod">Period beginning</param>
        /// <param name="endPeriod">End of period</param>
        /// <returns>Selected period</returns>
        public  string SwitchPeriod(WebSession _session, string beginPeriod, string endPeriod)
        {

            string periodText;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            switch (_session.PeriodType)
            {

                case CstCustomerSession.Period.Type.nLastMonth:
                case CstCustomerSession.Period.Type.dateToDateMonth:
                case CstCustomerSession.Period.Type.previousMonth:
                case CstCustomerSession.Period.Type.currentYear:
                case CstCustomerSession.Period.Type.previousYear:
                case CstCustomerSession.Period.Type.nextToLastYear:

                    if (beginPeriod != endPeriod)
                        periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + "-" + MonthString.GetCharacters(int.Parse(endPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
                    else
                        periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
                    break;
                default:
                    throw new System.Exception("switchPeriod(_session _session,string beginPeriod,string endPeriod)-->Unable to determine type of period.");

            }

            return periodText;
        }

    }
}
