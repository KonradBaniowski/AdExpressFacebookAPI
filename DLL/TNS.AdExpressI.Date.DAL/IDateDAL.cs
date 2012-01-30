#region Information
/*
 * Author : Y R'kaina
 * Created on : 09/10/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;

namespace TNS.AdExpressI.Date.DAL {
    /// <summary>
    /// This interface provides all the methods to determine specific dates for specific modules.
    /// The specific dates can be the last day in which we have data in the database or the last loaded year. 
    /// </summary>
    public interface IDateDAL {
        /// <summary>
        /// Calculate the first day from which we don't have data in the database
        /// We check for the vehicles list selected (associated to the media type selected) the last publication date in the data table.
        /// if we don't have data, we calculate the previous year according to the startYear variable and the datetime returned is the last day of the previous year.
        /// <remarks>
        /// This method is used in the date selection page to show periods that have not yet been completely uploaded
        /// and this information is represented by using dark gray color in the calendar 
        /// </remarks>
        /// <example>
        /// The sub media selected : press
        /// startYear : 2008
        /// case 1 :
        ///     last publication date : 15/06/2008
        ///     value returned : last publication date + periodicity (daily : periodicity = 1 day, weekly : periodicity = 7 days, monthly : periodicity = 1 month)
        /// case 2 :
        ///     last publication date : not found = no data
        ///     value returned : 31/12/2007
        /// </example>
        /// </summary>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <param name="startYear">Used to calculate the fist day not enable : date format yyyy, example : 2008</param>
        DateTime GetFirstDayNotEnabled(List<Int64> selectedVehicleList, int startYear);
        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// </summary>
        /// <returns>Year</returns>
        int GetLastLoadedYear();
        /// <summary>
        /// Get the last publication date for a list of vehicles according to the media type passed to the method
        /// At first we select the max date for every vehicle, after that we select the min date from the list built in the first step 
        /// </summary>
        /// <example>
        /// The media type selected is press 
        /// The max date of the first vehicle is 01/05/2008
        /// The max date of the second vehicle is 15/04/2008
        /// The max date of the third vehicle is 21/01/2008
        /// The date returned will be the min of the three previous date so : 21/01/2008
        /// </example>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <returns>Last date publication : date format yyyyMMdd, example 20081015</returns>
        string GetLatestPublication(List<Int64> selectedVehicleList);
        /// <summary>
        /// Get the last available date for which we have data in the data base (for the media type selected)
        /// </summary>
        /// <param name="vehicleId">Media Type Identifier</param>
        /// <returns>DataSet containing the result of the SQL request</returns>
        DataSet GetLastAvailableDate(Int64 vehicleId);
        /// <summary>
        /// Get the calendar starting date
        /// </summary>
        /// <returns>The year corresponding to the starting date : date format yyyy example 2008</returns>
        int GetCalendarStartDate();

        /// <summary>
        /// Get Media type's last available date  which has data 
        /// </summary>
        /// <remarks>Use particularly in  Product class analysis modules</remarks>
        /// <param name="idVehicle">Media type ID</param>
        /// <param name="webSession">Session</param>
        /// <returns>Last available month which has data : YYYYMM</returns>
        string CheckAvailableDateForMedia(Int64 idVehicle);
        /// <summary>
        /// Check period depending on data delivering frequency
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="EndPeriod">End period</param>
        /// <returns>Period End</returns>
        string CheckPeriodValidity(WebSession _session, string EndPeriod);
    }
}
