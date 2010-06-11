#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion


using System;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using System.Collections;


namespace TNS.AdExpressI.Portofolio {
    
    /// <summary>
    /// Portofolio result Interface
    /// </summary>
    public interface IPortofolioResults {	

		/// <summary>
		/// Get Structure html result
		/// </summary>
		/// <param name="excel">True if export excel</param>
		/// <returns>html code</returns>
		string GetStructureHtml(bool excel);
		/// <summary>
		/// Get structure chart data
		/// </summary>
		/// <returns></returns>
		DataTable GetStructureChartData();		
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
		/// </summary>
		/// <returns>Result Table</returns>
        ResultTable GetResultTable();
		/// <summary>
		/// Get portofolio media detail insertion result
		/// </summary>
		/// <returns>Result Table</returns>
		ResultTable GetInsertionDetailResultTable();
        /// <summary>
        /// Get data for vehicle view
        /// </summary>
        /// <param name="dtVisuel">Visuel information</param>
        /// <param name="htValue">investment values</param>
        /// <returns>Media name</returns>
        string GetVehicleViewData(out DataTable dtVisuel, out Hashtable htValue);
        /// <summary>
        /// Get detail media html
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <returns>HTML Code</returns>
        string GetDetailMediaHtml(bool excel);
		/// <summary>
		/// Get visual list
		/// </summary>
		/// <param name="beginnDate">beginning Date</param>
		/// <param name="endDate">end Date</param>
		/// <returns></returns>
		Dictionary<string, string> GetVisualList(string beginnDate, string endDate);
	
    }
}
