using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.RecpaSchema
{
    public class AdvertiserRecap
    {
        public long IdAdvertiser { get; set; }
        public long IdLanguage { get; set; }
        public long IdHoldingcompany { get; set; }
        public string AdvertiserLabel { get; set; }
        public long Activation { get; set; }
    }
}
