using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using Table = TNS.AdExpress.Domain.DataBaseDescription.Table;
namespace TNS.AdExpressI.Insertions.DAL.France {
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL {

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="module">Current Module</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId) {
        }
        #endregion

        #region Get Fields
        /// <summary>
        /// Get SqL fields specific to one Mdia type
        /// </summary>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="prefixeTable">Table Prefixe</param>
        /// <returns> champs de requêtes </returns>
        protected override string GetFields(Vehicles.names idVehicle, string prefixeTable) {
            switch (idVehicle) {
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.others:
                    return ",id_media,TO_CHAR( duration)  as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.internationalPress:
                case Vehicles.names.press:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return ",id_media,format as advertDimension, visual as associated_file";
                case Vehicles.names.outdoor:
                case Vehicles.names.dooh:
                case Vehicles.names.indoor:
                    return ",id_media,type_board as advertDimension, associated_file as associated_file";
                case Vehicles.names.mailValo:
                case Vehicles.names.directMarketing:
                    return ",id_media,TO_CHAR(weight) as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.adnettrack:
                case Vehicles.names.evaliantMobile:
                    return ",id_media, (dimension || ' / ' || format) as advertDimension, TO_CHAR(associated_file) as associated_file, id_country";
                default: return "";
            }
        }
        #endregion

        #region AppendSqlTables
        /// <summary>
        /// Get from clause
        /// The list of tables used in the query will be generated as following :
        ///     + Tables that correspond to the detail levels : Level1Table, Level2Table, Level3Table.
        ///       We can get this tables using _session.DetailLevel.GetSqlTables(sAdExpr03.Label)
        ///     + Tables that correspond to the list of columns, we can get this tables using GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds)
        ///     + Specific tables : for some columns we need to get informations not only from the column table but from others tables.
        ///       So, to do that we define in the Xml configuration file a list of tables related to a column 
        ///       and we can get this list by using GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns)
        /// </summary>
        /// <param name="tData">Data table</param>
        /// <param name="sql">Sql request container</param>
        protected override void AppendSqlTables(Schema sAdExpr03, Table tData
            , StringBuilder sql, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            /* Get tables that corresponds to the classification columns
             * */
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, null);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);
            }
            /* Get tables that corresponds to specific columns
             * */
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);//Rules constraints management
            }
        }
        /// <summary>
        /// Get from clause
        /// The list of tables used in the query will be generated as following :
        ///     + Tables that correspond to the detail levels : Level1Table, Level2Table, Level3Table.
        ///       We can get this tables using _session.DetailLevel.GetSqlTables(sAdExpr03.Label)
        ///     + Tables that correspond to the list of columns, we can get this tables using GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds)
        ///     + Specific tables : for some columns we need to get informations not only from the column table but from others tables.
        ///       So, to do that we define in the Xml configuration file a list of tables related to a column 
        ///       and we can get this list by using GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns)
        /// </summary>
        /// <param name="tData">Data table</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevelsIds">Level details</param>
        protected override void AppendSqlTables(Schema sAdExpr03, Table tData, StringBuilder sql,
            List<GenericColumnItemInformation> columns, ArrayList detailLevelsIds)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            /* Get tables that corresponds to the classification levels
             * */
            if (!_msCreaConfig && _session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlTables(sAdExpr03.Label);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(", {0} ", tmp);
                }
            }
            /* Get tables that corresponds to the classification columns
             * */
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);
            }
            /* Get tables that corresponds to specific columns
             * */
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);//Rules constraints management
            }
        }
        #endregion

    }
}
