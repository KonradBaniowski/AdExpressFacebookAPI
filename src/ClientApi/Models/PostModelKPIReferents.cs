using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApi.Models
{
    public class PostModelKPIReferents

    {
        public int IdLogin { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public List<long> IdAdvertisers { get; set; }
        public List<long> IdBrands { get; set; }
        public List<long> IdPages { get; set; }
        public int IdLanguage { get; set; }
        public long IdPost { get; set; }
        public List<long> IdAdvertisersRef { get; set; }
        public List<long> IdBrandsRef { get; set; }
    }
}