using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Result;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioExportController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;

        #region Couleurs

        Color HeaderTabBackground = Color.FromArgb(105, 112, 129);
        Color HeaderTabText = Color.White;
        Color HeaderBorderTab = Color.White;

        Color L1Background = Color.FromArgb(64, 68, 79);
        Color L1Text = Color.FromArgb(0, 193, 255);

        Color L2Background = Color.FromArgb(120, 120, 120);
        Color L2Text = Color.White;

        Color L3Background = Color.FromArgb(105, 112, 129);
        Color L3Text = Color.White;

        Color L4Background = Color.FromArgb(231, 231, 231);
        Color L4Text = Color.Black;

        Color LTotalBackground = Color.FromArgb(0, 193, 255);
        Color LTotalText = Color.White;

        Color TabBackground = Color.FromArgb(64, 68, 79);
        Color TabText = Color.White;
        Color BorderTab = Color.Black;

        Color PresentText = Color.Black;
        Color PresentBackground = Color.FromArgb(255, 150, 23);

        Color NotPresentText = Color.FromArgb(64, 68, 79);
        Color NotPresentBackground = Color.FromArgb(64, 68, 79);

        Color ExtendedText = Color.Black;
        Color ExtendedBackground = Color.FromArgb(243, 209, 97);
        #endregion

        public PortfolioExportController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService)
        {
            _portofolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;

        }

        // GET: PortfolioExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        private void export(ResultTable data)
        {
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add("WorkSheet1");

            document.ChangePalette(HeaderTabBackground, 25);
            document.ChangePalette(HeaderTabText, 24);
            document.ChangePalette(HeaderBorderTab, 23);

            document.ChangePalette(L1Background, 22);
            document.ChangePalette(L1Text, 21);

            document.ChangePalette(L2Background, 20);
            document.ChangePalette(L2Text, 19);

            document.ChangePalette(L3Background, 18);
            document.ChangePalette(L3Text, 17);

            document.ChangePalette(L4Background, 16);
            document.ChangePalette(L4Text, 15);

            document.ChangePalette(LTotalBackground, 14);
            document.ChangePalette(LTotalText, 13);

            document.ChangePalette(TabBackground, 12);
            document.ChangePalette(TabText, 11);
            document.ChangePalette(BorderTab, 10);

            document.ChangePalette(PresentText, 9);
            document.ChangePalette(PresentBackground, 8);

            document.ChangePalette(NotPresentText, 7);
            document.ChangePalette(NotPresentBackground, 6);

            document.ChangePalette(ExtendedText, 5);
            document.ChangePalette(ExtendedBackground, 4);


            int rowStart = 1;
            int columnStart = 1;
            //int cellRow = 0;
            //int cellCol = 0;
            bool columnHide = false;

            Color textColor;
            Color backColor;
            Color borderColor;


            int coltmp = columnStart;
            foreach (var item in data.HeadersIndexInResultTable)
            {
                HeaderBase header = item.Value;

                if (header is HeaderMediaSchedule || header is HeaderCreative)
                    continue;

                if (header.ColSpan > 1)
                {
                    Range range = sheet.Cells.CreateRange(rowStart, coltmp, 1, header.ColSpan);
                    range.Merge();

                    //sheet.Cells.Merge(rowStart, coltmp, 1, header.ColSpan);

                    sheet.Cells[rowStart, coltmp].Value = header.Label;

                    TextStyle(sheet.Cells[rowStart, coltmp], HeaderTabText, HeaderTabBackground);
                    //BorderStyle(sheet, rowStart, coltmp, CellBorderType.Thin, HeaderBorderTab);

                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
                }
                else
                {
                    sheet.Cells[rowStart, coltmp].Value = header.Label;

                    TextStyle(sheet.Cells[rowStart, coltmp], HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, rowStart, coltmp, CellBorderType.Thin, HeaderBorderTab);
                }
                coltmp++;
            }

            rowStart++;

            for (int idxCol = 0, cellCol = columnStart; idxCol < data.ColumnsNumber; idxCol++)
            {
                columnHide = false;

                for (int idxRow = 0, cellRow = rowStart; idxRow < data.LinesNumber; idxRow++, cellRow++)
                {

                    var cell = data[idxRow, idxCol];



                    if (cell is LineStart || cell is LineStop || cell is CellImageLink)
                    {
                        // if (((LineStart)cell).LineType == LineType.)

                        columnHide = true;
                        break;
                    }

                    if (cell is CellPercent)
                    {
                        sheet.Cells[cellRow, cellCol].Value = ((CellPercent)cell).Value / 100;
                        SetPercentFormat(sheet.Cells[cellRow, cellCol]);
                        SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                    }
                    else if (cell is CellDuration)
                    {
                        //DateTime dt = new DateTime();

                        //double value = ((CellUnit)cell).GetValue();

                        //dt = dt.AddSeconds(value);

                        //sheet.Cells[cellRow, cellCol].Value = dt.ToLongTimeString();

                        //SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);

                        double value = ((CellUnit)cell).GetValue();

                        double hours = Math.Floor(value / 3600);
                        double minutes = Math.Floor((value - (hours * 3600)) / 60);
                        double secondes = value - hours * 3600 - minutes * 60;

                        sheet.Cells[cellRow, cellCol].Value = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + secondes.ToString("00");

                        SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                    }
                    else if (cell is CellDate)
                    {

                        sheet.Cells[cellRow, cellCol].Value = ((CellDate)cell).Date.ToShortDateString();
                                                
                        SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, false);
                    }
                    else if (cell is CellUnit)
                    {
                        double value = ((CellUnit)cell).GetValue();
                        if (value != 0.0)
                        {
                            sheet.Cells[cellRow, cellCol].Value = value;

                            if (((CellUnit)cell).AsposeFormat == -1)
                                SetDecimalFormat(sheet.Cells[cellRow, cellCol]);
                            else
                                SetAsposeFormat(sheet.Cells[cellRow, cellCol], ((CellUnit)cell).AsposeFormat);

                            SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                        }
                    }
                    else if (cell is CellLabel)
                        sheet.Cells[cellRow, cellCol].Value = ((CellLabel)cell).Label;
                    else
                    {
                        int i = 0;

                    }


                    switch (((LineStart)data[idxRow, 0]).LineType)
                    {
                        case LineType.total:
                            textColor = LTotalText;
                            backColor = LTotalBackground;
                            borderColor = BorderTab;
                            break;
                        case LineType.level1:
                            textColor = L1Text;
                            backColor = L1Background;
                            borderColor = BorderTab;
                            break;
                        case LineType.level2:
                            textColor = L2Text;
                            backColor = L2Background;
                            borderColor = BorderTab;
                            if (cell is CellLabel)
                                SetIndentLevel(sheet.Cells[cellRow, cellCol], 1);
                            break;
                        case LineType.level3:
                            textColor = L3Text;
                            backColor = L3Background;
                            borderColor = BorderTab;
                            if (cell is CellLabel)
                                SetIndentLevel(sheet.Cells[cellRow, cellCol], 2);
                            break;
                        case LineType.level4:
                            textColor = L4Text;
                            backColor = L4Background;
                            borderColor = BorderTab;
                            if (cell is CellLabel)
                                SetIndentLevel(sheet.Cells[cellRow, cellCol], 3);
                            break;
                        default:
                            textColor = Color.Black;
                            backColor = Color.White;
                            borderColor = Color.Black;
                            break;
                    }



                    TextStyle(sheet.Cells[cellRow, cellCol], textColor, backColor);
                    BorderStyle(sheet, cellRow, cellCol, CellBorderType.Thin, borderColor);

                }

                if (columnHide == false)
                    cellCol++;
            }

            #region Ajustement de la taile des cellules en fonction du contenu 

            sheet.AutoFitColumns();

            #endregion

            string documentFileNameRoot;
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();
        }

        private void BorderStyle(Worksheet sheet, int idxRow, int idxCol, CellBorderType borderLineStyle, Color color)
        {
            Style style = sheet.Cells[idxRow, idxCol].GetStyle();

            style.Borders[BorderType.LeftBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.TopBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.RightBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.BottomBorder].LineStyle = borderLineStyle;

            style.Borders[BorderType.LeftBorder].Color = color;
            style.Borders[BorderType.TopBorder].Color = color;
            style.Borders[BorderType.RightBorder].Color = color;
            style.Borders[BorderType.BottomBorder].Color = color;

            sheet.Cells[idxRow, idxCol].SetStyle(style);
        }

        private void BorderStyle(Worksheet sheet, Range range, CellBorderType borderLineStyle, Color color)
        {
            //Range range = worksheet.getCells().createRange("A1:F10");

            Style style;
            style = sheet.Workbook.CreateStyle();
            //Specify the font attribute.
            //style.Font.Name = "Calibri";
            //Specify the shading color.
            //style.ForegroundColor = Color.Yellow;
            //style.Pattern = BackgroundType.Solid;
            //Specify the border attributes.
            style.Borders[BorderType.TopBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.TopBorder].Color = color;
            style.Borders[BorderType.BottomBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.BottomBorder].Color = color;
            style.Borders[BorderType.LeftBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.LeftBorder].Color = color;
            style.Borders[BorderType.RightBorder].LineStyle = borderLineStyle;
            style.Borders[BorderType.RightBorder].Color = color;
            //Create the styleflag object.
            StyleFlag flag1 = new StyleFlag();
            //Implement font attribute
            flag1.FontName = false;
            //Implement the shading / fill color.
            flag1.CellShading = false;
            //Implment border attributes.
            flag1.Borders = true;
            //Set the Range style.

            range.ApplyStyle(style, flag1);
        }

        private void TextStyle(Aspose.Cells.Cell cell, TextAlignmentType horizontalAlignment, TextAlignmentType verticalAlignment, Color fontColor, Color foregroundColor)
        {
            TextStyle(cell, fontColor, foregroundColor);
            TextStyle(cell, horizontalAlignment, verticalAlignment);
        }
        private void TextStyle(Aspose.Cells.Cell cell, TextAlignmentType horizontalAlignment, TextAlignmentType verticalAlignment)
        {
            Style style = cell.GetStyle();

            style.HorizontalAlignment = horizontalAlignment;
            style.VerticalAlignment = verticalAlignment;

            cell.SetStyle(style);
        }

        private void TextStyle(Aspose.Cells.Cell cell, Color fontColor, Color foregroundColor)
        {
            Style style = cell.GetStyle();

            style.ForegroundColor = foregroundColor;
            //style.BackgroundColor = backgroundColor;
            style.Pattern = BackgroundType.Solid;

            style.Font.Color = fontColor;

            cell.SetStyle(style);
        }

        #region AsposeFormat
        //Value Type    Format String
        //0	General General
        //1	Decimal	0
        //2	Decimal	0.00
        //3	Decimal	#,##0
        //4	Decimal	#,##0.00
        //5	Currency	$#,##0;$-#,##0
        //6	Currency	$#,##0;[Red]$-#,##0
        //7	Currency	$#,##0.00;$-#,##0.00
        //8	Currency	$#,##0.00;[Red]$-#,##0.00
        //9	Percentage	0%
        //10	Percentage	0.00%
        //11	Scientific	0.00E+00
        //12	Fraction	# ?/?
        //13	Fraction	# /
        //14	Date m/d/yy
        //15	Date d-mmm-yy
        //16	Date d-mmm
        //17	Date mmm-yy
        //18	Time h:mm AM/PM
        //19	Time h:mm:ss AM/PM
        //20	Time h:mm
        //21	Time h:mm:ss
        //22	Time m/d/yy h:mm
        //37	Currency	#,##0;-#,##0
        //38	Currency	#,##0;[Red]-#,##0
        //39	Currency	#,##0.00;-#,##0.00
        //40	Currency	#,##0.00;[Red]-#,##0.00
        //41	Accounting _ * #,##0_ ;_ * "_ ;_ @_
        //42	Accounting _ $* #,##0_ ;_ $* "_ ;_ @_
        //43	Accounting _ * #,##0.00_ ;_ * "??_ ;_ @_
        //44	Accounting _ $* #,##0.00_ ;_ $* "??_ ;_ @_
        //45	Time mm:ss
        //46	Time h :mm:ss
        //47	Time mm:ss.0
        //48	Scientific	##0.0E+00
        //49	Text	@
        #endregion

        private void SetAsposeFormat(Aspose.Cells.Cell cell, int asposeFormat)
        {
            Style style = cell.GetStyle();

            style.Number = asposeFormat;

            cell.SetStyle(style);
        }

        private void SetDecimalFormat(Aspose.Cells.Cell cell)
        {
            Style style = cell.GetStyle();

            style.Number = 3;

            cell.SetStyle(style);
        }

        private void SetPercentFormat(Aspose.Cells.Cell cell)
        {
            Style style = cell.GetStyle();

            style.Number = 10;

            cell.SetStyle(style);
        }

        private void SetIndentLevel(Aspose.Cells.Cell cell, int indentLevel, bool isRight = false)
        {
            Style style = cell.GetStyle();

            if (isRight == true)
                style.HorizontalAlignment = TextAlignmentType.Right;

            style.IndentLevel = indentLevel;

            cell.SetStyle(style);
        }
    }
}