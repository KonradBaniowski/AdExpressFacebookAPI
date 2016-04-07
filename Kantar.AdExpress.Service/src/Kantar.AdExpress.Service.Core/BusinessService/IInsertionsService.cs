using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
   public interface IInsertionsService
    {
        
        InsertionCreativeResponse GetInsertionsGridResult(string idWebSession,string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged = false);
        List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId);

        ResultTable GetInsertionsResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long idVehicle);

        SpotResponse GetCreativePath(string idWebSession, string idVersion, long idVehicle);
    }
}
