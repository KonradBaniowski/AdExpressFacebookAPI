using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassReports;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class AnalysisService : IAnalysisService
    {
        private WebSession _customerSession = null;

        public GridResult GetGridResult(string idWebSession, ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var module = ModulesList.GetModule(WebConstantes.Module.Name.TABLEAU_DYNAMIQUE);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Product Class indicator"));
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            var param = new object[1];
            param[0] = _customerSession;

            var productClassLayer = (IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            var gridResult = productClassLayer.GetGridResult(sortOrder, columnIndex);
            return gridResult;
        }

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            var module = ModulesList.GetModule(WebConstantes.Module.Name.TABLEAU_DYNAMIQUE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);

            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var result = (IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            data = result.GetGenericProductClassReport();
            return data;
        }
    }
}
