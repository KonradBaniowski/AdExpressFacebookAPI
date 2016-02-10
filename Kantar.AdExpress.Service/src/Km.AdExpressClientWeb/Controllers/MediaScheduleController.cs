using Km.AdExpressClientWeb.Models;
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
                icon = getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            return View(model);
        }

        private string getIcon(VehiclesEnum element)
        {
            var icon = string.Empty;
           switch (element)
            {
                case VehiclesEnum.cinema:
                    icon = "iconkantar-video163";
                    break;
                case VehiclesEnum.search:
                    icon = "iconkantar-internet94";
                    break;
                case VehiclesEnum.press:
                    icon = "iconkantar-news12";
                    break;
                case VehiclesEnum.tv:
                    icon = "iconkantar-television20";
                    break;
                case VehiclesEnum.evaliantMobile:
                    icon = "iconkantar-cellphone55";
                    break;
                case VehiclesEnum.mms:
                    icon = "iconkantar-planetary2";
                    break;
                case VehiclesEnum.directMarketing:
                    icon = "iconkantar-mail114";
                    break;
                case VehiclesEnum.emailing:
                    icon = "iconkantar-envelope82";
                    break;
                case VehiclesEnum.adnettrack:
                    icon = "iconkantar-monitor74";
                    break;
                case VehiclesEnum.internationalPress:
                    icon = "iconkantar-newspapers5";
                    break;
                case VehiclesEnum.outdoor:
                    icon = "iconkantar-commercial";
                    break;
                case VehiclesEnum.radio:
                    icon = "iconkantar-radio46";
                    break;
                case VehiclesEnum.tvSponsorship:
                    icon = "iconkantar-wireless-connectivity79";
                    break;
                default:
                    icon = "iconkantar-window50";
                    break;

            }
            return icon;
        }

        public ActionResult PeriodSelection()
        {
            return View();
        }
    }
}