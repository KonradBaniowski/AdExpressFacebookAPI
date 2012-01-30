using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.Core.Utilities
{
    public class Creatives
    {

        public Creatives()
        {
        }

        public virtual bool IsSloganZoom(long sloganId)
        {
            return (sloganId > 0) ;
        }
    }
}
