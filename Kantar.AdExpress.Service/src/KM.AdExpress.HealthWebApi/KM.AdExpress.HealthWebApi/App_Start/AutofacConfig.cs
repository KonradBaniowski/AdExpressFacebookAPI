using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using KM.AdExpress.Health.Core.Interfaces;
using KM.AdExpress.Health.Infrastructure;
using KM.AdExpress.Health.Infrastructure.App_Start;
using System.Reflection;
using System.Web.Http;

namespace KM.AdExpress.HealthWebApi.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;


            builder.RegisterType<HealthContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<HealthUow>().As<IHealthUow>();

            var mapper = new AutoMapperConfig().mapper;
            builder.RegisterInstance(mapper).As<IMapper>();

            builder.RegisterAssemblyTypes(Assembly.Load("KM.AdExpress.Health.Infrastructure")).AsImplementedInterfaces();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}