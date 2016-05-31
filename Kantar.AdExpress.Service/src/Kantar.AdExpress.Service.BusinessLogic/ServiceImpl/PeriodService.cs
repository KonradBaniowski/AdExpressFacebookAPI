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
using TNS.AdExpress.Domain.Web.Navigation;
using System.Collections.Generic;

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
        public const string MEDIATYPESELECTIONERROR = "Selection of media type is not correct";
        #endregion
        WebSession _customerSession = null;
        public WebConstantes.globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
        public WebConstantes.globalCalendar.comparativePeriodType comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;



        public PeriodResponse CalendarValidation(PeriodSaveRequest request)
        {
            var result = new PeriodResponse();
            try
            {
                WebSession _customerSession = (WebSession)WebSession.Load(request.IdWebSession);
                DateTime lastDayEnable = DateTime.Now;
                result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, request.NextStep);
                switch (_customerSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.INDICATEUR:
                    case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                        AnalysisCalendarValidation(request,result, _customerSession);
                        break;
                    default:
                        DefaultCalendarValidation(request, result, _customerSession);
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

        private void DefaultCalendarValidation(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            DateTime startDate = new DateTime(Convert.ToInt32(request.StartDate.Substring(6, 4)), Convert.ToInt32(request.StartDate.Substring(3, 2)), Convert.ToInt32(request.StartDate.Substring(0, 2)));
            DateTime endDate = new DateTime(Convert.ToInt32(request.EndDate.Substring(6, 4)), Convert.ToInt32(request.EndDate.Substring(3, 2)), Convert.ToInt32(request.EndDate.Substring(0, 2)));
            webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
            webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            webSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            webSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
            if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
            {
                //TODO: Gerer last Complete Period
                if (endDate < DateTime.Now || DateTime.Now < startDate)
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                else
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
            }
            else
            {

                if (endDate < DateTime.Now || DateTime.Now < startDate)
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
                else
                    webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
            }

            webSession.Save();

            result.Success = true;
        }
        public PeriodResponse GetPeriod(string idWebSession)
        {
            var result = new PeriodResponse();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                switch (_customerSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.INDICATEUR:
                    case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                        //GetAnalysisPeriod(_customerSession, result);
                        GetDefaultPeriod(_customerSession, result);
                        break;
                    default:
                        GetDefaultPeriod(_customerSession, result);
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de recupérer la date de début du calendrier";//TODO : a mettre dans ressources
            }
            return result;
        }

        public PeriodResponse SlidingDateValidation(PeriodSaveRequest request)
        {
            var result = new PeriodResponse();
            _customerSession = (WebSession)WebSession.Load(request.IdWebSession);

            result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, request.NextStep);
            try
            {
                globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                globalCalendar.comparativePeriodType comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
                IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

                if (request.SelectedValue == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = GestionWeb.GetWebWord(885, _customerSession.SiteLanguage);
                    return result;
                }
                date.SetDate(ref _customerSession, DateTime.Now, periodCalendarDisponibilityType, comparativePeriodCalendarType, request.SelectedPeriod, request.SelectedValue);

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
        private void AnalysisSlidingValidation(string selectedStartDate, string selectedEndDate, PeriodResponse result, WebSession _webSession, string nextStep, bool isComparativeStudy)
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
        private void GetDefaultPeriod(WebSession webSesion, PeriodResponse result)
        {
            result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule);
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _customerSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            int startYear = dateDAL.GetCalendarStartDate();
            int endYear = DateTime.Now.Year;
            result.StartYear = startYear;
            result.EndYear = endYear;
            result.SiteLanguage = _customerSession.SiteLanguage;
            result.Success = true;
        }
        private void GetAnalysisPeriod(WebSession webSession, PeriodResponse result)
        {
            int downloadDate = _customerSession.DownLoadDate;
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = webSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            bool isRestricted = ModulesList.GetModule(webSession.CurrentModule).DisplayIncompleteDateInCalendar;
            List<Int64> selectedVehicleList = new List<Int64>();
            if (isRestricted)
            {
                string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (vehicleSelection == null)
                    result.ErrorMessage = MEDIATYPESELECTIONERROR;
                selectedVehicleList = new List<Int64>((new List<string>(vehicleSelection.Split(','))).ConvertAll<Int64>(ConvertStringToInt64));
            }
            //TODO
        }
        private Int64 ConvertStringToInt64(string p)
        {
            return Int64.Parse(p);

        }
        private void AnalysisCalendarValidation(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
            {
                try
                {
                   Validate(request, result, webSession);
                   webSession.ComparativeStudy = IsComparativeStudy(request.StudyId)? true :false;
                   webSession.Save();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
                {
                    webSession.ComparativeStudy = false;
                    result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery,webSession.SiteLanguage);
                }
                catch (Exception ex )
                {
                    webSession.ComparativeStudy = false;
                    result.ErrorMessage = ex.Message;
                }
            }
            else
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.SelectPeriodErrorMsg, webSession.SiteLanguage);
            }
        }
        private PeriodResponse Validate (PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            int startYear = Convert.ToInt32(request.StartDate.Substring(3, 4));
            int endYear = Convert.ToInt32(request.EndDate.Substring(3, 4));
            result.SiteLanguage = webSession.SiteLanguage;
            if (startYear == endYear)
            {
                int starMonth = Convert.ToInt32(request.StartDate.Substring(0, 2));
                int endMonth = Convert.ToInt32(request.EndDate.Substring(0, 2));
                DateTime startDate = new DateTime(startYear, starMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                if (startDate > endDate)
                {
                    result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.EndDateAfterBeginDate, webSession.SiteLanguage);
                    return result;
                }

                if (DateTime.Now.Year > webSession.DownLoadDate)
                {
                    if (IsComparativeStudy(request.StudyId) && ((startYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear || endYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear)))
                    {
                        result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.ComparativeStudyUnvailableForCurrentYear, webSession.SiteLanguage);
                        return result;
                    }
                    
                }
                else
                {
                    if (IsComparativeStudy(request.StudyId) && ((startYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1) || endYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1))))
                    {
                        result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.ComparativeStudyUnvailableForCurrentYear, webSession.SiteLanguage);
                        return result;
                    }
                }
                //webSession.PeriodType = monthCalendarBeginWebControl.SelectedDateType; //TODO
                try
                {
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                }
                catch (Exception ex)
                {
                    webSession.ComparativeStudy = false;
                    result.ErrorMessage = ex.Message;
                }
                webSession.PeriodBeginningDate = string.Format("{0}{1}", startYear, starMonth.ToString("00"));//request.StartDate;//monthCalendarBeginWebControl.SelectedDate.ToString();
                webSession.PeriodEndDate = string.Format("{0}{1}", endYear, endMonth.ToString("00"));//request.EndDate;//monthCalendarEndWebControl.SelectedDate.ToString();

                // On sauvegarde les données
                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
                //du dernier mois dispo en BDD
                //traitement de la notion de fréquence	
                if (endYear == DateTime.Now.Year && startYear == DateTime.Now.Year)
                {

                    CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                    object[] param = new object[1];
                    param[0] = webSession;
                    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                    string absolutEndPeriod = dateDAL.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
                    if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
                    {
                        result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);
                        return result;
                    }
                    else
                    {
                        result.StartYear = startYear;
                        result.EndYear = endYear;
                        result.Success = true;                        
                        if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodEndDate))
                        {
                            webSession.PeriodEndDate = absolutEndPeriod;
                        }
                    }
                }
                else
                {
                    result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.AnalysisPeriodErrorMsg, webSession.SiteLanguage);
                    return result;
                }                
            }
            else
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.AnalysisPeriodErrorMsg, webSession.SiteLanguage);
                return result;
            }
            return result;
        }
    }
}