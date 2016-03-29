using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class DetailLevel
    {
        public int level { get; set; }

        public List<DetailLevelItems> Items { get; set; }
    }
}
