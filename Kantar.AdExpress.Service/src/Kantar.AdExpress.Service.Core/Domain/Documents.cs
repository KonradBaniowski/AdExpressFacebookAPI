using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class Documents
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public List<InfosNews> InfosNews { get; set; }
    }
}
