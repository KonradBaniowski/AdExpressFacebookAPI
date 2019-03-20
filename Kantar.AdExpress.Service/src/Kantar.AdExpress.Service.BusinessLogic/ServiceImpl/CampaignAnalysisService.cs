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
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class CampaignAnalysisService : ICampaignAnalysisService
    {
        private WebSession _customerSession = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

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

                string periodBeginning = Dates.GetPeriodBeginningDate(_customerSession.PeriodBeginningDate, _customerSession.PeriodType).ToString("yyyyMMdd");
                string periodEnd = Dates.GetPeriodEndDate(_customerSession.PeriodEndDate, _customerSession.PeriodType).ToString("yyyyMMdd");

                gridResult = CampaignAnalysisResult.GetGridResult(_customerSession, periodBeginning, periodEnd);
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
            return gridResult;
        }

        public ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext)
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
                if (_customerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return data;
        }
    }
}
