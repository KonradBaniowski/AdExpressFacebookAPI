using Kantar.AdExpress.Service.Core.DataAccess.Repository.Mau;
using Kantar.AdExpress.Service.Core.Domain.Mau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.Repository.Mau
{
    public class LoginRepository: GenericRepository<AdExpressContext, Login>, ILoginRepository
    {
        public LoginRepository(AdExpressContext context) : base(context)
        {
        }

        public Login getLogin()
        {
            var res = (from a in mycontext.Login
                       select a).FirstOrDefault();
                return res;
        }
    }
}
