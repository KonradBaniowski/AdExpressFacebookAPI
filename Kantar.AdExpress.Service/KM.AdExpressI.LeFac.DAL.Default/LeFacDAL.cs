using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Web.Core.Sessions;

namespace KM.AdExpressI.LeFac.DAL.Default
{
    public class LeFacDAL : DAL.LeFacDAL
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        public LeFacDAL(WebSession session, string idAddress):base(session,idAddress)
        {
        }

    }

}
