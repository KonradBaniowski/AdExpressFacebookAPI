#region Information
//  Author : Y. R'kaina
//  Creation  date: 18/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.DataBaseDescription {
    /// <summary>
    /// Database view description
    /// </summary>
    public class View {

        #region variables
        /// <summary>
        /// Id
        /// </summary>
        private ViewIds _id;
        /// <summary>
        /// View name
        /// </summary>
        private string _label;
        /// <summary>
        /// Prefix view
        /// </summary>
        private string _prefix;
        /// <summary>
        /// Schema Id
        /// </summary>
        private SchemaIds _schemaId;
        /// <summary>
        /// Schema
        /// </summary>
        private Schema _schema = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewId">Id</param>
        /// <param name="label">Table label</param>
        /// <param name="prefix">Table prefix</param>
        public View(ViewIds viewId,string label,string prefix,Schema schema) {
            if(label==null || label.Length==0) throw (new ArgumentException("Invalid label parameter"));
            if(prefix==null || prefix.Length==0) throw (new ArgumentException("Invalid prefix parameter"));
            if(schema==null) throw (new ArgumentException("Invalid schema parameter"));
            _id=viewId;
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
        /// <example>am</example>
        public string Prefix {
            get { return (_prefix); }
        }

        /// <summary>
        /// Get SQL code
        /// Schema.View prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example>adexpr03.all_media am</example>
        public string SqlWithPrefix {
            get { return (_schema.Sql + _label + " " + _prefix); }
        }
        /// <summary>
        /// Get SQL code
        /// Schema.View
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example>adexpr03.all_media</example>
        public string Sql {
            get { return (_schema.Sql + _label); }
        }
        /// <summary>
        /// Get view label
        /// View
        /// </summary>
        /// <example>all_media</example>
        public string Label {
            get { return (_label); }
        }

        /// <summary>
        /// Get view label with prefix
        /// View prefix
        /// </summary>
        /// <example>all_media</example>
        public string LabelWithPrefix {
            get { return (_label + " " + _prefix); }
        }
        #endregion

    }
}
