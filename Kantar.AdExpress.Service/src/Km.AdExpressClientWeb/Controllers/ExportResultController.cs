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
using Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.I18n;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class ExportResultController : Controller
    {
        private IWebSessionService _webSessionService;
        private IExportService _exportService;
        private IDetailSelectionService _detailSelectionService;
        public ExportResultController(IWebSessionService webSessionService, IExportService exportService,IDetailSelectionService detailSelectionService)
        {
            _webSessionService = webSessionService;
            _exportService = exportService;
            _detailSelectionService = detailSelectionService;
        }
        public ActionResult CreateExport(string exportType)
        {
            var cp = new ClaimsPrincipal(User.Identity);
            var idWebSession = cp.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var siteLanguage = _webSessionService.GetSiteLanguage(idWebSession);
            var model = new ExportResultModel
            {
                RememberEmail = false,
                Labels= LabelsHelper.LoadPageLabels(siteLanguage),
                ExportType = (exportType=="4")? GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage)
                                                : GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage)
            };
            return PartialView("ExportResult", model);
        }

        public JsonResult Export( string fileName, string email, string type)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var webSessionId = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var request = new ExportRequest
            {
                FileName =fileName??string.Empty,
                Email =email??string.Empty,
                ExportType =type,
                WebSessionId = webSessionId
            };            
            var response = _exportService.Export(request);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}