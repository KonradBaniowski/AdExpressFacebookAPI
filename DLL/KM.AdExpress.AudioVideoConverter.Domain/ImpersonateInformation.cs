using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.Domain
{
    // <summary>
    /// Impersonate Information
    /// </summary>
    public class ImpersonateInformation
    {

        #region variables
        /// <summary>
        /// User Name
        /// </summary>
        private string _userName;
        /// <summary>
        /// Domain
        /// </summary>
        private string _domain;
        /// <summary>
        /// Password
        /// </summary>
        private string _password;
        #endregion

        #region Propriétés
        /// <summary>
        /// Get User Name
        /// </summary>
        public string Domain
        {
            get { return _domain; }
        }
        /// <summary>
        /// Get Domain
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }
        /// <summary>
        /// Get Password
        /// </summary>
        public string Password
        {
            get { return _password; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="userName">user Name</param>  
        /// <param name="password"></param>
        /// <param name="domain"></param>
        public ImpersonateInformation(String userName, String password, String domain)
        {
            _userName = userName;
            _domain = domain;
            _password = password;
        }
        #endregion

    }
}
