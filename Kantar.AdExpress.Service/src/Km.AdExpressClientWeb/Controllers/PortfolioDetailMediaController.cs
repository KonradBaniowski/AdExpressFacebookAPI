using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using VM = Km.AdExpressClientWeb.Models.MediaSchedule;
using KM.AdExpress.Framework.MediaSelection;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core;
using AutoMapper;
using Newtonsoft.Json;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using TNS.AdExpress.Domain.Web;
using Km.AdExpressClientWeb.Helpers;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class PortfolioDetailMediaController : Controller
    {
        private IWebSessionService _webSessionService;
        private IPortfolioDetailMediaService _portfolioDetailMediaService;
        private const string _controller = "PortfolioDetailMedia";

        public PortfolioDetailMediaController(IWebSessionService webSessionService, IPortfolioDetailMediaService portfolioDetailMediaService)
        {
            _webSessionService = webSessionService;
            _portfolioDetailMediaService = portfolioDetailMediaService;
        }

        // GET: PortfolioDetailMedia
        public ActionResult Index(string idMedia, string dayOfWeek, string ecran)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            WebSession customerSession = (WebSession)WebSession.Load(idWebSession);

            var model = new InsertionCreativeViewModel()
            {
                paramsUrl = new List<string>(),
                SiteLanguage = WebApplicationParameters.DefaultLanguage, // Default
            };

            model.paramsUrl.Add(idMedia);
            model.paramsUrl.Add(dayOfWeek);
            model.paramsUrl.Add(ecran);
            model.SiteLanguage = customerSession.SiteLanguage;
            model.Labels = LoadPageLabels(customerSession.SiteLanguage);

            var isRadio = _portfolioDetailMediaService.IsIndeRadioMessage(idWebSession, this.HttpContext);

            //Les indes Radio
            if (isRadio)
            {
                model.Labels.IndeRadioMessage = GestionWeb.GetWebWord(LanguageConstantes.IndeRadioMessage, customerSession.SiteLanguage);
            }

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(customerSession.SiteLanguage);
            ViewBag.SiteLanguage = customerSession.SiteLanguage;

            return View(model);
        }



        public JsonResult DetailMediaResult(string idMedia, string dayOfWeek, string ecran)
        {

            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _portfolioDetailMediaService.GetDetailMediaGridResult(idWebSession, idMedia, dayOfWeek, ecran, this.HttpContext);

            try
            {
                if (!reponse.HasData)
                    return null;

                jsonData = JsonConvert.SerializeObject(reponse.Data);
                JsonResult jsonModel = Json(new { datagrid = jsonData, columns = reponse.Columns, schema = reponse.Schema, columnsfixed = reponse.ColumnsFixed, needfixedcolumns = reponse.NeedFixedColumns }, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                EmptyGrid = GestionWeb.GetWebWord(LanguageConstantes.EmptyGrid, siteLanguage),
                VisuelLabel = GestionWeb.GetWebWord(LanguageConstantes.VisuelLabel, siteLanguage)
            };

            return result;
        }

    }
}