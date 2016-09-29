using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UserFilter
    {
        public GenericDetailLevelFilter GenericDetailLevelFilter { get; set; }

        public GenericColumnDetailLevelFilter GenericColumnDetailLevelFilter { get; set; }

        public PeriodDetailFilter PeriodDetailFilter { get; set; }

        public UnitFilter UnitFilter { get; set; }

        public InsertionFilter InsertionFilter { get; set; }

        public AutoPromoFilter AutoPromoFilter { get; set; }

        public FormatFilter FormatFilter { get; set; }

        public PurchaseModeFilter PurchaseModeFilter { get; set; }

        public ResultTypeFilter ResultTypeFilter { get; set; }

        public bool InitializeMedia { get; set; }

        public bool InitializeProduct { get; set; }

        public bool ComparativeStudy { get; set; }

        public int ComparativePeriodType { get; set; }

        public bool Evol { get; set; }

        public bool PDM { get; set; }

        public bool PDV { get; set; }

        public bool IsSelectRetailerDisplay { get; set; }

        public PercentageFilter PercentageFilter { get; set; }
    }
}
