using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class HealthMediaResponse
    {
        public HealthMediaResponse(int siteLanguage)
        {
            Medias = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
            MultipleSelection = true;
            SiteLanguage = siteLanguage;
            CanRefineMediaSupport = true;
        }
        public HealthMediaResponse()
        {
            Medias = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
        }

        public List<Media> Medias { get; set; }
        public List<int> MediaCommon { get; set; }
        public bool MultipleSelection { get; set; }
        public ControllerDetails ControllerDetails { get; set; }
        public string ErrorMessage { get; set; }
        public int SiteLanguage { get; set; }
        public bool Success { get; set; }

        public bool CanRefineMediaSupport { get; set; }
    }
}
