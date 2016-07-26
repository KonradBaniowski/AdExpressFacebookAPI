using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.RecpaSchema
{
   public class BrandRecap
    {
        public long Id { get; set; }
        public long IdLanguage { get; set; }
        public string BrandLabel { get; set; }
        public long Activation { get; set; }
    }
}
