using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Russia
{
    /// <summary>
    /// Provides Russia data source object to execute SQL queries
    /// 
    /// The mehod <code>GetSource()</code> return  a object of type IDataSource which allow to execute SQL query
    /// on database of website's current country. For example in France the source will be an Oracle database, but an SQL database in Russia.
    /// </summary>
    public class SourceProvider : TNS.AdExpress.Web.Core.SourceProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Client Session</param>
        public SourceProvider(WebSession session)
            : base(session)
        {
        }

        /// <summary>
        /// Get data source for AdExpress Russia website
        /// </summary>
        /// <returns>Data source</returns>
        public override IDataSource GetSource()
        {
            return WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.adExpressRussia);
        }
    }
}
