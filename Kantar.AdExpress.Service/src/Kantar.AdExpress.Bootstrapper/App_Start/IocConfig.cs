using Autofac;
using Autofac.Integration.Mvc;
using Kantar.AdExpress.Service.BusinessLogic.Identity;
using Kantar.AdExpress.Service.BusinessLogic.ServiceImpl;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.DataAccess;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Kantar.AdExpress.Service.DataAccess.IdentityImpl;
using Km.AdExpressClientWeb;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Kantar.AdExpress.Bootstrapper
{
    public class IocConfig
    {
        public static void RegisterDependencies(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule<AutofacWebTypesModule>();
            //API
            //var config = new HttpConfiguration();
            //var toto = Assembly.Load("Kantar.AdExpress.ServiceRest");
            //builder.RegisterApiControllers(toto);

            #region ...identity
            builder.RegisterType<ApplicationUserManager>().As<IApplicationUserManager>();
            builder.RegisterType<ApplicationRoleManager>().As<IApplicationRoleManager>();
            builder.RegisterType(typeof(ApplicationIdentityUser)).As(typeof(IUser<int>));

            builder.Register(b => b.Resolve<IdentityContext>() as DbContext);
            builder.Register(b =>
            {
                var manager = IdentityFactory.CreateUserManager(b.Resolve<DbContext>());
                if (Startup.DataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<ApplicationIdentityUser, int>(
                            Startup.DataProtectionProvider.Create("ASP.NET Identity")
                            );

                }
                return manager;
            });
            builder.Register(b => IdentityFactory.CreateRoleManager(b.Resolve<DbContext>()));
            builder.Register(b => HttpContext.Current.GetOwinContext().Authentication);

            #endregion

            //DATA
            builder.RegisterType<AdExpressContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<IdentityContext>().AsSelf();
            builder.RegisterType<AdExpressUnitOfWork>().As<IUnitOfWork>();
            builder.RegisterAssemblyTypes(Assembly.Load("Kantar.AdExpress.Service.BusinessLogic")).AsImplementedInterfaces();

            //BUILD
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //API 
            //var resolver = new AutofacWebApiDependencyResolver(container);
            //config.DependencyResolver = resolver;
            //GlobalConfiguration.Configuration.DependencyResolver = resolver;
            //app.UseAutofacMiddleware(container);
            //app.UseAutofacWebApi(config);
            //app.UseWebApi(config);
        }
    }
}
