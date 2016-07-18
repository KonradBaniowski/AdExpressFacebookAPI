using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
    public class KPIPercentPageFacebookContract
    {
        public string Month { get; set; }
        public long ReferentPercent { get; set; }
        public long ConcurrentPercent { get; set; }
        public long ReferentFBPercent { get; set; }
        public long ConcurrentFBPercent { get; set; }
    }
}