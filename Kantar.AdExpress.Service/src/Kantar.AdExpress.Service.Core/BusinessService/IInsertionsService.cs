using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
   public interface IInsertionsService
    {
        
        InsertionCreativeResponse GetInsertionsGridResult(string idWebSession,string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, HttpContextBase httpContext, bool isVehicleChanged = false);
        List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId, HttpContextBase httpContext, bool slogan = false);

        ResultTable GetInsertionsResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long idVehicle, HttpContextBase httpContext);

        SpotResponse GetCreativePath(string idWebSession, string idVersion, long idVehicle, HttpContextBase httpContext);
    }
}
