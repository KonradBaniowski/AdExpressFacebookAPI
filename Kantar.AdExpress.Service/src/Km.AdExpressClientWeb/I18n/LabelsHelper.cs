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


            };
            return result;
        }
    }
}