using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.GAD.DAL.Default
{
    public class GadDAL : DAL.GadDAL
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        public GadDAL(WebSession session, string idAddress):base(session,idAddress)
        {           
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="idAddress">id adresss</param>
        public GadDAL(IDataSource source, string idAddress) : base(source,idAddress)
        {
        }
    }
}
