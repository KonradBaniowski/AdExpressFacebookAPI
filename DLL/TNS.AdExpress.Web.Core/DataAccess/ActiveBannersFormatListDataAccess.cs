using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Exceptions;
using System.Data;

namespace TNS.AdExpress.Web.Core.DataAccess {
    /// <summary>
    /// Class used to extract the list of active banners format according to the specified vehicle
    /// </summary>
    class ActiveBannersFormatListDataAccess {

        #region Get DATA
        /// <summary>
        /// Method used to extract the list of active banners format according to the specified vehicle
        /// </summary>
        /// <param name="vehicleId">Vehicle id</param>
        /// <param name="siteLanguage">Site language</param>
        /// <returns></returns>
        public static DataSet GetActiveBannersFormatData(Int64 vehicleId, int siteLanguage) {

            StringBuilder sql = new StringBuilder(500);

            TNS.FrameWork.DB.Common.IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            Table formatBannersTable;
            Table bannersTableName;

            switch (VehiclesInformation.DatabaseIdToEnum(vehicleId)) {
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    bannersTableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.banners);
                    break;
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    bannersTableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.banners_mobile);
                    break;
                default:
                    throw (new ActiveBannersFormatListDataAccessException("Impossible to get the table name for this vehicle"));
            }

            formatBannersTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.formatBanners);

            sql.Append("Select distinct " + bannersTableName.Prefix + ".id_format_banners, format_banners");
            sql.Append(" from " + bannersTableName.SqlWithPrefix + " , " + formatBannersTable.SqlWithPrefix);
            sql.Append(" where " + formatBannersTable.Prefix + ".id_format_banners = " + bannersTableName.Prefix + ".id_format_banners");
            sql.AppendFormat(" and " + formatBannersTable.Prefix + ".id_language = " + siteLanguage);
            sql.Append(" and " + formatBannersTable.Prefix + ".activation <" + DBConstantes.ActivationValues.UNACTIVATED.ToString());
            sql.Append(" order by id_format_banners asc");

            #region Request execution
            try {
                return dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new TNS.AdExpress.Web.Core.Exceptions.ActiveBannersFormatListDataAccessException("Impossible to get the list of active banners format: " + sql, err));
            }
            #endregion
        }
        #endregion

    }
}
