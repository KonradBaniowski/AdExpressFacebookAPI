using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.BusinessEntities.Classification
{
  public  class ClassificationItem
    {
        public long Id{ get; set; }

        public long IdLanguage { get; set; }  

        public string Label { get; set; }

        public long Activation { get; set; }
    }
}
