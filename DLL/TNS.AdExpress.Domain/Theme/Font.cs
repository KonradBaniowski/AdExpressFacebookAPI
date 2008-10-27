using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public class Font : Tag {

        #region Property
        /// <summary>
        /// Police name
        /// </summary>
        private string _name = "Arial";
        /// <summary>
        /// Color
        /// </summary>
        private Color _color = Color.Black;
        /// <summary>
        /// Size
        /// </summary>
        private double _size = 8;
        /// <summary>
        /// Is italic
        /// </summary>
        private bool _isItalic = false;
        /// <summary>
        /// Is Underline
        /// </summary>
        private bool _isUnderline = false;
        /// <summary>
        /// Is Bold
        /// </summary>
        private bool _isBold = false;
        /// <summary>
        /// Is Strikeout
        /// </summary>
        private bool _isStrikeout = false;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Police name
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// Get Color
        /// </summary>
        public Color Color { get { return _color; } }
        /// <summary>
        /// get Size
        /// </summary>
        public double Size { get { return _size; } }
        /// <summary>
        /// Get Is italic
        /// </summary>
        public bool IsItalic { get { return _isItalic; } }
        /// <summary>
        /// get Is Underline
        /// </summary>
        public bool IsUnderline { get { return _isUnderline; } }
        /// <summary>
        /// Get Is Bold
        /// </summary>
        public bool IsBold { get { return _isBold; } }
        /// <summary>
        /// Get Is Strikeout
        /// </summary>
        public bool IsStrikeout { get { return _isStrikeout; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of Police</param>
        /// <param name="color">Color</param>
        /// <param name="size">Size</param>
        /// <param name="isBold">Is Bold</param>
        /// <param name="isItalic">Is Italic</param>
        /// <param name="isUnderline">Is Underline</param>
        /// <param name="isStrikeout">Is Strikeout</param>
        public Font(string name, Color color, double size, bool isBold, bool isItalic, bool isUnderline, bool isStrikeout)
            : this(name, color, size, isBold, isItalic, isUnderline) {
            _isStrikeout = IsStrikeout;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of Police</param>
        /// <param name="color">Color</param>
        /// <param name="size">Size</param>
        /// <param name="isBold">Is Bold</param>
        /// <param name="isItalic">Is Italic</param>
        /// <param name="isUnderline">Is Underline</param>
        public Font(string name, Color color, double size, bool isBold, bool isItalic, bool isUnderline)
            : this(name, color, size, isBold, isItalic) {
            _isUnderline = isUnderline;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of Police</param>
        /// <param name="color">Color</param>
        /// <param name="size">Size</param>
        /// <param name="isBold">Is Bold</param>
        /// <param name="isItalic">Is Italic</param>
        public Font(string name, Color color, double size, bool isBold, bool isItalic)
            : this(name, color, size, isBold) {
            _isItalic = isItalic;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of Police</param>
        /// <param name="color">Color</param>
        /// <param name="size">Size</param>
        /// <param name="isBold">Is Bold</param>
        public Font(string name, Color color, double size, bool isBold)
            : this(name, color, size) {
            _isBold = isBold;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of Police</param>
        /// <param name="color">Color</param>
        /// <param name="size">Size</param>
        public Font(string name, Color color, double size) {
            _name = name;
            _color = color;
            _size = size;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Font() {
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="excel">Excel Object</param>
        /// <param name="cells"></param>
        /// <param name="row"></param>
        /// <param name="firstColumn"></param>
        /// <param name="lastColumn"></param>
        public override void SetStyleExcel(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            cells[row, column].Style.Font.Name = this._name;
            cells[row, column].Style.Font.Color = this._color;
            cells[row, column].Style.Font.Size = Convert.ToInt32(this._size);
            cells[row, column].Style.Font.IsItalic = this._isItalic;
            cells[row, column].Style.Font.IsBold = this._isBold;
            cells[row, column].Style.Font.IsStrikeout = this._isStrikeout;
        }

        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public override void SetStyleExcel(Aspose.Cells.Worksheet sheet) {
            throw new Exception("The method or operation is not implemented.");
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
        public override void SetStylePdf(PDFCreatorPilotLib.PDFDocument3Class pdfObject,PDFCreatorPilotLib.TxFontCharset charset) {
            pdfObject.PDFPAGE_SetActiveFont(_name, _isBold, _isItalic, _isUnderline, _isStrikeout, _size, charset);
            pdfObject.PDFPAGE_SetRGBColor(((double)_color.R) / 256.0
                        , ((double)_color.G) / 256.0
                        , ((double)_color.B) / 256.0);
        }

        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="font">Font To Init</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Legend legend) {
            legend.Font = new System.Drawing.Font(_name, (float) _size,
                        ((_isBold) ? FontStyle.Bold : 0)
                        | ((_isUnderline) ? FontStyle.Underline : 0)
                        | ((_isItalic) ? FontStyle.Italic : 0)
                        | ((_isStrikeout) ? FontStyle.Italic : 0)
                        );
            legend.FontColor = _color;
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="title">title</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Title title) {
            title.Font = new System.Drawing.Font(_name, (float)_size,
                        ((_isBold) ? FontStyle.Bold : 0)
                        | ((_isUnderline) ? FontStyle.Underline : 0)
                        | ((_isItalic) ? FontStyle.Italic : 0)
                        | ((_isStrikeout) ? FontStyle.Italic : 0)
                        );
            title.Color = _color;
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="label">label</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Label label) {
            label.Font = new System.Drawing.Font(_name, (float)_size,
                        ((_isBold) ? FontStyle.Bold : 0)
                        | ((_isUnderline) ? FontStyle.Underline : 0)
                        | ((_isItalic) ? FontStyle.Italic : 0)
                        | ((_isStrikeout) ? FontStyle.Italic : 0)
                        );
            label.FontColor = _color;
        }
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="series">series</param>
        public override void SetStyleDundas(Dundas.Charting.WinControl.Series series) {
            series.Font = new System.Drawing.Font(_name, (float)_size,
                        ((_isBold) ? FontStyle.Bold : 0)
                        | ((_isUnderline) ? FontStyle.Underline : 0)
                        | ((_isItalic) ? FontStyle.Italic : 0)
                        | ((_isStrikeout) ? FontStyle.Italic : 0)
                        );
            series.FontColor = _color;
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
            axis.TitleFont = new System.Drawing.Font(_name, (float)_size,
                        ((_isBold) ? FontStyle.Bold : 0)
                        | ((_isUnderline) ? FontStyle.Underline : 0)
                        | ((_isItalic) ? FontStyle.Italic : 0)
                        | ((_isStrikeout) ? FontStyle.Italic : 0)
                        );
            axis.TitleColor = _color;
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
