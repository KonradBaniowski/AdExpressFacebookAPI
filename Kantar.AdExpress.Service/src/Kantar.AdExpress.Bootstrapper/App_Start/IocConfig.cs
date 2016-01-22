using Autofac;
using Autofac.Integration.WebApi;
using Kantar.AdExpress.Bootstrapper;
using Kantar.AdExpress.Service.BusinessLogic.Identity;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.DataAccess;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Kantar.AdExpress.Service.DataAccess.IdentityImpl;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Http;

//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(IocConfig), "RegisterDependencies")]

namespace Kantar.AdExpress.Bootstrapper
{
    public class IocConfig
    {
        public static void RegisterDependencies(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            var config = new HttpConfiguration();

            var toto = Assembly.Load("Kantar.AdExpress.ServiceRest");

            builder.RegisterApiControllers(toto);

            //DATA
            builder.RegisterType<AdExpressContext>().AsSelf();
            builder.RegisterType<IdentityContext>().AsSelf();
            builder.RegisterType<AdExpressUnitOfWork>().As<IUnitOfWork>();
            
            //IDENTITY
            builder.RegisterType<ApplicationUserManager>().As<IApplicationUserManager>();
            builder.RegisterType<ApplicationRoleManager>().As<IApplicationRoleManager>();
            builder.RegisterType(typeof(ApplicationIdentityUser)).As(typeof(IUser<int>));
            //builder.Register<IdentityContext>(b =>
            //{
            //    var context = new IdentityContext();
            //    return context;
            //});
            builder.Register(b => b.Resolve<IdentityContext>() as DbContext);
            builder.Register(b =>
            {
                var manager = IdentityFactory.CreateUserManager(b.Resolve<DbContext>());
                if (Startup.DataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<ApplicationIdentityUser, int>(
                            Startup.DataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            });
            builder.Register(b => IdentityFactory.CreateRoleManager(b.Resolve<DbContext>()));
            builder.Register(b => HttpContext.Current.GetOwinContext().Authentication);

            //BL
            builder.RegisterType<LoginService>().As<ILoginService>();
            //BUILD
            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }
    }
}
