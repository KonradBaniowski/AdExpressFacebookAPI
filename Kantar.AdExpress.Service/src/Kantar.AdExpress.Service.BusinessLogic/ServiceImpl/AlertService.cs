using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.DataAccess.IdentityImpl;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using FrameworkDB = TNS.FrameWork.DB.Common;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using ModuleName = TNS.AdExpress.Constantes.Web.Module.Name;
using TNS.Ares.Constantes;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class AlertService : IAlertService
    {
        private IApplicationUserManager _userManager;

        public AlertService(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public string GetRedirectUrl(WebSession session, string idSession, AlertOccurence occ)
        {
            string redirectUrl = string.Empty;

            try
            {
                WebSession webSession = null;

                if (!string.IsNullOrEmpty(idSession))
                    session.IdSession = idSession;

                TNS.AdExpress.Right newRight = new TNS.AdExpress.Right(session.CustomerLogin.Login, session.CustomerLogin.PassWord, session.SiteLanguage);
                if (!newRight.CanAccessToAdExpress())
                {
                    // Todo Redirect vers page index
                    return "Home";
                }
                else
                {
                    newRight.SetModuleRights();
                    newRight.SetFlagsRights();
                    newRight.SetRights();
                    if (WebApplicationParameters.VehiclesFormatInformation.Use)
                        newRight.SetBannersAssignement();
                }
                webSession = new WebSession(newRight);

                if (!string.IsNullOrEmpty(idSession))
                    webSession.IdSession = idSession;

                TNS.AdExpress.Right CustomerLogin = webSession.CustomerLogin;
                idSession = webSession.IdSession;
                webSession = session;
                webSession.CustomerLogin = CustomerLogin;
                webSession.Source = webSession.CustomerLogin.Source;
                //_siteLanguage = webSession.SiteLanguage;
                webSession.IdSession = idSession;

                // Opening connection
                webSession.Source.Open();

                TNS.AdExpress.Domain.Web.Navigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

                // Updating session information
                switch (webSession.CurrentModule)
                {
                    case ModuleName.INDICATEUR:
                    case ModuleName.TABLEAU_DYNAMIQUE:
                        webSession.PeriodType = CustomerSessions.Period.Type.dateToDateMonth;
                        webSession.PeriodBeginningDate = occ.DateBeginStudy.ToString("yyyyMM");
                        webSession.PeriodEndDate = occ.DateEndStudy.ToString("yyyyMM");
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
                        webSession.PeriodType = CustomerSessions.Period.Type.dateToDate;
                        webSession.PeriodBeginningDate = occ.DateBeginStudy.ToString("yyyyMMdd");
                        webSession.PeriodEndDate = occ.DateEndStudy.ToString("yyyyMMdd");
                        break;
                    case ModuleName.BILAN_CAMPAGNE:
                    case ModuleName.DONNEES_DE_CADRAGE:
                        TNS.FrameWork.Date.AtomicPeriodWeek dateBegin = new TNS.FrameWork.Date.AtomicPeriodWeek(occ.DateBeginStudy);
                        TNS.FrameWork.Date.AtomicPeriodWeek dateEnd = new TNS.FrameWork.Date.AtomicPeriodWeek(occ.DateEndStudy);
                        webSession.PeriodType = CustomerSessions.Period.Type.dateToDateWeek;
                        webSession.PeriodBeginningDate = TNS.FrameWork.Date.DateString.AtomicPeriodWeekToYYYYWW(dateBegin);
                        webSession.PeriodEndDate = TNS.FrameWork.Date.DateString.AtomicPeriodWeekToYYYYWW(dateEnd);
                        break;
                }

                switch (webSession.CurrentModule)
                {
                    case ModuleName.ANALYSE_DYNAMIQUE:
                    case ModuleName.ANALYSE_PLAN_MEDIA:
                    case ModuleName.ANALYSE_CONCURENTIELLE:
                    case ModuleName.ANALYSE_PORTEFEUILLE:
                    case ModuleName.NEW_CREATIVES:
                    case ModuleName.ANALYSE_MANDATAIRES:
                        // Updated customer period (module use globalCalendar)
                        //TODO: Vérifier pour fusion Dev=>Trunk
                        if (session.CustomerPeriodSelected.WithComparativePeriodPersonnalized)
                        {
                            session.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(occ.DateBeginStudy.ToString("yyyyMMdd"), occ.DateEndStudy.ToString("yyyyMMdd"),
                                                                session.CustomerPeriodSelected.ComparativeStartDate, session.CustomerPeriodSelected.ComparativeEndDate);
                        }
                        else
                        {
                            session.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(occ.DateBeginStudy.ToString("yyyyMMdd"), occ.DateEndStudy.ToString("yyyyMMdd"),
                                                                                                    session.CustomerPeriodSelected.WithComparativePeriod, session.CustomerPeriodSelected.ComparativePeriodType,
                                                                                                session.CustomerPeriodSelected.PeriodDisponibilityType);
                        }
                        break;
                    default:
                        break;
                }

                webSession.Save();

                switch (module.Id)
                {
                    case ModuleName.ANALYSE_PLAN_MEDIA:
                        redirectUrl = "MediaSchedule";
                        break;
                    case ModuleName.ANALYSE_PORTEFEUILLE:
                        redirectUrl = "Portfolio";
                        break;
                    case ModuleName.ANALYSE_CONCURENTIELLE:
                        redirectUrl = "PresentAbsent";
                        break;
                    case ModuleName.ANALYSE_DYNAMIQUE:
                        redirectUrl = "LostWon";
                        break;
                };
            }
            catch (System.Exception exc)
            {
                //Todo throw exception
            }

            return redirectUrl;
        }

        public SaveAlertResponse SaveAlert(SaveAlertRequest request)
        {
            var response = new SaveAlertResponse();
            var webSession = (WebSession)WebSession.Load(request.IdWebSession);
            if (ValidateFields(request))
            {
                if(!IsValidEmail(request.Email))
                {
                    response.Message = GestionWeb.GetWebWord(LanguageConstantes.NotValidEmail, webSession.SiteLanguage);
                    return response;
                }
                else
                {
                    response = SaveAlertData(request,webSession);
                }
            }
            else
            {
                response.Message = GestionWeb.GetWebWord(LanguageConstantes.AlertEmptyFields, webSession.SiteLanguage);
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

        private SaveAlertResponse SaveAlertData (SaveAlertRequest request, WebSession webSession)
        {
            #region Init
            var result = new SaveAlertResponse();
            webSession.ExportedPDFFileName = request.AlertTitle;
            string[] mails = new string[1];
            mails[0] = request.Email;
            webSession.EmailRecipient = mails;
            int occurrenceDate = -1;
            #endregion

            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
            FrameworkDB.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
            IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
            if (alertDAL.GetAlerts(webSession.CustomerLogin.IdLogin).Count >= webSession.CustomerLogin.GetNbAlertsAdExpress())
            {
                result.Message = GestionWeb.GetWebWord(LanguageConstantes.MaxAlertLimit, webSession.SiteLanguage);
                return result;
            }
            else
            {
                occurrenceDate = (request.Type == Constantes.Alerts.AlertPeriodicity.Monthly || request.Type == Constantes.Alerts.AlertPeriodicity.Weekly) ?int.Parse(request.OccurrenceDate):-1;
                Int64 idAlertSchedule = 0;
                AlertHourCollection alertHourCollection = alertDAL.GetAlertHours();
                for (int i = 0; i < alertHourCollection.Count; i++)
                {
                    if (alertHourCollection[i].HoursSchedule.Ticks == (new TimeSpan(18, 0, 0)).Ticks)
                    {
                        idAlertSchedule = alertHourCollection[i].IdAlertSchedule;
                    }
                }
                //var periodicityType = (TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity)Enum.Parse(typeof(TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity), request.Type);
                alertDAL.InsertAlertData(request.AlertTitle, webSession.ToBinaryData(), webSession.CurrentModule,request.Type,
                                               int.Parse(request.OccurrenceDate), request.Email, webSession.CustomerLogin.IdLogin, idAlertSchedule);
            }
            return result;
        }       

        private bool ValidateFields (SaveAlertRequest request)
        {
            bool result = (!String.IsNullOrEmpty(request.AlertTitle) && !String.IsNullOrEmpty(request.Email))? true :false;
            if (request.Type == Constantes.Alerts.AlertPeriodicity.Monthly || request.Type == Constantes.Alerts.AlertPeriodicity.Weekly)
                result = result && !String.IsNullOrEmpty(request.OccurrenceDate);
            return result;
        }
    }
}
