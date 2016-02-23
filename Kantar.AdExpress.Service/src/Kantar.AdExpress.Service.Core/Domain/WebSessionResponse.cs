using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class WebSessionResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public MediaScheduleStep MediaScheduleStep { get; set; }
    }

    public enum MediaScheduleStep
    {
        Market,
        Media,
        Period,
        Result
    }
}
