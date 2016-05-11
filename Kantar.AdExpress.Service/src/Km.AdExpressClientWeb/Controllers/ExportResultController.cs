using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using KM.Framework.Constantes;
using TNS.AdExpress.Constantes.Web;
using System.Text.RegularExpressions;
using TNS.AdExpress.Domain.Translation;

namespace Km.AdExpressClientWeb.Controllers
{
    public class ExportResultController : Controller
    {
        private IWebSessionService _webSessionService;
        public ExportResultController(IWebSessionService webSessionService)
        {
            _webSessionService = webSessionService;
        }
        public ActionResult CreateExport(string exportType)
        {
            var cp = new ClaimsPrincipal(User.Identity);
            var idWebSession = cp.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var siteLanguage = _webSessionService.GetSiteLanguage(idWebSession);
            var model = new ExportResultModel
            {
                Labels= LoadPageLabels(siteLanguage),
                ExportType = (exportType=="4")? GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage)
                                                : GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage)
            };
            return PartialView("ExportResult", model);
        }

        private Labels LoadPageLabels(int siteLanguage)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"(<br />|<br/>|</ br>|</br>)");
            
            var result = new Labels
            {
                Export =GestionWeb.GetWebWord(LanguageConstantes.Export,siteLanguage),
                FileName = GestionWeb.GetWebWord(LanguageConstantes.FileName, siteLanguage),
                Email = GestionWeb.GetWebWord(LanguageConstantes.MailCode, siteLanguage),
                Submit = GestionWeb.GetWebWord(LanguageConstantes.Submit, siteLanguage),
                Close = GestionWeb.GetWebWord(LanguageConstantes.Close, siteLanguage)
            };
            return result;
        }
    }
}