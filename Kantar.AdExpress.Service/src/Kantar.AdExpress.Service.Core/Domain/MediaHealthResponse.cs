using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class MediaHealthResponse
    {
        public MediaHealthResponse(int siteLanguage, long currentModule)
        {
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
            MultipleSelection = true;
        }
        public MediaHealthResponse()
        {
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
        }
      
        public List<Media> Media { get; set; }
        public List<int> MediaCommon { get; set; }
        public bool MultipleSelection { get; set; }
        public ControllerDetails ControllerDetails { get; set; }
        public string ErrorMessage { get; set; }
    }
}
