using TNS.AdExpress.Web.Core.Sessions;

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
    }
}
