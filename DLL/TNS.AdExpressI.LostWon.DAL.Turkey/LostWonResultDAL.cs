using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.LostWon.DAL.Turkey
{
    public class LostWonResultDAL : DAL.LostWonResultDAL
    {
        public LostWonResultDAL(WebSession session) : base(session)
        {
        }

        protected override void AppendGad(ref string dataTableNameForGad, Schema schAdExpr03, string DATA_TABLE_PREFIXE,
       ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = String.Empty;

             dataFieldsForGad = String.Empty;
            dataJointForGad = String.Empty;

        }

        protected override string GetDataFieldsForGadWithoutTablePrefix(string dataFieldsForGadWithoutTablePrefix)
        {
          
            return String.Empty;
        }


    }
}
