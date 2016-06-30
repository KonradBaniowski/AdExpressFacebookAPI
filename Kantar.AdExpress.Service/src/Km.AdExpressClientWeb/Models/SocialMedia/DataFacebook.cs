using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
   
    public class DataFacebook
    {
        public long PID { get; set; }
        public long IdPage { get; set; }
        public long ID { get; set; }
        public string IdPageFacebook { get; set; }
        public string Url { get; set; }
        //public string AdvertiserLabel { get; set; }
        //public string BrandLabel { get; set; }
        public long NbPage { get; set; }
        public string PageName { get; set; }
        public long NumberPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberShare { get; set; }
        public long NumberFan { get; set; }
        public long NumberComment { get; set; }
        public long Expenditure { get; set; }
    }

    public class PageFacebookContract
    {
        public long PID { get; set; }
        public string Url { get; set; }
        public long IdPage { get; set; }
        public long IdAdvertiser { get; set; }
        public long IdPageFacebook { get; set; }
        public string PageName { get; set; }
        public long NumberPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberShare { get; set; }
        public long NumberFan { get; set; }
        public long NumberComment { get; set; }
        public long Expenditure { get; set; }


    }

    public class PostFacebook
    {
        public long IdPostFacebook { get; set; }
        public string Advertiser { get; set; }
        public string PageName { get; set; }
        public string IdPost { get; set; }
        public DateTime DateCreationPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberComment { get; set; }
        public long NumberShare { get; set; }
        public long Commitment { get; set; }
        public string Brand { get; set; }
    }
}