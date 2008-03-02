#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.DataBaseDescription {
    /// <summary>
    /// Customer connection
    /// </summary>
    public class DefaultConnection:CustomerConnection {

        #region Variables
        /// <summary>
        /// Default Connection Id
        /// </summary>
        private DefaultConnectionIds _id; 
        /// <summary>
        /// Database user name
        /// </summary>
        private string _userId;
        /// <summary>
        /// Database password
        /// </summary>
        private string _password;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultConnection(DefaultConnectionIds id){
            _id=id;
        } 
        #endregion

    }
}
