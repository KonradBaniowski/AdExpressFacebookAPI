﻿using Km.AdExpressClientWeb.Models;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

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
                Results = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
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
                UserUniversCode = GestionWeb.GetWebWord(LanguageConstantes.UserUniversCode, siteLanguage),
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
                PreSelectionWithoutEvaliant = GestionWeb.GetWebWord(LanguageConstantes.PreSelectionWithoutEvaliant, siteLanguage),
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
                ResultError = GestionWeb.GetWebWord(LanguageConstantes.ResultErrorCode, siteLanguage),
                RememberEmailLabel = GestionWeb.GetWebWord(LanguageConstantes.RememberEmailCode,  siteLanguage),
                Export = GestionWeb.GetWebWord(LanguageConstantes.Export, siteLanguage),
                FileName = GestionWeb.GetWebWord(LanguageConstantes.FileName, siteLanguage),
                Email = GestionWeb.GetWebWord(LanguageConstantes.MailCode, siteLanguage),
                Submit = GestionWeb.GetWebWord(LanguageConstantes.Submit, siteLanguage),
                Close = GestionWeb.GetWebWord(LanguageConstantes.Close, siteLanguage),
                DefaultUniverse = GestionWeb.GetWebWord(LanguageConstantes.DefaultUniverse, siteLanguage),
                FacebookAllowedLevels = GestionWeb.GetWebWord(LanguageConstantes.FacebookAllowedLevels, siteLanguage),
                MaxFacebookItems = GestionWeb.GetWebWord(LanguageConstantes.MaxFacebookItems, siteLanguage),
                Redirecting = GestionWeb.GetWebWord(LanguageConstantes.Redirecting, siteLanguage),
                WriteToUs = GestionWeb.GetWebWord(LanguageConstantes.WriteToUs, siteLanguage),
                CallUs = GestionWeb.GetWebWord(LanguageConstantes.CallUs, siteLanguage),
                ClientService = GestionWeb.GetWebWord(LanguageConstantes.ClientService, siteLanguage),
                Unit =GestionWeb.GetWebWord(LanguageConstantes.Unit,siteLanguage),
                FacebookDescription = GestionWeb.GetWebWord(LanguageConstantes.FacebookDescription, siteLanguage),
                FacebookModalTitle = GestionWeb.GetWebWord(LanguageConstantes.FacebookPost, siteLanguage),
                EmptyGrid = GestionWeb.GetWebWord(LanguageConstantes.EmptyGrid, siteLanguage),
                Timeout = GestionWeb.GetWebWord(LanguageConstantes.Timeout, siteLanguage),
                TimeoutBis = GestionWeb.GetWebWord(LanguageConstantes.TimeoutBis, siteLanguage),
                MaxAllowedRows = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRows, siteLanguage),
                MaxAllowedRowsBis = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsBis, siteLanguage),
                MaxAllowedRowsRefine = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsRefine, siteLanguage),
                DeleteUniversMessageConfirmLabel = GestionWeb.GetWebWord(LanguageConstantes.DeleteUniversMessageConfirmLabel, siteLanguage),
                DeleteResultMessageConfirmLabel = GestionWeb.GetWebWord(LanguageConstantes.DeleteResultMessageConfirmLabel, siteLanguage),
                TitleLabel = GestionWeb.GetWebWord(LanguageConstantes.TitleLabel, siteLanguage),
                SelectVehicle = GestionWeb.GetWebWord(LanguageConstantes.SelectVehicle, siteLanguage),
                RefineProgrammeSponsorshpCategory = GestionWeb.GetWebWord(LanguageConstantes.RefineProgrammeSponsorshpCategory, siteLanguage),
                SelectAllVehicles = GestionWeb.GetWebWord(LanguageConstantes.SelectAllVehicles, siteLanguage),
                SponsorshipMedias = GestionWeb.GetWebWord(LanguageConstantes.SponsorshipMedias, siteLanguage),
                SponsorshipWorkingSet = GestionWeb.GetWebWord(LanguageConstantes.SponsorshipWorkingSet, siteLanguage),
                VisualsLabel = GestionWeb.GetWebWord(LanguageConstantes.VisualsLabel, siteLanguage),
                AdvertiserLabel = GestionWeb.GetWebWord(LanguageConstantes.AdvertiserLabel, siteLanguage),
                ProductLabel = GestionWeb.GetWebWord(LanguageConstantes.ProductLabel, siteLanguage),
                SectorLabel = GestionWeb.GetWebWord(LanguageConstantes.SectorLabel, siteLanguage),
                GroupLabel = GestionWeb.GetWebWord(LanguageConstantes.GroupLabel, siteLanguage),
                MediaPagingLabel = GestionWeb.GetWebWord(LanguageConstantes.MediaPagingLabel, siteLanguage),
                AreaPageLabel = GestionWeb.GetWebWord(LanguageConstantes.AreaPageLabel, siteLanguage),
                AreaMmcLabel = GestionWeb.GetWebWord(LanguageConstantes.AreaMmcLabel, siteLanguage),
                ExpenditureEuroLabel = GestionWeb.GetWebWord(LanguageConstantes.ExpenditureEuroLabel, siteLanguage),
                LocationLabel = GestionWeb.GetWebWord(LanguageConstantes.LocationLabel, siteLanguage),
                FormatLabel = GestionWeb.GetWebWord(LanguageConstantes.FormatLabel, siteLanguage),
                ColorLabel = GestionWeb.GetWebWord(LanguageConstantes.ColorLabel, siteLanguage),
                RankSectorLabel = GestionWeb.GetWebWord(LanguageConstantes.RankSectorLabel, siteLanguage),
                RankMediaLabel = GestionWeb.GetWebWord(LanguageConstantes.RankMediaLabel, siteLanguage),
                RankGroupLabel = GestionWeb.GetWebWord(LanguageConstantes.RankGroupLabel, siteLanguage),
                TableParameters = GestionWeb.GetWebWord(LanguageConstantes.TableParameters, siteLanguage),
                VehicleBreakdown = GestionWeb.GetWebWord(LanguageConstantes.VehicleBreakdown, siteLanguage),
                VehicleLabel = GestionWeb.GetWebWord(LanguageConstantes.VehicleLabel, siteLanguage),
                DateLabel = GestionWeb.GetWebWord(LanguageConstantes.DateLabel, siteLanguage),
                WithInsetLabel = GestionWeb.GetWebWord(LanguageConstantes.WithInsetLabel, siteLanguage),
                WithoutInsetLabel = GestionWeb.GetWebWord(LanguageConstantes.WithoutInsetLabel, siteLanguage),
                WithAutopromoLabel = GestionWeb.GetWebWord(LanguageConstantes.WithAutopromoLabel, siteLanguage),
                WithoutAutopromoLabel = GestionWeb.GetWebWord(LanguageConstantes.WithoutAutopromoLabel, siteLanguage),
                SubCategoryLabel = GestionWeb.GetWebWord(LanguageConstantes.SubCategoryLabel, siteLanguage),
                SubGroupLabel = GestionWeb.GetWebWord(LanguageConstantes.SubGroupLabel, siteLanguage),
                NbPage = GestionWeb.GetWebWord(LanguageConstantes.NbPage, siteLanguage),
                LienCheminDeFerLabel = GestionWeb.GetWebWord(LanguageConstantes.LienCheminDeFerLabel, siteLanguage),
                GroupAdvertisingAgencyLabel = GestionWeb.GetWebWord(LanguageConstantes.GroupAdvertisingAgencyLabel, siteLanguage),
                AdvertisingAgencyLabel = GestionWeb.GetWebWord(LanguageConstantes.AdvertisingAgencyLabel, siteLanguage),
                AccountFindOutMoreLabel = GestionWeb.GetWebWord(LanguageConstantes.AccountFindOutMoreLabel, siteLanguage),
                AllRightsReservedLabel = string.Format(GestionWeb.GetWebWord(LanguageConstantes.AllRightsReservedLabel, siteLanguage),DateTime.Now.Year),
                FindOutMoreLabel = GestionWeb.GetWebWord(LanguageConstantes.FindOutMoreLabel, siteLanguage),

            };

            if (WebApplicationParameters.CountryCode.Equals(CountryCode.FINLAND)
                || WebApplicationParameters.CountryCode.Equals(CountryCode.SLOVAKIA)
                || WebApplicationParameters.CountryCode.Equals(CountryCode.POLAND))
                result.PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelectionWithoutEvaliant, siteLanguage);

            return result;
        }
    }
}