#region Information
/*
 * Author : G Ragneau
 * Created on : 30/06/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion


using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.ProductClassReports
{
    /// <summary>
    /// Contract to respect when computing product class reports
    /// </summary>
    public interface IProductClassReports
    {
        /// <summary>
        /// Compute Product Class Report depending on type of result specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetProductClassReport();
        /// <summary>
        /// Compute Product Class Report matching the "result" param (HTML code)
        /// </summary>
        /// <param name="result">Type of result</param>
        /// <returns>HTML Code</returns>
        string GetProductClassReport(int result);
        /// <summary>
        /// Compute Product Class Report depending on type of result specified in user session (HTML code) as an excel code
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetProductClassReportExcel();
        /// <summary>
        /// Compute Product Class Report matching the "result" param (HTML code) as an excel code
        /// </summary>
        /// <param name="result">Type of result</param>
        /// <returns>HTML Code</returns>
        string GetProductClassReportExcel(int result);


        /// <summary>
        /// Compute Product Class Report depending on type of result specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        ResultTable GetGenericProductClassReport();
        /// <summary>
        /// Get GridResult 
        /// </summary>
        /// <returns>Grid Result</returns>
        GridResult GetGridResult(ResultTable.SortOrder sortOrder, int columnIndex);
        /// <summary>
        /// Compute Product Class Report matching the "result" param (HTML code)
        /// </summary>
        /// <param name="result">Type of result</param>
        /// <returns>HTML Code</returns>
        ResultTable GetGenericProductClassReport(int result);
        /// <summary>
        /// Compute Product Class Report depending on type of result specified in user session (HTML code) as an excel code
        /// </summary>
        /// <returns>HTML Code</returns>
        ResultTable GetGenericProductClassReportExcel();
        /// <summary>
        /// Compute Product Class Report matching the "result" param (HTML code) as an excel code
        /// </summary>
        /// <param name="result">Type of result</param>
        /// <returns>HTML Code</returns>
        ResultTable GetGenericProductClassReportExcel(int result);

		/// <summary>
		/// Determine if module bridge can be show
		/// </summary>
		/// <returns>True if module bridge can be show</returns>
		bool ShowModuleBridge();

    }
}
