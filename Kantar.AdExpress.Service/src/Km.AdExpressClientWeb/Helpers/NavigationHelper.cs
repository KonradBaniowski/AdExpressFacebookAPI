using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

namespace Km.AdExpressClientWeb.Helpers
{
    public class NavigationHelper
    {
        public List<NavigationNode> LoadNavBar(string idWebSession, string controller, int siteLanguage = 33)
        {

            var model = new List<NavigationNode>();
            var webSession = (WebSession)WebSession.Load(idWebSession);
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new NavigationNode
            {
                Id = 1,
                IsActive = webSession.IsCurrentUniversProductSelected(),
                Description = "Market",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Market, siteLanguage),
                Action = "Index",
                Controller = controller,
                IconCssClass = "fa fa-file-text"
            };
            model.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = webSession.isMediaSelected(),
                Description = "Media",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Media, siteLanguage),
                Action = "MediaSelection",
                Controller = controller,
                IconCssClass = "fa fa-eye"
            };
            model.Add(media);
            var dates = new NavigationNode
            {
                Id = 3,
                IsActive = webSession.isDatesSelected(),
                Description = "Dates",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Dates, siteLanguage),
                Action = "PeriodSelection",
                Controller = controller,
                IconCssClass = "fa fa-calendar"
            };
            model.Add(dates);
            var result = new NavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Results",
                Title = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Action = "Results",
                Controller = controller,
                IconCssClass = "fa fa-check"
            };
            model.Add(result);
            #endregion
            return model;
        }

        public static string GetSiteLanguageName(int siteLanguage)
        {
            switch (siteLanguage)
            {
                case 44: return "EN";
                default: return "FR";
            }
        }
    }
}