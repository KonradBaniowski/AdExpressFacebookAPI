using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class Media
    {
        public string icon { get; set; }
        public Vehicles.names MediaEnum { get; set; }
        public long Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }

    public class MediaResponse
    {
        public MediaResponse(int siteLanguage)
        {
            ControllerDetails = new ControllerDetails();
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
            SiteLanguage = siteLanguage;
        }
        public MediaResponse()
        {
            ControllerDetails = new ControllerDetails();
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
        }
        public List<Media> Media { get; set; }
        public int SiteLanguage { get; set; }
        public List<int> MediaCommon { get; set; }
        public ControllerDetails ControllerDetails { get; set;}
    }

    public class ControllerDetails
    {
        public ControllerDetails()
            {
            ControllerCode = 0;
            Name = string.Empty;
            ModuleId = 0;
        }
        public long ControllerCode { get; set; }
        public string Name { get; set; }
        public long ModuleId { get; set; }
    }
}
