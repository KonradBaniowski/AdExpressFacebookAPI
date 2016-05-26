using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class GridResultResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public GridResult GridResult { get; set; }
    }
}
