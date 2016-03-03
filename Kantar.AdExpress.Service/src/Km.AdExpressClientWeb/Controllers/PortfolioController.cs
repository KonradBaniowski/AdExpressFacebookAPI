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
using VM = Km.AdExpressClientWeb.Models.Portfolio;
using KM.AdExpress.Framework.MediaSelection;
using Domain = Kantar.AdExpress.Service.Core.Domain;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private IPortfolioService _portoFolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universeService;
        private const string _controller = "Portfolio";
        public PortfolioController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universeService)
        {
            _portoFolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universeService = universeService;
        }
        // GET: Portfolio
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Presentation()
        {
            return View();
        }

        public ActionResult MediaSelection()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _mediaService.GetMedia(idWebSession);

            #region model data
            var idMediasCommon = Array.ConvertAll(Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaInSelectAll).Split(','), Convert.ToInt32).ToList();
            var model = new VM.MediaSelectionViewModel()
            {
                Multiple = false,
                Medias = result.Media,
                IdMediasCommon = idMediasCommon
            };
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            var mediaNode = new NavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            model.ErrorMessage = new Km.AdExpressClientWeb.Models.MediaSchedule.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
            #endregion
            return View(model);
        }

        public JsonResult SaveMediaSelection(List<long> selectedMedia, string nextStep)
        {
            string url = string.Empty;
            var response = new Domain.WebSessionResponse();
            if (selectedMedia != null)
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                response = _webSessionService.SaveMediaSelection(selectedMedia, idWebSession);
            }
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action(nextStep, _controller);
            JsonResult jsonModel = Json(new { RedirectUrl = url });
            return jsonModel;
        }

        #region Private methodes
        private List<NavigationNode> LoadNavBar(int currentPosition)
        {
            var model = new List<NavigationNode>();
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new NavigationNode
            {
                Id = 1,
                IsActive = false,
                Description = "Market",
                Title = "Marché",
                Action = "Index",
                Controller = _controller,
                IconCssClass = "fa fa-file-text"
            };
            model.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = false,
                Description = "Media",
                Title = "Media",
                Action = "MediaSelection",
                Controller = _controller,
                IconCssClass = "fa fa-eye"
            };
            model.Add(media);
            var dates = new NavigationNode
            {
                Id = 3,
                IsActive = false,
                Description = "Dates",
                Title = "Dates",
                Action = "PeriodSelection",
                Controller = _controller,
                IconCssClass = "fa fa-calendar"
            };
            model.Add(dates);
            var result = new NavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Results",
                Title = "Resultats",
                Action = "Results",
                Controller = _controller,
                IconCssClass = "fa fa-check"
            };
            model.Add(result);
            foreach (var nav in model)
            {
                nav.IsActive = (nav.Id > currentPosition) ? false : true;
            }
            #endregion
            return model;
        }

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                KeyWordLabel = GestionWeb.GetWebWord(LanguageConstantes.KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(LanguageConstantes.KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.ErrorMsgCode, siteLanguage),
                BranchLabel = GestionWeb.GetWebWord(LanguageConstantes.BranchLabelCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                UserSavedUniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                Include = GestionWeb.GetWebWord(LanguageConstantes.IncludeCode, siteLanguage),
                Exclude = GestionWeb.GetWebWord(LanguageConstantes.ExcludeCode, siteLanguage),
                LoadUnivers = GestionWeb.GetWebWord(LanguageConstantes.LoadUniversCode, siteLanguage),
                Save = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage)               
            };
            return result;
        }
        private Models.MediaSchedule.PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection=true)
        {
            Models.MediaSchedule.PresentationModel result = new Models.MediaSchedule.PresentationModel
            {
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SiteLanguage = siteLanguage,
                ShowCurrentSelection = showCurrentSelection,
                ModuleDecriptionCode = LanguageConstantes.PortfolioDescriptionCode
            };
            return result;
        }
        #endregion
    }
}