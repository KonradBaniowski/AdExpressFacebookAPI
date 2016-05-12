using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class  ExportRequest
    {
        public string FileName { get; set; }
        public string Email { get; set; }
        public string WebSessionId { get; set; }
        public string ExportType { get;set;}
    }
    public class ExportResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

}
