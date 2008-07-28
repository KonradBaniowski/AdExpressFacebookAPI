#region Information
/*
 * Author : G Ragneau
 * Created on : 22/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 */
#endregion

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassReports.DAL.Default
{

    /// <summary>
    /// Default behaviour of DAL layer
    /// </summary>
    public class ProductClassReportDAL : TNS.AdExpressI.ProductClassReports.DAL.ProductClassReportDAL
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session):base(session){}
        #endregion

    }
}
