using Kantar.AdExpress.Service.Contracts;
using Kantar.AdExpress.Service.Contracts.Mau;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using Kantar.AdExpress.ServiceRest.Models;
using System.Threading.Tasks;
using System.Web.Http;


namespace Kantar.AdExpress.ServiceRest.Controllers
{

    public class LoginController : ApiController
    {
        private readonly ILoginService _loginService;
        public LoginController()
        {

        }
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [Route("api/login/connect")]
        public async Task<SignInStatus> loginUser([FromBody]stuff model)
        {
            return await _loginService.Password(model.Email, model.Password);
        }

        [Route("api/login/register")]
        public async Task<ApplicationIdentityResult> Register([FromBody]stuff model)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var result = await _loginService.Register(user, model.Password);
            return result;
        }

        [Route("api/loginKantar/")]
        public KantarUserModel loginKantar([FromBody]stuff model)
        {
            
            var result = _loginService.GeApplicationIdentityUser(model.Email, model.Password);
            return result;
        }

    }

}



