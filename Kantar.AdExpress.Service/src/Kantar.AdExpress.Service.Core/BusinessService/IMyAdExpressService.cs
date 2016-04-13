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
        AdExpressResponse RenameUniversDirectory(string name, string universId, string webSessionId);
    }
}
