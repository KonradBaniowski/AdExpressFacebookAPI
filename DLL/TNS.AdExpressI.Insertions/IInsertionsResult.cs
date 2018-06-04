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
using System.Data;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions
{
    public interface IInsertionsResult
    {

        #region Properties
        /// <summary>
        /// Define Render Type
        /// </summary>
        TNS.FrameWork.WebResultUI.RenderType RenderType
        {
            get;
            set;
        }


        /// <summary>
        ///Use Blur Image For Press
        /// </summary>
        bool UseBlurImageForPress
        {
            get;
            set;
        }
        #endregion

        ResultTable GetInsertions(VehicleInformation vehicle, int fromDate,
            int toDate, string filters, int universId, string zoomDate);

        ResultTable GetInsertionsExcel(VehicleInformation vehicle, int fromDate,
            int toDate, string filters, int universId, string zoomDate);

        GridResult GetInsertionsGridResult(VehicleInformation vehicle, int fromDate,
         int toDate, string filters, int universId, string zoomDate);


        ResultTable GetCreatives(VehicleInformation vehicle, int fromDate, 
            int toDate, string filters, int universId, string zoomDate);

        GridResult GetCreativesGridResult(VehicleInformation vehicle, int fromDate,
          int toDate, string filters, int universId, string zoomDate,
          List<GenericColumnItemInformation> columnFilters, Dictionary<GenericColumnItemInformation.Columns, 
              List<string>> availableFilterValues, Dictionary<GenericColumnItemInformation.Columns, List<string>> customFilterValues
            ,WebSession session = null);

        ResultTable GetMSCreatives(VehicleInformation vehicle, int fromDate,
            int toDate, string filters, int universId, string zoomDate);

        GridResult GetMSCreativesGridResult(VehicleInformation vehicle, int fromDate,
         int toDate, string filters, int universId, string zoomDate);

        List<VehicleInformation> GetPresentVehicles(string filters, int universId, bool sloaganNotNull);
          /// <summary>
        /// True if can show insertion 
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns>True if can show insertion</returns>
        bool CanShowInsertion(VehicleInformation vehicle);

        /// <summary>
        /// Get creative Links
        /// </summary>
        /// <param name="idVehicle">Identifier Vehicle</param>
        /// <param name="currentRow">Current row</param>
        /// <returns>Creative Links string</returns>
        string GetCreativeLinks(long idVehicle, DataRow currentRow);

     
    }
}
