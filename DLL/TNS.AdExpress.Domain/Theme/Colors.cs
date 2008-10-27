 using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Define a Pie Colors
    /// </summary>
    public class Colors : TNS.AdExpress.Domain.Theme.Tag {

        #region Variables
        /// <summary>
        /// Color list
        /// </summary>
        private List<Color> _colorList = new List<Color>();
        #endregion

        #region Assessor
        /// <summary>
        /// Get Color list
        /// </summary>
        public List<Color> ColorList {
            get { return _colorList; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pieColor"></param>
        public Colors(List<Color> pieColor) {
            if (pieColor == null) throw new ArgumentNullException("PieColor parameter is invalid ! (In constructor in class Pie)");
            _colorList = pieColor;
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
            throw new Exception("The method or operation is not implemented.");
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
            color = _colorList[0];
        }
        #endregion
    }
}
