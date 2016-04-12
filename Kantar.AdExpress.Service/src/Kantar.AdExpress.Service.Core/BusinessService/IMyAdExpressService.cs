using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMyAdExpressService
    {
        string RenameUnivers(string name, string universId, string webSessionId);
        string MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId);
    }
}
