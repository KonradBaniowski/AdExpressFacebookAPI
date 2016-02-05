using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using Kantar.AdExpress.Service.DataAccess.Extensions;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.IdentityImpl
{
    //public class CustomUserStore : IUserStore<Identity.AppUser>
    //{
    //    private readonly IUnitOfWork _uow;
    //    public CustomUserStore(IUnitOfWork uow)
    //    {
    //        _uow = uow;
    //    }

    //    public Task CreateAsync(Identity.AppUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task DeleteAsync(Identity.AppUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IdentityUser> FindByIdAsync(string userId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<Core.Domain.Identity.AppUser> FindByNameAsync(string userName)
    //    {
    //        var user = await _uow.LoginRepository.GetLogin(userName);
    //        Identity.AppUser aIdentityUser = new Identity.AppUser();
    //        aIdentityUser.Id = user.Id;
    //        aIdentityUser.Email = user.LoginName;
    //        aIdentityUser.TwoFactorEnabled = false;
    //        aIdentityUser.LockoutEnabled = false;
    //        aIdentityUser.UserName = user.LoginName;
    //        return aIdentityUser.ToAppUser();
    //    }

    //    public Task UpdateAsync(IdentityUser user)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
