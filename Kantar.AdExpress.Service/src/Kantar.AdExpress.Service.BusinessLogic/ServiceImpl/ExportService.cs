using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class ExportService :IExportService
    {
        public ExportResponse Export (ExportRequest request)
        {
            ExportResponse response = new ExportResponse();
            return response;
        }
    }
}
