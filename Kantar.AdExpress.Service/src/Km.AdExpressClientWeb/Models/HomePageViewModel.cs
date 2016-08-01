using Kantar.AdExpress.Service.Core.Domain;
using KM.Framework.Constantes;
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
        public string EncryptedPassword { get; set; }
        public string EncryptedLogin { get; set; }
        public int SiteLanguage { get; set; }
        public Labels Labels { get; set; }
        public string CountryCode { get; set; }
    }
}