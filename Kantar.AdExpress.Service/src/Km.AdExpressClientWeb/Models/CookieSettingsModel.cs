using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class CookieSettingsModel
    {
        public int SiteLanguage { get; set; }

        public bool EnableTracking { get; set; }

        public bool EnableTroubleshooting { get; set; }
    }
}