using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.DataBaseDescription {
    public class DefaultConnection:CustomerConnection {
        /// <summary>
        /// Database user name
        /// </summary>
        private string _userId;

        /// <summary>
        /// Database password
        /// </summary>
        private string _password;
        private DefaultConnectionIds _id;
    }
}
