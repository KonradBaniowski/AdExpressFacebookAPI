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
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Date;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using FrameWorkCsts = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.Layers;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpress.Web.Core.Utilities
{

    /// <summary>
    /// Set of function usefull to treat periods
    /// </summary>
    public class Dates
    {
        /// <summary>
        ///  func to manage PeriodDisponibilityType
        /// </summary>
        /// <param name="webSession"></param>
        /// <returns></returns>
        public static string GetPeriodDisponibilityTypeDetail(WebSession webSession)
        {
            try
            {
                switch (webSession.CustomerPeriodSelected.PeriodDisponibilityType)
                {
                    case globalCalendar.periodDisponibilityType.currentDay:
                        return GestionWeb.GetWebWord(2297, webSession.SiteLanguage);
                    case globalCalendar.periodDisponibilityType.lastCompletePeriod:
                        return GestionWeb.GetWebWord(2298, webSession.SiteLanguage);
                    default:
                        return "";
                }
            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate the html code for period disponibility type detail", e));
            }
        }

        /// <summary>
        ///  func to manage comparativePeriodType
        /// </summary>
        /// <param name="webSession"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static string GetComparativePeriodTypeDetail(WebSession webSession, long moduleId)
        {
            try
            {
                globalCalendar.comparativePeriodType comparativePeriodType;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    comparativePeriodType = webSession.ComparativePeriodType;
                else
                    comparativePeriodType = webSession.CustomerPeriodSelected.ComparativePeriodType;

                switch (comparativePeriodType)
                {
                    case globalCalendar.comparativePeriodType.comparativeWeekDate:
                        return GestionWeb.GetWebWord(2295, webSession.SiteLanguage);
                    case globalCalendar.comparativePeriodType.dateToDate:
                        return GestionWeb.GetWebWord(2294, webSession.SiteLanguage);
                    default:
                        return "";
                }
            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate the html code for study period detail", e));
            }
        }

        /// <summary>
        /// func to manage comparativePeriod
        /// </summary>
        /// <param name="webSession"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static string GetComparativePeriodDetail(WebSession webSession, long moduleId)
        {
            try
            {
                string dateBegin;
                string dateEnd;
                DateTime dateBeginDT;
                DateTime dateEndDT;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    // get date begin and date end according to period type
                    dateBeginDT = Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                    dateEndDT = Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);

                    // get comparative date begin and date end
                    dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, webSession.ComparativePeriodType);
                    dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, webSession.ComparativePeriodType);

                    // Formating date begin and date end
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(dateBeginDT.ToString("yyyyMMdd"), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(dateEndDT.ToString("yyyyMMdd"), webSession.SiteLanguage);

                }
                else {
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);
                }

                if (!dateBegin.Equals(dateEnd))
                    return GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + dateBegin + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + dateEnd;
                else return " " + dateBegin;

            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate the html code for comparative period type detail", e));
            }
        }

        /// <summary>
        ///  func to manage StudyPeriod
        /// </summary>
        /// <param name="webSession"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static string GetStudyPeriodDetail(WebSession webSession, long moduleId)
        {
            try
            {
                string dateBegin;
                string dateEnd;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    dateBegin = Dates.YYYYMMDDToDD_MM_YYYY2(webSession.PeriodBeginningDate.ToString(), webSession.SiteLanguage);
                    dateEnd = Dates.YYYYMMDDToDD_MM_YYYY2(webSession.PeriodEndDate.ToString(), webSession.SiteLanguage);
                }
                else {
                    dateBegin = Dates.YYYYMMDDToDD_MM_YYYY2(webSession.CustomerPeriodSelected.StartDate.ToString(), webSession.SiteLanguage);
                    dateEnd = Dates.YYYYMMDDToDD_MM_YYYY2(webSession.CustomerPeriodSelected.EndDate.ToString(), webSession.SiteLanguage);
                }

                if (!dateBegin.Equals(dateEnd))
                    return GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + dateBegin + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + dateEnd;
                else return " " + dateBegin;
            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate the html code for study period detail", e));
            }
        }


        /// <summary>
        /// Check if two dates are set, normally begin and end date
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool isPeriodSet(DateTime? begin, DateTime? end)
        {
            return (begin.HasValue && end.HasValue) ?true : false;
        }

        #region 
        public static string GetPeriodDetail(WebSession webSession)
        {
            try
            {
                string str = "";
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

                switch (webSession.PeriodType)
                {
                    case CustomerSessions.Period.Type.nLastMonth:
                        return webSession.PeriodLength.ToString() + " " + GestionWeb.GetWebWord(783, webSession.SiteLanguage);
                    case CustomerSessions.Period.Type.nLastYear:
                        return webSession.PeriodLength.ToString() + " " + GestionWeb.GetWebWord(781, webSession.SiteLanguage);
                    case CustomerSessions.Period.Type.previousMonth:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(788, webSession.SiteLanguage));
                    // Année courante		
                    case CustomerSessions.Period.Type.currentYear:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(1228, webSession.SiteLanguage));
                    // Année N-1
                    case CustomerSessions.Period.Type.previousYear:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(787, webSession.SiteLanguage));
                    // Année N-2
                    case CustomerSessions.Period.Type.nextToLastYear:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(1229, webSession.SiteLanguage));

                    case CustomerSessions.Period.Type.dateToDateMonth:
                        string monthBegin;
                        string monthEnd;
                        string yearBegin;
                        string yearEnd;
                        if (int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4, 2)) < 10)
                        {
                            monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(5, 1)), cultureInfo, 10);
                            yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0, 4);
                        }
                        else {
                            monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4, 2)), cultureInfo, 10);
                            yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0, 4);
                        }
                        if (int.Parse(webSession.PeriodEndDate.ToString().Substring(4, 2)) < 10)
                        {
                            monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(5, 1)), cultureInfo, 10);
                            yearEnd = webSession.PeriodEndDate.ToString().Substring(0, 4);
                        }
                        else {
                            monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(4, 2)), cultureInfo, 10);
                            yearEnd = webSession.PeriodEndDate.ToString().Substring(0, 4);
                        }
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(846, webSession.SiteLanguage) + " " + monthBegin + " " + yearBegin + " " + GestionWeb.GetWebWord(847, webSession.SiteLanguage) + " " + monthEnd + " " + yearEnd);
                    case CustomerSessions.Period.Type.dateToDateWeek:
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4, 2)));
                        str = FctUtilities.Dates.DateToString(tmp.FirstDay.Date, webSession.SiteLanguage);
                        tmp = new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0, 4)), int.Parse(webSession.PeriodEndDate.ToString().Substring(4, 2)));
                        str += " " + GestionWeb.GetWebWord(125, webSession.SiteLanguage) + "";
                        str += " " + FctUtilities.Dates.DateToString(tmp.LastDay.Date, webSession.SiteLanguage) + "";
                        return str;
                    case CustomerSessions.Period.Type.nLastWeek:
                        //return Convertion.ToHtmlString(webSession.PeriodLength.ToString() + " " + GestionWeb.GetWebWord(784, webSession.SiteLanguage));
                        return webSession.PeriodLength.ToString() + " " + GestionWeb.GetWebWord(784, webSession.SiteLanguage);
                    case CustomerSessions.Period.Type.previousWeek:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(789, webSession.SiteLanguage));
                    case CustomerSessions.Period.Type.dateToDate:
                    case CustomerSessions.Period.Type.cumlDate:
                    case CustomerSessions.Period.Type.personalize:
                        string dateBegin;
                        string dateEnd;
                        dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(webSession.PeriodBeginningDate.ToString(), webSession.SiteLanguage);
                        dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY2(webSession.PeriodEndDate.ToString(), webSession.SiteLanguage);
                        if (!dateBegin.Equals(dateEnd))
                            return Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + dateBegin + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + dateEnd);
                        else return " " + dateBegin;
                    case CustomerSessions.Period.Type.previousDay:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(1975, webSession.SiteLanguage));
                    case CustomerSessions.Period.Type.nLastDays:
                        return Convertion.ToHtmlString(webSession.PeriodLength.ToString() + " " + GestionWeb.GetWebWord(1974, webSession.SiteLanguage));
                    case CustomerSessions.Period.Type.LastLoadedMonth:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(1619, webSession.SiteLanguage));
                    case CustomerSessions.Period.Type.LastLoadedWeek:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(1618, webSession.SiteLanguage));
                    case CustomerSessions.Period.Type.cumulWithNextMonth:
                    case CustomerSessions.Period.Type.allHistoric:
                    case CustomerSessions.Period.Type.currentMonth:
                        foreach (DateConfiguration cVpDateConfiguration in WebApplicationParameters.VpDateConfigurations.VpDateConfigurationList)
                        {
                            if (cVpDateConfiguration.DateType == webSession.PeriodType)
                                return Convertion.ToHtmlString(GestionWeb.GetWebWord(cVpDateConfiguration.TextId, webSession.SiteLanguage));
                        }
                        return "";
                    default:
                        return "";
                }
            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate the html code for period detail", e));
            }
        }
        #endregion

        #region YYYYMM => "Month, Year" ou YYYYSS => "Week nn, Year"
        /// <summary>
        /// Transforme une periode  sous forme YYYYMM ou YYYYSS en "Month, Yeaar" ou "Week nn, Year"
        /// </summary>
        /// <param name="webSession">Session utilisateur</param>
        /// <param name="period">Période à "traduire"</param>
        /// <returns>"Month, Year" ou "Week nn, Year"</returns>
        public static string getPeriodTxt(WebSession webSession, string period)
        {

            StringBuilder txt = new StringBuilder(20);

            // Texte de période
            if (webSession.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            {
                txt.AppendFormat("{0} {1} ({2})", GestionWeb.GetWebWord(848, webSession.SiteLanguage), period.Substring(4, 2), period.Substring(0, 4));
            }
            else
            {
                txt.AppendFormat("{0} {1}", GetMonthLabel(Convert.ToInt32(period.Substring(4, 2)), webSession.SiteLanguage), period.Substring(0, 4));
            }
            return txt.ToString();

        }
        #endregion

        #region Compare two dates and return the greater
        /// <summary>
        /// Compare two dates and return the greater
        /// </summary>
        /// <param name="date1">First Date Param</param>
        /// <param name="date2">Second Date Parm</param>
        /// <returns>Greater Date</returns>
        public static DateTime Max(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) >= 0)
            {
                return date1;
            }
            return date2;
        }
        #endregion

        #region Compare two dates and return the smaller
        /// <summary>
        /// Compare two dates and return the smalelr
        /// </summary>
        /// <param name="date1">First Date Param</param>
        /// <param name="date2">Second Date Parm</param>
        /// <returns>Smaller Date</returns>
        public static DateTime Min(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) >= 0)
            {
                return date2;
            }
            return date1;
        }
        #endregion

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
		/// <param name="date">Date à convertir</param>
		/// <param name="language">Identifiant de la langue</param>
		/// <returns>Date convertie</returns>
		public static string YYYYMMDDToDD_MM_YYYY2(string date, int language) {
			if (date.Length != 8) throw (new ArgumentException("La date en entrée n'est pas valide"));
			return DateToString(new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))), language);
		}

        /// <summary>
        /// New method refacto without Datetostring
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? YYYYMMDDToDD_MM_YYYY(string date)
        {
            if (date.Length != 8) throw (new ArgumentException("La date en entrée n'est pas valide"));
            return new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
        }
        #endregion

        #region GetPreviousYearDate
        /// <summary>
        /// Obtient la date de l'année précédente
        /// </summary>
        /// <param name="date">date de l'année en cours</param>
        /// <param name="comparativePeriodType">Type de la période comparative</param>
        /// <returns>date de l'année précédente</returns>
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
        /// Obtient la date de l'année prochaine
        /// </summary>
        /// <param name="date">date de l'année en cours</param>
        /// <param name="comparativePeriodType">Type de la période comparative</param>
        /// <returns>date de l'année prochaine</returns>
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
        /// Détermine à quel mois appartient la semaine
        /// </summary>
        /// <param name="week">Numéro de semaine</param>
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

        #region Is 4M
        /// <summary>
        /// Is 4M
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <returns>true if 4M</returns>
        public static bool Is4M(int startDate) {

            return Is4M(startDate.ToString());

        }
        /// <summary>
        /// Is 4M
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <returns>true if 4M</returns>
        public static bool Is4M(string startDate) {

            int month4M = DateTime.Now.AddMonths(-3).Year * 100 + DateTime.Now.AddMonths(-3).Month;
            int date = Convert.ToInt32(startDate.Substring(0, 6));

            if (date >= month4M) return true;

            return false;

        }
        #endregion

        #region Date de début d'un zoom en fonction d'un type de période
        /// <summary>
        /// Foction qui extrait à partir d'un zoom et d'un type de période la date de début de cette période zoom
        /// </summary>
        /// <param name="period">Zoom dont on veut la date de début</param>
        /// <param name="periodType">Type de période considérée</param>
        /// <returns>Date de début de zoom</returns>
        /// <remarks>
        /// Utilise la classe:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getZoomBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
        {
            AtomicPeriodWeek tmpWeek;
            switch (periodType)
            {
                case CstCustomerSession.Period.Type.dateToDateWeek:
                case CstCustomerSession.Period.Type.nLastWeek:
                case CstCustomerSession.Period.Type.previousWeek:
                case CstCustomerSession.Period.Type.LastLoadedWeek:
                    tmpWeek = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                    return tmpWeek.FirstDay;
                case CstCustomerSession.Period.Type.dateToDateMonth:
                case CstCustomerSession.Period.Type.nLastMonth:
                case CstCustomerSession.Period.Type.LastLoadedMonth:
                case CstCustomerSession.Period.Type.nLastYear:
                case CstCustomerSession.Period.Type.previousMonth:
                case CstCustomerSession.Period.Type.previousYear:
                case CstCustomerSession.Period.Type.nextToLastYear:
                case CstCustomerSession.Period.Type.currentYear:
                case CstCustomerSession.Period.Type.previousDay:
                case CstCustomerSession.Period.Type.nLastDays:
                default:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1);
            }
        }
        #endregion

        #region Date de fin d'un zoom en fonction d'un type de période
        /// <summary>
        /// Fonction qui extrait à partir d'un zoom et d'un type de période la date de fin de cette période
        /// </summary>
        /// <param name="period">Zoom dont on veut la date de fin</param>
        /// <param name="periodType">Type de période considérée</param>
        /// <returns>Date de fin de zoom</returns>
        /// <remarks>
        /// Utilise la classe:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getZoomEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
        {
            switch (periodType)
            {
                case CstCustomerSession.Period.Type.dateToDateWeek:
                case CstCustomerSession.Period.Type.nLastWeek:
                case CstCustomerSession.Period.Type.previousWeek:
                case CstCustomerSession.Period.Type.LastLoadedWeek:
                    return (new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)))).LastDay;
                case CstCustomerSession.Period.Type.dateToDateMonth:
                case CstCustomerSession.Period.Type.LastLoadedMonth:
                case CstCustomerSession.Period.Type.nLastMonth:
                case CstCustomerSession.Period.Type.nLastYear:
                case CstCustomerSession.Period.Type.previousMonth:
                case CstCustomerSession.Period.Type.previousYear:
                case CstCustomerSession.Period.Type.nextToLastYear:
                case CstCustomerSession.Period.Type.currentYear:
                default:
                case CstCustomerSession.Period.Type.nLastDays:
                case CstCustomerSession.Period.Type.previousDay:
                    return (new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1)).AddMonths(1).AddDays(-1);
            }
        }
        #region  Dernier mois chargé
        /// <summary>
        /// Dernier mois chargé
        /// </summary>
        /// <param name="PeriodBeginningDate">date de début</param>		
        /// <param name="PeriodEndDate">date de fin</param>	
        ///  <param name="periodType">type de période</param>
        public static void LastLoadedMonth(ref string PeriodBeginningDate, ref string PeriodEndDate, CstPeriodType periodType)
        {
            double currentWeek = ((double)DateTime.Now.Day / (double)7);
            currentWeek = Math.Ceiling(currentWeek);
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);

            if (DateTime.Now.Month > 2)
            {
                if (IsPeriodActive(currentWeek, week, periodType))
                {
                    PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                    PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                }
                else {
                    PeriodBeginningDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
                    PeriodEndDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
                }
            }
            else {
                if (DateTime.Now.Month == 2 && IsPeriodActive(currentWeek, week, periodType))
                {
                    PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                    PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                }
                else {
                    if (DateTime.Now.Month == 2 || IsPeriodActive(currentWeek, week, periodType))
                    {
                        PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
                        PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
                    }
                    else {
                        PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
                        PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Période active
        /// <summary>
        /// Vérifie si une période peut être traitée
        /// </summary>
        /// <param name="currentWeek">semaine courante</param>
        /// <param name="week">semaine </param>
        /// <param name="periodType">type de période</param>
        /// <returns>vrai si période active</returns>
        private static bool IsPeriodActive(double currentWeek, AtomicPeriodWeek week, CstPeriodType periodType)
        {
            bool enabled = false;
            AtomicPeriodWeek previousWeek;
            if (currentWeek == 1 && week.FirstDay.Month == week.LastDay.Month
                && (int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString()) >= 5
                || int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString()) == 0))
            {
                enabled = true;
            }
            //2ème semaine du mois en cours
            else if (currentWeek == 2)
            {
                if (((int)DateTime.Now.DayOfWeek >= 5 || (int)DateTime.Now.DayOfWeek == 0))
                {
                    enabled = true;
                }
                else {
                    previousWeek = new AtomicPeriodWeek(DateTime.Now.AddDays(-7));
                    if (previousWeek.FirstDay.Month == previousWeek.LastDay.Month)
                        enabled = true;
                }
                //Plus de 2 semaines du  mois en cours
            }
            else if (currentWeek > 2 && !((CstPeriodType.previousYear == periodType || (CstPeriodType.LastLoadedMonth == periodType && week.FirstDay.Month != 12) || CstPeriodType.LastLoadedWeek == periodType) && (week.Week == 52 || week.Week == 53)))
            {
                enabled = true;
            }
            return enabled;
        }
        #endregion

        #region Derniere semaine chargée
        /// <summary>
        /// Derniere semaine chargée
        /// </summary>
        /// <param name="PeriodBeginningDate">date de début</param>		
        /// <param name="PeriodEndDate">date de fin</param>			
        public static void LastLoadedWeek(ref string PeriodBeginningDate, ref string PeriodEndDate)
        {
            AtomicPeriodWeek currentWeek = new AtomicPeriodWeek(DateTime.Now);
            int numberWeek = currentWeek.Week;
            //			int LoadedWeek=0;		
            //			AtomicPeriodWeek previousYearWeek=new AtomicPeriodWeek(DateTime.Now.AddYears(-1));

            AtomicPeriodWeek previousWeek = new AtomicPeriodWeek(DateTime.Now.AddDays(-7));
            int days = 0;
            days = DateTime.Now.Subtract(previousWeek.LastDay).Days;
            if (days < 5)
            {
                previousWeek = new AtomicPeriodWeek(DateTime.Now.AddDays(-14));
            }
            PeriodBeginningDate = previousWeek.Year.ToString() + ((previousWeek.Week > 9) ? "" : "0") + previousWeek.Week.ToString();
            PeriodEndDate = previousWeek.Year.ToString() + ((previousWeek.Week > 9) ? "" : "0") + previousWeek.Week.ToString();
        }
        #endregion

        #region Période chargement des données
        /// <summary>
        /// Dates de chargement des données
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="PeriodBeginningDate">date de début</param>		
        /// <param name="PeriodEndDate">date de fin</param>
        /// <param name="periodType">type de période</param>
        public static void DownloadDates(WebSession webSession, ref string PeriodBeginningDate, ref string PeriodEndDate, CstPeriodType periodType)
        {
            double currentWeek = ((double)DateTime.Now.Day / (double)7);
            currentWeek = Math.Ceiling(currentWeek);
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);


            if (DateTime.Now.Month == 1)
            {
                if (CstPeriodType.previousYear == periodType)
                {
                    if (IsPeriodActive(currentWeek, week, periodType))
                    {
                        PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
                        PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
                    }
                    else {
                        PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
                        PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
                    }
                }
                else
                    throw new TNS.AdExpress.Domain.Exceptions.NoDataException(GestionWeb.GetWebWord(1612, webSession.SiteLanguage));
            }
            else {
                if (CstPeriodType.currentYear == periodType)
                {
                    if (IsPeriodActive(currentWeek, week, periodType))
                    {
                        PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
                        PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                    }
                    else {
                        if (((currentWeek == 1 || currentWeek == 2) && week.Week <= 2 && DateTime.Now.Year == week.FirstDay.Year)
                            || (DateTime.Now.Month == 2 && !IsPeriodActive(currentWeek, week, periodType))
                            )
                            throw new TNS.AdExpress.Domain.Exceptions.NoDataException(GestionWeb.GetWebWord(1612, webSession.SiteLanguage));
                        else {
                            PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
                            PeriodEndDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
                        }
                    }
                }
                else {
                    PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
                    PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
                }
            }
        }
        #endregion
        #region Période chargement des données sauvegardées
        /// <summary>
        /// Dates de chargement des données sauvegardées
        /// </summary>
        /// <param name="webSessionSave">session du client sauvegardée</param>
        /// <param name="PeriodBeginningDate">date de début </param>
        /// <param name="PeriodEndDate">date de fin</param>
        public static void WebSessionSaveDownloadDates(WebSession webSessionSave, ref string PeriodBeginningDate, ref string PeriodEndDate)
        {
            //Patch Finland pour le Tableau de bord PRESSE
            bool finland = false;
            VehicleInformation _vehicleInformation = null;
            DateTime downloadDate = DateTime.Now;
            if (WebApplicationParameters.CountryCode.Equals("35") && Modules.IsDashBoardModule(webSessionSave))
            {

                long vehicleId = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
                _vehicleInformation = VehiclesInformation.Get(vehicleId);
                if (TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList.ContainsKey(_vehicleInformation.Id))
                {
                    downloadDate = TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList[_vehicleInformation.Id];
                }
                finland = true;
            }

            switch (webSessionSave.PeriodType)
            {
                case CstCustomerSession.Period.Type.LastLoadedWeek:
                    LastLoadedWeek(ref PeriodBeginningDate, ref PeriodEndDate);
                    break;
                case CstCustomerSession.Period.Type.LastLoadedMonth:
                    if (finland)
                    {
                        //Patch Finland pour le Tableau de bord PRESSE
                        PeriodEndDate = PeriodBeginningDate = String.Format("{0:yyyyMM}", downloadDate);
                    }
                    else
                        LastLoadedMonth(ref PeriodBeginningDate, ref PeriodEndDate, webSessionSave.PeriodType);
                    break;
                case CstCustomerSession.Period.Type.currentYear:
                    if (finland)
                    {
                        //Patch Finland pour le Tableau de bord PRESSE
                        PeriodBeginningDate = String.Format("{0:yyyy01}", downloadDate);
                        PeriodEndDate = String.Format("{0:yyyyMM}", downloadDate);
                    }
                    else
                        DownloadDates(webSessionSave, ref PeriodBeginningDate, ref PeriodEndDate, webSessionSave.PeriodType);
                    break;
                case CstCustomerSession.Period.Type.previousYear:
                    if (finland)
                    {
                        //Patch Finland pour le Tableau de bord PRESSE
                        int yearN1 = downloadDate.Year - 1;
                        PeriodBeginningDate = yearN1.ToString() + "01";
                        PeriodEndDate = yearN1.ToString() + "12";
                    }
                    else
                        DownloadDates(webSessionSave, ref PeriodBeginningDate, ref PeriodEndDate, webSessionSave.PeriodType);
                    break;
                case CstCustomerSession.Period.Type.nLastYear:
                    PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
                    PeriodEndDate = DateTime.Now.ToString("yyyyMM");
                    break;
                case CstCustomerSession.Period.Type.dateToDateMonth:
                    PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                    PeriodEndDate = webSessionSave.PeriodEndDate;
                    break;
                case CstCustomerSession.Period.Type.dateToDateWeek:
                    PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                    PeriodEndDate = webSessionSave.PeriodEndDate;
                    break;
                case CstCustomerSession.Period.Type.nextToLastYear:
                    if (finland)
                    {
                        //Patch Finland pour le Tableau de bord PRESSE
                        int yearN2 = downloadDate.Year - 2;
                        PeriodBeginningDate = yearN2.ToString() + "01";
                        PeriodEndDate = yearN2.ToString() + "12";
                    }
                    else
                    {
                        PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy01");
                        PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy12");
                    }
                    break;
                case CstCustomerSession.Period.Type.dateToDate:
                    PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                    PeriodEndDate = webSessionSave.PeriodEndDate;
                    break;
            }
        }
        #endregion


    }
}
