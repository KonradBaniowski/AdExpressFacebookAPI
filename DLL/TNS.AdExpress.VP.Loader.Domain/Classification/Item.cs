using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class Item {

        #region Variables
        /// <summary>
        /// Status ID
        /// </summary>
        Int64 _id;
        /// <summary>
        /// Status Text
        /// </summary>
        string _label = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="id">Clip text</param>
        /// <param name="label">Creation Date</param>
        public Item(Int64 id, string label) {
            if(label == null) throw new ArgumentNullException("Label parameter is null");
            if(label.Length <= 0) throw new ArgumentException("Label parameter is invalid");
            _id = id;
            _label = label;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public Item() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get ID
        /// </summary>
        public Int64 Id {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Get Label
        /// </summary>
        public string Label {
            get { return _label; }
            set { _label = value; }
        }
        #endregion
    }
}
