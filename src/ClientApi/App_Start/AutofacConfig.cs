using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Facebook.DataAccess;
using Facebook.Service.BusinessLogic;
using Facebook.Service.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ClientApi.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register dependencies in controllers
            builder.RegisterControllers(typeof(WebApiApplication).Assembly);

            // Register dependencies in filter attributes
            builder.RegisterFilterProvider();

            // Register dependencies in custom views
            builder.RegisterSource(new ViewRegistrationSource());

            // Register our Data dependencies
            //builder.RegisterModule(new DataModule("MVCWithAutofacDB"));
            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterType<FacebookContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<FacebookUow>().As<IFacebookUow>();
            var mapper = new AutoMapperConfig().mapper;
            builder.RegisterInstance(mapper).As<IMapper>();
            builder.RegisterAssemblyTypes(Assembly.Load("Facebook.Service.BusinessLogic")).AsImplementedInterfaces();

            var container = builder.Build();
          
            // Set MVC DI resolver to use our Autofac container
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}