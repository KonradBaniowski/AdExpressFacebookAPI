using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.MediaSchedule.Exceptions;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.MediaSchedule.Turkey
{
    public class MediaScheduleResults : MediaSchedule.MediaScheduleResults
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Filter</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle) : base(session, period, idVehicle) { }
        /// <summary>
        /// Constructor of a Media Schedule on a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, string zoom) : base(session, period, zoom) { }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle and a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Id</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle, string zoom) : base(session, period, idVehicle, zoom) { }
        #endregion

        #region Compute Grid result
        protected override GridResult ComputeGridResult(object[,] data)
        {
            GridResult gridResult = new GridResult();
            MediaScheduleData oMediaScheduleData = new MediaScheduleData();
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            IFormatProvider fp =
                (!_isExcelReport || _isCreativeDivisionMS) ? WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo
                : WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfoExcel;

            #region No data
            if (data.GetLength(0) == 0)
            {
                gridResult.HasData = false;
                return (gridResult);
            }

            if (data.GetLength(0) > CstWeb.Core.MAX_ALLOWED_ROWS_NB)
            {
                gridResult.HasData = true;
                gridResult.HasMoreThanMaxRowsAllowed = true;
                return (gridResult);
            }
            #endregion

            #region Init Variables
            int yearBegin = _period.Begin.Year;
            int yearEnd = _period.End.Year;
            Int64 periodNb = 0;
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
            int nbline = data.GetLength(0);

            try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
            catch (System.Exception) { }
            periodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            bool isExport = _isExcelReport || _isPDFReport;
            int labColSpan = (isExport && !_allowTotal) ? 2 : 1;
            UnitInformation unitTmp = UnitsInformation.Get(_session.Unit);
            List<UnitInformation> units = _session.GetSelectedUnits();

            int nbLineGrid = 0;
            for (int r = 1; r < nbline; r++)
            {
                if (data[r, 0] != null && data[r, 0].GetType() == typeof(MemoryArrayEnd))
                    break;

                nbLineGrid++;
            }

            object[,] gridData = new object[nbLineGrid, data.GetLength(1)];
            #endregion

            #region Colonnes

            #region basic columns (product, total, PDM, version, insertion, years totals)
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();
            List<object> columnsNotAllowedSorting = new List<object>();

            int tableWidth = 0;
            string periodLabel = string.Empty;

            // Product Column
            columns.Add(new { headerText = "ID_PRODUCT", key = "ID_PRODUCT", dataType = "number", width = "350", hidden = true });
            schemaFields.Add(new { name = "ID_PRODUCT" });
            columns.Add(new { headerText = GestionWeb.GetWebWord(804, _session.SiteLanguage), key = "PRODUCT", dataType = "string", width = "350" });
            schemaFields.Add(new { name = "PRODUCT" });
            columnsFixed.Add(new { columnKey = "PRODUCT", isFixed = true, allowFixing = false });
            tableWidth = 350;

            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(_session.Unit).StringFormat);

            foreach (var unit in units)
            {
                tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth);
                tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth);
                tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth);
            }

            if (_session.Grp)
            {
                tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, "GRP", format, tableWidth);
                tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, "GRP", format, tableWidth);
                tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, "GRP", format, tableWidth);
            }
            else if (_session.Grp30S)
            {
                tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, "GRP 30s", format, tableWidth);
                tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, "GRP 30s", format, tableWidth);
                tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, "GRP 30s", format, tableWidth);
            }

            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            //Version
            if (_allowVersion)
            {
                columns.Add(new { headerText = GestionWeb.GetWebWord(1994, _session.SiteLanguage)[0], key = "VERSION", dataType = "string", width = "30" });
                schemaFields.Add(new { name = "VERSION" });
                columnsFixed.Add(new { columnKey = "VERSION", isFixed = false, allowFixing = false });
                columnsNotAllowedSorting.Add(new { columnKey = "VERSION", allowSorting = false });
                tableWidth += 30;
            }
            // Insertions
            if (_allowInsertions)
            {
                string insertionLabel = GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].WebTextId, _session.SiteLanguage);
                columns.Add(new { headerText = insertionLabel[0], key = "INSERTION", dataType = "string", width = "30" });
                schemaFields.Add(new { name = "INSERTION" });
                columnsFixed.Add(new { columnKey = "INSERTION", isFixed = false, allowFixing = false });
                columnsNotAllowedSorting.Add(new { columnKey = "INSERTION", allowSorting = false });
                tableWidth += 30;
            }
            #endregion

            #region Period
            nbPeriod = 0;
            int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            StringBuilder periods = new StringBuilder();
            StringBuilder headers = new StringBuilder();
            string link = string.Empty;
            link = "";
            List<object> periodColumnsL1 = new List<object>();
            List<object> periodColumnsL2 = new List<object>();

            switch (_period.PeriodDetailLEvel)
            {
                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                    prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                    for (int j = firstPeriodIndex; j < nbColTab; j++)
                    {
                        if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
                        {
                            if (nbPeriod < 3)
                                columns.Add(new { headerText = "", key = prevPeriod, group = periodColumnsL1 });
                            else
                                columns.Add(new { headerText = prevPeriod, key = prevPeriod, group = periodColumnsL1 });
                            columnsFixed.Add(new { columnKey = prevPeriod, isFixed = false, allowFixing = false });
                            columnsNotAllowedSorting.Add(new { columnKey = prevPeriod, allowSorting = false });
                            nbPeriod = 0;
                            prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));
                            periodColumnsL1 = new List<object>();
                        }

                        switch (_period.PeriodDetailLEvel)
                        {
                            case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                                string monthLabel = MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1);
                                string monthKey = "m" + data[0, j].ToString();
                                string monthLabelHtml = "<span class=\"open-media-schedule\" date=\"" + data[0, j].ToString() + "\" style=\"cursor:pointer;\">" + monthLabel + "</span>";
                                periodColumnsL1.Add(new { headerText = monthLabelHtml, key = monthKey, dataType = "string", width = "20", template = "<span {{if ${" + monthKey + "} == 1 }} class='blueTg' {{elseif ${" + monthKey + "} == 2 }} class='blueExtendedTg' {{elseif ${" + monthKey + "} == 3 }} class='blackTg' {{else}} class='${" + monthKey + "}' {{/if}} ></span>" });
                                schemaFields.Add(new { name = monthKey });
                                columnsFixed.Add(new { columnKey = monthKey, isFixed = false, allowFixing = false });
                                columnsNotAllowedSorting.Add(new { columnKey = monthKey, allowSorting = false });
                                tableWidth += 20;
                                break;
                            case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                                string weekLabel = data[0, j].ToString().Substring(4, 2);
                                string weekLabelHtml = "<span class=\"open-media-schedule\" date=\"" + data[0, j].ToString() + "\" style=\"cursor:pointer;\">" + weekLabel + "</span>";
                                if (!IsCreativeDivisionMS)
                                {
                                    periodColumnsL1.Add(new { headerText = weekLabelHtml, key = weekLabel, dataType = "string", width = "20", template = "<span {{if ${" + weekLabel + "} == 1 }} class='blueTg' {{elseif ${" + weekLabel + "} == 2 }} class='blueExtendedTg' {{elseif ${" + weekLabel + "} == 3 }} class='blackTg' {{else}} class='${" + weekLabel + "}' {{/if}} ></span>" });
                                    schemaFields.Add(new { name = weekLabel });
                                }
                                else
                                {
                                    periodColumnsL1.Add(new { headerText = weekLabelHtml, key = weekLabel, dataType = "string", width = "20", template = "<span {{if ${" + weekLabel + "} == 1 }} class='blueTg' {{elseif ${" + weekLabel + "} == 2 }} class='blueExtendedTg' {{elseif ${" + weekLabel + "} == 3 }} class='blackTg' {{else}} class='${" + weekLabel + "}' {{/if}} ></span>" });
                                    schemaFields.Add(new { name = weekLabel });
                                }
                                columnsFixed.Add(new { columnKey = weekLabel, isFixed = false, allowFixing = false });
                                columnsNotAllowedSorting.Add(new { columnKey = weekLabel, allowSorting = false });
                                tableWidth += 20;
                                break;

                        }
                        nbPeriod++;
                    }
                    // Compute last date
                    if (nbPeriod < 3)
                        columns.Add(new { headerText = "", key = prevPeriod, group = periodColumnsL1 });
                    else
                        columns.Add(new { headerText = prevPeriod, key = prevPeriod, group = periodColumnsL1 });

                    columnsFixed.Add(new { columnKey = prevPeriod, key = prevPeriod, isFixed = false, allowFixing = false });
                    columnsNotAllowedSorting.Add(new { columnKey = prevPeriod, allowSorting = false });
                    break;
                case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                    StringBuilder days = new StringBuilder();
                    periods.Append("<tr>");
                    days.Append("\r\n\t<tr>");
                    DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
                    prevPeriod = currentDay.Month;
                    currentDay = currentDay.AddDays(-1);
                    string periodDayLabel = string.Empty;
                    int globalHeaderKey = 9999;
                    for (int j = firstPeriodIndex; j < nbColTab; j++)
                    {
                        currentDay = currentDay.AddDays(1);
                        if (currentDay.Month != prevPeriod)
                        {
                            if (nbPeriod >= 8)
                            {
                                periodDayLabel = TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM"));
                                columns.Add(new { headerText = periodDayLabel, key = globalHeaderKey.ToString(), group = periodColumnsL1 });
                            }
                            else
                                columns.Add(new { headerText = "", key = globalHeaderKey.ToString(), group = periodColumnsL1 });
                            columnsFixed.Add(new { columnKey = globalHeaderKey.ToString(), key = globalHeaderKey.ToString(), isFixed = false, allowFixing = false });
                            columnsNotAllowedSorting.Add(new { columnKey = globalHeaderKey.ToString(), allowSorting = false });
                            globalHeaderKey++;
                            nbPeriod = 0;
                            prevPeriod = currentDay.Month;
                            periodColumnsL1 = new List<object>();
                        }
                        nbPeriod++;
                        //Period Number
                        periodColumnsL2 = new List<object>();
                        periods.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;{1}&nbsp;</td>", _style.CellPeriod, currentDay.ToString("dd"));
                        periodColumnsL1.Add(new { headerText = currentDay.ToString("dd"), group = periodColumnsL2 });
                        //Period day
                        string dayLabel = DayString.GetCharacters(currentDay, cultureInfo, 1);
                        string dayKey = currentDay.ToString("yyyyMMdd") + currentDay.ToString("dd");
                        if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                        {
                            periodColumnsL2.Add(new { headerText = dayLabel, key = dayKey, dataType = "string", width = "22", template = "<span {{if ${" + dayKey + "} == 1 }} class='blueTg' {{elseif ${" + dayKey + "} == 2 }} class='blueExtendedTg' {{elseif ${" + dayKey + "} == 3 }} class='blackTg' {{else}} class='${" + dayKey + "}' {{/if}} ></span>" });
                            schemaFields.Add(new { name = dayKey });
                            columnsFixed.Add(new { columnKey = dayKey, isFixed = false, allowFixing = false });
                            columnsNotAllowedSorting.Add(new { columnKey = dayKey, allowSorting = false });
                        }
                        else
                        {
                            periodColumnsL2.Add(new { headerText = dayLabel, key = dayKey, dataType = "string", width = "22", template = "<span {{if ${" + dayKey + "} == 1 }} class='blueTg' {{elseif ${" + dayKey + "} == 2 }} class='blueExtendedTg' {{elseif ${" + dayKey + "} == 3 }} class='blackTg' {{else}} class='${" + dayKey + "}' {{/if}} ></span>" });
                            schemaFields.Add(new { name = dayKey });
                            columnsFixed.Add(new { columnKey = dayKey, isFixed = false, allowFixing = false });
                            columnsNotAllowedSorting.Add(new { columnKey = dayKey, allowSorting = false });
                        }
                        tableWidth += 22;
                    }
                    if (nbPeriod >= 8)
                    {
                        periodDayLabel = TNS.FrameWork.Convertion.ToHtmlString(TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.ToString("yyyyMM")));
                        columns.Add(new { headerText = periodDayLabel, key = globalHeaderKey.ToString(), group = periodColumnsL1 });
                    }
                    else
                    {
                        columns.Add(new { headerText = "", key = globalHeaderKey.ToString(), group = periodColumnsL1 });
                    }

                    columnsFixed.Add(new { columnKey = globalHeaderKey.ToString(), key = globalHeaderKey.ToString(), isFixed = false, allowFixing = false });
                    columnsNotAllowedSorting.Add(new { columnKey = globalHeaderKey.ToString(), allowSorting = false });
                    globalHeaderKey++;


                    periods.Append("</tr>");
                    days.Append("</tr>");
                    headers.Append("</tr>");
                    break;
            }
            #endregion

            #endregion

            #region Media Schedule
            int i = -1;
            try
            {
                int colorItemIndex = 1;
                int colorNumberToUse = 0;
                int sloganIndex = GetSloganIdIndex();
                Int64 sloganId = long.MinValue;
                string stringItem = "&nbsp;";
                string cssPresentClass = string.Empty;
                string cssExtendedClass = string.Empty;
                string cssClasse = string.Empty;
                string cssClasseNb = string.Empty;
                Int64 idLv1 = 0, idLv2 = 0, idLv3 = 0, idLv4 = 0, pid = 0;
                GenericDetailLevel detailLevel = null;
                detailLevel = GetDetailsLevelSelected();
                _activePeriods = new List<string>();
                int gridColumnId = 0;
                int unitColumnIndex = 0;

                for (i = 1; i < nbline; i++)
                {
                    gridColumnId = 0;
                    unitColumnIndex = 0;

                    #region Color Management
                    if (sloganIndex != -1 && data[i, sloganIndex] != null &&
                        ((detailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == detailLevel.GetNbLevels) ||
                        (detailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < detailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
                    {
                        sloganId = Convert.ToInt64(data[i, sloganIndex]);
                        if (!_session.SloganColors.ContainsKey(sloganId))
                        {
                            colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
                            cssClasse = _style.CellVersions[colorNumberToUse];
                            cssClasseNb = _style.CellVersions[colorNumberToUse];
                            _session.SloganColors.Add(sloganId, _style.CellVersions[colorNumberToUse]);
                            if (_allowVersion)
                            {
                                oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, cssClasse));
                            }
                            colorItemIndex++;
                        }
                        if (sloganId != 0 && !oMediaScheduleData.VersionsDetail.ContainsKey(sloganId))
                        {
                            if (_allowVersion)
                            {
                                oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                            }
                        }
                        cssPresentClass = _session.SloganColors[sloganId].ToString();
                        cssExtendedClass = _session.SloganColors[sloganId].ToString();
                        stringItem = "x";
                    }
                    else
                    {
                        cssPresentClass = _style.CellPresent;
                        cssExtendedClass = _style.CellExtended;
                        stringItem = "&nbsp;";
                    }
                    #endregion

                    #region Line Treatement
                    for (int j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {

                            #region Level 1
                            case L1_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    if (i == TOTAL_LINE_INDEX)
                                    {
                                        cssClasse = _style.CellLevelTotal;
                                        cssClasseNb = _style.CellLevelTotalNb;
                                    }
                                    else
                                    {
                                        cssClasse = _style.CellLevelL1;
                                        cssClasseNb = _style.CellLevelL1Nb;
                                    }
                                    if (data[i, j].GetType() == typeof(MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    ++pid;
                                    idLv1 = pid;
                                    gridData[i - 1, gridColumnId++] = idLv1;

                                    foreach (var unit in units)
                                    {
                                        SetLabelTotalPDM(data, ref gridData, i, cssClasse, cssClasseNb, j, ref gridColumnId, fp, unit, ref unitColumnIndex);

                                        if (IsCurrency(unit) && IsGrpSelected())
                                        {
                                            SetGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);

                                            if(_session.SpendsGrp)
                                                SetSpendsPerGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);
                                        }
                                    }

                                    if (idLv1 == 1)
                                        gridData[i - 1, gridColumnId++] = -1;
                                    else
                                        gridData[i - 1, gridColumnId++] = 1;

                                    if (_allowVersion)
                                    {
                                        if (i != TOTAL_LINE_INDEX && !IsAgencyLevelType(L1_COLUMN_INDEX))
                                        {
                                            SetCreativeLink(data, ref gridData, i, ref gridColumnId, cssClasse, j);
                                        }
                                        else
                                        {
                                            gridData[i - 1, gridColumnId++] = string.Empty;
                                        }

                                    }
                                    if (_allowInsertions)
                                    {
                                        if (i != TOTAL_LINE_INDEX && !IsAgencyLevelType(L1_COLUMN_INDEX))
                                        {
                                            SetInsertionLink(data, ref gridData, i, ref gridColumnId, cssClasse, j);
                                        }
                                        else
                                        {
                                            gridData[i - 1, gridColumnId++] = string.Empty;
                                        }
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 2
                            case L2_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    ++pid;
                                    idLv2 = pid;
                                    gridData[i - 1, gridColumnId++] = idLv2;

                                    foreach (var unit in units)
                                    {
                                        SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL2, _style.CellLevelL2Nb,j, ref gridColumnId, fp, unit, ref unitColumnIndex);

                                        if (IsCurrency(unit) && IsGrpSelected())
                                        {
                                            SetGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);

                                            if (_session.SpendsGrp)
                                                SetSpendsPerGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);
                                        }
                                    }

                                    gridData[i - 1, gridColumnId++] = idLv1;
                                    if (_allowVersion)
                                    {
                                        if (!IsAgencyLevelType(L2_COLUMN_INDEX)) SetCreativeLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL2, j);
                                        else gridData[i - 1, gridColumnId++] = string.Empty;
                                    }
                                    if (_allowInsertions)
                                    {
                                        if (!IsAgencyLevelType(L2_COLUMN_INDEX)) SetInsertionLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL2, j);
                                        else gridData[i - 1, gridColumnId++] = string.Empty;
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case L3_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    ++pid;
                                    idLv3 = pid;
                                    gridData[i - 1, gridColumnId++] = idLv3;

                                    foreach (var unit in units)
                                    {
                                        SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, ref gridColumnId, fp, unit, ref unitColumnIndex);

                                        if (IsCurrency(unit) && IsGrpSelected())
                                        {
                                            SetGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);

                                            if (_session.SpendsGrp)
                                                SetSpendsPerGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);
                                        }
                                    }

                                    gridData[i - 1, gridColumnId++] = idLv2;
                                    if (_allowVersion)
                                    {
                                        if (!IsAgencyLevelType(L3_COLUMN_INDEX)) SetCreativeLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL3, j);
                                        else gridData[i - 1, gridColumnId++] = string.Empty;
                                    }
                                    if (_allowInsertions)
                                    {
                                        if (!IsAgencyLevelType(L3_COLUMN_INDEX)) SetInsertionLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL3, j);
                                        else gridData[i - 1, gridColumnId++] = string.Empty;
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 4
                            case L4_COLUMN_INDEX:
                                ++pid;
                                idLv4 = pid;
                                gridData[i - 1, gridColumnId++] = idLv4;

                                foreach (var unit in units)
                                {
                                    SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, ref gridColumnId, fp, unit, ref unitColumnIndex);

                                    if (IsCurrency(unit) && IsGrpSelected())
                                    {
                                        SetGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);

                                        if (_session.SpendsGrp)
                                            SetSpendsPerGRP(data, ref gridData, i, ref gridColumnId, ref unitColumnIndex);
                                    }
                                }

                                gridData[i - 1, gridColumnId++] = idLv3;
                                if (_allowVersion)
                                {
                                    if (!IsAgencyLevelType(L4_COLUMN_INDEX)) SetCreativeLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL4, j);
                                    else gridData[i - 1, gridColumnId++] = string.Empty;
                                }
                                if (_allowInsertions)
                                {
                                    if (!IsAgencyLevelType(L4_COLUMN_INDEX)) SetInsertionLink(data, ref gridData, i, ref gridColumnId, _style.CellLevelL4, j);
                                    else gridData[i - 1, gridColumnId++] = string.Empty;
                                }
                                j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                break;
                            #endregion

                            #region Other
                            default:
                                if (data[i, j] == null)
                                {
                                    gridData[i - 1, gridColumnId++] = string.Empty;
                                    break;
                                }
                                if (data[i, j] is MediaPlanItem)
                                {
                                    switch (((MediaPlanItem)data[i, j]).GraphicItemType)
                                    {
                                        case DetailledMediaPlan.graphicItemType.present:
                                            if (_showValues)
                                            {
                                                if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                                                    gridData[i - 1, gridColumnId++] = string.Format("<span class=\"{0}\">{1}</span>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItemIds)data[i, j]).IdsNumber.Value, _session.Unit, fp));
                                                else if (_isCreativeDivisionMS || !IsExcelReport || unitTmp.Id != CstWeb.CustomerSessions.Unit.duration)
                                                    gridData[i - 1, gridColumnId++] = string.Format("<span class=\"{0}\">{1}</span>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItem)data[i, j]).Unit, _session.Unit, fp));
                                                else
                                                    gridData[i - 1, gridColumnId++] = string.Format("<span class=\"{0}\">{1}</span>", cssPresentClass, string.Format(fp, unitTmp.StringFormat, ((MediaPlanItem)data[i, j]).Unit));
                                            }
                                            else
                                            {
                                                if (stringItem == "x")
                                                    gridData[i - 1, gridColumnId++] = "version " + cssPresentClass;
                                                else
                                                    gridData[i - 1, gridColumnId++] = 1;
                                            }
                                            break;
                                        case DetailledMediaPlan.graphicItemType.extended:
                                            if (stringItem == "x")
                                                gridData[i - 1, gridColumnId++] = cssExtendedClass;
                                            else
                                                gridData[i - 1, gridColumnId++] = 2;
                                            break;
                                        default:
                                            gridData[i - 1, gridColumnId++] = 3;
                                            break;
                                    }
                                }
                                break;
                                #endregion

                        }
                    }
                    #endregion

                }
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleException("Error i=" + i, err));
            }
            #endregion

            if (tableWidth > 1140)
                gridResult.NeedFixedColumns = true;

            gridResult.HasMSCreatives = false;

            if (Vehicles != null && Vehicles.Count == 1 && oMediaScheduleData.VersionsDetail.Count > 0)
            {
                if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Vehicles[0].DatabaseId) && _session.CustomerLogin.ShowCreatives(Vehicles[0].Id))
                {
                    gridResult.HasMSCreatives = true;
                }
            }

            gridResult.HasMSCreatives = oMediaScheduleData.VersionsDetail.Count > 0 ? true : false;
            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.ColumnsNotAllowedSorting = columnsNotAllowedSorting;
            gridResult.Data = gridData;
            gridResult.Unit = _session.Unit.ToString();

            _session.Save();

            return gridResult;
        }

        #region Table Building Functions

        #region SetLabelTotalPDM
        /// <summary>
        /// Append Label, Total and PDM
        /// </summary>
        protected void SetLabelTotalPDM(object[,] data, ref object[,] gridData, int line, string cssClasse,
            string cssClasseNb, int col, ref int gridColumnId, IFormatProvider fp, UnitInformation unit, ref int unitColumnIndex)
        {
            int unitColumnIndexOld = unitColumnIndex;
            //int unitColumnIndexOld = 0;
            gridData[line - 1, gridColumnId++] = data[line, col];

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    if (data[line, TOTAL_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld] != null)
                    {
                        if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                        {
                            gridData[line - 1, gridColumnId++] =
                                Units.ConvertUnitValue(data[line, TOTAL_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld], unit.Id);
                        }
                        else
                        {
                            gridData[line - 1, gridColumnId++] =
                                Convert.ToDouble(data[line, TOTAL_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld]);
                        }
                    }
                    else
                        gridData[line - 1, gridColumnId++] = "";

                    unitColumnIndex++;
                }
                if (_allowPdm)
                {
                    gridData[line - 1, gridColumnId++] = ((double) data[line, PDM_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld]) / 100;
                    unitColumnIndex++;
                }
            }
            if (_allowTotal)
            {
                if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                {
                    gridData[line - 1, gridColumnId++] = Units.ConvertUnitValue(data[line, TOTAL_COLUMN_INDEX + unitColumnIndexOld],
                        unit.Id);
                }
                else
                {
                    gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, TOTAL_COLUMN_INDEX + unitColumnIndexOld]);
                }
                unitColumnIndex++;
            }
            if (_allowPdm)
            {
                gridData[line - 1, gridColumnId++] = ((double) data[line, PDM_COLUMN_INDEX + unitColumnIndexOld]) / 100;
                unitColumnIndex++;
            }

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                double evol = (double)data[line, EVOL_COLUMN_INDEX + unitColumnIndexOld];

                if (Double.IsInfinity(evol))
                {
                    gridData[line - 1, gridColumnId++] = (evol < 0) ? "-Infinity" : "+Infinity";
                }
                else if (Double.IsNaN(evol))
                {
                    gridData[line - 1, gridColumnId++] = null;
                }
                else if (evol == 0)
                {
                    gridData[line - 1, gridColumnId++] = 0;
                }
                else
                {
                    gridData[line - 1, gridColumnId++] = ((double)evol) / 100;
                }
                unitColumnIndex++;
            }
        }
        #endregion

        #region SetGRP
        /// <summary>
        /// Set GRP
        /// </summary>
        protected void SetGRP(object[,] data, ref object[,] gridData, int line, ref int gridColumnId, ref int unitColumnIndex)
        {
            int unitColumnIndexOld = unitColumnIndex;

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    if (data[line, TOTAL_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld] != null)
                    {
                        gridData[line - 1, gridColumnId++] =
                            Convert.ToDouble(data[line, TOTAL_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld]);
                    }
                    else
                        gridData[line - 1, gridColumnId++] = "";

                    unitColumnIndex++;
                }
                if (_allowPdm)
                {
                    gridData[line - 1, gridColumnId++] = ((double)data[line, PDM_COMPARATIVE_COLUMN_INDEX + unitColumnIndexOld]) / 100;
                    unitColumnIndex++;
                }
            }
            if (_allowTotal)
            {
                gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, TOTAL_COLUMN_INDEX + unitColumnIndexOld]);
                unitColumnIndex++;
            }
            if (_allowPdm)
            {
                gridData[line - 1, gridColumnId++] = ((double)data[line, PDM_COLUMN_INDEX + unitColumnIndexOld]) / 100;
                unitColumnIndex++;
            }
            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                double evol = (double)data[line, EVOL_COLUMN_INDEX + unitColumnIndexOld];

                if (Double.IsInfinity(evol))
                {
                    gridData[line - 1, gridColumnId++] = (evol < 0) ? "-Infinity" : "+Infinity";
                }
                else if (Double.IsNaN(evol))
                {
                    gridData[line - 1, gridColumnId++] = null;
                }
                else if (evol == 0)
                {
                    gridData[line - 1, gridColumnId++] = 0;
                }
                else
                {
                    gridData[line - 1, gridColumnId++] = ((double)evol) / 100;
                }
                unitColumnIndex++;
            }
        }
        #endregion

        #region SetSpendsPerGRP
        /// <summary>
        /// Set Spends Per GRP
        /// </summary>
        protected void SetSpendsPerGRP(object[,] data, ref object[,] gridData, int line, ref int gridColumnId, ref int unitColumnIndex)
        {
            SetGRP(data, ref gridData, line, ref gridColumnId, ref unitColumnIndex);
        }
        #endregion

        #region SetInsertionLink
        /// <summary>
        /// Append Lionk to insertion popup
        /// </summary>
        protected override void SetInsertionLink(object[,] data, ref object[,] gridData, int line, ref int gridColumnId, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                //TODO : A faire côté client
                gridData[line - 1, gridColumnId++] = string.Format("<center><a href='/Insertions?ids={0}&zoomDate={1}&idUnivers=-1&moduleId={2}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , CstWeb.Module.Name.ANALYSE_PLAN_MEDIA
                    );
            }
            else
            {
                gridData[line - 1, gridColumnId++] = "";
            }
        }
        #endregion

        #region SetCreativeLink
        /// <summary>
        /// Append Link to version popup
        /// </summary>
        protected override void SetCreativeLink(object[,] data, ref object[,] gridData, int line, ref int gridColumnId, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                //TODO : A faire côté client
                gridData[line - 1, gridColumnId++] = string.Format("<center><a href='/Creative?ids={0}&zoomDate={1}&idUnivers=-1&moduleId={2}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , CstWeb.Module.Name.ANALYSE_PLAN_MEDIA
                    );
            }
            else
            {
                gridData[line - 1, gridColumnId++] = "";
            }
        }
        #endregion

        #region Add Comparative Period Columns (Period Comparative Total + PDM)
        /// <summary>
        /// Add Comparative Period Columns (Period Comparative Total + PDM)
        /// </summary>
        private int AddComparativePeriodColumns(List<object> columns, List<object> schemaFields, List<object> columnsFixed, string unitName, string format, int tableWidth)
        {
            string periodLabel = string.Empty;

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
                    periodLabel =
                        TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.Begin, _session.SiteLanguage,
                            TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" +
                        TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.End, _session.SiteLanguage,
                            TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    columns.Add(
                        new
                        {
                            headerText = periodLabel,
                            key = $"PERIOD_COMP_{unitName}",
                            dataType = "number",
                            format = format,
                            columnCssClass = "colStyle",
                            width = "100",
                            allowSorting = true
                        });
                    schemaFields.Add(new { name = $"PERIOD_COMP_{unitName}" });
                    columnsFixed.Add(new { columnKey = $"PERIOD_COMP_{unitName}", isFixed = false, allowFixing = false });
                    tableWidth += 100;
                }
                //PDM
                if (_allowPdm)
                {
                    columns.Add(
                        new
                        {
                            headerText = GestionWeb.GetWebWord(806, _session.SiteLanguage),
                            key = $"PDM_COMP_{unitName}",
                            dataType = "number",
                            format = "percent",
                            columnCssClass = "colStyle",
                            width = "82",
                            allowSorting = true
                        });
                    schemaFields.Add(new { name = $"PDM_COMP_{unitName}" });
                    columnsFixed.Add(new { columnKey = $"PDM_COMP_{unitName}", isFixed = false, allowFixing = false });
                    tableWidth += 82;
                }
            }

            return tableWidth;
        }
        #endregion

        #region Add Current Period Columns (Total + PDM)
        /// <summary>
        /// Add Current Period Columns (Total + PDM)
        /// </summary>
        private int AddCurrentPeriodColumns(List<object> columns, List<object> schemaFields, List<object> columnsFixed, string unitName, string format, int tableWidth)
        {
            string periodLabel = string.Empty;

            #region Total Column
            if (_allowTotal)
            {
                if (WebApplicationParameters.UseComparativeMediaSchedule &&
                    _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    periodLabel =
                        TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage,
                            TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" +
                        TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage,
                            TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern);
                    columns.Add(
                        new
                        {
                            headerText = periodLabel,
                            key = $"PERIOD_{unitName}",
                            dataType = "number",
                            format = format,
                            columnCssClass = "colStyle",
                            width = "100",
                            allowSorting = true
                        });
                    schemaFields.Add(new { name = $"PERIOD_{unitName}" });
                    columnsFixed.Add(new { columnKey = $"PERIOD_{unitName}", isFixed = false, allowFixing = false });
                    tableWidth += 100;
                }
                else
                {
                    columns.Add(
                        new
                        {
                            headerText = GestionWeb.GetWebWord(805, _session.SiteLanguage),
                            key = $"PERIOD_{unitName}",
                            dataType = "number",
                            format = format,
                            columnCssClass = "colStyle",
                            width = "100",
                            allowSorting = true
                        });
                    schemaFields.Add(new { name = $"PERIOD_{unitName}" });
                    columnsFixed.Add(new { columnKey = $"PERIOD_{unitName}", isFixed = false, allowFixing = false });
                    tableWidth += 100;
                }
            }
            #endregion

            #region  PDM
            if (_allowPdm)
            {
                columns.Add(
                    new
                    {
                        headerText = GestionWeb.GetWebWord(806, _session.SiteLanguage),
                        key = $"PDM_{unitName}",
                        dataType = "number",
                        format = "percent",
                        columnCssClass = "colStyle",
                        width = "82",
                        allowSorting = true
                    });
                schemaFields.Add(new { name = $"PDM_{unitName}" });
                columnsFixed.Add(new { columnKey = $"PDM_{unitName}", isFixed = false, allowFixing = false });
                tableWidth += 82;
            }
            #endregion

            return tableWidth;
        }
        #endregion

        #region Add Evolution Columns
        /// <summary>
        /// Add Evolution Columns
        /// </summary>
        private int AddEvolutionColumns(List<object> columns, List<object> schemaFields, List<object> columnsFixed, string unitName, string format, int tableWidth)
        {
            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                columns.Add(
                    new
                    {
                        headerText = GestionWeb.GetWebWord(1212, _session.SiteLanguage),
                        key = $"EVOL_{unitName}",
                        dataType = "number",
                        format = "percent",
                        columnCssClass = "colStyle",
                        width = "82",
                        allowSorting = true
                    });
                schemaFields.Add(new { name = $"EVOL_{unitName}" });
                columnsFixed.Add(new { columnKey = $"EVOL_{unitName}", isFixed = false, allowFixing = false });
                tableWidth += 82;
            }

            return tableWidth;
        }
        #endregion

        #region Is Grp Selected
        private bool IsGrpSelected()
        {
            if (_session.Grp || _session.Grp30S)
                return true;

            return false;
        }
        #endregion

        #region Is Currency
        private bool IsCurrency(UnitInformation unit)
        {
            if (unit.Id == CstWeb.CustomerSessions.Unit.euro ||
                unit.Id == CstWeb.CustomerSessions.Unit.tl ||
                unit.Id == CstWeb.CustomerSessions.Unit.usd)
                return true;

            return false;
        }
        #endregion

        #endregion

        #endregion
    }
}
