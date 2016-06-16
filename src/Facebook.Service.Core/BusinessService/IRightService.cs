using Facebook.Service.Contract.ContractModels.RightService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.BusinessService
{
    public interface IRightService
    {
        List<Right> GetProductRight(int idLogin);
        List<Right> GetMediaRight(int idLogin);
    }
}
