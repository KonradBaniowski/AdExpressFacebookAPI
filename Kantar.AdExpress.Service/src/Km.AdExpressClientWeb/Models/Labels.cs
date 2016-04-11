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
        public string ErrorMessageLimitKeyword { get; set; }//1370
        public string ErrorMessageLimitUniverses { get; set; }
        public string ErrorMininumInclude { get; set; }
        public string ErrorMediaSelected { get; set; }
        public string ErrorNoSupport { get; set; }
        public string ErrorItemExceeded { get; set; }
        public string DeleteAll { get; set; }// 2279
        public string ErrorOnlyOneItemAllowed { get; set; }//3036
        public string ErrorOverLimit { get; set; }//w2264==> only 200items
        public string SaveUnivers { get; set; }//769
        public string AddConcurrent { get; set;}//3037
        public string ErrorSupportAlreadyDefine { get; set; } // 3038
        public string Concurrent { get; set; } //w2869
        public string Referent { get; set; } //3039
        public string UserUniversCode { get; set; }//875
        public string MyResultsDescription { get; set; }//827
        public string AlertsCode { get; set; }//2585

        public string CompanyDescription { get; set; } //w68
        public string NameDescription { get; set; } //w67
        public string JobTitleDescription { get; set; } //w1976
        public string PhoneNumberDescription { get; set; } //w71
        public string MailDescription { get; set; }  //w1136
        public string CountryDescription { get; set; } //w70
        public string CommentDescription { get; set; } //w74
        public string QuestionTagDefault { get; set; } //w3041
        public string QuestionTag1 { get; set; } //w3042
        public string QuestionTag2 { get; set; } //w3043
        public string QuestionTag3 { get; set; } //w647
    }
}