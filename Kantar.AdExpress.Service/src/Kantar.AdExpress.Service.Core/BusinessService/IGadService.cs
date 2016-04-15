using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IGadService
    {
        Gad GetGadInfos(string idWebSession, string idAddress, string advertiser);
    }
}
