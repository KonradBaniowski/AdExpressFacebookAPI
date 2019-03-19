using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;

using System.Reflection;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.NewCreatives;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.WebResultUI;
using NLog;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class NewCreativesService : INewCreativesService
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

                var module = ModulesList.GetModule(WebConstantes.Module.Name.NEW_CREATIVES);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Product Class indicator"));
                var param = new object[1];
                param[0] = _customerSession;

                var newCreativesResult = (INewCreativesResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                gridResult = newCreativesResult.GetGridResult();
            }
            catch(Exception ex)
            {
                if (_customerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

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
                var module = ModulesList.GetModule(WebConstantes.Module.Name.NEW_CREATIVES);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var result = (INewCreativesResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                data = result.GetData();
            }
            catch(Exception ex)
            {
                if (_customerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return data;
        }

        public long CountDataRows(string idWebSession, HttpContextBase httpContext)
        {
            long nbRows = 0;
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.NEW_CREATIVES);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var result = (INewCreativesResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                nbRows = result.CountData();
            }
            catch (Exception ex)
            {
                if (_customerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return nbRows;
        }
    }
}
