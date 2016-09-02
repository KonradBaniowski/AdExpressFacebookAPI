using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
    public class KPIClassificationContract
    {
        public KPIClassificationContract()
        {
            Id = 0;
            Label = "";
            Month = "0";
            Like = 0;
            Share = 0;
            Comment = 0;
            Post = 0;
            Expenditure = 0;
            UniverseMarket = 0;
        }

        public string Label { get; set; }
        public long Id { get; set; }
        public string Month { get; set; }
        public long Like { get; set; }
        public long Share { get; set; }
        public long Comment { get; set; }
        public long Post { get; set; }
        public long Expenditure { get; set; }
        public long UniverseMarket { get; set; }
    }
}