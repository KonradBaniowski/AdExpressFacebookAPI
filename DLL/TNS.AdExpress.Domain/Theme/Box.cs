using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Define object Box (for create an object Header or Footer)
    /// </summary>
    public class Box : Tag {

        #region Variables
        /// <summary>
        /// Margin of Box
        /// </summary>
        private Margin _margin;
        /// <summary>
        /// Height of Box
        /// </summary>
        private double _height = -1;
        /// <summary>
        /// Width of Box
        /// </summary>
        private double _width = -1;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Margin of Box
        /// </summary>
        public Margin Margin { get { return _margin; } }
        /// <summary>
        /// Get Height of Box
        /// </summary>
        public double Height { get { return _height; } }
        /// <summary>
        /// Get Width of Box
        /// </summary>
        public double Width { get { return _width; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="margin">Margin of Box</param>
        /// <param name="height">Height of Box</param>
        /// <param name="width">Width of Box</param>
        public Box(Margin margin, double height,double width):this(margin) {
            _height = height;
            _width = width;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="margin">Margin of Box</param>
        public Box(Margin margin) {
            if (margin == null) throw new ArgumentNullException("Margin parameter is null");
            _margin = margin;
        }
        #endregion

        #region Methods
        public override void SetStyleExcel(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public override void SetStyleExcel(Aspose.Cells.Worksheet sheet) {
            sheet.PageSetup.TopMarginInch = this.Margin.MarginTop;
            sheet.PageSetup.BottomMarginInch = this.Margin.MarginBottom;
            sheet.PageSetup.RightMargin = this.Margin.MarginRight;
            sheet.PageSetup.LeftMargin = this.Margin.MarginLeft;
        }

        /// <summary>
        /// Set a Style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        /// <param name="upperLeftRow">Upper Left Row</param>
        /// <param name="upperLeftColumn">Upper Left Column</param>
        public override void SetStyleExcel(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn) {
            throw new Exception("The method or operation is not implemented.");
        }
        
        /// <summary>
        /// Apllied a style to PDF Object
        /// </summary>
        /// <param name="pdfObject">PDF Object</param>
        public override void SetStylePdf(PDFCreatorPilotLib.PDFDocument3Class pdfObject, PDFCreatorPilotLib.TxFontCharset charset) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="font">Font To Init</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Legend legend) {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="title">title</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Title title) {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="label">label</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Label label) {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="series">series</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Series series) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style Line To an Object chart
        /// </summary>
        /// <param name="chart">Chart</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Chart chart) {
            chart.Size = new System.Drawing.Size(Convert.ToInt32(_width), Convert.ToInt32(_height));
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
    }
}