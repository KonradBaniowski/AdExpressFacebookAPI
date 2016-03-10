//using Infragistics.Documents.Excel;
using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpressI.MediaSchedule.Functions;
using TNS.FrameWork.Date;

using CstWeb = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class TestExportController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaScheduleService _mediaSchedule;
        private WebSession _session;

        #region Constantes

        #region Line Constants
        /// <summary>
        /// Index of line "Total"
        /// </summary>
        public const int TOTAL_LINE_INDEX = 1;
        #endregion

        #region Column Indexes
        /// <summary>
        /// Index of N1 label
        /// </summary>
        public const int L1_COLUMN_INDEX = 0;
        /// <summary>
        /// Index of N2 label
        /// </summary>
        public const int L2_COLUMN_INDEX = 1;
        /// <summary>
        /// Index of N3 label
        /// </summary>
        public const int L3_COLUMN_INDEX = 2;
        /// <summary>
        /// Index of N4 label
        /// </summary>
        public const int L4_COLUMN_INDEX = 3;
        /// <summary>
        /// Index of periodicity column
        /// </summary>
        public const int PERIODICITY_COLUMN_INDEX = 4;
        /// <summary>
        /// Index of total column
        /// </summary>
        public const int TOTAL_COLUMN_INDEX = 5;
        /// <summary>
        /// Index de la colonne des pdms dans le tableau en mémoire
        /// </summary>
        public const int PDM_COLUMN_INDEX = 6;
        /// <summary>
        /// Index de la colonne du niveau 1
        /// </summary>
        public const int L1_ID_COLUMN_INDEX = 7;
        /// <summary>
        /// Index de la colonne du niveau 2
        /// </summary>
        public const int L2_ID_COLUMN_INDEX = 8;
        /// <summary>
        /// Index de la colonne du niveau 3
        /// </summary>
        public const int L3_ID_COLUMN_INDEX = 9;
        /// <summary>
        /// Index de la colonne du niveau 4
        /// </summary>
        public const int L4_ID_COLUMN_INDEX = 10;
        /// <summary>
        /// Index of total column de l'annee de comparaison
        /// </summary>
        public const int TOTAL_COMPARATIVE_COLUMN_INDEX = 11;
        /// <summary>
        /// Index de la colonne des pdms de l'annee de comparaison dans le tableau en mémoire
        /// </summary>
        public const int PDM_COMPARATIVE_COLUMN_INDEX = 12;
        /// <summary>
        /// Evolution des annees comparés
        /// </summary>
        public const int EVOL_COLUMN_INDEX = 13;
        #endregion

        /// <summary>
        /// Total line label
        /// </summary>
        public const string TOTAL_STRING = "TOTAL MEDIA";
        /// <summary>
        /// name of EXCEL PATTERN PERCENTAGE
        /// </summary>
        public const string EXCEL_PATTERN_NAME_PERCENTAGE = "percentage";

        #endregion

        #region Couleurs
        //Color Orange = Color.FromArgb(255, 140, 0);
        //Color DarkGray = Color.FromArgb(64, 68, 79);
        //Color MiddleGray = Color.FromArgb(67, 73, 95);
        //Color LightGray = Color.FromArgb(110, 117, 136);
        //Color Cyan = Color.FromArgb(38, 202, 255);
        //Color White = Color.White;
        //Color Black = Color.Black;

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

        private string icon;
        public TestExportController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
        }

        protected int GetSloganIdIndex()
        {
            int rank = _session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            switch (rank)
            {
                case 1:
                    return (DetailledMediaPlan.L1_ID_COLUMN_INDEX);
                case 2:
                    return (DetailledMediaPlan.L2_ID_COLUMN_INDEX);
                case 3:
                    return (DetailledMediaPlan.L3_ID_COLUMN_INDEX);
                case 4:
                    return (DetailledMediaPlan.L4_ID_COLUMN_INDEX);
                default:
                    return (-1);
            }
        }

        // GET: TestExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _mediaSchedule.GetMediaScheduleData(idWebSession);

            _session = (WebSession)WebSession.Load(idWebSession);

            //DateTime begin = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);            
            //DateTime end = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);

            //MediaSchedulePeriod _period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod);
            MediaSchedulePeriod _period = new MediaSchedulePeriod(_session.PeriodBeginningDate, _session.PeriodEndDate, _session.DetailPeriod);

            var _style = new TNS.AdExpressI.MediaSchedule.Style.DefaultMediaScheduleStyle();
            //var _style = new TNS.AdExpressI.MediaSchedule.Style.PDFMediaScheduleStyle();

            //MediaScheduleService mediaSchedule = _mediaSchedule as MediaScheduleService;

            //IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfoExcel;

            //WorkbookFormat excelFormat;

            //if (exportFormat)
            //excelFormat = WorkbookFormat.Excel2007;
            //else
            //    excelFormat = WorkbookFormat.Excel97To2003;

            //Workbook document = new Workbook(excelFormat);


            //var styleExcel = new TNS.AdExpressI.MediaSchedule.Style.ExcelMediaScheduleStyle();

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add("WorkSheet1");

            #region Fred
            //for (int idxRow = 0; idxRow < data.GetLength(0); idxRow++)
            //{
            //    for (int idxCol = 0; idxCol < data.GetLength(1); idxCol++)
            //    {
            //        string value = "";

            //        object obj = data[idxRow, idxCol];

            //        if (obj == null)
            //        {
            //            value = "";
            //        }
            //        else if (obj is MediaPlanItem)
            //        {
            //            MediaPlanItem item = obj as MediaPlanItem;

            //            value = Units.ConvertUnitValueToString(item.Unit, _session.Unit, fp);
            //        }
            //        else if (obj is string)
            //        {
            //            value = obj as string;
            //        }
            //        else if (obj.GetType() == typeof(MemoryArrayEnd))
            //        {
            //            value = "";
            //        }
            //        else
            //        {
            //            value = Units.ConvertUnitValueToString(obj, _session.Unit, fp);
            //        }


            //        currentWorksheet.Rows[idxRow].Cells[idxCol].Value = value;
            //    }
            //}
            #endregion

            #region old            

            //#region Theme Name
            //string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            //#endregion

            //if (data.GetLength(0) == 0)
            //{

            //    #region Init Variables
            //    int yearBegin = _period.Begin.Year;
            //    int yearEnd = _period.End.Year;
            //    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            //    {
            //        yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
            //        yearEnd = new AtomicPeriodWeek(_period.End).Year;
            //    }
            //    int nbColYear = yearEnd - yearBegin;
            //    if (nbColYear > 0) nbColYear++;
            //    int firstPeriodIndex = 0;

            //    if (WebApplicationParameters.UseComparativeMediaSchedule)
            //    {
            //        if (_session.ComparativeStudy)
            //        {
            //            firstPeriodIndex = EVOL_COLUMN_INDEX + 1;
            //        }
            //        else
            //        {
            //            firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //        }
            //    }
            //    else
            //    {
            //        firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //    }

            //    firstPeriodIndex += nbColYear;

            //    int nbColTab = data.GetLength(1);
            //    int nbPeriod = nbColTab - firstPeriodIndex - 1;
            //    int nbline = data.GetLength(0);

            //    try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
            //    catch (System.Exception) { }
            //    oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            //    bool isExport = true;
            //    int labColSpan = (isExport && !_allowTotal) ? 2 : 1;
            //    UnitInformation unit = UnitsInformation.Get(_session.Unit);
            //    #endregion

            //    GenericDetailLevel detailLevel = GetDetailsLevelSelected();

            //    #region Columns

            //    #region basic columns (product, total, PDM, version, insertion, years totals)
            //    int rowSpanNb = 3;
            //    if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //    {
            //        rowSpanNb = 2;
            //    }
            //    t.Append("<table id=\"calendartable\" border=1 cellpadding=0 cellspacing=0>\r\n\t<tr>");


            //    //Add Classification labels in columns
            //    for (int l = 1; l <= detailLevel.GetNbLevels; l++)
            //    {
            //        t.AppendFormat("\r\n\t\t<td colSpan=\"{2}\" rowspan=\"{1}\" width=\"250px\">{0}</td>"
            //       , TNS.FrameWork.Convertion.ToHtmlString(GestionWeb.GetWebWord(detailLevel[l].WebTextId, _session.SiteLanguage))
            //       , rowSpanNb
            //       , labColSpan);
            //    }

            //    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            //    {
            //        if (_allowTotal)
            //        {
            //            MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
            //            t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}", TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern), rowSpanNb);

            //            int nbtot;
            //            if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            //                nbtot = Units.ConvertUnitValueToString(((CellIdsNumber)data[1, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit, fp).Length;
            //            else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
            //            {
            //                nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit, fp).Length;
            //            }
            //            else
            //                nbtot = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[1, TOTAL_COMPARATIVE_COLUMN_INDEX])).Length;

            //            int nbSpace = (nbtot - 1) / 3;
            //            int nbCharTotal = nbtot + nbSpace - 5;
            //            if (nbCharTotal < 5) nbCharTotal = 0;
            //            for (int h = 0; h < nbCharTotal; h++)
            //            {
            //                t.Append("&nbsp;");
            //            }
            //            t.Append("</td>");
            //        }
            //        //PDM
            //        if (_allowPdm)
            //        {
            //            t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", GestionWeb.GetWebWord(806, _session.SiteLanguage), rowSpanNb);
            //        }
            //    }


            //    // Total Column
            //    if (_allowTotal)
            //    {
            //        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            //            t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}", TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern), rowSpanNb);
            //        else t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}", GestionWeb.GetWebWord(805, _session.SiteLanguage), rowSpanNb);


            //        int nbtot;
            //        if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            //            nbtot = Units.ConvertUnitValueToString(((CellIdsNumber)data[1, TOTAL_COLUMN_INDEX]).Value, _session.Unit, fp).Length;
            //        else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
            //        {
            //            nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
            //        }
            //        else
            //            nbtot = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[1, TOTAL_COLUMN_INDEX])).Length;

            //        int nbSpace = (nbtot - 1) / 3;
            //        int nbCharTotal = nbtot + nbSpace - 5;
            //        if (nbCharTotal < 5) nbCharTotal = 0;
            //        for (int h = 0; h < nbCharTotal; h++)
            //        {
            //            t.Append("&nbsp;");
            //        }
            //        t.Append("</td>");
            //    }
            //    //PDM
            //    if (_allowPdm)
            //    {
            //        t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", GestionWeb.GetWebWord(806, _session.SiteLanguage), rowSpanNb);

            //    }
            //    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            //    {
            //        //Evol
            //        t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", GestionWeb.GetWebWord(1212, _session.SiteLanguage), rowSpanNb);


            //    }
            //    //Version
            //    if (_allowVersion)
            //    {
            //        t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(1994, _session.SiteLanguage), rowSpanNb);
            //    }
            //    // Insertions
            //    if (_allowInsertions)
            //    {
            //        t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].WebTextId, _session.SiteLanguage), rowSpanNb);
            //    }
            //    if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //    {
            //        // Years necessary if the period consists of several years
            //        for (int k = firstPeriodIndex - nbColYear; k < firstPeriodIndex && _allowTotal; k++)
            //        {
            //            t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", data[0, k], rowSpanNb);

            //        }
            //    }
            //    #endregion

            //    #region Period
            //    nbPeriod = 0;
            //    int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //    StringBuilder periods = new StringBuilder();
            //    StringBuilder headers = new StringBuilder();
            //    string periodClass;
            //    string link = string.Empty;
            //    System.Uri uri = new Uri(_session.LastWebPage);
            //    link = uri.AbsolutePath;

            //    switch (_period.PeriodDetailLEvel)
            //    {
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
            //            prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //            for (int j = firstPeriodIndex; j < nbColTab; j++)
            //            {
            //                if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
            //                {
            //                    if (nbPeriod < 3)
            //                        headers.AppendFormat("<td colspan={0} >&nbsp;</td>", nbPeriod);

            //                    else
            //                        headers.AppendFormat("<td colspan={0} >{1}</td>", nbPeriod, prevPeriod);

            //                    nbPeriod = 0;
            //                    prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));
            //                }

            //                switch (_period.PeriodDetailLEvel)
            //                {
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

            //                        #region Period Color Management
            //                        // First Period or last period is incomplete
            //                        periodClass = _style.CellPeriod;
            //                        if ((j == firstPeriodIndex && _period.Begin.Day != 1)
            //                           || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
            //                        {
            //                            periodClass = _style.CellPeriodIncomplete;
            //                        }
            //                        #endregion

            //                        if (!isExport)
            //                        {
            //                            periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{2}?idSession={3}&zoomDate={4}\">&nbsp;{5}&nbsp;</td>"
            //                                , periodClass
            //                                , _style.CellPeriod
            //                                , link
            //                                , _session.IdSession
            //                                , data[0, j]
            //                                , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));
            //                        }
            //                        else
            //                        {
            //                            periods.AppendFormat("<td >&nbsp;{0}&nbsp;</td>"
            //                               , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));

            //                        }
            //                        break;
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

            //                        #region Period Color Management
            //                        periodClass = _style.CellPeriod;
            //                        if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
            //                           || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
            //                        {
            //                            periodClass = _style.CellPeriodIncomplete;
            //                        }
            //                        #endregion

            //                        if (!isExport && !IsCreativeDivisionMS)
            //                        {
            //                            periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{0}?idSession={1}&zoomDate={2}\">&nbsp;{3}&nbsp;</a></td>"
            //                               , link
            //                               , _session.IdSession
            //                               , data[0, j]
            //                               , data[0, j].ToString().Substring(4, 2));


            //                        }
            //                        else
            //                        {
            //                            periods.AppendFormat("<td >&nbsp;{0}&nbsp;</td>"
            //                              , data[0, j].ToString().Substring(4, 2));
            //                        }
            //                        break;

            //                }
            //                nbPeriod++;
            //            }
            //            // Compute last date
            //            if (nbPeriod < 3)
            //                headers.AppendFormat("<td colspan={0} >&nbsp;</td>", nbPeriod);

            //            else
            //                headers.AppendFormat("<td colspan={0}>{1}</td>", nbPeriod, prevPeriod);

            //            t.AppendFormat("{0}</tr>", headers);

            //            t.AppendFormat("<tr>{0}</tr>", periods);

            //            oMediaScheduleData.Headers = t.ToString();

            //            // t.Append("\r\n\t<tr>");

            //            break;
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
            //            StringBuilder days = new StringBuilder();
            //            periods.Append("<tr>");
            //            days.Append("\r\n\t<tr>");
            //            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
            //            prevPeriod = currentDay.Month;
            //            currentDay = currentDay.AddDays(-1);
            //            for (int j = firstPeriodIndex; j < nbColTab; j++)
            //            {
            //                currentDay = currentDay.AddDays(1);
            //                if (currentDay.Month != prevPeriod)
            //                {
            //                    if (nbPeriod >= 8)
            //                        headers.AppendFormat("<td colspan=\"{0}\" align=center>{1}</td>", nbPeriod, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")));
            //                    else
            //                        headers.AppendFormat("<td colspan=\"{0}\" align=center>&nbsp;</td>", nbPeriod);
            //                    nbPeriod = 0;
            //                    prevPeriod = currentDay.Month;
            //                }
            //                nbPeriod++;
            //                //Period Number
            //                periods.AppendFormat("<td nowrap>&nbsp;{0}&nbsp;</td>", currentDay.ToString("dd"));

            //                //Period day
            //                if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
            //                    days.AppendFormat("<td >{0}</td>", DayString.GetCharacters(currentDay, cultureInfo, 1));
            //                else
            //                    days.AppendFormat("<td >{0}</td>", DayString.GetCharacters(currentDay, cultureInfo, 1));

            //            }
            //            if (nbPeriod >= 8)
            //                headers.AppendFormat("<td colspan=\"{0}\" align=center>{1}</td>", nbPeriod, TNS.FrameWork.Convertion.ToHtmlString(TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.ToString("yyyyMM"))));
            //            else
            //                headers.AppendFormat("<td colspan=\"{0}\"  align=center>&nbsp;</td>", nbPeriod);

            //            periods.Append("</tr>");
            //            days.Append("</tr>");
            //            headers.Append("</tr>");

            //            t.Append(headers);
            //            t.Append(periods);
            //            t.Append(days);

            //            oMediaScheduleData.Headers = t.ToString();
            //            break;
            //    }
            //    #endregion

            //    #endregion

            //    #region Media Schedule
            //    int i = -1;
            //    try
            //    {
            //        int colorItemIndex = 1;
            //        int colorNumberToUse = 0;
            //        int sloganIndex = GetSloganIdIndex();
            //        Int64 sloganId = long.MinValue;
            //        string stringItem = "&nbsp;";
            //        string cssPresentClass = string.Empty;
            //        string cssExtendedClass = string.Empty;
            //        string cssClasse = string.Empty;
            //        string cssClasseNb = string.Empty;
            //        var classifLabels = new string[detailLevel.GetNbLevels];

            //        for (i = 1; i < nbline; i++)
            //        {

            //            #region Color Management
            //            if (sloganIndex != -1 && data[i, sloganIndex] != null &&
            //                ((detailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == detailLevel.GetNbLevels) ||
            //                (detailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < detailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
            //            {
            //                sloganId = Convert.ToInt64(data[i, sloganIndex]);
            //                if (!_session.SloganColors.ContainsKey(sloganId))
            //                {
            //                    colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
            //                    cssClasse = _style.CellVersions[colorNumberToUse];
            //                    cssClasseNb = _style.CellVersions[colorNumberToUse];
            //                    _session.SloganColors.Add(sloganId, _style.CellVersions[colorNumberToUse]);
            //                    if (_allowVersion)
            //                    {
            //                        oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, cssClasse));
            //                    }
            //                    else if (_isPDFReport && VehiclesInformation.Contains(_vehicleId))
            //                    {
            //                        switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
            //                        {
            //                            case CstDBClassif.Vehicles.names.directMarketing:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, cssClasse));
            //                                break;
            //                            case CstDBClassif.Vehicles.names.outdoor:
            //                            case CstDBClassif.Vehicles.names.indoor:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, cssClasse));
            //                                break;
            //                            case CstDBClassif.Vehicles.names.instore:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportInstoreVersionItem(sloganId, cssClasse));
            //                                break;
            //                            default:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, cssClasse));
            //                                break;
            //                        }

            //                    }
            //                    colorItemIndex++;
            //                }
            //                if (sloganId != 0 && !oMediaScheduleData.VersionsDetail.ContainsKey(sloganId))
            //                {
            //                    if (_allowVersion)
            //                    {
            //                        oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
            //                    }
            //                    else if (_isPDFReport && VehiclesInformation.Contains(_vehicleId))
            //                    {
            //                        switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
            //                        {
            //                            case CstDBClassif.Vehicles.names.directMarketing:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
            //                                break;
            //                            case CstDBClassif.Vehicles.names.outdoor:
            //                            case CstDBClassif.Vehicles.names.indoor:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
            //                                break;
            //                            case CstDBClassif.Vehicles.names.instore:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportInstoreVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
            //                                break;
            //                            default:
            //                                oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
            //                                break;
            //                        }

            //                    }
            //                }
            //                cssPresentClass = _session.SloganColors[sloganId].ToString();
            //                cssExtendedClass = _session.SloganColors[sloganId].ToString();
            //                stringItem = "x";
            //            }
            //            else
            //            {
            //                cssPresentClass = _style.CellPresent;
            //                cssExtendedClass = _style.CellExtended;
            //                stringItem = "&nbsp;";
            //            }
            //            #endregion

            //            #region Line Treatement
            //            for (int j = 0; j < nbColTab; j++)
            //            {
            //                switch (j)
            //                {

            //                    #region Level 1
            //                    case L1_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {
            //                            if (i == TOTAL_LINE_INDEX)
            //                            {
            //                                cssClasse = _style.CellLevelTotal;
            //                                cssClasseNb = _style.CellLevelTotalNb;
            //                            }
            //                            else
            //                            {
            //                                cssClasse = _style.CellLevelL1;
            //                                cssClasseNb = _style.CellLevelL1Nb;
            //                            }
            //                            if (data[i, j].GetType() == typeof(MemoryArrayEnd))
            //                            {
            //                                i = int.MaxValue - 2;
            //                                j = int.MaxValue - 2;
            //                                break;
            //                            }
            //                            classifLabels[0] = Convert.ToString(data[i, j]);
            //                            AppenLabelTotalPDM(data, t, i, cssClasse, cssClasseNb, j, string.Empty, labColSpan, fp, unit, classifLabels, 1);

            //                            if (_allowVersion)
            //                            {
            //                                if (i != TOTAL_LINE_INDEX && !IsAgencyLevelType(L1_COLUMN_INDEX))
            //                                {
            //                                    AppendCreativeLink(data, t, themeName, i, cssClasse, j);
            //                                }
            //                                else
            //                                {
            //                                    t.AppendFormat("<td align=\"center\"></td>");
            //                                }

            //                            }
            //                            if (_allowInsertions)
            //                            {
            //                                if (i != TOTAL_LINE_INDEX && !IsAgencyLevelType(L1_COLUMN_INDEX))
            //                                {
            //                                    AppendInsertionLink(data, t, themeName, i, cssClasse, j);
            //                                }
            //                                else
            //                                {
            //                                    t.AppendFormat("<td align=\"center\"></td>");
            //                                }
            //                            }
            //                            //Totals
            //                            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                            {
            //                                for (int k = 1; k <= nbColYear; k++)
            //                                {
            //                                    AppendYearsTotal(data, t, i, cssClasseNb, (j + firstPeriodIndex - nbColYear - 1) + k, fp, unit);
            //                                }
            //                            }
            //                            j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 2
            //                    case L2_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {
            //                            classifLabels[1] = Convert.ToString(data[i, j]);
            //                            AppenLabelTotalPDM(data, t, i, _style.CellLevelL2, _style.CellLevelL2Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 2);
            //                            if (_allowVersion)
            //                            {
            //                                if (!IsAgencyLevelType(L2_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL2, j);
            //                                else t.AppendFormat("<td align=\"center\" ></td>");
            //                            }
            //                            if (_allowInsertions)
            //                            {
            //                                if (!IsAgencyLevelType(L2_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL2, j);
            //                                else t.AppendFormat("<td align=\"center\" ></td>");
            //                            }
            //                            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                            {
            //                                for (int k = 1; k <= nbColYear; k++)
            //                                {
            //                                    AppendYearsTotal(data, t, i, _style.CellLevelL2Nb, j + (firstPeriodIndex - nbColYear - 2) + k, fp, unit);
            //                                }
            //                            }
            //                            j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 3
            //                    case L3_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {
            //                            classifLabels[2] = Convert.ToString(data[i, j]);
            //                            AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 3);
            //                            if (_allowVersion)
            //                            {
            //                                if (!IsAgencyLevelType(L3_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL3, j);
            //                                else t.AppendFormat("<td align=\"center\"></td>");
            //                            }
            //                            if (_allowInsertions)
            //                            {
            //                                if (!IsAgencyLevelType(L3_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL3, j);
            //                                else t.AppendFormat("<td align=\"center\"></td>");
            //                            }
            //                            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                            {
            //                                for (int k = 1; k <= nbColYear; k++)
            //                                {
            //                                    AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear - 3) + k, fp, unit);
            //                                }
            //                            }
            //                            j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 4
            //                    case L4_COLUMN_INDEX:
            //                        classifLabels[3] = Convert.ToString(data[i, j]);
            //                        AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 4);
            //                        if (_allowVersion)
            //                        {
            //                            if (!IsAgencyLevelType(L4_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL4, j);
            //                            else t.AppendFormat("<td align=\"center\" ></td>");
            //                        }
            //                        if (_allowInsertions)
            //                        {
            //                            if (!IsAgencyLevelType(L4_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL4, j);
            //                            else t.AppendFormat("<td align=\"center\"></td>");
            //                        }
            //                        if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                        {
            //                            for (int k = 1; k <= nbColYear; k++)
            //                            {
            //                                AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear - 4) + k, fp, unit);
            //                            }
            //                        }
            //                        j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
            //                        break;
            //                    #endregion

            //                    #region Other
            //                    default:
            //                        if (data[i, j] == null)
            //                        {
            //                            t.AppendFormat("<td >&nbsp;</td>");
            //                            break;
            //                        }
            //                        if (data[i, j] is MediaPlanItem)
            //                        {
            //                            switch (((MediaPlanItem)data[i, j]).GraphicItemType)
            //                            {
            //                                case DetailledMediaPlan.graphicItemType.present:
            //                                    if (_showValues)
            //                                    {
            //                                        if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            //                                            t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItemIds)data[i, j]).IdsNumber.Value, _session.Unit, fp));
            //                                        else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
            //                                            t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItem)data[i, j]).Unit, _session.Unit, fp));
            //                                        else
            //                                            t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, string.Format(fp, unit.StringFormat, ((MediaPlanItem)data[i, j]).Unit));
            //                                    }
            //                                    else
            //                                    {
            //                                        t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, stringItem);
            //                                    }
            //                                    break;
            //                                case DetailledMediaPlan.graphicItemType.extended:
            //                                    t.AppendFormat("<td class=\"{0}\">&nbsp;</td>", cssExtendedClass);
            //                                    break;
            //                                default:
            //                                    t.AppendFormat("<td class=\"{0}\"nbsp;</td>", "NpX");
            //                                    break;
            //                            }
            //                        }
            //                        break;
            //                        #endregion

            //                }
            //            }
            //            t.Append("</tr>");
            //            #endregion

            //        }
            //    }
            //    catch (System.Exception err)
            //    {
            //        throw (new MediaScheduleException("Error i=" + i, err));
            //    }
            //    t.Append("</table>");
            //    #endregion
            //}

            #endregion


            #region Old Aspose
            //if (data.GetLength(0) != 0)
            //{

            //    bool _allowTotal = true;
            //    bool _allowPdm = true;
            //    bool _showValues = false;

            //    #region Init Variables
            //    CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            //    IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            //    MediaScheduleData oMediaScheduleData = new MediaScheduleData();
            //    UnitInformation unitInformation = _session.GetSelectedUnit();

            //    #region Excel
            //    //Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
            //    Cells cells = sheet.Cells;
            //    string formatTotal = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(unitInformation.Format);
            //    string formatPdm = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(EXCEL_PATTERN_NAME_PERCENTAGE);

            //    bool premier = true;
            //    int header = 1;
            //    string prevYearString = string.Empty;
            //    int nbMaxRowByPage = 42;
            //    int s = 1;
            //    int cellRow = 5;
            //    int startIndex = cellRow;
            //    int upperLeftColumn = 10;
            //    string vPageBreaks = "";
            //    double columnWidth = 0, indexLogo = 0, index;
            //    bool verif = true;
            //    int colSupport = 1;
            //    int colTotal = 2;
            //    int colPdm = 2;
            //    int colTotalYears = 2;
            //    int colVersion = 2;
            //    int colInsertion = 2;
            //    int colFirstMediaPlan = 2;

            //    int colorItemIndex = 1;
            //    int colorNumberToUse = 0;
            //    int sloganIndex = GetSloganIdIndex();
            //    Int64 sloganId = long.MinValue;
            //    string stringItem = "";
            //    string presentstyle = string.Empty;
            //    string extendedStyle = string.Empty;
            //    string style = string.Empty;
            //    string styleNb = string.Empty;
            //    string stylePdmNb = string.Empty;
            //    #endregion

            //    int yearBegin = _period.Begin.Year;
            //    int yearEnd = _period.End.Year;
            //    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            //    {
            //        yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
            //        yearEnd = new AtomicPeriodWeek(_period.End).Year;
            //    }
            //    int nbColYear = yearEnd - yearBegin;
            //    if (nbColYear > 0) nbColYear++;
            //    int firstPeriodIndex = 0;

            //    if (WebApplicationParameters.UseComparativeMediaSchedule)
            //    {
            //        if (_session.ComparativeStudy)
            //        {
            //            firstPeriodIndex = EVOL_COLUMN_INDEX + 1;
            //        }
            //        else
            //        {
            //            firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //        }
            //    }
            //    else
            //    {
            //        firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //    }
            //    firstPeriodIndex += nbColYear;

            //    int nbColTab = data.GetLength(1);
            //    int nbPeriod = nbColTab - firstPeriodIndex - 1;
            //    int nbPeriodTotal = 0;
            //    int nbline = data.GetLength(0);
            //    int nbColTabFirst = 0;
            //    int nbColTabCell = 0;

            //    try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
            //    catch (System.Exception) { }
            //    oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            //    int labColSpan = 1;
            //    #endregion

            //    #region Rappel de sélection
            //    /*if (_isExcelReport) {
            //        if (_isCreativeDivisionMS) {
            //            t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
            //        }
            //        else {
            //            if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE) {
            //                t.Append(FctExcel.GetLogo(_session));
            //                if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA) {
            //                    t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
            //                }
            //                else {
            //                    t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
            //                }
            //            }
            //            else {
            //                t.Append(FctExcel.GetAppmLogo(_session));
            //                t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474, _session.SiteLanguage)));
            //            }
            //        }
            //    }*/
            //    #endregion

            //    #region basic columns (product, total, PDM, years totals)
            //    int rowSpanNb = 3;
            //    if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //    {
            //        rowSpanNb = 2;
            //    }

            //    #region Title first column (Product Column)
            //    cells.Merge(cellRow - 1, colSupport, rowSpanNb, labColSpan);
            //    WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(804, _session.SiteLanguage), cellRow - 1, colSupport, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
            //    cells[cellRow - 1, colSupport].GetStyle().HorizontalAlignment = TextAlignmentType.Left;
            //    cells[cellRow - 1, colSupport].GetStyle().VerticalAlignment = TextAlignmentType.Top;
            //    //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
            //    //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //    //{
            //    //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colSupport);
            //    //}
            //    nbColTabFirst++;
            //    #endregion

            //    #region Total Column
            //    if (_allowTotal)
            //    {
            //        colTotal = 2;
            //        colPdm++;
            //        colVersion++;
            //        colInsertion++;
            //        colTotalYears++;
            //        colFirstMediaPlan++;
            //        cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
            //        WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(805, _session.SiteLanguage), cellRow - 1, colTotal, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
            //        styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
            //        //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        //{
            //        //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotal);
            //        //}
            //        int nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
            //        int nbSpace = (nbtot - 1) / 3;
            //        int nbCharTotal = nbtot + nbSpace - 5;
            //        nbColTabFirst++;
            //    }
            //    else
            //    {
            //        colTotal = 0;
            //        colPdm = 2;
            //        colVersion = 2;
            //        colInsertion = 2;
            //        colFirstMediaPlan = 2;
            //        colTotalYears = 2;
            //    }
            //    #endregion

            //    #region PDM Column
            //    if (_allowPdm)
            //    {
            //        colVersion++;
            //        colInsertion++;
            //        colFirstMediaPlan++;
            //        colTotalYears++;
            //        cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
            //        WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(806, _session.SiteLanguage), cellRow - 1, colPdm, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
            //        //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colPdm);
            //        //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        //{
            //        //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colPdm);
            //        //}
            //        nbColTabFirst++;
            //    }
            //    else
            //    {
            //        colPdm = 0;
            //    }
            //    #endregion

            //    #region Total Years
            //    if (nbColYear > 0 && _allowTotal)
            //    {
            //        int nbAddCol = 1;
            //        if (nbColYear != 0)
            //            nbAddCol = nbColYear;
            //        colFirstMediaPlan += nbAddCol;
            //        // Years necessary if the period consists of several years
            //        for (int k = firstPeriodIndex - nbColYear, l = 0; k < firstPeriodIndex; k++, l++)
            //        {
            //            cells.Merge(cellRow - 1, colTotalYears + l, rowSpanNb, labColSpan);
            //            //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colTotalYears + l);
            //            //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //            //{
            //            //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotalYears + l);
            //            //}
            //            WorkSheet.PutCellValue(excel, cells, data[0, k], cellRow - 1, colTotalYears + l, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
            //            nbColTabFirst++;
            //        }

            //    }
            //    else
            //    {
            //        colTotalYears = 0;
            //    }
            //    #endregion

            //    #region Period
            //    nbPeriod = 0;
            //    int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //    int lastPeriod = prevPeriod;
            //    bool first = true;
            //    string periodStyle = string.Empty;
            //    switch (_period.PeriodDetailLEvel)
            //    {
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
            //            prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //            for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
            //            {
            //                if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
            //                {
            //                    cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
            //                    if (nbPeriod < 3)
            //                        WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
            //                    else
            //                        WorkSheet.PutCellValue(excel, cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);

            //                    //styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
            //                    nbColTabFirst += nbPeriod;
            //                    nbPeriod = 0;
            //                    prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

            //                }

            //                switch (_period.PeriodDetailLEvel)
            //                {
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

            //                        #region Period Color Management
            //                        // First Period or last period is incomplete
            //                        periodStyle = "MediaPlanCellPeriod";
            //                        if ((j == firstPeriodIndex && _period.Begin.Day != 1)
            //                           || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
            //                        {
            //                            periodStyle = "MediaPlanCellPeriodIncomplete";
            //                        }
            //                        #endregion

            //                        WorkSheet.PutCellValue(excel, cells, MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null, styleExcel);
            //                        break;
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

            //                        #region Period Color Management
            //                        periodStyle = "MediaPlanCellPeriod";
            //                        if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
            //                           || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
            //                        {
            //                            periodStyle = "MediaPlanCellPeriodIncomplete";
            //                        }
            //                        #endregion

            //                        WorkSheet.PutCellValue(excel, cells, int.Parse(data[0, j].ToString().Substring(4, 2)), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null, styleExcel);
            //                        break;

            //                }
            //                nbPeriod++;
            //                nbPeriodTotal++;
            //            }
            //            // Compute last date
            //            cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
            //            if (nbPeriod < 3)
            //                WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);
            //            else
            //                WorkSheet.PutCellValue(excel, cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);

            //            for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
            //            {
            //                cells[startIndex - 1, k].Style.Number = 1;
            //                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            //                    cells[startIndex, k].Style.Number = 1;
            //            }

            //            break;
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
            //            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
            //            prevPeriod = currentDay.Month;
            //            currentDay = currentDay.AddDays(-1);
            //            for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
            //            {
            //                currentDay = currentDay.AddDays(1);
            //                if (currentDay.Month != prevPeriod)
            //                {
            //                    cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
            //                    if (nbPeriod >= 8)
            //                        WorkSheet.PutCellValue(excel, cells, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
            //                    else
            //                        WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
            //                    //styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
            //                    nbColTabFirst += nbPeriod;
            //                    nbPeriod = 0;
            //                    prevPeriod = currentDay.Month;
            //                }
            //                nbPeriod++;
            //                nbPeriodTotal++;
            //                //Period Number
            //                WorkSheet.PutCellValue(excel, cells, currentDay.ToString("dd"), startIndex, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPeriod", null, styleExcel);
            //                //Period day
            //                if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
            //                    WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDayWE", null, styleExcel);
            //                else
            //                    WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDay", null, styleExcel);
            //            }


            //            cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
            //            if (nbPeriod >= 8)
            //                WorkSheet.PutCellValue(excel, cells, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);
            //            else
            //                WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);


            //            for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
            //            {
            //                cells[startIndex, k].Style.Number = 1;
            //            }


            //            break;

            //    }
            //    #endregion

            //    #endregion

            //    #region init Row Media Shedule
            //    cellRow++;

            //    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        cellRow++;
            //    #endregion

            //    #region Media Schedule
            //    int i = -1;
            //    try
            //    {
            //        first = true;
            //        nbColTabCell = colFirstMediaPlan;
            //        int currentColMediaPlan = 0;
            //        for (i = 1; i < nbline; i++)
            //        {

            //            #region Color Management
            //            if (sloganIndex != -1 && data[i, sloganIndex] != null &&
            //                ((_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == _session.GenericMediaDetailLevel.GetNbLevels) ||
            //                (_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < _session.GenericMediaDetailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
            //            {
            //                sloganId = Convert.ToInt64(data[i, sloganIndex]);
            //                if (!_session.SloganColors.ContainsKey(sloganId))
            //                {
            //                    colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
            //                    _session.SloganColors.Add(sloganId, "MediaPlanCellVersions" + colorNumberToUse);
            //                    colorItemIndex++;
            //                }

            //                presentstyle = _session.SloganColors[sloganId].ToString();
            //                extendedStyle = _session.SloganColors[sloganId].ToString();
            //                stringItem = "x";
            //            }
            //            else
            //            {
            //                presentstyle = "MediaPlanCellPresent";
            //                extendedStyle = "MediaPlanCellExtended";
            //                stringItem = "";
            //            }
            //            #endregion

            //            #region Line Treatement
            //            currentColMediaPlan = colFirstMediaPlan;
            //            for (int j = 0; j < nbColTab; j++)
            //            {
            //                switch (j)
            //                {
            //                    #region Level 1
            //                    case L1_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {

            //                            if (data[i, j].GetType() == typeof(MemoryArrayEnd))
            //                            {
            //                                i = int.MaxValue - 2;
            //                                j = int.MaxValue - 2;
            //                                break;
            //                            }

            //                            #region Style define
            //                            if (i == TOTAL_LINE_INDEX)
            //                            {
            //                                style = "MediaPlanCellLevelTotal";
            //                                styleNb = "MediaPlanCellLevelTotalNb";
            //                                stylePdmNb = "MediaPlanCellLevelTotalPdmNb";
            //                            }
            //                            else
            //                            {
            //                                style = "MediaPlanCellLevelL1";
            //                                styleNb = "MediaPlanCellLevelL1Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL1PdmNb";
            //                            }
            //                            #endregion

            //                            #region Label
            //                            WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null, styleExcel);
            //                            #endregion

            //                            #region Total
            //                            if (_allowTotal)
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal, styleExcel);
            //                            #endregion

            //                            #region PDM
            //                            if (_allowPdm)
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm, styleExcel);
            //                            #endregion

            //                            #region Totals years
            //                            for (int k = 1; k <= nbColYear && _allowTotal; k++)
            //                            {
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, j + (firstPeriodIndex - nbColYear - 1) + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal, styleExcel);
            //                            }
            //                            #endregion

            //                            j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 2
            //                    case L2_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {

            //                            #region Style define
            //                            if (premier)
            //                            {
            //                                style = "MediaPlanCellLevelL2_1";
            //                                styleNb = "MediaPlanCellLevelL2_1Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL2_1PdmNb";
            //                            }
            //                            else
            //                            {
            //                                style = "MediaPlanCellLevelL2_2";
            //                                styleNb = "MediaPlanCellLevelL2_2Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL2_2PdmNb";
            //                            }
            //                            #endregion

            //                            #region Label
            //                            WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null, styleExcel);
            //                            #endregion

            //                            #region Total
            //                            if (_allowTotal)
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal, styleExcel);
            //                            #endregion

            //                            #region PDM
            //                            if (_allowPdm)
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm, styleExcel);
            //                            #endregion

            //                            #region Totals years
            //                            for (int k = 1; k <= nbColYear && _allowTotal; k++)
            //                            {
            //                                WorkSheet.PutCellValue(excel, cells, ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal, styleExcel);
            //                            }
            //                            #endregion

            //                            premier = !premier;

            //                            j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 3
            //                    case L3_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {
            //                            WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL3", null, styleExcel);
            //                            /*
            //                            AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
            //                            */
            //                            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                            {
            //                                for (int k = 1; k <= nbColYear; k++)
            //                                {
            //                                    //AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear-3) + k);
            //                                }
            //                            }
            //                            j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 4
            //                    case L4_COLUMN_INDEX:
            //                        WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL4", null, styleExcel);
            //                        //AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);
            //                        if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                        {
            //                            for (int k = 1; k <= nbColYear; k++)
            //                            {
            //                                //AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear-4) + k);
            //                            }
            //                        }
            //                        j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
            //                        break;
            //                    #endregion

            //                    #region Other
            //                    default:
            //                        if (data[i, j] == null)
            //                        {
            //                            WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);
            //                            currentColMediaPlan++;
            //                            break;
            //                        }
            //                        if (data[i, j].GetType() == typeof(MediaPlanItem))
            //                        {
            //                            switch (((MediaPlanItem)data[i, j]).GraphicItemType)
            //                            {
            //                                case DetailledMediaPlan.graphicItemType.present:
            //                                    if (_showValues)
            //                                    {
            //                                        WorkSheet.PutCellValue(excel, cells, ((MediaPlanItem)data[i, j]).Unit, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", formatTotal, styleExcel);
            //                                    }
            //                                    else
            //                                    {
            //                                        WorkSheet.PutCellValue(excel, cells, stringItem, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", null, styleExcel);
            //                                    }
            //                                    break;
            //                                case DetailledMediaPlan.graphicItemType.extended:
            //                                    WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellExtended", null, styleExcel);
            //                                    break;
            //                                default:
            //                                    WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);
            //                                    break;
            //                            }
            //                            currentColMediaPlan++;
            //                        }
            //                        break;
            //                        #endregion
            //                }
            //            }
            //            if (first)
            //            {
            //                first = !first;
            //                nbColTabCell += currentColMediaPlan - 1;
            //            }
            //            cellRow++;
            //            #endregion

            //        }
            //    }
            //    catch (System.Exception err)
            //    {
            //        //throw (new MediaScheduleException("Error i=" + i, err));
            //    }
            //    #endregion

            //    #region Mise en forme de la page

            //    #region Ajustement de la taile des cellules en fonction du contenu
            //    sheet.AutoFitColumn(colSupport);

            //    for (int c = colFirstMediaPlan; c <= (nbColTabCell + 1 - colFirstMediaPlan); c++)
            //    {
            //        if (_showValues)
            //        {
            //            sheet.AutoFitColumn(c);
            //        }
            //        else
            //        {
            //            cells.SetColumnWidth((byte)c, 2);
            //        }
            //    }

            //    #endregion

            //    if (_session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.monthly)
            //    {
            //        for (index = 0; index < 30; index++)
            //        {
            //            columnWidth += cells.GetColumnWidth((byte)index);
            //            if ((columnWidth < 124) && verif)
            //                indexLogo++;
            //            else
            //                verif = false;
            //        }
            //        upperLeftColumn = (int)indexLogo - 1;
            //        vPageBreaks = cells[cellRow, (int)indexLogo].Name;
            //        WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), data.GetLength(0) + 3, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), styleExcel);
            //    }
            //    else
            //    {
            //        if (nbColTabCell > 44)
            //        {
            //            upperLeftColumn = nbColTabCell - 4;
            //            vPageBreaks = cells[cellRow, nbColTabCell - 4].Name;
            //        }
            //        else
            //        {
            //            for (index = 0; index < 30; index++)
            //            {
            //                columnWidth += cells.GetColumnWidth((byte)index);
            //                if ((columnWidth < 124) && verif)
            //                    indexLogo++;
            //                else
            //                    verif = false;
            //            }
            //            upperLeftColumn = (int)indexLogo - 1;
            //            vPageBreaks = cells[cellRow, (int)indexLogo].Name;
            //        }

            //        WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), ref s, upperLeftColumn, header.ToString(), styleExcel);
            //    }
            //    #endregion
            //}
            #endregion

            #region Infragistics

            //if (data.GetLength(0) != 0)
            //{
            //    bool _allowTotal = true;
            //    bool _allowPdm = true;
            //    bool _showValues = false;

            //    #region Init Variables
            //    CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            //    IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            //    MediaScheduleData oMediaScheduleData = new MediaScheduleData();
            //    UnitInformation unitInformation = _session.GetSelectedUnit();

            //    #region Excel
            //    //Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
            //    //Worksheet sheet = document.Worksheets.Add("WorkSheet1");

            //    //Cells cells = sheet.Cells;
            //    string formatTotal = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(unitInformation.Format);
            //    string formatPdm = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(EXCEL_PATTERN_NAME_PERCENTAGE);

            //    bool premier = true;
            //    int header = 1;
            //    string prevYearString = string.Empty;
            //    int nbMaxRowByPage = 42;
            //    int s = 1;
            //    int cellRow = 5;
            //    int startIndex = cellRow;
            //    int upperLeftColumn = 10;
            //    string vPageBreaks = "";
            //    double columnWidth = 0, indexLogo = 0, index;
            //    bool verif = true;
            //    int colSupport = 1;
            //    int colTotal = 2;
            //    int colPdm = 2;
            //    int colTotalYears = 2;
            //    int colVersion = 2;
            //    int colInsertion = 2;
            //    int colFirstMediaPlan = 2;

            //    int colorItemIndex = 1;
            //    int colorNumberToUse = 0;
            //    int sloganIndex = GetSloganIdIndex();
            //    Int64 sloganId = long.MinValue;
            //    string stringItem = "";
            //    string presentstyle = string.Empty;
            //    string extendedStyle = string.Empty;
            //    string style = string.Empty;
            //    string styleNb = string.Empty;
            //    string stylePdmNb = string.Empty;
            //    #endregion

            //    int yearBegin = _period.Begin.Year;
            //    int yearEnd = _period.End.Year;
            //    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            //    {
            //        yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
            //        yearEnd = new AtomicPeriodWeek(_period.End).Year;
            //    }
            //    int nbColYear = yearEnd - yearBegin;
            //    if (nbColYear > 0) nbColYear++;
            //    int firstPeriodIndex = 0;

            //    if (WebApplicationParameters.UseComparativeMediaSchedule)
            //    {
            //        if (_session.ComparativeStudy)
            //        {
            //            firstPeriodIndex = EVOL_COLUMN_INDEX + 1;
            //        }
            //        else
            //        {
            //            firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //        }
            //    }
            //    else
            //    {
            //        firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
            //    }
            //    firstPeriodIndex += nbColYear;

            //    int nbColTab = data.GetLength(1);
            //    int nbPeriod = nbColTab - firstPeriodIndex - 1;
            //    int nbPeriodTotal = 0;
            //    int nbline = data.GetLength(0);
            //    int nbColTabFirst = 0;
            //    int nbColTabCell = 0;

            //    try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
            //    catch (System.Exception) { }
            //    oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            //    int labColSpan = 1;
            //    #endregion

            //    #region Rappel de sélection
            //    /*if (_isExcelReport) {
            //        if (_isCreativeDivisionMS) {
            //            t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
            //        }
            //        else {
            //            if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE) {
            //                t.Append(FctExcel.GetLogo(_session));
            //                if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA) {
            //                    t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
            //                }
            //                else {
            //                    t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
            //                }
            //            }
            //            else {
            //                t.Append(FctExcel.GetAppmLogo(_session));
            //                t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474, _session.SiteLanguage)));
            //            }
            //        }
            //    }*/
            //    #endregion

            //    #region basic columns (product, total, PDM, years totals)
            //    int rowSpanNb = 3;
            //    if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //    {
            //        rowSpanNb = 2;
            //    }

            //    #region Title first column (Product Column)
            //    WorksheetMergedCellsRegion region = sheet.MergedCellsRegions.Add(cellRow - 1, colSupport, cellRow - 1 + rowSpanNb - 1, colSupport + labColSpan - 1);
            //    sheet.Rows[cellRow - 1].Cells[colSupport].Value = GestionWeb.GetWebWord(804, _session.SiteLanguage);
            //    //WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(804, _session.SiteLanguage), cellRow - 1, colSupport, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
            //    sheet.Rows[cellRow - 1].Cells[colSupport].CellFormat.Alignment = HorizontalCellAlignment.Center;
            //    sheet.Rows[cellRow - 1].Cells[colSupport].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //    sheet.Rows[cellRow - 1].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //    sheet.Rows[cellRow - 1].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //    BorderStyle(region, CellBorderLineStyle.Thin, LightGray);

            //    //TODO
            //    //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
            //    //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //    //{
            //    //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colSupport);
            //    //}
            //    nbColTabFirst++;
            //    #endregion

            //    #region Total Column
            //    if (_allowTotal)
            //    {
            //        colTotal = 2;
            //        colPdm++;
            //        colVersion++;
            //        colInsertion++;
            //        colTotalYears++;
            //        colFirstMediaPlan++;
            //        region = sheet.MergedCellsRegions.Add(cellRow - 1, colTotal, cellRow - 1 + rowSpanNb - 1, colTotal + labColSpan - 1);
            //        sheet.Rows[cellRow - 1].Cells[colTotal].Value = GestionWeb.GetWebWord(805, _session.SiteLanguage);

            //        sheet.Rows[cellRow - 1].Cells[colTotal].CellFormat.Alignment = HorizontalCellAlignment.Center;
            //        sheet.Rows[cellRow - 1].Cells[colTotal].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //        sheet.Rows[cellRow - 1].Cells[colTotal].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //        sheet.Rows[cellRow - 1].Cells[colTotal].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //        BorderStyle(region, CellBorderLineStyle.Thin, LightGray);

            //        //TODO
            //        //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
            //        //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        //{
            //        //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotal);
            //        //}
            //        int nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
            //        int nbSpace = (nbtot - 1) / 3;
            //        int nbCharTotal = nbtot + nbSpace - 5;
            //        nbColTabFirst++;
            //    }
            //    else
            //    {
            //        colTotal = 0;
            //        colPdm = 2;
            //        colVersion = 2;
            //        colInsertion = 2;
            //        colFirstMediaPlan = 2;
            //        colTotalYears = 2;
            //    }
            //    #endregion

            //    #region PDM Column
            //    if (_allowPdm)
            //    {
            //        colVersion++;
            //        colInsertion++;
            //        colFirstMediaPlan++;
            //        colTotalYears++;
            //        region = sheet.MergedCellsRegions.Add(cellRow - 1, colPdm, cellRow - 1 + rowSpanNb - 1, colPdm + labColSpan - 1);
            //        sheet.Rows[cellRow - 1].Cells[colPdm].Value = GestionWeb.GetWebWord(806, _session.SiteLanguage);

            //        sheet.Rows[cellRow - 1].Cells[colPdm].CellFormat.Alignment = HorizontalCellAlignment.Center;
            //        sheet.Rows[cellRow - 1].Cells[colPdm].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //        sheet.Rows[cellRow - 1].Cells[colPdm].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //        sheet.Rows[cellRow - 1].Cells[colPdm].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //        BorderStyle(region, CellBorderLineStyle.Thin, LightGray);

            //        //TODO
            //        //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colPdm);
            //        //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        //{
            //        //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colPdm);
            //        //}
            //        nbColTabFirst++;
            //    }
            //    else
            //    {
            //        colPdm = 0;
            //    }
            //    #endregion

            //    #region Total Years
            //    if (nbColYear > 0 && _allowTotal)
            //    {
            //        int nbAddCol = 1;
            //        if (nbColYear != 0)
            //            nbAddCol = nbColYear;
            //        colFirstMediaPlan += nbAddCol;
            //        // Years necessary if the period consists of several years
            //        for (int k = firstPeriodIndex - nbColYear, l = 0; k < firstPeriodIndex; k++, l++)
            //        {
            //            region = sheet.MergedCellsRegions.Add(cellRow - 1, colTotalYears + l, cellRow - 1 + rowSpanNb - 1, colTotalYears + l + labColSpan - 1);
            //            //TODO
            //            //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colTotalYears + l);
            //            //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //            //{
            //            //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotalYears + l);
            //            //}
            //            sheet.Rows[cellRow - 1].Cells[colTotalYears + l].Value = data[0, k];

            //            sheet.Rows[cellRow - 1].Cells[colTotalYears + l].CellFormat.Alignment = HorizontalCellAlignment.Center;
            //            sheet.Rows[cellRow - 1].Cells[colTotalYears + l].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //            sheet.Rows[cellRow - 1].Cells[colTotalYears + 1].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //            sheet.Rows[cellRow - 1].Cells[colTotalYears + 1].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //            BorderStyle(region, CellBorderLineStyle.Thin, LightGray);

            //            nbColTabFirst++;
            //        }

            //    }
            //    else
            //    {
            //        colTotalYears = 0;
            //    }
            //    #endregion

            //    #region Period
            //    nbPeriod = 0;
            //    int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //    int lastPeriod = prevPeriod;
            //    bool first = true;
            //    string periodStyle = string.Empty;
            //    switch (_period.PeriodDetailLEvel)
            //    {
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
            //            prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            //            for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
            //            {
            //                if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
            //                {
            //                    sheet.MergedCellsRegions.Add(startIndex - 1, nbColTabFirst + 1, startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);
            //                    if (nbPeriod < 3)
            //                        sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = "";

            //                    else
            //                        sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = prevPeriod;

            //                    sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                    sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                    BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderLineStyle.Thin, LightGray);

            //                    //TODO
            //                    //styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
            //                    nbColTabFirst += nbPeriod;
            //                    nbPeriod = 0;
            //                    prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

            //                }

            //                switch (_period.PeriodDetailLEvel)
            //                {
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

            //                        #region Period Color Management
            //                        // First Period or last period is incomplete
            //                        periodStyle = "MediaPlanCellPeriod";
            //                        if ((j == firstPeriodIndex && _period.Begin.Day != 1)
            //                           || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
            //                        {
            //                            periodStyle = "MediaPlanCellPeriodIncomplete";
            //                        }
            //                        #endregion

            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                        BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);

            //                        break;
            //                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

            //                        #region Period Color Management
            //                        periodStyle = "MediaPlanCellPeriod";
            //                        if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
            //                           || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
            //                        {
            //                            periodStyle = "MediaPlanCellPeriodIncomplete";
            //                        }
            //                        #endregion

            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                        sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                        BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);

            //                        break;

            //                }
            //                nbPeriod++;
            //                nbPeriodTotal++;
            //            }
            //            // Compute last date
            //            sheet.MergedCellsRegions.Add(startIndex - 1, nbColTabFirst + 1, startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);
            //            if (nbPeriod < 3)
            //                sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = "";
            //            else
            //                sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = prevPeriod;

            //            sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //            sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //            BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderLineStyle.Thin, LightGray);

            //            //TODO
            //            //for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
            //            //{
            //            //    //TODO
            //            //    //cells[startIndex - 1, k].Style.Number = 1;
            //            //    //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            //            //    //    cells[startIndex, k].Style.Number = 1;
            //            //}

            //            break;
            //        case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
            //            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
            //            prevPeriod = currentDay.Month;
            //            currentDay = currentDay.AddDays(-1);
            //            for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
            //            {
            //                currentDay = currentDay.AddDays(1);
            //                if (currentDay.Month != prevPeriod)
            //                {
            //                    region = sheet.MergedCellsRegions.Add(startIndex - 1, nbColTabFirst + 1, startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);
            //                    if (nbPeriod >= 8)
            //                        sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
            //                    else
            //                        sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = "";

            //                    sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                    sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                    BorderStyle(region, CellBorderLineStyle.Thin, LightGray);

            //                    //TODO
            //                    //styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
            //                    nbColTabFirst += nbPeriod;
            //                    nbPeriod = 0;
            //                    prevPeriod = currentDay.Month;
            //                }
            //                nbPeriod++;
            //                nbPeriodTotal++;
            //                //Period Number
            //                sheet.Rows[startIndex].Cells[currentColMediaPlan].Value = currentDay.ToString("dd");
            //                sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Alignment = HorizontalCellAlignment.Center;

            //                sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                sheet.Rows[startIndex].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);

            //                //WorkSheet.PutCellValue(excel, cells, currentDay.ToString("dd"), startIndex, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPeriod", null, styleExcel);
            //                //Period day
            //                if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
            //                    sheet.Rows[startIndex + 1].Cells[currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);
            //                //WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDayWE", null, styleExcel);
            //                else
            //                    sheet.Rows[startIndex + 1].Cells[currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);
            //                //WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDay", null, styleExcel);

            //                sheet.Rows[startIndex + 1].Cells[currentColMediaPlan].CellFormat.Alignment = HorizontalCellAlignment.Center;

            //                sheet.Rows[startIndex + 1].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                sheet.Rows[startIndex + 1].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //                BorderStyle(sheet, startIndex + 1, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //            }


            //            sheet.MergedCellsRegions.Add(startIndex - 1, nbColTabFirst + 1, startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);
            //            if (nbPeriod >= 8)
            //                sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
            //            //WorkSheet.PutCellValue(excel, cells, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);
            //            else
            //                sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].Value = "";
            //            //WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);

            //            sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //            sheet.Rows[startIndex - 1].Cells[nbColTabFirst + 1].CellFormat.Fill = CellFill.CreateSolidFill(Black);
            //            BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderLineStyle.Thin, LightGray);

            //            //TODO
            //            //for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
            //            //{
            //            //    cells[startIndex, k].Style.Number = 1;
            //            //}


            //            break;

            //    }
            //    #endregion

            //    #endregion

            //    #region init Row Media Shedule
            //    cellRow++;

            //    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            //        cellRow++;
            //    #endregion

            //    #region Media Schedule
            //    int i = -1;
            //    try
            //    {
            //        first = true;
            //        nbColTabCell = colFirstMediaPlan;
            //        int currentColMediaPlan = 0;
            //        for (i = 1; i < nbline; i++)
            //        {

            //            #region Color Management
            //            if (sloganIndex != -1 && data[i, sloganIndex] != null &&
            //                ((_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == _session.GenericMediaDetailLevel.GetNbLevels) ||
            //                (_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < _session.GenericMediaDetailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
            //            {
            //                sloganId = Convert.ToInt64(data[i, sloganIndex]);
            //                if (!_session.SloganColors.ContainsKey(sloganId))
            //                {
            //                    colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
            //                    _session.SloganColors.Add(sloganId, "MediaPlanCellVersions" + colorNumberToUse);
            //                    colorItemIndex++;
            //                }

            //                presentstyle = _session.SloganColors[sloganId].ToString();
            //                extendedStyle = _session.SloganColors[sloganId].ToString();
            //                stringItem = "x";
            //            }
            //            else
            //            {
            //                presentstyle = "MediaPlanCellPresent";
            //                extendedStyle = "MediaPlanCellExtended";
            //                stringItem = "";
            //            }
            //            #endregion

            //            #region Line Treatement
            //            currentColMediaPlan = colFirstMediaPlan;
            //            for (int j = 0; j < nbColTab; j++)
            //            {
            //                switch (j)
            //                {
            //                    #region Level 1
            //                    case L1_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {

            //                            if (data[i, j].GetType() == typeof(MemoryArrayEnd))
            //                            {
            //                                i = int.MaxValue - 2;
            //                                j = int.MaxValue - 2;
            //                                break;
            //                            }

            //                            #region Style define
            //                            if (i == TOTAL_LINE_INDEX)
            //                            {
            //                                style = "MediaPlanCellLevelTotal";
            //                                styleNb = "MediaPlanCellLevelTotalNb";
            //                                stylePdmNb = "MediaPlanCellLevelTotalPdmNb";
            //                            }
            //                            else
            //                            {
            //                                style = "MediaPlanCellLevelL1";
            //                                styleNb = "MediaPlanCellLevelL1Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL1PdmNb";
            //                            }
            //                            #endregion

            //                            #region Label
            //                            sheet.Rows[cellRow].Cells[colSupport].Value = data[i, j].ToString();


            //                            if (i == TOTAL_LINE_INDEX)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                sheet.Rows[cellRow].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(Cyan);
            //                                BorderStyle(sheet, cellRow, colSupport, CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            else
            //                            {
            //                                sheet.Rows[cellRow].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                sheet.Rows[cellRow].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                BorderStyle(sheet, cellRow, colSupport, CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            #endregion

            //                            #region Total
            //                            if (_allowTotal)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);

            //                                if (i == TOTAL_LINE_INDEX)
            //                                {
            //                                    sheet.Rows[cellRow].Cells[colTotal].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                    sheet.Rows[cellRow].Cells[colTotal].CellFormat.Fill = CellFill.CreateSolidFill(Cyan);
            //                                    BorderStyle(sheet, cellRow, colTotal, CellBorderLineStyle.Thin, DarkGray);
            //                                }
            //                                else
            //                                {
            //                                    sheet.Rows[cellRow].Cells[colTotal].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                    sheet.Rows[cellRow].Cells[colTotal].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                    BorderStyle(sheet, cellRow, colTotal, CellBorderLineStyle.Thin, DarkGray);
            //                                }

            //                            }
            //                            #endregion

            //                            #region PDM
            //                            if (_allowPdm)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]);

            //                                if (i == TOTAL_LINE_INDEX)
            //                                {
            //                                    sheet.Rows[cellRow].Cells[colPdm].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                    sheet.Rows[cellRow].Cells[colPdm].CellFormat.Fill = CellFill.CreateSolidFill(Cyan);
            //                                    BorderStyle(sheet, cellRow, colPdm, CellBorderLineStyle.Thin, DarkGray);
            //                                }
            //                                else
            //                                {
            //                                    sheet.Rows[cellRow].Cells[colPdm].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                    sheet.Rows[cellRow].Cells[colPdm].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                    BorderStyle(sheet, cellRow, colPdm, CellBorderLineStyle.Thin, DarkGray);
            //                                }
            //                            }


            //                            #endregion

            //                            #region Totals years
            //                            for (int k = 1; k <= nbColYear && _allowTotal; k++)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 1) + k]);

            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            #endregion

            //                            j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 2
            //                    case L2_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {

            //                            #region Style define
            //                            if (premier)
            //                            {
            //                                style = "MediaPlanCellLevelL2_1";
            //                                styleNb = "MediaPlanCellLevelL2_1Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL2_1PdmNb";
            //                            }
            //                            else
            //                            {
            //                                style = "MediaPlanCellLevelL2_2";
            //                                styleNb = "MediaPlanCellLevelL2_2Nb";
            //                                stylePdmNb = "MediaPlanCellLevelL2_2PdmNb";
            //                            }
            //                            #endregion

            //                            #region Label
            //                            sheet.Rows[cellRow].Cells[colSupport].Value = data[i, j].ToString();
            //                            //WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null, styleExcel);
            //                            sheet.Rows[cellRow].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                            sheet.Rows[cellRow].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                            BorderStyle(sheet, cellRow, colSupport, CellBorderLineStyle.Thin, DarkGray);
            //                            #endregion

            //                            #region Total
            //                            if (_allowTotal)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);
            //                                //WorkSheet.PutCellValue(excel, cells, ((double)data[i, TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal, styleExcel);

            //                                sheet.Rows[cellRow].Cells[colTotal].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                sheet.Rows[cellRow].Cells[colTotal].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                BorderStyle(sheet, cellRow, colTotal, CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            #endregion

            //                            #region PDM
            //                            if (_allowPdm)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]);
            //                                //WorkSheet.PutCellValue(excel, cells, ((double)data[i, PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm, styleExcel);
            //                                sheet.Rows[cellRow].Cells[colPdm].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                sheet.Rows[cellRow].Cells[colPdm].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                BorderStyle(sheet, cellRow, colPdm, CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            #endregion

            //                            #region Totals years
            //                            for (int k = 1; k <= nbColYear && _allowTotal; k++)
            //                            {
            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]);
            //                                //WorkSheet.PutCellValue(excel, cells, ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal, styleExcel);
            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                sheet.Rows[cellRow].Cells[colTotalYears + (k - 1)].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                                BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderLineStyle.Thin, DarkGray);
            //                            }
            //                            #endregion

            //                            premier = !premier;

            //                            j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 3
            //                    case L3_COLUMN_INDEX:
            //                        if (data[i, j] != null)
            //                        {
            //                            sheet.Rows[cellRow].Cells[colSupport].Value = data[i, j].ToString();
            //                            //WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL3", null, styleExcel);
            //                            sheet.Rows[cellRow].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                            sheet.Rows[cellRow].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                            BorderStyle(sheet, cellRow, colSupport, CellBorderLineStyle.Thin, DarkGray);

            //                            /*
            //                            AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
            //                            */
            //                            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                            {
            //                                for (int k = 1; k <= nbColYear; k++)
            //                                {
            //                                    //AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear-3) + k);
            //                                }
            //                            }
            //                            j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
            //                        }
            //                        break;
            //                    #endregion

            //                    #region Level 4
            //                    case L4_COLUMN_INDEX:
            //                        sheet.Rows[cellRow].Cells[colSupport].Value = data[i, j].ToString();
            //                        //WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL4", null, styleExcel);
            //                        sheet.Rows[cellRow].Cells[colSupport].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                        sheet.Rows[cellRow].Cells[colSupport].CellFormat.Fill = CellFill.CreateSolidFill(MiddleGray);
            //                        BorderStyle(sheet, cellRow, colSupport, CellBorderLineStyle.Thin, DarkGray);

            //                        //AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);
            //                        if (!WebApplicationParameters.UseComparativeMediaSchedule)
            //                        {
            //                            for (int k = 1; k <= nbColYear; k++)
            //                            {
            //                                //AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear-4) + k);
            //                            }
            //                        }
            //                        j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
            //                        break;
            //                    #endregion

            //                    #region Other
            //                    default:
            //                        if (data[i, j] == null)
            //                        {
            //                            sheet.Rows[cellRow].Cells[currentColMediaPlan].Value = "";
            //                            //WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);
            //                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(DarkGray);
            //                            BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);

            //                            currentColMediaPlan++;
            //                            break;
            //                        }
            //                        if (data[i, j].GetType() == typeof(MediaPlanItem))
            //                        {
            //                            switch (((MediaPlanItem)data[i, j]).GraphicItemType)
            //                            {
            //                                case DetailledMediaPlan.graphicItemType.present:
            //                                    if (_showValues)
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].Value = ((MediaPlanItem)data[i, j]).Unit;
            //                                        //WorkSheet.PutCellValue(excel, cells, ((MediaPlanItem)data[i, j]).Unit, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", formatTotal, styleExcel);

            //                                        if (i == TOTAL_LINE_INDEX)
            //                                        {
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(White);
            //                                            BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                        }
            //                                        else
            //                                        {
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Black);
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Orange);
            //                                            BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].Value = stringItem;
            //                                        //WorkSheet.PutCellValue(excel, cells, stringItem, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", null, styleExcel);

            //                                        if (i == TOTAL_LINE_INDEX)
            //                                        {
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Cyan);
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(White);
            //                                            BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                        }
            //                                        else
            //                                        {
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(Black);
            //                                            sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Orange);
            //                                            BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                        }
            //                                    }
            //                                    break;
            //                                case DetailledMediaPlan.graphicItemType.extended:
            //                                    sheet.Rows[cellRow].Cells[currentColMediaPlan].Value = "";
            //                                    //WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellExtended", null, styleExcel);

            //                                    if (i == TOTAL_LINE_INDEX)
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Cyan);
            //                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                    }
            //                                    else
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(DarkGray);
            //                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                    }

            //                                    break;
            //                                default:
            //                                    sheet.Rows[cellRow].Cells[currentColMediaPlan].Value = "";
            //                                    //WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);

            //                                    if (i == TOTAL_LINE_INDEX)
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(Cyan);
            //                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                    }
            //                                    else
            //                                    {
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Font.ColorInfo = new WorkbookColorInfo(White);
            //                                        sheet.Rows[cellRow].Cells[currentColMediaPlan].CellFormat.Fill = CellFill.CreateSolidFill(DarkGray);
            //                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderLineStyle.Thin, LightGray);
            //                                    }

            //                                    break;
            //                            }
            //                            currentColMediaPlan++;
            //                        }
            //                        break;
            //                        #endregion
            //                }
            //            }
            //            if (first)
            //            {
            //                first = !first;
            //                nbColTabCell += currentColMediaPlan - 1;
            //            }
            //            cellRow++;
            //            #endregion

            //        }
            //    }
            //    catch (System.Exception err)
            //    {
            //        //throw (new MediaScheduleException("Error i=" + i, err));
            //    }
            //    #endregion

            //    //#region Mise en forme de la page

            //    #region Ajustement de la taile des cellules en fonction du contenu                
            //    //sheet.AutoFitColumn(colSupport);
            //    AutoFit(sheet, colSupport, startIndex, data.GetLength(0));

            //    if (_allowTotal)
            //        AutoFit(sheet, colTotal, startIndex, data.GetLength(0));

            //    if (_allowPdm)
            //        AutoFit(sheet, colPdm, startIndex, data.GetLength(0));

            //    if (_showValues)
            //    {
            //        for (int c = colFirstMediaPlan; c <= (nbColTabCell + 1 - colFirstMediaPlan); c++)
            //        {
            //            AutoFit(sheet, c, startIndex, data.GetLength(0));
            //        }
            //    }
            //    #endregion

            //    //if (_session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.monthly)
            //    //{
            //    //    for (index = 0; index < 30; index++)
            //    //    {
            //    //        columnWidth += cells.GetColumnWidth((byte)index);
            //    //        if ((columnWidth < 124) && verif)
            //    //            indexLogo++;
            //    //        else
            //    //            verif = false;
            //    //    }
            //    //    upperLeftColumn = (int)indexLogo - 1;
            //    //    vPageBreaks = cells[cellRow, (int)indexLogo].Name;
            //    //    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), data.GetLength(0) + 3, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), styleExcel);
            //    //}
            //    //else
            //    //{
            //    //    if (nbColTabCell > 44)
            //    //    {
            //    //        upperLeftColumn = nbColTabCell - 4;
            //    //        vPageBreaks = cells[cellRow, nbColTabCell - 4].Name;
            //    //    }
            //    //    else
            //    //    {
            //    //        for (index = 0; index < 30; index++)
            //    //        {
            //    //            columnWidth += cells.GetColumnWidth((byte)index);
            //    //            if ((columnWidth < 124) && verif)
            //    //                indexLogo++;
            //    //            else
            //    //                verif = false;
            //    //        }
            //    //        upperLeftColumn = (int)indexLogo - 1;
            //    //        vPageBreaks = cells[cellRow, (int)indexLogo].Name;
            //    //    }

            //    //    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), ref s, upperLeftColumn, header.ToString(), styleExcel);
            //    //}
            //    //#endregion
            //}
            #endregion

            #region Aspose
            if (data.GetLength(0) != 0)
            {

                //document.ChangePalette(LightGray, 25);
                //document.ChangePalette(MiddleGray, 24);
                //document.ChangePalette(DarkGray, 23);
                //document.ChangePalette(Orange, 22);
                //document.ChangePalette(Cyan, 21);

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

                bool _allowTotal = true;
                bool _allowPdm = true;
                bool _showValues = false;

                #region Init Variables
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                UnitInformation unitInformation = _session.GetSelectedUnit();

                #region Excel
                //Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                //Worksheet sheet = document.Worksheets.Add("WorkSheet1");

                //Cells cells = sheet.Cells;
                string formatTotal = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(unitInformation.Format);
                string formatPdm = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(EXCEL_PATTERN_NAME_PERCENTAGE);

                bool premier = true;
                //int header = 1;
                string prevYearString = string.Empty;
                //int nbMaxRowByPage = 42;
                //int s = 1;
                int cellRow = 5;
                int startIndex = cellRow;
                //int upperLeftColumn = 10;
                //string vPageBreaks = "";
                //double columnWidth = 0, indexLogo = 0, index;
                //bool verif = true;
                int colSupport = 1;
                int colTotal = 2;
                int colPdm = 2;
                int colTotalYears = 2;
                int colVersion = 2;
                int colInsertion = 2;
                int colFirstMediaPlan = 2;

                int colorItemIndex = 1;
                int colorNumberToUse = 0;
                int sloganIndex = GetSloganIdIndex();
                Int64 sloganId = long.MinValue;
                string stringItem = "";
                //string presentstyle = string.Empty;
                //string extendedStyle = string.Empty;
                //string style = string.Empty;
                //string styleNb = string.Empty;
                //string stylePdmNb = string.Empty;
                #endregion

                int yearBegin = _period.Begin.Year;
                int yearEnd = _period.End.Year;
                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
                {
                    yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
                    yearEnd = new AtomicPeriodWeek(_period.End).Year;
                }
                int nbColYear = yearEnd - yearBegin;
                if (nbColYear > 0) nbColYear++;
                int firstPeriodIndex = 0;

                if (WebApplicationParameters.UseComparativeMediaSchedule)
                {
                    if (_session.ComparativeStudy)
                    {
                        firstPeriodIndex = EVOL_COLUMN_INDEX + 1;
                    }
                    else
                    {
                        firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
                    }
                }
                else
                {
                    firstPeriodIndex = L4_ID_COLUMN_INDEX + 1;
                }
                firstPeriodIndex += nbColYear;

                int nbColTab = data.GetLength(1);
                int nbPeriod = nbColTab - firstPeriodIndex - 1;
                int nbPeriodTotal = 0;
                int nbline = data.GetLength(0);
                int nbColTabFirst = 0;
                int nbColTabCell = 0;

                try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
                catch (System.Exception) { }
                oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

                int labColSpan = 1;
                #endregion

                #region Rappel de sélection
                /*if (_isExcelReport) {
                    if (_isCreativeDivisionMS) {
                        t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
                    }
                    else {
                        if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE) {
                            t.Append(FctExcel.GetLogo(_session));
                            if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA) {
                                t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
                            }
                            else {
                                t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
                            }
                        }
                        else {
                            t.Append(FctExcel.GetAppmLogo(_session));
                            t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474, _session.SiteLanguage)));
                        }
                    }
                }*/
                #endregion

                #region basic columns (product, total, PDM, years totals)
                int rowSpanNb = 3;
                if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                {
                    rowSpanNb = 2;
                }

                #region Title first column (Product Column)
                sheet.Cells.Merge(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                Range range = sheet.Cells.CreateRange(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                sheet.Cells[cellRow - 1, colSupport].Value = GestionWeb.GetWebWord(804, _session.SiteLanguage);
                TextStyle(sheet.Cells[cellRow - 1, colSupport], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                //TODO
                //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
                //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                //{
                //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colSupport);
                //}
                nbColTabFirst++;
                #endregion

                #region Total Column
                if (_allowTotal)
                {
                    colTotal = 2;
                    colPdm++;
                    colVersion++;
                    colInsertion++;
                    colTotalYears++;
                    colFirstMediaPlan++;
                    sheet.Cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    sheet.Cells[cellRow - 1, colTotal].Value = GestionWeb.GetWebWord(805, _session.SiteLanguage);

                    TextStyle(sheet.Cells[cellRow - 1, colTotal], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    //TODO
                    //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
                    //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    //{
                    //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotal);
                    //}
                    int nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
                    int nbSpace = (nbtot - 1) / 3;
                    int nbCharTotal = nbtot + nbSpace - 5;
                    nbColTabFirst++;
                }
                else
                {
                    colTotal = 0;
                    colPdm = 2;
                    colVersion = 2;
                    colInsertion = 2;
                    colFirstMediaPlan = 2;
                    colTotalYears = 2;
                }
                #endregion

                #region PDM Column
                if (_allowPdm)
                {
                    colVersion++;
                    colInsertion++;
                    colFirstMediaPlan++;
                    colTotalYears++;

                    sheet.Cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colPdm, rowSpanNb, labColSpan);

                    sheet.Cells[cellRow - 1, colPdm].Value = GestionWeb.GetWebWord(806, _session.SiteLanguage);

                    TextStyle(sheet.Cells[cellRow - 1, colPdm], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    //TODO
                    //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colPdm);
                    //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    //{
                    //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colPdm);
                    //}
                    nbColTabFirst++;
                }
                else
                {
                    colPdm = 0;
                }
                #endregion

                #region Total Years
                if (nbColYear > 0 && _allowTotal)
                {
                    int nbAddCol = 1;
                    if (nbColYear != 0)
                        nbAddCol = nbColYear;
                    colFirstMediaPlan += nbAddCol;
                    // Years necessary if the period consists of several years
                    for (int k = firstPeriodIndex - nbColYear, l = 0; k < firstPeriodIndex; k++, l++)
                    {
                        sheet.Cells.Merge(cellRow - 1, colTotalYears + l, rowSpanNb, labColSpan);
                        range = sheet.Cells.CreateRange(cellRow - 1, colTotalYears + l, rowSpanNb, labColSpan);

                        //TODO
                        //styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colTotalYears + l);
                        //if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                        //{
                        //    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotalYears + l);
                        //}
                        sheet.Cells[cellRow - 1, colTotalYears + l].Value = data[0, k];

                        TextStyle(sheet.Cells[cellRow - 1, colTotalYears + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                        nbColTabFirst++;
                    }

                }
                else
                {
                    colTotalYears = 0;
                }
                #endregion

                #region Period
                nbPeriod = 0;
                int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                int lastPeriod = prevPeriod;
                bool first = true;

                switch (_period.PeriodDetailLEvel)
                {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                        prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
                        {
                            if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
                            {
                                sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                                if (nbPeriod < 3)
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = "";

                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Thin, HeaderBorderTab);

                                //TODO
                                //styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);

                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }

                        // Compute last date                        
                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod < 3)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = "";
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Thin, HeaderBorderTab);

                        break;
                    case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                        DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
                        prevPeriod = currentDay.Month;
                        currentDay = currentDay.AddDays(-1);
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
                        {
                            currentDay = currentDay.AddDays(1);
                            if (currentDay.Month != prevPeriod)
                            {
                                sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                                range = sheet.Cells.CreateRange(startIndex - 1, nbColTabFirst + 1, startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);

                                if (nbPeriod >= 8)
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = "";

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            sheet.Cells[startIndex, currentColMediaPlan].Value = currentDay.ToString("dd");

                            TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);

                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);
                            else
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);

                            TextStyle(sheet.Cells[startIndex + 1, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex + 1, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);
                        }

                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod >= 8)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = "";

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Thin, HeaderBorderTab);

                        break;

                }
                #endregion

                #endregion

                #region init Row Media Shedule
                cellRow++;

                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    cellRow++;
                #endregion

                // Fige les entêtes de lignes et de colonnes
                sheet.FreezePanes(cellRow, colFirstMediaPlan, cellRow, colFirstMediaPlan);

                #region Media Schedule
                int i = -1;
                try
                {
                    first = true;
                    nbColTabCell = colFirstMediaPlan;
                    int currentColMediaPlan = 0;
                    for (i = 1; i < nbline; i++)
                    {

                        #region Color Management
                        if (sloganIndex != -1 && data[i, sloganIndex] != null &&
                            ((_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == _session.GenericMediaDetailLevel.GetNbLevels) ||
                            (_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < _session.GenericMediaDetailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
                        {
                            sloganId = Convert.ToInt64(data[i, sloganIndex]);
                            if (!_session.SloganColors.ContainsKey(sloganId))
                            {
                                colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
                                _session.SloganColors.Add(sloganId, "MediaPlanCellVersions" + colorNumberToUse);
                                colorItemIndex++;
                            }

                            stringItem = "x";
                        }
                        else
                        {
                            stringItem = "";
                        }
                        #endregion

                        #region Line Treatement
                        currentColMediaPlan = colFirstMediaPlan;
                        for (int j = 0; j < nbColTab; j++)
                        {
                            switch (j)
                            {
                                #region Level 1
                                case L1_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {

                                        if (data[i, j].GetType() == typeof(MemoryArrayEnd))
                                        {
                                            i = int.MaxValue - 2;
                                            j = int.MaxValue - 2;
                                            break;
                                        }

                                        #region Label
                                        sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();


                                        if (i == TOTAL_LINE_INDEX)
                                        {
                                            TextStyle(sheet.Cells[cellRow, colSupport], LTotalText, LTotalBackground);
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        }
                                        else
                                        {
                                            TextStyle(sheet.Cells[cellRow, colSupport], LTotalText, LTotalBackground);
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);
                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                            }
                                        }


                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                        {
                                            sheet.Cells[cellRow, colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 1) + k]);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + (k - 1)], L1Text, L1Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case L2_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        #region Label
                                        sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();

                                        TextStyle(sheet.Cells[cellRow, colSupport], L2Text, L2Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 1);
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);
                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                        {
                                            sheet.Cells[cellRow, colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + (k - 1)], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    //else
                                    //{
                                    //    //#region Label
                                    //    //TextStyle(sheet.Cells[cellRow, colSupport], L2Text, L2Background);
                                    //    //BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                    //    //#endregion

                                    //    #region Total
                                    //    if (_allowTotal)
                                    //    {
                                    //        sheet.Cells[cellRow, colTotal].Value = "toto";
                                    //        TextStyle(sheet.Cells[cellRow, colTotal], L2Text, L2Background);
                                    //        BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                    //    }
                                    //    #endregion

                                    //    #region PDM
                                    //    if (_allowPdm)
                                    //    {
                                    //        TextStyle(sheet.Cells[cellRow, colPdm], L2Text, L2Background);
                                    //        BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                    //    }
                                    //    #endregion

                                    //    #region Totals years
                                    //    for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                    //    {
                                    //        TextStyle(sheet.Cells[cellRow, colTotalYears + (k - 1)], L2Text, L2Background);
                                    //        BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderType.Thin, BorderTab);
                                    //    }
                                    //    #endregion
                                    //}
                                    break;
                                #endregion

                                #region Level 3
                                case L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();

                                        TextStyle(sheet.Cells[cellRow, colSupport], L3Text, L3Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 2);

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);
                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                        {
                                            sheet.Cells[cellRow, colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + (k - 1)], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        /*
                                        AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
                                        */
                                        //if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                        //{
                                        //    for (int k = 1; k <= nbColYear; k++)
                                        //    {
                                        //        //AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear-3) + k);
                                        //    }
                                        //}
                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    //else
                                    //{
                                    //    //TextStyle(sheet.Cells[cellRow, colSupport], L3Text, L3Background);
                                    //    //BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                    //}
                                    break;
                                #endregion

                                #region Level 4
                                case L4_COLUMN_INDEX:
                                    sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();

                                    TextStyle(sheet.Cells[cellRow, colSupport], L4Text, L4Background);
                                    BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                    SetIndentLevel(sheet.Cells[cellRow, colSupport], 3);

                                    #region Total
                                    if (_allowTotal)
                                    {
                                        sheet.Cells[cellRow, colTotal].Value = ((double)data[i, TOTAL_COLUMN_INDEX]);
                                        SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotal], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    #region PDM
                                    if (_allowPdm)
                                    {
                                        sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;
                                        SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                        SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colPdm], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    #region Totals years
                                    for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                    {
                                        sheet.Cells[cellRow, colTotalYears + (k - 1)].Value = ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]);

                                        TextStyle(sheet.Cells[cellRow, colTotalYears + (k - 1)], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotalYears + (k - 1), CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    //AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);
                                    //if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                    //{
                                    //    for (int k = 1; k <= nbColYear; k++)
                                    //    {
                                    //        //AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear-4) + k);
                                    //    }
                                    //}
                                    j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                    break;
                                #endregion

                                #region Other
                                default:
                                    if (data[i, j] == null)
                                    {
                                        sheet.Cells[cellRow, currentColMediaPlan].Value = "";

                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], TabText, TabBackground);
                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);

                                        currentColMediaPlan++;
                                        break;
                                    }
                                    if (data[i, j].GetType() == typeof(MediaPlanItem))
                                    {
                                        switch (((MediaPlanItem)data[i, j]).GraphicItemType)
                                        {
                                            case DetailledMediaPlan.graphicItemType.present:
                                                if (_showValues)
                                                {
                                                    sheet.Cells[cellRow, currentColMediaPlan].Value = ((MediaPlanItem)data[i, j]).Unit;
                                                    SetDecimalFormat(sheet.Cells[cellRow, currentColMediaPlan]);

                                                    if (i == TOTAL_LINE_INDEX)
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                    }
                                                }
                                                else
                                                {
                                                    sheet.Cells[cellRow, currentColMediaPlan].Value = stringItem;

                                                    if (i == TOTAL_LINE_INDEX)
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                    }
                                                }
                                                break;
                                            case DetailledMediaPlan.graphicItemType.extended:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = "";

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                }

                                                break;
                                            default:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = "";

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Thin, BorderTab);
                                                }

                                                break;
                                        }
                                        currentColMediaPlan++;
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        if (first)
                        {
                            first = !first;
                            nbColTabCell += currentColMediaPlan - 1;
                        }
                        cellRow++;
                        #endregion

                    }
                }
                catch (System.Exception err)
                {
                    //throw (new MediaScheduleException("Error i=" + i, err));
                }
                #endregion

                //#region Mise en forme de la page

                #region Ajustement de la taile des cellules en fonction du contenu 

                sheet.AutoFitColumns();
                //sheet.AutoFitColumn(colSupport);
                //AutoFit(sheet, colSupport, startIndex, data.GetLength(0));

                //if (_allowTotal)
                //    AutoFit(sheet, colTotal, startIndex, data.GetLength(0));

                //if (_allowPdm)
                //    AutoFit(sheet, colPdm, startIndex, data.GetLength(0));

                //if (_showValues)
                //{
                //    for (int c = colFirstMediaPlan; c <= (nbColTabCell + 1 - colFirstMediaPlan); c++)
                //    {
                //        AutoFit(sheet, c, startIndex, data.GetLength(0));
                //    }
                //}
                #endregion



                //sheet.PageSetup.Orientation = PageOrientationType.Landscape;
                //sheet.PageSetup.FitToPagesWide = 1;
                //sheet.PageSetup.FitToPagesTall = 1;

                //if (_session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.monthly)
                //{
                //    for (index = 0; index < 30; index++)
                //    {
                //        columnWidth += cells.GetColumnWidth((byte)index);
                //        if ((columnWidth < 124) && verif)
                //            indexLogo++;
                //        else
                //            verif = false;
                //    }
                //    upperLeftColumn = (int)indexLogo - 1;
                //    vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                //    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), data.GetLength(0) + 3, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), styleExcel);
                //}
                //else
                //{
                //    if (nbColTabCell > 44)
                //    {
                //        upperLeftColumn = nbColTabCell - 4;
                //        vPageBreaks = cells[cellRow, nbColTabCell - 4].Name;
                //    }
                //    else
                //    {
                //        for (index = 0; index < 30; index++)
                //        {
                //            columnWidth += cells.GetColumnWidth((byte)index);
                //            if ((columnWidth < 124) && verif)
                //                indexLogo++;
                //            else
                //                verif = false;
                //        }
                //        upperLeftColumn = (int)indexLogo - 1;
                //        vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                //    }

                //    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), ref s, upperLeftColumn, header.ToString(), styleExcel);
                //}
                //#endregion
            }

            #endregion

            string documentFileNameRoot;
            //documentFileNameRoot = string.Format("Document.{0}", excelFormat == WorkbookFormat.Excel97To2003 ? "xls" : "xlsx");
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";
            //document.SetCurrentFormat(excelFormat);
            //document.Save(Response.OutputStream);



            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));



            Response.End();

            return View();
        }

        private void AutoFit(Worksheet Sheet, int Column, int StartRow, int NbRows)
        {

            Sheet.AutoFitColumns();

            //int maxSize = 0;
            //for (int idxRow = StartRow; idxRow < NbRows + StartRow; idxRow++)
            //{
            //    if (Sheet.Rows[idxRow].Cells[Column].Value != null)
            //    {
            //        int size = Sheet.Rows[idxRow].Cells[Column].Value.ToString().Length;

            //        if (size > maxSize)
            //            maxSize = size;
            //    }
            //}

            //Sheet.Columns[Column].Width = maxSize * 256;
        }


        //private void BorderStyle(Worksheet sheet, int idxRow, int idxCol, CellBorderLineStyle borderLineStyle, Color color)
        //{

        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.RightBorderStyle = borderLineStyle;
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.TopBorderStyle = borderLineStyle;
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.LeftBorderStyle = borderLineStyle;
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.BottomBorderStyle = borderLineStyle;

        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.RightBorderColorInfo = new WorkbookColorInfo(color);
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.TopBorderColorInfo = new WorkbookColorInfo(color);
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.LeftBorderColorInfo = new WorkbookColorInfo(color);
        //    sheet.Rows[idxRow].Cells[idxCol].CellFormat.BottomBorderColorInfo = new WorkbookColorInfo(color);



        //}

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

        //private void BorderStyle(WorksheetMergedCellsRegion region, CellBorderLineStyle borderLineStyle, Color color)
        //{

        //    region.CellFormat.RightBorderStyle = borderLineStyle;
        //    region.CellFormat.TopBorderStyle = borderLineStyle;
        //    region.CellFormat.LeftBorderStyle = borderLineStyle;
        //    region.CellFormat.BottomBorderStyle = borderLineStyle;

        //    region.CellFormat.RightBorderColorInfo = new WorkbookColorInfo(color);
        //    region.CellFormat.TopBorderColorInfo = new WorkbookColorInfo(color);
        //    region.CellFormat.LeftBorderColorInfo = new WorkbookColorInfo(color);
        //    region.CellFormat.BottomBorderColorInfo = new WorkbookColorInfo(color);
        //}

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


            //style.ForegroundColor = Color.Aqua;

            ////style.BackgroundColor = Color.Red;

            //style.Pattern = BackgroundType.Solid;



            range.ApplyStyle(style, flag1);
        }

        private void TextStyle(Cell cell, TextAlignmentType horizontalAlignment, TextAlignmentType verticalAlignment, Color fontColor, Color foregroundColor)
        {
            TextStyle(cell, fontColor, foregroundColor);
            TextStyle(cell, horizontalAlignment, verticalAlignment);
        }
        private void TextStyle(Cell cell, TextAlignmentType horizontalAlignment, TextAlignmentType verticalAlignment)
        {
            Style style = cell.GetStyle();

            style.HorizontalAlignment = horizontalAlignment;
            style.VerticalAlignment = verticalAlignment;

            cell.SetStyle(style);
        }

        private void TextStyle(Cell cell, Color fontColor, Color foregroundColor)
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

        private void SetDecimalFormat(Cell cell)
        {
            Style style = cell.GetStyle();

            style.Number = 3;

            cell.SetStyle(style);
        }

        private void SetPercentFormat(Cell cell)
        {
            Style style = cell.GetStyle();

            style.Number = 10;

            cell.SetStyle(style);
        }

        private void SetIndentLevel(Cell cell, int indentLevel, bool isRight = false)
        {
            Style style = cell.GetStyle();

            if (isRight == true)
                style.HorizontalAlignment = TextAlignmentType.Right;

            style.IndentLevel = indentLevel;

            cell.SetStyle(style);
        }

    }
}