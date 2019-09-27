using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;
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

        protected override void AppendMediaFields(StringBuilder sql, int mediaIndex)
        {
            #region Champs nomenclature media

            if (mediaIndex > -1)
            {
                //nomenclature media présente dans le tableau préformaté
                switch (_session.PreformatedMediaDetail)
                {
                    case CstFormat.PreformatedMediaDetails.vehicle:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle_long as m1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleCategory:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle_long as m1, {0}.id_category as id_m2, category as m2",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                        sql.AppendFormat(
                            " {0}.id_vehicle as id_m1, vehicle_long as m1, {0}.id_category as id_m2, category as m2, {0}.id_media as id_m3, media as m3",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleMedia:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle_long as m1, {0}.id_media as id_m2, media as m2",
                            _dataTable.Prefix);
                        break;
                    default:
                        _session.PreformatedMediaDetail = CstFormat.PreformatedMediaDetails.vehicle;
                        sql.Append(" {0}.id_vehicle as id_m1, vehicle_long as m1");
                        break;
                }
            }

            #endregion
        }

        protected override void FilterOnDigitalMediaTypes(StringBuilder sql) {
            if (VehiclesInformation.Contains(AdExpress.Constantes.Classification.DB.Vehicles.names.audioDigital) 
                && !_session.CustomerLogin.CustormerFlagAccess(Flags.ID_AUDIO_DIGITAL_ACCESS_FLAG))
            {
                long id = VehiclesInformation.Get(AdExpress.Constantes.Classification.DB.Vehicles.names.audioDigital).DatabaseId;
                sql.AppendFormat(" and {0}.id_vehicle not in ({1}) ", _dataTable.Prefix, id);
            }

            if (VehiclesInformation.Contains(AdExpress.Constantes.Classification.DB.Vehicles.names.search)
               && !_session.CustomerLogin.CustormerFlagAccess(Flags.ID_PAID_SEARCH_ACCESS_FLAG))
            {
                long id = VehiclesInformation.Get(AdExpress.Constantes.Classification.DB.Vehicles.names.search).DatabaseId;
                sql.AppendFormat(" and  {0}.id_vehicle not in ({1}) ", _dataTable.Prefix, id);
            }
            if (VehiclesInformation.Contains(AdExpress.Constantes.Classification.DB.Vehicles.names.paidSocial)
               && !_session.CustomerLogin.CustormerFlagAccess(Flags.ID_PAID_SOCIAL_ACCESS_FLAG))
            {
                long id = VehiclesInformation.Get(AdExpress.Constantes.Classification.DB.Vehicles.names.paidSocial).DatabaseId;
                sql.AppendFormat(" and  {0}.id_vehicle not in ({1}) ", _dataTable.Prefix, id);
            }
        }


    }
}
