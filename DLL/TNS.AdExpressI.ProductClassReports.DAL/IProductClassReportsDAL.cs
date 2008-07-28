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
using System.Data;
using System.Text;

namespace TNS.AdExpressI.ProductClassReports.DAL
{
    /// <summary>
    /// Contract to respect to provide access to the DAL Layer
    /// </summary>
    public interface IProductClassReportsDAL
    {
        /// <summary>
        /// Get Data from database for the report of the user session
        /// </summary>
        DataSet GetData();

        /// <summary>
        /// Get Data from database for the specified report
        /// </summary>
        /// <param name="resultType">Type of report</param>
        DataSet GetData(int resultType);
    }
}
