
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
