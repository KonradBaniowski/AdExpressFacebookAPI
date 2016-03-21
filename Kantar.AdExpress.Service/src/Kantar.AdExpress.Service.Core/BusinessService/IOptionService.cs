using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IOptionService
    {
        Options GetOptions(string idWebSession);
        void SetOptions(string idWebSession, UserFilter userFilter);
    }
}
