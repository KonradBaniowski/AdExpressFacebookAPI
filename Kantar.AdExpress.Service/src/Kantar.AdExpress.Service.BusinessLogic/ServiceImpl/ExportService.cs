using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using KM.AdExpressI.MyAdExpress.DAL;
using System.Text.RegularExpressions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebCst = TNS.AdExpress.Constantes.Web;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.Core;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.Classification;
using AnubisCst = TNS.AdExpress.Anubis.Constantes;
using NLog;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class ExportService :IExportService
    {
        private const int FileMaxLength = 80;
        protected string StartDate = string.Empty;
        protected string EndDate = string.Empty;
        private string _idUnit = string.Empty;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public ExportResponse Export (ExportRequest request, HttpContextBase httpContext)
        {
            ExportResponse response = new ExportResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(request.WebSessionId);
            try {
                #region Validation

                AnubisCst.Result.type resultType = new AnubisCst.Result.type();
                switch (request.ExportType)
                {
                    case "4":
                        resultType = AnubisCst.Result.type.miysis;
                        request.ExportType = GestionWeb.GetWebWord(WebCst.LanguageConstantes.ExportPdfResult, webSession.SiteLanguage);
                        break;
                    case "5":
                        resultType = AnubisCst.Result.type.miysisPptx;
                        request.ExportType = GestionWeb.GetWebWord(WebCst.LanguageConstantes.ExportPptResult, webSession.SiteLanguage);
                        break;
                    case "6":
                        resultType = AnubisCst.Result.type.pachet;
                        request.ExportType = GestionWeb.GetWebWord(WebCst.LanguageConstantes.ExportSpotsResult, webSession.SiteLanguage);
                        break;
                    default:
                        resultType = AnubisCst.Result.type.miysis;
                        break;
                }

                List<int> sel = new List<int>();
                Int64 idStaticNavSession = 0;
                string zoomDate = string.Empty;

                try
                {
                    #region Process
                    if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Email) || request.FileName.Length == 0 || request.Email.Length == 0)
                    {
                        response.Message = GestionWeb.GetWebWord(WebCst.LanguageConstantes.AlertEmptyFields, webSession.SiteLanguage);
                    }
                    else if (webSession.CurrentModule == WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS && !string.IsNullOrEmpty(request.FileName) && request.FileName.Length > FileMaxLength)
                    {
                        response.Message = String.Format(GestionWeb.GetWebWord(WebCst.LanguageConstantes.MaxLengthExceeded, webSession.SiteLanguage), FileMaxLength.ToString());
                    }
                    else if (!IsValidEmail(request.Email))
                    {
                        response.Message = GestionWeb.GetWebWord(WebCst.LanguageConstantes.NotValidEmail, webSession.SiteLanguage);
                    }
                    else
                    {
                        webSession.ExportedPDFFileName = CheckedAccentText(request.FileName);
                        string[] mails = new string[1];
                        mails[0] = request.Email;
                        webSession.EmailRecipient = mails;
                        if (request.ExportType == GestionWeb.GetWebWord(WebCst.LanguageConstantes.ExportPdfResult, webSession.SiteLanguage))
                            request.ExportType = string.Empty;

                        switch (webSession.CurrentModule)
                        {
                            case WebCst.Module.Name.BILAN_CAMPAGNE:
                                idStaticNavSession = ExportResultsDAL.Save(webSession, AnubisCst.Result.type.mnevis);
                                break;
                            case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
                            case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
                            case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
                            case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                            case WebCst.Module.Name.ANALYSE_DES_PROGRAMMES:
                            case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
                            case WebCst.Module.Name.CELEBRITIES:

                                if (WebCst.Module.Name.ANALYSE_CONCURENTIELLE == webSession.CurrentModule && !string.IsNullOrEmpty(request.ExportType) && Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
                                {
                                    //TODO
                                    //CheckBoxList groupAdByCheckBoxList = askremoteexportwebControl1.GroupAdByCheckBoxList;
                                    //foreach (ListItem it in groupAdByCheckBoxList.Items)
                                    //{
                                    //    if (it.Selected) sel.Add(int.Parse(it.Value));
                                    //}
                                    if (sel.Count > 0)
                                        webSession.CreativesExportOptions = sel;
                                    idStaticNavSession = ExportResultsDAL.Save(webSession, AnubisCst.Result.type.dedoum);

                                }
                                else
                                {
                                    #region Classification Filter Init
                                    string id = string.Empty;
                                    string Level = string.Empty;
                                    //if (Page.Request.QueryString.Get("id") != null) id = Page.Request.QueryString.Get("id").ToString();
                                    //if (Page.Request.QueryString.Get("Level") != null) Level = Page.Request.QueryString.Get("Level").ToString();

                                    if (id.Length > 0 && Level.Length > 0)
                                    {
                                        SetProduct(int.Parse(id), int.Parse(Level), webSession);
                                    }
                                    #endregion

                                    #region Period Detail
                                    DateTime begin;
                                    DateTime end;
                                    if (!string.IsNullOrEmpty(zoomDate))
                                    {
                                        if (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.weekly)
                                        {
                                            begin = Dates.GetPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                            end = Dates.GetPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                        }
                                        else
                                        {
                                            begin = Dates.GetPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                            end = Dates.GetPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                        }
                                        begin = Dates.Max(begin,
                                            Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                                        end = Dates.Min(end,
                                            Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                                        webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                                    }
                                    else
                                    {
                                        begin = Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                                        end = Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
                                        if (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                                        {
                                            webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                                        }
                                    }
                                    webSession.PeriodBeginningDate = begin.ToString("yyyyMMdd");
                                    webSession.PeriodEndDate = end.ToString("yyyyMMdd");
                                    switch (webSession.PeriodType)
                                    {
                                        case ConstantesPeriod.Type.currentYear:
                                        case ConstantesPeriod.Type.dateToDateMonth:
                                        case ConstantesPeriod.Type.dateToDateWeek:
                                        case ConstantesPeriod.Type.LastLoadedMonth:
                                        case ConstantesPeriod.Type.LastLoadedWeek:
                                        case ConstantesPeriod.Type.nextToLastYear:
                                        case ConstantesPeriod.Type.nLastMonth:
                                        case ConstantesPeriod.Type.nLastWeek:
                                        case ConstantesPeriod.Type.nLastYear:
                                        case ConstantesPeriod.Type.previousWeek:
                                        case ConstantesPeriod.Type.previousYear:
                                            webSession.PeriodType = ConstantesPeriod.Type.dateToDate;
                                            break;
                                    }
                                    webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
                                    #endregion

                                    if (!string.IsNullOrEmpty(_idUnit))
                                        webSession.Units = new List<WebCst.CustomerSessions.Unit> {(WebCst.CustomerSessions.Unit)int.Parse(_idUnit)};

                                    idStaticNavSession = (webSession.CurrentModule == WebCst.Module.Name.CELEBRITIES) ? ExportResultsDAL.Save(webSession, AnubisCst.Result.type.apis) :
                                        ExportResultsDAL.Save(webSession, resultType);
                                }
                                break;

                            case WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS:
                                idStaticNavSession = ExportResultsDAL.Save(webSession, AnubisCst.Result.type.pachet);
                                break;
                            case WebCst.Module.Name.INDICATEUR:
                                idStaticNavSession = ExportResultsDAL.Save(webSession, AnubisCst.Result.type.hotep);
                                break;

                            //case WebCst.Module.Name.VP:
                            //    if (!string.IsNullOrEmpty(request.ExportType))
                            //    {
                            //        if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode())
                            //        {
                            //            if (!string.IsNullOrEmpty(_idDataPromotion)) webSession.IdPromotion = long.Parse(_idDataPromotion);
                            //            idStaticNavSession = ExportResultsDAL.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.selket);
                            //        }
                            //        else if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.thoueris.GetHashCode())
                            //        {
                            //            idStaticNavSession = ExportResultsDAL.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.thoueris);
                            //        }
                            //    }
                            //    break;

                            default:
                                throw new Exception(" Impossssile d'identifier le module.");

                        }
                    }
                    #endregion
                    response.Success = true;
                    response.Message = GestionWeb.GetWebWord(3251, webSession.SiteLanguage);
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType() != typeof(System.Threading.ThreadAbortException))
                    {
                        throw (ex);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return response;
        }
        private bool IsValidEmail(string email)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(email))
            {
                Regex regex = new Regex(@"^[\w_.~-]+@[\w][\w.\-]*[\w]\.[\w][\w.]*[a-zA-Z]$");
                result = regex.IsMatch(email);
            }
            return result;
        }
        private string CheckedAccentText(string text)
        {
            text = text.TrimEnd();
            text = text.TrimStart();
            text = Regex.Replace(text, "[']", "''");
            return text;
        }
        private void SetProduct(int id, int level, WebSession webSession)
        {
            var currentLevel = (DetailLevelItemInformation.Levels)webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level);
            SetSessionProductDetailLevel(webSession, id, currentLevel);
        }
        private static void SetSessionProductDetailLevel(WebSession webSession, int id, DetailLevelItemInformation.Levels level)
        {
            var tree = new System.Windows.Forms.TreeNode();
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[WebCst.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            var param = new object[2];
            param[0] = webSession.CustomerDataFilters.DataSource;
            param[1] = webSession.DataLanguage;
            var factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            ClassificationLevelListDAL levels = null;

            switch (level)
            {
                case DetailLevelItemInformation.Levels.sector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.sectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.sectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.sector, tree);
                    break;
                case DetailLevelItemInformation.Levels.subSector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.subSectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.subSectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.subsector, tree);
                    break;
                case DetailLevelItemInformation.Levels.@group:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.groupAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.groupAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.@group, tree);
                    break;
                case DetailLevelItemInformation.Levels.segment:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.segmentAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.segmentAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.segment, tree);
                    break;
                case DetailLevelItemInformation.Levels.product:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.productAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.productAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.product, tree);
                    break;
                case DetailLevelItemInformation.Levels.advertiser:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.advertiserAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.advertiserAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.advertiser, tree);
                    break;
                case DetailLevelItemInformation.Levels.brand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.brandAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.brandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.brand, tree);
                    break;
                case DetailLevelItemInformation.Levels.holdingCompany:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.holdingCompanyAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.holdingCompanyAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.holding_company, tree);
                    break;
                case DetailLevelItemInformation.Levels.subBrand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(Right.type.subBrandAccess, id.ToString());
                    tree.Tag = new LevelInformation(Right.type.subBrandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(Level.type.subBrand, tree);
                    break;
            }
        }
    }
}
