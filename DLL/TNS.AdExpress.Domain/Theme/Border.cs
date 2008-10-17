using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public class Border {

        #region Enum
        public enum CellBorderType {
            None = 0,
            Thin = 1,
            Medium = 2,
            Dashed = 3,
            Dotted = 4,
            Thick = 5,
            Double = 6,
            Hair = 7,
            MediumDashed = 8,
            DashDot = 9,
            MediumDashDot = 10,
            DashDotDot = 11,
            MediumDashDotDot = 12,
            SlantedDashDot = 13,
        }
        #endregion

        #region Variables
        /// <summary>
        /// Border Color
        /// </summary>
        private Color _color = Color.Black;
        /// <summary>
        /// Border Type
        /// </summary>
        private CellBorderType _lineStyle = CellBorderType.None;
        
        #endregion

        #region Assessor
        /// <summary>
        /// Get / Set Border Color
        /// </summary>
        public Color Color {
            get { return _color; }
            set { _color = value; }
        }
        /// <summary>
        /// Get / Set Border Type
        /// </summary>
        public CellBorderType LineStyle {
            get { return _lineStyle; }
            set { _lineStyle = value; }
        }
        #endregion

    }
}
