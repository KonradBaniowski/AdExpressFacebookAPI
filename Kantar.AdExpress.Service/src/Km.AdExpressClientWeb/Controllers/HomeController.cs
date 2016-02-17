﻿using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Date.DAL;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IRightService _rightService;

        public HomeController(IRightService rightService)
        {
            _rightService = rightService;
        }

        public ActionResult Index()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idLogin = cla.Claims.Where(e => e.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var login = cla.Claims.Where(e => e.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var password = cla.Claims.Where(e => e.Type == ClaimTypes.Hash).Select(c => c.Value).SingleOrDefault();
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            //int _siteLanguage = WebApplicationParameters.DefaultLanguage;
            //var right = new TNS.AdExpress.Right(long.Parse(idLogin), login, password, _siteLanguage);
            if (password != null)
            {
                WebSession _webSession = null;
                int _siteLanguage = WebApplicationParameters.DefaultLanguage;
                var right = new TNS.AdExpress.Right(long.Parse(idLogin), login, password, _siteLanguage);
                if (right != null && right.CanAccessToAdExpress())
                {
                    right.SetModuleRights();
                    right.SetFlagsRights();
                    right.SetRights();
                    if (WebApplicationParameters.VehiclesFormatInformation.Use)
                        right.SetBannersAssignement();
                    //newRight.HasModuleAssignmentAlertsAdExpress();
                    if (_webSession == null) _webSession = new WebSession(right);
                    _webSession.IdSession = idWS;
                    //_webSession.SiteLanguage = _siteLanguage;
                    // Année courante pour les recaps                    
                    TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Date DAL"));
                    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
                    _webSession.DownLoadDate = dateDAL.GetLastLoadedYear();
                    // On met à jour IDataSource à partir de la session elle même.
                    _webSession.Source = right.Source;
                    //Sauvegarder la session
                    _webSession.Save();
                    // Tracking (NewConnection)
                    // On obtient l'adresse IP:
                    _webSession.OnNewConnection(this.Request.UserHostAddress);
                }

                //_webSession.CustomerLogin.
            }




            //var test = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage)
            var res = _rightService.GetModule(idWS);
            var docu = new Documents()
            {
                Id = 1,
                Label = "AdExpress News",
                InfosNews = new List<InfosNews>()
                {
                    new InfosNews()
                    {
                        Label = "Novembre 2015",
                        Url = "/AdExNews/AdexNews_201511.pdf"
                    },
                    new InfosNews()
                    {
                        Label = "Octobre 2015",
                        Url = "/AdExNews/AdexNews_201510.pdf"
                    }
                }
            };

            var Home = new HomePageViewModel()
            {
                ModuleRight = new System.Collections.Generic.Dictionary<long, Models.Module>()
                {
                    { 198, new Models.Module() },
                    { 197, new Models.Module() },
                    { 7216, new Models.Module() },
                    { 1781, new Models.Module() },
                    { 4370, new Models.Module() }
                },
                Documents = new List<Documents>() {
                    new Documents()
                    {
                        Id = 1,
                        Label = "AdExpress News",
                        InfosNews = new List<InfosNews>()
                        {
                            new InfosNews()
                            {
                                Label = "Novembre 2015",
                                Url = "/AdExNews/AdexNews_201511.pdf"
                            },
                            new InfosNews()
                            {
                                Label = "Octobre 2015",
                                Url = "/AdExNews/AdexNews_201510.pdf"
                            }
                        }
                    },
                    new Documents()
                    {
                        Id = 2,
                        Label = "AdExpress Report",
                        InfosNews = new List<InfosNews>()
                        {
                            new InfosNews()
                            {
                                Label = "Novembre 2015",
                                Url = "/AdExReport/AdExReport_201511.pdf"
                            },
                            new InfosNews()
                            {
                                Label = "Octobre 2015",
                                Url = "/AdExReport/AdExReport_201510.pdf"
                            }
                        }
                    },
                     new Documents()
                    {
                        Id = 3,
                        Label = "Documents",
                        InfosNews = null
                    }
                }
            };
            return View(Home);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
