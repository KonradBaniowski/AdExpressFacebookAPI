using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;

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

        public CheckBoxOption InitializeMedia { get; set; }

        public CheckBoxOption InitializeProduct { get; set; }

        public int SiteLanguage { get; set; }

        public bool ComparativeStudy { get; set; }

        public globalCalendar.comparativePeriodType ComparativePeriodType { get; set; }

        public CheckBoxOption Evol { get; set; }

        public CheckBoxOption PDM { get; set; }

        public CheckBoxOption PDV { get; set; }

        public bool IsSelectRetailerDisplay { get; set; }

        public PercentageOption PercentageOption { get; set; }
    }
}
