
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class PresentationModel
    {
        public int SiteLanguage { get; set; }
        public long ModuleCode { get; set; }
        public long ModuleDecriptionCode { get; set; }
        public bool ShowCurrentSelection { get; set; }

        public string ModuleDescription { get; set; }
        public string ModuleTitle { get; set; }
    }

}