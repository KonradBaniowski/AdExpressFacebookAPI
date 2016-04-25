using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.DataAccess.IdentityImpl;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using ModuleName = TNS.AdExpress.Constantes.Web.Module.Name;

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
    }
}
