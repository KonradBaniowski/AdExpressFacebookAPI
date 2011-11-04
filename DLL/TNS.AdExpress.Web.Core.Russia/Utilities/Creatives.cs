using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.Core.Russia.Utilities
{
    public class Creatives : TNS.AdExpress.Web.Core.Utilities.Creatives
    {
        public Creatives()
            : base()
        {
        }


        public override bool IsSloganZoom(long sloganId)
        {
            return (sloganId > long.MinValue);
        }
    }
}
