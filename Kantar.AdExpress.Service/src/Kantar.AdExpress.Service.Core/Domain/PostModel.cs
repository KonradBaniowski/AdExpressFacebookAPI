using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class PostModel
    {
        public PostModel()
        {
            IdAdvertisers = new List<long>();
            IdBrands = new List<long>();
            IdPages = new List<long>();
        }
        public int IdLogin { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public List<long> IdAdvertisers { get; set; }
        public List<long> IdBrands { get; set; }
        public List<long> IdPages { get; set; }
        public int IdLanguage { get; set; }
        public long IdPost { get; set;}
    }
}
