using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class HealthMediaRequest
    {
        public string IdSession { get; set; }

        public ClientInformation ClientInformation { get; set; }
        
    }
}
