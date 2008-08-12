#region Information
/*
 * Author : G Ragneau
 * Created on : 31/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 */
#endregion

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.Default
{

    /// <summary>
    /// Default behaviour of DAL layer
    /// </summary>
    public class ProductClassIndicatorsDAL : TNS.AdExpressI.ProductClassIndicators.DAL.ProductClassIndicatorsDAL
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicatorsDAL(WebSession session):base(session){}
        #endregion

    }
}
