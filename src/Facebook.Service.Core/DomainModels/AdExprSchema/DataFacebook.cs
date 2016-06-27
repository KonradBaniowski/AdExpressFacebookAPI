using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class DataFacebook : Data
    {
        
        public long DateMediaNum { get; set; }
        public long IdDataFacebook { get; set; }
        public long IdLanguageData { get; set; }
        public long Expenditure { get; set; }
        public long ExpenditureLocal { get; set; }
        public long NumberFan { get; set; }
        public long NumberLocalFan { get; set; }
        public long NumberPost { get; set; }
        public long NumberAction { get; set; }
        public long NumberLike { get; set; }
        public long NumberComment { get; set; }
        public long NumberShare { get; set; }
        public long IdPageFacebook { get; set; }
        public string PageName { get; set; }
        public long IdPage { get; set; }
        public long IdProduct { get; set; }
        public string Url { get; set; }
        public string GlobalType { get; set; }
        public string MainCategory { get; set; }
        public string PictureProfileUrl { get; set; }
        public long VerifiedPage { get; set; }
        public long IdHoldingCompany { get; set; }
        public long IdBasicMedia { get; set; }
        public long IdInterestCenter { get; set; }
        public long IdPeriodicity { get; set; }
        public long IdMediaSeller { get; set; }
        public long IdTitle { get; set; }
        public long IdCountry { get; set; }
        public long IdMediaGroup { get; set; }
        public long IdGroupAdvertisingAgency { get; set; }
        public long IdAdvertisingAgency { get; set; }
        public virtual Advertiser Advertiser { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
