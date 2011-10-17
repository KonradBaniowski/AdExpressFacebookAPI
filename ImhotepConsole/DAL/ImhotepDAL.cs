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
using System.Collections;

namespace ImhotepConsole.DAL
{
    public class ImhotepDAL
    {
        const string MY_SESSION_TEST = "webnav01.my_session";//
        //const string MY_SESSION_TEST = "webnav01.my_session";//
        const string ALERT_TEST = "MAU01.ALERT";//
        ////FINLANDE
       // const string DEV_CONNECTION_STRING = "User Id=gfacon; Password=sandie5; Data Source=adexprfi01.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";

        //SLOVAKIA
        const string DEV_CONNECTION_STRING = "User Id=gfacon; Password=sandie5; Data Source=adexwebsk01.prod;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";

        const string UNIVERSE_CLIENT_TEST = "universe_client_test";

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
                    " SELECT Blob_content INTO :1 FROM " + MY_SESSION_TEST + " WHERE id_my_session = " + idWebSession + "; " +
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
                    " UPDATE " + MY_SESSION_TEST +
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
                    " UPDATE " + ALERT_TEST +
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


        public static DataSet GetVehicles(string categoryList, string vehicleList, int idLanguage)
        {
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;

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
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible d'ouvrir la base de données : " + e.Message));
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles: Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            try
            {
                string sql = "select vh.id_vehicle, vehicle ";
                if (!string.IsNullOrEmpty(categoryList)) sql += ", ct.id_category, category ";

                sql += " from ";
                sql += " RECAPFI01B.VEHICLE vh ";
                if (!string.IsNullOrEmpty(categoryList)) sql += " ,RECAPFI01B.CATEGORY ct ";
                sql += " where 0 = 0 ";
                if (!string.IsNullOrEmpty(categoryList))
                {
                    sql += " and   ct.id_category in (" + categoryList + ") ";
                    sql += " and ct.id_language=" + idLanguage;
                }
                if (!string.IsNullOrEmpty(vehicleList))
                {
                    sql += " and   vh.id_vehicle in (" + vehicleList + ") ";
                    sql += " and vh.id_language=" + idLanguage;
                }
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de charger les identifiants de session : " + et.Message));
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
            return ds;
        }

        public static DataSet GetVehicleInterectcenterMedia( string vehicleList, int idLanguage)
        {

            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;

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
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible d'ouvrir la base de données : " + e.Message));
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles: Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            try
            {
                string sql = "";
                sql += " Select distinct vh.id_vehicle,vh.vehicle,ic.id_interest_center,ic.interest_center , md.id_media , md.media ";
                sql += " from  adexprfi01.vehicle vh, adexprfi01.interest_center ic, adexprfi01.media md ";
                sql += " , adexprfi01.category ct,  adexprfi01.basic_media bm  where vh.id_language=" + idLanguage;
                sql += " and ic.id_language=" + idLanguage + " and md.id_language=" + idLanguage + " and bm.id_language=" + idLanguage;
                sql += " and ct.id_language=" + idLanguage + " and vh.activation<50 and ic.activation<50 and md.activation<50 ";
                sql += " and ct.activation<50 and bm.activation<50 and vh.id_vehicle=ct.id_vehicle ";
                sql += " and ic.id_interest_center=md.id_interest_center and ct.id_category=bm.id_category ";
                sql += " and bm.id_basic_media=md.id_basic_media and  ( vh.id_vehicle in ( "+vehicleList+" )  ) ";
                sql += " order by  vh.vehicle , ic.interest_center , md.media";
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de charger les identifiants de session : " + et.Message));
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
            return ds;
        }
        public static DataSet GetVehiclerMedia(string vehicleList, int idLanguage)
        {

            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;

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
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible d'ouvrir la base de données : " + e.Message));
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles: Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            try
            {
                string sql = "";
                sql += " Select distinct vh.id_vehicle,vh.vehicle, md.id_media , md.media ";
                sql += " from  adexprfi01.vehicle vh,  adexprfi01.media md ";
                sql += " , adexprfi01.category ct,  adexprfi01.basic_media bm  where vh.id_language=" + idLanguage;
                sql += "  and md.id_language=" + idLanguage + " and bm.id_language=" + idLanguage;
                sql += " and ct.id_language=" + idLanguage + " and vh.activation<50 and md.activation<50 ";
                sql += " and ct.activation<50 and bm.activation<50 and vh.id_vehicle=ct.id_vehicle ";
                sql += " and ct.id_category=bm.id_category ";
                sql += " and bm.id_basic_media=md.id_basic_media and  ( vh.id_vehicle in ( " + vehicleList + " )  ) ";
                sql += " order by  vh.vehicle  , md.media";
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de charger les identifiants de session : " + et.Message));
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
            return ds;
        }
        public static DataSet GetCategoriVehicles(string categoryList, int idLanguage)
        {
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;

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
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible d'ouvrir la base de données : " + e.Message));
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles: Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            try
            {
                string sql = "select * from RECAPFI01B.CATEGORY";
                sql += " where id_vehicle in (" + categoryList + ") ";
                sql += " and id_language=" + idLanguage;
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
                    throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new MySessionDataAccessException("ERROR : GetVehicles : Impossible de charger les identifiants de session : " + et.Message));
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
            return ds;
        }


        #region Univers

        /// <summary>
        /// Obtenir la liste des identifiants d'univers clients
        /// </summary>
        /// <returns>List d'identifiants séparés par une virgule</returns>
        public static string GetUniversList(string IdUniverseClientDescription, string IdTyeUniverseClient)
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
                throw (new BaseException("ImhotepDataAccess.GetUniversList(...) : Impossible d'ouvrir la base de données : " + e.Message));
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
                    throw (new BaseException("ImhotepDataAccess.GetUniversList(...) : Impossible d'ouvrir la base de données : " + e.Message));
                }
            }
            #endregion

            #region chargement des identifiants session
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            try
            {
                sql += " SELECT id_universe_client FROM " + DBConstantes.Schema.UNIVERS_SCHEMA + "." + UNIVERSE_CLIENT_TEST;
                 sql += " WHERE ID_UNIVERSE_CLIENT_DESCRIPTION in (" + IdUniverseClientDescription+") ";
                 sql += " AND ID_TYPE_UNIVERSE_CLIENT in ("+IdTyeUniverseClient+") ";
                sql+=" ORDER BY date_creation";
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
                    throw (new BaseException("ImhotepDataAccess.GetUniversList(...) : Impossible de libérer les ressources après échec de la méthode : " + e.Message));
                }
                throw (new BaseException("GetUniverseList() : Impossible de charger les identifiants de session : " + et.Message));
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
                throw (new BaseException("GetUniverseList() : Impossible de fermer la base de données : " + ex.Message));
            }
            #endregion

            string lst = "";
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lst += r[0].ToString() + ",";
            }
            if (lst.Length > 0)
            {
                lst = lst.Substring(0, lst.Length - 1);
            }
            return lst;
        }


        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un univers à partir de la table Universe Client
        /// </summary>
        /// <param name="idUniverse">Identifiant de l'univers</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        public static Object GetTreeNodeUniverse(Int64 idUniverse)
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
                    throw (new BaseException("Impossible d'ouvrir la base de données", e));
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
                    " SELECT blob_universe_client INTO :1 FROM "
                    + DBConstantes.Schema.UNIVERS_SCHEMA + "." + UNIVERSE_CLIENT_TEST
                    + " WHERE id_universe_client = " + idUniverse + "; " +
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

            #region Gestion des erreurs de chargement et de deserialization de l'objet
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
                    throw (new BaseException("Impossible de libérer les ressources après échec de la méthode", et));
                }
                throw (new BaseException("Problème au chargement de la session à partir de la base de données", e));
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
                throw (new Exception("Impossible de fermer la base de données : ", et));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }


        /// <summary>
        /// Sauvegarde d'un univers
        /// </summary>
        /// <remarks>A Tester</remarks>
        /// <param name="alUniverseTreeNode"> Liste contenant les 2 arbres utilisés pour la sauvegarde des univers</param>
        /// <returns>Retourne true si l'univers a été crée</returns>
        public static bool UpDateUniverse(Int64 idUniverse, Int64 idTypeUniverseclient,ArrayList alUniverseTreeNode)
        {

            #region Ouverture de la base de données
            OracleConnection cnx = new OracleConnection(DEV_CONNECTION_STRING);
            bool DBToClosed = false;
            bool success = false;
            if (cnx.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    cnx.Open();
                }
                catch (System.Exception e)
                {
                    throw (new BaseException("Impossible d'ouvrir la base de données", e));
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
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, alUniverseTreeNode);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " UPDATE " + DBConstantes.Schema.UNIVERS_SCHEMA + "." + UNIVERSE_CLIENT_TEST +
                    " SET BLOB_UNIVERSE_CLIENT = :1,DATE_MODIFICATION=sysdate,ID_TYPE_UNIVERSE_CLIENT ="+  idTypeUniverseclient + 
                    " WHERE ID_UNIVERSE_CLIENT=" + idUniverse + " ;" +                 
                    " END; ";

                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = cnx;
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
                    if (DBToClosed) cnx.Close();
                }
                catch (System.Exception et)
                {
                    throw (new BaseException("Impossible de libérer les ressources après échec de la méthode" + et));
                }
                throw (new BaseException("Echec de la sauvegarde de l'objet dans la base de donnée" + e));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures
                ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) cnx.Close();
                success = true;
            }
            catch (System.Exception et)
            {
                throw (new BaseException("Impossible de fermer la base de données", et));
            }
            #endregion

            return (success);
        }

        #endregion

    }
}
