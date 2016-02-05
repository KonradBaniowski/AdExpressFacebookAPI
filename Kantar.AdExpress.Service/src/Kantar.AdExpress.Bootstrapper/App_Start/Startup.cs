using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Bootstrapper
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IocConfig.RegisterDependencies(app);

            ConfigureAuth(app);
        }
    }
}
