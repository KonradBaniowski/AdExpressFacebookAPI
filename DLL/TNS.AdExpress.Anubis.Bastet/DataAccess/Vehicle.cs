#region Informations
// Auteur: D. V. Mussuma
// Date de création: 5/12/2005
// Date de modification:
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Constantes.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;
using System.Collections.Generic;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Description résumée de Vehicle.
	/// </summary>
    public class Vehicle {

        /// <summary>
        /// Obtient les données des médias les plus utilisés
        /// </summary>
        /// <param name="parameters">parametres des statistiques</param>
        /// <returns>données des médias les plus utilisés</returns>
        public static DataTable TopUsed(BastetCommon.Parameters parameters, int language, IDataSource dataSourceClassification) {
            try {
                #region Requête
                StringBuilder sql = new StringBuilder(3000);
                //Table vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
                Table topVehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopVehicle);

                //select
                sql.Append(" select ");

                sql.Append(" " + topVehicleTable.Prefix + ".id_vehicle");
                //sql.Append("," + vehicleTable.Prefix + ".vehicle");
                sql.Append(",sum(" + topVehicleTable.Prefix + ".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

                //From
                sql.Append(" from " + topVehicleTable.SqlWithPrefix);
                //sql.Append(" ," + vehicleTable.SqlWithPrefix);

                //Where
                sql.Append(" where " + topVehicleTable.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
                if (parameters != null && parameters.Logins.Length > 0) {
                    sql.Append(" and " + topVehicleTable.Prefix + ".id_login in (" + parameters.Logins + ") ");
                }
                //sql.Append(" and "+topVehicleTable.Prefix+".id_vehicle="+vehicleTable.Prefix+".id_vehicle");
                //sql.Append(" and "+vehicleTable.Prefix+".id_language="+language);
                //Gourp by
                sql.Append(" and  " + topVehicleTable.Prefix + ".id_vehicle not in (50)");
                sql.Append(" group by  ");
                sql.Append("  " + topVehicleTable.Prefix + ".ID_vehicle ");
                //sql.Append("," + vehicleTable.Prefix + ".vehicle");
                //Order by
                sql.Append(" order by  CONNECTION_NUMBER  desc");
                #endregion

                #region Execution
                DataTable dt = (parameters.Source.Fill(sql.ToString()).Tables[0]);
                #endregion

                return AddLabel(dt, language, dataSourceClassification);
            }
            catch (System.Exception err) {
                throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible to get most used media list ", err));

            }

        }

        /// <summary>
        /// Obtient les données des médias les plus utilisés par module
        /// </summary>
        /// <param name="parameters">parametres des statistiques</param>
        /// <returns>données des médias les plus utilisés par module</returns>
        public static DataTable TopByModule(BastetCommon.Parameters parameters, int language, IDataSource dataSourceClassification) {
            try {
                #region Requête
                StringBuilder sql = new StringBuilder(3000);
                //Table vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
                Table topVehicleByModuleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopVehicleByModule);
                Table moduleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);

                //select
                sql.Append(" select ");
                sql.Append(" " + topVehicleByModuleTable.Prefix + ".id_vehicle");
                //sql.Append(", vehicle");
                sql.Append(", " + topVehicleByModuleTable.Prefix + ".id_module,module");
                sql.Append(",sum(" + topVehicleByModuleTable.Prefix + ".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

                //From
                sql.Append(" from " + topVehicleByModuleTable.SqlWithPrefix);
                //sql.Append(" ," + vehicleTable.SqlWithPrefix); 
                sql.Append("," + moduleTable.SqlWithPrefix);

                //Where
                sql.AppendFormat(" where {0}.date_connection  between {1} and {2}",
                    topVehicleByModuleTable.Prefix, parameters.PeriodBeginningDate.ToString("yyyyMMdd"), parameters.PeriodEndDate.ToString("yyyyMMdd"));
                if (parameters != null && parameters.Logins.Length > 0) {
                    sql.Append(" and " + topVehicleByModuleTable.Prefix + ".id_login in (" + parameters.Logins + ") ");
                }
                //sql.Append(" and " + topVehicleByModuleTable.Prefix + ".id_vehicle=" + vehicleTable.Prefix + ".id_vehicle");
                //sql.Append(" and " + vehicleTable.Prefix + ".id_language=" + language);
                //sql.Append(" and " + vehicleTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED);
                sql.Append(" and " + topVehicleByModuleTable.Prefix + ".id_module=" + moduleTable.Prefix + ".id_module");
                sql.Append(" and " + moduleTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED);

                sql.Append("  and " + topVehicleByModuleTable.Prefix + ".id_vehicle not in (50)");
                //Gourp by
                sql.Append(" group by  ");
                sql.AppendFormat("  {0}.id_module,module,{0}.id_vehicle ", topVehicleByModuleTable.Prefix);
                //sql.Append(",vehicle ");
                //Order by
                sql.Append(" order by  module asc," + topVehicleByModuleTable.Prefix + ".id_vehicle ");
                //sql.Append(" ,vehicle");
                #endregion

                #region Execution

                DataTable dt = (parameters.Source.Fill(sql.ToString()).Tables[0]);
                #endregion

                return AddLabel(dt, language, dataSourceClassification);
            }
            catch (System.Exception err) {
                throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopByModule : Impossible t oget most used media by module", err));

            }

        }

        private static string Int64ToString(Int64 pf) {
            return pf.ToString();
        }

        private static DataTable AddLabel(DataTable dt, int language, IDataSource dataSourceClassification) {

            #region Build Id List
            List<Int64> idVehicleList = new List<Int64>();

            foreach (DataRow currentRow in dt.Rows) {
                Int64 vehicleId = Int64.Parse(currentRow["id_vehicle"].ToString());
                if (!idVehicleList.Contains(vehicleId)) idVehicleList.Add(vehicleId);
            }
            #endregion

            #region Get All label
            CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.classificationLevelList];
            object[] parameter = new object[2];
            parameter[0] = dataSourceClassification;
            parameter[1] = language;
            var classificationLevelListDALFactory = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain
                .CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + "Bin\\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null);

            ClassificationLevelListDAL classificationLevelListDAL = classificationLevelListDALFactory
                .CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,
                string.Join(",", (idVehicleList.ConvertAll(Convert.ToString)).ToArray()));
            DataTable dtRetour = new DataTable();

            foreach (DataColumn currentColumn in dt.Columns) {
                dtRetour.Columns.Add(currentColumn.ColumnName, currentColumn.DataType);
            }
            dtRetour.Columns.Add("vehicle", typeof(System.String));

            foreach (DataRow currentRow in dt.Rows) {
                string labelVehicle = string.Empty;
                DataRow[] dataRowArray = classificationLevelListDAL.GetDataTable.Select("id_vehicle = " + currentRow["id_vehicle"]);
                if (dataRowArray != null && dataRowArray.Length != 1) throw new Exception("Invalid dataRowArray request count");
                labelVehicle = dataRowArray[0]["vehicle"].ToString();

                List<object> itemArrayList = new List<object>(dt.Columns.Count + 1);
                itemArrayList.AddRange(currentRow.ItemArray);
                itemArrayList.Add(labelVehicle);
                dtRetour.Rows.Add(itemArrayList.ToArray());
            }
            return dtRetour;
            #endregion
        }

    }
}
