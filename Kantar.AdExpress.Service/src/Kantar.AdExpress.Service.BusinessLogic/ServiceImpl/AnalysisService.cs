using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;

using System.Reflection;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassReports;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.WebResultUI;
using NLog;
using TNS.AdExpress.Domain.Translation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class AnalysisService : IAnalysisService
    {
        private WebSession _customerSession = null;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public GridResult GetGridResult(string idWebSession, ResultTable.SortOrder sortOrder, int columnIndex)
        {
            GridResult gridResult = new GridResult();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.TABLEAU_DYNAMIQUE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Product Class indicator"));
                var param = new object[1];
                param[0] = _customerSession;

                var productClassLayer = (IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                gridResult = productClassLayer.GetGridResult(sortOrder, columnIndex);
            }
            catch(Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return gridResult;
        }

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.TABLEAU_DYNAMIQUE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var result = (IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                data = result.GetGenericProductClassReport();
            }
            catch(Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return data;
        }
    }
}
