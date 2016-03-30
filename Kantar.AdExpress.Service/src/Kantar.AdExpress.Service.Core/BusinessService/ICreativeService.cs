using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
  public  interface ICreativeService
    {


        InsertionResponse GetCreativeGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle);
        List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId);

        SpotResponse GetCreativePath(string idWebSession, string idVersion, long idVehicle);
    }
}
