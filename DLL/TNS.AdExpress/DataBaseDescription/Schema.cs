using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.DataBaseDescription {
    public class Schema {

        #region variables
        /// <summary>
        /// Id
        /// </summary>
        private SchemaIds _id;
        #endregion
        /// <summary>
        /// Schema Name
        /// </summary>
        private string _name;

        #region Accessors
        /// <summary>
        /// Id
        /// </summary>
        public SchemaIds Id {
            get { return (_id); }
        }
        #endregion

    }
}
