#define Debug

using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.PresentAbsent;
using System.Reflection;
using TNS.Classification.Universe;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Translation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
   public class PresentAbsentService : IPresentAbsentService
    {
        private WebSession _customerSession = null;

        public GridResultResponse GetGridResult(string idWebSession)
        {
            GridResultResponse gridResultResponse = new GridResultResponse();
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE);
                _customerSession = (WebSession)WebSession.Load(idWebSession);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the present/absent result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var presentAbsentResult = (IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                var gridResult = presentAbsentResult.GetGridResult();
                gridResultResponse.Success = true;
                gridResultResponse.GridResult = gridResult;
            }
            catch (Exception e)
            {
                gridResultResponse.Success = false;
                gridResultResponse.Message = string.Format("{0}.{1}", GestionWeb.GetWebWord(1973, _customerSession.SiteLanguage), GestionWeb.GetWebWord(2099, _customerSession.SiteLanguage));

            }
            return gridResultResponse;
        }

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);

            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the present/absent result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var presentAbsentResult = (IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            data = presentAbsentResult.GetResult();
            return data;
        }
    }
}
