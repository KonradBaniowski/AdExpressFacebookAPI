#region Information
/*
 * auteur : 
 * cr�� le :
 * modification : 
 *      22/07/2004 - - Guillaume Ragneau
 *      22/07/2008 - D�placement depuis TNS.AdExpress.Web.Functions
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
using FrameWorkCsts = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.Layers;

namespace TNS.AdExpress.Web.Core.Utilities
{

    /// <summary>
    /// Set of function usefull to treat periods
    /// </summary>
    public class Dates
    {

        #region Get Period Label
       

        /// <summary>
        ///  Get Period label in product class analysis depending on selected year
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="beginPeriod">Begin Period</param>
        /// <param name="endPeriod">End Period</param>
        /// <returns>Label describing period in Product Class Analysis depending on selected year</returns>
        public static string getPeriodLabel(WebSession _session, string beginPeriod, string endPeriod)
        {
            string periodText;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            if (beginPeriod != endPeriod)
                periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + "-" + MonthString.GetCharacters(int.Parse(endPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
            else
                periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);

            return periodText;
        }

        /// <summary>
        ///  Get Period label in product class analysis depending on selected year
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="beginPeriod">Begin Period</param>
        /// <param name="endPeriod">End Period</param>
        /// <returns>Label describing period in Product Class Analysis depending on selected year</returns>
        public static string getPeriodLabelComparative(WebSession _session, string beginPeriod, string endPeriod)
        {
            string periodText;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            if (beginPeriod.Substring(0, 6) != endPeriod.Substring(0, 6))
                if(beginPeriod.Substring(0, 4) == endPeriod.Substring(0, 4))
                    periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " - " + MonthString.GetCharacters(int.Parse(endPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);
                else
                    periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4) + "-" + MonthString.GetCharacters(int.Parse(endPeriod.Substring(4, 2)), cultureInfo, 0) + " " + endPeriod.Substring(0, 4);
            else
                periodText = MonthString.GetCharacters(int.Parse(beginPeriod.Substring(4, 2)), cultureInfo, 0) + " " + beginPeriod.Substring(0, 4);

            return periodText;
        }

      

        #endregion

        #region Get period dateTime
        /// <summary>
        /// Extract period DateTime from string
        /// </summary>
        /// <param name="period">Studied period</param>
        /// <param name="displayLevel">Type of period</param>
        /// <returns>Begin of the period</returns>
        /// <remarks>
        /// Use class:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getPeriodDate(string period, CstPeriod.DisplayLevel displayLevel) {
            AtomicPeriodWeek tmpWeek;
            switch (displayLevel) {
                case CstPeriod.DisplayLevel.weekly:
                    if (period.Length == 6) {
                        tmpWeek = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                        return tmpWeek.FirstDay;
                    }
                    else {
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                    }
                case CstPeriod.DisplayLevel.monthly:
                    if (period.Length == 6)
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1);
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                default:
                case CstPeriod.DisplayLevel.dayly:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
            }
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
        /// <summary>
        /// Extract period end
        /// </summary>
        /// <param name="period">Period to study</param>
        /// <param name="periodType">Type of period</param>
        /// <param name="comparative">Comparative</param>
        /// <returns>End of the period</returns>
        /// <remarks>
        /// Uses TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getPeriodEndDate(string period, CstPeriod.Type periodType, bool comparative) {
            switch (periodType) {
                case CstPeriod.Type.dateToDateWeek:
                case CstPeriod.Type.nLastWeek:
                case CstPeriod.Type.previousWeek:
                case CstPeriod.Type.LastLoadedWeek:
                    if (period.Length == 6)
                        if (comparative)
                            return (new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)))).LastDay;
                        else
                            return (new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)))).LastDay;
                    else
                        if (comparative)
                            return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                        else
                            return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                case CstPeriod.Type.dateToDateMonth:
                case CstPeriod.Type.LastLoadedMonth:
                case CstPeriod.Type.nLastMonth:
                case CstPeriod.Type.nLastYear:
                case CstPeriod.Type.previousMonth:
                case CstPeriod.Type.previousYear:
                case CstPeriod.Type.nextToLastYear:
                case CstPeriod.Type.currentYear:
                    if (period.Length == 6)
                        if (comparative)
                            return (new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), 1)).AddMonths(1).AddDays(-1);
                        else
                            return (new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), 1)).AddMonths(1).AddDays(-1);
                    else
                        if (comparative)
                            return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                        else
                            return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                default:
                case CstPeriod.Type.nLastDays:
                case CstPeriod.Type.previousDay:
                    if (comparative)
                        return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)) - 1, int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
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
            idYear = yearID(period, session);
            if (idYear > 0)
            {
                yearStr = idYear.ToString();
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
                if (YearSelected <= WebApplicationParameters.DataNumberOfYear)
                {
                    year = System.DateTime.Now.AddYears(YearSelected*-1).ToString("yy");
                }
                else
                {
                    throw (new ArgumentException("Unvalid selected year. Must be between 0 and " + WebApplicationParameters.DataNumberOfYear));
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
            switch (month.ToUpper())
            {
                case "JAN":
                    intMonth = 1;
                    break;
                case "FEB":
                    intMonth = 2;
                    break;
                case "MAR":
                    intMonth = 3;
                    break;
                case "APR":
                    intMonth = 4;
                    break;
                case "MAY":
                    intMonth = 5;
                    break;
                case "JUN":
                    intMonth = 6;
                    break;
                case "JUL":
                    intMonth = 7;
                    break;
                case "AUG":
                    intMonth = 8;
                    break;
                case "SEP":
                    intMonth = 9;
                    break;
                case "OCT":
                    intMonth = 10;
                    break;
                case "NOV":
                    intMonth = 11;
                    break;
                case "DEC":
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

		#region DateTime => dd/MM/YYYY ou MM/dd/YYYY suivant la langue
		/// <summary>
		/// Get date string dd MM YYYY  acording to culture information
		/// </summary>
		/// <param name="date">Date to fromat</param>
		/// <param name="language">Site langage </param>
		/// <returns>dd/MM/YYYY ou MM/dd/YYYY or MM.dd.YYYY etc</returns>
        /// <remarks>Warning : The '/' caractere define to xml can change with culture of Operating System</remarks>
		public static string DateToString(DateTime date, int language) {

			AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[language].CultureInfo;
			return date.ToString(cInfo.GetFormatPattern(FrameWorkCsts.Dates.Pattern.shortDatePattern.ToString()));
		}
		#endregion

        #region DateTime to string acording to culture information and date format
        /// <summary>
        /// Get date string acording to culture information
        /// </summary>
        /// <param name="date">Date to fromat</param>
        /// <param name="language">Site langage </param>
        /// <param name="dateFormat">Date format</param>
        /// <returns>dd/MM/YYYY ou MM/dd/YYYY or MM.dd.YYYY etc</returns>
        public static string DateToString(DateTime date, int language, FrameWorkCsts.Dates.Pattern dateFormat) {

            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[language].CultureInfo;
            return date.ToString(cInfo.GetFormatPattern(dateFormat.ToString()), cInfo);
        }
        #endregion

		#region YYYYMMDDToDD_MM_YYYY
		/// <summary>
		/// Transforme une date sous la forme YYYYMMDD en DD/MM/YYYY ou MM/DD/YYYY suivant la langue
		/// </summary>
		/// <param name="date">Date � convertir</param>
		/// <param name="language">Identifiant de la langue</param>
		/// <returns>Date convertie</returns>
		public static string YYYYMMDDToDD_MM_YYYY(string date, int language) {
			if (date.Length != 8) throw (new ArgumentException("La date en entr�e n'est pas valide"));
			return DateToString(new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))), language);
		}
		#endregion

        #region GetPreviousYearDate
        /// <summary>
        /// Obtient la date de l'ann�e pr�c�dente
        /// </summary>
        /// <param name="date">date de l'ann�e en cours</param>
        /// <param name="comparativePeriodType">Type de la p�riode comparative</param>
        /// <returns>date de l'ann�e pr�c�dente</returns>
        public static DateTime GetPreviousYearDate(DateTime date, Constantes.Web.globalCalendar.comparativePeriodType comparativePeriodType) {

            AtomicPeriodWeek tmpWeek;
            int currentDay;

            switch (comparativePeriodType) {

                case Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                    date = date.AddYears(-1);
                    break;
                case Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    currentDay = date.DayOfWeek.GetHashCode();
                    tmpWeek = new AtomicPeriodWeek(date);
                    tmpWeek.SubWeek(52);
                    if (currentDay == 0)
                        date = tmpWeek.FirstDay.AddDays(6);
                    else
                        date = tmpWeek.FirstDay.AddDays(currentDay - 1);
                    break;
                default:
                    throw new ArgumentException("Comparative Period Type not valid !!!");
            }

            return date;

        }
        #endregion

        #region GetNextYearDate
        /// <summary>
        /// Obtient la date de l'ann�e prochaine
        /// </summary>
        /// <param name="date">date de l'ann�e en cours</param>
        /// <param name="comparativePeriodType">Type de la p�riode comparative</param>
        /// <returns>date de l'ann�e prochaine</returns>
        public static DateTime GetNextYearDate(DateTime date, Constantes.Web.globalCalendar.comparativePeriodType comparativePeriodType)
        {

            AtomicPeriodWeek tmpWeek;
            int currentDay;

            switch (comparativePeriodType)
            {

                case Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                    date = date.AddYears(1);
                    break;
                case Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    currentDay = date.DayOfWeek.GetHashCode();
                    tmpWeek = new AtomicPeriodWeek(date);
                    tmpWeek.AddWeek(52);
                    if (currentDay == 0)
                        date = tmpWeek.FirstDay.AddDays(6);
                    else
                        date = tmpWeek.FirstDay.AddDays(currentDay - 1);
                    break;
                default:
                    throw new ArgumentException("Comparative Period Type not valid !!!");
            }

            return date;

        }
        #endregion

        #region GetMonthFromWeek
        /// <summary>
        /// D�termine � quel mois appartient la semaine
        /// </summary>
        /// <param name="week">Num�ro de semaine</param>
        /// <returns>Mois de la semaine</returns>
        public static int GetMonthFromWeek(int year, int week)
        {
            DateTime firstDay = (new AtomicPeriodWeek(year, week)).FirstDay;
            int firstDayMonthNumber = 0;
            int otherMonthNumber = 0;
            int firstDayMonth = firstDay.Month;
            int i;
            for (i = 1; i < 8; i++)
            {
                if (firstDay.Month == firstDayMonth) firstDayMonthNumber++;
                else otherMonthNumber++;
                firstDay = firstDay.AddDays(1.0);
            }
            if (firstDayMonthNumber > 3) return (firstDayMonth);
            else return (firstDay.Month);

        }
        #endregion
      

    }
}
