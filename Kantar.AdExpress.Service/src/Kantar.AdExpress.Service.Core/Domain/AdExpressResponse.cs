using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class AdExpressResponse
    {
        public bool Success { get; set; }
        
        public string Message { get; set;} 
        public long ModuleId { get; set; }
    }
}
