using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Core.Model
{
    public class DataCost
    {
        public double IdCanal { get; set; }
        public double IdCategory { get; set; }
        public double IdSpecialist { get; set; }
        public double IdGrpPharma { get; set; }
        public double IdLaboratory { get; set; }
        public double IdProduct { get; set; }
        public double IdFormat { get; set; }
        public DateTime Date { get; set; }
        public double Euro { get; set; }
        public Canal Canal { get; set; }
        public Category Category { get; set; }
        public Format Format { get; set; }
        public GrpPharma GrpPharma { get; set; }
        public Product Product { get; set; }
        public Specialist Specialist { get; set; }
        public Laboratory Laboratory { get; set; }

    }
}
