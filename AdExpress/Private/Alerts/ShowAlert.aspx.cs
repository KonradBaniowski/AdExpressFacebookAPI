using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using TNS.Alert.Domain;
using TNS.Ares.Alerts;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;

using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.Web;

using ModuleName = TNS.AdExpress.Constantes.Web.Module.Name;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.Layers;

public partial class Private_Alerts_ShowAlert : TNS.AdExpress.Web.UI.PrivateWebPage
{
    #region Variables

    IAlertDAL alertDAL = null;

    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        try {
            int idOccurrence = -1;
            int idAlert = -1;
            string stringIdOccurrence = Request.QueryString["idOcc"];
            string idSession = "";

            // Loading alert data access layer
            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
            TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
            alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null, null);

            if (stringIdOccurrence != null && int.TryParse(stringIdOccurrence, out idOccurrence)) {
                // Checking if the query string contains information
                // about an alert
                if (Request.QueryString["idAlert"] == null || int.TryParse(Request.QueryString["idAlert"], out idAlert) == false)
                    idAlert = -1;

                AlertOccurence occ = alertDAL.GetOccurrence(idOccurrence, idAlert);
                if (occ != null) {
                    Alert alert = alertDAL.GetAlert(occ.AlertId);
                    if (alert != null) {
                        WebSession session = (WebSession)alert.Session;
                        if (Request.QueryString["idSession"] != null)
                            session.IdSession = Request.QueryString["idSession"];


                        if (_webSession == null){
                            TNS.AdExpress.Right newRight = new TNS.AdExpress.Right(session.CustomerLogin.Login, session.CustomerLogin.PassWord, session.SiteLanguage);
                            if (!newRight.CanAccessToAdExpress())
                            {
                                //Redirect vers page index
                                string redirectUrl = "/index.aspx?";
                                if (idAlert > 0)
                                    redirectUrl += "idAlert=" + idAlert;
                                if (stringIdOccurrence != null)
                                    redirectUrl += "&idOcc=" + stringIdOccurrence;
                                HttpContext.Current.Response.Redirect(redirectUrl);
                            }
                            else
                            {
                                newRight.SetModuleRights();
                                newRight.SetFlagsRights();
                                newRight.SetRights();
                            }
                            _webSession = new WebSession(newRight);
                        }


                        TNS.AdExpress.Right CustomerLogin = _webSession.CustomerLogin;
                         idSession = _webSession.IdSession;
                        _webSession = session;
                        _webSession.CustomerLogin = CustomerLogin;
                        _webSession.Source = _webSession.CustomerLogin.Source;
                        _siteLanguage = _webSession.SiteLanguage;
                        _webSession.IdSession = idSession;


                        // Opening connection
                        _webSession.Source.Open();

                        TNS.AdExpress.Domain.Web.Navigation.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                        ResultPageInformation info = module.GetResultPageInformation(_webSession.CurrentTab);

                        // Updating session information
                        DateTime FirstDayNotEnable = DateTime.Now;
                        if (_webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.JUSTIFICATIFS_PRESSE)
                            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                            {
                                try {
                                    int oldYear = 2000;
                                    long selectedVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                                    FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(_webSession, selectedVehicle, oldYear, _webSession.Source);
                                }
                                catch { }
                            }

                        switch (_webSession.CurrentModule)
                        {
                            case ModuleName.INDICATEUR:
                            case ModuleName.TABLEAU_DYNAMIQUE:
                                _webSession.PeriodType = CustomerSessions.Period.Type.dateToDateMonth;
                                _webSession.PeriodBeginningDate = occ.DateBeginStudy.ToString("yyyyMM");
                                _webSession.PeriodEndDate = occ.DateEndStudy.ToString("yyyyMM");
                                break;
                            case ModuleName.ANALYSE_PLAN_MEDIA:
                            case ModuleName.ANALYSE_DYNAMIQUE:
                            case ModuleName.ANALYSE_CONCURENTIELLE:
                            case ModuleName.JUSTIFICATIFS_PRESSE:
                            case ModuleName.ANALYSE_PORTEFEUILLE:
                            case ModuleName.TENDACES:
                            case ModuleName.ANALYSE_DES_DISPOSITIFS:
                            case ModuleName.ANALYSE_DES_PROGRAMMES:
                            case ModuleName.NEW_CREATIVES:
                            case ModuleName.ANALYSE_MANDATAIRES:
                                _webSession.PeriodType = CustomerSessions.Period.Type.dateToDate;
                                _webSession.PeriodBeginningDate = occ.DateBeginStudy.ToString("yyyyMMdd");
                                _webSession.PeriodEndDate = occ.DateEndStudy.ToString("yyyyMMdd");
                                break;
                            case ModuleName.BILAN_CAMPAGNE:
                            case ModuleName.DONNEES_DE_CADRAGE:
                                TNS.FrameWork.Date.AtomicPeriodWeek dateBegin = new TNS.FrameWork.Date.AtomicPeriodWeek(occ.DateBeginStudy);
                                TNS.FrameWork.Date.AtomicPeriodWeek dateEnd = new TNS.FrameWork.Date.AtomicPeriodWeek(occ.DateEndStudy);
                                _webSession.PeriodType = CustomerSessions.Period.Type.dateToDateWeek;
                                _webSession.PeriodBeginningDate = TNS.FrameWork.Date.DateString.AtomicPeriodWeekToYYYYWW(dateBegin);
                                _webSession.PeriodEndDate = TNS.FrameWork.Date.DateString.AtomicPeriodWeekToYYYYWW(dateEnd);
                                break;
                        }

                        switch (_webSession.CurrentModule)
                        {
                            case ModuleName.ANALYSE_DYNAMIQUE:
                            case ModuleName.ANALYSE_PLAN_MEDIA:
                            case ModuleName.ANALYSE_CONCURENTIELLE:
                            case ModuleName.ANALYSE_PORTEFEUILLE:
                            case ModuleName.NEW_CREATIVES:
                            case ModuleName.ANALYSE_MANDATAIRES:
                                // Updated customer period (module use globalCalendar)
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(occ.DateBeginStudy.ToString("yyyyMMdd"), occ.DateEndStudy.ToString("yyyyMMdd"),
                                                                                                       _webSession.CustomerPeriodSelected.WithComparativePeriod, _webSession.CustomerPeriodSelected.ComparativePeriodType,
                                                                                                       _webSession.CustomerPeriodSelected.PeriodDisponibilityType);
                                break;
                            default:
                                break;
                        }

                        _webSession.Save();

                        // Adding an alert auto connect cookie
                        if (Request.Cookies[Cookies.AlertAutoConnectCookie] == null) {
                            HttpCookie autoReconnect = new HttpCookie(Cookies.AlertAutoConnectCookie);
                            autoReconnect.Expires = DateTime.MaxValue;
                            Response.Cookies.Add(autoReconnect);
                        }

                        Response.Redirect(info.Url + "?idSession=" + _webSession.IdSession.ToString());
                    }
                }
            }
            else
                Response.Redirect("/Private/Alerts/ShowAlerts.aspx?idSession=" + Request.QueryString["idSession"].ToString());
        }
        catch (System.Exception exc) {
            if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
        }
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
