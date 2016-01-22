using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.Mau
{
    public class ModuleAssignment
    {
        public int IdModule { get; set; }
        public int IdLogin { get; set; }
        public int Activation { get; set; }
        public DateTime DateDebutModule { get; set; }
        public DateTime DateFinModule { get; set; }

    }
}
