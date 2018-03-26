using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.NewCreatives.DAL.France
{
    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL
    {


        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate)
            : base(session, idSectors, beginingDate, endDate)
        {
        }
    }
}