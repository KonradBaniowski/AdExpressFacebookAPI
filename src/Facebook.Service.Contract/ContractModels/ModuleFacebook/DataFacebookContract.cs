using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class DataFacebookContract
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
        //public List<PageFacebookContract> PageFacebookContracts { get; set; }
    }

    public class PageFacebookContract
    {
        public long PID { get; set; }
        public string Url { get; set; }
        public long IdPage { get; set; }
        public long IdAdvertiser { get; set; }
        public string IdPageFacebook { get; set; }
        public string PageName { get; set; }
        public long NumberPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberShare { get; set; }
        public long NumberFan { get; set; }
        public long NumberComment { get; set; }
        public long Expenditure { get; set; }


    }
}
