using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class PostModel
    {
        public int idLogin { get; set; }
        public long beginDate { get; set; }
        public long endDate { get; set; }
        public List<long> idAdvertisers { get; set; }
        public List<long> idBrands { get; set; }
    }
}
