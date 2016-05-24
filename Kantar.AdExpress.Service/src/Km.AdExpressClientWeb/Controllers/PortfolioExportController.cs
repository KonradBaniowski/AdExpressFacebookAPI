using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioExportController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;

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
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document);
            export.Export(document, data, session);

            document.Worksheets.ActiveSheetIndex = 1;

            string documentFileNameRoot;
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            ResultTable data = _portofolioService.GetResultTable(idWebSession);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document);
            export.Export(document, data, session, true);

            document.Worksheets.ActiveSheetIndex = 1;

            string documentFileNameRoot;
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();

            return View();
        }
    }

    public class ExportAspose : Controller
    {

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

        public ExportAspose()
        { }

        public void Export(Workbook document, ResultTable data, WebSession session, bool isExportBrut = false)
        {
            data.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            data.CultureInfo = WebApplicationParameters.AllowedLanguages[session.SiteLanguage].CultureInfo;


            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            //document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add("Resultat");

            document.ChangePalette(HeaderTabBackground, 3);
            document.ChangePalette(HeaderTabText, 2);
            document.ChangePalette(HeaderBorderTab, 1);

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
            bool columnHide = false;

            Color textColor;
            Color backColor;
            Color borderColor;


            int coltmp = columnStart;

            int nbRowTotal = NbRow(data.NewHeaders.Root) - 1;

            GenericDetailLevel detailLevel = session.GenericProductDetailLevel;
            int nbLevel = detailLevel.GetNbLevels;
            HeaderBase headerBase = data.NewHeaders.Root;

            if (isExportBrut)
            {
                if (nbLevel == 1)
                    headerBase = data.NewHeaders.Root;
                else
                {
                    headerBase = new Header();

                    for (int l = 1; l <= nbLevel; l++)
                    {
                        Header headerTmp = new Header(GestionWeb.GetWebWord(detailLevel[l].WebTextId, session.SiteLanguage));

                        headerBase.Add(headerTmp);
                    }

                    bool first = true;
                    foreach (var item in data.NewHeaders.Root)
                    {
                        if (!first)
                        {
                            HeaderBase header = item as HeaderBase;
                            headerBase.Add(header);
                        }

                        first = false;
                    }
                }
            }

            DrawHeaders(headerBase, sheet, rowStart, columnStart);

            rowStart += nbRowTotal;

            int colLevel = 0;

            

            #region colonne en mode brut
            if (isExportBrut && nbLevel > 1)
            {
                ICell cell = new CellLabel("");
                string[] tabLevel = new string[nbLevel];

                colLevel = nbLevel;

                for (int idxRow = 0, cellRow = rowStart; idxRow < data.LinesNumber; idxRow++, cellRow++)
                {
                    for (int idxCol = 0, cellCol = columnStart; idxCol < nbLevel; cellCol++, idxCol++)
                    {
                        switch (((LineStart)data[idxRow, 0]).LineType)
                        {
                            case LineType.total:

                                if (idxCol == 0)
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                else
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, session.SiteLanguage));

                                textColor = LTotalText;
                                backColor = LTotalBackground;
                                borderColor = BorderTab;
                                break;
                            case LineType.level1:

                                if (idxCol == 0)
                                { 
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                    tabLevel[0] = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                }
                                else
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, session.SiteLanguage));

                                textColor = L1Text;
                                backColor = L1Background;
                                borderColor = BorderTab;
                                break;
                            case LineType.level2:

                                if (idxCol == 1)
                                {
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                    tabLevel[1] = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                }
                                else if (idxCol < 1)
                                    sheet.Cells[cellRow, cellCol].Value = tabLevel[idxCol];
                                else
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, session.SiteLanguage));

                                textColor = L2Text;
                                backColor = L2Background;
                                borderColor = BorderTab;
                                //if (cell is CellLabel)
                                //    SetIndentLevel(sheet.Cells[cellRow, cellCol], 1);
                                break;
                            case LineType.level3:

                                if (idxCol == 2)
                                {
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                    tabLevel[2] = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                }
                                else if (idxCol < 2)
                                    sheet.Cells[cellRow, cellCol].Value = tabLevel[idxCol];
                                else
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, session.SiteLanguage));

                                textColor = L3Text;
                                backColor = L3Background;
                                borderColor = BorderTab;
                                //if (cell is CellLabel)
                                //    SetIndentLevel(sheet.Cells[cellRow, cellCol], 2);
                                break;
                            case LineType.level4:

                                if (idxCol == 3)
                                {
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                    tabLevel[3] = WebUtility.HtmlDecode(((CellLabel)data[idxRow, 1]).Label);
                                }
                                else if (idxCol < 3)
                                    sheet.Cells[cellRow, cellCol].Value = tabLevel[idxCol];
                                else
                                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, session.SiteLanguage));

                                textColor = L4Text;
                                backColor = L4Background;
                                borderColor = BorderTab;
                                //if (cell is CellLabel)
                                //    SetIndentLevel(sheet.Cells[cellRow, cellCol], 3);
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
                }

            }
            #endregion

            // Fige les entêtes de lignes et de colonnes
            sheet.FreezePanes(rowStart, columnStart, rowStart, columnStart);

            for (int idxCol = colLevel, cellCol = columnStart + colLevel; idxCol < data.ColumnsNumber; idxCol++)
            {
                columnHide = false;

                for (int idxRow = 0, cellRow = rowStart; idxRow < data.LinesNumber; idxRow++, cellRow++)
                {
                    ICell cell = data[idxRow, idxCol];

                    if (cell is LineStart || cell is LineStop) //|| cell is CellImageLink
                    {
                        columnHide = true;
                        break;
                    }

                    if (cell is CellImageLink && idxRow == 0)
                    {
                        columnHide = true;
                        break;
                    }

                    if (cell is CellPercent || cell is CellEvol)
                    {
                        double value = ((CellUnit)cell).Value;

                        if (double.IsInfinity(value) || double.IsNaN(value))
                            sheet.Cells[cellRow, cellCol].Value = "";
                        else
                            sheet.Cells[cellRow, cellCol].Value = value / 100;

                        SetPercentFormat(sheet.Cells[cellRow, cellCol]);
                        SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                    }
                    //else if (cell is CellEvol)
                    //{
                    //    sheet.Cells[cellRow, cellCol].Value = ((CellEvol)cell).Value;
                    //    SetPercentFormat(sheet.Cells[cellRow, cellCol]);
                    //    SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                    //}
                    else if (cell is CellDuration)
                    {
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
                        sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)cell).Label);
                    else if (cell is CellEmpty)
                        sheet.Cells[cellRow, cellCol].Value = "";
                    else
                    {
                        sheet.Cells[cellRow, cellCol].Value = "";
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

            //string documentFileNameRoot;
            //documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            //Response.Clear();
            //Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            //Response.ContentType = "application/octet-stream";

            //document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            //Response.End();
        }

        public void ExportSelection(Workbook document)
        {
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            //document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add("Selection");


            //TODO

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

        private int NbRow(HeaderBase root)
        {
            int nbRow = 1;
            int maxRow = 0;
            bool haveGroup = false;

            foreach (HeaderBase item in root)
            {
                if (item is HeaderGroup)
                {
                    int tmp = NbRow(item);

                    if (tmp > maxRow)
                        maxRow = tmp;

                    haveGroup = true;
                }
            }

            if (!haveGroup && root.Count > 0)
                nbRow++;

            return nbRow + maxRow;
        }

        private int NbColumn(HeaderBase root)
        {
            int nbCol = 0;
            int maxCol = 0;

            if (root is HeaderGroup)
            {
                foreach (HeaderBase item in root)
                {
                    if (item is HeaderGroup)
                    {
                        int tmp = NbColumn(item);

                        maxCol += tmp;
                    }
                    else
                    {
                        maxCol++;
                    }
                }
            }
            else
            {
                nbCol = 1;
            }

            return nbCol + maxCol;
        }

        private void DrawHeaders(HeaderBase head, Worksheet sheet, int rowStart, int colStart)
        {
            int nbRowTotal = NbRow(head) - 1;

            foreach (var item in head)
            {
                HeaderBase header = item as HeaderBase;

                if (header is HeaderMediaSchedule || header is HeaderCreative || header is HeaderInsertions)
                    continue;

                int ronSpan = nbRowTotal - (NbRow(header) - 1);
                int colSpan = NbColumn(header);

                if (colSpan > 1 || ronSpan > 1)
                {
                    Range range = sheet.Cells.CreateRange(rowStart, colStart, ronSpan, colSpan);
                    range.Merge();

                    sheet.Cells[rowStart, colStart].Value = WebUtility.HtmlDecode(header.Label);

                    TextStyle(sheet.Cells[rowStart, colStart], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
                }
                else
                {
                    sheet.Cells[rowStart, colStart].Value = WebUtility.HtmlDecode(header.Label);

                    TextStyle(sheet.Cells[rowStart, colStart], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, rowStart, colStart, CellBorderType.Thin, HeaderBorderTab);
                }

                if (header is HeaderGroup)
                    DrawHeaders(header, sheet, rowStart + 1, colStart);

                colStart += colSpan;
            }

        }

    }
}