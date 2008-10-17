using System;
using System.Collections.Generic;
using System.Text;

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
        public override void SetStyle(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public override void SetStyle(Aspose.Cells.Worksheet sheet) {
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
        public override void SetStyle(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn) {
            throw new Exception("The method or operation is not implemented.");
        }
        /*
        /// <summary>
        /// Apllied a style to PDF Object
        /// </summary>
        /// <param name="pdfObject">PDF Object</param>
        public override void SetStyle(PDFCreatorPilotLib.PDFDocument3Class pdfObject) {
            throw new Exception("The method or operation is not implemented.");
        }*/
        #endregion
    }
}
