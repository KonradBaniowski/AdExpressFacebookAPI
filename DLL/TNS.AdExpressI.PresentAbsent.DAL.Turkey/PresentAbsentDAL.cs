using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.PresentAbsent.DAL.Turkey
{
    public class PresentAbsentDAL : DAL.PresentAbsentDAL
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public PresentAbsentDAL(WebSession session) : base(session){ }
        #endregion

        #region Get Fields For Gad Without Table Prefix
        /// <summary>
        /// Get Fields For Gad Without Table Prefix
        /// </summary>
        /// <returns></returns>
        protected override string GetFieldsForGadWithoutTablePrefix()
        {
            return "";
        }
        #endregion

        #region Init Gad Params
        /// <summary>
        /// Init Gad Params
        /// </summary>
        protected override void InitGadParams(Table tblGad, ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = "";
            dataFieldsForGad = "";
            dataJointForGad = "";
        }
        #endregion
    }
}
