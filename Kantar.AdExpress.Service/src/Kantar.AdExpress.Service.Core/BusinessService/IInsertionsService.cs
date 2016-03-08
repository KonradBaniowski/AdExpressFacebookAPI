using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
   public interface IInsertionsService
    {
        
        InsertionResponse GetInsertionsGridResult(string idWebSession,string ids, string zoomDate, int idUnivers, long moduleId);
    }
}
