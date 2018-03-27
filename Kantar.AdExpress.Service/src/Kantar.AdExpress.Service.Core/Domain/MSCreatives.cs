using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class MSCreatives
    {
        public List<MSCreative> Items { get; set; }

        public Vehicles.names VehicleId;

        public int SiteLanguage { get; set; }
    }
}
