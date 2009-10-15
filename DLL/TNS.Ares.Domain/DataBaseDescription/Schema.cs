#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Domain.DataBaseDescription {
    /// <summary>
    /// Database schema description
    /// </summary>
    public class Schema {

        #region variables
        /// <summary>
        /// Schema id
        /// </summary>
        private SchemaIds _id;
        /// <summary>
        /// Schema Name
        /// </summary>
        private string _label;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schemaId">Schema id</param>
        /// <param name="label">Schema Name</param>
        public Schema(SchemaIds schemaId,string label) {
            if(label==null ||label.Length==0)throw (new ArgumentException("Invalid label parameter"));
            _label=label;
            _id=schemaId;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get SQL code for the schema
        /// </summary>
        /// <remarks>
        /// A space is put before the schema
        /// </remarks>
        /// <example> adexpr03.</example>
        public string Sql {
            get{return(" "+_label+".");}
        }
        /// <summary>
        /// Get Schema label
        /// </summary>
        /// <remarks>
        /// <example> adexpr03</example>
        public string Label {
            get { return (_label); }
        }
        #endregion

    }
}
