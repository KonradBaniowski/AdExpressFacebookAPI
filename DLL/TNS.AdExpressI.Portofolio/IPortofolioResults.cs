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
		///// <summary>
		///// Get Press structure result
		///// </summary>
		///// <param name="portofolioDAL">Portofolio DAL</param>
		///// <param name="dtFormat">data table Format</param>
		///// <param name="dtColor">data table Color</param>
		///// <param name="dtLocation">data table Location</param>
		///// <param name="dtInsert">data table Inset</param>
		//void GetStructPressResult(out DataTable dtFormat, out DataTable dtColor, out DataTable dtLocation, out DataTable dtInsert);
		///// <summary>
		///// Get structure Tv result
		///// </summary>
		///// <returns></returns>
		//object[,] GetStructTV();
		///// <summary>
		///// Get structure Radio result
		///// </summary>
		///// <returns></returns>
		//object[,] GetStructRadio();
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
        /// <returns>HTML Code</returns>
        string GetVehicleViewHtml(bool excel);
        /// <summary>
        /// Get detail media html
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <returns>HTML Code</returns>
        string GetDetailMediaHtml(bool excel);
		///// <summary>
		///// Get detail media for press
		///// </summary>
		///// <param name="excel">True for excel result</param>
		///// <returns>HTML Code</returns>
		//string GetDetailMediaPressHtml(bool excel);
		//    /// <summary>
		///// Get detail media for tv & radio
		///// </summary>
		///// <param name="excel">True for excel result</param>
		///// <returns>HTML Code</returns>
		//string GetDetailMediaTvRadioHtml(bool excel);

		///// <summary>
		///// Get portofolio media detail insertion result
		///// </summary>
		///// <returns>Result Table</returns>
		//ResultTable GetPortofolioDetailMediaResultTable();
    }
}
