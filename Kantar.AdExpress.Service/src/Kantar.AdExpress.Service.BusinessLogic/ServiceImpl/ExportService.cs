using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebCst = TNS.AdExpress.Constantes.Web;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class ExportService :IExportService
    {
        private const int FileMaxLength = 80;
        protected string StartDate = string.Empty;
        protected string EndDate = string.Empty;
        private string _idUnit = string.Empty;
        public ExportResponse Export (ExportRequest request)
        {
            ExportResponse response = new ExportResponse
            {
                Message= "This feature is still under construction."
            };
            #region Validation
            //var webSession = (WebSession)WebSession.Load(request.WebSessionId);
            ////string fileName = askremoteexportwebControl1.TbxFileName.Text;
            ////string mail = askremoteexportwebControl1.TbxMail.Text;
            //List<int> sel = new List<int>();
            //Int64 idStaticNavSession = 0;
            //string zoomDate = string.Empty;

            //try
            //{

            //    if (string.IsNullOrEmpty(request.FileName)|| string.IsNullOrEmpty(request.Email)|| request.FileName.Length == 0 || request.Email.Length == 0)
            //    {
            //       response.Message = GestionWeb.GetWebWord(LanguageConstantes.AlertEmptyFields, webSession.SiteLanguage);
            //    }
            //    else if (webSession.CurrentModule == WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS && !string.IsNullOrEmpty(request.FileName) && request.FileName.Length > FileMaxLength)
            //    {
            //        response.Message = String.Format(GestionWeb.GetWebWord(LanguageConstantes.MaxLengthExceeded, webSession.SiteLanguage), FileMaxLength.ToString());
            //    }
            //    else if (!IsValidEmail(request.Email))
            //    {
            //        response.Message = GestionWeb.GetWebWord(LanguageConstantes.NotValidEmail, webSession.SiteLanguage);
            //    }
            //    else
            //    {

            //        #region Gestion des cookies

            //        #region Cookies enregistrement des préférences

            //        //Vérifie si le navigateur accepte les cookies
            //        //if (Request.Browser.Cookies)
            //        //{
            //        //    WebFunctions.Cookies.SaveEmailForRemotingExport(Page, mail, askremoteexportwebControl1.CbxRegisterMail);//cbxRegisterMail
            //        //}
            //        #endregion

            //        #endregion

            //        webSession.ExportedPDFFileName = CheckedAccentText(request.FileName);
            //        string[] mails = new string[1];
            //        mails[0] = request.Email;
            //        webSession.EmailRecipient = mails;
            //        if (request.ExportType == GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, webSession.SiteLanguage))
            //            request.ExportType = string.Empty;


            //        switch (webSession.CurrentModule)
            //        {

            //            case WebCst.Module.Name.BILAN_CAMPAGNE:
            //                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.mnevis);
            //                break;
            //            //Plan média
            //            case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
            //            case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
            //            case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
            //            case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
            //            case WebCst.Module.Name.ANALYSE_DES_PROGRAMMES:
            //            case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
            //            case WebCst.Module.Name.CELEBRITIES:
                            
            //                if (WebCst.Module.Name.ANALYSE_CONCURENTIELLE == webSession.CurrentModule && !string.IsNullOrEmpty(request.ExportType) && Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            //                {
            //                    //TODO
            //                    //CheckBoxList groupAdByCheckBoxList = askremoteexportwebControl1.GroupAdByCheckBoxList;
            //                    //foreach (ListItem it in groupAdByCheckBoxList.Items)
            //                    //{
            //                    //    if (it.Selected) sel.Add(int.Parse(it.Value));
            //                    //}
            //                    if (sel.Count > 0)
            //                        webSession.CreativesExportOptions = sel;
            //                    idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.dedoum);

            //                }
            //                else
            //                {
            //                    #region Classification Filter Init
            //                    string id = "";
            //                    string Level = "";
            //                    //if (Page.Request.QueryString.Get("id") != null) id = Page.Request.QueryString.Get("id").ToString();
            //                    //if (Page.Request.QueryString.Get("Level") != null) Level = Page.Request.QueryString.Get("Level").ToString();

            //                    if (id.Length > 0 && Level.Length > 0)
            //                    {
            //                        SetProduct(int.Parse(id), int.Parse(Level));
            //                    }
            //                    #endregion

            //                    #region Period Detail
            //                    DateTime begin;
            //                    DateTime end;
            //                    if (zoomDate != null && zoomDate != string.Empty)
            //                    {
            //                        if (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.weekly)
            //                        {
            //                            begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
            //                            end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
            //                        }
            //                        else
            //                        {
            //                            begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
            //                            end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
            //                        }
            //                        begin = WebFunctions.Dates.Max(begin,
            //                            WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
            //                        end = WebFunctions.Dates.Min(end,
            //                            WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

            //                        webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
            //                    }
            //                    else
            //                    {
            //                        begin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
            //                        end = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
            //                        if (webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            //                        {
            //                            webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
            //                        }
            //                    }
            //                    webSession.PeriodBeginningDate = begin.ToString("yyyyMMdd");
            //                    webSession.PeriodEndDate = end.ToString("yyyyMMdd");
            //                    switch (webSession.PeriodType)
            //                    {
            //                        case ConstantesPeriod.Type.currentYear:
            //                        case ConstantesPeriod.Type.dateToDateMonth:
            //                        case ConstantesPeriod.Type.dateToDateWeek:
            //                        case ConstantesPeriod.Type.LastLoadedMonth:
            //                        case ConstantesPeriod.Type.LastLoadedWeek:
            //                        case ConstantesPeriod.Type.nextToLastYear:
            //                        case ConstantesPeriod.Type.nLastMonth:
            //                        case ConstantesPeriod.Type.nLastWeek:
            //                        case ConstantesPeriod.Type.nLastYear:
            //                        case ConstantesPeriod.Type.previousWeek:
            //                        case ConstantesPeriod.Type.previousYear:
            //                            webSession.PeriodType = ConstantesPeriod.Type.dateToDate;
            //                            break;
            //                    }
            //                    webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
            //                    #endregion

            //                    if (!string.IsNullOrEmpty(_idUnit))
            //                        webSession.Unit = (CustomerSessions.Unit)int.Parse(_idUnit);

            //                    idStaticNavSession = (webSession.CurrentModule == WebCst.Module.Name.CELEBRITIES) ? TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.apis) :
            //                        TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.miysis);
            //                }
            //                break;

            //            case WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS:
            //                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.pachet);
            //                break;
            //            case WebCst.Module.Name.INDICATEUR:
            //                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.hotep);
            //                break;
            //            case WebCst.Module.Name.JUSTIFICATIFS_PRESSE:
            //                var pDetail = new ProofDetail(webSession, _idMedia, _idProduct, _dateCover, _dateParution, _pageNumber);
            //                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(pDetail, TNS.AdExpress.Anubis.Constantes.Result.type.shou, webSession.ExportedPDFFileName);
            //                break;
            //            case WebCst.Module.Name.DONNEES_DE_CADRAGE:
            //                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.aton);
            //                break;
            //            case WebCst.Module.Name.VP:
            //                if (!string.IsNullOrEmpty(request.ExportType))
            //                {
            //                    if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode())
            //                    {
            //                        if (!string.IsNullOrEmpty(_idDataPromotion)) webSession.IdPromotion = long.Parse(_idDataPromotion);
            //                        idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.selket);
            //                    }
            //                    else if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.thoueris.GetHashCode())
            //                    {
            //                        idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.thoueris);
            //                    }
            //                }
            //                break;
            //            case WebCst.Module.Name.ROLEX:
            //                if (!string.IsNullOrEmpty(request.ExportType))
            //                {
            //                    if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.ptah.GetHashCode())
            //                    {
            //                        if (!string.IsNullOrEmpty(_levelsValue)) webSession.SelectedLevelsValue = new List<string>(_levelsValue.Split(',')).ConvertAll(Convert.ToInt64);
            //                        webSession.DetailPeriodBeginningDate = StartDate;
            //                        webSession.DetailPeriodEndDate = EndDate;
            //                        TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.ptah);
            //                    }
            //                    else if (Convert.ToInt32(request.ExportType) == TNS.AdExpress.Anubis.Constantes.Result.type.amon.GetHashCode())
            //                    {
            //                        TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(webSession, TNS.AdExpress.Anubis.Constantes.Result.type.amon);
            //                    }
            //                }
            //                break;
            //                //default :
            //                //throw new AdExpress. Exceptions.PdfSavePopUpException(" Impossssile d'identifier le module.");

            //        }


            //        closeRollOverWebControl_Click(this, null);

            //    }
            //}
            //catch (System.Exception exc)
            //{
            //    if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
            //    {
            //        this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession));
            //    }
            //}
            #endregion
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
    }
}
