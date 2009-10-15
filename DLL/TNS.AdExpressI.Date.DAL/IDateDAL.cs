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

namespace TNS.AdExpressI.Date.DAL {
    /// <summary>
    /// This interface provides all the methods to determine specific dates for specific modules.
    /// The specific dates can be the last day in which we have data in the database or the last loaded year. 
    /// </summary>
    public interface IDateDAL {
        /// <summary>
        /// Calculate the first day from which we don't hava data in the database
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedVehicle">The madia type selected by the customer</param>
        /// <param name="startYear">Used to calculate the fist day not enable</param>
        DateTime GetFirstDayNotEnabled(WebSession webSession, long selectedVehicle, int startYear);
        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// </summary>
        /// <returns>Year</returns>
        int GetLastLoadedYear();
        /// <summary>
        /// Get the latest publication date
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="idVehicle">Media type identifier</param>
        /// <returns></returns>
        string GetLatestPublication(WebSession webSession, Int64 idVehicle);
    }
}
