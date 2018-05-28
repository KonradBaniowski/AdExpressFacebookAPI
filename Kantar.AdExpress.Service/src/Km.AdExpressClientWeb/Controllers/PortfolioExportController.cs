using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using Km.AdExpressClientWeb.I18n;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Aspose.Cells.Drawing;
using Aspose.Cells.Rendering;
using Infragistics.Imaging;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.Cells;
using TNS.FrameWork.WebResultUI;
using LineType = TNS.FrameWork.WebResultUI.LineType;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = WebConstantes.Role.ADEXPRESS)]
    public class PortfolioExportController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;

        public PortfolioExportController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _portofolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: PortfolioExport
        public ActionResult Index(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession, this.HttpContext);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, false, sortOrder, columnIndex);

            document.Worksheets.ActiveSheetIndex = 1;

            string documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();

            return View();
        }

        public ActionResult ResultBrut(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            ResultTable data = _portofolioService.GetResultTable(idWebSession, this.HttpContext);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, true, sortOrder, columnIndex);

            document.Worksheets.ActiveSheetIndex = 1;

            string documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

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
        Color HeaderTabBackground = Color.FromArgb(128, 128, 128);
        Color HeaderTabText = Color.Black;
        Color HeaderBorderTab = Color.Black;

        Color L1Background = Color.FromArgb(166, 166, 166);
        Color L1Text = Color.Black;

        Color L2Background = Color.FromArgb(191, 191, 191);
        Color L2Text = Color.Black;

        Color L3Background = Color.FromArgb(217, 217, 217);
        Color L3Text = Color.Black;

        Color L4Background = Color.White;
        Color L4Text = Color.Black;

        Color LTotalBackground = Color.FromArgb(128, 128, 128);
        Color LTotalText = Color.Black;

        Color TabBackground = Color.Black;
        Color TabText = Color.Black;
        Color BorderTab = Color.Black;

        Color PresentText = Color.Black;
        Color PresentBackground = Color.FromArgb(255, 150, 23);

        Color NotPresentText = Color.Black;
        Color NotPresentBackground = Color.White;

        Color ExtendedText = Color.Black;
        Color ExtendedBackground = Color.FromArgb(243, 209, 97);
        #endregion

        private bool _skipIndent = false;
        private AdExpressCultureInfo cInfo;

        public ExportAspose()
        { }


        private void SetSetsOfColorByMaxLevel(int maxLevel)
        {
            switch (maxLevel)
            {
                case (1):
                    this.L1Background = this.L4Background;
                    break;
                case (2):
                    this.L1Background = this.L3Background;
                    this.L2Background = this.L4Background;
                    break;
                case (3):
                    this.L1Background = this.L2Background;
                    this.L2Background = this.L3Background;
                    this.L3Background = this.L4Background;
                    break;
            }
        }


        public void Export(Workbook document, ResultTable data, WebSession session, bool isExportBrut = false, ResultTable.SortOrder sortOrder = ResultTable.SortOrder.NONE, int columnIndex = 1, bool isInsertionExport = false)
        {
            this.cInfo = WebApplicationParameters.AllowedLanguages[session.SiteLanguage].CultureInfo;
            
            data.Sort(sortOrder, columnIndex); //Important, pour hierarchie du tableau Infragistics
            data.CultureInfo = WebApplicationParameters.AllowedLanguages[session.SiteLanguage].CultureInfo;

            int rowStart = 1;
            int columnStart = 1;
            bool columnHide = false;
            GenericDetailLevel detailLevel = null;

            Color textColor;
            Color backColor;
            Color borderColor;

            int nbRowTotal = 0;
            int nbLevel = 0;

            _skipIndent = isInsertionExport;

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            //document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1983, session.SiteLanguage));
            sheet.IsGridlinesVisible = false;

            if (session.CurrentModule != WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)
            {
                detailLevel = session.GenericProductDetailLevel;
                nbLevel = detailLevel.GetNbLevels;
                SetSetsOfColorByMaxLevel(nbLevel);
            }

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

            if (data.NewHeaders != null)
            {
                nbRowTotal = NbRow(data.NewHeaders.Root, session) - 1;

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
                            if (detailLevel != null)
                            {
                                Header headerTmp = new Header(GestionWeb.GetWebWord(detailLevel[l].WebTextId, session.SiteLanguage));
                            
                                headerBase.Add(headerTmp);
                            }
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

                DrawHeaders(headerBase, sheet, session, rowStart, columnStart);
            }

            rowStart += nbRowTotal;

            int colLevel = 0;
            int rowStartValue = rowStart;
            int rowEndValue = 0;
            HashSet<int> lstColEvol = new HashSet<int>();


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
                            case LineType.nbParution:
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
                        BorderStyle(sheet, cellRow, cellCol, CellBorderType.Hair, borderColor);
                    }
                }

            }
            #endregion

            // Fige les entêtes de lignes et de colonnes
            sheet.FreezePanes(rowStart, columnStart, rowStart, columnStart);

            int startIndex = colLevel;

            if (isExportBrut)
                startIndex = nbLevel > 1 ? 2 : 1;

            for (int idxCol = startIndex, cellCol = columnStart + colLevel; idxCol < data.ColumnsNumber; idxCol++)
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

                    MEFCell(session, cell, sheet, ref cellRow, cellCol, ((LineStart)data[idxRow, 0]).LineType, lstColEvol);

                    if (rowEndValue < cellRow)
                        rowEndValue = cellRow;
                }

                if (columnHide == false)
                    cellCol++;
            }

            #region Ajustement de la taile des cellules en fonction du contenu 

            sheet.AutoFitColumns();

            #endregion

            #region Ajoute les icones des cellules
            if (lstColEvol.Count() > 0)
            {
                int idxCondis = sheet.ConditionalFormattings.Add();
                FormatConditionCollection fcs = sheet.ConditionalFormattings[idxCondis];

                foreach (int col in lstColEvol)
                {
                    CellArea cellArea = new CellArea();
                    cellArea.StartRow = rowStartValue;
                    cellArea.EndRow = rowEndValue;
                    cellArea.StartColumn = col;
                    cellArea.EndColumn = col;

                    fcs.AddArea(cellArea);
                }

                // Adds condition.
                int conditionIndex = fcs.AddCondition(FormatConditionType.IconSet, OperatorType.None, "0", "0");
                fcs[conditionIndex].IconSet.Type = IconSetType.Arrows3;

                //fcs[conditionIndex].IconSet.Cfvos[0].Type = FormatConditionValueType.Number;
                //fcs[conditionIndex].IconSet.Cfvos[0].Value = 0;                    
                fcs[conditionIndex].IconSet.Cfvos[1].Type = FormatConditionValueType.Number;
                fcs[conditionIndex].IconSet.Cfvos[1].Value = 0;
                fcs[conditionIndex].IconSet.Cfvos[2].Type = FormatConditionValueType.Number;
                fcs[conditionIndex].IconSet.Cfvos[2].Value = 0;
            }
            #endregion

            //string documentFileNameRoot;
            //documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

            //Response.Clear();
            //Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            //Response.ContentType = "application/octet-stream";

            //document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            //Response.End();
        }

        public void ExportFromGridResult(Workbook document, GridResultExport data, WebSession session, int columnIndex = 1)
        {

            this.cInfo = WebApplicationParameters.AllowedLanguages[session.SiteLanguage].CultureInfo;

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            //document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1983, session.SiteLanguage));

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

            int coltmp = columnStart;
            int nbRowTotal = 0;

            if (data.HasData)
            {
                nbRowTotal = data.Columns.Count;

                DrawHeaders(data, sheet, rowStart, columnStart);
            }

            //rowStart += nbRowTotal;
            rowStart++;

            int colLevel = 0;
            int rowStartValue = rowStart;
            int rowEndValue = 0;
            HashSet<int> lstColEvol = new HashSet<int>();


            // Fige les entêtes de lignes et de colonnes
            sheet.FreezePanes(rowStart, columnStart, rowStart, columnStart);

            for (int idxCol = colLevel, cellCol = columnStart + colLevel; idxCol < data.Columns.Count; idxCol++)
            {
                columnHide = false;

                for (int idxRow = 0, cellRow = rowStart; idxRow < data.Data.Count; idxRow++, cellRow++)
                {

                    InfragisticData cell = data.Data[idxRow];

                    if (data.Columns[idxCol].Hidden == true)
                    {
                        columnHide = true;
                        break;
                    }

                    MEFData(session, cell, sheet, ref cellRow, cellCol, lstColEvol, idxCol);

                    if (rowEndValue < cellRow)
                        rowEndValue = cellRow;
                }

                if (columnHide == false)
                    cellCol++;
            }

            #region Ajustement de la taile des cellules en fonction du contenu 

            sheet.AutoFitColumns();

            #endregion

            #region Ajoute les icones des cellules
            if (lstColEvol.Count() > 0)
            {
                int idxCondis = sheet.ConditionalFormattings.Add();
                FormatConditionCollection fcs = sheet.ConditionalFormattings[idxCondis];

                foreach (int col in lstColEvol)
                {
                    CellArea cellArea = new CellArea();
                    cellArea.StartRow = rowStartValue;
                    cellArea.EndRow = rowEndValue;
                    cellArea.StartColumn = col;
                    cellArea.EndColumn = col;

                    fcs.AddArea(cellArea);
                }

                // Adds condition.
                int conditionIndex = fcs.AddCondition(FormatConditionType.IconSet, OperatorType.None, "0", "0");
                fcs[conditionIndex].IconSet.Type = IconSetType.Arrows3;

                //fcs[conditionIndex].IconSet.Cfvos[0].Type = FormatConditionValueType.Number;
                //fcs[conditionIndex].IconSet.Cfvos[0].Value = 0;                    
                fcs[conditionIndex].IconSet.Cfvos[1].Type = FormatConditionValueType.Number;
                fcs[conditionIndex].IconSet.Cfvos[1].Value = 0;
                fcs[conditionIndex].IconSet.Cfvos[2].Type = FormatConditionValueType.Number;
                fcs[conditionIndex].IconSet.Cfvos[2].Value = 0;
            }
            #endregion
        }

        private void MEFCell(WebSession session, ICell cell, Aspose.Cells.Worksheet sheet, ref int cellRow, int cellCol, LineType lineType, HashSet<int> lstColEvol)
        {
            Color textColor;
            Color backColor;
            Color borderColor;

            if (cell is CellPercent || cell is CellVersionNbPDM || cell is CellPDM)
            {
                double value = ((CellUnit)cell).Value;

                if (double.IsInfinity(value) || double.IsNaN(value) || double.Equals(value, 0.0))
                    sheet.Cells[cellRow, cellCol].Value = null;
                else
                    sheet.Cells[cellRow, cellCol].Value = value / 100;

                SetPercentFormat(sheet.Cells[cellRow, cellCol]);
                SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
            }
            else if (cell is CellEvol)
            {
                double value = ((CellUnit)cell).Value;

                if (double.IsInfinity(value) || double.IsNaN(value) || double.Equals(value, 0.0))
                    sheet.Cells[cellRow, cellCol].Value = null;
                else
                    sheet.Cells[cellRow, cellCol].Value = value / 100;

                SetPercentFormat(sheet.Cells[cellRow, cellCol]);
                SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);

                lstColEvol.Add(cellCol);
            }
            else if (cell is CellPage)
            {
                double value = ((CellPage)cell).Value;

                if (double.IsInfinity(value) || double.IsNaN(value) || double.Equals(value, 0.0))
                    sheet.Cells[cellRow, cellCol].Value = null;
                else
                    sheet.Cells[cellRow, cellCol].Value = value / 1000.0;

                if (((CellPage)cell).AsposeFormat == -1)
                    SetDecimalFormat(sheet.Cells[cellRow, cellCol]);
                else
                    SetAsposeFormat(sheet.Cells[cellRow, cellCol], ((CellPage)cell).AsposeFormat);

                SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
            }
            else if (cell is CellKEuro)
            {
                double value = ((CellKEuro)cell).Value;

                if (double.IsInfinity(value) || double.IsNaN(value) || double.Equals(value, 0.0))
                    sheet.Cells[cellRow, cellCol].Value = null;
                else
                    sheet.Cells[cellRow, cellCol].Value = value / 1000.0;

                if (((CellKEuro)cell).AsposeFormat == -1)
                    SetDecimalFormat(sheet.Cells[cellRow, cellCol]);
                else
                    SetAsposeFormat(sheet.Cells[cellRow, cellCol], ((CellKEuro)cell).AsposeFormat);

                SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
            }
            else if (cell is CellDuration)
            {
                double value = ((CellUnit)cell).GetValue();

                long hours = (long) value / 3600L;
                long minutes = (long) (value - (hours * 3600L)) / 60L;
                long secondes = (long) (value - (hours * 3600L) - (minutes * 60L));

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
                //string formatString = string.Format("{{0:{0}}}",
                //                  WebApplicationParameters.GenericColumnItemsInformation.
                //                  Get(GenericColumnItemInformation.Columns.airtime.GetHashCode()).StringFormat);
                if (((CellUnit)cell).StringFormat.Contains("airtime"))//formatString)
                {
                    sheet.Cells[cellRow, cellCol].Value = cell;
                }
                else
                {
                    double value = ((CellUnit)cell).GetValue();
                    if (value != 0.0)
                    {
                        sheet.Cells[cellRow, cellCol].Value = value;

                        SetAsposeFormat(sheet.Cells[cellRow, cellCol],
                            Int32.Parse(this.cInfo.GetAsposeFormatPatternFromStringFormat(((CellUnit) cell).StringFormat)));

                        //if (((CellUnit)cell).AsposeFormat == -1)
                        //    SetDecimalFormat(sheet.Cells[cellRow, cellCol]);
                        //else
                        //    SetAsposeFormat(sheet.Cells[cellRow, cellCol], ((CellUnit)cell).AsposeFormat);

                        SetIndentLevel(sheet.Cells[cellRow, cellCol], 1, true);
                    }
                    else
                    {
                        sheet.Cells[cellRow, cellCol].Value = null;
                    }
                }
            }
            else if (cell is CellLabel)
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(((CellLabel)cell).Label);
            else if (cell is CellInsertionInformation)
            {
                CellInsertionInformation cellI = cell as CellInsertionInformation;

                int i = 0;

                foreach (GenericColumnItemInformation g in cellI.GetColumns)
                {
                    if (g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster)
                    {
                        string name = WebUtility.HtmlDecode(GestionWeb.GetWebWord(g.WebTextId, session.SiteLanguage));

                        ICell cellTmp = cellI.GetValues[i];

                        if (cellTmp != null)
                        {
                            //MEFCell(session, cellTmp, sheet, cellRow, cellCol, lineType, lstColEvol);

                            if (!(cellTmp is CellUnit))
                            {
                                //values = value.Split(',');
                                //foreach (string s in values)
                                //{
                                //    if (hasData)
                                //    {
                                //        str.Append("<br/>");
                                //    }
                                //    hasData = true;
                                //    str.AppendFormat("{0}", s);
                                //}

                                sheet.Cells[cellRow, cellCol].Value = name + " : " + cellTmp.ToString();
                            }
                            else
                            {
                                sheet.Cells[cellRow, cellCol].Value = name + " : " + ((CellUnit)cellTmp).Value.ToString();
                            }

                            cellRow++;
                        }
                    }

                    i++;
                }

                //foreach (TNS.FrameWork.WebResultUI.Cell item in cellI.GetValues)
                //{
                //    MEFCell(item, sheet, cellRow, cellCol, lineType, lstColEvol);
                //}

            }
            else if (cell is CellEmpty)
                sheet.Cells[cellRow, cellCol].Value = null;
            else
            {
                sheet.Cells[cellRow, cellCol].Value = null;
            }


            switch (lineType)
            {
                case LineType.total:
                case LineType.nbParution:
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
            BorderStyle(sheet, cellRow, cellCol, CellBorderType.Hair, borderColor);
        }

        private void MEFData(WebSession session, InfragisticData cell, Aspose.Cells.Worksheet sheet, ref int cellRow, int cellCol, HashSet<int> lstColEvol, int idxCol)
        {
            Color textColor;
            Color backColor;
            Color borderColor;

            long number;
            bool result = long.TryParse((cell).Values[idxCol], out number);
            if (result) sheet.Cells[cellRow, cellCol].Value = number;
            else sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode((cell).Values[idxCol]);

            switch (cell.Level)
            {
                case 0:
                    textColor = LTotalText;
                    backColor = LTotalBackground;
                    borderColor = BorderTab;
                    break;
                case 1:
                    textColor = L1Text;
                    backColor = L1Background;
                    borderColor = BorderTab;
                    break;
                case 2:
                    textColor = L2Text;
                    backColor = L2Background;
                    borderColor = BorderTab;
                    if (idxCol == 0) SetIndentLevel(sheet.Cells[cellRow, cellCol], 1);
                    break;
                case 3:
                    textColor = L3Text;
                    backColor = L3Background;
                    borderColor = BorderTab;
                    if (idxCol == 0) SetIndentLevel(sheet.Cells[cellRow, cellCol], 2);
                    break;
                case 4:
                    textColor = L4Text;
                    backColor = L4Background;
                    borderColor = BorderTab;
                    if (idxCol == 0) SetIndentLevel(sheet.Cells[cellRow, cellCol], 3);
                    break;
                default:
                    textColor = Color.Black;
                    backColor = Color.White;
                    borderColor = Color.Black;
                    break;
            }

            TextStyle(sheet.Cells[cellRow, cellCol], textColor, backColor);
            BorderStyle(sheet, cellRow, cellCol, CellBorderType.Hair, borderColor);
        }

        public void ExportSelection(Workbook document, WebSession session, DetailSelectionResponse detailSelectionResponse)
        {
            Labels labels = LabelsHelper.LoadPageLabels(session.SiteLanguage);
            int cellRow = 4;
            int cellCol = 1;
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");
            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1989, session.SiteLanguage));
            sheet.IsGridlinesVisible = false;

            //Adding logo top
            string pathLogo = $"/Content/img/{WebApplicationParameters.CountryCode}/export_logo_km.png";
            pathLogo = System.Web.HttpContext.Current.Server.MapPath(pathLogo);

            Image img = Image.FromFile(pathLogo);
            int picId = sheet.Pictures.Add(0, 0, pathLogo);
            Picture pic = sheet.Pictures[picId];
            pic.Placement = PlacementType.FreeFloating;
            pic.Width = img.Width;
            pic.Height = img.Height;

            if (detailSelectionResponse.ShowStudyType)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.StudySelectionLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.ModuleLabel);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowDate)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.PeriodSelectionLabel);

                string startDate = string.Empty;
                string endDate = string.Empty;

                if (detailSelectionResponse.DateBegin != null)
                {
                    startDate = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(detailSelectionResponse.DateBegin.Value, detailSelectionResponse.SiteLanguage);
                }
                if (detailSelectionResponse.DateEnd != null)
                {
                    endDate = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(detailSelectionResponse.DateEnd.Value, detailSelectionResponse.SiteLanguage);
                }

                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.Dates).Trim();

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    if (!startDate.Equals(endDate))
                        sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.Dates).Trim() + " (" + startDate + " - " + endDate + ")";
                    else
                        sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.Dates).Trim() + " (" + startDate + ")";
                }

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowStudyPeriod)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.StudyPeriodLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.StudyPeriod);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowComparativePeriod)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.ComparativePeriodLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.ComparativePeriod);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowComparativePeriodType)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.ComparativePeriodTypeLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.ComparativePeriodType);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowPeriodDisponibilityType)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.PeriodDisponibilityTypeLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.PeriodDisponibilityType);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowUnivers)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.MediaLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.MediasSelectedLabel);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;

               


                #region New version

                if (detailSelectionResponse.ShowUniversDetails)
                {
                    sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.UniversSupportLabel);

                    for (int i = 0; i < detailSelectionResponse.UniversMedia.Count; i++)
                    {
                        sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMedia[i].Label);
                        TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                        TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                        cellRow++;

                        for (int j = 0; j < detailSelectionResponse.UniversMedia[i].UniversLevels.Count; j++)
                        {
                            sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMedia[i].UniversLevels[j].Label);
                            SetIndentLevel(sheet.Cells[cellRow, cellCol + 1], 1);
                            TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                            TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                            cellRow++;

                            for (int z = 0; z < detailSelectionResponse.UniversMedia[i].UniversLevels[j].UniversItems.Count; z++)
                            {
                                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMedia[i].UniversLevels[j].UniversItems[z].Label);
                                SetIndentLevel(sheet.Cells[cellRow, cellCol + 1], 2);
                                TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                                TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                                cellRow++;
                            }
                        }
                    }
                    cellRow++;

                }
                #endregion
            }
            if (detailSelectionResponse.ShowGenericlevelDetail)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.LevelDetailsLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.NiveauDetailLabel);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowGenericLevelDetailColumn)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.GenericLevelDetailColumnLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.GenericLevelDetailColumn);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }
            if (detailSelectionResponse.ShowUnity)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.UnitLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniteLabel);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }

            if (detailSelectionResponse.ShowIdSlogansLabel)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.IdSlogansLabel);
                sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.IdSlogansLabel);

                TextStyle(sheet.Cells[cellRow, cellCol], L1Text, L1Background);
                TextStyle(sheet.Cells[cellRow, cellCol + 1], L1Text, L1Background);

                cellRow++;
            }

            if (detailSelectionResponse.ShowMarket)
            {
                sheet.Cells[cellRow, cellCol].Value = WebUtility.HtmlDecode(labels.UniversProductLabel);

                //var size = (12 / Model.DetailSelectionWSModel.UniversMarket.Count);

                for (int i = 0; i < detailSelectionResponse.UniversMarket.Count; i++)
                {
                    sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMarket[i].Label);
                    TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                    TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                    cellRow++;

                    for (int j = 0; j < detailSelectionResponse.UniversMarket[i].UniversLevels.Count; j++)
                    {
                        sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMarket[i].UniversLevels[j].Label);
                        SetIndentLevel(sheet.Cells[cellRow, cellCol + 1], 1);
                        TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                        TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                        cellRow++;

                        for (int z = 0; z < detailSelectionResponse.UniversMarket[i].UniversLevels[j].UniversItems.Count; z++)
                        {
                            sheet.Cells[cellRow, cellCol + 1].Value = WebUtility.HtmlDecode(detailSelectionResponse.UniversMarket[i].UniversLevels[j].UniversItems[z].Label);
                            SetIndentLevel(sheet.Cells[cellRow, cellCol + 1], 2);
                            TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                            TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                            cellRow++;
                        }
                    }
                }

                //sheet.Cells[cellRow, cellCol + 1].Value = tmp;

                //TextStyle(sheet.Cells[cellRow, cellCol], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);
                //TextStyle(sheet.Cells[cellRow, cellCol + 1], TextAlignmentType.Left, TextAlignmentType.Top, L1Text, L1Background);

                cellRow++;
            }

            //Set copyright label
            sheet.Cells[cellRow + 1, cellCol].Value = $"{GestionWeb.GetWebWord(2266, session.SiteLanguage)} {DateTime.Now.Year.ToString()}";

            sheet.AutoFitColumns();
            sheet.AutoFitRows();

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

            style.Number = 4;

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
            if (!_skipIndent)
            {
                Style style = cell.GetStyle();

                if (isRight == true)
                    style.HorizontalAlignment = TextAlignmentType.Right;

                style.IndentLevel = indentLevel;

                cell.SetStyle(style);
            }
        }

        private int NbRow(HeaderBase root, WebSession session)
        {
            int nbRow = 1;
            int maxRow = 0;
            bool haveGroup = false;

            foreach (HeaderBase item in root)
            {
                if ((session.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES && item.Count > 0) || item is HeaderGroup)
                {
                    int tmp = NbRow(item, session);

                    if (tmp > maxRow)
                        maxRow = tmp;

                    haveGroup = true;
                }
            }

            if (!haveGroup && root.Count > 0)
                nbRow++;

            return nbRow + maxRow;
        }

        private int NbColumn(HeaderBase root, WebSession session)
        {
            int nbCol = 0;
            int maxCol = 0;

            if ((session.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES && root.Count > 0) || root is HeaderGroup)
            {
                if (root.Capacity == 0)
                    maxCol++;

                foreach (HeaderBase item in root)
                {
                    if ((session.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES && item.Count > 0) || item is HeaderGroup)
                    {
                        int tmp = NbColumn(item, session);

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

        private void DrawHeaders(HeaderBase head, Worksheet sheet, WebSession session, int rowStart, int colStart)
        {
            int nbRowTotal = NbRow(head, session) - 1;

            foreach (var item in head)
            {
                HeaderBase header = item as HeaderBase;

                if (header is HeaderMediaSchedule || header is HeaderCreative || header is HeaderInsertions)
                    continue;

                int rowSpan = nbRowTotal - (NbRow(header, session) - 1);
                int colSpan = NbColumn(header, session);

                if (colSpan > 1 || rowSpan > 1)
                {
                    Range range = sheet.Cells.CreateRange(rowStart, colStart, rowSpan, colSpan);
                    range.Merge();

                    sheet.Cells[rowStart, colStart].Value = WebUtility.HtmlDecode(header.Label);

                    TextStyle(sheet.Cells[rowStart, colStart], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
                }
                else
                {
                    sheet.Cells[rowStart, colStart].Value = WebUtility.HtmlDecode(header.Label);

                    TextStyle(sheet.Cells[rowStart, colStart], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, rowStart, colStart, CellBorderType.Hair, HeaderBorderTab);
                }

                if (session.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES && header.Count > 0)
                {
                    DrawHeaders(header, sheet, session, rowStart + rowSpan, colStart);
                }
                else if (header is HeaderGroup)
                {
                    DrawHeaders(header, sheet, session, rowStart + 1, colStart);
                }

                colStart += colSpan;
            }

        }


        private void DrawHeaders(GridResultExport head, Worksheet sheet, int rowStart, int colStart)
        {
            int nbRowTotal = head.Data.Count;

            foreach (var item in head.Columns)
            {
                int colSpan = 1;

                sheet.Cells[rowStart, colStart].Value = WebUtility.HtmlDecode(item.HeaderText);

                TextStyle(sheet.Cells[rowStart, colStart], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                BorderStyle(sheet, rowStart, colStart, CellBorderType.Hair, HeaderBorderTab);

                colStart += colSpan;
            }

        }

    }
}