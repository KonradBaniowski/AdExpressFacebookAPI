#region Information
/*
 * Author : G. RAGNEAU
 * Creation : 22/08/2005
 * Modifications :
 *		D. V. Mussuma Liens tri des colonnes		
 * */
#endregion

using System;
using System.Globalization;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork;
using TNS.FrameWork.Date;
using FctUtilities=TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpress.Web.UI
{


	///<summary>
	/// Usefull tools for UI design
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class HtmlFunctions {

		/// <summary>
		/// Generate html code for period detail design
		/// </summary>
		/// <param name="webSession">User Session</param>
		/// <returns>Html code</returns>
		public static string GetPeriodDetail(WebSession webSession){
			try{
				string str = "";
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

				switch(webSession.PeriodType){
					case CustomerSessions.Period.Type.nLastMonth:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(783,webSession.SiteLanguage);					
					case CustomerSessions.Period.Type.nLastYear:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(781,webSession.SiteLanguage);					
					case CustomerSessions.Period.Type.previousMonth:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(788,webSession.SiteLanguage));
						// Année courante		
					case CustomerSessions.Period.Type.currentYear:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1228,webSession.SiteLanguage));
						// Année N-1
					case CustomerSessions.Period.Type.previousYear:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(787,webSession.SiteLanguage));
						// Année N-2
					case CustomerSessions.Period.Type.nextToLastYear:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1229,webSession.SiteLanguage));

					case CustomerSessions.Period.Type.dateToDateMonth:
						string monthBegin;
						string monthEnd;
						string yearBegin;
						string yearEnd;
						if(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2))<10){
							monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(5,1)),cultureInfo,10);
							yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0,4);
						}
						else{
							monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)),cultureInfo,10);
							yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0,4);
						}
						if(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2))<10){
							monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(5,1)),cultureInfo,10);
							yearEnd = webSession.PeriodEndDate.ToString().Substring(0,4);
						}
						else{
							monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)),cultureInfo,10);
							yearEnd = webSession.PeriodEndDate.ToString().Substring(0,4);
						}					
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(846,webSession.SiteLanguage)+" "+monthBegin+" "+yearBegin+" "+GestionWeb.GetWebWord(847,webSession.SiteLanguage)+" "+monthEnd+" "+yearEnd);
					case CustomerSessions.Period.Type.dateToDateWeek:
						AtomicPeriodWeek tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)));
						str = FctUtilities.Dates.DateToString(tmp.FirstDay.Date, webSession.SiteLanguage);  //tmp.FirstDay.Date.ToString("dd/MM/yyyy");
						tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)));
						str+=" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+"";
						str += " " + FctUtilities.Dates.DateToString(tmp.LastDay.Date, webSession.SiteLanguage) + "";// tmp.LastDay.Date.ToString("dd/MM/yyyy") + "";
						return str;
					case CustomerSessions.Period.Type.nLastWeek:
						return Convertion.ToHtmlString(webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(784,webSession.SiteLanguage));
					case CustomerSessions.Period.Type.previousWeek:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(789,webSession.SiteLanguage));
					case CustomerSessions.Period.Type.dateToDate:
                    case CustomerSessions.Period.Type.cumlDate:
                    case CustomerSessions.Period.Type.personalize:
						string dateBegin;
						string dateEnd;
						dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate.ToString(), webSession.SiteLanguage);
						dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate.ToString(), webSession.SiteLanguage);
						if(!dateBegin.Equals(dateEnd))
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(896,webSession.SiteLanguage)+" "+dateBegin+" "+GestionWeb.GetWebWord(897,webSession.SiteLanguage)+" "+dateEnd);
						else return " "+dateBegin;
					case CustomerSessions.Period.Type.previousDay:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1975,webSession.SiteLanguage));
					case CustomerSessions.Period.Type.nLastDays:
						return Convertion.ToHtmlString(webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(1974,webSession.SiteLanguage));
					case CustomerSessions.Period.Type.LastLoadedMonth:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1619,webSession.SiteLanguage));
					case CustomerSessions.Period.Type.LastLoadedWeek:
						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1618,webSession.SiteLanguage));
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
			catch(System.Exception e){
				throw(new FunctionsUIException("Unable to generate the html code for period detail",e));
			}
		}

        /// <summary>
		/// Generate html code for zoom period detail design
		/// </summary>
		/// <param name="webSession">User Session</param>
        /// <param name="zoomDate">Date de zoom</param>
		/// <returns>Html code</returns>
        public static string GetZoomPeriodDetail(WebSession webSession, string zoomDate) {

            try {
                string str = "";
                DateTime firstDayOfMonth;
                DateTime lastDayOfMonth;
                DateTime begin;
                DateTime end;

                switch (webSession.DetailPeriod) {
                    case CustomerSessions.Period.DisplayLevel.weekly:

                        AtomicPeriodWeek tmp=new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
                        begin = tmp.FirstDay.Date;
                        end = tmp.LastDay.Date;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        str += "" + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " ";
						str += FctUtilities.Dates.DateToString(begin.Date, webSession.SiteLanguage);// begin.Date.ToString("dd/MM/yyyy");
                        str += " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + "";
						str += " " + FctUtilities.Dates.DateToString(end.Date, webSession.SiteLanguage) + "";//end.Date.ToString("dd/MM/yyyy")
						return str;
                    case CustomerSessions.Period.DisplayLevel.monthly:
                        
                        firstDayOfMonth = new DateTime(int.Parse(zoomDate.Substring(0,4)), int.Parse(zoomDate.Substring(4,2)),1);
                        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
                        lastDayOfMonth = firstDayOfMonth.AddDays(-1);

                        begin = new DateTime(lastDayOfMonth.Year, lastDayOfMonth.Month, 1);
                        end = lastDayOfMonth;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + FctUtilities.Dates.DateToString(begin,webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + FctUtilities.Dates.DateToString(end,webSession.SiteLanguage));
                    default:
                        return "";
                }
            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for zoom period detail", e));
            }

        }

        /// <summary>
        /// Generate html code for zoom comparative period detail design
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <param name="zoomDate">Date de zoom</param>
        /// <returns>Html code</returns>
        public static string GetZoomComparativePeriodDetail(WebSession webSession, string zoomDate) {

            try {
                string str = "";
                DateTime firstDayOfMonth;
                DateTime lastDayOfMonth;
                DateTime begin;
                DateTime end;

                switch (webSession.DetailPeriod) {
                    case CustomerSessions.Period.DisplayLevel.weekly:

                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        begin = tmp.FirstDay.Date;
                        end = tmp.LastDay.Date;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(begin.Date, webSession.ComparativePeriodType);
                        end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(end.Date, webSession.ComparativePeriodType);

                        str += "" + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " ";
                        str += FctUtilities.Dates.DateToString(begin.Date, webSession.SiteLanguage);// begin.Date.ToString("dd/MM/yyyy");
                        str += " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + "";
                        str += " " + FctUtilities.Dates.DateToString(end.Date, webSession.SiteLanguage) + "";//end.Date.ToString("dd/MM/yyyy")
                        return str;
                    case CustomerSessions.Period.DisplayLevel.monthly:

                        firstDayOfMonth = new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1);
                        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
                        lastDayOfMonth = firstDayOfMonth.AddDays(-1);

                        begin = new DateTime(lastDayOfMonth.Year, lastDayOfMonth.Month, 1);
                        end = lastDayOfMonth;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(begin.Date, webSession.ComparativePeriodType);
                        end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(end.Date, webSession.ComparativePeriodType);

                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + FctUtilities.Dates.DateToString(begin, webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + FctUtilities.Dates.DateToString(end, webSession.SiteLanguage));
                    default:
                        return "";
                }
            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for zoom period detail", e));
            }
        }

        /// <summary>
		/// Generate html code for study period detail design
		/// </summary>
        /// <param name="webSession">User Session</param>
        /// <param name="moduleId">Module Id</param>
		/// <returns>Html code</returns>
        public static string GetStudyPeriodDetail(WebSession webSession, long moduleId) {
            try {
                string dateBegin;
                string dateEnd;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate.ToString(), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate.ToString(), webSession.SiteLanguage);
                }
                else {
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.StartDate.ToString(), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.EndDate.ToString(), webSession.SiteLanguage);
                }
                
                if (!dateBegin.Equals(dateEnd))
                    return Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + dateBegin + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + dateEnd);
                else return " " + dateBegin;
            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for study period detail", e));
            }
        }

        /// <summary>
        /// Generate html code for comparative period detail design
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <param name="moduleId">Module Id</param>
        /// <returns>Html code</returns>
        public static string GetComparativePeriodDetail(WebSession webSession, long moduleId) {
            try {
                string dateBegin;
                string dateEnd;
                DateTime dateBeginDT;
                DateTime dateEndDT;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                    // get date begin and date end according to period type
                    dateBeginDT = Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                    dateEndDT = Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
                    
                    // get comparative date begin and date end
                    dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, webSession.ComparativePeriodType);
                    dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, webSession.ComparativePeriodType);
                    
                    // Formating date begin and date end
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(dateBeginDT.ToString("yyyyMMdd"), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(dateEndDT.ToString("yyyyMMdd"), webSession.SiteLanguage);
                }
                else {
                    dateBegin = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
                    dateEnd = FctUtilities.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);
                }

                if (!dateBegin.Equals(dateEnd))
                    return Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + dateBegin + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + dateEnd);
                else return " " + dateBegin;

            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for comparative period type detail", e));
            }
        }

        /// <summary>
        /// Generate html code for comparative period detail design
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <param name="moduleId">Module Id</param>
        /// <returns>Html code</returns>
        public static string GetComparativePeriodTypeDetail(WebSession webSession, long moduleId) {
            try {
                globalCalendar.comparativePeriodType comparativePeriodType;

                if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    comparativePeriodType = webSession.ComparativePeriodType;
                else
                    comparativePeriodType = webSession.CustomerPeriodSelected.ComparativePeriodType;

                switch (comparativePeriodType) {
                    case globalCalendar.comparativePeriodType.comparativeWeekDate:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(2295, webSession.SiteLanguage));
                    case globalCalendar.comparativePeriodType.dateToDate:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(2294, webSession.SiteLanguage));
                    default:
                        return "";
                }
            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for study period detail", e));
            }
        }

        /// <summary>
        /// Generate html code for period disponibility type detail design
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        public static string GetPeriodDisponibilityTypeDetail(WebSession webSession) {
            try {
                switch (webSession.CustomerPeriodSelected.PeriodDisponibilityType) {
                    case globalCalendar.periodDisponibilityType.currentDay:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(2297, webSession.SiteLanguage));
                    case globalCalendar.periodDisponibilityType.lastCompletePeriod:
                        return Convertion.ToHtmlString(GestionWeb.GetWebWord(2298, webSession.SiteLanguage));
                    default:
                        return "";
                }
            }
            catch (System.Exception e) {
                throw (new FunctionsUIException("Unable to generate the html code for period disponibility type detail", e));
            }
        }

		/// <summary>
		/// Generate Excel code for period detail design
		/// </summary>
		/// <param name="webSession">User Session</param>
		/// <returns>Excel code</returns>
		public static string GetPeriodDetailForExcel(WebSession webSession){
			try{
				string str = "";
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

				switch(webSession.PeriodType){
					case CustomerSessions.Period.Type.nLastMonth:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(783,webSession.SiteLanguage);					
					case CustomerSessions.Period.Type.nLastYear:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(781,webSession.SiteLanguage);					
					case CustomerSessions.Period.Type.previousMonth:
						return GestionWeb.GetWebWord(788,webSession.SiteLanguage);
						// Année courante		
					case  CustomerSessions.Period.Type.currentYear:
						return GestionWeb.GetWebWord(1228,webSession.SiteLanguage);
						// Année N-1
					case CustomerSessions.Period.Type.previousYear:
						return GestionWeb.GetWebWord(787,webSession.SiteLanguage);
						// Année N-2
					case CustomerSessions.Period.Type.nextToLastYear:
						return GestionWeb.GetWebWord(1229,webSession.SiteLanguage);

					case CustomerSessions.Period.Type.dateToDateMonth:
						string monthBegin;
						string monthEnd;
						string yearBegin;
						string yearEnd;
						if(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2))<10){
							monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(5,1)),cultureInfo,10);
							yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0,4);
						}
						else{
							monthBegin = MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)),cultureInfo,10);
							yearBegin = webSession.PeriodBeginningDate.ToString().Substring(0,4);
						}
						if(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2))<10){
							monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(5,1)),cultureInfo,10);
							yearEnd = webSession.PeriodEndDate.ToString().Substring(0,4);
						}
						else{
							monthEnd = MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)),cultureInfo,10);
							yearEnd = webSession.PeriodEndDate.ToString().Substring(0,4);
						}					
						return GestionWeb.GetWebWord(846,webSession.SiteLanguage)+" "+monthBegin+" "+yearBegin+" "+GestionWeb.GetWebWord(847,webSession.SiteLanguage)+" "+monthEnd+" "+yearEnd;
					case CustomerSessions.Period.Type.dateToDateWeek:
						AtomicPeriodWeek tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)));
						str=tmp.FirstDay.Date.ToString("dd/MM/yyyy");
						tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)));
						str+=" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+"";
						str+=" "+tmp.LastDay.Date.ToString("dd/MM/yyyy")+"";
						return str;
					case CustomerSessions.Period.Type.nLastWeek:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(784,webSession.SiteLanguage);
					case CustomerSessions.Period.Type.previousWeek:
						return GestionWeb.GetWebWord(789,webSession.SiteLanguage);
					case CustomerSessions.Period.Type.dateToDate:
                    case CustomerSessions.Period.Type.personalize:
						string dateBegin;
						string dateEnd;
						dateBegin = DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate.ToString(),webSession.SiteLanguage);
						dateEnd = DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate.ToString(),webSession.SiteLanguage);
						return GestionWeb.GetWebWord(896,webSession.SiteLanguage)+" "+dateBegin+" "+GestionWeb.GetWebWord(897,webSession.SiteLanguage)+" "+dateEnd;
					case CustomerSessions.Period.Type.LastLoadedMonth:
						return GestionWeb.GetWebWord(1619,webSession.SiteLanguage);
					case CustomerSessions.Period.Type.LastLoadedWeek:
						return GestionWeb.GetWebWord(1618,webSession.SiteLanguage);
					case CustomerSessions.Period.Type.nLastDays:
						return webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(1974,webSession.SiteLanguage);
					case CustomerSessions.Period.Type.previousDay:
						return GestionWeb.GetWebWord(1975,webSession.SiteLanguage);
                    case CustomerSessions.Period.Type.cumulWithNextMonth:
                    case CustomerSessions.Period.Type.allHistoric:
                    case CustomerSessions.Period.Type.currentMonth:
                        foreach (DateConfiguration cVpDateConfiguration in WebApplicationParameters.VpDateConfigurations.VpDateConfigurationList)
                        {
                            if(cVpDateConfiguration.DateType==webSession.PeriodType)
                                return GestionWeb.GetWebWord(cVpDateConfiguration.TextId, webSession.SiteLanguage);
                        }
                        return "";
					default:
						return "";
				}
			}
			catch(System.Exception e){
				throw(new FunctionsUIException("Unable to generate the excel code for period detail",e));
			}
		}


		#region Lien choix colonne à trier
		/// <summary>
		/// Génère les liens à cliquer pour trier une colonne 
		/// </summary>
		/// <param name="indexCurrentColumn">index colonne en cours de création</param>
		/// <param name="indexColumnToSort">index colonne à trier</param>		
		/// <param name="ID">Identifiant de l'élément qui lance l'évènement de tri</param>
		/// <param name="classe"> style css des  liens</param>
		/// <returns>Code HTML des leins</returns>
		public static string GenerateSortLink(int indexColumnToSort,int indexCurrentColumn,string ID,string classe){
			string sortLink="";
			string imageSortAsc="/Images/Common/fl_tri_croi1.gif";
			string imageSortDesc="/Images/Common/fl_tri_decroi1.gif";

			//Lien tri croissant
			if(indexCurrentColumn==indexColumnToSort)
				imageSortAsc="/Images/Common/fl_tri_croi1_in.gif";
			
			sortLink+="&nbsp;<a "+classe+" href=\"javascript:__doPostBack('"+ID+"','"+indexCurrentColumn.ToString()+","+CustomerSessions.SortOrder.CROISSANT.ToString()+"')\">"
				+"<img src="+imageSortAsc+" border=0></a>";
			
			//Lien tri décroissant
			if(indexCurrentColumn==indexColumnToSort)
				imageSortDesc="/Images/Common/fl_tri_decroi1_in.gif";
			
			sortLink+="&nbsp;<a "+classe+" href=\"javascript:__doPostBack('"+ID+"','"+indexCurrentColumn.ToString()+","+CustomerSessions.SortOrder.DECROISSANT.ToString()+"')\">"
				+"<img src="+imageSortDesc+" border=0></a>";

			return sortLink;
			
		}
		#endregion

	}
}
