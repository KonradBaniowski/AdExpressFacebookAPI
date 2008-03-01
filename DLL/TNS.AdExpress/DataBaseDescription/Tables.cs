using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.DataBaseDescription {
    public class Table {
        /// <summary>
        /// Id
        /// </summary>
        private TableIds _id;
        /// <summary>
        /// Table name
        /// </summary>
        private string _name;
        /// <summary>
        /// Prefix table
        /// </summary>
        private string _prefix;
        private SchemaIds _schema;
    
        public TableIds Id {
            get {
                throw new System.NotImplementedException();
            }
            set {
            }
        }
    }
}
