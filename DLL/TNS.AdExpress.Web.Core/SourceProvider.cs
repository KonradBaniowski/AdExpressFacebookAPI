using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core
{
    /// <summary>
    /// Provides data source object to execute SQL queries
    /// 
    /// The mehod <code>GetSource()</code> return  a object of type IDataSource which allow to execute SQL query
    /// on database of website's current country. For example in France the source will be an Oracle database, but an SQL database in Russia.
    /// </summary>
    public abstract class SourceProvider : ISourceProvider
    {
        /// <summary>
        /// Client session
        /// </summary>
        WebSession _session = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Client session</param>
        public SourceProvider(WebSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Get data source for AdExpress website's current country
        /// </summary>
        /// <returns>Data source</returns>
        public virtual IDataSource GetSource()
        {
            return _session.Source;
        }
    }
}
