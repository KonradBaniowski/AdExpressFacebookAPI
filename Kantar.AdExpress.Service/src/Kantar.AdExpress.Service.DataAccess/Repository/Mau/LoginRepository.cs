using Kantar.AdExpress.Service.Core.DataAccess.Repository.Mau;
using Kantar.AdExpress.Service.Core.Domain.Mau;
using Kantar.AdExpress.Service.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.Repository.Mau
{
    public class LoginRepository : GenericRepository<AdExpressContext, Login>, ILoginRepository
    {
        public LoginRepository(AdExpressContext context) : base(context)
        {
        }

        public async Task<Login> GetLogin(string name)
        {
            return await Task.Run(() =>
            {
                var res = (from a in mycontext.Login
                           where a.LoginName == name.ToUpper()
                           select a).FirstOrDefault();
                return res;
            });
        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            return await Task.Run(() =>
            {
                var res = (from login in mycontext.Login
                           where login.Password == password.ToUpper() && login.LoginName == userName.ToUpper()
                           select login).Any();
                return res;
            });
        }
    }
}
