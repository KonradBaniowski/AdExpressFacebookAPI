using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

namespace TNS.AdExpressI.ProductClassReports.DAL.France
{
    public class ProductClassReportDAL : DAL.ProductClassReportDAL
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session) : base(session) { }
        #endregion

       

    }
}
