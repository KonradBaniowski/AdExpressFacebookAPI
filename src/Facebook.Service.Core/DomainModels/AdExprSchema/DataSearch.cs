using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class DataSearch: Data
    {
        public long DateMediaNum { get; set; }
        public long IdDataSearch { get; set; }
        public long IdLanguageData { get; set; }
        public long IdProduct { get; set; }
        public long ExpenditureEuro { get; set; }
    }
}
