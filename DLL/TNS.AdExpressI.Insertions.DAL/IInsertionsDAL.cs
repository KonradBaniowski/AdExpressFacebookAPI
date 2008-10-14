using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using System.Data;

namespace TNS.AdExpressI.Insertions.DAL
{
    public interface IInsertionsDAL
    {
        Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters);
        List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters, int fromDate, int toDate, int universId, Module module, bool sloganNotNull);
        DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters);
        DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters);
    }
}
