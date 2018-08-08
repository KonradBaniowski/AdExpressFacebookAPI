using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using Table = TNS.AdExpress.Domain.DataBaseDescription.Table;

namespace TNS.AdExpressI.Insertions.DAL.Turkey
{
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Current Module id</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
        }
        #endregion

        #region AppendInsertionsSqlFields
        /// <summary>
        /// Append data fields
        /// The list of data fields used in the SQL query will be generated as following :
        ///     + Three detail Levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label. 
        ///       We can get this levels from _session.DetailLevel.Levels.
        ///     + A list of detail columns that we have initialize above according to column configuration files, we have two differents type of columns :
        ///         - The first one corresponds to a classification level, like Advertiser for example, in this case we need : Level_id and Level_label
        ///         - The second one corresponds to a feature of the insertion or the version, like page or format for example, in this case we need : column_value (page = 119, format = 1 PAGE)
        /// </summary>
        /// <param name="tData">Data base table description</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="columns">List of detail columns</param>
        /// <returns>True if there is detail levels selected</returns>
        protected override bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
            StringBuilder sql, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool detailLevelSelected = false;

            //Insertions fields
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */

            tmp = GenericColumns.GetSqlFields(columns, null);           
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);
            }

            //Constraints fields
            /* For some columns, we not only need information about the column but we need information about others fileds related to the column
             * example : 
             * <column id="35" name="Visuel" webTextId="1909"  dbLabel="visual">
             *  <constraints>
             *      <dbConstraints>
             *          <dbFieldConstraints>
             *              <dbFieldConstraint id="1" name="Visuel Disponibility" dbField="disponibility_visual" dbTablePrefixe="appliMd"/>
             *              <dbFieldConstraint id="2" name="Visuel Activation" dbField="activation" dbTablePrefixe="appliMd"/>
             *          </dbFieldConstraints>
             *      </dbConstraints>
             *  </constraints>
             * </column>
             * so we define in the Xml configuration file all the fields related to one column (like above), 
             * and we can get this fileds by using the method GenericColumns.GetSqlConstraintFields(columns)
             * */
            tmp = GenericColumns.GetSqlConstraintFields(columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            return detailLevelSelected;
        }
        /// <summary>
        /// Append data fields
        /// The list of data fields used in the SQL query will be generated as following :
        ///     + Three detail Levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label. 
        ///       We can get this levels from _session.DetailLevel.Levels.
        ///     + A list of detail columns that we have initialize above according to column configuration files, we have two differents type of columns :
        ///         - The first one corresponds to a classification level, like Advertiser for example, in this case we need : Level_id and Level_label
        ///         - The second one corresponds to a feature of the insertion or the version, like page or format for example, in this case we need : column_value (page = 119, format = 1 PAGE)
        /// </summary>
        /// <param name="tData">Data base table description</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevelsIds">List of level details Identifiers</param>
        /// <param name="columns">List of detail columns</param>
        /// <returns>True if there is detail levels selected</returns>
        protected override bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
            StringBuilder sql, ArrayList detailLevelsIds, List<GenericColumnItemInformation> columns)
        {

            bool detailLevelSelected = false;
            bool hasCategory = false;
            string tmp = string.Empty;

            //Detail levels
            /* The list of detail levels Identifiers is initialized according to levels saved in the _session.DetailLevel object.
             * to get the fields corresponding to the levels we use _session.DetailLevel.GetSqlFields().
             * */
            if (!_msCreaConfig && _session.DetailLevel != null &&
                _session.DetailLevel.Levels != null && _session.DetailLevel.Levels.Count > 0)
            {
                foreach (DetailLevelItemInformation detailLevelItemInformation in _session.DetailLevel.Levels)
                {
                    if (detailLevelItemInformation.Id == DetailLevelItemInformation.Levels.category)
                    {
                        hasCategory = true;
                    }
                    detailLevelsIds.Add(detailLevelItemInformation.Id.GetHashCode());
                }
                tmp = _session.DetailLevel.GetSqlFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    detailLevelSelected = true;
                }
            }

            //Insertions fields
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */

            tmp = GenericColumns.GetSqlFields(columns, detailLevelsIds);
           
            if (tmp.Length > 0)
            {
                if (detailLevelSelected) sql.Append(",");
                sql.AppendFormat(" {0},", tmp);
            }

            //Constraints fields
            /* For some columns, we not only need information about the column but we need information about others fileds related to the column
             * example : 
             * <column id="35" name="Visuel" webTextId="1909"  dbLabel="visual">
             *  <constraints>
             *      <dbConstraints>
             *          <dbFieldConstraints>
             *              <dbFieldConstraint id="1" name="Visuel Disponibility" dbField="disponibility_visual" dbTablePrefixe="appliMd"/>
             *              <dbFieldConstraint id="2" name="Visuel Activation" dbField="activation" dbTablePrefixe="appliMd"/>
             *          </dbFieldConstraints>
             *      </dbConstraints>
             *  </constraints>
             * </column>
             * so we define in the Xml configuration file all the fields related to one column (like above), 
             * and we can get this fileds by using the method GenericColumns.GetSqlConstraintFields(columns)
             * */
            tmp = GenericColumns.GetSqlConstraintFields(columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            // Slogan fields
            // Add slogan field if required (only for radio because it is required to build path
            if (!_msCreaConfig)
                AppendSloganField(sql, tData, vehicle, columns);

           

            return detailLevelSelected;
        }
        #endregion

    }
}
