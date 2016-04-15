using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMyAdExpressService
    {
        AdExpressResponse RenameSession(string name, string universId, string webSessionId);
        AdExpressResponse MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId);
        AdExpressResponse DeleteSession(string id, string webSessionId);
        AdExpressResponse RenameUnivers(string name, string universId, string webSessionId);
        AdExpressResponse MoveUnivers(string id, string idOldGroupUnivers, string idNewGroupUnivers, string webSessionId);
        AdExpressResponse DeleteUnivers(string id, string webSessionId);
        AdExpressResponse CreateDirectory(string directoryName, UniversType type, string webSessionId);
        AdExpressResponse RenameDirectory(string directoryName, UniversType type, string idDirectory, string webSessionId);
        AdExpressResponse DropDirectory(string idDirectory, UniversType type, string webSessionId);
        AdExpressResponse LoadSession(string idSession, UniversType type, string webSessionId);
    }
}
