using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public class Margin {

        #region Variables
        /// <summary>
        /// Margin Top
        /// </summary>
        private double _marginTop;
        /// <summary>
        /// Margin Right
        /// </summary>
        private double _marginRight;
        /// <summary>
        /// Margin Bottom
        /// </summary>
        private double _marginBottom;
        /// <summary>
        /// Margin Left
        /// </summary>
        private double _marginLeft;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Margin Top
        /// </summary>
        public double MarginTop {
            get { return _marginTop; }
        }
        /// <summary>
        /// Get Margin Right
        /// </summary>
        public double MarginRight {
            get { return _marginRight; }
        }
        /// <summary>
        /// Get Margin Bottom
        /// </summary>
        public double MarginBottom {
            get { return _marginBottom; }
        }
        /// <summary>
        /// Get Margin Left
        /// </summary>
        public double MarginLeft {
            get { return _marginLeft; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="marginTop">Margin Top</param>
        /// <param name="marginRight">Margin Right</param>
        /// <param name="marginBottom">Margin Bottom</param>
        /// <param name="marginLeft">Margin Left</param>
        public Margin(double marginTop,double marginRight,double marginBottom,double marginLeft) {
            _marginTop = marginTop;
            _marginRight = marginRight;
            _marginBottom = marginBottom;
            _marginLeft = marginLeft;
        }
        #endregion
    }
}
