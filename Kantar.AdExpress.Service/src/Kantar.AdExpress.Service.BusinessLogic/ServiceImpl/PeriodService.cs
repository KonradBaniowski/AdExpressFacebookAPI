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
using NLog;

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
        private const string FACEBOOK = "SocialMedia";
        private const string RESULTS = "Results";
        private const string ADVERTISING_AGENCY = "AdvertisingAgency";
        private const string NEW_CREATIVES = "NewCreatives";
        private const string ANALYSE_DES_DISPOSITIFS = "CampaignAnalysis";
        public const string MEDIATYPESELECTIONERROR = "Selection of media type is not correct";
        public const string YYYYMM = "yyyyMM";
        public const string YYYY01 = "yyyy01";
        public const string YYYY12 = "yyyy12";
        #endregion
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        WebSession _customerSession = null;
        public WebConstantes.globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
        public WebConstantes.globalCalendar.comparativePeriodType comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;



        public PeriodResponse CalendarValidation(PeriodSaveRequest request)
        {

            comparativePeriodCalendarType = request.ComparativePeriodType;

            var result = new PeriodResponse();
            try
            {
                WebSession _customerSession = (WebSession)WebSession.Load(request.IdWebSession);
                DateTime lastDayEnable = DateTime.Now;
                result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, request.NextStep);
                result.SiteLanguage = _customerSession.SiteLanguage;
                switch (_customerSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.INDICATEUR:
                    case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                        AnalysisCalendarValidation(request, result, _customerSession);
                        break;
                    case WebConstantes.Module.Name.FACEBOOK:
                        SocialMediaCalendarValidation(request, result, _customerSession);
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
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
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
            //ClearPeriod(_customerSession);
            try
            {
                switch (_customerSession.CurrentModule)
                {
                    //case WebConstantes.Module.Name.INDICATEUR:
                    //case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    //    GetAnalysisPeriod(_customerSession, result);
                    //    break;
                    default:
                        GetDefaultPeriod(_customerSession, result);
                        break;
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace);
                Logger.Log(LogLevel.Error, message);
                result.Success = false;
                result.ErrorMessage = message; //"Une erreur est survenue. Impossible de recupérer la date de début du calendrier";//TODO : a mettre dans ressources
                throw;
            }
            return result;
        }

        public PeriodResponse SlidingDateValidation(PeriodSaveRequest request)
        {
            var result = new PeriodResponse();
            _customerSession = (WebSession)WebSession.Load(request.IdWebSession);
            try
            {
                result.ControllerDetails = GetCurrentControllerDetails(_customerSession.CurrentModule, request.NextStep);
                result.SiteLanguage = _customerSession.SiteLanguage;
                switch (_customerSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.INDICATEUR:
                    case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                        AnalysisSlidingValidation(request, result, _customerSession);
                        break;
                    case WebConstantes.Module.Name.FACEBOOK:
                        SocialMediaSlidingValidation(request, result, _customerSession);
                        break;
                    default:
                        DefaultSlidingPeriodValidation(request, result, _customerSession);
                        break;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
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
                    currentModuleCode = LanguageConstantes.MediaScheduleCode;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? MEDIASCHEDULE : SELECTION;
                    currentModuleIcon = "icon-chart";
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = LanguageConstantes.PortfolioCode;
                    currentController = PORTFOLIO;
                    currentModuleIcon = "icon-layers";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = LanguageConstantes.LostWonCode;
                    currentController = LOSTWON;
                    currentModuleIcon = "icon-calculator";
                    break;
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = LanguageConstantes.PresentAbsentCode;
                    currentController = PRESENTABSENT;
                    currentModuleIcon = "icon-equalizer";
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    currentModuleCode = LanguageConstantes.AnalysisGraphics;
                    currentController = SELECTION;
                    break;
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = LanguageConstantes.AnalysisDetailedReport;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSIS : SELECTION;
                    currentModuleIcon = "icon-book-open";
                    break;
                case WebConstantes.Module.Name.FACEBOOK:
                    currentModuleCode = LanguageConstantes.FacebookCode;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? FACEBOOK : SELECTION;
                    currentModuleIcon = "icon-social-facebook";
                    break;
                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                    currentModuleCode = LanguageConstantes.MediaAgencyAnalysis;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ADVERTISING_AGENCY : SELECTION;
                    currentModuleIcon = "icon-picture";
                    break;
                case WebConstantes.Module.Name.NEW_CREATIVES:
                    currentModuleCode = LanguageConstantes.NewCreatives;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? NEW_CREATIVES : SELECTION;
                    currentModuleIcon = "icon-camrecorder";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    currentModuleCode = LanguageConstantes.AnalyseDispositifsLabel;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSE_DES_DISPOSITIFS : SELECTION;
                    currentModuleIcon = "icon-puzzle";
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
        #region Private Methods for Analysis Calendar Period Validation
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
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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

        private void SocialMediaCalendarValidation(PeriodSaveRequest request, PeriodResponse result, WebSession _customerSession)
        {

            if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
            {

                #region Set Datetime variables
                int startYear = Convert.ToInt32(request.StartDate.Substring(3, 4));
                int endYear = Convert.ToInt32(request.EndDate.Substring(3, 4));
                int starMonth = Convert.ToInt32(request.StartDate.Substring(0, 2));
                int endMonth = Convert.ToInt32(request.EndDate.Substring(0, 2));
                DateTime startDate = new DateTime(startYear, starMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                #endregion

                #region Try-Catch block
                try
                {
                    SaveSocialMediaCalendarData(_customerSession, result, startDate, endDate);
                    _customerSession.Save();
                }
                catch (Exception ex)
                {
                    _customerSession.ComparativeStudy = false;
                    result.ErrorMessage = ex.Message;
                }
                #endregion

                try
                {
                    _customerSession.Save();
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                }
            }
            else
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.SelectPeriodErrorMsg, _customerSession.SiteLanguage);
            }
        }


        private void AnalysisCalendarValidation(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
            {
                try
                {
                    ValidateCalendar(request, result, webSession);
                    webSession.ComparativeStudy = IsComparativeStudy(request.StudyId) ? true : false;
                    webSession.Save();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
                {
                    webSession.ComparativeStudy = false;
                    result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);
                }
                catch (Exception ex)
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
        private PeriodResponse ValidateCalendar(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            #region Set Datetime variables
            int startYear = Convert.ToInt32(request.StartDate.Substring(3, 4));
            int endYear = Convert.ToInt32(request.EndDate.Substring(3, 4));
            int starMonth = Convert.ToInt32(request.StartDate.Substring(0, 2));
            int endMonth = Convert.ToInt32(request.EndDate.Substring(0, 2));
            DateTime startDate = new DateTime(startYear, starMonth, 1);
            DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            #endregion
            #region EndDate must be greater that StartDate
            if (startDate > endDate)
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.EndDateAfterBeginDate, webSession.SiteLanguage);
                return result;
            }
            #endregion
            #region Study is only possible  if StartDate and EndDate are in the same year
            if (startYear == endYear)
            {
                #region Try-Catch block
                try
                {
                    SaveCalendarData(request.StudyId, webSession, result, startDate, endDate);
                }
                catch (Exception ex)
                {
                    webSession.ComparativeStudy = false;
                    result.ErrorMessage = ex.Message;
                    return result;
                }
                #endregion
            }
            else
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.AnalysisPeriodErrorMsg, webSession.SiteLanguage);
                return result;
            }
            return result;
            #endregion
        }
        private PeriodResponse VerifyComparativeStudy(bool isComparativeStudy, int startYear, int endYear, PeriodResponse response, WebSession webSession, int year = 0)
        {
            if (isComparativeStudy && ((startYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear + year || endYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear + year)))
            {
                response.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.ComparativeStudyUnvailableForCurrentYear, webSession.SiteLanguage);
                return response;
            }
            return response;
        }
        private PeriodResponse SaveCalendarData(int studyId, WebSession webSession, PeriodResponse result, DateTime startDate, DateTime endDate)
        {
            bool isComparativeStudy = IsComparativeStudy(studyId);
            int year = (DateTime.Now.Year > webSession.DownLoadDate) ? 0 : 1;

            VerifyComparativeStudy(isComparativeStudy, startDate.Year, endDate.Year, result, webSession, year);
            webSession.PeriodType = CstPeriodType.dateToDateMonth;
            webSession.DetailPeriod = CstPeriodDetail.monthly;
            webSession.PeriodBeginningDate = string.Format("{0}{1}", startDate.Year, startDate.Month.ToString("00"));
            webSession.PeriodEndDate = string.Format("{0}{1}", endDate.Year, endDate.Month.ToString("00"));
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = webSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutEndPeriod = dateDAL.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);
                return result;
            }
            else
            {
                result.StartYear = startDate.Year;
                result.EndYear = endDate.Year;
                result.Success = true;
                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodEndDate))
                {
                    webSession.PeriodEndDate = absolutEndPeriod;
                }
            }
            return result;
        }

        private PeriodResponse SaveSocialMediaCalendarData(WebSession webSession, PeriodResponse result, DateTime startDate, DateTime endDate)
        {

            webSession.PeriodType = CstPeriodType.dateToDate;
            webSession.DetailPeriod = CstPeriodDetail.monthly;
            webSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            webSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
            result.StartYear = startDate.Year;
            result.EndYear = endDate.Year;
            result.Success = true;

            return result;
        }
        #endregion

        private void AnalysisSlidingValidation(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            #region TOREFACTOR           
            try
            {
                HandleSlidingData(request, webSession, result);
            }
            catch (TNS.AdExpress.Domain.Exceptions.NoDataException exception)
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, exception.InnerException + exception.Message);
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            catch (System.Exception ex)
            {
                result.ErrorMessage = ex.Message;
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule), webSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }

            #endregion
        }

        private void SocialMediaSlidingValidation(PeriodSaveRequest request, PeriodResponse result, WebSession webSession)
        {
            #region TOREFACTOR           
            try
            {
                HandleSocialMediaSlidingData(request, webSession, result);
            }
            catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
            {
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);
            }
            catch (System.Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            #endregion
        }

        private void HandleSocialMediaSlidingData(PeriodSaveRequest request, WebSession webSession, PeriodResponse result)
        {

            DateTime downloadDate = new DateTime(webSession.DownLoadDate, 12, 31);
            int months = 0;
            int years = 0;
            string startDateFormat = string.Empty;
            string endDateFormat = string.Empty;
            switch (request.SelectedPeriod)
            {
                case 0: // N last year
                    webSession.PeriodType = CstPeriodType.nLastYear;
                    webSession.PeriodLength = request.SelectedValue;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    break;
                case 1: // N last months
                    webSession.PeriodLength = request.SelectedValue;
                    months = 1 - webSession.PeriodLength;
                    startDateFormat = "yyyyMM01";
                    endDateFormat = "yyyyMMdd";
                    webSession.PeriodType = CstPeriodType.nLastMonth;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    break;
                case 8:// Current year
                    startDateFormat = "yyyy0101";
                    endDateFormat = "yyyyMMdd";
                    webSession.PeriodType = CstPeriodType.currentYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    break;
                case 4:// Previous year
                    startDateFormat = "yyyy0101";
                    endDateFormat = "yyyy12dd";
                    webSession.PeriodType = CstPeriodType.previousYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    years = 1;
                    break;
                case 9:// N-2 year
                    startDateFormat = "yyyy0101";
                    endDateFormat = "yyyy12dd";
                    webSession.PeriodType = CstPeriodType.nextToLastYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    years = 2;
                    break;
                default:
                    break;
            };



            try
            {
                if (webSession.PeriodType == CstPeriodType.nLastYear)
                {
                    var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSession.PeriodLength).ToString("yyyy0101");
                    webSession.PeriodEndDate = endDate.ToString("yyyyMMdd");
                }
                else
                {
                    //previous month
                    var endDate = new DateTime(DateTime.Now.AddYears(-years).AddMonths(-1).Year, DateTime.Now.AddYears(-years).AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddYears(-years).AddMonths(-1).Year, DateTime.Now.AddYears(-years).AddMonths(-1).Month));
                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-years).AddMonths(months).ToString(startDateFormat);
                    webSession.PeriodEndDate = endDate.ToString(endDateFormat);
                }

                webSession.Save();
                webSession.Source.Close();
                result.Success = true;
                result.StartYear = int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
                result.EndYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4));
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
        }
        private void HandleSlidingData(PeriodSaveRequest request, WebSession webSession, PeriodResponse result)
        {
            // Cas où l'année de chargement des données est inférieur à l'année actuelle

            DateTime downloadDate = new DateTime(webSession.DownLoadDate, 12, 31);
            int months = 0;
            int years = 0;
            string startDateFormat = string.Empty;
            string endDateFormat = string.Empty;
            switch (request.SelectedPeriod)
            {

                case 1: // N last months
                    webSession.PeriodLength = request.SelectedValue;
                    months = 1 - webSession.PeriodLength;
                    startDateFormat = YYYYMM;
                    endDateFormat = YYYYMM;
                    webSession.PeriodType = CstPeriodType.nLastMonth;
                    webSession.DetailPeriod = CstPeriodDetail.dayly;
                    ManageAbsoluEndDate(webSession, result);
                    break;
                case 8:// Current year
                    startDateFormat = YYYY01;
                    endDateFormat = YYYYMM;
                    webSession.PeriodType = CstPeriodType.currentYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    ManageAbsoluEndDate(webSession, result);
                    break;
                case 4:// Previous year
                    startDateFormat = YYYY01;
                    endDateFormat = YYYY12;
                    webSession.PeriodType = CstPeriodType.previousYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    years = 1;
                    break;
                case 9: // Penultimate year
                    if (IsComparativeStudy(request.StudyId) && WebApplicationParameters.DataNumberOfYear <= 3)
                    {
                        result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.PeriodRequired, webSession.SiteLanguage);
                        break;
                    }
                    webSession.PeriodType = CstPeriodType.nextToLastYear;
                    webSession.PeriodLength = 1;
                    webSession.DetailPeriod = CstPeriodDetail.monthly;
                    startDateFormat = YYYY01;
                    endDateFormat = YYYY12;
                    years = 2;
                    break;
                default:
                    break;
            };


            try
            {

                if (DateTime.Now.Year > webSession.DownLoadDate)
                {
                    webSession.PeriodBeginningDate = downloadDate.AddYears(-years).AddMonths(months).ToString(startDateFormat);
                    webSession.PeriodEndDate = downloadDate.AddYears(-years).ToString(endDateFormat);

                }
                else
                {
                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-years).AddMonths(months).ToString(startDateFormat);
                    webSession.PeriodEndDate = DateTime.Now.AddYears(-years).ToString(endDateFormat);
                }
                webSession.ComparativeStudy = IsComparativeStudy(request.StudyId);


                webSession.Save();
                webSession.Source.Close();
                result.Success = true;
                result.StartYear = int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
                result.EndYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4));
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
        }

        public void ManageAbsoluEndDate(WebSession webSession, PeriodResponse result)
        {
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = webSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            string absolutEndPeriod = dateDAL.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
            if ((int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
                result.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.IncompleteDataForQuery, webSession.SiteLanguage);

            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodEndDate))
                webSession.PeriodEndDate = absolutEndPeriod;
        }
        private void DefaultSlidingPeriodValidation(PeriodSaveRequest request, PeriodResponse response, WebSession webSession)
        {
            try
            {
                globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
                globalCalendar.comparativePeriodType comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
                IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

                if (request.SelectedValue == 0)
                {
                    response.Success = false;
                    response.ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.PeriodRequired, webSession.SiteLanguage);
                }
                else
                {
                    date.SetDate(ref webSession, DateTime.Now, periodCalendarDisponibilityType, comparativePeriodCalendarType, request.SelectedPeriod, request.SelectedValue);
                    webSession.Save();
                    response.Success = true;
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Une erreur est survenue. Impossible de sauvegarder les dates sélectionnées";//TODO : a mettre dans ressources
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule), webSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
        }

    }
}