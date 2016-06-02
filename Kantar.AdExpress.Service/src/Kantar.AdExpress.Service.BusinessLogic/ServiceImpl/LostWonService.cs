#define Debug

using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.LostWon;
using TNS.Classification.Universe;
using System.Reflection;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
  public  class LostWonService : ILostWonService
    {
        private WebSession _customerSession = null;

        public GridResult GetGridResult(string idWebSession)
        {
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_DYNAMIQUE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);

            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Lost/Won result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var lostWonResult = (ILostWonResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" 
            + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null,
            parameters, null, null);

            return lostWonResult.GetGridResult();

        }

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_DYNAMIQUE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);
           
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Lost/Won result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var lostWonResult = (ILostWonResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
            + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null,
            parameters, null, null);
            data = lostWonResult.GetResult();
            return data;
        }
    }
}
