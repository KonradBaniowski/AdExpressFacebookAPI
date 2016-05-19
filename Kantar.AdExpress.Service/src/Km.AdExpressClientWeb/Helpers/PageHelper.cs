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
        public List<NavigationNode> LoadNavBar(string idWebSession, string controller,int siteLanguage = 33, int CurrentPosition = 0)
        {

            var model = new List<NavigationNode>();
            var webSession = (WebSession)WebSession.Load(idWebSession);
            #region   nav Bar.
            var market = new NavigationNode
            {
                Id = 1,
                IsActive = webSession.IsCurrentUniversProductSelected(),
                Description = "Market",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Market, siteLanguage),
                Action = "Index",
                Controller = controller,
                IconCssClass = "fa fa-file-text",
                Position = CurrentPosition
            };
            model.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = webSession.isMediaSelected(),
                Description = "Media",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Media, siteLanguage),
                Action = "MediaSelection",
                Controller = controller,
                IconCssClass = "fa fa-eye",
                Position = CurrentPosition
            };
            model.Add(media);
            var dates = new NavigationNode
            {
                Id = 3,
                IsActive = webSession.isDatesSelected(),
                Description = "Dates",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Dates, siteLanguage),
                Action = "PeriodSelection",
                Controller = controller,
                IconCssClass = "fa fa-calendar",
                Position = CurrentPosition
            };
            model.Add(dates);
            var result = new NavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Results",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Action = "Results",
                Controller = controller,
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
                Search = GestionWeb.GetWebWord(LanguageConstantes.Search, siteLanguage)
                //CurrentController = controller
            };
            return result;
        }

        public PresentationModel LoadPresentationBar(int siteLanguage, long module,bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = module,
                SiteLanguage = siteLanguage,
                ShowCurrentSelection = showCurrentSelection
            };
            return result;
        }
    }
    
}