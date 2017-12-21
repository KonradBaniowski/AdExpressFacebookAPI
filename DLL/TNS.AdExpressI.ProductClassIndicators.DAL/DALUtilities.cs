using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;



namespace TNS.AdExpressI.ProductClassIndicators.DAL
{
    public class DALUtilities
    {

        protected WebSession _session;
        protected CstDBClassif.Vehicles.names _vehicle;
        #region Accessors
        public CstDBClassif.Vehicles.names Vehicle
        {
            get { return _vehicle; }
        }
        #endregion

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

            var sql = new StringBuilder();
            bool first = true;
            string temp = "";
            temp = FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, prefix, !first);
            sql.Append(" " + temp);

            if (_vehicle != CstDBClassif.Vehicles.names.plurimedia && _vehicle != CstDBClassif.Vehicles.names.PlurimediaWithoutMms
               && _vehicle != CstDBClassif.Vehicles.names.plurimediaWithSearch && _vehicle != CstDBClassif.Vehicles.names.plurimediaOnline
                 &&   _vehicle != CstDBClassif.Vehicles.names.plurimediaOffline)
            {
                if (!string.IsNullOrEmpty(temp)) first = false;
                sql.Append(FctUtilities.SQLGenerator.getAccessVehicleList(_session, prefix, !first));
                first = false;
            }

            //if (_vehicle == CstDBClassif.Vehicles.names.PlurimediaWithoutMms &&
            //    VehiclesInformation.Contains(CstDBClassif.Vehicles.names.PlurimediaWithoutMms))
            //{
            //    sql.AppendFormat("  and  {0}.id_vehicle not in ( {1},{2}) "
            //      , prefix, VehiclesInformation.Get(CstDBClassif.Vehicles.names.mms).DatabaseId,
            //      VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId);

            //}

            //TV Sponsorship rights
			if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
			{
			    string idSponsorShipCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.category,
			                                                                        WebConstantes.GroupList.Type.productClassAnalysisSponsorShipTv);
			    if (!string.IsNullOrEmpty(idSponsorShipCategory)) {
					 sql.AppendFormat("  and  {0}.id_category not in ( {1}) ", prefix, idSponsorShipCategory);
					 first = false;
				 }
			}
            sql.Append(FctUtilities.SQLGenerator.GetRecapMediaSelection(_session.GetSelection(_session.CurrentUniversMedia, CstRight.type.categoryAccess)
                , _session.GetSelection(_session.CurrentUniversMedia, CstRight.type.mediaAccess), true));


            return sql.ToString();
        }
        #endregion

    }
}
