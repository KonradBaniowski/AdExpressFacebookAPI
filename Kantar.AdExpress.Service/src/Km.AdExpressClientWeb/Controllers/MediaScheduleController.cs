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
                        MediaEnum = VehiclesEnum.press,
                        Id= 1,
                        
                        Label = "Nom 1",
                        Disabled = false
                    },
                    new Media()
                    {
                        Id = 3,
                        Label = "Nom 3",
                        Disabled = false
                    },
                    new Media()
                    {
                        Id = 2,
                        Label = "Nom 2",
                        Disabled = true
                    },
                        new Media()
                    {
                        Id = 4,
                        Label = "Nom 4",
                        Disabled = false
                    },
                            new Media()
                    {
                        Id = 5,
                        Label = "Nom 5",
                        Disabled = false
                    },
                      new Media()
                    {
                        Id = 6,
                        Label = "Nom 6",
                        Disabled = false
                    },
                          new Media()
                    {
                        Id = 7,
                        Label = "Nom 7",
                        Disabled = true
                    }

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

            foreach(var e in model.Medias)
            {
                icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            return View(model);
        }

        public ActionResult PeriodSelection()
        {
            return View();
        }
    }
}