using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kantar.AdExpress.Service.DataAccess.IdentityImpl
{
    public class CustomUserManager : UserManager<IdentityUser>
    {
        public CustomUserManager(IUserStore<IdentityUser> store)
            : base(store)
        {
        }
    }
}
