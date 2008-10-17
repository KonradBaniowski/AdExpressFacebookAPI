using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public class Cell : Tag {

        #region Variables
        /// <summary>
        /// Foreground Color
        /// </summary>
        private Color _foregroundColor = Color.White;
        /// <summary>
        /// Border (type, color, etc...)
        /// </summary>
        private Borders _borders = new Borders();
        /// <summary>
        /// Font
        /// </summary>
        private Font _font=new Font();
        #endregion

        #region Assessor
        /// <summary>
        /// Get Foreground Color
        /// </summary>
        public Color ForegroundColor { get { return _foregroundColor; } }
        /// <summary>
        /// Get Border (type, color, etc...)
        /// </summary>
        public Borders Borders { get { return _borders; } }
        /// <summary>
        /// Get Font Name
        /// </summary>
        public Font Font { get { return _font; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="font">Font</param>
        /// <param name="foregroundColor">Foreground Color</param>
        /// <param name="borders">Border (type, color, etc...)</param>
        public Cell(Font font, Color foregroundColor, Borders borders):this(font,foregroundColor) {
            if (borders == null) throw new ArgumentNullException("borders parameter is null ! (in Constructor in class Font)");
            _borders = borders;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="font">Font</param>
        /// <param name="foregroundColor">Foreground Color</param>
        public Cell(Font font, Color foregroundColor)
            : this(font) {
            if (foregroundColor == null) throw new ArgumentNullException("Foreground Color parameter is null ! (in Constructor in class Font)");
            _foregroundColor = foregroundColor;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="font">Font</param>
        /// <param name="borders">Border (type, color, etc...)</param>
        public Cell(Font font, Borders borders)
            : this(font) {
            if (borders == null) throw new ArgumentNullException("borders parameter is null ! (in Constructor in class Font)");
            _borders = borders;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="font">Font</param>
        public Cell(Font font) {
            if (font == null) throw new ArgumentNullException("font parameter is null ! (in Constructor in class Font)");
           _font = font;
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="cells">Excel Object</param>
        public override void SetStyle(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
                cells[row, column].Style.Font.Name = this._font.Name;
                cells[row, column].Style.Font.Color = this._font.Color;
                cells[row, column].Style.Font.Size = Convert.ToInt32(this._font.Size);
                cells[row, column].Style.Font.IsItalic = this._font.IsItalic;
                cells[row, column].Style.Font.IsBold = this._font.IsBold;
                cells[row, column].Style.Font.IsStrikeout = this._font.IsStrikeout;
                cells[row, column].Style.ForegroundColor = _foregroundColor;
                SetBorderCell(cells[row, column].Style.Borders);
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

        /* /// <summary>
         /// Apllied a style to PDF Object
         /// </summary>
         /// <param name="pdfObject">PDF Object</param>
         public override void SetStyle(PDFCreatorPilotLib.PDFDocument3Class pdfObject) {
            throw new Exception("The method or operation is not implemented.");
         }*/
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="borders"></param>
        private void SetBorderCell(Aspose.Cells.Borders borders) {
            if (_borders.Border.ContainsKey(Borders.BorderType.TopBorder)) {
                borders[Aspose.Cells.BorderType.TopBorder].Color = _borders.Border[Borders.BorderType.TopBorder].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.TopBorder].LineStyle, _borders.Border[Borders.BorderType.TopBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.RightBorder)) {
                borders[Aspose.Cells.BorderType.RightBorder].Color = _borders.Border[Borders.BorderType.RightBorder].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.RightBorder].LineStyle, _borders.Border[Borders.BorderType.RightBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.BottomBorder)) {
                borders[Aspose.Cells.BorderType.BottomBorder].Color = _borders.Border[Borders.BorderType.BottomBorder].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.BottomBorder].LineStyle, _borders.Border[Borders.BorderType.BottomBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.LeftBorder)) {
                borders[Aspose.Cells.BorderType.LeftBorder].Color = _borders.Border[Borders.BorderType.LeftBorder].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.LeftBorder].LineStyle, _borders.Border[Borders.BorderType.LeftBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.DiagonalDown)) {
                borders[Aspose.Cells.BorderType.DiagonalDown].Color = _borders.Border[Borders.BorderType.DiagonalDown].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.DiagonalDown].LineStyle, _borders.Border[Borders.BorderType.DiagonalDown].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.DiagonalUp)) {
                borders[Aspose.Cells.BorderType.DiagonalUp].Color = _borders.Border[Borders.BorderType.DiagonalUp].Color;
                SetLineStyleCell(borders[Aspose.Cells.BorderType.DiagonalUp].LineStyle, _borders.Border[Borders.BorderType.DiagonalUp].LineStyle);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellBorderTypeExcel"></param>
        /// <param name="cellBorderType"></param>
        private void SetLineStyleCell(Aspose.Cells.CellBorderType cellBorderTypeExcel, Border.CellBorderType cellBorderType) {
            switch (cellBorderType) {
                case Border.CellBorderType.DashDot:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.DashDot;
                    break;
                case Border.CellBorderType.DashDotDot:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.DashDotDot;
                    break;
                case Border.CellBorderType.Dashed:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Dashed;
                    break;
                case Border.CellBorderType.Dotted:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Dotted;
                    break;
                case Border.CellBorderType.Double:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Double;
                    break;
                case Border.CellBorderType.Hair:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Hair;
                    break;
                case Border.CellBorderType.Medium:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Medium;
                    break;
                case Border.CellBorderType.MediumDashDot:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.MediumDashDot;
                    break;
                case Border.CellBorderType.MediumDashDotDot:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.MediumDashDotDot;
                    break;
                case Border.CellBorderType.MediumDashed:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.MediumDashed;
                    break;
                case Border.CellBorderType.SlantedDashDot:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.SlantedDashDot;
                    break;
                case Border.CellBorderType.Thick:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Thick;
                    break;
                case Border.CellBorderType.Thin:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.Thin;
                    break;
                default:
                    cellBorderTypeExcel = Aspose.Cells.CellBorderType.None;
                    break;
            }
        }
        #endregion

    }
}
