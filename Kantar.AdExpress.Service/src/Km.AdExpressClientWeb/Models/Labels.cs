using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class Labels
    {
        public string CurrentController { get; set; }
        public string KeyWordLabel { get; set; }// 972
        public string KeyWordDescription { get; set; }//phrase under search input 2287
        public string ErrorMessage { get; set; }//930
        public string KeyWord { get; set; }
        public string ElementLabel { get; set; }//2278
        public string BranchLabel { get; set; }//2272
        public string NoSavedUnivers { get; set; }
        public string UserSavedUniversLabel { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public string LoadUnivers { get; set; }
        public string Save { get; set; }
        public string Submit { get; set; }
        public string CleanSelectionMsg { get; set; }
        public string IncludedElements { get; set; }
        public string ExcludedElements { get; set; }
        public string Results { get; set; }
        public string Refine { get; set; }
        public string ErrorMessageLimitKeyword { get; set; }
        public string ErrorMessageLimitUniverses { get; set; }
        public string ErrorMininumInclude { get; set; }
        public string ErrorMediaSelected { get; set; }
        public string ErrorNoSupport { get; set; }
        public string ErrorItemExceeded { get; set; }
    }
}