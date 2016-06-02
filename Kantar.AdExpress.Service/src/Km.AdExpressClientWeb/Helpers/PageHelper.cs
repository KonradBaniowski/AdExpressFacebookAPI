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
        public const string PERIODSELECTION = "PeriodSelection";
        private const string SELECTION = "Selection";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string ANALYSIS = "Analysis";
        public List<NavigationNode> LoadNavBar(string idWebSession, string controller,int siteLanguage = 33, int CurrentPosition = 0)
        {

            var model = new List<NavigationNode>();
            var webSession = (WebSession)WebSession.Load(idWebSession);
            string resultController = string.Empty;
            switch (webSession.CurrentModule)
            {
                case Module.Name.ANALYSE_CONCURENTIELLE:
                    resultController = PRESENTABSENT;
                    break;
                case Module.Name.ALERTE_PORTEFEUILLE:
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
            }
            //var ctr = (webSession.CurrentModule == Module.Name.ANALYSE_CONCURENTIELLE || webSession.CurrentModule == Module.Name.ALERTE_PORTEFEUILLE || webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) ? controller : SELECTION;
            #region   nav Bar.
            var market = new NavigationNode
            {
                Id = 1,
                IsActive = webSession.IsCurrentUniversProductSelected(),
                Description = MARKET,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Market, siteLanguage),
                Action =  (webSession.CurrentModule== Module.Name.ANALYSE_CONCURENTIELLE || webSession.CurrentModule == Module.Name.ALERTE_PORTEFEUILLE || webSession.CurrentModule == Module.Name.ANALYSE_DYNAMIQUE) ? INDEX : MARKET,
                Controller = controller,
                IconCssClass = "fa fa-file-text",
                Position = CurrentPosition
            };
            model.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = webSession.isMediaSelected(),
                Description = MEDIA,
                Title = GestionWeb.GetWebWord(LanguageConstantes.Media, siteLanguage),
                Action = MEDIASELECTION,
                Controller = controller,
                IconCssClass = "fa fa-eye",
                Position = CurrentPosition
            };
            model.Add(media);
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
            model.Add(dates);
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
            model.Add(result);
            #endregion
            return model;
        }

        public static string GetSiteLanguageName(int siteLanguage)
        {
            switch (siteLanguage)
            {
                case 44: return "EN";
                default: return "FR";
            }
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
                ResultError = ConvertToHtmlString(GestionWeb.GetWebWord(LanguageConstantes.ResultErrorCode, siteLanguage))
            };
            return result;
        }

        public PresentationModel LoadPresentationBar(int siteLanguage, Domain.ControllerDetails controllerDetails,bool showCurrentSelection = true)
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
    }
    
}