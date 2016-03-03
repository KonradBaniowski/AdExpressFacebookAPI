using Kantar.AdExpress.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain.BusinessService;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PeriodService : IPeriodService
    {
        public PeriodResponse GetPeriod(string idWebSession)
        {
            WebSession webSession = (WebSession)WebSession.Load(idWebSession);
         
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = webSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            int startYear = dateDAL.GetCalendarStartDate();
            int endYear = DateTime.Now.Year;
            var result = new PeriodResponse
            {
                StartYear = startYear,
                EndYear = endYear,
                SiteLanguage = webSession.SiteLanguage
            };

            return result;
        }
    }
}
