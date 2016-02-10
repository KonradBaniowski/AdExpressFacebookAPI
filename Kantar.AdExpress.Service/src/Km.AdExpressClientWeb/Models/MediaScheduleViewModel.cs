using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Classification;

namespace Km.AdExpressClientWeb.Models
{
    public class MediaSelectionViewModel
    {
        public List<Media> Medias { get; set; }
        public bool Multiple { get; set; }
        public List<int> IdMediasCommon { get; set; }

    }

    public class Media
    {
        public string icon { get; set; }
        public VehiclesEnum MediaEnum { get; set; }
        public int Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }
}