using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Web.Core.Sessions;
using NLog;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.CampaignAnalysis;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Translation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class CampaignAnalysisService : ICampaignAnalysisService
    {
        private WebSession _customerSession = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public GridResult GetGridResult(string idWebSession)
        {
            GridResult gridResult = new GridResult();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                string periodBeginning = Dates.GetPeriodBeginningDate(_customerSession.PeriodBeginningDate, _customerSession.PeriodType).ToString("yyyyMMdd");
                string periodEnd = Dates.GetPeriodEndDate(_customerSession.PeriodEndDate, _customerSession.PeriodType).ToString("yyyyMMdd");

                gridResult = CampaignAnalysisResult.GetGridResult(_customerSession, periodBeginning, periodEnd);
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
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
                string periodBeginning = Dates.GetPeriodBeginningDate(_customerSession.PeriodBeginningDate, _customerSession.PeriodType).ToString("yyyyMMdd");
                string periodEnd = Dates.GetPeriodEndDate(_customerSession.PeriodEndDate, _customerSession.PeriodType).ToString("yyyyMMdd");

                data = CampaignAnalysisResult.GetData(_customerSession, periodBeginning, periodEnd);
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, _customerSession.UserAgent, _customerSession.CustomerLogin.Login, _customerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_customerSession.CurrentModule), _customerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return data;
        }
    }
}
