using Kantar.AdExpress.Service.Core.Domain.Mau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.DataAccess.Repository.Mau
{
    public interface ILoginRepository : IGenericRepository<Login>
    {
        Task<Login> GetLogin(string name);

        Task<bool> CheckPasswordAsync(string userName, string password);
    }
}
