
using KM.AdExpressI.MyAdExpress.Exceptions;
using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace KM.AdExpressI.MyAdExpress
{
    public class MyResultsDAL
    {
        /// <summary>
		/// Donne la liste des sessions d'un client qui sont enregistrées
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Listes de sessions</returns>
		public static DataSet GetData(WebSession webSession)
        {

            #region Construction de la requête
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            string sql = "select " + directoryTable.Prefix + ".ID_DIRECTORY, " + directoryTable.Prefix + ".DIRECTORY, " + mySessionTable.Prefix + ".ID_MY_SESSION, " + mySessionTable.Prefix + ".MY_SESSION ";
            sql += " from " + directoryTable.SqlWithPrefix + " , " + mySessionTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + ActivationValues.UNACTIVATED;
            sql += " and (" + mySessionTable.Prefix + ".ACTIVATION<" + ActivationValues.UNACTIVATED + " or " + mySessionTable.Prefix + ".ACTIVATION is null) ";
            sql += " and " + directoryTable.Prefix + ".ID_DIRECTORY=" + mySessionTable.Prefix + ".ID_DIRECTORY(+) ";
            sql += " order by " + directoryTable.Prefix + ".DIRECTORY, " + mySessionTable.Prefix + ".MY_SESSION ";

            #endregion

            #region Execution de la requête
            try
            {
                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (err);
                //throw (new MyAdExpressDataAccessException("Impossible d'obtenir la liste des sessions d'un client qui sont enregistrées", err));
            }
            #endregion

        }

        /// <summary>
		/// Déplace une session
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idOldDirectory">Identifiant du Répertoire source</param>
		/// <param name="idNewDirectory">Identifiant du Répertoire de destination</param>
		/// <param name="idMySession">Identifiant du résultat</param>
		/// <param name="webSession">Session du client</param>	
		public static bool MoveSession(Int64 idOldDirectory, Int64 idNewDirectory, Int64 idMySession, WebSession webSession)
        {
            bool result = false;
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region requête
            string sql = "UPDATE " + mySessionTable.Sql;
            sql += " SET ID_DIRECTORY=" + idNewDirectory + ", DATE_MODIFICATION=sysdate ";
            sql += " WHERE ID_DIRECTORY=" + idOldDirectory + "";
            sql += " and ID_MY_SESSION=" + idMySession + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Update(sql);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Déplace une session", err));
            }
            return result;
            #endregion
        }

        /// <summary>
		///  Renomme une session
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="newName">Nouveau nom de la session</param>
		/// <param name="idMySession">Identifiant de la session</param>
		/// <param name="webSession">web Session</param>
		public static bool RenameSession(string newName, Int64 idMySession, WebSession webSession)
        {
            bool result = false;
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region requête
            string sql = "UPDATE " + mySessionTable.Sql;
            sql += " SET MY_SESSION ='" + newName + "', DATE_MODIFICATION=sysdate ";
            sql += " WHERE ID_MY_SESSION=" + idMySession + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Update(sql);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Renommer une session sauvegardée", err));
            }
            #endregion
            return result;
        }

        /// <summary>
		/// Suppression de la sauvegarde
		/// </summary>
		public static bool DeleteSession(long idSession, WebSession webSession)
        {
            bool result = false;
            if (idSession > 0)
            {
                #region Construction de la requête SQL
                Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

                string sql = "delete from " + mySessionTable.Sql;
                sql += " where id_my_session=" + idSession;
                #endregion

                #region Execution de la requête
                try
                {
                    webSession.Source.Delete(sql);
                    result = true;
                }
                catch (System.Exception err)
                {
                    //throw (new MySessionDataAccessException("Impossible de supprimer la sauvegarde", err));
                }
            }
            #endregion
            
            return result;
        }

        public static bool IsDirectoryExist(WebSession webSession, string DirectoryName)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            bool result = false;
            #region Construction de la requête
            string sql = "select distinct " + directoryTable.Prefix + ".ID_DIRECTORY, " + directoryTable.Prefix + ".DIRECTORY ";
            sql += "  from " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and UPPER(" + directoryTable.Prefix + ".DIRECTORY)=UPPER('" + DirectoryName + "') ";
            #endregion

            #region Execution de la requête
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0)
                    result = true;               
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de vérifier l'existance d'un répertoire", err));
            }
            return result;
            #endregion
        }

        public static bool CreateDirectory(string nameDirectory, WebSession webSession)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            TNS.AdExpress.Domain.DataBaseDescription.Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
            bool result = false;
            #region Requête pour insérer les champs dans la table Group_universe_client	
            string sql = "INSERT INTO " + directoryTable.Sql + "(ID_DIRECTORY,id_login,DIRECTORY,DATE_CREATION,ACTIVATION) ";
            sql += "  values (" + schema.Label + ".seq_directory.nextval," + webSession.CustomerLogin.IdLogin + ",'" + nameDirectory + "',SYSDATE," + TNS.AdExpress.Constantes.DB.ActivationValues.ACTIVATED + ")";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Insert(sql);
                result = true;
            }
            catch (System.Exception)
            {
                
            }
            return result;
            #endregion
        }

        public static bool RenameDirectory(string newName, Int64 idDirectory, WebSession webSession)
        {
            bool result = false;
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            
            #region Requête pour mettre à jour le nom du répertoire dans la table	
            string sql1 = "update " + directoryTable.Sql;
            sql1 += "  SET DIRECTORY ='" + newName + "', DATE_MODIFICATION = SYSDATE ";
            sql1 += " where ID_DIRECTORY =" + idDirectory + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Update(sql1);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Renommer un répertoire", err));
            }
            return result;
            #endregion
        }

        public static bool DropDirectory(Int64 idDirectory, WebSession webSession)
        {
            bool result = false;
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);

            #region requête
            string sql = " delete from " + directoryTable.Sql;
            sql += " where ID_DIRECTORY=" + idDirectory + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Delete(sql);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Supprimer un répertoire dans la table Directory", err));
            }
            return result;
            #endregion
        }

        public static bool ContainsDirectories(WebSession webSession)
        {
            bool result = false;
            #region Construction de la requête
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);

            //Requête pour récupérer tous les univers d'un idLogin
            string sql = "select distinct " + directoryTable.Prefix + ".ID_DIRECTORY, " + directoryTable.Prefix + ".DIRECTORY ";
            sql += " from " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " order by " + directoryTable.Prefix + ".DIRECTORY ";

            #endregion

            #region Execution de la requête
            try
            {
                DataSet ds =webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 1)
                    result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible d'obtenir la liste des répertoires", err));
            }
            #endregion
            return result;
        }

        public static bool IsSessionsInDirectoryExist(WebSession webSession, Int64 idDirectory)
        {
            bool result = false;
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region Construction de la requête
            string sql = "select " + mySessionTable.Prefix + ".MY_SESSION from " + mySessionTable.SqlWithPrefix + ", ";
            sql += " " + directoryTable.SqlWithPrefix;
            sql += "  where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + mySessionTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + directoryTable.Prefix + ".ID_DIRECTORY = " + mySessionTable.Prefix + ".ID_DIRECTORY ";
            sql += " and " + directoryTable.Prefix + ".ID_DIRECTORY=" + idDirectory + " ";
            #endregion

            #region Execution de la requête
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0)
                    result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de vérifier s'il existe des sessions dans un répertoire", err));
            }
            return result;
            #endregion
        }
        
        /// <summary>
		/// Sauvegarde d'une nouvelle session
		/// </summary>
		/// <param name="idDirectory">Identifiant du répertoire de sauvegarde</param>
		/// <param name="mySession">Nom de la session</param>
		/// <param name="webSession">Session du client</param>
		public static bool SaveMySession(Int64 idDirectory, string mySession, WebSession webSession)
        {

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    connection.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("Impossible d'ouvrir la base de données :", e));
                }
            }
            #endregion

            #region Sérialisation et sauvegarde de la session
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;


            try
            {
                Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);
                TNS.AdExpress.Domain.DataBaseDescription.Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, webSession);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " INSERT INTO " + mySessionTable.Sql +
                    " (ID_MY_SESSION, ID_DIRECTORY, MY_SESSION, BLOB_CONTENT, DATE_CREATION, DATE_MODIFICATION, ACTIVATION) " +
                    " VALUES " +
                    " (" + schema.Label + ".seq_my_session.nextval, " + idDirectory + ", '" + mySession + "', :1, sysdate, sysdate," + ActivationValues.ACTIVATED + "); " +
                    " END; ";
                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                //Fill parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;

                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            #endregion

            #region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
            catch (System.Exception e)
            {
                // Fermeture des structures
                try
                {
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et)
                {
                    throw (new MySessionDataAccessException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new MySessionDataAccessException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures
                ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et)
            {
                throw (new MySessionDataAccessException("WebSession.Save() : Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            return (success);
        }

        /// <summary>
		/// Mise à jour d'une session
		/// </summary>
		/// <param name="idDirectory">Identifiant du répertoire de sauvegarde</param>
		/// <param name="idMySession">Id session</param>
		/// <param name="mySession">Nom de la session</param>
		/// <param name="webSession">Session du client</param>
		public static bool UpdateMySession(Int64 idDirectory, string idMySession, string mySession, WebSession webSession)
        {

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    connection.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("Impossible d'ouvrir la base de données :" + e.Message));
                }
            }
            #endregion

            #region Sérialisation et Mise à jour de la session
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            try
            {
                Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, webSession);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //mise à jour de la session
                string sql = " BEGIN " +
                    " UPDATE " + mySessionTable.Sql +
                    " SET BLOB_CONTENT = :1,ID_DIRECTORY=" + idDirectory + ",MY_SESSION='" + mySession + "',DATE_MODIFICATION=sysdate" +
                    " WHERE ID_MY_SESSION=" + idMySession + " ;" +
                    " END; ";
                //Exécution de la requête
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                //parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;
                //Execution PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            #endregion

            #region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
            catch (System.Exception e)
            {
                // Fermeture des structures
                try
                {
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et)
                {
                    throw (new MySessionDataAccessException("UpdateMySession : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new MySessionDataAccessException("UpdateMySession : Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures
                ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et)
            {
                throw (new MySessionDataAccessException("UpdateMySession : Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            return (success);
        }

        /// <summary>
		/// Retourne le nom d'une session à partir de son identifiant
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idSession">Identifiant de la session à charger</param>
		/// <param name="webSession">Session du client</param>
		/// <returns></returns>
		public static string GetSession(Int64 idSession, WebSession webSession)
        {
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region Requête
            string sql = " select MY_SESSION ";
            sql += " FROM " + mySessionTable.Sql;
            sql += " WHERE ID_MY_SESSION=" + idSession + "";
            #endregion

            #region Execution de la requête
            try
            {
                return (webSession.Source.Fill(sql).Tables[0].Rows[0][0].ToString());
            }
            catch (System.Exception err)
            {
                throw (new MySessionDataAccessException("Impossible d'obtenir le nom d'une session à partir de son identifiant", err));
            }
            #endregion
        }

        /// <summary>
		/// Vérifie si une session est déjà enregistrée dans la base de données
		/// </summary>
		/// <remarks>Pas testée</remarks>
		/// <param name="webSession">webSession</param>
		/// <param name="sessionName">Nom de la session</param>
		/// <returns>Retourne vrai si la session existe</returns>
		public static bool IsSessionExist(WebSession webSession, string sessionName)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region Construction de la requête
            string sql = "select " + mySessionTable.Prefix + ".MY_SESSION from " + mySessionTable.SqlWithPrefix + ", ";
            sql += " " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + mySessionTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + directoryTable.Prefix + ".ID_DIRECTORY = " + mySessionTable.Prefix + ".ID_DIRECTORY ";
            sql += " and UPPER(" + mySessionTable.Prefix + ".MY_SESSION) like UPPER('" + sessionName + "')";
            #endregion

            #region Execution de la requête
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0) return (true);
                return (false);
            }
            catch (System.Exception err)
            {
                throw (new MySessionDataAccessException("Impossible de vérifier si une session est déjà enregistrée dans la base de données", err));
            }
            #endregion
        }
    }
}
