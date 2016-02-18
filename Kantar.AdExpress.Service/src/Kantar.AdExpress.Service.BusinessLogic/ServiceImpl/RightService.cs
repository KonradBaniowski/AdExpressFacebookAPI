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

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class RightService : IRightService
    {
        private WebSession webSession = null;

        public Dictionary<long, Core.Domain.Module> GetModules(string idWSession)
        {
            webSession = (WebSession)WebSession.Load(idWSession);

            var stuff = webSession.CustomerLogin.GetModuleRights();
            var dico = new Dictionary<long, Core.Domain.Module>();
            foreach (var item in stuff)
            {
                //if (item.Value != null)
                //{
                //    var module = new Core.Domain.Module
                //    {
                //        NextUrl = stuff[item.Key].UrlNextPage,
                //        Title = GestionWeb.GetWebWord(stuff[item.Key].IdWebText, webSession.SiteLanguage),
                //        Description = GestionWeb.GetWebWord(stuff[item.Key].DescriptionWebTextId, webSession.SiteLanguage)
                //    };
                //    dico.Add(item.Key, module);
                //}
                //else
                    dico.Add(item.Key, null);
            }
            return dico;
        }

        public Dictionary<long, Core.Domain.Module> GetModulesList(string idWSession)
        {
            //TODO
            webSession = (WebSession)WebSession.Load(idWSession);

            Dictionary<long, Core.Domain.Module> modules = new Dictionary<long, Core.Domain.Module>();
            var res = ModulesList.GetModules();
            modules = res.ToDictionary(kvp => kvp.Key, kvp => new Core.Domain.Module()
            {
                Description = GestionWeb.GetWebWord(kvp.Value.DescriptionWebTextId, webSession.SiteLanguage),
                Title = GestionWeb.GetWebWord(kvp.Value.IdWebText, webSession.SiteLanguage),
                NextUrl = kvp.Value.UrlNextPage
            });
            return modules;
        }
    }
}
