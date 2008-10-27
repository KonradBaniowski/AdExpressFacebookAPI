using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Defined a line
    /// </summary>
    public class Line : Tag {

        #region Enum
        /// <summary>
        /// Enum Border Style
        /// </summary>
        public enum BordersStyle {
            NotSet = 0,
            Dash = 1,
            DashDot = 2,
            DashDotDot = 3,
            Dot = 4,
            Solid = 5,
        }
        #endregion

        #region Variables
        /// <summary>
        /// Size
        /// </summary>
        private double _size = 1;
        /// <summary>
        /// Color 
        /// </summary>
        private Color _color = Color.Black;
        /// <summary>
        /// Border Style
        /// </summary>
        private BordersStyle _borderStyle = BordersStyle.NotSet;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Color list
        /// </summary>
        public BordersStyle BorderStyle {
            get { return _borderStyle; }
        }
        /// <summary>
        /// Get Color list
        /// </summary>
        public double Size {
            get { return _size; }
        }
        /// <summary>
        /// Get Color 
        /// </summary>
        public Color Color {
            get { return _color; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size</param>
        /// <param name="color">Color</param>
        /// <param name="borderStyle">Border Style</param>
        public Line(double size, Color color, BordersStyle borderStyle) {
            _size = size;
            _color = color;
            _borderStyle = borderStyle;
        }
        #endregion

        #region Methods
        public override void SetStyleExcel(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleExcel(Aspose.Cells.Worksheet sheet) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleExcel(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStylePdf(PDFCreatorPilotLib.PDFDocument3Class pdfObject, PDFCreatorPilotLib.TxFontCharset charset) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleDundas(Dundas.Charting.WinControl.Legend legend) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleDundas(Dundas.Charting.WinControl.Title title) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleDundas(Dundas.Charting.WinControl.Label label) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetStyleDundas(Dundas.Charting.WinControl.Series series) {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Apllied a style Line To an Object chart
        /// </summary>
        /// <param name="chart">Chart</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Chart chart) {
            chart.BorderStyle = GetChartDashStyle(_borderStyle);
            chart.BorderLineColor = _color;
            chart.BorderLineWidth = Convert.ToInt32(_size);
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="axis">Title axis</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Axis axis) {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Apllied a style Color To an Object Color
        /// </summary>
        /// <param name="axis">Title axis</param>
        public override void SetStyleDundas(ref Color color) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Methods private
        /// <summary>
        /// Get Chart Dash Style
        /// </summary>
        /// <param name="borderStyle">Border Style</param>
        /// <returns>Chart Dash Style</returns>
        private Dundas.Charting.WinControl.ChartDashStyle GetChartDashStyle(BordersStyle borderStyle) {
            Dundas.Charting.WinControl.ChartDashStyle chartDashStyle;
            switch (borderStyle) {
                case BordersStyle.Dash:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.Dash;
                    break;
                case BordersStyle.DashDot:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.DashDot;
                    break;
                case BordersStyle.DashDotDot:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.DashDotDot;
                    break;
                case BordersStyle.Dot:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.Dot;
                    break;
                case BordersStyle.Solid:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.Solid;
                    break;
                default:
                    chartDashStyle = Dundas.Charting.WinControl.ChartDashStyle.NotSet;
                    break;
            }
            return chartDashStyle;
        }
        #endregion
    }
}
