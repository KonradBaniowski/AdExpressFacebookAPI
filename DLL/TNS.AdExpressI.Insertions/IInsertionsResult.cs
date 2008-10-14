#region Info
/*
 * Author : G Ragneau 
 * Created on 23/09/2008
 * Modifications :
 *      datre - Author - Description
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Insertions
{
    public interface IInsertionsResult
    {

        ResultTable GetInsertions(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId);
        ResultTable GetCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId);
        List<VehicleInformation> GetPresentVehicles(string filters, int universId, bool sloaganNotNull);
    }
}
