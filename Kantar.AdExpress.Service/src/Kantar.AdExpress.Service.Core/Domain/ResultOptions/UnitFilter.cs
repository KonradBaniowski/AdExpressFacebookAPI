using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebConstantes = TNS.AdExpress.Constantes.Web;
namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UnitFilter
    {
        public int Unit { get; set; } = WebConstantes.CustomerSessions.Unit.none.GetHashCode();
    }
}
