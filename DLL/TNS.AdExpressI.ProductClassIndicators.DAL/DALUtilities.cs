using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;


using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;



namespace TNS.AdExpressI.ProductClassIndicators.DAL
{
    public class DALUtilities
    {

        protected WebSession _session;
        protected CstDBClassif.Vehicles.names _vehicle;

        #region Constructor
        public DALUtilities(WebSession session)
        {
            _session = session;
            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.CurrentUniversMedia.FirstNode.Tag).ID);

        }
        #endregion

        #region Get Media Selection
        /// <summary>
        /// Get Media Selection Clause (plurimedia?, Sponsorship?)
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public virtual string GetMediaSelection(string prefix)
        {

            StringBuilder sql = new StringBuilder();
            bool first = true;
            string temp = "";
            temp = FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, prefix, !first);
            sql.Append(" " + temp);

            if (_vehicle != CstDBClassif.Vehicles.names.plurimedia)
            {
                if (temp != null && temp.Length > 0) first = false;
                sql.Append(FctUtilities.SQLGenerator.getAccessVehicleList(_session, prefix, !first));
                first = false;
            }
            //TV Sponsorship rights
            if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
            {
                sql.AppendFormat("  and  {0}.id_category not in (68) ", prefix);
                first = false;
            }
            sql.Append(FctUtilities.SQLGenerator.GetRecapMediaSelection(_session.GetSelection(_session.CurrentUniversMedia, CstRight.type.categoryAccess), _session.GetSelection(_session.CurrentUniversMedia, CstRight.type.mediaAccess), true));


            return sql.ToString();
        }
        #endregion

    }
}
