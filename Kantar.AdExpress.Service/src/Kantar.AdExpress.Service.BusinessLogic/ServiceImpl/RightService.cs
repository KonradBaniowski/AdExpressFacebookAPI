using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using NLog;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class RightService : IRightService
    {
        private WebSession webSession = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Dictionary<long, Core.Domain.Module> GetModules(string idWSession)
        {
            webSession = (WebSession)WebSession.Load(idWSession);
            var result = new Dictionary<long, Core.Domain.Module>();
            try
            {
                var stuff = webSession.CustomerLogin.GetModuleRights();

                foreach (var item in stuff)
                {
                    result.Add(item.Key, null);
                }
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", webSession.IdSession, webSession.UserAgent, webSession.CustomerLogin.Login, webSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            return result;
        }

        public Dictionary<long, Core.Domain.Module> GetModulesList(string idWSession)
        {
            //TODO
            webSession = (WebSession)WebSession.Load(idWSession);
            Dictionary<long, Core.Domain.Module> modules = new Dictionary<long, Core.Domain.Module>();
            try
            {
                var res = ModulesList.GetModules();
                modules = res.ToDictionary(kvp => kvp.Key, kvp => new Core.Domain.Module()
                {
                    Description = GestionWeb.GetWebWord(kvp.Value.DescriptionWebTextId, webSession.SiteLanguage),
                    Title = GestionWeb.GetWebWord(kvp.Value.IdWebText, webSession.SiteLanguage),
                    NextUrl = kvp.Value.UrlNextPage,
                    SiteLanguage = webSession.SiteLanguage
                });
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", webSession.IdSession, webSession.UserAgent, webSession.CustomerLogin.Login, webSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            
            return modules;
        }
    }
}
