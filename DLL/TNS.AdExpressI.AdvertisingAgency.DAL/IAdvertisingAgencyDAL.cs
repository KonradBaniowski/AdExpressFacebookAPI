using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TNS.AdExpressI.AdvertisingAgency.DAL{
    /// <summary>
    /// Defines interface for methods to Extract data for different type of results of the module Advertising Agency Report.
    /// </summary>
    public interface IAdvertisingAgencyDAL{
        /// <summary>
        /// Load data for the module Advertising Agency report.
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetData();
    }
}