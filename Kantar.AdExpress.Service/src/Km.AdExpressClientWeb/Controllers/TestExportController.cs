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
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpressI.MediaSchedule.Functions;
using TNS.FrameWork.Date;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstWeb = TNS.AdExpress.Constantes.Web;
using System.Net;
using FwkWebRsltUI = TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class TestExportController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaScheduleService _mediaSchedule;
        private IDetailSelectionService _detailSelectionService;
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
        public TestExportController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule, IDetailSelectionService detailSelectionService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _detailSelectionService = detailSelectionService;
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

            Export();

            return View();
        }

        public ActionResult ResultValue()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Export(true);

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            ExportBrut();

            return View();
        }

        void ExportBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _mediaSchedule.GetMediaScheduleData(idWebSession, this.HttpContext);

            _session = (WebSession)WebSession.Load(idWebSession);

            MediaSchedulePeriod _period = new MediaSchedulePeriod(DateString.YYYYMMDDToDateTime(_session.PeriodBeginningDate), DateString.YYYYMMDDToDateTime(_session.PeriodEndDate), _session.DetailPeriod, _session.ComparativePeriodType);

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            ExportAspose export = new ExportAspose();
            export.ExportSelection(document, _session, _detailSelectionService.GetDetailSelection(idWebSession));

            Worksheet sheet = document.Worksheets.Add("WorkSheet1");

            #region Aspose
            if (data.GetLength(0) != 0)
            {
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
                bool _showValues = true;

                #region Init Variables
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                UnitInformation unitInformation = _session.GetSelectedUnit();

                bool isVersionNb = (unitInformation.Id == CstWeb.CustomerSessions.Unit.versionNb) ? true : false;

                #region Excel                
                bool premier = true;
                string prevYearString = string.Empty;
                int cellRow = 5;
                int startIndex = cellRow;
                int colSupport = 1;
                int colTotal = 2;
                int colPdm = 2;
                int colTotalYears = 2;
                int colVersion = 2;
                int colInsertion = 2;
                int colFirstMediaPlan = 3;
                int colPdmComp = 2;
                int colTotalComp = 2;

                int sloganIndex = GetSloganIdIndex();
                string stringItem = "";
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

                Range range;

                oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

                GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

                int labColSpan = 1;
                #endregion

                #region basic columns (product, total, PDM, years totals)
                int rowSpanNb = 3;
                if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                {
                    rowSpanNb = 2;
                }

                for (int l = 1; l <= detailLevel.GetNbLevels; l++)
                {
                    sheet.Cells.Merge(cellRow - 1, colSupport + l - 1, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colSupport + l - 1, rowSpanNb, labColSpan);
                    sheet.Cells[cellRow - 1, colSupport + l - 1].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(detailLevel[l].WebTextId, _session.SiteLanguage));

                    TextStyle(sheet.Cells[cellRow - 1, colSupport + l - 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    nbColTabFirst++;
                }


                colTotal = nbColTabFirst;
                colPdm = nbColTabFirst;
                colTotalYears = nbColTabFirst;
                colVersion = nbColTabFirst;
                colInsertion = nbColTabFirst;
                colFirstMediaPlan = nbColTabFirst + 1;
                colPdmComp = nbColTabFirst;
                colTotalComp = nbColTabFirst;

                #region Total Column


                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    if (_allowTotal)
                    {
                        colPdmComp++;
                        colTotal++;
                        colPdm++;
                        colVersion++;
                        colInsertion++;
                        colTotalYears++;
                        colFirstMediaPlan++;
                        nbColTabFirst++;
                        colTotalComp++;

                        sheet.Cells.Merge(cellRow - 1, colTotalComp, rowSpanNb, labColSpan);
                        range = sheet.Cells.CreateRange(cellRow - 1, colTotalComp, rowSpanNb, labColSpan);

                        //MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();

                        DateTime begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.Begin.Date, _period.ComparativePeriodType);
                        DateTime end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.End.Date, _period.ComparativePeriodType);

                        sheet.Cells[cellRow - 1, colTotalComp].Value =
                            TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)
                            + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(end, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);

                        TextStyle(sheet.Cells[cellRow - 1, colTotalComp], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    }
                    //PDM
                    if (_allowPdm)
                    {
                        colTotal++;
                        colPdm++;
                        colVersion++;
                        colInsertion++;
                        colFirstMediaPlan++;
                        colTotalYears++;
                        nbColTabFirst++;
                        colPdmComp++;

                        sheet.Cells.Merge(cellRow - 1, colPdmComp, rowSpanNb, labColSpan);
                        range = sheet.Cells.CreateRange(cellRow - 1, colPdmComp, rowSpanNb, labColSpan);

                        sheet.Cells[cellRow - 1, colPdmComp].Value = GestionWeb.GetWebWord(806, _session.SiteLanguage);

                        TextStyle(sheet.Cells[cellRow - 1, colPdmComp], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
                    }
                }


                if (_allowTotal)
                {
                    //colTotal = 2;
                    colPdm++;
                    colVersion++;
                    colInsertion++;
                    colTotalYears++;
                    colFirstMediaPlan++;
                    nbColTabFirst++;
                    colTotal++;

                    sheet.Cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colTotal, rowSpanNb, labColSpan);

                    //sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));

                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        sheet.Cells[cellRow - 1, colTotal].Value = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    else
                        sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));



                    TextStyle(sheet.Cells[cellRow - 1, colTotal], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    int nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
                    int nbSpace = (nbtot - 1) / 3;
                    int nbCharTotal = nbtot + nbSpace - 5;
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
                    nbColTabFirst++;
                    colPdm++;

                    sheet.Cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colPdm, rowSpanNb, labColSpan);

                    sheet.Cells[cellRow - 1, colPdm].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(806, _session.SiteLanguage));

                    TextStyle(sheet.Cells[cellRow - 1, colPdm], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
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

                        sheet.Cells[cellRow - 1, colTotalYears + l].Value = data[0, k];

                        TextStyle(sheet.Cells[cellRow - 1, colTotalYears + l], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Thin, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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
                    string[] classifLabels = new string[detailLevel.GetNbLevels];
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
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());
                                        classifLabels[0] = WebUtility.HtmlDecode(data[i, j].ToString());

                                        for (int colLevel = colSupport; colLevel < colSupport + detailLevel.GetNbLevels; colLevel++)
                                        {

                                            if (colLevel != colSupport)
                                            {
                                                sheet.Cells[cellRow, colLevel].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, _session.SiteLanguage));
                                            }

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colLevel], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colLevel, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colLevel], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colLevel, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                                }
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);
                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], L1Text, L1Background);
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
                                                TextStyle(sheet.Cells[cellRow, colPdm], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                            }
                                        }


                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                            }
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
                                        //sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();
                                        classifLabels[1] = WebUtility.HtmlDecode(data[i, j].ToString());

                                        for (int colLevel = colSupport, level = 0; colLevel < colSupport + detailLevel.GetNbLevels; colLevel++, level++)
                                        {

                                            if (level <= 1)
                                            {
                                                sheet.Cells[cellRow, colLevel].Value = classifLabels[level];
                                            }
                                            else if (level > 1)
                                            {
                                                sheet.Cells[cellRow, colLevel].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, _session.SiteLanguage));
                                            }

                                            TextStyle(sheet.Cells[cellRow, colLevel], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colLevel, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);
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
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        //sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();
                                        classifLabels[2] = WebUtility.HtmlDecode(data[i, j].ToString());

                                        for (int colLevel = colSupport, level = 0; colLevel < colSupport + detailLevel.GetNbLevels; colLevel++, level++)
                                        {

                                            if (level <= 2)
                                            {
                                                sheet.Cells[cellRow, colLevel].Value = classifLabels[level];
                                            }
                                            else if (level > 2)
                                            {
                                                sheet.Cells[cellRow, colLevel].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, _session.SiteLanguage));
                                            }

                                            TextStyle(sheet.Cells[cellRow, colLevel], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colLevel, CellBorderType.Thin, BorderTab);
                                        }

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);
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
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
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
                                    break;
                                #endregion

                                #region Level 4
                                case L4_COLUMN_INDEX:
                                    //sheet.Cells[cellRow, colSupport].Value = data[i, j].ToString();

                                    classifLabels[3] = WebUtility.HtmlDecode(data[i, j].ToString());

                                    for (int colLevel = colSupport, level = 0; colLevel < colSupport + detailLevel.GetNbLevels; colLevel++, level++)
                                    {

                                        if (level <= 3)
                                        {
                                            sheet.Cells[cellRow, colLevel].Value = classifLabels[level];
                                        }
                                        else if (level > 3)
                                        {
                                            sheet.Cells[cellRow, colLevel].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(1401, _session.SiteLanguage));
                                        }

                                        TextStyle(sheet.Cells[cellRow, colLevel], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colLevel, CellBorderType.Thin, BorderTab);
                                    }

                                    #region Comparative
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                    {
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value,
                                                    _session.Unit);
                                            else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                        }
                                        //PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdmComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                        }
                                    }
                                    #endregion

                                    #region Total
                                    if (_allowTotal)
                                    {
                                        sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);
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
                                    for (int k = 0; k < nbColYear && _allowTotal; k++)
                                    {
                                        if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotalYears + k], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
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
                                    //if (data[i, j].GetType() == typeof(MediaPlanItem))
                                    if (data[i, j] is MediaPlanItem)
                                    {
                                        switch (((MediaPlanItem)data[i, j]).GraphicItemType)
                                        {
                                            case DetailledMediaPlan.graphicItemType.present:
                                                if (_showValues)
                                                {
                                                    //sheet.Cells[cellRow, currentColMediaPlan].Value = ((MediaPlanItem)data[i, j]).Unit;
                                                    SetDecimalFormat(sheet.Cells[cellRow, currentColMediaPlan]);
                                                    SetIndentLevel(sheet.Cells[cellRow, currentColMediaPlan], 1, true);

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
                                                    //sheet.Cells[cellRow, currentColMediaPlan].Value = stringItem;

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

                #endregion



             
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


        }

        void Export(bool _showValues = false)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _mediaSchedule.GetMediaScheduleData(idWebSession, this.HttpContext);

            _session = (WebSession)WebSession.Load(idWebSession);

            MediaSchedulePeriod _period = new MediaSchedulePeriod(DateString.YYYYMMDDToDateTime(_session.PeriodBeginningDate), DateString.YYYYMMDDToDateTime(_session.PeriodEndDate), _session.DetailPeriod, _session.ComparativePeriodType);

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            ExportAspose export = new ExportAspose();
            export.ExportSelection(document, _session, _detailSelectionService.GetDetailSelection(idWebSession));

            Worksheet sheet = document.Worksheets.Add("WorkSheet1");



            #region Aspose
            if (data.GetLength(0) != 0)
            {
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
                //bool _showValues = false;

                //_allowTotal = _allowPdm = ((!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.evaliantMobile && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.internet)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);

                #region Init Variables
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                UnitInformation unitInformation = _session.GetSelectedUnit();
                bool isVersionNb = (unitInformation.Id == CstWeb.CustomerSessions.Unit.versionNb) ? true : false;
                #region Excel

                bool premier = true;
                string prevYearString = string.Empty;
                int cellRow = 5;
                int cellRowStart = 5;
                int startIndex = cellRow;
                int colSupport = 1;
                int colTotal = 2;
                int colPdm = 2;
                int colTotalComp = 2;
                int colPdmComp = 2;
                int colEvo = 2;
                int colTotalYears = 2;
                int colVersion = 2;
                int colInsertion = 2;
                int colFirstMediaPlan = 2;

                int sloganIndex = GetSloganIdIndex();
                string stringItem = "";
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
                sheet.Cells[cellRow - 1, colSupport].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(804, _session.SiteLanguage));
                TextStyle(sheet.Cells[cellRow - 1, colSupport], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                nbColTabFirst++;
                #endregion

                #region Total Column

                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    if (_allowTotal)
                    {
                        colPdmComp++;
                        colTotal++;
                        colPdm++;
                        colEvo++;
                        colVersion++;
                        colInsertion++;
                        colTotalYears++;
                        colFirstMediaPlan++;
                        nbColTabFirst++;

                        sheet.Cells.Merge(cellRow - 1, colTotalComp, rowSpanNb, labColSpan);
                        range = sheet.Cells.CreateRange(cellRow - 1, colTotalComp, rowSpanNb, labColSpan);

                        //MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();

                        DateTime begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.Begin.Date, _period.ComparativePeriodType);
                        DateTime end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.End.Date, _period.ComparativePeriodType);

                        sheet.Cells[cellRow - 1, colTotalComp].Value =
                            TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)
                            + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(end, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);

                        TextStyle(sheet.Cells[cellRow - 1, colTotalComp], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                        ////int nbtot = FctWeb.Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX].ToString(), _session.Unit).Length;
                        //int nbtot;
                        //if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                        //    nbtot = Units.ConvertUnitValueToString(((CellIdsNumber)data[1, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit, fp).Length;
                        //else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                        //{
                        //    nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit, fp).Length;
                        //}
                        //else
                        //    nbtot = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[1, TOTAL_COMPARATIVE_COLUMN_INDEX])).Length;

                        //int nbSpace = (nbtot - 1) / 3;
                        //int nbCharTotal = nbtot + nbSpace - 5;
                        //if (nbCharTotal < 5) nbCharTotal = 0;
                        //for (int h = 0; h < nbCharTotal; h++)
                        //{
                        //    t.Append("&nbsp;");
                        //}
                        //t.Append("</td>");
                    }
                    //PDM
                    if (_allowPdm)
                    {
                        colTotal++;
                        colPdm++;
                        colEvo++;
                        colVersion++;
                        colInsertion++;
                        colFirstMediaPlan++;
                        colTotalYears++;
                        nbColTabFirst++;

                        sheet.Cells.Merge(cellRow - 1, colPdmComp, rowSpanNb, labColSpan);
                        range = sheet.Cells.CreateRange(cellRow - 1, colPdmComp, rowSpanNb, labColSpan);

                        sheet.Cells[cellRow - 1, colPdmComp].Value = GestionWeb.GetWebWord(806, _session.SiteLanguage);

                        TextStyle(sheet.Cells[cellRow - 1, colPdmComp], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
                    }
                }


                if (_allowTotal)
                {
                    //colTotal = 2;
                    colPdm++;
                    colEvo++;
                    colVersion++;
                    colInsertion++;
                    colTotalYears++;
                    colFirstMediaPlan++;
                    nbColTabFirst++;

                    sheet.Cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colTotal, rowSpanNb, labColSpan);

                    //sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));

                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        sheet.Cells[cellRow - 1, colTotal].Value = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    else
                        sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));



                    TextStyle(sheet.Cells[cellRow - 1, colTotal], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);

                    int nbtot = Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
                    int nbSpace = (nbtot - 1) / 3;
                    int nbCharTotal = nbtot + nbSpace - 5;
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
                    colEvo++;
                    colVersion++;
                    colInsertion++;
                    colFirstMediaPlan++;
                    colTotalYears++;
                    nbColTabFirst++;

                    sheet.Cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colPdm, rowSpanNb, labColSpan);

                    sheet.Cells[cellRow - 1, colPdm].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(806, _session.SiteLanguage));

                    TextStyle(sheet.Cells[cellRow - 1, colPdm], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
                }
                else
                {
                    colPdm = 0;
                }

                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                {
                    colVersion++;
                    colInsertion++;
                    colFirstMediaPlan++;
                    colTotalYears++;
                    nbColTabFirst++;

                    sheet.Cells.Merge(cellRow - 1, colEvo, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colEvo, rowSpanNb, labColSpan);

                    MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
                    sheet.Cells[cellRow - 1, colEvo].Value = GestionWeb.GetWebWord(1212, _session.SiteLanguage);

                    TextStyle(sheet.Cells[cellRow - 1, colEvo], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Thin, HeaderBorderTab);
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

                        sheet.Cells[cellRow - 1, colTotalYears + l].Value = data[0, k];

                        TextStyle(sheet.Cells[cellRow - 1, colTotalYears + l], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Thin, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Thin, HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
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
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());


                                        if (i == TOTAL_LINE_INDEX)
                                        {
                                            TextStyle(sheet.Cells[cellRow, colSupport], LTotalText, LTotalBackground);
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        }
                                        else
                                        {
                                            TextStyle(sheet.Cells[cellRow, colSupport], L1Text, L1Background);
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                                }
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, PDM_COLUMN_INDEX] != null)
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
                                                TextStyle(sheet.Cells[cellRow, colPdm], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                            }
                                        }

                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colEvo], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colEvo, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colEvo], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colEvo, CellBorderType.Thin, BorderTab);
                                            }
                                        }

                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            //if (data[i, j + (firstPeriodIndex - nbColYear - 2) + k] != null)
                                            //    sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, j + (firstPeriodIndex - nbColYear - 2) + k], _session.Unit);
                                            if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value, _session.Unit);
                                            else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);


                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                            }
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
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                        TextStyle(sheet.Cells[cellRow, colSupport], L2Text, L2Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 1);
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, PDM_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region EVO
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colEvo], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colEvo, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            //if (data[i, j + (firstPeriodIndex - nbColYear - 2) + k] != null)
                                            //    sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, j + (firstPeriodIndex - nbColYear - 2) + k], _session.Unit);

                                            if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value, _session.Unit);
                                            else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                        TextStyle(sheet.Cells[cellRow, colSupport], L3Text, L3Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 2);

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (data[i, TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, PDM_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region EVO
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colEvo], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colEvo, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value,
                                                    _session.Unit);
                                            else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                        }
                                        #endregion


                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case L4_COLUMN_INDEX:
                                    sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                    TextStyle(sheet.Cells[cellRow, colSupport], L4Text, L4Background);
                                    BorderStyle(sheet, cellRow, colSupport, CellBorderType.Thin, BorderTab);
                                    SetIndentLevel(sheet.Cells[cellRow, colSupport], 3);

                                    #region Comparative
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                    {
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX]).Value,
                                                    _session.Unit);
                                            else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Thin, BorderTab);
                                        }
                                        //PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdmComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Thin, BorderTab);
                                        }
                                    }
                                    #endregion

                                    #region Total
                                    if (_allowTotal)
                                    {
                                        if (isVersionNb && data[i, TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                        else if (data[i, TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, TOTAL_COLUMN_INDEX], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotal], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotal, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    #region PDM
                                    if (_allowPdm)
                                    {
                                        if (data[i, PDM_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, PDM_COLUMN_INDEX]) / 100;

                                        SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                        SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colPdm], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colPdm, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    #region EVO
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                    {
                                        if (data[i, EVOL_COLUMN_INDEX] != null)
                                            if (!double.IsInfinity((double)data[i, EVOL_COLUMN_INDEX]))
                                                sheet.Cells[cellRow, colEvo].Value = ((double)data[i, EVOL_COLUMN_INDEX]) / 100;

                                        SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                        SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colEvo], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colEvo, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion

                                    #region Totals years
                                    for (int k = 0; k < nbColYear && _allowTotal; k++)
                                    {
                                        if (isVersionNb && data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value,
                                                _session.Unit);

                                        else if (data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotalYears + k], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Thin, BorderTab);
                                    }
                                    #endregion                               

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
                                    if (data[i, j] is MediaPlanItem)
                                    {
                                        switch (((MediaPlanItem)data[i, j]).GraphicItemType)
                                        {
                                            case DetailledMediaPlan.graphicItemType.present:
                                                if (_showValues)
                                                {
                                                    if (isVersionNb)
                                                        sheet.Cells[cellRow, currentColMediaPlan].Value = Units.ConvertUnitValue(((MediaPlanItemIds)data[i, j]).IdsNumber.Value, _session.Unit);
                                                    else
                                                        sheet.Cells[cellRow, currentColMediaPlan].Value = Units.ConvertUnitValue(((MediaPlanItem)data[i, j]).Unit, _session.Unit);

                                                    SetDecimalFormat(sheet.Cells[cellRow, currentColMediaPlan]);    
                                                    SetIndentLevel(sheet.Cells[cellRow, currentColMediaPlan], 1, true);

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

                #region Ajoute les icones des cellules
                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                {
                    int idxCondis = sheet.ConditionalFormattings.Add();
                    FormatConditionCollection fcs = sheet.ConditionalFormattings[idxCondis];

                    CellArea cellArea = new CellArea();
                    cellArea.StartRow = cellRowStart;
                    cellArea.EndRow = cellRow;
                    cellArea.StartColumn = colEvo;
                    cellArea.EndColumn = colEvo;

                    fcs.AddArea(cellArea);

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

                #region Ajustement de la taile des cellules en fonction du contenu 

                sheet.AutoFitColumns();

                #endregion




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


        }

        private void AutoFit(Worksheet Sheet, int Column, int StartRow, int NbRows)
        {

            Sheet.AutoFitColumns();

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

        private void SetSelectionCallback()
        {
            //Choix de l'étude           
            var studychoice = GestionWeb.GetWebWord(842, _session.SiteLanguage);

            //Période sélectionée
            var startDate = _session.PeriodBeginningDate;
            var endDate = _session.PeriodEndDate;

            //période comparative
            var comparativePeriodLabel = GestionWeb.GetWebWord(2292, _session.SiteLanguage);

            if (_session.ComparativeStudy)
            {

                var comparativePeriod = GestionWeb.GetWebWord(1118, _session.SiteLanguage);
            }


            //Module
            var moduleLabel = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_session.CurrentModule), _session.SiteLanguage);

            // Détail Média
            if (_session.SelectionUniversMedia.FirstNode != null && _session.SelectionUniversMedia.FirstNode.Nodes.Count > 0)
            {

                System.Text.StringBuilder detailMedia = new System.Text.StringBuilder(1000);


                var detailmediaLabel = GestionWeb.GetWebWord(1194, _session.SiteLanguage);

                //Voir function =>
                // var mediaDetailText =
                //TNS.AdExpress.Web.Core.Utilities.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_session.SelectionUniversMedia.FirstNode, false, true, true, "100", true, false, _webSession.SiteLanguage, 2, i, true, _webSession.DataLanguage, dataSource) + "</TD>");


                #region Univers produit principal sélectionné
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                {
                    System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
                    int code = 0;
                    string nameProduct = "";
                    string productText = "";

                    if (_session.PrincipalProductUniverses.Count > 1)
                    {
                        code = 2302;
                    }
                    else
                    {

                        code = 1759;
                    }


                    for (int k = 0; k < _session.PrincipalProductUniverses.Count; k++)
                    {
                        if (_session.PrincipalProductUniverses.Count > 1)
                        {
                            if (_session.PrincipalProductUniverses.ContainsKey(k))
                            {
                                if (k > 0)
                                {
                                    nameProduct = GestionWeb.GetWebWord(2301, _session.SiteLanguage);
                                }
                                else
                                {
                                    nameProduct = GestionWeb.GetWebWord(2302, _session.SiteLanguage);
                                }

                                t.Append("<TR>");
                                t.Append("<TD class=\"txtViolet11Bold\" >&nbsp;");
                                t.Append("<label>" + nameProduct + "</label></TD>");
                                t.Append("</TR>");

                                // Universe Label
                                if (_session.PrincipalProductUniverses[k].Label != null && _session.PrincipalProductUniverses[k].Label.Length > 0)
                                {
                                    t.Append("<TR>");
                                    t.Append("<TD class=\"txtViolet11Bold\" >&nbsp;");
                                    t.Append("<Label>" + _session.PrincipalProductUniverses[k].Label + "</Label>");
                                    t.Append("</TD></TR>");
                                }

                                //Voir Function : ShowUniverse ===>
                                // Render universe html code
                                //t.Append("<TR height=\"20\">");
                                //t.Append("<TD vAlign=\"top\">" + selectItemsInClassificationWebControl.ShowUniverse(_session.PrincipalProductUniverses[k], _session.DataLanguage, DBFunctions.GetDataSource(_session)) + "</TD>");
                                //t.Append("</TR>");
                                //t.Append("<TR height=\"10\"><TD></TD></TR>");
                            }
                        }
                        else
                        {
                            if (_session.PrincipalProductUniverses.ContainsKey(k))
                            {
                                //Voir Function : ShowUniverse ===>
                                // productText += selectItemsInClassificationWebControl.ShowUniverse(_session.PrincipalProductUniverses[k], _session.DataLanguage, DBFunctions.GetDataSource(_session));
                            }
                        }
                    }

                }
                #endregion

                #region Univers support principal sélectionné
                if (_session.PrincipalMediaUniverses != null && _session.PrincipalMediaUniverses.Count > 0)
                {


                    var mediaSelectiondWebText = GestionWeb.GetWebWord(2540, _session.SiteLanguage);


                    //Voir fonction ==>
                    // string  mediaSelectiondText += selectItemsInClassificationWebControl.ShowUniverse(_webSession.PrincipalMediaUniverses[0], _webSession.DataLanguage, dataSource);
                }
                #endregion

                #region Type de pourcentage
                string percentageAlignmentLabel = string.Empty;
                switch (_session.PercentageAlignment)
                {
                    case CstWeb.Percentage.Alignment.vertical:
                        percentageAlignmentLabel = GestionWeb.GetWebWord(2065, _session.SiteLanguage);
                        break;
                    case CstWeb.Percentage.Alignment.horizontal:
                        percentageAlignmentLabel = GestionWeb.GetWebWord(2064, _session.SiteLanguage);
                        break;
                    default: break;
                }
                #endregion

            }

        }

    }
}