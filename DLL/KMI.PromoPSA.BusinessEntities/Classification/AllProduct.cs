using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.BusinessEntities.Classification
{
  public  class AllProduct
    {
        public long IdProduct { get; set; }

        public long ProductLabel { get; set; }      

        public long IdCategory { get; set; }

        public string CategoryLabel { get; set; }

        public long IdSegment { get; set; }

        public string SegmentLabel { get; set; }
    }
}
