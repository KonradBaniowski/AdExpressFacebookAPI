using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
  public  class SpotResponse
    {
        public string PathDownloadingFile { get; set; }
        public string PathReadingFile { get; set; }
        public int SiteLanguage { get; set; }
    }
}
