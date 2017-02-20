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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class LostWonService : ILostWonService
    {
        private WebSession _customerSession = null;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public GridResult GetGridResult(string idWebSession, HttpContextBase httpContext)
        {
            GridResult gridResult = new GridResult();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                string sortKey = httpContext.Request.Cookies["sortKey"].Value;
                string sortOrder = httpContext.Request.Cookies["sortOrder"].Value;

                if (!string.IsNullOrEmpty(sortKey) && !string.IsNullOrEmpty(sortOrder))
                {
                    _customerSession.SortKey = sortKey;
                    _customerSession.Sorting = (ResultTable.SortOrder)Enum.Parse(typeof(ResultTable.SortOrder), sortOrder);
                    _customerSession.Save();
                }

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
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return gridResult;
}

        public ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext)
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
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return data;
        }
    }
}
