using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class PDVByMediaPageFacebookContract
    {
        public long IdVehicle { get; set; }
        public string LabelVehicle { get; set; }

        public long Id { get; set; }
        public string Label { get; set; }

        public double Expenditure { get; set; }

        public long MaxDataCommon { get; set; }

    }
}
