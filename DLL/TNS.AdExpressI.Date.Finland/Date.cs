using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.Date.Finland {
    /// <summary>
    /// This class inherits from the Date Class which provides all the methods to Set and Upadte the websession date attributes (PeriodType, PeriodLength, PeriodBeginningDate, PeriodEndDate, DetailPeriod ...)
    /// provides also a dictionary with the list of pairs vehicleId:lastAvailableDate
    /// </summary>
    class Date : TNS.AdExpressI.Date.Date {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Date() { }
        #endregion

        #region Override

        #region SetCurrentYear
        /// <summary>
        /// Set date attributes for the current year
        /// </summary>
        /// <param name="webSession">The customer session</param>
        public override void SetCurrentYear(ref WebSession webSession) {

            webSession.PeriodType = CustomerSessions.Period.Type.currentYear;
            webSession.PeriodLength = 1;
            webSession.PeriodEndDate = LastAvailableDate.GetLastAvailableDate(webSession).ToString("yyyyMMdd");
            webSession.PeriodBeginningDate = webSession.PeriodEndDate.Substring(0,4) + "0101";
            webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
        }
        #endregion

        #endregion

    }
}
