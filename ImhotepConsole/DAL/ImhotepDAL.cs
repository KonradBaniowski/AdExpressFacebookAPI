using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using TNS.AdExpress.Web.Core.Sessions;

//using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Exceptions;

using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.Web;
using DateFrameWork = TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;
using System.IO;

namespace ImhotepConsole.DAL
{
    public class ImhotepDAL
    {
        const string MY_SESSION_TEST = "webnav01.my_session";//
        const string ALERT_TEST = "MAU01.ALERT";//
        const string DEV_CONNECTION_STRING = "User Id=gfacon; Password=sandie5; Data Source=adexpr03.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";
        const int ID_ALERT_TYPE = 2;

        #region Chargement des identifiants session
        /// <summary>
        /// Charge les identifiants de session 
        /// </summary>
        /// <returns>liste des identifiants de session</returns>
        public static DataSet LoadSessionIds()
        {
            string sql = "";
            DataSet ds = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;

            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible d'ouvrir la base de données : " + e.Message));
            }

            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region chargement des identifiants session
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            try
            {
                sql += " SELECT id_my_session,my_session FROM " + MY_SESSION_TEST + " ORDER BY id_my_session";
                ds = new DataSet();
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = cnx;
                sqlAdapter = new OracleDataAdapter(sqlCommand);
                sqlAdapter.Fill(ds);
            }
            catch (System.Exception et)
            {
                try
                {
                    // Fermeture des structures
                    if (sqlAdapter != null) sqlAdapter.Dispose();
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible de charger les identifiants de session : " + et.Message));
            }
            try
            {
                // Fermeture des structures
                if (sqlAdapter != null) sqlAdapter.Dispose();
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception ex)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() :  Impossible de fermer la base de données : " + ex.Message));
            }
            #endregion

            return ds;
        }
        #endregion

        #region Chargement des identifiants session Alerts
        /// <summary>
        /// Charge les identifiants de session alerts
        /// </summary>
        /// <returns>liste des identifiants de session</returns>
        public static DataSet LoadSessionAlertIds()
        {
            string sql = "";
            DataSet ds = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;

            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible d'ouvrir la base de données : " + e.Message));
            }

            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region chargement des identifiants session
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            try
            {
                sql += " SELECT id_alert,alert FROM " + ALERT_TEST + " WHERE  id_alert_type =  " + ID_ALERT_TYPE + " ORDER BY id_alert";
                ds = new DataSet();
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = cnx;
                sqlAdapter = new OracleDataAdapter(sqlCommand);
                sqlAdapter.Fill(ds);
            }
            catch (System.Exception et)
            {
                try
                {
                    // Fermeture des structures
                    if (sqlAdapter != null) sqlAdapter.Dispose();
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() : Impossible de charger les identifiants de session : " + et.Message));
            }
            try
            {
                // Fermeture des structures
                if (sqlAdapter != null) sqlAdapter.Dispose();
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception ex)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadSessionIds() :  Impossible de fermer la base de données : " + ex.Message));
            }
            #endregion

            return ds;
        }
        #endregion

        #region Chargement de l'objet session
        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des sessions
        /// </summary>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        /// <param name="idWebSession">Identifiant de la session</param>
        /// <param name="webSession">Session à la connexion d l'utilisateur</param>
        public static Object LoadMySession(string idWebSession)
        {

            #region Ouverture de la base de données
            bool DBToClosed = false;
            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region Chargement et deserialization de l'objet
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            Object o = null;
            try
            {
                binaryData = new byte[0];
                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " SELECT Blob_content INTO :1 FROM "  + MY_SESSION_TEST + " WHERE id_my_session = " + idWebSession + "; " +
                    " END; ";
                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //Initialize parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobfromdb1", OracleDbType.Blob);
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

            #region Gestion des erreurs de chargement et de désérialization de l'objet
            catch (System.Exception e)
            {
                try
                {
                    // Fermeture des structures
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (binaryData != null) binaryData = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception et)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Problème au chargement de la session à partir de la base de données : " + e.Message));
            }
            try
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                if (binaryData != null) binaryData = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception et)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }
        #endregion

        #region Chargement de l'objet session alert
        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des sessions
        /// </summary>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        /// <param name="idWebSession">Identifiant de la session</param>
        /// <param name="webSession">Session à la connexion d l'utilisateur</param>
        public static Object LoadMySessionAlert(string idWebSession)
        {

            #region Ouverture de la base de données
            bool DBToClosed = false;
            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region Chargement et deserialization de l'objet
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            Object o = null;
            try
            {
                binaryData = new byte[0];
                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " SELECT session_ INTO :1 FROM " + ALERT_TEST + " WHERE id_alert = " + idWebSession + " AND id_alert_type =  " + ID_ALERT_TYPE + " ; " +
                    " END; ";
                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //Initialize parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobfromdb1", OracleDbType.Blob);
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

            #region Gestion des erreurs de chargement et de désérialization de l'objet
            catch (System.Exception e)
            {
                try
                {
                    // Fermeture des structures
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (binaryData != null) binaryData = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception et)
                {
                    throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Problème au chargement de la session à partir de la base de données : " + e.Message));
            }
            try
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                if (binaryData != null) binaryData = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception et)
            {
                throw (new MySessionDataAccessException("ERROR : ImhotepDAL.LoadMySession(string idWebSession) : Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }
        #endregion

        #region Enregistrement de la session
        /// <summary>
        /// Met à jour d'une session
        /// </summary>
        /// <param name="idWebSession">identifiant de la sesion</param>
        /// <param name="session">objet session</param>
        public static void UpdateSession(string idWebSession, Object session)
        {
            string sql = "";
            DataSet ds = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;

            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);

            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : UpdateSession(...) : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region Sérialisation et enregistrement de l'objet session
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            try
            {
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, session);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //mise à jour de la session
                sql = " BEGIN " +
                    " UPDATE " +  MY_SESSION_TEST +
                    " SET BLOB_CONTENT = :1,DATE_MODIFICATION=sysdate" +
                    " WHERE ID_MY_SESSION=" + idWebSession + " ;" +
                    " END; ";
                //Exécution de la requête
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;
                //Execution PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception et)
            {
                try
                {
                    // Fermeture des structures
                    if (sqlAdapter != null) sqlAdapter.Dispose();
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : UpdateSession(...) : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException(" ERROR : UpdateSession(...) : Impossible de sauvegarder la session : " + et.Message));
            }
            try
            {
                // Fermeture des structures
                if (sqlAdapter != null) sqlAdapter.Dispose();
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception ex)
            {
                throw (new MySessionDataAccessException(" ERROR : UpdateSession(...) : Impossible de fermer la base de données : " + ex.Message));
            }
            #endregion
        }
        #endregion

        #region Enregistrement de la session
        /// <summary>
        /// Met à jour d'une session
        /// </summary>
        /// <param name="idWebSession">identifiant de la sesion</param>
        /// <param name="session">objet session</param>
        public static void UpdateSessionAlert(string idWebSession, Object session)
        {
            string sql = "";
            DataSet ds = null;

            #region Ouverture de la base de données
            bool DBToClosed = false;

            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);

            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : UpdateSession(...) : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region Sérialisation et enregistrement de l'objet session
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            try
            {
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, session);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //mise à jour de la session
                sql = " BEGIN " +
                    " UPDATE " +ALERT_TEST +
                    " SET session_ = :1,DATE_MODIFICATION=sysdate" +
                    " WHERE ID_ALERT=" + idWebSession + " ;" +
                    " END; ";
                //Exécution de la requête
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;
                //Execution PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception et)
            {
                try
                {
                    // Fermeture des structures
                    if (sqlAdapter != null) sqlAdapter.Dispose();
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception e)
                {
                    throw (new MySessionDataAccessException("ERROR : UpdateSession(...) : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException(" ERROR : UpdateSession(...) : Impossible de sauvegarder la session : " + et.Message));
            }
            try
            {
                // Fermeture des structures
                if (sqlAdapter != null) sqlAdapter.Dispose();
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
            }
            catch (System.Exception ex)
            {
                throw (new MySessionDataAccessException(" ERROR : UpdateSession(...) : Impossible de fermer la base de données : " + ex.Message));
            }
            #endregion
        }
        #endregion

    }
}
