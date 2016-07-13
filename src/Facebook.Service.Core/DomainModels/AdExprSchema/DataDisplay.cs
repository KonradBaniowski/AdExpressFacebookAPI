using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class DataDisplay: Data
    {
        public long DateMediaNum { get; set; }
        public long IdGroupFormatBanners { get; set; }
        public long IdLanguageData { get; set; }
        public long IdProduct { get; set; }
        public long ExpenditureEuro { get; set; }
        public virtual Advertiser Advertiser { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
