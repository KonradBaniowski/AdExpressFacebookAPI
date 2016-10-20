using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IOptionAnalysisService
    {
        OptionsAnalysis GetOptions(string idWebSession, HttpContextBase httpContext);
        void SetOptions(string idWebSession, UserAnalysisFilter userFilter, HttpContextBase httpContext);
    }
}
