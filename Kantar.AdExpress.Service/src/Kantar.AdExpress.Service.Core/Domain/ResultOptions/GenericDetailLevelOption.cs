using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class GenericDetailLevelOption
    {
        public SelectControl DefaultDetail { get; set; }

        public SelectControl CustomDetail { get; set; }

        public SelectControl L1Detail { get; set; }

        public SelectControl L2Detail { get; set; }

        public SelectControl L3Detail { get; set; }

        public SelectControl L4Detail { get; set; }
    }
}
