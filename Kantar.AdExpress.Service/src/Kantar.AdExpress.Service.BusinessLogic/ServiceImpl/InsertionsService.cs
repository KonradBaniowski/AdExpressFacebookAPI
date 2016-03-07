using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class InsertionsService : IInsertionsService
    {
        private WebSession _customerWebSession = null;
        private int _fromDate;
        private int _toDate;


        public GridResult GetGridResult(string idWebSession, string ids, string zoomDate, string idUnivers, long moduleId)
        {

            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            IInsertionsResult insertionResult = InitInsertionCall(_customerWebSession, moduleId);

            //date
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
            string periodBegin = _customerWebSession.PeriodBeginningDate;
            string periodEnd = _customerWebSession.PeriodEndDate;

            if (!string.IsNullOrEmpty(zoomDate))
            {
                periodType = _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly
                    ? TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateWeek : TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth;
                _fromDate = Convert.ToInt32(
                    Dates.Max(Dates.getZoomBeginningDate(zoomDate, periodType),
                        Dates.getPeriodBeginningDate(periodBegin, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
                _toDate = Convert.ToInt32(
                    Dates.Min(Dates.getZoomEndDate(zoomDate, periodType),
                        Dates.getPeriodEndDate(periodEnd, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
            }
            else
            {
                _fromDate = Convert.ToInt32(Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                _toDate = Convert.ToInt32(Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
            }


            return null;

        }


        public IInsertionsResult InitInsertionCall(WebSession custSession, long moduleId)
        {
            //**TODO : IdVehicules not null


            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertions];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
            var param = new object[2];
            param[0] = custSession;
            param[1] = moduleId;
            var result = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);


            return result;

        }


    }
}
