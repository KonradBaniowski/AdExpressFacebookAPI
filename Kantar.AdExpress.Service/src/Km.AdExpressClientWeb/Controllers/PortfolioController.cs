using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Web.Core.Sessions;

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

        //public ActionResult Presentation()
        //{
        //    return View();
        //}

        //public ActionResult MediaSelection()
        //{
        //    //var model = new MediaSelectionViewModel();

        //    var claim = new ClaimsPrincipal(User.Identity);
        //    string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
        //    var media = _mediaService.GetMedia(idWebSession);
        //    var _webSession = (WebSession)WebSession.Load(idWebSession);

        //    #region model data
        //    var idMediasCommon = Array.ConvertAll(Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaInSelectAll).Split(','), Convert.ToInt32).ToList();
        //    var model = new VM.MediaSelectionViewModel()
        //    {
        //        Multiple = false,
        //        Medias = media,
        //        IdMediasCommon = idMediasCommon
        //    };
        //    model.Presentation = LoadPresentationBar(_webSession.SiteLanguage);
        //    foreach (var e in model.Medias)
        //    {
        //        e.icon = IconSelector.getIcon(e.MediaEnum);
        //    }
        //    model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
        //    var mediaNode = new VM.MediaPlanNavigationNode { Position = 2 };
        //    model.NavigationBar = LoadNavBar(mediaNode.Position);
        //    model.ErrorMessage = new VM.ErrorMessage
        //    {
        //        EmptySelection = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage),
        //        SearchErrorMessage = GestionWeb.GetWebWord(3011, _webSession.SiteLanguage),
        //        SocialErrorMessage = GestionWeb.GetWebWord(3030, _webSession.SiteLanguage),
        //        UnitErrorMessage = GestionWeb.GetWebWord(2541, _webSession.SiteLanguage)
        //    };
        //    #endregion
        //    return View(model);
        //}
    }
}