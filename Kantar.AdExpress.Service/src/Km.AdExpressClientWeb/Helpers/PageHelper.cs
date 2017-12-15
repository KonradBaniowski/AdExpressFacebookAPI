using Domain = Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.Web.Mvc;
using TNS.Alert.Domain;

namespace Km.AdExpressClientWeb.Helpers
{
    public class PageHelper
    {
        public const string INDEX = "Index";
        public const string MARKET = "Market";
        public const string MEDIA = "Media";
        public const string DATES = "Dates";
        public const string RESULTS = "Results";
        public const string MEDIASELECTION = "MediaSelection";
        public const string SPONSORSHIPMEDIASELECTION = "SponsorshipMediaSelection";
        public const string HEALTH_MEDIA_SELECTION = "HealthMediaSelection";
        public const string PERIODSELECTION = "PeriodSelection";
        private const string SELECTION = "Selection";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string FACEBOOK = "SocialMedia";
        private const string ANALYSIS = "Analysis";
        private const string NEW_CREATIVES = "NewCreatives";
        private const string ADVERTISING_AGENCY = "AdvertisingAgency";
        private const string CAMPAIGN_ANALYSIS = "CampaignAnalysis";
        private const string PROGRAM_ANALYSIS = "ProgramAnalysis";
        private const string HEALTH = "Health";
        const int ExportFormattedResult = 1;
        const int ExportResultWithValue = 2;
        const int ExportGrossResult = 3;
        const int ExportPdfResult = 4;
        const int ExportPptResult = 5;
        const int ExportSpotsResult = 6;



        public NavigationBarViewModel LoadNavBar(string idWebSession, string controller, int siteLanguage = -1, int CurrentPosition = 0)
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            NavigationBarViewModel model = new NavigationBarViewModel();
            model.Labels = new Labels();
            model.Labels.NavigationNodeRequired = GestionWeb.GetWebWord(LanguageConstantes.NavigationNodeRequired, siteLanguage);
            var webSession = (WebSession)WebSession.Load(idWebSession);
            string resultController = string.Empty;
            string mediaSelection = MEDIASELECTION;
            switch (webSession.CurrentModule)
            {
                case Module.Name.ANALYSE_CONCURENTIELLE:
                    resultController = PRESENTABSENT;
                    break;
                case Module.Name.ANALYSE_PORTEFEUILLE:
                    resultController = PORTFOLIO;
                    break;
                case Module.Name.ANALYSE_DYNAMIQUE:
                    resultController = LOSTWON;
                    break;
                case Module.Name.ANALYSE_PLAN_MEDIA:
                    resultController = MEDIASCHEDULE;
                    controller = SELECTION;
                    break;
                case Module.Name.TABLEAU_DYNAMIQUE:
                case Module.Name.INDICATEUR:
                    resultController = ANALYSIS;
                    controller = SELECTION;
                    break;
                case Module.Name.FACEBOOK:
                    resultController = FACEBOOK;
                    controller = SELECTION;
                    break;
                case Module.Name.NEW_CREATIVES:
                    resultController = NEW_CREATIVES;
                    controller = SELECTION;
                    break;
                case Module.Name.ANALYSE_MANDATAIRES:
                    resultController = ADVERTISING_AGENCY;
                    controller = SELECTION;
                    break;
                case Module.Name.ANALYSE_DES_DISPOSITIFS:
                    resultController = CAMPAIGN_ANALYSIS;
                    controller = SELECTION;
                    break;
                case Module.Name.ANALYSE_DES_PROGRAMMES:
                    resultController = PROGRAM_ANALYSIS;
                    controller = SELECTION;
                    mediaSelection = SPONSORSHIPMEDIASELECTION;
                    break;
                case Module.Name.HEALTH:
                    resultController = HEALTH;
                    controller = SELECTION;
                    mediaSelection = HEALTH_MEDIA_SELECTION;
                    break;
            }
            //var ctr = (webSession.CurrentModule == Module.Name.ANALYSE_CONCURENTIELLE || webSession.CurrentModule == Module.Name.ALERTE_PORTEFEUILLE || webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) ? controller : SELECTION;
            #region   nav Bar.
            bool marketActive = false;

            if (webSession.CurrentModule == Module.Name.ANALYSE_MANDATAIRES)
                marketActive = webSession.IsCurrentUniversAdvertisingAgnecySelected();
            else
                marketActive = webSession.IsCurrentUniversProductSelected();

            model.NavigationNodes = new List<NavigationNode>();

            var market = new NavigationNode
            {
                Id = 1,
                IsActive = marketActive,
                Description = MARKET,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Market, siteLanguage),
                Action = (webSession.CurrentModule == Module.Name.ANALYSE_CONCURENTIELLE || webSession.CurrentModule == Module.Name.ANALYSE_PORTEFEUILLE || webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) ? INDEX : MARKET,
                Controller = controller,
                IconCssClass = "fa fa-file-text",
                Position = CurrentPosition,
                IsDisabled = IsMarketSelectionDisabled(webSession.CurrentModule)
            };
            model.NavigationNodes.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = webSession.isMediaSelected(),
                Description = MEDIA,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Media, siteLanguage),
                Action = mediaSelection,
                Controller = controller,
                IconCssClass = "fa fa-eye",
                Position = CurrentPosition,
                IsDisabled = IsMediaSelectionDisabled(webSession.CurrentModule)
            };
            model.NavigationNodes.Add(media);
            var dates = new NavigationNode
            {
                Id = 3,
                IsActive = webSession.isDatesSelected(),
                Description = DATES,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Dates, siteLanguage),
                Action = PERIODSELECTION,
                Controller = controller,
                IconCssClass = "fa fa-calendar",
                Position = CurrentPosition
            };
            model.NavigationNodes.Add(dates);
            var result = new NavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = RESULTS,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Action = RESULTS,
                Controller = resultController,
                IconCssClass = "fa fa-check",
                Position = CurrentPosition
            };
            model.NavigationNodes.Add(result);
            #endregion
            return model;
        }

        public static string GetSiteLanguageName(int siteLanguage)
        {
            foreach (WebLanguage currentLanguage in WebApplicationParameters.AllowedLanguages.Values)
            {
                if (currentLanguage.Id == siteLanguage)
                    return currentLanguage.Name.Substring(0, 2).ToUpper();
            }

            return "FR";
        }

        public Labels LoadPageLabels(int siteLanguage, string controller)
        {
            var result = new Labels
            {
                KeyWordLabel = GestionWeb.GetWebWord(LanguageConstantes.KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(LanguageConstantes.KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                BranchLabel = GestionWeb.GetWebWord(LanguageConstantes.BranchLabelCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                UserSavedUniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                Include = GestionWeb.GetWebWord(LanguageConstantes.IncludeCode, siteLanguage),
                Exclude = GestionWeb.GetWebWord(LanguageConstantes.ExcludeCode, siteLanguage),
                LoadUnivers = GestionWeb.GetWebWord(LanguageConstantes.LoadUniversCode, siteLanguage),
                IncludedElements = GestionWeb.GetWebWord(LanguageConstantes.IncludedElements, siteLanguage),
                ExcludedElements = GestionWeb.GetWebWord(LanguageConstantes.ExcludedElements, siteLanguage),
                MyResults = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),
                Refine = GestionWeb.GetWebWord(LanguageConstantes.RefineCode, siteLanguage),
                ErrorMessageLimitKeyword = GestionWeb.GetWebWord(LanguageConstantes.LimitKeyword, siteLanguage),
                ErrorMessageLimitUniverses = GestionWeb.GetWebWord(LanguageConstantes.LimitUniverses, siteLanguage),
                ErrorMininumInclude = GestionWeb.GetWebWord(LanguageConstantes.MininumInclude, siteLanguage),
                ErrorItemExceeded = GestionWeb.GetWebWord(LanguageConstantes.ItemExceeded, siteLanguage),
                ErrorMediaSelected = GestionWeb.GetWebWord(LanguageConstantes.MediaSelected, siteLanguage),
                ErrorNoSupport = GestionWeb.GetWebWord(LanguageConstantes.NoSupport, siteLanguage),
                DeleteAll = GestionWeb.GetWebWord(LanguageConstantes.DeleteAllcode, siteLanguage),
                ErrorOnlyOneItemAllowed = GestionWeb.GetWebWord(LanguageConstantes.ErrorOnlyOneItemAllowed, siteLanguage),
                ErrorOverLimit = GestionWeb.GetWebWord(LanguageConstantes.ErrorOverLimit, siteLanguage),
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                UnityError = GestionWeb.GetWebWord(LanguageConstantes.UnityError, siteLanguage),
                SelectMedia = GestionWeb.GetWebWord(LanguageConstantes.SelectMedia, siteLanguage),
                PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelection, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Save = GestionWeb.GetWebWord(LanguageConstantes.Save, siteLanguage),
                CreateAlert = GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage),
                ExportFormattedResult = GestionWeb.GetWebWord(LanguageConstantes.ExportFormattedResult, siteLanguage),
                ExportResultWithValue = GestionWeb.GetWebWord(LanguageConstantes.ExportResultWithValue, siteLanguage),
                ExportGrossResult = GestionWeb.GetWebWord(LanguageConstantes.ExportGrossResult, siteLanguage),
                ExportPdfResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage),
                ExportPptResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage),
                Search = GestionWeb.GetWebWord(LanguageConstantes.Search, siteLanguage),
                AddConcurrent = GestionWeb.GetWebWord(LanguageConstantes.AddConcurrentCode, siteLanguage),
                ErrorSupportAlreadyDefine = GestionWeb.GetWebWord(LanguageConstantes.SupportAlreadyDefine, siteLanguage),
                Concurrent = GestionWeb.GetWebWord(LanguageConstantes.Concurrent, siteLanguage),
                Referent = GestionWeb.GetWebWord(LanguageConstantes.Referent, siteLanguage),
                WarningBackNavigator = GestionWeb.GetWebWord(LanguageConstantes.WarningBackNavigatorCode, siteLanguage),
                ResultError = ConvertToHtmlString(GestionWeb.GetWebWord(LanguageConstantes.ResultErrorCode, siteLanguage)),
                EmptyGrid = GestionWeb.GetWebWord(LanguageConstantes.EmptyGrid, siteLanguage),
                FacebookModalTitle = GestionWeb.GetWebWord(LanguageConstantes.FacebookPost, siteLanguage),
                UnityLabel = GestionWeb.GetWebWord(LanguageConstantes.UnityLabel, siteLanguage),
                AllLabel = GestionWeb.GetWebWord(LanguageConstantes.UnityLabel, siteLanguage),
                SeasonalityOfLabel = GestionWeb.GetWebWord(LanguageConstantes.SeasonalityOfLabel, siteLanguage),
                PDMChartTitleLabel = GestionWeb.GetWebWord(LanguageConstantes.PDMChartTitleLabel, siteLanguage),
                EngagementLevel = GestionWeb.GetWebWord(LanguageConstantes.EngagementLevel, siteLanguage),
                CumulKPILabel = GestionWeb.GetWebWord(LanguageConstantes.CumulKPILabel, siteLanguage),
                PluriStackedChartTitleLabel = GestionWeb.GetWebWord(LanguageConstantes.PluriStackedChartTitleLabel, siteLanguage),
                MonthByMonthLabel = GestionWeb.GetWebWord(LanguageConstantes.MonthByMonthLabel, siteLanguage),
                PDMChartSubTitleLabel = GestionWeb.GetWebWord(LanguageConstantes.PDMChartSubTitleLabel, siteLanguage),
                AdvertiserOrBrandMonthByMonthLabel = GestionWeb.GetWebWord(LanguageConstantes.AdvertiserOrBrandMonthByMonthLabel, siteLanguage),
                VentilatedByAdvertiserOrBrandLabel = GestionWeb.GetWebWord(LanguageConstantes.VentilatedByAdvertiserOrBrandLabel, siteLanguage),
                InvestAndBEXLabel = GestionWeb.GetWebWord(LanguageConstantes.InvestAndBEXLabel, siteLanguage),
                ReferentsSelectorLabel = GestionWeb.GetWebWord(LanguageConstantes.ReferentsSelectorLabel, siteLanguage),
                CompetingSelectorLabel = GestionWeb.GetWebWord(LanguageConstantes.CompetingSelectorLabel, siteLanguage),
                TopPostsFBLabel = GestionWeb.GetWebWord(LanguageConstantes.TopPostsFBLabel, siteLanguage),
                Export = GestionWeb.GetWebWord(LanguageConstantes.Export, siteLanguage),
                SelectUniverseMarketLabel = GestionWeb.GetWebWord(LanguageConstantes.SelectUniverseMarketLabel, siteLanguage),
                PDMLabel = GestionWeb.GetWebWord(LanguageConstantes.PDMLabel, siteLanguage),
                PDVLabel = GestionWeb.GetWebWord(LanguageConstantes.PDVLabel, siteLanguage),
                MonthLabel = GestionWeb.GetWebWord(LanguageConstantes.MonthLabel, siteLanguage),
                Timeout = GestionWeb.GetWebWord(LanguageConstantes.Timeout, siteLanguage),
                TimeoutBis = GestionWeb.GetWebWord(LanguageConstantes.TimeoutBis, siteLanguage),
                MaxAllowedRows = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRows, siteLanguage),
                MaxAllowedRowsBis = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsBis, siteLanguage),
                MaxAllowedRowsRefine = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsRefine, siteLanguage)
            };

            if (WebApplicationParameters.CountryCode.Equals(CountryCode.FINLAND)
                || WebApplicationParameters.CountryCode.Equals(CountryCode.SLOVAKIA))
                result.PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelectionWithoutEvaliant, siteLanguage);

            return result;
        }

        public PresentationModel LoadPresentationBar(int siteLanguage, Domain.ControllerDetails controllerDetails, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = controllerDetails.ModuleCode,
                SiteLanguage = siteLanguage,
                ShowCurrentSelection = showCurrentSelection,
                ModuleIcon = controllerDetails.ModuleIcon
            };
            return result;
        }

        private string ConvertToHtmlString(string text)
        {
            string[,] strArrays = new string[25, 2];
            strArrays[0, 0] = "Á";
            strArrays[0, 1] = "&Aacute;";
            strArrays[1, 0] = "á";
            strArrays[1, 1] = "&aacute;";
            strArrays[2, 0] = "Â";
            strArrays[2, 1] = "&Acirc;";
            strArrays[3, 0] = "â";
            strArrays[3, 1] = "&acirc;";
            strArrays[4, 0] = "À";
            strArrays[4, 1] = "&Agrave;";
            strArrays[5, 0] = "à";
            strArrays[5, 1] = "&agrave;";
            strArrays[6, 0] = "Ç";
            strArrays[6, 1] = "&Ccedil;";
            strArrays[7, 0] = "ç";
            strArrays[7, 1] = "&ccedil;";
            strArrays[8, 0] = "É";
            strArrays[8, 1] = "&Eacute;";
            strArrays[9, 0] = "é";
            strArrays[9, 1] = "&eacute;";
            strArrays[10, 0] = "Ê";
            strArrays[10, 1] = "&Ecirc;";
            strArrays[11, 0] = "ê";
            strArrays[11, 1] = "&ecirc;";
            strArrays[12, 0] = "È";
            strArrays[12, 1] = "&Egrave;";
            strArrays[13, 0] = "è";
            strArrays[13, 1] = "&egrave;";
            strArrays[14, 0] = "Î";
            strArrays[14, 1] = "&Icirc;";
            strArrays[15, 0] = "î";
            strArrays[15, 1] = "&icirc;";
            strArrays[16, 0] = "Ô";
            strArrays[16, 1] = "&Ocirc;";
            strArrays[17, 0] = "ô";
            strArrays[17, 1] = "&ocirc;";
            strArrays[18, 0] = "Ö";
            strArrays[18, 1] = "&Ouml;";
            strArrays[19, 0] = "ö";
            strArrays[19, 1] = "&ouml;";
            strArrays[20, 0] = "Û";
            strArrays[20, 1] = "&Ucirc;";
            strArrays[21, 0] = "û";
            strArrays[21, 1] = "&ucirc;";
            strArrays[22, 0] = "Ü";
            strArrays[22, 1] = "&Uuml;";
            strArrays[23, 0] = "ü";
            strArrays[23, 1] = "&uuml;";
            strArrays[24, 0] = "€";
            strArrays[24, 1] = "&euro;";
            string[,] characters = strArrays;
            for (int i = 0; i < characters.GetLength(0); i++)
            {
                text = text.Replace(characters[i, 0], characters[i, 1]);
            }
            return text;
        }

        bool IsMediaSelectionDisabled(long module)
        {
            switch (module)
            {

                case Module.Name.FACEBOOK:
                case Module.Name.ANALYSE_DES_DISPOSITIFS:
                    return true;
                default:
                    return false;
            }
        }

        bool IsMarketSelectionDisabled(long module)
        {
            bool isDisabled = false; // (module == Module.Name.NEW_CREATIVES);
            return isDisabled;
        }

        public static bool IsAlertVisible(string countryCode, string idWebSession)
        {
            bool returnValue = false;
            var webSession = (WebSession)WebSession.Load(idWebSession);
            switch (countryCode)
            {
                case TNS.AdExpress.Constantes.Web.CountryCode.FRANCE:
                    if (AlertConfiguration.IsActivated
                        && webSession.CustomerLogin.HasModuleAssignmentAlertsAdExpress()
                        && webSession.CustomerLogin.IsModuleAssignmentValidDateAlertsAdExpress())
                    {
                        returnValue = true;
                    }
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        internal static bool IsMyAdexpressVisible(string countryCode)
        {
            bool returnValue = false;
            switch (countryCode)
            {
                case TNS.AdExpress.Constantes.Web.CountryCode.FRANCE:
                    returnValue = true;
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        internal static bool IsPowerpointResultVisible(string countryCode)
        {
            bool returnValue = false;
            switch (countryCode)
            {
                case TNS.AdExpress.Constantes.Web.CountryCode.FRANCE:
                    returnValue = true;
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        public static List<ExportTypeViewModel> GetExportTypes(string countryCode, int currentModule, int siteLanguage)
        {
            List<ExportTypeViewModel> exportTypeViewModels = new List<ExportTypeViewModel>();
            exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportFormattedResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportFormattedResult, siteLanguage), Visible = true });
            exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportResultWithValue, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportResultWithValue, siteLanguage), Visible = true });
            exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportPdfResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage), Visible = true });

            switch (countryCode)
            {
                case TNS.AdExpress.Constantes.Web.CountryCode.FRANCE:
                    exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportGrossResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportGrossResult, siteLanguage), Visible = true });
                    exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportPptResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage), Visible = true });
                    exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportSpotsResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportSpotsResult, siteLanguage), Visible = true });
                    break;
                case TNS.AdExpress.Constantes.Web.CountryCode.SLOVAKIA:
                    exportTypeViewModels.Add(new ExportTypeViewModel { Id = ExportPptResult, Label = GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage), Visible = true });
                    break;
            }

            SetExportTypesVisibilityByModule(exportTypeViewModels, currentModule);
            return exportTypeViewModels;
        }

        private static void SetExportTypesVisibilityByModule(List<ExportTypeViewModel> exportTypeViewModels, int currentModule)
        {
            List<int> ids = new List<int>();
            switch (currentModule)
            {
                case Module.Name.ANALYSE_PLAN_MEDIA:
                    ids.Add(ExportFormattedResult);
                    ids.Add(ExportGrossResult);
                    ids.Add(ExportResultWithValue);
                    if (WebApplicationParameters.CountryCode != CountryCode.TURKEY)
                    {
                        ids.Add(ExportPdfResult);
                        ids.Add(ExportPptResult);
                    }
                    break;
                case Module.Name.ANALYSE_DYNAMIQUE:
                case Module.Name.ANALYSE_PORTEFEUILLE:
                case Module.Name.ANALYSE_CONCURENTIELLE:
                case Module.Name.NEW_CREATIVES:
                    ids.Add(ExportFormattedResult);
                    ids.Add(ExportGrossResult);
                    break;
                case Module.Name.TABLEAU_DYNAMIQUE:
                case Module.Name.INDICATEUR:
                case Module.Name.FACEBOOK:
                case Module.Name.ANALYSE_MANDATAIRES:
                    ids.Add(ExportFormattedResult);
                    break;
                case Module.Name.ANALYSE_DES_DISPOSITIFS:
                    ids.Add(ExportFormattedResult);
                    ids.Add(ExportSpotsResult);
                    break;
                case Module.Name.ANALYSE_DES_PROGRAMMES:
                    ids.Add(ExportFormattedResult);
                    break;
                default:
                    break;
            }

            exportTypeViewModels.ForEach(p =>
            {
                if (ids.Any() && !ids.Contains(p.Id)) p.Visible = false;
            });
        }

        public static Kantar.AdExpress.Service.Core.Domain.ClientInformation GetClientInformation(HttpContextBase httpContext)
        {
            var clientInformation = new Kantar.AdExpress.Service.Core.Domain.ClientInformation();
            clientInformation.Browser = httpContext.Request.Browser.Browser;
            clientInformation.BrowserVersion = httpContext.Request.Browser.Version;
            clientInformation.BrowserMinorVersion = httpContext.Request.Browser.MinorVersion;
            clientInformation.BrowserPlatform = httpContext.Request.Browser.Platform;
            clientInformation.UserAgent = httpContext.Request.UserAgent;
            clientInformation.UserHostAddress = httpContext.Request.UserHostAddress;
            clientInformation.Url = httpContext.Request.Url.ToString();
            clientInformation.ServerMachineName = httpContext.Server.MachineName;
            return clientInformation;
        }
    }

}