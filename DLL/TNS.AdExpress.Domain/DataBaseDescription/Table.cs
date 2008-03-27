#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.DataBaseDescription {
    /// <summary>
    /// Database table description
    /// </summary>
    public class Table {

        #region variables
        /// <summary>
        /// Id
        /// </summary>
        private TableIds _id;
        /// <summary>
        /// Table name
        /// </summary>
        private string _label;
        /// <summary>
        /// Prefix table
        /// </summary>
        private string _prefix;
        /// <summary>
        /// Schema Id
        /// </summary>
        private SchemaIds _schemaId;
        /// <summary>
        /// Schema
        /// </summary>
        private Schema _schema=null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableId">Id</param>
        /// <param name="label">Table label</param>
        /// <param name="prefix">Table prefix</param>
        public Table(TableIds tableId,string label,string prefix,Schema schema) {
            if(label==null || label.Length==0) throw (new ArgumentException("Invalid label parameter"));
            if(prefix==null || prefix.Length==0) throw (new ArgumentException("Invalid prefix parameter"));
            if(schema==null) throw (new ArgumentException("Invalid schema parameter"));
            _id=tableId;
            _label=label;
            _prefix=prefix;
            _schema=schema;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// Get SQL code
        /// prefix
        /// </summary>
        /// <example> wp</example>
        public string Prefix {
            get { return (_prefix); }
        }

        /// <summary>
        /// Get SQL code
        /// Schema.Table prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example> adexpr03.data_press_4M wp</example>
        public string SqlWithPrefix {
            get { return (_schema.Sql+_label+" "+_prefix); }
        }
        /// <summary>
        /// Get SQL code
        /// Schema.Table
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example> adexpr03.data_press_4M</example>
        public string Sql{
            get { return (_schema.Sql+_label); }
        }
        /// <summary>
        /// Get table label
        /// Table
        /// </summary>
        /// <example> data_press_4M</example>
        public string Label {
            get { return (_label); }
        }

        /// <summary>
        /// Get table label
        /// Table prefix
        /// </summary>
        /// <example> data_press_4M wp</example>
        public string LabelWithPrefix {
            get { return (_label+" "+_prefix); }
        }
        #endregion
    }
}
