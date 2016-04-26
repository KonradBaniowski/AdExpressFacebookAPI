using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IDetailSelectionService
    {
        DetailSelectionResponse GetDetailSelection(string idWebSession);
        DetailSelectionResponse LoadSessionDetails(string idSession, string idWebSession);
        DetailSelectionResponse LoadUniversDetails(string idUnivers, string idWebSession);
    }
}
