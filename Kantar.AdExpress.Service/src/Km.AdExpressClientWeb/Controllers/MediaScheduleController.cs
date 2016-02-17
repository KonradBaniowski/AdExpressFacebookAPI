using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models;
using KM.AdExpress.Framework.MediaSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class MediaScheduleController : Controller
    {
        private IMediaScheduleService _mediaScheduleService;

        private string icon;
        public MediaScheduleController(IMediaScheduleService mediaScheduleService)
        {
            _mediaScheduleService = mediaScheduleService;
        }
        public ActionResult Index()
        {
            var marketNode = new MediaPlanNavigationNode { Position = 1 };
            var model = LoadNavBar(marketNode.Position);
            return View(model);
        }

        public ActionResult Presentation()
        {
            return View();
        }

        public ActionResult MediaSelection()
        {
            //var model = new MediaSelectionViewModel();
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var media = _mediaScheduleService.GetMedia(idWebSession);
            #region Hardcoded model data
            var model = new MediaSelectionViewModel()
            {
                Multiple = true,
                Medias = new List<Media>()
                {
                    new Media()
                    {
                        MediaEnum = Vehicles.names.cinema,
                        Id= 1,

                        Label = "Cinéma",
                        Disabled = false
                    },
                    new Media()
                    {
                          MediaEnum = Vehicles.names.search,
                        Id = 34,
                        Label = "Search",
                        Disabled = false
                    },
                    new Media()
                    {
                          MediaEnum = Vehicles.names.tv,
                        Id = 2,
                        Label = "Télévision",
                        Disabled = true
                    },
                        new Media()
                    {
                              MediaEnum = Vehicles.names.evaliantMobile,
                        Id = 4,
                        Label = "Evaliant Mobile",
                        Disabled = false
                    },
                            new Media()
                    {
                                MediaEnum = Vehicles.names.directMarketing,
                        Id = 10,
                        Label = "Courrier",
                        Disabled = false
                    }
                    //  new Media()
                    //{
                    //    Id = 6,
                    //    Label = "Nom 6",
                    //    Disabled = false
                    //},
                    //      new Media()
                    //{
                    //    Id = 7,
                    //    Label = "Nom 7",
                    //    Disabled = true
                    //}

                },
                IdMediasCommon =
                    new List<int>()
                    {
                    { 1 },
                    { 2 },
                    { 4 },
                    { 6 }
                    }
            };

            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            #endregion
            var mediaNode = new MediaPlanNavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            
            return View(model);
        }

        public ActionResult PeriodSelection()
        {
            var periodeNode = new MediaPlanNavigationNode { Position = 3 };
            var model = LoadNavBar(periodeNode.Position);
            return View(model);
        }

        public ActionResult Results()
        {
            var resultNode = new MediaPlanNavigationNode { Position = 4 };
            var model = LoadNavBar(resultNode.Position);
            return View(model);
        }

        private List< MediaPlanNavigationNode> LoadNavBar(int currentPosition)
        {
            var model = new List<MediaPlanNavigationNode>();
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new MediaPlanNavigationNode
            {
                Id = 1,
                IsActive = false,
                Description = "Lorem ipsum incidiunt empror....",
                Title = "Marché",
                Action = "Index",
                Controller = "MediaSchedule"
            };
            model.Add(market);
            var media = new MediaPlanNavigationNode
            {
                Id = 2,
                IsActive = false,
                Description = "Lorem ipsum incidiunt empror....",
                Title = "Media",
                Action = "MediaSelection",
                Controller = "MediaSchedule"
            };
            model.Add(media);
            var dates = new MediaPlanNavigationNode
            {
                Id = 3,
                IsActive = false,
                Description = "Lorem ipsum incidiunt empror....",
                Title = "Dates",
                Action = "PeriodSelection",
                Controller = "MediaSchedule"
            };
            model.Add(dates);
            var result = new MediaPlanNavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Lorem ipsum incidiunt empror....",
                Title = "Resultats",
                Action = "Results",
                Controller = "MediaSchedule"
            };
            model.Add(result);
            foreach( var nav in model)
            {
                nav.IsActive = (nav.Id > currentPosition) ? false : true;
            }
            #endregion
            return model;
        }

    }
}