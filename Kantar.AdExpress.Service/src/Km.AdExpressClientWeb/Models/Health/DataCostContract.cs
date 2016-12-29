using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Health
{
    public class DataCostContract
    {
        public double IdCanal { get; set; }
        public string Canal { get; set; }
        public double IdCategory { get; set; }
        public string Category { get; set; }
        public double IdSpecialist { get; set; }
        public string Specialist { get; set; }
        public double IdGrpPharma { get; set; }
        public string GrpPharma { get; set; }
        public double IdLabratory { get; set; }
        public string Laboratory { get; set; }
        public double IdProduct { get; set; }
        public string Product { get; set; }
        public double IdFormat { get; set; }
        public string Format { get; set; }
        public DateTime Date { get; set; }
        public double Euro { get; set; }
    }
}