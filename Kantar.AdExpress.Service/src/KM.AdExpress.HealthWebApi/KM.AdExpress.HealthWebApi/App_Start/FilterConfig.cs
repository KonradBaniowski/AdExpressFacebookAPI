using System.Web;
using System.Web.Mvc;

namespace KM.AdExpress.HealthWebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
