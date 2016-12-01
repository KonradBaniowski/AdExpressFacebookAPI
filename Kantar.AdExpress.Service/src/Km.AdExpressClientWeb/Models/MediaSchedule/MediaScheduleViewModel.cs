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
        public int SiteLanguage { get; set; }
        public int BeginDate { get; set; }

        public int EndDate { get; set; }

        public string   MediaTypeIds { get; set; }

        public string productIds { get; set; }

        public string CreativeIds { get; set; }

        public string ErrorMessage { get; set; }

    }

  

}