using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IAlertService
    {
        string GetRedirectUrl(WebSession session, string idSession, AlertOccurence occ);
    }
}
