using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaScheduleService
    {
        object[,] GetMediaScheduleData(string idWebSession);
        GridResult GetGridResult(string idWebSession, string periodType);
    }
}
