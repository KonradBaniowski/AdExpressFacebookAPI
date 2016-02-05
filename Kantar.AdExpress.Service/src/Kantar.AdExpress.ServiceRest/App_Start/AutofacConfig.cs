
using Autofac;
using Autofac.Integration.WebApi;
using Kantar.AdExpress.Service.BusinessLogic.Identity;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.DataAccess;
using Kantar.AdExpress.Service.DataAccess;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Kantar.AdExpress.Service.DataAccess.IdentityImpl;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Kantar.AdExpress.ServiceRest.App_Start
{
	public class AutofacConfig
    {
        public static void RegisterDependencies()
        {
         
        }
    }
}