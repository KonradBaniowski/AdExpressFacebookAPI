#region Information
//  Author : Y. R'kaina
//  Creation  date: 06/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.WebService.Domain.Configuration {
    /// <summary>
    /// Creative Information
    /// </summary>
    public class ImpersonateInformation {

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
		public string Domain{
			get{return _domain;}
		}
		/// <summary>
        /// Get Domain
		/// </summary>
        public string UserName {
			get{return _userName;}
		}
		/// <summary>
        /// Get Password
		/// </summary>
		public string Password{
			get{return _password;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
        public ImpersonateInformation(String userName, String domain, String password) {
            _userName = userName;
            _domain = domain;
            _password = password;
		}
		#endregion

    }
}
