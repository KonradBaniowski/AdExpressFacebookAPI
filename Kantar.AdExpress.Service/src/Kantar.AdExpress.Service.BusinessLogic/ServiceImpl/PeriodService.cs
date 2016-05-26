using Kantar.AdExpress.Service.Core;
using System;
using Kantar.AdExpress.Service.Core.Domain.BusinessService;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Date;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using Kantar.AdExpress.Service.Core.Domain;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PeriodService : IPeriodService
    {
        #region CONST
        private const string SELECTION = "Selection";
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string ANALYSIS = "Analysis";
        private const string RESULTS = "Results";
        #endregion
        WebSession _customerSession = null;
        public WebConstantes.globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
        public WebConstantes.globalCalendar.comparativePeriodType comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;

        public PeriodResponse CalendarValidation(string idWebSession, string selectedStartDate, string selectedEndDate, string nextStep)
        {
            var result = new PeriodResponse();
            try
            {
                WebSession _customerSession = (WebSession)WebSession.Load(idWebSession);
                DateTime lastDayEnable = DateTime.Now;

                switch (_customerSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.INDICATEUR:
                    case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                        AnalysisValidation(selectedStartDate, selectedEndDate, result, _customerSession, nextStep);
                        break;
                    default:
                        DefaultValidation(selectedStartDate, selectedEndDate, result, _customerSession, nextStep);
                        break;

                }
            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de sauvegarder les dates sélectionnées";//TODO : a mettre dans ressources
            }
            return result;

        }

        private void DefaultValidation(string selectedStartDate, string selectedEndDate, PeriodResponse result, WebSession _customerSession, string nextStep)
        {
            DateTime startDate = new DateTime(Convert.ToInt32(selectedStartDate.Substring(6, 4)), Convert.ToInt32(selectedStartDate.Substring(3, 2)), Convert.ToInt32(selectedStartDate.Substring(0, 2)));
            DateTime endDate = new DateTime(Convert.ToInt32(selectedEndDate.Substring(6, 4)), Convert.ToInt32(selectedEndDate.Substring(3, 2)), Convert.ToInt32(selectedEndDate.Substring(0, 2)));

            result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, nextStep);

            _customerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
            _customerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            _customerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            _customerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
            if (_customerSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
            {
                //TODO: Gerer last Complete Period
                if (endDate < DateTime.Now || DateTime.Now < startDate)
                    _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                else
                    _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
            }
            else
            {

                if (endDate < DateTime.Now || DateTime.Now < startDate)
                    _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate);
                else
                    _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
            }

            _customerSession.Save();

            result.Success = true;
        }

        private void AnalysisValidation(string selectedStartDate, string selectedEndDate, PeriodResponse result, WebSession _customerSession, string nextStep)
        {
            int startYear = Convert.ToInt32(selectedStartDate.Substring(3, 4));
            int endYear = Convert.ToInt32(selectedEndDate.Substring(3, 4));
            if (startYear == endYear)
            {
                DateTime startDate = new DateTime(startYear, Convert.ToInt32(selectedStartDate.Substring(0, 2)), 1);
                int month = Convert.ToInt32(selectedEndDate.Substring(0, 2));
                DateTime endDate = new DateTime(endYear, month, DateTime.DaysInMonth(endYear, month));
                result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, nextStep);

                _customerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly;
                _customerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
                _customerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
                _customerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
                if (_customerSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)
                {
                    //TODO: Gerer last Complete Period
                    if (endDate < DateTime.Now || DateTime.Now < startDate)
                        _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                    else
                        _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                }
                else
                {

                    if (endDate < DateTime.Now || DateTime.Now < startDate)
                        _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate);
                    else
                        _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                }

                _customerSession.Save();

                result.Success = true;
            }
            else
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.AnalysisPeriodErrorMsg,_customerSession.SiteLanguage);
            }
        }

        public PeriodResponse GetPeriod(string idWebSession)
        {
            var result = new PeriodResponse();
            try
            {
                _customerSession = (WebSession)WebSession.Load(idWebSession);
                //result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule);
                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _customerSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                int startYear = dateDAL.GetCalendarStartDate();
                int endYear = DateTime.Now.Year;                
                result = new PeriodResponse
                {
                    StartYear = startYear,
                    EndYear = endYear,
                    SiteLanguage = _customerSession.SiteLanguage,
                    Success = true,
                    ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule)
                };
            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de recupérer la date de début du calendrier";//TODO : a mettre dans ressources
            }
            return result;
        }

        public PeriodResponse SlidingDateValidation(string idWebSession, int selectedPeriod, int selectedValue, string nextStep="")
        {
            var result = new PeriodResponse();
            try
            {
                _customerSession = (WebSession)WebSession.Load(idWebSession);

                result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, nextStep);

                globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                globalCalendar.comparativePeriodType comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
                IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

                if (selectedValue == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = GestionWeb.GetWebWord(885, _customerSession.SiteLanguage);
                    return result;
                }
                date.SetDate(ref _customerSession, DateTime.Now, periodCalendarDisponibilityType, comparativePeriodCalendarType, selectedPeriod, selectedValue);

                _customerSession.Save();
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de sauvegarder les dates sélectionnées";//TODO : a mettre dans ressources
            }
            return result;
        }
        private ControllerDetails GetCurrentControllerDetails(long currentModule, string nextStep="")
        {
            long currentModuleCode = 0;
            string currentController = string.Empty;
            switch (currentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentModuleCode = LanguageConstantes.MediaScheduleCode;
                    currentController =  (!string.IsNullOrEmpty(nextStep)&& nextStep == RESULTS)?MEDIASCHEDULE:SELECTION;
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = LanguageConstantes.PortfolioCode;
                    currentController = PORTFOLIO;
                    break;
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = LanguageConstantes.LostWonCode;
                    currentController = LOSTWON;
                    break;
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = LanguageConstantes.PresentAbsentCode;
                    currentController = PRESENTABSENT;
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    currentModuleCode = LanguageConstantes.AnalysisGraphics;
                    currentController = SELECTION;
                    break;
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = LanguageConstantes.AnalysisDetailedReport;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSIS : SELECTION; ;
                    break;
                default:
                    break;
            }
            var current = new ControllerDetails
            {
                ModuleCode = currentModuleCode,
                Name = currentController,
                ModuleId = currentModule
            };
            return current;
        }
    }
 
}
