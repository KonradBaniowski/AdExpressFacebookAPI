using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class HomePageViewModel
    {
        public Dictionary<Int64, Module> ModuleRight { get; set; }
        public List<Documents> Documents { get; set; }
        
    }
    public class Module
    {

    }

    public class Documents
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public List<InfosNews> InfosNews { get; set; }
    }

    public class InfosNews
    {
        public string Url { get; set; }
        public string Label { get; set; }
    }
}