using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Rolex.Style;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpressI.Rolex
{
    public interface IRolexResults
    {

        #region Properties
        /// <summary>
        /// Define Current Module
        /// </summary>
        string ResultControlId
        {
            set;
        }
        /// <summary>
        /// Define Theme
        /// </summary>
        string Theme
        {
            set;
        }
        /// <summary>
        /// Get Period Beginning Date 
        /// </summary>
        string PeriodBeginningDate
        {
            get;
        }

        /// <summary>
        /// Get Period EndDate
        /// </summary>
        string PeriodEndDate
        {
            get;
        }
        /// <summary>
        /// User Session
        /// </summary>
        WebSession Session
        { get; set; }

        /// <summary>
        /// Get / Set flag to specify if Rolex Schedule output is PDF
        /// </summary>
        RolexScheduleStyle RolexScheduleStyle
        { get; set; }


        #endregion

        /// <summary>
        /// Get HTML code for the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetHtml();
        /// <summary>
        /// Get HTML code for an excel export of the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetExcelHtml();
        /// <summary>
        /// Get HTML code for a pdf export of the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        string[] GetPDFHtml();

        /// <summary>
        /// Get HTML code for the rolex file
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetRolexFileHtml(GenericDetailLevel selectedDetailLevel, List<long> selectedLevelValues, out  List<string> visuals);




    }
}
