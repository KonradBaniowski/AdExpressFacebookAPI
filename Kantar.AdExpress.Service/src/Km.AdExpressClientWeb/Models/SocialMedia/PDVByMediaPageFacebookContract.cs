using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
    public class PDVByMediaPageFacebookContract
    {
        public long IdMedia { get; set; }
        public string LabelMedia { get; set; }

        public long IdAdvertiser_Brand { get; set; }
        public string LabelAdvertiser_Brand { get; set; }

        public long Expenditure { get; set; }
    }
}