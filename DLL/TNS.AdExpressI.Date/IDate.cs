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
using TNS.AdExpress.Constantes.Classification.DB;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;

namespace TNS.AdExpressI.Date
{
    /// <summary>
    /// This interface provides all the methods to Set and Upadte the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...) 
    /// from the selection in the GlobalCalendar aspx page or from the session already saved
    /// </summary>
    public interface IDate {
        /// <summary>
        /// This method sets the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
        /// to sets this attributes, it use the informations provided by globalCalendarWebControl
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="FirstDayNotEnable">First day for which we don't have data</param>
        /// <param name="periodCalendarDisponibilityType">Disponibility type : currentDay or lastCompletePeriod</param>
        /// <param name="comparativePeriodCalendarType">Comparative period type : comparativeWeekDate, dateToDate or manual</param>
        /// <param name="selectedPeriod">Period type selected</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        void SetDate(ref WebSession webSession, DateTime FirstDayNotEnable, globalCalendar.periodDisponibilityType periodCalendarDisponibilityType, globalCalendar.comparativePeriodType comparativePeriodCalendarType, int selectedPeriod, int selectedValue);
        /// <summary>
        /// This method update the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
        /// to update this attributes, it use the informations provided by the saved websession
        /// </summary>
        /// <param name="type">Type of period (dateToDate, nLastDays ...)</param>
        /// <param name="webSession">The customer session</param>
        /// <param name="webSessionSave">The customer session from DB</param>
        /// <param name="FirstDayNotEnable">First day for which we don't have data</param>
        void UpdateDate(CustomerSessions.Period.Type type, ref WebSession webSession, WebSession webSessionSave, DateTime FirstDayNotEnable);
        /// <summary>
        /// Get the last available date for which we have data in the data base
        /// </summary>
        /// <returns>dictionary with the list of pairs vehicleId:lastAvailableDate</returns>
        Dictionary<Vehicles.names, DateTime> GetLastAvailableDate(); 
        /// <summary>
        /// Set date attributes for the n last years
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        void SetNLastYears(ref WebSession webSession, int selectedValue);
        /// <summary>
        /// Set date attributes for the n last months
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="isLastCompletePeriod">True if it's verify the last complete period</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        void SetNLastMonths(ref WebSession webSession, int selectedValue, bool isLastCompletePeriod, DateTime lastDayEnable);
        /// <summary>
        /// Set date attributes for the n last weeks
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="isLastCompletePeriod">True if it's verify the last complete period</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        void SetNLastWeeks(ref WebSession webSession, int selectedValue, bool isLastCompletePeriod, DateTime lastDayEnable);
        /// <summary>
        /// Set date attributes for the n last days
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedValue">Selected value from the dropdown list</param>
        /// <param name="lastDayEnable">Last day for which we have data</param>
        void SetNLastDays(ref WebSession webSession, int selectedValue, DateTime lastDayEnable);
        /// <summary>
        /// Set date attributes for the previous year
        /// </summary>
        /// <param name="webSession">The customer session</param>
        void SetPreviousYear(ref WebSession webSession);
        /// <summary>
        /// Set date attributes for the previous month
        /// </summary>
        /// <param name="webSession">The customer session</param>
        void SetPreviousMonth(ref WebSession webSession);
        /// <summary>
        /// Set date attributes for the previous week
        /// </summary>
        /// <param name="webSession">The customer session</param>
        void SetPreviousWeek(ref WebSession webSession);
        /// <summary>
        /// Set date attributes for the previous day
        /// </summary>
        /// <param name="webSession">The customer session</param>
        void SetPreviousDay(ref WebSession webSession);
        /// <summary>
        /// Set date attributes for the current year
        /// </summary>
        /// <param name="webSession">The customer session</param>
        void SetCurrentYear(ref WebSession webSession);

        string GetPeriodLabel(WebSession _session, CstCustomerSession.Period.Type period);
    }
}
