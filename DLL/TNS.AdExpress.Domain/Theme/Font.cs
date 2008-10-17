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
        public override void SetStyle(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public override void SetStyle(Aspose.Cells.Worksheet sheet) {
            throw new Exception("The method or operation is not implemented.");
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
