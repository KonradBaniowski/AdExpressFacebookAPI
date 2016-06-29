using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApi.Models
{
    public class PostModel
    {
        public int IdLogin { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public List<long> IdAdvertisers { get; set; }
        public List<long> IdBrands { get; set; }
        public List<long> IdPages { get; set; }
    }
}