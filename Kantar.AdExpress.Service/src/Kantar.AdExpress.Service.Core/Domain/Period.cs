using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.BusinessService
{
    public class PeriodResponse
    {
        public PeriodResponse()
        {
            ControllerDetails = new ControllerDetails();
            ErrorMessage = string.Empty;
        }

        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public int SiteLanguage { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
        public ControllerDetails ControllerDetails { get; set; }

    }
}
