using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.PSALoader.Domain;
using KMI.AdExpress.PSALoader.DAL.Exceptions;
using Oracle.DataAccess.Client;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

namespace KMI.AdExpress.PSALoader.DAL {
    /// <summary>
    /// DAL class to insert data from XML in database
    /// </summary>
    public class PSALoaderDAL {

        #region Constantes
        /// <summary>
        /// Id language data
        /// </summary>
        private const string ID_LANGUAGE_DATA_I = "33";
        /// <summary>
        /// Indicates if the promo is exclusively for web
        /// </summary>
        private const string EXCLU_WEB = "0";
        /// <summary>
        /// Indicates that the row is loaded
        /// </summary>
        private const string ACTIVATION_LOADED = "80";
        private const string ACTIVATION_REJECTED = "60";
        private const string ACTIVATION_CODIFIED = "90";
        #endregion

        public static List<long> GetFormIdList(string connectionString) {

            OracleConnection connection = new OracleConnection(connectionString);
            List<long> formIdList = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;
            // On teste si la base est déjà ouverte
            if (connection.State == System.Data.ConnectionState.Closed) {
                DBToClosed = true;
                try {
                    connection.Open();
                }
                catch (System.Exception et) {
                    throw (new PSALoaderDALException("Impossible d'ouvrir la base de données", et));
                }
            }
            #endregion

            StringBuilder sql = new StringBuilder();
            OracleCommand sqlCommand = null;

            sql.Append(" SELECT id_form ");
            sql.Append(" FROM PROMO03.DATA_PROMOTION ");
            sql.Append(" where id_form > 0 ");

            sqlCommand = new OracleCommand(sql.ToString());
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "DATA_PROMOTION");

            if(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0){
            
                formIdList = new List<long>();
                foreach (DataRow row in ds.Tables[0].Rows)
                    formIdList.Add(Int64.Parse(row["id_form"].ToString()));
            }

            #region Fermeture de la base de données
            try {
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
            }
            catch (System.Exception et) {
                throw (new PSALoaderDALException("Impossible to close database", et));
            }
            #endregion

            return formIdList;
        }

        #region Insert Data
        public static void InsertData(string connectionString, FormInformations formInformations, List<long> formIdList) {

            StringBuilder sql = new StringBuilder();

            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;
            // On teste si la base est déjà ouverte
            if (connection.State == System.Data.ConnectionState.Closed) {
                DBToClosed = true;
                try {
                    connection.Open();
                }
                catch (System.Exception et) {
                    throw (new PSALoaderDALException("Impossible d'ouvrir la base de données", et));
                }
            }
            #endregion

            foreach (FormInformation formInformation in formInformations.FormInformationList) {

                if (!formIdList.Contains(formInformation.FormId)) {
                    sql = new StringBuilder();
                    sqlCommand = null;
                    ms = null;
                    bf = null;
                    binaryData = null;

                    try {
                        //"Serialization"
                        ms = new MemoryStream();
                        bf = new BinaryFormatter();
                        if ((formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode() ||
                            formInformation.VehicleId == Constantes.Vehicles.names.RADIO.GetHashCode() )
                            && formInformation.Script.Length > 0) {

                            bf.Serialize(ms, formInformation.Script);
                            binaryData = new byte[ms.GetBuffer().Length];
                            binaryData = ms.GetBuffer();
                        }

                        sql.AppendFormat(" BEGIN   ");
                        sql.AppendFormat(" INSERT INTO promo03.data_promotion   ");
                        sql.AppendFormat(" (                                    ");
                        sql.AppendFormat(" id_data_promotion,                   ");
                        sql.AppendFormat(" id_language_data_i,                  ");
                        sql.AppendFormat(" promotion_visual,                    ");
                        sql.AppendFormat(" activation,                          ");
                        sql.AppendFormat(" load_date,                           ");
                        sql.AppendFormat(" exclu_web,                           ");
                        sql.AppendFormat(" id_form,                             ");
                        sql.AppendFormat(" id_vehicle,                          ");
                        if (formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode()
                            || formInformation.VehicleId == Constantes.Vehicles.names.RADIO.GetHashCode()) {

                            sql.AppendFormat(" id_slogan,                           ");
                            if (formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode())
                                sql.AppendFormat(" tv_board,                            ");
                            if (formInformation.Script.Length > 0)
                                sql.AppendFormat(" script,                              ");
                        }
                        sql.AppendFormat(" date_media_num                       ");
                        sql.AppendFormat(" )                                    ");

                        sql.AppendFormat(" VALUES                               ");

                        sql.AppendFormat(" (                                    ");
                        sql.AppendFormat(" PROMO03.SEQ_DATA_PROMOTION.NEXTVAL,  ");
                        sql.AppendFormat(" {0},                                 ", ID_LANGUAGE_DATA_I);
                        sql.AppendFormat(" '{0}',                               ", formInformation.GetPromotionVisual());
                        sql.AppendFormat(" {0},                                 ", ACTIVATION_LOADED);
                        sql.AppendFormat(" {0},                                 ", formInformations.LoadDate);
                        sql.AppendFormat(" {0},                                 ", EXCLU_WEB);
                        sql.AppendFormat(" {0},                                 ", formInformation.FormId);
                        sql.AppendFormat(" {0},                                 ", formInformation.VehicleId);
                        if (formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode()
                           || formInformation.VehicleId == Constantes.Vehicles.names.RADIO.GetHashCode()) {
                               sql.AppendFormat(" {0},                                 ", formInformation.SloganId);
                               if (formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode())
                                   sql.AppendFormat(" '{0}',                               ", formInformation.GetTvBoard());
                            if (formInformation.Script.Length > 0)
                                sql.AppendFormat(" :clobtodb,                               ");
                        }
                        sql.AppendFormat(" {0}                                 ", formInformation.DateMediaNum);
                        sql.AppendFormat(" );  \n                                ");
                        sql.AppendFormat(" END; ");

                        sqlCommand = new OracleCommand(sql.ToString());
                        sqlCommand.Connection = connection;
                        sqlCommand.CommandType = CommandType.Text;

                        if ((formInformation.VehicleId == Constantes.Vehicles.names.TELEVISION.GetHashCode() ||
                            formInformation.VehicleId == Constantes.Vehicles.names.RADIO.GetHashCode())
                            && formInformation.Script.Length > 0) {
                            //Fill parametres
                            OracleParameter param = sqlCommand.Parameters.Add("clobtodb", OracleDbType.Clob);
                            param.Direction = ParameterDirection.Input;
                            //param.Value = binaryData;
                            param.Value = formInformation.Script;
                        }

                        //Execute PL/SQL block
                        sqlCommand.ExecuteNonQuery();

                    }

                    #region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
                    catch (System.Exception e) {
                        // Fermeture des structures
                        try {
                            if (ms != null) ms.Close();
                            if (bf != null) bf = null;
                            if (sqlCommand != null) sqlCommand.Dispose();
                            connection.Close();
                            connection.Dispose();
                        }
                        catch (System.Exception et) {
                            throw (new PSALoaderDALException("Insert PSA DATA : Impossible to free resources", et));
                        }
                        throw (new PSALoaderDALException("Insert PSA DATA : Error while inserting PSA data", e));
                    }
                    #endregion
                }
            }

            #region Fermeture de la base de données
            try {
                if (ms != null)
                    ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
            }
            catch (System.Exception et) {
                throw (new PSALoaderDALException("Impossible to close database", et));
            }
            #endregion
        }
        #endregion

    }
}
