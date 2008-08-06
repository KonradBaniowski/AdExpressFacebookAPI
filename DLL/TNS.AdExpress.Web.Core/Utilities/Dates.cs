#region Information
/*
 * auteur : 
 * créé le :
 * modification : 
 *      22/07/2004 - - Guillaume Ragneau
 *      22/07/2008 - Déplacement depuis TNS.AdExpress.Web.Functions
 *      
 * */
#endregion

using System;
using System.Collections;
using System.Text;
using System.Globalization;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstFrequency = TNS.AdExpress.Constantes.Customer.DB.Frequency;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;

using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Date;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Utilities
{

    /// <summary>
    /// Set of function usefull to treat periods
    /// </summary>
    public class Dates
    {

        #region Check validity of the period depending on data delivering frequency
        /// <summary>
        /// Check period depending on data delivering frequency
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="EndPeriod">End period</param>
        /// <returns>Period End</returns>
        public static string CheckPeriodValidity(WebSession _session, string EndPeriod)
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
        private static string GetAbsoluteEndPeriod(WebSession _session, string EndPeriod, int divisor)
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

        #region Get Period Label
        /// <summary>
        ///  Get Period label in product class analysis depending on selected year
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="period">period</param>
        /// <returns>Label describing period in Product Class Analysis depending on selected year</returns>
        public static string getPeriodLabel(WebSession _session, CstPeriod.Type period)
        {

            string beginPeriod = "";
            string endPeriod = "";
            string year = "";


            switch (period)
            {
                case CstWeb.CustomerSessions.Period.Type.currentYear:
                    beginPeriod = _session.PeriodBeginningDate;
                    endPeriod = CheckPeriodValidity(_session, _session.PeriodEndDate);

                    break;
                case CstWeb.CustomerSessions.Period.Type.previousYear:
                    year = (int.Parse(_session.PeriodBeginningDate.Substring(0, 4)) - 1).ToString();
                    beginPeriod = year + _session.PeriodBeginningDate.Substring(4);
                    endPeriod = year + CheckPeriodValidity(_session, _session.PeriodEndDate).Substring(4);

                    break;
                default:
                    throw new ArgumentException(string.Format("Unable to treat this type of period ({0}) .", period.ToString()));
            }

            return Convertion.ToHtmlString(switchPeriod(_session, beginPeriod, endPeriod));
        }

        /// <summary>
        /// Display of period in product classs analysis
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="beginPeriod">Period beginning</param>
        /// <param name="endPeriod">End of period</param>
        /// <returns>Selected period</returns>
        public static string switchPeriod(WebSession _session, string beginPeriod, string endPeriod)
        {

            string periodText;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            switch (_session.PeriodType)
            {

                case CstPeriod.Type.nLastMonth:
                case CstPeriod.Type.dateToDateMonth:
                case CstPeriod.Type.previousMonth:
                case CstPeriod.Type.currentYear:
                case CstPeriod.Type.previousYear:
                case CstPeriod.Type.nextToLastYear:

                    if (beginPeriod != endPeriod)
                        periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + "-" + MonthString.GetCharacters(int.Parse(endPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
                    else
                        periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
                    break;
                default:
                    throw new Exception("switchPeriod(_session _session,string beginPeriod,string endPeriod)-->Unable to determine type of period.");

            }

            return periodText;
        }

        #endregion

        #region Begin of a period
        /// <summary>
        /// Extract begin of a period from period and type of period
        /// </summary>
        /// <param name="period">Studied period</param>
        /// <param name="periodType">Type of period</param>
        /// <returns>Begin of the period</returns>
        /// <remarks>
        /// Use class:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getPeriodBeginningDate(string period, CustomerSessions.Period.Type periodType)
        {
            AtomicPeriodWeek tmpWeek;
            switch (periodType)
            {
                case CstPeriod.Type.dateToDateWeek:
                case CstPeriod.Type.nLastWeek:
                case CstPeriod.Type.previousWeek:
                case CstPeriod.Type.LastLoadedWeek:
                    if (period.Length == 6)
                    {
                        tmpWeek = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                        return tmpWeek.FirstDay;
                    }
                    else
                    {
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                    }
                case CstPeriod.Type.dateToDateMonth:
                case CstPeriod.Type.nLastMonth:
                case CstPeriod.Type.LastLoadedMonth:
                case CstPeriod.Type.nLastYear:
                case CstPeriod.Type.previousMonth:
                case CstPeriod.Type.previousYear:
                case CstPeriod.Type.nextToLastYear:
                case CstPeriod.Type.currentYear:
                    if (period.Length == 6)
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1);
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                default:
                case CstPeriod.Type.previousDay:
                case CstPeriod.Type.nLastDays:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
            }
        }
        #endregion

        #region End of a period
        /// <summary>
        /// Extract period end
        /// </summary>
        /// <param name="period">Period to study</param>
        /// <param name="periodType">Type of period</param>
        /// <returns>End of the period</returns>
        /// <remarks>
        /// Uses TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getPeriodEndDate(string period, CstPeriod.Type periodType)
        {
            switch (periodType)
            {
                case CstPeriod.Type.dateToDateWeek:
                case CstPeriod.Type.nLastWeek:
                case CstPeriod.Type.previousWeek:
                case CstPeriod.Type.LastLoadedWeek:
                    if (period.Length == 6)
                        return (new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)))).LastDay;
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                case CstPeriod.Type.dateToDateMonth:
                case CstPeriod.Type.LastLoadedMonth:
                case CstPeriod.Type.nLastMonth:
                case CstPeriod.Type.nLastYear:
                case CstPeriod.Type.previousMonth:
                case CstPeriod.Type.previousYear:
                case CstPeriod.Type.nextToLastYear:
                case CstPeriod.Type.currentYear:
                    if (period.Length == 6)
                        return (new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1)).AddMonths(1).AddDays(-1);
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                default:
                case CstPeriod.Type.nLastDays:
                case CstPeriod.Type.previousDay:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
            }
        }
        #endregion

        #region Year ID : 0==N , 1==N-1,2==N-2 (only for Analyse secto==to move)
        /// <summary>
        /// Get ID of the selected year : 0==N , 1==N-1,2==N-2
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="yearStr">(out) year ID as string ("", "1", "2")</param>
        /// <param name="idYear">(out) Id of year</param>
        /// <param name="period">Begin of the period</param>
        public static void GetYearSelected(WebSession session, ref string yearStr, ref int idYear, DateTime period)
        {
            int year = yearID(period, session);
            if (year > 0)
            {
                yearStr = year.ToString();
            }
            else
            {
                yearStr = string.Empty;
            }

        }
        /// <summary>
        /// Get year : 0==N , 1==N-1,2==N-2
        /// </summary>
        /// <param name="period">DateTime</param>		
        ///<param name="session">User Session</param>
        /// <returns>ID</returns>
        public static int yearID(DateTime period, WebSession session)
        {

            if (DateTime.Now.Year > session.DownLoadDate)
            {
                return (DateTime.Now.Year - period.Year - 1);
            }
            else
            {
                return (DateTime.Now.Year - period.Year);
            }

            return 0;
        }
        #endregion

        #region Identifie le mois actif
        /// <summary>
        /// Get last month of the selected period in product class analysis
        /// </summary>
        /// <param name="period">Period to study</param>
        ///<param name="session">User Session</param>
        /// <returns>Month</returns>
        public static string CurrentActiveMonth(DateTime period, WebSession session)
        {
            string s = "";
            if (period.Month == DateTime.Now.Month)
            {
                s = GetMonthAlias(period.Month - 1, yearID(period.AddMonths(-1).Date, session), 3, session);
            }
            else
            {
                s = GetMonthAlias(period.Month, yearID(period, session), 3, session);
            }
            return s;
        }
        #endregion

        #region Get alias of a month
        /// <summary>
        /// Get Label of the month as MMMYY
        /// </summary>
        /// <param name="monthNumber">Month ID (01 - 12)</param>
        /// <param name="YearSelected">Selected Year</param>
        /// <param name="numberOfChar">Number of characters in label (2-->MMYY, 4-->MMMMYY)</param>
        /// <param name="session">User session</param>
        /// <returns>Label of the month as MMMYY</returns>
        public static string GetMonthAlias(int monthNumber, int YearSelected, int numberOfChar, WebSession session)
        {
            string month = "";
            string year = "";
            if (DateTime.Now.Year > session.DownLoadDate)
            {
                YearSelected++;
            }

            try
            {
                #region Month
                switch (monthNumber)
                {
                    case 1:
                        month = "January";
                        break;
                    case 2:
                        month = "February";
                        break;
                    case 3:
                        month = "March";
                        break;
                    case 4:
                        month = "April";
                        break;
                    case 5:
                        month = "May";
                        break;
                    case 6:
                        month = "June";
                        break;
                    case 7:
                        month = "July";
                        break;
                    case 8:
                        month = "August";
                        break;
                    case 9:
                        month = "September";
                        break;
                    case 10:
                        month = "October";
                        break;
                    case 11:
                        month = "November";
                        break;
                    case 12:
                        month = "December";
                        break;

                    default:
                        throw (new ArgumentException("Unvalid month parameter. must be between 1 and 12."));
                }
                #endregion

                #region Year
                if (YearSelected <= 3)
                {
                    year = System.DateTime.Now.AddYears(YearSelected*-1).ToString("yy");
                }
                else
                {
                    throw (new ArgumentException("Unvalid selected year. Must be between 0 and 3."));
                }
                #endregion

                if (numberOfChar <= month.Length && numberOfChar > 0){
                    return string.Format("{0}{1}", month.Substring(0, numberOfChar), year);
                }
                return (month);

            }
            catch (System.Exception e)
            {
                throw new Exception("Unable to build alias : " + e.Message, e);
            }
        }
        #endregion

        #region Get Date from alias MMMYY
        /// <summary>
        /// Get date from alias MMMYY
        /// </summary>
        /// <param name="DateAlias">Date alias</param>		
        /// <returns>Date</returns>
        public static DateTime GetDateFromAlias(string alias)
        {
            string month = "";
            int intMonth = 0;
            int year = 0;
            if (alias.Length != 5)
            {
                throw new ArgumentException("Date format not supported. Must be MMMYY.");
            }
            try
            {
                month = alias.ToString().Substring(0, 3);
                year = 2000 + Convert.ToInt32(alias.ToString().Substring(3, 2));
            }
            catch (Exception)
            {
                throw new ArgumentException("Date format not supported. Must be MMMYY.");
            }
            switch (month)
            {
                case "Jan":
                    intMonth = 1;
                    break;
                case "Feb":
                    intMonth = 2;
                    break;
                case "Mar":
                    intMonth = 3;
                    break;
                case "Apr":
                    intMonth = 4;
                    break;
                case "May":
                    intMonth = 5;
                    break;
                case "Jun":
                    intMonth = 6;
                    break;
                case "Jul":
                    intMonth = 7;
                    break;
                case "Aug":
                    intMonth = 8;
                    break;
                case "Sep":
                    intMonth = 9;
                    break;
                case "Oct":
                    intMonth = 10;
                    break;
                case "Nov":
                    intMonth = 11;
                    break;
                case "Dec":
                    intMonth = 12;
                    break;
                default:
                    throw (new ArgumentException("Input date not supported : unknown month."));
            }
            return new DateTime(year, intMonth, 1);
        }
        #endregion

        #region Get Month Label
        /// <summary>
        /// Retourne month label matching the language
        /// </summary>
        /// <param name="month">Month to translate</param>
        /// <param name="language">Translation language</param>
        /// <returns>Month Label</returns>
        public static string GetMonthLabel(int month, int language)
        {
            switch (month)
            {
                case 1:
                    return GestionWeb.GetWebWord(945, language);
                case 2:
                    return GestionWeb.GetWebWord(946, language);
                case 3:
                    return GestionWeb.GetWebWord(947, language);
                case 4:
                    return GestionWeb.GetWebWord(948, language);
                case 5:
                    return GestionWeb.GetWebWord(949, language);
                case 6:
                    return GestionWeb.GetWebWord(950, language);
                case 7:
                    return GestionWeb.GetWebWord(951, language);
                case 8:
                    return GestionWeb.GetWebWord(952, language);
                case 9:
                    return GestionWeb.GetWebWord(953, language);
                case 10:
                    return GestionWeb.GetWebWord(954, language);
                case 11:
                    return GestionWeb.GetWebWord(955, language);
                case 12:
                    return GestionWeb.GetWebWord(956, language);
                default:
                    throw new ArgumentException(string.Format("Unknown month {0}.", month));
            }
            return string.Empty;
        }
        #endregion


    }
}
