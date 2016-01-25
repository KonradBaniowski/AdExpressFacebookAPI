using Kantar.AdExpress.Service.Contracts;
using Kantar.AdExpress.Service.Contracts.Mau;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface ILoginService
    {
        Task<SignInStatus> Password(string login, string password);

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ApplicationIdentityResult> Register(AppUser user, string password);

        /// <summary>
        /// Retourne l'utilisateur Kantar avec ses modules
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        KantarUserModel GeApplicationIdentityUser(string login, string password);
    }
}
