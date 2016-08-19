#define Debug

using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.LostWon;
using System.Reflection;
using NLog;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class LostWonService : ILostWonService
    {
        private WebSession _customerSession = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public GridResult GetGridResult(string idWebSession)
        {
            GridResult gridResult = new GridResult();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_DYNAMIQUE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Lost/Won result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var lostWonResult = (ILostWonResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null,
                parameters, null, null);
                gridResult = lostWonResult.GetGridResult();
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            return gridResult;
}

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_DYNAMIQUE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Lost/Won result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var lostWonResult = (ILostWonResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null,
                parameters, null, null);
                data = lostWonResult.GetResult();
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}, user agent: {1}, Login: {2}, password: {3}, error: {4}, StackTrace: {5}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace);
                logger.Log(LogLevel.Error, message);
            }
            return data;
        }
    }
}
