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

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
   public class PresentAbsentService : IPresentAbsentService
    {
        private WebSession _customerSession = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            catch (Exception ex)
            {
                gridResultResponse.Success = false;
                gridResultResponse.Message = string.Format("{0}.{1}", GestionWeb.GetWebWord(1973, _customerSession.SiteLanguage), GestionWeb.GetWebWord(2099, _customerSession.SiteLanguage));
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            return gridResultResponse;
        }

        public ResultTable GetResultTable(string idWebSession)
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
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", _customerSession.IdSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            return data;
        }
    }
}
