using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TNS.AdExpressI.Celebrities.DAL
{
    /// <summary>
    /// Celebrities Report DataAccess Contract
    /// </summary>
    public interface ICelebritiesDAL
    {
        /// <summary>
        /// Get data to build a Celebrities Report
        /// </summary>
        DataSet GetCelebritiesData();
    }
}
