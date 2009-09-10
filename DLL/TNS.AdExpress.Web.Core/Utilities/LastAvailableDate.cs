#region Information
// Author: Y. R'kaina
// Creation date: 09/09/2009
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpress.Web.Core.Utilities {
    /// <summary>
    /// Represents a dictionary with the list of pairs vehicleId:lastAvailableDate
    /// </summary>
    public class LastAvailableDate {

        #region Variables
        /// <summary>
        /// Dictionary with the list of pairs vehicleId:lastAvailableDate
        /// </summary>
        static Dictionary<Vehicles.names, DateTime> _lastAvailableDateList;
        #endregion

        #region Accessors
        /// <summary>
        /// Get a dictionary with the list of pairs vehicleId:lastAvailableDate
        /// </summary>
        public static Dictionary<Vehicles.names, DateTime> LastAvailableDateList {
            get { return _lastAvailableDateList; }
        }
        #endregion

        #region Init
        /// <summary>
        /// Use to initialize the lastAvailableDateList object
        /// </summary>
        /// <param name="lastAvailableDateList"></param>
        public static void Init(Dictionary<Vehicles.names, DateTime> lastAvailableDateList) {
            _lastAvailableDateList = new Dictionary<Vehicles.names, DateTime>();
            _lastAvailableDateList = lastAvailableDateList;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the last available date
        /// </summary>
        /// <returns>Last available date</returns>
        public static DateTime GetLastAvailableDate(WebSession webSession) {

            DateTime lastAvailableDate = new DateTime(1,1,1);

            foreach(DateTime currentDate in LastAvailableDate.LastAvailableDateList.Values){
                if (lastAvailableDate < currentDate)
                    lastAvailableDate = currentDate;
            }

            return lastAvailableDate;
        }
        #endregion
    }
}
