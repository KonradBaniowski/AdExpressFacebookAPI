using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.GAD.Default
{
    public class GadResults : TNS.AdExpressI.GAD.GadResults
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        /// <param name="advertiser"> advertiser</param>
        public GadResults(WebSession session, string idAddress, string advertiser) : base(session, idAddress, advertiser)
        {
        }
    }
}
