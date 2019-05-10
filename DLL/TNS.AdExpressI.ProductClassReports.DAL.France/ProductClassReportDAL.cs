using System.Text;
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

    }
}
