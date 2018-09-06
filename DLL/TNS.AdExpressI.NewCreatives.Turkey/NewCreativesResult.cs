using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.NewCreatives.Turkey
{
    public class NewCreativesResult : NewCreatives.NewCreativesResult
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public NewCreativesResult(WebSession session)
            : base(session)
        {
        }
        #endregion
    }
}
