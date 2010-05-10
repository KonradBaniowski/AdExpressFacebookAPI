using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Data;

using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TNS.Ares.Domain.LS;
using AresConst = TNS.Ares.Constantes.Constantes;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.Ares.StaticNavSession.Exceptions;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using TNS.Ares.StaticNavSession.DAL.Exceptions;
using TNS.Ares.Domain.DataBase;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.StaticNavSession.DAL
{
    public abstract class StaticNavSessionDAL : IStaticNavSessionDAL {

        #region Variables
        /// <summary>
        /// Data Source
        /// </summary>
        protected IDataSource _source;

        #endregion

        #region Constructor
        public StaticNavSessionDAL(IDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("StaticNavSession constructor: datasource cannot be null");
            this._source = source;
        }
        #endregion

        #region IStaticNavSessionDAL Members

        #region InsertData
        /// <summary>
        /// InsertData
        /// </summary>
        /// <param name="resultType">Result Type Id</param>
        /// <param name="fileName">FileName</param>
        /// <param name="alertId">alertId</param>
        /// <param name="loginId">loginId</param>
        public virtual void InsertData(int resultType, string fileName, int alertId, Int64 loginId)
        {
            StringBuilder insertCommand = new StringBuilder();
            try {
                insertCommand.AppendFormat("INSERT INTO {0}.STATIC_NAV_SESSION (ID_STATIC_NAV_SESSION, STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_NAME, STATUS, DATE_CREATION, DATE_MODIFICATION, ID_LOGIN, PDF_USER_FILENAME, DATE_EXEC) VALUES({0}.SEQ_STATIC_NAV_SESSION.NEXTVAL, NULL, {1}, '{2}', {3}, SYSDATE, SYSDATE, {4}, '{5}', NULL)",
                                            DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label,
                                            resultType,
                                            alertId,
                                            AresConst.Result.status.newOne.GetHashCode(),
                                            loginId,
                                            fileName.Replace("'", "''"));

                this._source.Insert(insertCommand.ToString());
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to insert data in InsertData(int resultType, string fileName, int alertId, Int64 loginId). Query : " + insertCommand.ToString(), e);
            }
        }

        /// <summary>
        /// InsertData
        /// </summary>
        /// <param name="resultType">Result Type Id</param>
        /// <param name="fileName">FileName</param>
        /// <param name="alertId">alertId</param>
        /// <param name="loginId">loginId</param>
        /// <param name="dateExec">Date Execution</param>
        public virtual void InsertData(int resultType, string fileName, int alertId, Int64 loginId, DateTime dateExec) {
            StringBuilder insertCommand = new StringBuilder();
            try{
            insertCommand.AppendFormat("INSERT INTO {0}.STATIC_NAV_SESSION (ID_STATIC_NAV_SESSION, STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_NAME, STATUS, DATE_CREATION, DATE_MODIFICATION, ID_LOGIN, PDF_USER_FILENAME, DATE_EXEC) VALUES({0}.SEQ_STATIC_NAV_SESSION.NEXTVAL, NULL, {1}, '{2}', {3}, SYSDATE, SYSDATE, {4}, '{5}', to_date('{6}','YYYYMMDDHH24MISS'))",
                                        DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label,
                                        resultType,
                                        alertId,
                                        AresConst.Result.status.newOne.GetHashCode(),
                                        loginId,
                                        fileName.Replace("'", "''"),
                                        dateExec.ToString("yyyyMMddHHmmss"));

            this._source.Insert(insertCommand.ToString());
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to insert data in InsertData(int resultType, string fileName, int alertId, Int64 loginId, DateTime dateExec). Query : " + insertCommand.ToString(), e);
            }
        }

        public virtual int InsertData(byte[] binaryData, Int64 loginId, int resultType, string fileName) {
            int idStaticNavSession = -1;

            // Opening connection
            OracleConnection connection = (OracleConnection)((OracleConnection)_source.GetSource()).Clone();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            StringBuilder insertCommand = new StringBuilder();
            OracleCommand command = null;

            try {
                command = connection.CreateCommand();

                // Preparing PL/SQL block
                insertCommand.Append("BEGIN ");
                insertCommand.Append("SELECT " + DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label + @".SEQ_STATIC_NAV_SESSION.NEXTVAL INTO :new_id FROM dual; ");
                insertCommand.Append("INSERT INTO " + DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label + ".STATIC_NAV_SESSION " );
                insertCommand.Append("(ID_LOGIN, ID_STATIC_NAV_SESSION, STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_USER_FILENAME, STATUS, DATE_CREATION, DATE_MODIFICATION) ");
                insertCommand.Append("VALUES(" + loginId + ", :new_id, :blobtodb, " + resultType + ", '" + fileName + @"', 0, SYSDATE, SYSDATE); ");
                insertCommand.Append("END;");

                command.CommandText = insertCommand.ToString();

                // Fill parametres
                OracleParameter param2 = command.Parameters.Add("new_id", OracleDbType.Int64);
                param2.Direction = ParameterDirection.Output;
                OracleParameter param = command.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;

                // Execute PL/SQL block
                command.ExecuteNonQuery();
                idStaticNavSession = int.Parse(param2.Value.ToString());
            }
            catch (System.Exception e) {
                throw new BaseException("StaticNavSession.InsertData : Impossible to insert blob in database."+((command!=null)?(" Command : " + command.CommandText):(string.Empty)), e);
            }
            finally {
                // Fermeture des structures
                connection.Close();
                connection.Dispose();
            }

            return (idStaticNavSession);
        }

        public virtual int InsertData(byte[] binaryData, Int64 loginId, int resultType, string fileName, object option) {
            return InsertData(binaryData, loginId, resultType, fileName);
        }
        #endregion

        #region DeleteExpiredRequests
        /// <summary>
        /// Delete the expired rows depending on today's date and
        /// the expiration date in the table. The plugin dictionary
        /// let you filter which type of rows should be deleted.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pluginList"></param>
        /// <param name="filePath"></param>
        public virtual void DeleteExpiredRequests(Dictionary<PluginType, PluginInformation> pluginList, string filePath)
        {
            int rowNb = 1;
            try {
                // Selecting the expired rows
                DataTable expired = GetExpired(pluginList);

                foreach (DataRow row in expired.Rows) {
                    // Checking if the file path 
                    if (filePath.EndsWith("\\") == false)
                        filePath += "\\";

                    // Deleting file
                    System.IO.File.Delete(filePath + row["PDF_NAME"].ToString());

                    // Deleting corresponding row
                    DeleteRow(int.Parse(row["ID_STATIC_NAV_SESSION"].ToString()));
                    rowNb++;
                }
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to Delete Expired Requests in DeleteExpiredRequests(Dictionary<PluginType, PluginInformation> pluginList, string filePath), row number '" + rowNb + "'", e);
            }
        }
        #endregion

        #region Get
        /// <summary>
        /// Returns a datatable containing all the new rows
        /// </summary>
        public virtual DataTable Get()
        {
            return (Get(AresConst.Result.status.newOne.GetHashCode(), null, false));
        }

        /// <summary>
        /// Returns a datatable containing all the rows corresponding
        /// to the given status
        /// </summary>
        /// <param name="status">Row status</param>
        public virtual DataTable Get(int status)
        {
            return (Get(status, null, false));
        }

        /// <summary>
        /// Returns a datatable containing all the rows corresponding
        /// to the given status, filtered by a pluginList
        /// </summary>
        /// <param name="status">Row status</param>
        /// <param name="pluginList">Plugin list limitation</param>
        /// <returns>A datatable</returns>
        public DataTable Get(int status, Dictionary<PluginType, PluginInformation> pluginList) {
            return (Get(status, pluginList, false));
        }
        #endregion

        #region GetData
        public virtual DataSet GetData(Dictionary<PluginType, PluginInformation> pluginList, Int64 loginId) {
            string typeList = string.Empty;
            if (pluginList != null) {

                foreach (PluginInformation currentPluginInformation in pluginList.Values) {
                    typeList += ((typeList != null && typeList.Length > 0) ? "," : "") + currentPluginInformation.ResultType.ToString();
                }

                #region Requete
                StringBuilder sql = new StringBuilder(1000);
                sql.Append("select pdf_name,id_static_nav_session , PDF_USER_FILENAME , ID_LOGIN, id_pdf_result_type   from ");
                sql.Append(DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label + ".STATIC_NAV_SESSION  ");
                sql.Append(" where  STATUS in (3) and id_pdf_result_type in (" + typeList + ")");
                sql.Append(" and ID_LOGIN=" + loginId.ToString());
                sql.Append(" order by id_pdf_result_type,pdf_name,DATE_CREATION ");
                #endregion

                #region Execution de la requête
                try {
                    return (_source.Fill(sql.ToString()));
                }
                catch (System.Exception err) {
                    throw (new StaticNavDALExceptions("Impossible de charger les données de la table static nav session. Query : " + sql.ToString(), err));
                }
                #endregion
            }
            return null;
        }
        #endregion

        #region GetExpired
        public virtual DataTable GetExpired(Dictionary<PluginType, PluginInformation> pluginList)
        {
            return (Get(AresConst.Result.status.sent.GetHashCode(), pluginList, true));
        }
        #endregion

        #region UpdateStatus
        /// <summary>
        /// Updates a row's status according to a given static nav
        /// session identifier.
        /// </summary>
        /// <param name="source">Data access interface</param>
        /// <param name="staticNavSessionId">Session identifier</param>
        /// <param name="status">Status to set on the row</param>
        /// <param name="longevity">How long the record should be valid from the update date</param>
        public virtual void UpdateStatus(Int64 staticNavSessionId, int status)
        {
            
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0}.STATIC_NAV_SESSION ",
                DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label);
            sql.AppendFormat("SET STATUS = {0}, DATE_MODIFICATION = SYSDATE ",
                status.ToString());
            sql.AppendFormat("Where id_static_nav_session = {0} ",
                staticNavSessionId.ToString());
            // Preparing update command

            try {
                // Updating
                this._source.Update(sql.ToString());
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to update statut in UpdateStatus(Int64 staticNavSessionId, int status). query : " + sql.ToString(), e);
            }
        }
        #endregion

        #region Sent
        public virtual void Sent(int stativNavSessionId)
        {
            try {
                this.UpdateStatus(stativNavSessionId, AresConst.Result.status.sent.GetHashCode());
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to update statut to sent in void Sent(int stativNavSessionId)", e);
            }
        }
        #endregion

        #region DeleteRow
        /// <summary>
        /// Deletes a row according to a static nav session id
        /// </summary>
        /// <param name="staticNavSession">Request Identifier</param>
        public virtual void DeleteRow(long staticNavSessionId) {
            // Preparing delete command
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("DELETE {0}.STATIC_NAV_SESSION WHERE ID_STATIC_NAV_SESSION = {1} "
                ,DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label,
                staticNavSessionId.ToString());
            try {
                this._source.Delete(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new NoDataException("Unable to delete the old requests in DeleteRow(long staticNavSessionId). Query : " + sql.ToString(), err));
            }
        }
        #endregion

        #region GetRow
        /// <summary>
        /// Gets a specific row corresponding to the given id
        /// </summary>
        /// <param name="staticNavSession">Static nav session id</param>
        /// <returns>A DataRow object</returns>
        public virtual DataRow GetRow(long staticNavSession)
        {
            StringBuilder selectCommand = new StringBuilder();
            try {
                selectCommand.Append(@"SELECT ID_STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_NAME, PDF_USER_FILENAME, STATUS, DATE_CREATION, DATE_MODIFICATION, ID_LOGIN "); 
                selectCommand.Append(@"FROM " + DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label + ".STATIC_NAV_SESSION "); 
                selectCommand.Append(@"WHERE ID_STATIC_NAV_SESSION = " + staticNavSession.ToString() );

                DataSet data = this._source.Fill(selectCommand.ToString());
                return (data.Tables[0].Rows[0]);
            }
            catch (Exception e) {
                throw new StaticNavDALExceptions("Impossible to Get row in GetRow(long staticNavSession). Query : " + selectCommand.ToString(), e);
            }
        }
        #endregion

        #region Load Data
        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des static_nav_sessions
        /// </summary>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        /// <param name="idStaticNavSession">Identifiant de la session sauvegardée</param>
        public virtual Object LoadData(Int64 idStaticNavSession) {

            #region Ouverture de la base de données
            OracleConnection cnx = null;
            try {
                //TODO : develop IDataSource for blob loading
                //OracleConnection cnx = new OracleConnection(Connection.SESSION_CONNECTION_STRING_TEST);
                cnx = (OracleConnection)_source.GetSource();
                cnx.Open();
            }
            catch (System.Exception e) {
                if(cnx!=null)
                    throw (new StaticNavDALExceptions("Impossible d'ouvrir la base de données with connexion string : '" + cnx.ConnectionString + "'. Connexion State : '" + cnx.State.ToString() + "'", e));
                else
                    throw (new StaticNavDALExceptions("Impossible d'ouvrir la base de données", e));
            }
            #endregion

            #region Chargement et deserialization de l'objet
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            Object o = null;
            try {
                binaryData = new byte[0];
                //create PL/SQL command
                string block = " BEGIN " +
                    " SELECT static_nav_session INTO :1 FROM " + DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label + ".static_nav_session " + " WHERE id_static_nav_session = " + idStaticNavSession.ToString() + "; " +
                    " END; ";
                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //Initialize parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobfromdb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Output;

                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();
                //Récupération des octets du blob
                binaryData = (byte[])((OracleBlob)(sqlCommand.Parameters[0].Value)).Value;

                //Deserialization oft the object
                ms = new MemoryStream();
                ms.Write(binaryData, 0, binaryData.Length);
                bf = new BinaryFormatter();
                ms.Position = 0;
                o = bf.Deserialize(ms);
            }
            #endregion

            #region Gestion des erreurs de chargement et de deserialization de l'objet
            catch (System.Exception e) {
                try {
                    // Fermeture des structures
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (binaryData != null) binaryData = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    cnx.Close();
                }
                catch (System.Exception et) {
                    throw (new StaticNavDALExceptions("Impossible de libérer les ressources après échec de la méthode", et));
                }
                throw (new StaticNavDALExceptions("Problème au chargement de la session à partir de la base de données", e));
            }
            try {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                if (binaryData != null) binaryData = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                cnx.Close();
            }
            catch (System.Exception et) {
                throw (new StaticNavDALExceptions("Impossible de fermer la base de données", et));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }
        #endregion

        #region RegisterFile
        /// <summary>
        /// Register the physical file name associated to a session and update the staut to done
        /// </summary>
        /// <param name="idStaticNavSession">User session</param>
        /// <param name="fileName">File Name</param>
        public virtual void RegisterFile(Int64 idStaticNavSession, string fileName) {

            #region Requête
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0}.STATIC_NAV_SESSION ",
                DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label);
            sql.AppendFormat("SET STATUS = {0}, pdf_name = '{1}' ",
                AresConst.Result.status.sent.GetHashCode().ToString(),
                fileName);
            sql.AppendFormat("Where id_static_nav_session = {0} ",
                idStaticNavSession.ToString());

            #endregion

            try {
                this._source.Update(sql.ToString());
            }
            catch (System.Exception err) {
                throw new StaticNavDALExceptions("Impossible to Register File in RegisterFile(Int64 idStaticNavSession, string fileName). Query : " + sql.ToString(), err);
            }
        }
        #endregion

        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a datatable containing all the rows corresponding
        /// to the given status, filtered by a pluginList
        /// </summary>
        /// <param name="status">Row status</param>
        /// <param name="pluginList">Plugin list limitation</param>
        /// <returns>A datatable</returns>
        private DataTable Get(int status, Dictionary<PluginType, PluginInformation> pluginList, bool longevityConstraint) {
            string condition = String.Empty;

            StringBuilder sql = new StringBuilder();

            // Preparing select query
            sql.AppendFormat("SELECT ID_STATIC_NAV_SESSION, PDF_NAME, ID_LOGIN, ID_PDF_RESULT_TYPE ");
            sql.AppendFormat("FROM {0}.STATIC_NAV_SESSION ", DataBaseConfiguration.DataBase.GetSchema(SchemaIds.webnav01).Label);

            bool first = true;
            if (pluginList != null)
                foreach (PluginInformation currentPluginInformation in pluginList.Values) {
                    // Adding kind of record constraints
                    if (currentPluginInformation.ResultType > 0) {
                        if (first) {
                            condition += " WHERE (";
                            first = false;
                        }
                        else
                            condition += " OR ";

                        // Kind of record constraint
                        condition += "( ID_PDF_RESULT_TYPE = " + currentPluginInformation.ResultType + " ";
                        // Adding
                        if (longevityConstraint)
                            condition += "AND SYSDATE > DATE_CREATION + " + currentPluginInformation.Longevity.ToString() + " ";

                        if(currentPluginInformation.UseExec){
                            condition += "AND ( ";
                            condition += "DATE_EXEC < to_date('" + DateTime.Now.ToString("yyyyMMddHHmmss") + "','YYYYMMDDHH24MISS') ";
                            condition += "OR DATE_EXEC is null";
                            condition += ") ";
                        }
                        condition += ")";
                        
                    }
                }
            if (condition.Length < 1)
                condition += " WHERE ";
            else
                condition += ") AND ";

            // Adding date and status constraints
            condition += "STATUS = " + status.ToString();

            // Adding to select command
            sql.Append(condition);

            try {
                // Filling in datatable
                return (this._source.Fill(sql.ToString()).Tables[0]);
            }
            catch (System.Exception err) {
                throw (new NoDataException("Unable to load the list of files to delete in Get(int status, Dictionary<PluginType, PluginInformation> pluginList, bool longevityConstraint). Query : " + sql.ToString(), err));
            }
        }
        #endregion

    }
}
