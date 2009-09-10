#region Information
/*
 * Author : Y R'kaina
 * Created on : 04/09/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Date.Default {
    /// <summary>
    /// This class inherits from the Date Class which provides all the methods to Set and Upadte the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
    /// </summary>
    public class Date : TNS.AdExpressI.Date.Date {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Date() { }
        #endregion

        #region Override
        /// <summary>
        /// Get the last available date for which we have data in the data base (for a list of vehicle)
        /// </summary>
        /// <returns>dictionary with the list of pairs vehicleId:lastAvailableDate</returns>
        /// <remarks>This method has been added for Finland so the others countries don't need it</remarks>
        public override Dictionary<Vehicles.names, DateTime> GetLastAvailableDate() {
            return null;
        }
        #endregion

    }
}
