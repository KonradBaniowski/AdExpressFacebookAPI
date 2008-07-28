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
    }
}
