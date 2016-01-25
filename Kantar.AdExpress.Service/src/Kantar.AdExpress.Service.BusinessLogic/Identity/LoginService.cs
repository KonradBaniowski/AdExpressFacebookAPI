using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using System;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Contracts.Mau;
using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.DataAccess.Identity;

namespace Kantar.AdExpress.Service.BusinessLogic.Identity
{
    public class LoginService : ILoginService
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public LoginService(IApplicationUserManager userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }



        public async Task<SignInStatus> Password(string login, string password)
        {
            var res = await _userManager.PasswordSignIn(login, password, true, false);
            return res;
            //return Mapper.Map<SignInStatus>(res);
        }

        public async Task<ApplicationIdentityResult> Register(Core.Domain.Identity.AppUser user, string password)
        {

            var res = await _userManager.CreateAsync(user, password);
            return res;
            //return Mapper.Map<SignInStatus>(res);
        }

        public KantarUserModel GeApplicationIdentityUser(string login, string password)
        {
            var contract = new KantarUserModel();
            var res = _unitOfWork.LoginRepository.FirstOrDefault(e => e.LoginName == login.ToUpper()
                && e.Password == password.ToUpper()
                && e.DateExpiration >= DateTime.Today
                && e.Activation < 50);
            contract.IdLogin = res.Id;
             
            return contract;
        }

        //public ApplicationIdentityUser get()
        //{
        //    return _userManager.sig
        //}


    }
}
