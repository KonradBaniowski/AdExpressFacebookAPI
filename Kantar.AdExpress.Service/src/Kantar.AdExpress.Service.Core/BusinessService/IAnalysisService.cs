using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IAnalysisService
    {
        ResultTable GetResultTable(string idWebSession);
        GridResult GetGridResult(string idWebSession, ResultTable.SortOrder sortOrder, int columnIndex);

    }
}
