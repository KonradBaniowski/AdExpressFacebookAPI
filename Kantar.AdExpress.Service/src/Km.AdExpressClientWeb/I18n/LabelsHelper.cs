using Km.AdExpressClientWeb.Models;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;

namespace Km.AdExpressClientWeb.I18n
{
    public static class LabelsHelper
    {
        public static Labels LoadPageLabels(int siteLanguage)
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
                Save = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                IncludedElements = GestionWeb.GetWebWord(LanguageConstantes.IncludedElements, siteLanguage),
                ExcludedElements = GestionWeb.GetWebWord(LanguageConstantes.ExcludedElements, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),
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
                CompanyDescription = GestionWeb.GetWebWord(LanguageConstantes.CompanyCode, siteLanguage),
                NameDescription = GestionWeb.GetWebWord(LanguageConstantes.NameCode, siteLanguage),
                JobTitleDescription = GestionWeb.GetWebWord(LanguageConstantes.JobTitleCode, siteLanguage),
                PhoneNumberDescription = GestionWeb.GetWebWord(LanguageConstantes.PhoneNumberCode, siteLanguage),
                MailDescription = GestionWeb.GetWebWord(LanguageConstantes.MailCode, siteLanguage),
                CountryDescription = GestionWeb.GetWebWord(LanguageConstantes.CountryCode, siteLanguage),
                CommentDescription = GestionWeb.GetWebWord(LanguageConstantes.CommentCode, siteLanguage),
                QuestionTagDefault = GestionWeb.GetWebWord(LanguageConstantes.QuestionTagCode, siteLanguage),
                QuestionTag1 = GestionWeb.GetWebWord(LanguageConstantes.QuestionTag1Code, siteLanguage),
                QuestionTag2 = GestionWeb.GetWebWord(LanguageConstantes.QuestionTag2Code, siteLanguage),
                QuestionTag3 = GestionWeb.GetWebWord(LanguageConstantes.QuestionTag3Code, siteLanguage),

                //RAPPEL DE SELECTION
                StudySelectionLabel = GestionWeb.GetWebWord(LanguageConstantes.StudySelection, siteLanguage),
                PeriodSelectionLabel = GestionWeb.GetWebWord(LanguageConstantes.PeriodSelection, siteLanguage),
                LevelDetailsLabel = GestionWeb.GetWebWord(LanguageConstantes.LevelDetails, siteLanguage),
                MediaLabel = GestionWeb.GetWebWord(LanguageConstantes.MediaCode, siteLanguage),
                UnitLabel = GestionWeb.GetWebWord(LanguageConstantes.Unit, siteLanguage),
                InsertionLabel = GestionWeb.GetWebWord(LanguageConstantes.Insertion, siteLanguage),
                UniversProductLabel = GestionWeb.GetWebWord(LanguageConstantes.UniversProduct, siteLanguage),
                UniversSupportLabel = GestionWeb.GetWebWord(LanguageConstantes.UniversSupport, siteLanguage),
                StudyPeriodLabel = GestionWeb.GetWebWord(LanguageConstantes.StudyPeriodCode, siteLanguage),
                ComparativePeriodLabel = GestionWeb.GetWebWord(LanguageConstantes.ComparativePeriodCode, siteLanguage),
                ComparativePeriodTypeLabel = GestionWeb.GetWebWord(LanguageConstantes.ComparativePeriodTypeCode, siteLanguage),
                PeriodDisponibilityTypeLabel = GestionWeb.GetWebWord(LanguageConstantes.PeriodDisponibilityTypecode, siteLanguage),
                GenericLevelDetailColumnLabel = GestionWeb.GetWebWord(LanguageConstantes.GenericLevelDetailColumnCode, siteLanguage),

                //PAGE D ACCUEIL
                HomeLabel = GestionWeb.GetWebWord(LanguageConstantes.HomeCode, siteLanguage),

                //LOGIN
                SomeFigures = GestionWeb.GetWebWord(LanguageConstantes.SomeFigures, siteLanguage),
                Connection = GestionWeb.GetWebWord(LanguageConstantes.Connection, siteLanguage),
                SignIn = GestionWeb.GetWebWord(LanguageConstantes.SignIn, siteLanguage),
                UserName = GestionWeb.GetWebWord(LanguageConstantes.UserName, siteLanguage),
                Password = GestionWeb.GetWebWord(LanguageConstantes.Password, siteLanguage),

                DeleteRequestMessageConfirmLabel = GestionWeb.GetWebWord(LanguageConstantes.DeleteRequestMessageConfirm, siteLanguage),

                MyResults = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),
                UserUniversCode = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                MyResultsDescription = GestionWeb.GetWebWord(LanguageConstantes.MyResultsDescription, siteLanguage),
                AlertsCode = GestionWeb.GetWebWord(LanguageConstantes.AlertsCode, siteLanguage),
                Periodicity = GestionWeb.GetWebWord(LanguageConstantes.Periodicity, siteLanguage),
                Daily = GestionWeb.GetWebWord(LanguageConstantes.Daily, siteLanguage),
                Weekly = GestionWeb.GetWebWord(LanguageConstantes.Weekly, siteLanguage),
                Monthly = GestionWeb.GetWebWord(LanguageConstantes.Monthly, siteLanguage),
                Quartly = GestionWeb.GetWebWord(LanguageConstantes.Quartly, siteLanguage),
                SaveAlert = GestionWeb.GetWebWord(LanguageConstantes.SaveAlert, siteLanguage),
                NoAlerts = GestionWeb.GetWebWord(LanguageConstantes.NoAlerts, siteLanguage),
                SendDate = GestionWeb.GetWebWord(LanguageConstantes.SendDate, siteLanguage),
                Occurrence = GestionWeb.GetWebWord(LanguageConstantes.Occurrence, siteLanguage),
                Occurrences = GestionWeb.GetWebWord(LanguageConstantes.Occurrences, siteLanguage),
                AlertsDetails = GestionWeb.GetWebWord(LanguageConstantes.AlertDetails, siteLanguage),
                Deadline = GestionWeb.GetWebWord(LanguageConstantes.Deadline, siteLanguage),
                EveryWeek = GestionWeb.GetWebWord(LanguageConstantes.EveryWeek, siteLanguage),
                EveryMonth = GestionWeb.GetWebWord(LanguageConstantes.EveryMonth, siteLanguage),
                ExpirationDate = GestionWeb.GetWebWord(LanguageConstantes.ExpirationDate, siteLanguage),
                AlertType = GestionWeb.GetWebWord(LanguageConstantes.AlertType, siteLanguage),
                Receiver = GestionWeb.GetWebWord(LanguageConstantes.Receiver, siteLanguage),
                TimeSchedule = GestionWeb.GetWebWord(LanguageConstantes.TimeSchedule, siteLanguage),
                CreateDirectory = GestionWeb.GetWebWord(LanguageConstantes.CreateFolder, siteLanguage),
                RenameDirectory = GestionWeb.GetWebWord(LanguageConstantes.RenameSelectedFolder, siteLanguage),
                DropDirectory = GestionWeb.GetWebWord(LanguageConstantes.DropFolder, siteLanguage),
                Directories = GestionWeb.GetWebWord(LanguageConstantes.Directories, siteLanguage),
                ModuleLabel = GestionWeb.GetWebWord(LanguageConstantes.ModuleLabel, siteLanguage),
                NewsLabel = GestionWeb.GetWebWord(LanguageConstantes.NewsLabel, siteLanguage),
                YourModule = GestionWeb.GetWebWord(LanguageConstantes.YourModule, siteLanguage),
                NewsDescr = GestionWeb.GetWebWord(LanguageConstantes.NewsDescr, siteLanguage),
                ContactUsLabel = GestionWeb.GetWebWord(LanguageConstantes.ContactUsLabel, siteLanguage),
                Delete = GestionWeb.GetWebWord(LanguageConstantes.Delete, siteLanguage),
                DashboardsLabel = GestionWeb.GetWebWord(LanguageConstantes.DashboardsLabel, siteLanguage),
                AnalysisLabel = GestionWeb.GetWebWord(LanguageConstantes.AnalysisLabel, siteLanguage),
                UnityError = GestionWeb.GetWebWord(LanguageConstantes.UnityError, siteLanguage),
                SelectMedia = GestionWeb.GetWebWord(LanguageConstantes.SelectMedia, siteLanguage),
                PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelection, siteLanguage),
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
                ResultError = GestionWeb.GetWebWord(LanguageConstantes.ResultErrorCode, siteLanguage)
            };
            return result;
        }
    }
}