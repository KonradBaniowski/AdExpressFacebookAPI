using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Core.Poland
{
    /// <summary>
    /// Provides default data source object to execute SQL queries
    /// 
    /// The mehod <code>GetSource()</code> return  a object of type IDataSource which allow to execute SQL query
    /// on database of website's current country. For example in France the source will be an Oracle database, but an SQL database in Russia.
    /// </summary>
    public class SourceProvider : Core.SourceProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        public SourceProvider(WebSession session)
            : base(session)
        {
        }
    }
}
