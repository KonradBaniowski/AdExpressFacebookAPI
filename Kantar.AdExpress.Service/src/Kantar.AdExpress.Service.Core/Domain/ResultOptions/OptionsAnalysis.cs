using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class OptionsAnalysis
    {
        public GenericLevelOption MediaDetailLevel { get; set; }

        public GenericLevelOption ProductDetailLevel { get; set; }

        public ResultTypeOption ResultTypeOption { get; set; }

        public CheckBoxOption Evol { get; set; }

        public CheckBoxOption PDM { get; set; }

        public CheckBoxOption PDV { get; set; }

        public bool IsSelectRetailerDisplay { get; set; }

        public int SiteLanguage { get; set; }

        public UnitOption UnitOption { get; set; }
    }
}
