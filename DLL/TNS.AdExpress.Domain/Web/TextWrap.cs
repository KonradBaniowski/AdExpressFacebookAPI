#region Informations
// Author: J. Carannante
// Creation Date: 24/12/2010
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Web {
    /// <summary>
    /// TextWrap description
    /// </summary>
    public class TextWrap {

        #region Variables
        /// <summary>
        /// number of character 
        /// </summary>
        private int _nbChar = 0;
        /// <summary>
        /// number of character for header
        /// </summary>
        private int _nbCharHeader = 0;
        /// <summary>
        /// number of character for Description
        /// </summary>
        private int _nbCharDescription = 0;
        /// <summary>
        /// Tolerance
        /// </summary>
        private int _offset = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public TextWrap() {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get number of character 
        /// </summary>
        public int NbChar {
            get { return (_nbChar); }
            set { _nbChar = value; }
        }
        /// <summary>
        /// Get number of character for header
        /// </summary>
        public int NbCharHeader {
            get { return (_nbCharHeader); }
            set { _nbCharHeader = value; }
        }
        /// <summary>
        /// Get number of character for Description
        /// </summary>
        public int NbCharDescription
        {
            get { return (_nbCharDescription); }
            set { _nbCharDescription = value; }
        }
        /// <summary>
        /// Tolerance
        /// </summary>
        public int Offset {
            get { return (_offset); }
            set { _offset = value; }
        }
        #endregion

    }
}
