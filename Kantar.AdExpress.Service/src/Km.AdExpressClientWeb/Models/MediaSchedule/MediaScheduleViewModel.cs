using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class CreativeMediaScheduleResultsViewModel : Models.Shared.ResultsViewModel
    {
        public string SiteLanguage { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string MediaTypeIds { get; set; }
        public string ProductIds { get; set; }
        public string CreativeIds { get; set; }
        public List<string> ErrorMessages { get; set; }

    }

  

}