using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.Core.Domain
{
  public  class InsertionCreativeResponse
    {

        public GridResult GridResult { get; set; }

        public string Message { get; set; }

        public long IdVehicle { get; set; }

        protected IList<VehicleInformation> _vehicles = new List<VehicleInformation>();

        public IList<VehicleInformation> Vehicles
        {
            get
            {
                return _vehicles;
            }
            set
            {
                _vehicles = value;
                List<long> ids = new List<long>();
                foreach (VehicleInformation v in value)
                {
                    ids.Add(v.DatabaseId);
                }
                if (IdVehicle <= 0 && Vehicles.Count > 0)
                {
                    IdVehicle = Vehicles[0].DatabaseId;
                }
            }
        }

    }
}
