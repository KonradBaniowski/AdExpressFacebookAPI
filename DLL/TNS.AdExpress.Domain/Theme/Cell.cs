using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public class Cell : Tag {

        #region Enum
        /// <summary>
        /// Background Type
        /// </summary>
        public enum BackgroundType {
            None = 0,
            Solid = 1,
            Gray50 = 2,
            Gray75 = 3,
            Gray25 = 4,
            HorizontalStripe = 5,
            VerticalStripe = 6,
            ReverseDiagonalStripe = 7,
            DiagonalStripe = 8,
            DiagonalCrosshatch = 9,
            ThickDiagonalCrosshatch = 10,
            ThinHorizontalStripe = 11,
            ThinVerticalStripe = 12,
            ThinReverseDiagonalStripe = 13,
            ThinDiagonalStripe = 14,
            ThinHorizontalCrosshatch = 15,
            ThinDiagonalCrosshatch = 16,
            Gray12 = 17,
            Gray6 = 18,
        }
        #endregion

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
        /// <summary>
        /// Background Type
        /// </summary>
        private BackgroundType _backgroundType = BackgroundType.Solid;
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
        public override void SetStyleExcel(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
                cells[row, column].Style.Font.Name = this._font.Name;
                cells[row, column].Style.Font.Color = this._font.Color;
                cells[row, column].Style.Font.Size = Convert.ToInt32(this._font.Size);
                cells[row, column].Style.Font.IsItalic = this._font.IsItalic;
                cells[row, column].Style.Font.IsBold = this._font.IsBold;
                cells[row, column].Style.Font.IsStrikeout = this._font.IsStrikeout;
                cells[row, column].Style.ForegroundColor = _foregroundColor;
                cells[row, column].Style.Pattern = SetBackgroundType(cells[row, column].Style.Pattern,_backgroundType);
                SetBorderCell(cells[row, column].Style.Borders);
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
             throw new Exception("The method or operation is not implemented.");
         }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="borders"></param>
        private void SetBorderCell(Aspose.Cells.Borders borders) {
            if (_borders.Border.ContainsKey(Borders.BorderType.TopBorder)) {
                borders[Aspose.Cells.BorderType.TopBorder].Color = _borders.Border[Borders.BorderType.TopBorder].Color;
                borders[Aspose.Cells.BorderType.TopBorder].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.TopBorder].LineStyle, _borders.Border[Borders.BorderType.TopBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.RightBorder)) {
                borders[Aspose.Cells.BorderType.RightBorder].Color = _borders.Border[Borders.BorderType.RightBorder].Color;
                borders[Aspose.Cells.BorderType.RightBorder].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.RightBorder].LineStyle, _borders.Border[Borders.BorderType.RightBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.BottomBorder)) {
                borders[Aspose.Cells.BorderType.BottomBorder].Color = _borders.Border[Borders.BorderType.BottomBorder].Color;
                borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.BottomBorder].LineStyle, _borders.Border[Borders.BorderType.BottomBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.LeftBorder)) {
                borders[Aspose.Cells.BorderType.LeftBorder].Color = _borders.Border[Borders.BorderType.LeftBorder].Color;
                borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.LeftBorder].LineStyle, _borders.Border[Borders.BorderType.LeftBorder].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.DiagonalDown)) {
                borders[Aspose.Cells.BorderType.DiagonalDown].Color = _borders.Border[Borders.BorderType.DiagonalDown].Color;
                borders[Aspose.Cells.BorderType.DiagonalDown].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.DiagonalDown].LineStyle, _borders.Border[Borders.BorderType.DiagonalDown].LineStyle);
            }
            if (_borders.Border.ContainsKey(Borders.BorderType.DiagonalUp)) {
                borders[Aspose.Cells.BorderType.DiagonalUp].Color = _borders.Border[Borders.BorderType.DiagonalUp].Color;
                borders[Aspose.Cells.BorderType.DiagonalUp].LineStyle = SetLineStyleCell(borders[Aspose.Cells.BorderType.DiagonalUp].LineStyle, _borders.Border[Borders.BorderType.DiagonalUp].LineStyle);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellBorderTypeExcel"></param>
        /// <param name="cellBorderType"></param>
        private Aspose.Cells.CellBorderType SetLineStyleCell(Aspose.Cells.CellBorderType cellBorderTypeExcel, Border.CellBorderType cellBorderType) {
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
            return cellBorderTypeExcel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundType"></param>
        /// <returns></returns>
        private Aspose.Cells.BackgroundType SetBackgroundType(Aspose.Cells.BackgroundType backgroundTypeExcel, BackgroundType backgroundType) {
            switch (backgroundType) {
                case BackgroundType.DiagonalCrosshatch:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.DiagonalCrosshatch;
                    break;
                case BackgroundType.DiagonalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.DiagonalStripe;
                    break;
                case BackgroundType.Gray12:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.Gray12;
                    break;
                case BackgroundType.Gray25:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.Gray25;
                    break;
                case BackgroundType.Gray50:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.Gray50;
                    break;
                case BackgroundType.Gray75:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.Gray75;
                    break;
                case BackgroundType.HorizontalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.HorizontalStripe;
                    break;
                case BackgroundType.None:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.None;
                    break;
                case BackgroundType.ReverseDiagonalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ReverseDiagonalStripe;
                    break;
                case BackgroundType.ThickDiagonalCrosshatch:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThickDiagonalCrosshatch;
                    break;
                case BackgroundType.ThinDiagonalCrosshatch:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinDiagonalCrosshatch;
                    break;
                case BackgroundType.ThinDiagonalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinDiagonalStripe;
                    break;
                case BackgroundType.ThinHorizontalCrosshatch:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinHorizontalCrosshatch;
                    break;
                case BackgroundType.ThinHorizontalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinHorizontalStripe;
                    break;
                case BackgroundType.ThinReverseDiagonalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinReverseDiagonalStripe;
                    break;
                case BackgroundType.ThinVerticalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.ThinVerticalStripe;
                    break;
                case BackgroundType.VerticalStripe:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.VerticalStripe;
                    break;
                default:
                    backgroundTypeExcel = Aspose.Cells.BackgroundType.Solid;
                    break;
            }
            return backgroundTypeExcel;
        }
        #endregion

    }
}
