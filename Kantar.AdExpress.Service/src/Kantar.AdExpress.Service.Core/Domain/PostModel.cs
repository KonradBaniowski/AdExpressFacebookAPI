using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class PostModel
    {
        public int IdLogin { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public List<long> IdAdvertisers { get; set; }
        public List<long> IdBrands { get; set; }
        public List<long> IdPages { get; set; }
        public long IdLanguage { get; set; }
    }
}
