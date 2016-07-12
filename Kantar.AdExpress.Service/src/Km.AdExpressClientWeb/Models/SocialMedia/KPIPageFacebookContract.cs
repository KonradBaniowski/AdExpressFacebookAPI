using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
    public class KPIPageFacebookContract
    {

        public KPIPageFacebookContract()
        {
            Month = "0";
            Like = 0;
            Share = 0;
            Comment = 0;
            Post = 0;
            Expenditure = 0;
        }

        public string Month { get; set; }
        public long Like { get; set; }
        public long Share { get; set; }
        public long Comment { get; set; }
        public long Post { get; set; }
        public long Expenditure { get; set; }

    }
}