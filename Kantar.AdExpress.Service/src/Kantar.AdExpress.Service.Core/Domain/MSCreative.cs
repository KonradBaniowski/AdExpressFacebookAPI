using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Classification;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class MSCreative
    {
        public Int64 Id { get; set; }

        public VehicleInformation Vehicle { get; set; }

        public Int64 NbVisuals { get; set; }

        public List<string> Visuals { get; set; }

        public string Class { get; set; }
    }
}
