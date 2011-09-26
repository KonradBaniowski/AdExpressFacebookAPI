using System;
using System.Text;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Exceptions;
using System.Data;

namespace TNS.AdExpress.Web.Core.DataAccess {
    /// <summary>
    /// Class used to extract the list of active banners format according to the specified vehicle
    /// </summary>
    class VehicleFormatListDataAccess {

        #region Get DATA

        /// <summary>
        /// Method used to extract the list of active banners format according to the specified vehicle
        /// </summary>
        /// <param name="formatTable">Format Table</param>
        /// <param name="dataLanguage">Data language</param>
        /// <param name="dataTable">Data Table</param>
        /// <returns>Format List</returns>
        public static DataSet GetActiveData(TableIds dataTable, TableIds formatTable, int dataLanguage) {

            var sql = new StringBuilder(500);

            var dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            Table bannersTableName = WebApplicationParameters.DataBaseDescription.GetTable(dataTable);
            Table formatBannersTable = WebApplicationParameters.DataBaseDescription.GetTable(formatTable);

            sql.AppendFormat("Select distinct {0}.id_{1}, {0}.{1} from {2} ,{3} where {0}.id_{1} = {4}.id_{1} and {0}.id_language = {5} and {0}.activation <{6} order by {0}.{1} asc"
                , formatBannersTable.Prefix
                , formatBannersTable.Label
                , formatBannersTable.SqlWithPrefix
                , bannersTableName.SqlWithPrefix
                , bannersTableName.Prefix
                , dataLanguage
                , DBConstantes.ActivationValues.UNACTIVATED.ToString());

            #region Request execution
            try {
                return dataSource.Fill(sql.ToString());
            }
            catch (Exception err) {
                throw (new VehicleFormatListDataAccessException("Impossible to get the list of active banners format: " + sql, err));
            }
            #endregion
        }
        #endregion

    }
}
