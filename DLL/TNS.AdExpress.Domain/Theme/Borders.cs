using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Borders Object
    /// </summary>
    public class Borders {

        #region Enum
        public enum BorderType {
            LeftBorder = 1,
            RightBorder = 2,
            TopBorder = 4,
            BottomBorder = 8,
            DiagonalDown = 16,
            DiagonalUp = 32,
        }
        #endregion

        #region Variables
        /// <summary>
        /// Borders Object
        /// </summary>
        private Dictionary<BorderType, Border> _border = new Dictionary<BorderType,Border>();
        #endregion

        #region Assessor
        /// <summary>
        /// Get / Set Borders Object
        /// </summary>
        public Dictionary<BorderType, Border> Border {
            get { return _border; }
            set { _border = value; }
        }
        #endregion


    }
}
