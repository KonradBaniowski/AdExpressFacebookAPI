using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
    public class PDVByMediaPageFacebookContract
    {
        public long IdVehicle { get; set; }

        public string LabelVehicle { get; set; }

        public long Id { get; set; }

        public string Label { get; set; }

        public long Expenditure { get; set; }

        public long UniverseMarket { get; set; }
    }
}