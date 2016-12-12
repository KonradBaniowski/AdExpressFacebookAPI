using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
  public  class CreativeMediaScheduleRequest
    {
        public string IdWebSession { get; set; }
        public int SiteLanguage { get; set; }

        public string MediaTypeIds { get; set; }
        public int BeginDate { get; set; }
        public int EndDate { get; set; }

        public  string ProductIds { get; set; }
        public string CreativeIds { get; set; }

    }
}
