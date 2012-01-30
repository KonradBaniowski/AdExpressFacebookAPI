using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB;

namespace TNS.AdExpress.Web.Core
{
    /// <summary>
    /// Provides data source object to execute SQL queries
    /// 
    /// The mehod <code>GetSource()</code> return  a object of type IDataSource which allow to execute SQL query
    /// on database of website's current country. For example in France the source will be an Oracle database, but an SQL database in Russia.
    /// </summary>
    public interface ISourceProvider
    {
        /// <summary>
        /// Get data source for AdExpress website's current country
        /// </summary>
        /// <returns>Data source</returns>
        TNS.FrameWork.DB.Common.IDataSource GetSource();
    }
}
