using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UniverseItem
    {
        public int IdItem { get; set; }
        public string Label { get; set; }
        //TODO USEFULL?
        public int IdLevelUniverse { get; set; }
    }
}
