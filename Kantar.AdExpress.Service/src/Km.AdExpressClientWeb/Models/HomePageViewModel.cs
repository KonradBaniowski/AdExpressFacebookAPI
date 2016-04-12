﻿using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class HomePageViewModel
    {
        public Dictionary<long, Module> ModuleRight { get; set; }
        public Dictionary<long, Module> Modules { get; set; }
        public List<Documents> Documents { get; set; }
    }
}