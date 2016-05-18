using AutoMapper;
using Kantar.AdExpress.Service.Core;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class AnalysisController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaScheduleService _mediaSchedule;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private ISubPeriodService _subPeriodService;
        private const string _controller = "Analysis";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;
        private int _siteLanguage = 33;

        private string icon;
        public AnalysisController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule, IUniverseService universService, IPeriodService periodService, IOptionService optionService, ISubPeriodService subPeriodService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
            _subPeriodService = subPeriodService;
        }
        // GET: Analysis
        public ActionResult Market()
        {
            #region Init
            var model = new MarketViewModel
            {
                Trees = new List<Tree>(),
                Branches = new List<UniversBranch>(),
                UniversGroups = new UserUniversGroupsModel(),
                Dimension = Dimension.product
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            #endregion
            #region Load Branches
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true);
            #endregion

            #region Load each label's text in the appropriate language
            var helper = new Helpers.PageHelper();
            model.Labels = helper.LoadPageLabels(result.SiteLanguage);
            model.Branches = Mapper.Map<List<UniversBranch>>(result.Branches);
            foreach (var item in result.Trees)
            {
                Tree tree = new Tree
                {
                    Id = item.Id,
                    LabelId = item.LabelId,
                    AccessType = item.AccessType,
                    UniversLevels = Mapper.Map<List<UniversLevel>>(item.UniversLevels)
                };
                tree.Label = (tree.AccessType == TNS.Classification.Universe.AccessType.includes) ? model.Labels.IncludedElements : model.Labels.ExcludedElements;
                model.Trees.Add(tree);
            }
            #endregion
            #region Presentation
            model.Presentation = helper.LoadPresentationBar(result.SiteLanguage, LanguageConstantes.AnalysisDetailedReport);
            model.UniversGroups = new UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                SiteLanguage = result.SiteLanguage
            };
            #endregion
            _siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            var marketNode = new NavigationNode { Position = 1 };         
            model.NavigationBar = helper.LoadNavBar(webSessionId, _controller, _siteLanguage, 1);
            // listBranch=TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();
            
            return View(model);
        }
    }
}