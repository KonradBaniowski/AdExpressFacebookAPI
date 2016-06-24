using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class Data
    {
        public long IdSegment { get; set; }
        public long IdGroup { get; set; }
        public long IdSubSector { get; set; }
        public long IdSector { get; set; }
        public long IdAdvertiser { get; set; }
        public long IdBrand { get; set; }
        public long IdCategory { get; set; }
        public long IdVehicle { get; set; }
        public long IdMedia { get; set; }
    }
}
