using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using KMI.P3.Web.Core.Exceptions;
using CoreCst = KMI.P3.Constantes.Web;
using TNS.FrameWork.DB.Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using Oracle.DataAccess.Client;
using KMI.P3.Domain.Web;
using Oracle.DataAccess.Types;
using KMI.P3.Domain.DataBaseDescription;
using TNS.Isis.Right.Common;
namespace KMI.P3.Web.Core.Sessions
{
     [System.Serializable]
    public class WebSession 
    {
        #region Paramètres de la session
        /// <summary>
        ///Session ID
        /// </summary>
        protected string _sessionId = string.Empty;
        /// <summary>
        ///Session ID
        /// </summary>
        private string _login = string.Empty;

        /// <summary>
        ///Session ID
        /// </summary>
        protected string _password = string.Empty;
        /// <summary>
        /// Contient les nouvelles variables sessions
        /// </summary>
        protected Hashtable _userParameters = new Hashtable();
        /// <summary>
        /// Date de dernère modification de la session
        /// </summary>
        [System.NonSerialized]
        protected DateTime _modificationDate = DateTime.Now;
        #region Langues
        /// <summary>
        /// Langage du site
        /// </summary>
        [System.NonSerialized]
        protected int _siteLanguage = KMI.P3.Constantes.DB.Language.FRENCH;
        #endregion

        #endregion

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public WebSession()
        {
        }

        public WebSession(P3Login p3Login)
        {
            try {
                 _sessionId = DateTime.Now.ToString("yyyyMMddHHmmss") + Convert.ToString(p3Login.LoginId);
                 _login = p3Login.Label;
                 _password = p3Login.Password;
            }
            catch (System.Exception e) {
                throw new WebSessionException("WebSession.WebSession(...) : Paramètre \"login\" invalide : " + e.Message);
            }
        }

         #region Session Id
        /// <summary>
        /// Get Session Id
        /// </summary>
        public string SessionId {
            get {
                if (!string.IsNullOrEmpty(_sessionId))return (_sessionId);
                else throw (new NotImplementedException("Undefine Session Id"));
            }
        }
        #endregion

        #region Data Source
        /// <summary>
        /// Get Data Source
        /// </summary>
        public IDataSource Source
        {
            get
            {
                if (!_userParameters.ContainsKey(CoreCst.Core.SessionParamters.dataSource)){
                    if(!string.IsNullOrEmpty(_login) && !string.IsNullOrEmpty(_password)){
                        _userParameters[CoreCst.Core.SessionParamters.dataSource]=WebApplicationParameters.DataBaseDescription.GetCustomerConnection(_login, _password, CustomerConnectionIds.p3);
                    }
                    else throw (new NotImplementedException("Undefine Data Source"));
                }
                return ((IDataSource)_userParameters[CoreCst.Core.SessionParamters.dataSource]);
            }
        }
        #endregion

        #region Langues
        /// <summary>
        /// Get or Set Langues
        /// </summary>
        public int SiteLanguage
        {
            get
            {
                if (_siteLanguage == 0)
                {
                    if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.siteLanguage))
                        _siteLanguage = (int)_userParameters[CoreCst.Core.SessionParamters.siteLanguage];
                    else
                    {
                        throw (new NotImplementedException("Undefine Site Language"));
                    }
                }
                return _siteLanguage;
            }
            set
            {
                _siteLanguage = value;
                _userParameters[CoreCst.Core.SessionParamters.siteLanguage] = value;
                _modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Modification Date
        /// <summary>
        /// Get or Set Modification Date
        /// </summary>
        public DateTime ModificationDate
        {
            get
            {
                if (_modificationDate == null)
                {
                    if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.modificationDate))
                        _modificationDate = (DateTime)_userParameters[CoreCst.Core.SessionParamters.modificationDate];
                    else
                    {
                        throw (new NotImplementedException("Undefine Modification Date"));
                    }
                }
                return _modificationDate;
            }
            set
            {
                _modificationDate = value;
                _userParameters[CoreCst.Core.SessionParamters.modificationDate] = value;
                _modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Information Navigation
        /// <summary>
        /// Get/Set navigator informations
        /// </summary>
        public string Browser
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.browser))
                    return (_userParameters[CoreCst.Core.SessionParamters.browser].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.browser] = value; }

        }
        /// <summary>
        /// Get/Set navigator informations version
        /// </summary>
        public string BrowserVersion
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.browserVersion))
                    return (_userParameters[CoreCst.Core.SessionParamters.browserVersion].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.browserVersion] = value; }

        }


        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string UserAgent
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.userAgent))
                    return (_userParameters[CoreCst.Core.SessionParamters.userAgent].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.userAgent] = value; }

        }

        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string CustomerOs
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.customerOs))
                    return (_userParameters[CoreCst.Core.SessionParamters.customerOs].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.customerOs] = value; }

        }

        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string CustomerIp
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.customerIp))
                    return (_userParameters[CoreCst.Core.SessionParamters.customerIp].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.customerIp] = value; }

        }

        /// <summary>
        /// Get/Set last url set un WebPage (AdExpress base page) use for customer error
        /// </summary>
        public string LastWebPage
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.lastWebPage))
                    return (_userParameters[CoreCst.Core.SessionParamters.lastWebPage].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.lastWebPage] = value; }

        }

        /// <summary>
        /// Get/Set server name
        /// </summary>
        public string ServerName
        {
            get
            {
                if (_userParameters.ContainsKey(CoreCst.Core.SessionParamters.serverName))
                    return (_userParameters[CoreCst.Core.SessionParamters.serverName].ToString());
                return ("Not set");
            }
            set { _userParameters[CoreCst.Core.SessionParamters.serverName] = value; }

        }

        #endregion

        #region Customer rights
        /// <summary>
        /// Get customer login
        /// </summary>
        public string Login {get { return _login; }}
        /// <summary>
        /// Get customer password
        /// </summary>
        public string Password { get { return _password; } }
        #endregion

        #region Blob
        /// <summary>
        /// Méthode qui sauvegarde l'objet webSession courant dans la table de sauvegarde des sessions
        ///		Ouverture de la BD
        ///		Sérialisation en mémoire
        ///		Requête BD de sauvegarde dnas un Blob
        ///		Libération des ressources
        /// </summary>
        /// <exception cref="TNS.EasyMusic.Web.Core.Exceptions.WebSessionException">
        /// Lancée dans les cas suivant:
        ///		connection à la BD impossible à ouvrir
        ///		problème à la sértialisation ou à la sauvegarde de l'objet dans la BD
        ///		impossible de libérer les ressources
        /// </exception>
        public void Save()
        {

            #region Ouverture de la base de données
            OracleConnection cnx = ((OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.session).GetSource());
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new WebSessionException("WebSession.Save() : Impossible d'ouvrir la base de données" + e.Message));
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
                bf.Serialize(ms, this);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " DELETE " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mou01).Label + "." + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSession).Label + " WHERE ID_NAV_SESSION=" + SessionId + "; " +
                    " INSERT INTO " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mou01).Label + "." + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSession).Label + "(id_nav_session, nav_session) VALUES(" + SessionId + ", :1); " +
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
                    cnx.Close();
                    cnx.Dispose();
                }
                catch (System.Exception et)
                {
                    throw (new WebSessionException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new WebSessionException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures
                ms.Close();
                bf = null;
                sqlCommand.Dispose();
                cnx.Close();
                cnx.Dispose();
            }
            catch (System.Exception et)
            {
                throw (new WebSessionException("WebSession.Save() : Impossible de fermer la base de données : " + et.Message));
            }

            #endregion

        }


        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des sessions
        ///		Ouverture de la BD
        ///		Requête BD de sélection d'un un Blob
        ///		Désérialisation
        ///		Libération des ressources
        /// </summary>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        /// <param name="idWebSession">Identifiant de la session</param>
        /// <exception cref="TNS.EasyMusic.Web.Core.Exceptions.WebSessionException">
        /// Lancée dans les cas suivant:
        ///		connection à la BD impossible à ouvrir
        ///		problème à la sélection de l'enregistrement ou à la désérialisation
        ///		impossible de libérer les ressources
        /// </exception>
        public static Object Load(string idWebSession)
        {

            #region Ouverture de la base de données
            OracleConnection cnx = ((OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.session).GetSource());
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new WebSessionException("WebSession.Load(...) : Impossible d'ouvrir la base de données : " + e.Message));
            }
            #endregion

            #region Chargement et deserialization de l'objet
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            Object o = null;
            int i = 0;
            try
            {
                binaryData = new byte[0];
                i = 1;
                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " SELECT nav_session INTO :1 FROM " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mou01).Label + "." + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSession).Label + " WHERE id_nav_session = " + idWebSession + "; " +
                    " END; ";
                i = 2;
                sqlCommand = new OracleCommand(block);
                i = 3;
                sqlCommand.Connection = cnx;
                i = 4;
                sqlCommand.CommandType = CommandType.Text;
                i = 5;
                //Initialize parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobfromdb", OracleDbType.Blob);
                i = 6;
                param.Direction = ParameterDirection.Output;
                i = 7;

                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();
                i = 8;
                //Récupération des octets du blob
                binaryData = (byte[])((OracleBlob)(sqlCommand.Parameters[0].Value)).Value;
                i = 9;

                //Deserialization oft the object
                ms = new MemoryStream();
                i = 10;
                ms.Write(binaryData, 0, binaryData.Length);
                i = 11;
                bf = new BinaryFormatter();
                i = 12;
                ms.Position = 0;
                i = 13;
                o = bf.Deserialize(ms);
                i = 14;
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
                    cnx.Close();
                }
                catch (System.Exception et)
                {
                    throw (new WebSessionException("WebSession.Load(...) : Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new WebSessionException("WebSession.Load(...) : Problème au chargement de la session à partir de la base de données : " + e.Message + " " + i.ToString()));
            }
            try
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                if (binaryData != null) binaryData = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                cnx.Close();
            }
            catch (System.Exception et)
            {
                throw (new WebSessionException("WebSession.Load(...) : Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }
        #endregion

        #region ToBinaryData
        public byte[] ToBinaryData()
        {
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            binaryData = new byte[ms.GetBuffer().Length];
            binaryData = ms.GetBuffer();
            if (ms != null) ms.Close();
            if (bf != null) bf = null;
            return (binaryData);
        }
        #endregion


    }
}
