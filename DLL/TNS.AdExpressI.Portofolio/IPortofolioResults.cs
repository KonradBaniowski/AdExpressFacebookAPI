#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;


namespace TNS.AdExpressI.Portofolio {
    
    /// <summary>
    /// Portofolio result Interface
    /// </summary>
    public interface IPortofolioResults {
        /// <summary>
        /// Get HTML code for some portofolio result
        ///  - SYNTHESIS
        ///  - NOVELTY
        ///  - DETAIL_MEDIA
        ///  - PERFORMANCES
		/// </summary>
		/// <param name="page">Page</param>
		/// <returns>HTML Code</returns>
        string GetHtml(Page page);
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
		/// </summary>
		/// <returns>Result Table</returns>
        ResultTable GetResultTable();
    }
}
