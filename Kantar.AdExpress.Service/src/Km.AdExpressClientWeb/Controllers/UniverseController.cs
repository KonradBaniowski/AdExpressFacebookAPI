﻿using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class UniverseController : Controller
    {
        private IUniverseService _universeService;

        public UniverseController(IUniverseService universeService)
        {
            _universeService = universeService;
        }

        // GET: Universe
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// TEST : http://localhost:55658/Universe/GetBranches?keyWord=%27bmw%27&branchId=7
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public JsonResult GetUniverses(string keyWord, int universeId, Dimension dimension, List<int> idMedias = null)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int totalItems = 0;
            var model = _universeService.GetItems(universeId, keyWord, idSession, dimension, idMedias, out totalItems);
            return Json(new { data = model, total = totalItems } , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TEST : http://localhost:55658/Universe/GetClassification?levelId=7&selectedClassification=%27295196%27&selectedLevelId=6
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="selectedClassification"></param>
        /// <param name="selectedLevelId"></param>
        /// <returns></returns>
        public JsonResult GetClassification(int levelId, string selectedClassification, int selectedLevelId, List<int> idMedias, Dimension dimension)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int totalItems = 0;
            var model = _universeService.GetItems(levelId, selectedClassification,selectedLevelId, idSession, dimension, idMedias, out totalItems);
            return Json(new { data = model, total = totalItems }, JsonRequestBehavior.AllowGet);
        }
    }
}