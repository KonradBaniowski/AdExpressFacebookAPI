using Km.AdExpressClientWeb.Models;
using KM.AdExpress.Framework.MediaSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Date;
using TNS.AdExpressI.Date.DAL;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
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
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = CustomerSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            int startYear = dateDAL.GetCalendarStartDate();
            int endYear = DateTime.Now.Year;

            PeriodViewModel model = new PeriodViewModel();

            model.SiteLanguage = CustomerSession.SiteLanguage;
            model.StartYear = string.Format("{0}-01-01", startYear);
            model.EndYear = string.Format("{0}-12-31", endYear);

            return View(model);
        }

        public ActionResult CalendarValidation(string selectedStartDate, string selectedEndDate)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            DateTime lastDayEnable = DateTime.Now;

            DateTime startDate = new DateTime(Convert.ToInt32(selectedStartDate.Substring(6, 4)), Convert.ToInt32(selectedStartDate.Substring(3, 2)), Convert.ToInt32(selectedStartDate.Substring(0, 2)));
            DateTime endDate = new DateTime(Convert.ToInt32(selectedEndDate.Substring(6, 4)), Convert.ToInt32(selectedEndDate.Substring(3, 2)), Convert.ToInt32(selectedEndDate.Substring(0, 2)));

            CustomerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
            CustomerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            CustomerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            CustomerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");

            if (endDate < DateTime.Now || DateTime.Now < startDate)
                CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodEndDate);
            else
                CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));

             CustomerSession.Save();

            return RedirectToAction("");
        }

        public ActionResult SlidingDateValidation(int selectedPeriod, int selectedValue)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
            globalCalendar.comparativePeriodType comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;

            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);
           
            if (selectedValue == 0) throw new Exception(GestionWeb.GetWebWord(885, CustomerSession.SiteLanguage));
            date.SetDate(ref CustomerSession, DateTime.Now, periodCalendarDisponibilityType, comparativePeriodCalendarType, selectedPeriod, selectedValue);

            CustomerSession.Save();

            return RedirectToAction("");
        }

        public ActionResult Results()
        {
            return View();
        }

    }
}