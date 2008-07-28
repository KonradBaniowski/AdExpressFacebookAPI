#region Information
/*
 * Author : G Ragneau
 * Created on : 22/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassReports.Default
{
    /// <summary>
    /// Implements default results of the Product Class Analysis.
    /// </summary>
    public class ProductClassReports : TNS.AdExpressI.ProductClassReports.ProductClassReports
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReports(WebSession session):base(session){}
        #endregion

    }
}
