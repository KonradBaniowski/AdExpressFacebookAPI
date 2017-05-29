using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using NLog;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Utilities.Exceptions;
using TNS.AdExpressI.Portofolio;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PortfolioAlertService : IPortfolioAlertService
    {
        private WebSession _customerWebSession;

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public PortfolioAlertService(IApplicationUserManager userManager)
        {
        }

        public PortfolioAlertResultResponse GetPortfolioAlertResult(long alertId, long alertTypeId, string dateMediaNum)
        {
            IPortofolioResults portofolioResult = null;

            PortfolioAlertResultResponse response = new PortfolioAlertResultResponse();

            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);


                PortfolioAlert portfolioAlert = portofolioResult.GetPortfolioAlertResult(alertId, alertTypeId, dateMediaNum);

                response.Datas = portfolioAlert.Datas;
                response.Reminder = portfolioAlert.Reminder;

                return response;

            }
            catch (Exception ex)
            {
                //CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                //Logger.Log(LogLevel.Error, cwe.GetLog());

                throw new Exception();
            }

        }
    }
}
