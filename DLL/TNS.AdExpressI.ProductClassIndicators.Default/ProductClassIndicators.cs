#region Information
/*
 * Author : G Ragneau
 * Created on : 31/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassIndicators.Default
{
    /// <summary>
    /// Implements default results of the Product Class Indicators.
    /// </summary>
    public class ProductClassIndicators : TNS.AdExpressI.ProductClassIndicators.ProductClassIndicators
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicators(WebSession session) : base(session) { }
        #endregion

    }
}
