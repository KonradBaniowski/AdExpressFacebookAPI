using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class ClientInformation
    {
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public double BrowserMinorVersion { get; set; }
        public string BrowserPlatform { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string ServerMachineName { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
