using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPortfolioAlertService
    {
        PortfolioAlertResultResponse GetPortfolioAlertResult(long alertId, long alertTypeId, string dateMediaNum);
    }
}
