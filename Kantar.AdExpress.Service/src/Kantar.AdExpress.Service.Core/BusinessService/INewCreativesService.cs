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
    public interface INewCreativesService
    {
        ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext);
        GridResult GetGridResult(string idWebSession, HttpContextBase httpContext);

    }
}
