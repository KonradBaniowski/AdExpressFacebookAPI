using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.Core.Exceptions;

namespace KMI.PromoPSA.Web.Core {
    /// <summary>
    /// Customer rights
    /// </summary>
    [System.Serializable]
    public class Right {

        #region Variables
        /// <summary>
        /// identifiant login
        /// </summary>		
        protected Int64 _loginId;
        /// <summary>
        /// login
        /// </summary>
        protected string _login;
        /// <summary>
        /// mot de passe
        /// </summary>
        protected string _password;
        /// <summary>
        /// Vérifie si les droits ont été déterminés
        /// </summary>
        protected bool rightDetermined;
        /// <summary>
        /// Indique si l'utilisateur a le droit de se connecter
        /// </summary>
        protected bool rightValidated;
        /// <summary>
        /// date de connection
        /// </summary>		
        protected DateTime _connectionDate;
        /// <summary>
        /// Date de modification des droits utilisateur
        /// </summary>
        protected DateTime _lastModificationDate;
        /// <summary>
        /// Site language
        /// </summary>
        protected int _siteLanguage = 33;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="login">Customer Login</param>
        /// <param name="password">Customer Password</param>
        public Right(string login, string password) {
            if (string.IsNullOrEmpty(login)) throw (new ArgumentException("Invalid login parameter"));
            if (string.IsNullOrEmpty(password)) throw (new ArgumentException("Invalid password parameter"));
            _login = login;
            _password = password;
            _connectionDate = DateTime.Now;

            try {
                IResults results = new Results();
                _loginId = results.GetPSALoginId(_login, _password);
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to access to the Database", err));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get login Id
        /// </summary>
        public Int64 IdLogin {
            get { return _loginId; }
        }
        /// <summary>
        /// Get login
        /// </summary>
        public string Login {
            get { return _login; }
        }
        /// <summary>
        /// Get password
        /// </summary>
        public string Password {
            get { return _password; }
        }
        #endregion

        #region PSA Access

        #region CanAccessToPSA
        /// <summary>
        /// Vérifie l'existence du projet PSA 
        /// avec au moins un module.
        /// Si true assigne idLogin
        /// </summary>
        /// <returns></returns>
        public bool CanAccessToPSA() {

            try {
                IResults results = new Results();
                return results.CanAccessToPSA(_login, _password);
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to access to the Database", err));
            }
        }
        #endregion

        #region CheckLogin
        /// <summary>
        /// vérifie le Login-mot de passe
        /// </summary>
        /// <returns>true si login-mot de passe correct, false sinon</returns>
        public bool CheckLogin() {
            
            try {
                 IResults results = new Results();
                _loginId = results.GetPSALoginId(_login, _password);
                if (_loginId < 0) return (false);
                return (true);
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to access to the Database", err));
            }

        }
        #endregion

        #endregion
    }
}
