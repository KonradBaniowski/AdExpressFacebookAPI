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

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class RightService : IRightService
    {
        private WebSession webSession = null;

        public Dictionary<long, Module> GetModule(string idWSession)
        {
            webSession = (WebSession)WebSession.Load(idWSession);

            var stuff = webSession.CustomerLogin.GetModuleRights();
            var dico = new Dictionary<long, Module>();
            foreach (var item in stuff)
            {
                if (item.Value != null)
                {
                    var module = new Module
                    {
                        NextUrl = stuff[item.Key].UrlNextPage,
                        Title = GestionWeb.GetWebWord(stuff[item.Key].IdWebText, webSession.SiteLanguage),
                        Description = GestionWeb.GetWebWord(stuff[item.Key].DescriptionWebTextId, webSession.SiteLanguage)
                    };
                    dico.Add(item.Key, module);
                }
                else
                    dico.Add(item.Key, null);
            }
            return dico;
        }
    }
}
