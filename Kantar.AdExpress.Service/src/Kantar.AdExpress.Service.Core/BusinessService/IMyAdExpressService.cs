using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMyAdExpressService
    {
        AdExpressResponse RenameSession(string name, string universId, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse DeleteSession(string id, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse RenameUnivers(string name, string universId, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse MoveUnivers(string id, string idOldGroupUnivers, string idNewGroupUnivers, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse DeleteUnivers(string id, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse CreateDirectory(string directoryName, UniversType type, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse RenameDirectory(string directoryName, UniversType type, string idDirectory, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse DropDirectory(string idDirectory, UniversType type, string webSessionId, HttpContextBase httpContext);
        AdExpressResponse LoadSession(string idSession, UniversType type, string webSessionId, HttpContextBase httpContext);
    }
}
