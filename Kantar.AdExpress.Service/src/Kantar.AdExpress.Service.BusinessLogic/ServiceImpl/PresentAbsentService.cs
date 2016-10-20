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
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
   public class PresentAbsentService : IPresentAbsentService
    {
        private WebSession _customerSession = null;
        private static Logger Logger= LogManager.GetCurrentClassLogger();

        public GridResultResponse GetGridResult(string idWebSession, HttpContextBase httpContext)
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
            catch (Exception ex)
            {
                gridResultResponse.Success = false;
                gridResultResponse.Message = string.Format("{0}.{1}", GestionWeb.GetWebWord(1973, _customerSession.SiteLanguage), GestionWeb.GetWebWord(2099, _customerSession.SiteLanguage));

                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return gridResultResponse;
        }

        public ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext)
        {
            ResultTable data = null;           
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the present/absent result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var presentAbsentResult = (IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                data = presentAbsentResult.GetResult();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return data;
        }
    }
}
