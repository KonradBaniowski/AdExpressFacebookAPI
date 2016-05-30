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
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;

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
                        AnalysisValidation(selectedStartDate, selectedEndDate, result, _customerSession, nextStep,true);
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
        private ControllerDetails GetCurrentControllerDetails(long currentModule, string nextStep = "")
        {
            long currentModuleCode = 0;
            string currentController = string.Empty;
            string currentModuleIcon = "icon-chart";
            switch (currentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentModuleCode = WebConstantes.LanguageConstantes.MediaScheduleCode;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? MEDIASCHEDULE : SELECTION;
                    currentModuleIcon = "icon-chart";
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = WebConstantes.LanguageConstantes.PortfolioCode;
                    currentController = "Portfolio";
                    currentModuleIcon = "icon-layers";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = WebConstantes.LanguageConstantes.LostWonCode;
                    currentController = "LostWon";
                    currentModuleIcon = "icon-calculator";
                    break;
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = WebConstantes.LanguageConstantes.PresentAbsentCode;
                    currentController = "PresentAbsent";
                    currentModuleIcon = "icon-equalizer";
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalysisGraphics;
                    currentController = "Selection";
                    break;
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalysisDetailedReport;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSIS : SELECTION;
                    break;
                default:
                    break;
            }
            var current = new ControllerDetails
            {
                ModuleCode = currentModuleCode,
                Name = currentController,
                ModuleId = currentModule,
                ModuleIcon = currentModuleIcon
            };
            return current;
        }
        private void AnalysisValidation(string selectedStartDate, string selectedEndDate, PeriodResponse result, WebSession _webSession, string nextStep, bool isComparativeStudy)
        {
            #region TODO
            //if (Request.Form.GetValues("selectedItemIndex") != null) selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
            //if (Request.Form.GetValues("selectedComparativeStudy") != null) selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);

            //try
            //{
            //    CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            //    object[] param = new object[1];
            //    param[0] = _webSession;
            //    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

            //    DateTime downloadDate = new DateTime(_webSession.DownLoadDate, 12, 31);
            //    string absolutEndPeriod = "";
            //    switch (selectedIndex)
            //    {
            //        //Choix N derniers mois
            //        case 1:
            //            if (int.Parse(monthDateList.SelectedValue) != 0)
            //            {
            //                _webSession.PeriodType = CstPeriodType.nLastMonth;
            //                _webSession.PeriodLength = int.Parse(monthDateList.SelectedValue);

            //                // Cas où l'année de chargement des données est inférieur à l'année actuelle
            //                if (DateTime.Now.Year > _webSession.DownLoadDate)
            //                {
            //                    _webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
            //                    _webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");

            //                }
            //                else {
            //                    _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
            //                    _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
            //                }

            //                absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
            //                if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
            //                    //throw (new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787, _webSession.SiteLanguage)));

            //                    if (int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodEndDate))
            //                        _webSession.PeriodEndDate = absolutEndPeriod;

            //                _webSession.DetailPeriod = CstPeriodDetail.dayly;
            //                //Activation de l'option etude comparative si selectionné
            //                if (IsComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
            //                else _webSession.ComparativeStudy = false;
            //                _webSession.Save();
            //            }
            //            else
            //            {
            //                //throw (new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            //            }
            //            break;
            //        //Choix année courante
            //        case 2:
            //            _webSession.PeriodType = CstPeriodType.currentYear;
            //            _webSession.PeriodLength = 1;
            //            // Cas où l'année de chargement est inférieur à l'année en cours
            //            if (DateTime.Now.Year > _webSession.DownLoadDate)
            //            {
            //                _webSession.PeriodBeginningDate = downloadDate.ToString("yyyy01");
            //                _webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
            //            }
            //            else {
            //                _webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
            //                _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
            //            }

            //            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
            //            //du dernier mois dispo en BDD
            //            //traitement de la notion de fréquence
            //            absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
            //            if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
            //                //throw (new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787, _webSession.SiteLanguage)));

            //                _webSession.PeriodEndDate = absolutEndPeriod;

            //            _webSession.DetailPeriod = CstPeriodDetail.monthly;
            //            //Activation de l'option etude comparative si selectionné
            //            if (IsComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
            //            else _webSession.ComparativeStudy = false;
            //            _webSession.Save();
            //            break;

            //        //Choix année précedente
            //        case 3:
            //            _webSession.PeriodType = CstPeriodType.previousYear;
            //            _webSession.PeriodLength = 1;

            //            // Cas où l'année de chargement est inférieur à l'année en cours
            //            if (DateTime.Now.Year > _webSession.DownLoadDate)
            //            {
            //                _webSession.PeriodBeginningDate = downloadDate.AddYears(-1).ToString("yyyy01");
            //                _webSession.PeriodEndDate = downloadDate.AddYears(-1).ToString("yyyy12");
            //            }
            //            else {
            //                _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
            //                _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
            //            }
            //            _webSession.DetailPeriod = CstPeriodDetail.monthly;
            //            //Activation de l'option etude comparative si selectionné
            //            if (IsComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
            //            else _webSession.ComparativeStudy = false;
            //            _webSession.Save();

            //            break;

            //        //Choix année N-2
            //        case 4:
            //            if (IsComparativeStudy(selectedComparativeStudy) && (WebApplicationParameters.DataNumberOfYear <= 3)) throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            //            _webSession.PeriodType = CstPeriodType.nextToLastYear;
            //            _webSession.PeriodLength = 1;
            //            // Cas où l'année de chargement est inférieur à l'année en cours
            //            if (DateTime.Now.Year > _webSession.DownLoadDate)
            //            {
            //                _webSession.PeriodBeginningDate = downloadDate.AddYears(-2).ToString("yyyy01");
            //                _webSession.PeriodEndDate = downloadDate.AddYears(-2).ToString("yyyy12");
            //            }
            //            else {
            //                _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy01");
            //                _webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy12");
            //            }
            //            _webSession.DetailPeriod = CstPeriodDetail.monthly;
            //            if (IsComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
            //            else
            //                _webSession.ComparativeStudy = false;
            //            _webSession.Save();
            //            break;
            //        default:
            //            break;
            //            //throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            //    }
            //    _webSession.Source.Close();
            //    //Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            //}
            //catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
            //{
            //    //Response.Write("<script language=\"JavaScript\">alert(\"" + GestionWeb.GetWebWord(1787, _webSession.SiteLanguage) + "\");</script>");
            //}
            //catch (System.Exception ex)
            //{
            //    //Response.Write("<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>");
            //}
            //int startYear = Convert.ToInt32(selectedStartDate.Substring(3, 4));
            //int endYear = Convert.ToInt32(selectedEndDate.Substring(3, 4));
            //{
            //    DateTime startDate = new DateTime(startYear, Convert.ToInt32(selectedStartDate.Substring(0, 2)), 1);
            //    int month = Convert.ToInt32(selectedEndDate.Substring(0, 2));
            //    DateTime endDate = new DateTime(endYear, month, DateTime.DaysInMonth(endYear, month));
            //    result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, nextStep);

            //    _customerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly;
            //    _customerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            //    _customerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            //    _customerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
            //    if (_customerSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)
            //    {
            //        //TODO: Gerer last Complete Period
            //        if (endDate < DateTime.Now || DateTime.Now < startDate)
            //            _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
            //        else
            //            _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
            //    }
            //    else
            //    {

            //        if (endDate < DateTime.Now || DateTime.Now < startDate)
            //            _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, _customerSession.PeriodEndDate);
            //        else
            //            _customerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_customerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
            //    }

            //    _customerSession.Save();

            //    result.Success = true;
            //}
            //else
            //{
            //    result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.AnalysisPeriodErrorMsg, _customerSession.SiteLanguage);
            //}
            #endregion
        }
        private bool IsComparativeStudy(Int64 idStudy)
        {
            switch (idStudy)
            {
                case 5:
                case 8:
                    return (true);
                default:
                    return (false);
            }
        }
    }
 
}
