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


        InsertionCreativeResponse GetCreativeGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged);
        List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId, bool slogan = false);

        SpotResponse GetCreativePath(string idWebSession, string idVersion, long idVehicle);
    }
}
