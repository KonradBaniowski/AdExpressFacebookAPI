using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class Options
    {
        public GenericDetailLevelOption GenericDetailLevel { get; set; }

        public GenericColumnDetailLevelOption GenericColumnDetailLevelOption { get; set; }

        public PeriodDetailOption PeriodDetail { get; set; }

        public UnitOption UnitOption { get; set; }

        public InsertionOption InsertionOption { get; set; }

        public AutoPromoOption AutoPromoOption { get; set; }

        public FormatOption FormatOption { get; set; }

        public PurchaseModeOption PurchaseModeOption { get; set; }

        public ResultTypeOption ResultTypeOption { get; set; }

        public int SiteLanguage { get; set; }
    }
}
