using Km.AdExpressClientWeb.Models;
using KM.AdExpress.Framework.MediaSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Km.AdExpressClientWeb.Controllers
{
    public class MediaScheduleController : Controller
    {
        private string icon;

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
            return View(model);
        }

        public ActionResult PeriodSelection()
        {
            return View();
        }

        public ActionResult Results()
        {
            return View();
        }

    }
}