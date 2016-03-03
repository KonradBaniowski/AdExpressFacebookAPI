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
        public List<Media> Media { get; set; }
        public int SiteLanguage { get; set; }

        public List<int> MediaCommon { get; set; }
    }
}
