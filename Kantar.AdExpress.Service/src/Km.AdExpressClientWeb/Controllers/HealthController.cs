﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class HealthController : Controller
    {
        // GET: Health
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results()
        {
            return View();
        }

    }
}