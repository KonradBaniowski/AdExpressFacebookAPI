﻿//using Infragistics.Documents.Excel;
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
using Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Helpers;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using FrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = CstWeb.Role.ADEXPRESS + "," + CstWeb.Role.ADSCOPE)]
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
        Color HeaderTabBackground = Color.FromArgb(128, 128, 128);
        Color HeaderTabText = Color.White;
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
        Color LTotalText = Color.White;

        Color TabBackground = Color.Black;
        Color TabText = Color.Black;
        Color BorderTab = Color.Black;

        Color PresentText = Color.Black;
        Color PresentBackground = Color.FromArgb(60, 107, 191);

        Color NotPresentText = Color.Black;
        Color NotPresentBackground = Color.White;

        Color ExtendedText = Color.Black;
        Color ExtendedBackground = Color.FromArgb(0, 200, 255);
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
                    return (FrameWorkResults.MediaSchedule.L1_ID_COLUMN_INDEX);
                case 2:
                    return (FrameWorkResults.MediaSchedule.L2_ID_COLUMN_INDEX);
                case 3:
                    return (FrameWorkResults.MediaSchedule.L3_ID_COLUMN_INDEX);
                case 4:
                    return (FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX);
                default:
                    return (-1);
            }
        }

        // GET: TestExport
        public ActionResult Index(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Export(false, null, zoomDate);

            return View();
        }

        public ActionResult ResultValue(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Export(true, null, zoomDate);

            return View();
        }

        public ActionResult ResultBrut(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            ExportBrut(zoomDate);

            return View();
        }

        public ActionResult ResultBrutAdnetTrack(string id, string level, string zoomDate, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            ExportAdnetTrack(id, level, zoomDate, idVehicle);

            return View();
        }

        public ActionResult ResultCreativeMS(string l, string m, string b, string e, string p, string c)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(i => i.Type == ClaimTypes.UserData).Select(s => s.Value).SingleOrDefault();
            int siteLanguage = Int32.Parse(SecurityHelper.Decrypt(l, SecurityHelper.CryptKey));
            string mediaTypeIds = SecurityHelper.Decrypt(m, SecurityHelper.CryptKey);
            int beginDate = Int32.Parse(SecurityHelper.Decrypt(b, SecurityHelper.CryptKey));
            int endDate = Int32.Parse(SecurityHelper.Decrypt(e, SecurityHelper.CryptKey));
            string productIds = SecurityHelper.Decrypt(p, SecurityHelper.CryptKey);
            string creativeIds = string.Empty;

            if (!string.IsNullOrEmpty(c))
                creativeIds = SecurityHelper.Decrypt(c, SecurityHelper.CryptKey);

            CreativeMediaScheduleRequest request = new CreativeMediaScheduleRequest { IdWebSession = idWebSession, SiteLanguage = siteLanguage, MediaTypeIds = mediaTypeIds, BeginDate = beginDate, EndDate = endDate, ProductIds = productIds, CreativeIds = creativeIds };

            Export(false, request);

            return View();
        }

        void ExportBrut(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            object[,] data = null;

            if (!string.IsNullOrEmpty(zoomDate))
                data = _mediaSchedule.GetMediaScheduleData(idWebSession, zoomDate, "", this.HttpContext);
            else
                data = _mediaSchedule.GetMediaScheduleData(idWebSession, this.HttpContext);
            
            _session = (WebSession)WebSession.Load(idWebSession);
            
            #region Period Detail
            MediaSchedulePeriod period;
            DateTime beginP;
            DateTime endP;
            if (!string.IsNullOrEmpty(zoomDate))
            {
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    beginP = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    endP = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    beginP = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    endP = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                beginP = FctUtilities.Dates.Max(beginP,
                    FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType));
                endP = FctUtilities.Dates.Min(endP,
                    FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType));

                _session.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(beginP, endP, ConstantePeriod.DisplayLevel.dayly, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(beginP, endP, ConstantePeriod.DisplayLevel.dayly);
            }
            else
            {
                beginP = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
                endP = FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && beginP < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    _session.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }

                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(beginP, endP, _session.DetailPeriod, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(beginP, endP, _session.DetailPeriod);

            }
            #endregion

            //period = new MediaSchedulePeriod(DateString.YYYYMMDDToDateTime(_session.PeriodBeginningDate), DateString.YYYYMMDDToDateTime(_session.PeriodEndDate), _session.DetailPeriod, _session.ComparativePeriodType);

            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            ExportAspose export = new ExportAspose();
            export.ExportSelection(document, _session, _detailSelectionService.GetDetailSelection(idWebSession));

            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1983, _session.SiteLanguage));
            sheet.IsGridlinesVisible = false;

            int nbLevel = 1;

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

                int yearBegin = period.Begin.Year;
                int yearEnd = period.End.Year;
                if (period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
                {
                    yearBegin = new AtomicPeriodWeek(period.Begin).Year;
                    yearEnd = new AtomicPeriodWeek(period.End).Year;
                }
                int nbColYear = yearEnd - yearBegin;
                if (nbColYear > 0) nbColYear++;
                int firstPeriodIndex = 0;

                if (WebApplicationParameters.UseComparativeMediaSchedule)
                {
                    if (_session.ComparativeStudy)
                    {
                        firstPeriodIndex = FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX + 1;
                    }
                    else
                    {
                        firstPeriodIndex = FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
                    }
                }
                else
                {
                    firstPeriodIndex = FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
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
                if (period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                {
                    rowSpanNb = 2;
                }

                for (int l = 1; l <= detailLevel.GetNbLevels; l++)
                {
                    sheet.Cells.Merge(cellRow - 1, colSupport + l - 1, rowSpanNb, labColSpan);
                    range = sheet.Cells.CreateRange(cellRow - 1, colSupport + l - 1, rowSpanNb, labColSpan);
                    sheet.Cells[cellRow - 1, colSupport + l - 1].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(detailLevel[l].WebTextId, _session.SiteLanguage));

                    TextStyle(sheet.Cells[cellRow - 1, colSupport + l - 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

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

                        DateTime begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(period.Begin.Date, period.ComparativePeriodType);
                        DateTime end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(period.End.Date, period.ComparativePeriodType);

                        sheet.Cells[cellRow - 1, colTotalComp].Value =
                            TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)
                            + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(end, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);

                        TextStyle(sheet.Cells[cellRow - 1, colTotalComp], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

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
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
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
                        sheet.Cells[cellRow - 1, colTotal].Value = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    else
                        sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));



                    TextStyle(sheet.Cells[cellRow - 1, colTotal], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

                    int nbtot = Units.ConvertUnitValueToString(data[1, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
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
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
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
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

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

                switch (period.PeriodDetailLEvel)
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
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }

                        // Compute last date                        
                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod < 3)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

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
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            sheet.Cells[startIndex, currentColMediaPlan].Value = currentDay.ToString("dd");

                            TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);
                            else
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);

                            TextStyle(sheet.Cells[startIndex + 1, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex + 1, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);
                        }

                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod >= 8)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                        break;

                }
                #endregion

                #endregion

                #region init Row Media Shedule
                cellRow++;

                if (period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
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

                    #region Get Max Level
                    for (i = 1; i < nbline; i++)
                    {
                        for (int j = 0; j < nbColTab; j++)
                        {
                            switch (j)
                            {
                                #region Level 1
                                case FrameWorkResults.MediaSchedule.L1_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                    }
                                    break;
                                    #endregion
                            }
                        }
                    }
                    #endregion

                    SetSetsOfColorByMaxLevel(nbLevel);


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
                                case FrameWorkResults.MediaSchedule.L1_COLUMN_INDEX:
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
                                                BorderStyle(sheet, cellRow, colLevel, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colLevel], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colLevel, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                                }
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);
                                            if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                            }
                                        }


                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX:
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
                                            BorderStyle(sheet, cellRow, colLevel, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);
                                            if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {                                       
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
                                            BorderStyle(sheet, cellRow, colLevel, CellBorderType.Hair, BorderTab);
                                        }

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);
                                            if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;
                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
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
                                case FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX:
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
                                        BorderStyle(sheet, cellRow, colLevel, CellBorderType.Hair, BorderTab);
                                    }

                                    #region Comparative
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                    {
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value,
                                                    _session.Unit);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                        }
                                        //PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdmComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                        }
                                    }
                                    #endregion

                                    #region Total
                                    if (_allowTotal)
                                    {
                                        sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);
                                        if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotal], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    #region PDM
                                    if (_allowPdm)
                                    {
                                        sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;
                                        SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                        SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colPdm], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    #region Totals years
                                    for (int k = 0; k < nbColYear && _allowTotal; k++)
                                    {
                                        if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotalYears + k], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
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
                                        sheet.Cells[cellRow, currentColMediaPlan].Value = null;

                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], TabText, TabBackground);
                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);

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
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                }
                                                else
                                                {
                                                    //sheet.Cells[cellRow, currentColMediaPlan].Value = stringItem;

                                                    if (i == TOTAL_LINE_INDEX)
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                }
                                                break;
                                            case DetailledMediaPlan.graphicItemType.extended:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = null;

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }

                                                break;
                                            default:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = "";

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
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
            //documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";
            documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";
            //document.SetCurrentFormat(excelFormat);
            //document.Save(Response.OutputStream);



            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));



            Response.End();


        }


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


        void Export(bool _showValues = false, CreativeMediaScheduleRequest request = null, string zoomDate = "")
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            object[,] data = null;
            GridResultResponse response;

            if (request == null)
            {
                if(!string.IsNullOrEmpty(zoomDate))
                    data = _mediaSchedule.GetMediaScheduleData(idWebSession,zoomDate, "", this.HttpContext);
                else
                    data = _mediaSchedule.GetMediaScheduleData(idWebSession, this.HttpContext);
            }
            else
            {
                response = _mediaSchedule.GetMediaScheduleCreativeData(request);
                if (response.Success)
                    data = response.Data;
            }
            _session = (WebSession)WebSession.Load(idWebSession);


           
            #region Period Detail
            MediaSchedulePeriod period;
            DateTime begin;
            DateTime end;
            if (!string.IsNullOrEmpty(zoomDate))
            {
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                begin = FctUtilities.Dates.Max(begin,
                    FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType));
                end = FctUtilities.Dates.Min(end,
                    FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType));

                _session.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

            }
            else
            {
                begin = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
                end = FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    _session.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }

                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod);

            }
            #endregion

            //MediaSchedulePeriod _period = new MediaSchedulePeriod(DateString.YYYYMMDDToDateTime(_session.PeriodBeginningDate), DateString.YYYYMMDDToDateTime(_session.PeriodEndDate), _session.DetailPeriod, _session.ComparativePeriodType);

            ExportResponse(_showValues, idWebSession, _session, data, period);
        }

        void ExportAdnetTrack(string id, string level, string zoomDate, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _session = (WebSession)WebSession.Load(idWebSession);

            if (!string.IsNullOrEmpty(idVehicle))
            {
                _session.AdNetTrackSelection = new AdNetTrackProductSelection((FrameWorkResults.AdNetTrackMediaSchedule.Type)int.Parse(level), int.Parse(id));
                _session.Save();
            }
            else
            {
                _mediaSchedule.SetProductLevel(idWebSession, Int64.Parse(id), int.Parse(level));
            }

            object[,] data = _mediaSchedule.GetMediaScheduleData(idWebSession, zoomDate, idVehicle, this.HttpContext);

            #region Period Detail
            MediaSchedulePeriod period;
            DateTime begin;
            DateTime end;
            if (!string.IsNullOrEmpty(zoomDate))
            {
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = FctUtilities.Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = FctUtilities.Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                begin = FctUtilities.Dates.Max(begin,
                    FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType));
                end = FctUtilities.Dates.Min(end,
                    FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType));

                _session.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

            }
            else
            {
                begin = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
                end = FctUtilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
                if (_session.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    _session.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }

                if (_session.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod, _session.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod);

            }
            #endregion

            ExportResponse(false, idWebSession, _session, data, period);
        }

        private void ExportResponse(bool _showValues, string idWebSession, WebSession _session, object[,] data, MediaSchedulePeriod _period)
        {
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            ExportAspose export = new ExportAspose();
            export.ExportSelection(document, _session, _detailSelectionService.GetDetailSelection(idWebSession));

            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1983, _session.SiteLanguage));
            sheet.IsGridlinesVisible = false;

            int nbLevel = 1;

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
                        firstPeriodIndex = FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX + 1;
                    }
                    else
                    {
                        firstPeriodIndex = FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
                    }
                }
                else
                {
                    firstPeriodIndex = FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
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
                BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

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
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
                      
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
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
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
                  

                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        sheet.Cells[cellRow - 1, colTotal].Value = TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - " + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    else
                        sheet.Cells[cellRow - 1, colTotal].Value = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));



                    TextStyle(sheet.Cells[cellRow - 1, colTotal], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

                    int nbtot = Units.ConvertUnitValueToString(data[1, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
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
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
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
                    BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
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
                        BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

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
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value = int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }

                        // Compute last date                        
                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod < 3)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

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
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            sheet.Cells[startIndex, currentColMediaPlan].Value = currentDay.ToString("dd");

                            TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);
                            else
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value = DayString.GetCharacters(currentDay, cultureInfo, 1);

                            TextStyle(sheet.Cells[startIndex + 1, currentColMediaPlan], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex + 1, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);
                        }

                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod >= 8)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center, TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                        break;

                }
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

                    #region Get Max Level
                    for (i = 1; i < nbline; i++)
                    {
                        for (int j = 0; j < nbColTab; j++)
                        {
                            switch (j)
                            {
                                #region Level 1
                                case FrameWorkResults.MediaSchedule.L1_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        if (nbLevel < FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX + 1) nbLevel = FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX + 1;
                                        j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                    }
                                    break;
                                #endregion
                            }
                        }
                    }
                    #endregion

                    SetSetsOfColorByMaxLevel(nbLevel);

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
                                case FrameWorkResults.MediaSchedule.L1_COLUMN_INDEX:
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
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                        }
                                        else
                                        {
                                            TextStyle(sheet.Cells[cellRow, colSupport], L1Text, L1Background);
                                            BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colTotalComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                                }
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], LTotalText, LTotalBackground);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, colPdmComp], L1Text, L1Background);
                                                    BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotal], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colPdm], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                            }
                                        }

                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colEvo], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colEvo, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colEvo], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colEvo, CellBorderType.Hair, BorderTab);
                                            }
                                        }

                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            //if (data[i, j + (firstPeriodIndex - nbColYear - 2) + k] != null)
                                            //    sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, j + (firstPeriodIndex - nbColYear - 2) + k], _session.Unit);
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value, _session.Unit);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);


                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, colTotalYears + k], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case FrameWorkResults.MediaSchedule.L2_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        #region Label
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                        TextStyle(sheet.Cells[cellRow, colSupport], L2Text, L2Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 1);
                                        #endregion

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L2Text, L2Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);
                                            else if(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);


                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region EVO
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colEvo], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colEvo, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            //if (data[i, j + (firstPeriodIndex - nbColYear - 2) + k] != null)
                                            //    sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, j + (firstPeriodIndex - nbColYear - 2) + k], _session.Unit);

                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value, _session.Unit);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L2Text, L2Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case FrameWorkResults.MediaSchedule.L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                        TextStyle(sheet.Cells[cellRow, colSupport], L3Text, L3Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                        SetIndentLevel(sheet.Cells[cellRow, colSupport], 2);

                                        #region Comparative
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                        {
                                            if (_allowTotal)
                                            {
                                                if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit);
                                                else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                                SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colTotalComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                            }
                                            //PDM
                                            if (_allowPdm)
                                            {
                                                if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                    sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                                SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                                SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                                TextStyle(sheet.Cells[cellRow, colPdmComp], L3Text, L3Background);
                                                BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                            else if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotal], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdm], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region EVO
                                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX] != null)
                                                if (!double.IsInfinity((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]))
                                                    sheet.Cells[cellRow, colEvo].Value = ((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                            SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colEvo], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colEvo, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion

                                        #region Totals years
                                        for (int k = 0; k < nbColYear && _allowTotal; k++)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value,
                                                    _session.Unit);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                                sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalYears + k], L3Text, L3Background);
                                            BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                        }
                                        #endregion


                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case FrameWorkResults.MediaSchedule.L4_COLUMN_INDEX:
                                    sheet.Cells[cellRow, colSupport].Value = WebUtility.HtmlDecode(data[i, j].ToString());

                                    TextStyle(sheet.Cells[cellRow, colSupport], L4Text, L4Background);
                                    BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                    SetIndentLevel(sheet.Cells[cellRow, colSupport], 3);

                                    #region Comparative
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                                    {
                                        if (_allowTotal)
                                        {
                                            if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value,
                                                    _session.Unit);
                                            else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colTotalComp].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit);

                                            SetDecimalFormat(sheet.Cells[cellRow, colTotalComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colTotalComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colTotalComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colTotalComp, CellBorderType.Hair, BorderTab);
                                        }
                                        //PDM
                                        if (_allowPdm)
                                        {
                                            if (data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX] != null)
                                                sheet.Cells[cellRow, colPdmComp].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]) / 100;

                                            SetPercentFormat(sheet.Cells[cellRow, colPdmComp]);
                                            SetIndentLevel(sheet.Cells[cellRow, colPdmComp], 1, true);

                                            TextStyle(sheet.Cells[cellRow, colPdmComp], L4Text, L4Background);
                                            BorderStyle(sheet, cellRow, colPdmComp, CellBorderType.Hair, BorderTab);
                                        }
                                    }
                                    #endregion

                                    #region Total
                                    if (_allowTotal)
                                    {
                                        if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit);
                                        else if (_session.Unit == CstWeb.CustomerSessions.Unit.duration && data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertDurationToString(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX]);
                                        else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colTotal].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotal]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotal], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotal], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotal, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    #region PDM
                                    if (_allowPdm)
                                    {
                                        if (data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX] != null)
                                            sheet.Cells[cellRow, colPdm].Value = ((double)data[i, FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX]) / 100;

                                        SetPercentFormat(sheet.Cells[cellRow, colPdm]);
                                        SetIndentLevel(sheet.Cells[cellRow, colPdm], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colPdm], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colPdm, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    #region EVO
                                    if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
                                    {
                                        if (data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX] != null)
                                            if (!double.IsInfinity((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]))
                                                sheet.Cells[cellRow, colEvo].Value = ((double)data[i, FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX]) / 100;

                                        SetPercentFormat(sheet.Cells[cellRow, colEvo]);
                                        SetIndentLevel(sheet.Cells[cellRow, colEvo], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colEvo], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colEvo, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    #region Totals years
                                    for (int k = 0; k < nbColYear && _allowTotal; k++)
                                    {
                                        if (isVersionNb && data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(((FwkWebRsltUI.CellIdsNumber)data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k]).Value,
                                                _session.Unit);

                                        else if (data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k] != null)
                                            sheet.Cells[cellRow, colTotalYears + k].Value = Units.ConvertUnitValue(data[i, FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX + k], _session.Unit);

                                        SetDecimalFormat(sheet.Cells[cellRow, colTotalYears + k]);
                                        SetIndentLevel(sheet.Cells[cellRow, colTotalYears + k], 1, true);

                                        TextStyle(sheet.Cells[cellRow, colTotalYears + k], L4Text, L4Background);
                                        BorderStyle(sheet, cellRow, colTotalYears + k, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion                               

                                    j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;

                                    break;
                                #endregion

                                #region Other
                                default:
                                    if (data[i, j] == null)
                                    {
                                        sheet.Cells[cellRow, currentColMediaPlan].Value = null;

                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], TabText, TabBackground);
                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);

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
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                }
                                                else
                                                {
                                                    sheet.Cells[cellRow, currentColMediaPlan].Value = stringItem;

                                                    if (i == TOTAL_LINE_INDEX)
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                    else
                                                    {
                                                        TextStyle(sheet.Cells[cellRow, currentColMediaPlan], PresentText, PresentBackground);
                                                        BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                    }
                                                }
                                                break;
                                            case DetailledMediaPlan.graphicItemType.extended:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = null;

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], ExtendedText, ExtendedBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }

                                                break;
                                            default:
                                                sheet.Cells[cellRow, currentColMediaPlan].Value = null;

                                                if (i == TOTAL_LINE_INDEX)
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
                                                }
                                                else
                                                {
                                                    TextStyle(sheet.Cells[cellRow, currentColMediaPlan], NotPresentText, NotPresentBackground);
                                                    BorderStyle(sheet, cellRow, currentColMediaPlan, CellBorderType.Hair, BorderTab);
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
            //documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";
            documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";
            //document.SetCurrentFormat(excelFormat);
            //document.Save(Response.OutputStream);



            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));



            Response.End();
        }

        private void ExportWithMultipleUnitResponse(bool _showValues, string idWebSession, WebSession _session, object[,] data,
            MediaSchedulePeriod _period)
        {
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);


            document.Worksheets.Clear();

            ExportAspose export = new ExportAspose();
            export.ExportSelection(document, _session, _detailSelectionService.GetDetailSelection(idWebSession));

            Worksheet sheet = document.Worksheets.Add(GestionWeb.GetWebWord(1983, _session.SiteLanguage));
            sheet.IsGridlinesVisible = false;

            int nbLevel = 1;

            #region Aspose

            if (data.GetLength(0) != 0)
            {
                #region ChangePalette

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


                #endregion

                #region Init Variables

                bool _allowTotal = true;
                bool _allowPdm = true;

                int yearBegin = _period.Begin.Year;
                int yearEnd = _period.End.Year;
                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
                {
                    yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
                    yearEnd = new AtomicPeriodWeek(_period.End).Year;
                }
                CultureInfo cultureInfo =
                    new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                var units = _session.GetSelectedUnits();

                bool hasGrp = _session.Grp || _session.Grp30S;

                //Units Indexes
                Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes =
                    new Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>>();

                List<CstWeb.CustomerSessions.Unit> adspendUnits = GetAdSpendsUnit();
                List<CstWeb.CustomerSessions.Unit> selectUnits = new List<CstWeb.CustomerSessions.Unit>();
                bool hasAdSpend = false;
                units.ForEach(u =>
                {
                    selectUnits.Add(u.Id);
                    if (hasGrp && adspendUnits.Contains(u.Id))
                    {
                        selectUnits.Add(CstWeb.CustomerSessions.Unit.grp);
                        hasAdSpend = true;
                    }
                });

                if (hasGrp && !hasAdSpend)
                {
                    selectUnits.Insert(0, CstWeb.CustomerSessions.Unit.grp);
                }

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
                int labColSpan = 1;
                int nbColTabFirst = 0;
                bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule &&
                                          _session.ComparativeStudy;
                //Units Indexes
                Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsExcelColumnIndexes =
                    new Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>>();
                //years Index
                var yearsExcelIndex = new Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<int, int>>();
                var yearsIndex = new Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<int, int>>();
                string pdmLabel = GestionWeb.GetWebWord(806, _session.SiteLanguage);


                #endregion

                int rowSpanNb = 3;
                if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                {
                    rowSpanNb = 2;
                }

                #region Title first column (Product Column)

                sheet.Cells.Merge(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                Range range = sheet.Cells.CreateRange(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                sheet.Cells[cellRow - 1, colSupport].Value =
                    WebUtility.HtmlDecode(GestionWeb.GetWebWord(804, _session.SiteLanguage));
                TextStyle(sheet.Cells[cellRow - 1, colSupport], TextAlignmentType.Center, TextAlignmentType.Center,
                    HeaderTabText, HeaderTabBackground);
                BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);



                #endregion

                #region Total Column

                // bool first = true;
                int currentExcelColumnIndex = 2;
                int currentRowIndex = cellRow - 1;

                selectUnits.ForEach(un =>
                {
                    var unitInformation = UnitsInformation.Get(un);
                    string unitLabel = GestionWeb.GetWebWord(unitInformation.WebTextId, _session.SiteLanguage);

                    var currentUnitExcelColumnIndexes = new Dictionary<string, int>();
                    string cellValue = string.Empty;

                    if (isComparativeStudy)
                    {
                        //Total comparative column
                        DateTime begin = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.Begin.Date,
                            _period.ComparativePeriodType);
                        DateTime end = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(_period.End.Date,
                            _period.ComparativePeriodType);

                        cellValue =
                            $"{unitLabel} - {TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)} - {TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(end, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)}";



                        range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                            cellValue);

                        currentUnitExcelColumnIndexes.Add(
                            FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX_KEY,
                            currentExcelColumnIndex);

                        currentExcelColumnIndex++;

                        //PDM of comparative period                                                                  
                        range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                            pdmLabel);
                        currentUnitExcelColumnIndexes.Add(
                            FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX_KEY,
                            currentExcelColumnIndex);

                        currentExcelColumnIndex++;
                    }

                    //Total  column                   

                    if (WebApplicationParameters.UseComparativeMediaSchedule &&
                        _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        cellValue =
                            $"{unitLabel} - {TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)} - {TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern)}";
                    else
                        cellValue = WebUtility.HtmlDecode(GestionWeb.GetWebWord(805, _session.SiteLanguage));

                    range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                        cellValue);
                    currentExcelColumnIndex++;


                    //PDM of selected period
                    range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                        pdmLabel);
                    currentUnitExcelColumnIndexes.Add(FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX_KEY,
                        currentExcelColumnIndex);

                    currentExcelColumnIndex++;

                    //Evolution
                    if (isComparativeStudy)
                    {
                        MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
                        cellValue = GestionWeb.GetWebWord(1212, _session.SiteLanguage);

                        range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                            cellValue);
                        currentUnitExcelColumnIndexes.Add(FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX_KEY,
                            currentExcelColumnIndex);

                        currentExcelColumnIndex++;
                    }



                    //Total years index
                    if (yearBegin != yearEnd)
                    {
                        var currentYearColumnIndexes = new Dictionary<int, int>();
                        for (int k = yearBegin; k <= yearEnd; k++)
                        {
                            currentYearColumnIndexes.Add(k, currentExcelColumnIndex);
                            cellValue = $"{unitLabel} - {k}";
                            range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb,
                                labColSpan,
                                cellValue);
                            currentExcelColumnIndex++;
                        }
                        yearsExcelIndex.Add(un, currentYearColumnIndexes);
                    }

                    if (un == CstWeb.CustomerSessions.Unit.grp && hasAdSpend && _session.SpendsGrp)
                    {
                        //Add Spend per grp column Index
                        currentUnitExcelColumnIndexes.Add(FrameWorkResults.MediaSchedule.SPEND_PER_GRP_COLUMN_INDEX_KEY,
                            currentExcelColumnIndex);
                        cellValue = (_session.Grp)
                            ? GestionWeb.GetWebWord(3152, _session.SiteLanguage)
                            : GestionWeb.GetWebWord(3157, _session.SiteLanguage);
                        range = AdHeaderCellValue(sheet, currentRowIndex, currentExcelColumnIndex, rowSpanNb, labColSpan,
                            cellValue);
                        currentExcelColumnIndex++;
                    }

                    unitsExcelColumnIndexes.Add(un, currentUnitExcelColumnIndexes);


                });


                #endregion

                #region Get result tab Indexes

                int currentColumnIndex = FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX;
                bool first = true;

                //Add columns indexes for each unit selected
                selectUnits.ForEach(u =>
                {

                    currentColumnIndex = AddUnitsColumnIndexes(currentColumnIndex, first, unitsColumnIndexes, u,
                        yearBegin, yearEnd, yearsIndex);

                    if (first && !yearsIndex.Any())
                    {

                        currentColumnIndex = isComparativeStudy
                            ? FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX
                            : FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX;
                        first = false;
                    }

                    if (u == CstWeb.CustomerSessions.Unit.grp && hasAdSpend && _session.SpendsGrp)
                    {
                        //Add Spend per grp column Index
                        unitsColumnIndexes[u].Add(FrameWorkResults.MediaSchedule.SPEND_PER_GRP_COLUMN_INDEX_KEY,
                            ++currentColumnIndex);
                    }
                });

                int firstPeriodIndex = currentColumnIndex + 1;
                nbColTabFirst = currentColumnIndex;

                int nbColTab = data.GetLength(1);
                int nbline = data.GetLength(0);
                int nbPeriod = nbColTab - firstPeriodIndex - 1;
                int nbPeriodTotal = 0;
                //int nbColTabFirst = 0;
                int nbColTabCell = 0;

                oMediaScheduleData.PeriodNb = (Int64) Math.Round((double) (nbColTab - firstPeriodIndex) / 7);

                #endregion

                #region Period

                nbPeriod = 0;
                int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                int lastPeriod = prevPeriod;

                switch (_period.PeriodDetailLEvel)
                {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                        prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan;
                            j < nbColTab;
                            j++, currentColMediaPlan++)
                        {
                            if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
                            {
                                sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                                if (nbPeriod < 3)
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center,
                                    TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair,
                                    HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value =
                                        MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)),
                                            cultureInfo, 1);

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center,
                                        TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair,
                                        HeaderBorderTab);

                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    sheet.Cells[startIndex, currentColMediaPlan].Value =
                                        int.Parse(data[0, j].ToString().Substring(4, 2));

                                    TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center,
                                        TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                    BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair,
                                        HeaderBorderTab);

                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }

                        // Compute last date                        
                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod < 3)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = prevPeriod;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center,
                            TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                        break;
                    case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                        DateTime currentDay = DateString.YYYYMMDDToDateTime((string) data[0, firstPeriodIndex]);
                        prevPeriod = currentDay.Month;
                        currentDay = currentDay.AddDays(-1);
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan;
                            j < nbColTab;
                            j++, currentColMediaPlan++)
                        {
                            currentDay = currentDay.AddDays(1);
                            if (currentDay.Month != prevPeriod)
                            {
                                sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                                range = sheet.Cells.CreateRange(startIndex - 1, nbColTabFirst + 1,
                                    startIndex - 1 + 1 - 1, nbColTabFirst + 1 + nbPeriod - 1);

                                if (nbPeriod >= 8)
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value =
                                        TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session,
                                            currentDay.AddDays(-1).ToString("yyyyMM"));
                                else
                                    sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                                TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center,
                                    TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                                BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);

                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            sheet.Cells[startIndex, currentColMediaPlan].Value = currentDay.ToString("dd");

                            TextStyle(sheet.Cells[startIndex, currentColMediaPlan], TextAlignmentType.Center,
                                TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);

                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value =
                                    DayString.GetCharacters(currentDay, cultureInfo, 1);
                            else
                                sheet.Cells[startIndex + 1, currentColMediaPlan].Value =
                                    DayString.GetCharacters(currentDay, cultureInfo, 1);

                            TextStyle(sheet.Cells[startIndex + 1, currentColMediaPlan], TextAlignmentType.Center,
                                TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                            BorderStyle(sheet, startIndex + 1, currentColMediaPlan, CellBorderType.Hair, HeaderBorderTab);
                        }

                        sheet.Cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);

                        if (nbPeriod >= 8)
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value =
                                TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session,
                                    currentDay.AddDays(-1).ToString("yyyyMM"));
                        else
                            sheet.Cells[startIndex - 1, nbColTabFirst + 1].Value = null;

                        TextStyle(sheet.Cells[startIndex - 1, nbColTabFirst + 1], TextAlignmentType.Center,
                            TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
                        BorderStyle(sheet, startIndex - 1, nbColTabFirst + 1, CellBorderType.Hair, HeaderBorderTab);

                        break;

                }

                #endregion

                #region init Row Media Shedule

                cellRow++;

                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    cellRow++;

                #endregion

                // Fige les entêtes de lignes et de colonnes
                sheet.FreezePanes(cellRow, colFirstMediaPlan, cellRow, colFirstMediaPlan);

                #region Media Schedule
                first = true;
                nbColTabCell = colFirstMediaPlan;
                int currentColMediaPlan = 0;

                #region Get Max Level
                GenericDetailLevel detailLevel = GetDetailsLevelSelected();
                int nbLevels = detailLevel.GetNbLevels;
                #endregion

                SetSetsOfColorByMaxLevel(nbLevel);

                for (int i = 1; i < nbline; i++)
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
                            case FrameWorkResults.MediaSchedule.L1_COLUMN_INDEX:
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
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                    }
                                    else
                                    {
                                        TextStyle(sheet.Cells[cellRow, colSupport], L1Text, L1Background);
                                        BorderStyle(sheet, cellRow, colSupport, CellBorderType.Hair, BorderTab);
                                    }
                                    #endregion

                                    selectUnits.ForEach(u =>
                                    {
                                        #region Comparative

                                        if (isComparativeStudy)
                                        {
                                            int totalComparativeColumnIndex =
                                                unitsColumnIndexes[u][FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                                            int totalExcelComparativeColumnIndex =
                                               unitsExcelColumnIndexes[u][FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];

                                            //Total column comparative 
                                            if (data[i, totalComparativeColumnIndex] != null)
                                            {
                                                if (u == CstWeb.CustomerSessions.Unit.versionNb)
                                                {
                                                    sheet.Cells[cellRow, totalExcelComparativeColumnIndex].Value =
                                                        Units.ConvertUnitValue(
                                                        ((FwkWebRsltUI.CellIdsNumber)
                                                            data[i, totalComparativeColumnIndex]).Value, _session.Unit);
                                                }
                                                else
                                                {
                                                    sheet.Cells[cellRow, totalExcelComparativeColumnIndex].Value = Units.ConvertUnitValue(data[i, totalComparativeColumnIndex], _session.Unit);
                                                }
                                            }
                                           

                                            SetDecimalFormat(sheet.Cells[cellRow, totalExcelComparativeColumnIndex]);
                                            SetIndentLevel(sheet.Cells[cellRow, totalExcelComparativeColumnIndex], 1, true);

                                            if (i == TOTAL_LINE_INDEX)
                                            {
                                                TextStyle(sheet.Cells[cellRow, totalExcelComparativeColumnIndex], LTotalText, LTotalBackground);
                                                BorderStyle(sheet, cellRow, totalExcelComparativeColumnIndex, CellBorderType.Hair, BorderTab);
                                            }
                                            else
                                            {
                                                TextStyle(sheet.Cells[cellRow, totalExcelComparativeColumnIndex], L1Text, L1Background);
                                                BorderStyle(sheet, cellRow, totalExcelComparativeColumnIndex, CellBorderType.Hair, BorderTab);
                                            }
                                        }
                                        #endregion

                                    });

                                }
                                break;
                                #endregion


                        }
                    }

                    #endregion
                    }

                #endregion
            }


            #endregion
        }

        private Range AdHeaderCellValue(Worksheet sheet, int currentRowIndex, int currentColumIndex, int rowSpanNb,
            int labColSpan, string cellValue)
        {
            Range range;
            sheet.Cells.Merge(currentRowIndex, currentColumIndex, rowSpanNb, labColSpan);
            range = sheet.Cells.CreateRange(currentRowIndex, currentColumIndex, rowSpanNb, labColSpan);

            sheet.Cells[currentRowIndex, currentColumIndex].Value = cellValue;

            TextStyle(sheet.Cells[currentRowIndex, currentColumIndex], TextAlignmentType.Center,
                TextAlignmentType.Center, HeaderTabText, HeaderTabBackground);
            BorderStyle(sheet, range, CellBorderType.Hair, HeaderBorderTab);
            return range;
        }

        private List<CstWeb.CustomerSessions.Unit> GetAdSpendsUnit()
        {
            List<CstWeb.CustomerSessions.Unit> adSpendsUnits = new List<CstWeb.CustomerSessions.Unit>();
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.euro);
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.usd);
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.tl);
            return adSpendsUnits;
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

        private int AddUnitsColumnIndexes(int currentColumnIndex, bool first, Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes
         , CstWeb.CustomerSessions.Unit unit, int beginYear, int endYear, Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<int, int>> yearsIndex)
        {
            var currentUnitColumnIndexes = new Dictionary<string, int>();
            bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy;

            //Add Total column Indexes
            currentColumnIndex = (first) ? FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX : ++currentColumnIndex;
            currentUnitColumnIndexes.Add(FrameWorkResults.MediaSchedule.TOTAL_COLUMN_INDEX_KEY, currentColumnIndex);

            //Add PDM column Indexes
            currentColumnIndex = (first) ? FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX : ++currentColumnIndex;
            currentUnitColumnIndexes.Add(FrameWorkResults.MediaSchedule.PDM_COLUMN_INDEX_KEY, currentColumnIndex);

            if (isComparativeStudy)
            {
                //Add Total Comparative column Indexes                      
                currentColumnIndex = (first) ? FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(FrameWorkResults.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX_KEY, currentColumnIndex);


                //Add PDM Comparative column Indexes
                currentColumnIndex = (first) ? FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(FrameWorkResults.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX_KEY, currentColumnIndex);

                //Add Evolution column Indexes
                currentColumnIndex = (first) ? FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX_KEY, currentColumnIndex);
            }
            if (beginYear != endYear)
            {
                var currentYearColumnIndexes = new Dictionary<int, int>();
                for (int k = beginYear; k <= endYear; k++)
                {
                    if (first && k == beginYear)
                    {
                        currentColumnIndex = (isComparativeStudy) ? 1 + FrameWorkResults.MediaSchedule.EVOL_COLUMN_INDEX : 1 + FrameWorkResults.MediaSchedule.L4_ID_COLUMN_INDEX;
                        currentYearColumnIndexes.Add(k, currentColumnIndex);
                    }
                    else
                    {
                        currentColumnIndex++;
                        currentYearColumnIndexes.Add(k, currentColumnIndex);
                    }

                }
                yearsIndex.Add(unit, currentYearColumnIndexes);
            }
            unitsColumnIndexes.Add(unit, currentUnitColumnIndexes);
            return currentColumnIndex;
        }

        /// <summary>
        /// Get Details level Selected (in WebSession)
        /// </summary>
        /// <returns></returns>
        private GenericDetailLevel GetDetailsLevelSelected()
        {          
                return (_session.GenericMediaDetailLevel);
           
        }

      
       


    }
}