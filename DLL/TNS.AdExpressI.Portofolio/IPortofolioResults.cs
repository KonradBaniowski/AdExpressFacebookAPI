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
        /// Get view of the vehicle (HTML)
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        /// <returns>HTML Code</returns>
        string GetVehicleViewHtml(bool excel, int resultType);
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
