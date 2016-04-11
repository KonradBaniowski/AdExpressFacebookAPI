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
        public string MyResults { get; set; }
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
        public string UnityError { get; set; }
        public string Periodicity { get; set; }//1293
        public string Daily { get; set; }//w2579
        public string Weekly { get; set; }//2580
        public string SaveAlert { get; set; }//2581
        public string Monthly { get; set; }//1294
        public string Quartly { get; set; }//1293
        public string Receiver { get; set; } //2483
        public string NoAlerts { get; set; }//2587
        public string SendDate { get; set; }//2588
        public string Occurrence { get; set; }//2600
        public string Occurrences { get; set; }//2601
        public string AlertsDetails { get; set; }//2602
        public string Deadline { get; set; }//2603
        public string EveryWeek { get; set; }//2604
        public string EveryMonth { get; set; }//2605
        public string ExpirationDate { get; set; }//2606
        public string AlertType { get; set; }//2607
        public string Folder { get; set; }//TBD
        public string Results { get; set; }//TBD
        public string TimeSchedule { get; set; }//w2614

    }
}