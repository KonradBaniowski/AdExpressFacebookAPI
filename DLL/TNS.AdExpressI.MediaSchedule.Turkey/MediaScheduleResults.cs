﻿using System;
using System.Collections.Generic;
using System.Data;
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
using TNS.AdExpressI.MediaSchedule.DAL;
using TNS.AdExpressI.MediaSchedule.Exceptions;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.MediaSchedule.Turkey
{
    public class MediaScheduleResults : MediaSchedule.MediaScheduleResults
    {
        const string TOTAL_COLUMN_INDEX_KEY = "TOTAL_COLUMN_INDEX";
        const string PDM_COLUMN_INDEX_KEY = "PDM_COLUMN_INDEX";
        const string TOTAL_COMPARATIVE_COLUMN_INDEX_KEY = "TOTAL_COMPARATIVE_COLUMN_INDEX";
        const string PDM_COMPARATIVE_COLUMN_INDEX_KEY = "PDM_COMPARATIVE_COLUMN_INDEX";
        const string EVOL_COLUMN_INDEX_KEY = "EVOL_COLUMN_INDEX";
        const string SPEND_PER_GRP_COLUMN_INDEX_KEY = "SPEND_PER_GRP_COLUMN_INDEX";


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
            bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy;

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

            var unitsNumber = _session.Units.Count - 1;
            var unitColumnsNb = isComparativeStudy ? 5 : 2;
            if (_session.Grp || _session.Grp30S)
                unitsNumber++;

            if(_session.SpendsGrp)
                firstPeriodIndex++;

            firstPeriodIndex += unitsNumber * unitColumnsNb;

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
                tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth, unit.WebTextId);
                tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth, unit.WebTextId);
                tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, unit.Id.ToString(), format, tableWidth);

                if (unit.Id == CstWeb.CustomerSessions.Unit.euro || unit.Id == CstWeb.CustomerSessions.Unit.tl || unit.Id == CstWeb.CustomerSessions.Unit.usd)
                {
                    if (_session.Grp)
                    {
                        tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3150, _session.SiteLanguage), format, tableWidth, 3150);
                        tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3150, _session.SiteLanguage), format, tableWidth, 3150);
                        tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3150, _session.SiteLanguage), format, tableWidth);
                    }
                    else if (_session.Grp30S)
                    {
                        tableWidth = AddComparativePeriodColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3151, _session.SiteLanguage), format, tableWidth, 3151);
                        tableWidth = AddCurrentPeriodColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3151, _session.SiteLanguage), format, tableWidth, 3151);
                        tableWidth = AddEvolutionColumns(columns, schemaFields, columnsFixed, GestionWeb.GetWebWord(3151, _session.SiteLanguage), format, tableWidth);
                    }

                    if (_session.SpendsGrp)
                    {
                        columns.Add(new { headerText = GestionWeb.GetWebWord(3152, _session.SiteLanguage), key = "SPENDS_PER_GRP", dataType = "string", width = "100" });
                        schemaFields.Add(new { name = "SPENDS_PER_GRP" });
                        columnsFixed.Add(new { columnKey = "SPENDS_PER_GRP", isFixed = false, allowFixing = false });
                    }
                }
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
                int unitIndex = 0;
                bool firstUnit = true;

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
                        firstUnit = true;
                        unitIndex = 0;
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
                                        if (firstUnit)
                                            unitColumnIndex = TOTAL_COLUMN_INDEX - 1;
                                        
                                        SetLabelTotalPDM(data, ref gridData, i, cssClasse, cssClasseNb, j, ref gridColumnId, fp, unit, ref unitColumnIndex, firstUnit);

                                        if (firstUnit)
                                        {
                                            unitColumnIndex = isComparativeStudy ? EVOL_COLUMN_INDEX : L4_ID_COLUMN_INDEX;
                                            firstUnit = false;
                                        }

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
                                        if (firstUnit)
                                            unitColumnIndex = TOTAL_COLUMN_INDEX - 1;

                                        SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL2, _style.CellLevelL2Nb,j, ref gridColumnId, fp, unit, ref unitColumnIndex, firstUnit);

                                        if (firstUnit)
                                        {
                                            unitColumnIndex = isComparativeStudy ? EVOL_COLUMN_INDEX : L4_ID_COLUMN_INDEX;
                                            firstUnit = false;
                                        }

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
                                        if (firstUnit)
                                            unitColumnIndex = TOTAL_COLUMN_INDEX - 1;

                                        SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, ref gridColumnId, fp, unit, ref unitColumnIndex, firstUnit);

                                        if (firstUnit)
                                        {
                                            unitColumnIndex = isComparativeStudy ? EVOL_COLUMN_INDEX : L4_ID_COLUMN_INDEX;
                                            firstUnit = false;
                                        }

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
                                    if (firstUnit)
                                        unitColumnIndex = TOTAL_COLUMN_INDEX - 1;

                                    SetLabelTotalPDM(data, ref gridData, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, ref gridColumnId, fp, unit, ref unitColumnIndex, firstUnit);

                                    if (firstUnit)
                                    {
                                        unitColumnIndex = isComparativeStudy ? EVOL_COLUMN_INDEX : L4_ID_COLUMN_INDEX;
                                        firstUnit = false;
                                    }

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
            string cssClasseNb, int col, ref int gridColumnId, IFormatProvider fp, UnitInformation unit, ref int unitColumnIndex, bool first)
        {
            int TOTAL_INDEX = 1;
            int PDM_INDEX = 2;
            int TOTAL_COMPARATIVE_INDEX = 3;
            int PDM_COMPARATIVE_INDEX = 4;
            int EVOL_INDEX = 5;
            int startColumnIndex = unitColumnIndex;
            int currentColumnIndex = 0;

            if (unitColumnIndex == PERIODICITY_COLUMN_INDEX)
                gridData[line - 1, gridColumnId++] = data[line, col];

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    currentColumnIndex = (first) ? TOTAL_COMPARATIVE_COLUMN_INDEX : startColumnIndex + TOTAL_COMPARATIVE_INDEX;
                    if (data[line, currentColumnIndex] != null)
                    {
                        if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                        {
                            gridData[line - 1, gridColumnId++] = Units.ConvertUnitValue(data[line, currentColumnIndex], unit.Id);
                        }
                        else
                        {
                            gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, currentColumnIndex]);
                        }
                    }
                    else
                        gridData[line - 1, gridColumnId++] = "";

                    unitColumnIndex++;
                }
                if (_allowPdm)
                {
                    currentColumnIndex = (first) ? PDM_COMPARATIVE_COLUMN_INDEX : startColumnIndex + PDM_COMPARATIVE_INDEX;
                    gridData[line - 1, gridColumnId++] = ((double)data[line, currentColumnIndex]) / 100;
                    unitColumnIndex++;
                }
            }

            if (_allowTotal)
            {
                currentColumnIndex = (first) ? TOTAL_COLUMN_INDEX : startColumnIndex + TOTAL_INDEX;
                if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                {
                    gridData[line - 1, gridColumnId++] = Units.ConvertUnitValue(data[line, currentColumnIndex], unit.Id);
                }
                else
                {
                    gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, currentColumnIndex]);
                }
                unitColumnIndex++;
            }
            if (_allowPdm)
            {
                currentColumnIndex = (first) ? PDM_COLUMN_INDEX : startColumnIndex + PDM_INDEX;
                gridData[line - 1, gridColumnId++] = ((double)data[line, currentColumnIndex]) / 100;
                unitColumnIndex++;
            }

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                currentColumnIndex = (first) ? EVOL_COLUMN_INDEX : startColumnIndex + EVOL_INDEX;
                //Evol
                double evol = (double)data[line, currentColumnIndex];

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
            int TOTAL_INDEX = 1;
            int PDM_INDEX = 2;
            int TOTAL_COMPARATIVE_INDEX = 3;
            int PDM_COMPARATIVE_INDEX = 4;
            int EVOL_INDEX = 5;
            int startColumnIndex = unitColumnIndex;

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    if (data[line, startColumnIndex + TOTAL_COMPARATIVE_INDEX] != null)
                    {
                        gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, startColumnIndex + TOTAL_COMPARATIVE_INDEX]);
                    }
                    else
                        gridData[line - 1, gridColumnId++] = "";

                    unitColumnIndex++;
                }
                if (_allowPdm)
                {
                    gridData[line - 1, gridColumnId++] = ((double)data[line, startColumnIndex + PDM_COMPARATIVE_INDEX]) / 100;
                    unitColumnIndex++;
                }
            }

            if (_allowTotal)
            {
                gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, startColumnIndex + TOTAL_INDEX]);
                unitColumnIndex++;
            }

            if (_allowPdm)
            {
                gridData[line - 1, gridColumnId++] = ((double)data[line, startColumnIndex + PDM_INDEX]) / 100;
                unitColumnIndex++;
            }
           
            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                double evol = (double)data[line, startColumnIndex + EVOL_INDEX];

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
            gridData[line - 1, gridColumnId++] = Convert.ToDouble(data[line, ++unitColumnIndex]);
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
        private int AddComparativePeriodColumns(List<object> columns, List<object> schemaFields, List<object> columnsFixed, string unitName, string format, int tableWidth, long unitTextId)
        {
            string periodLabel = string.Empty;

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
                    periodLabel = GestionWeb.GetWebWord(unitTextId, _session.SiteLanguage) + "<br/>" +
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
        private int AddCurrentPeriodColumns(List<object> columns, List<object> schemaFields, List<object> columnsFixed, string unitName, string format, int tableWidth, long unitTextId)
        {
            string periodLabel = string.Empty;

            #region Total Column
            if (_allowTotal)
            {
                if (WebApplicationParameters.UseComparativeMediaSchedule &&
                    _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    periodLabel = GestionWeb.GetWebWord(unitTextId, _session.SiteLanguage) + "<br/>" +
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




        #region Compute Data
        /// <summary>
        /// Compute Data
        /// </summary>
        public override object[,] ComputeData()
        {
            object[,] oTab = null;
            DataSet ds = null;
            DataSet dsComp = null;
            DataTable dt = null;
            DataTable dtComp = null;
            DataTable dtLevels = null;
            object[] param = null;
            GenericDetailLevel detailLevel = GetDetailsLevelSelected();
            IMediaScheduleResultDAL mediaScheduleDAL = GetData(ref ds, ref dsComp, ref dtComp);
            bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy;

            try
            {
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    return (new object[0, 0]);
                }
                dt = ds.Tables[0];

                dtLevels = GetDataLevels(mediaScheduleDAL, detailLevel, dt);

                #region Count nb of elements for each classification level
                int nbLevels = detailLevel.GetNbLevels;
                Int64 oldIdL1 = long.MinValue;
                Int64 oldIdL2 = long.MinValue;
                Int64 oldIdL3 = long.MinValue;
                Int64 oldIdL4 = long.MinValue;
                int nbL1 = 0;
                int nbL2 = 0;
                int nbL3 = 0;
                int nbL4 = 0;
                bool newL2 = false;
                bool newL3 = false;
                bool newL4 = false;
                foreach (DataRow dataRow in dtLevels.Rows)
                {
                    if (nbLevels >= 1 && oldIdL1 != GetLevelId(dataRow, 1, detailLevel))
                    {
                        newL2 = true;
                        nbL1++;
                        oldIdL1 = GetLevelId(dataRow, 1, detailLevel);
                    }
                    if (nbLevels >= 2 && (oldIdL2 != GetLevelId(dataRow, 2, detailLevel) || newL2))
                    {
                        newL3 = true;
                        newL2 = false;
                        nbL2++;
                        oldIdL2 = GetLevelId(dataRow, 2, detailLevel);
                    }
                    if (nbLevels >= 3 && (oldIdL3 != GetLevelId(dataRow, 3, detailLevel) || newL3))
                    {
                        newL4 = true;
                        newL3 = false;
                        nbL3++;
                        oldIdL3 = GetLevelId(dataRow, 3, detailLevel);
                    }
                    if (nbLevels >= 4 && (oldIdL4 != GetLevelId(dataRow, 4, detailLevel) || newL4))
                    {
                        newL4 = false;
                        nbL4++;
                        oldIdL4 = GetLevelId(dataRow, 4, detailLevel);
                    }
                }

                newL2 = newL3 = newL4 = false;
                oldIdL1 = oldIdL2 = oldIdL3 = oldIdL4 = long.MinValue;
                #endregion

                //No Data
                if (nbL1 == 0)
                {
                    return (new object[0, 0]);
                }

                #region Create periods table

                List<Int64> periodItemsList = new List<Int64>();
                Dictionary<int, int> years_index = new Dictionary<int, int>();
                int currentDate = _period.Begin.Year;
                int oldCurrentDate = _period.End.Year;

                int firstPeriodIndex = 0;

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

                int currentColumnIndex = L4_ID_COLUMN_INDEX;
                bool first = true;

                //Add columns indexes for each unit selected
                selectUnits.ForEach(u =>
                {
                    
                    currentColumnIndex = AddUnitsColumnIndexes(currentColumnIndex, first, unitsColumnIndexes, u);

                    if (first)
                    {
                        
                        currentColumnIndex = isComparativeStudy ? EVOL_COLUMN_INDEX : L4_ID_COLUMN_INDEX;
                        first = false;
                    }

                    if (u == CstWeb.CustomerSessions.Unit.grp && hasAdSpend && _session.SpendsGrp)
                    {
                        //Add Spend per grp column Index
                        unitsColumnIndexes[u].Add(SPEND_PER_GRP_COLUMN_INDEX_KEY, ++currentColumnIndex);
                    }
                });

                firstPeriodIndex = currentColumnIndex + 1;

                switch (_period.PeriodDetailLEvel)
                {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                        AtomicPeriodWeek currentWeek = new AtomicPeriodWeek(_period.Begin);
                        AtomicPeriodWeek endWeek = new AtomicPeriodWeek(_period.End);
                        currentDate = currentWeek.Year;
                        oldCurrentDate = endWeek.Year;
                        endWeek.Increment();
                        while (!(currentWeek.Week == endWeek.Week && currentWeek.Year == endWeek.Year))
                        {
                            periodItemsList.Add(currentWeek.Year * 100 + currentWeek.Week);
                            currentWeek.Increment();
                        }
                        break;
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                        DateTime dateCurrent = _period.Begin;
                        DateTime dateEnd = _period.End;
                        dateEnd = dateEnd.AddMonths(1);
                        while (!(dateCurrent.Month == dateEnd.Month && dateCurrent.Year == dateEnd.Year))
                        {
                            periodItemsList.Add(Int64.Parse(dateCurrent.ToString("yyyyMM")));
                            dateCurrent = dateCurrent.AddMonths(1);
                        }
                        break;
                    case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                        DateTime currentDateTime = _period.Begin;
                        DateTime endDate = _period.End;
                        while (currentDateTime <= endDate)
                        {
                            periodItemsList.Add(Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime)));
                            currentDateTime = currentDateTime.AddDays(1);
                        }
                        break;
                    default:
                        throw (new MediaScheduleException("Unable to build periods table."));
                }
                if (currentDate != oldCurrentDate)
                {
                    for (int k = currentDate; k <= oldCurrentDate; k++)
                    {
                        years_index.Add(k, firstPeriodIndex);
                        firstPeriodIndex++;
                    }
                }
                currentDate = 0;
                oldCurrentDate = 0;

                #endregion

                #region Indexes tables

                // Column number
                int nbCol = periodItemsList.Count + firstPeriodIndex;
                // Line number
                int nbline = nbL1 + nbL2 + nbL3 + nbL4 + 3 + 1;
                // Result table
                oTab = new object[nbline, nbCol];
                // L3 elements indexes
                Int64[] tabL3Index = new Int64[nbL3 + 1];
                // L2 elements indexes
                Int64[] tabL2Index = new Int64[nbL2 + 1];
                // L1 elements indexes
                Int64[] tabL1Index = new Int64[nbL1 + 1];

                #endregion

                #region Column Labels
                currentDate = 0;
                while (currentDate < periodItemsList.Count)
                {
                    oTab[0, currentDate + firstPeriodIndex] = periodItemsList[currentDate].ToString(); ;
                    currentDate++;
                }
                foreach (int i in years_index.Keys)
                {
                    oTab[0, years_index[i]] = i;
                }
                #endregion

                #region Init totals
                Int64 currentTotalIndex = 1;
                Int64 currentLineIndex = 1;
                oTab[currentTotalIndex, 0] = TOTAL_STRING;

                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    selectUnits.ForEach(u =>
                    {

                        if (unitsColumnIndexes[u].ContainsKey(TOTAL_COMPARATIVE_COLUMN_INDEX_KEY))
                        {

                            oTab[currentTotalIndex, unitsColumnIndexes[u][TOTAL_COMPARATIVE_COLUMN_INDEX_KEY]] = null;

                        }
                    });
                }

                foreach (int i in years_index.Keys)
                {
                    if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                        oTab[currentLineIndex, int.Parse(years_index[i].ToString())] = new CellIdsNumber();
                    else
                        oTab[currentLineIndex, int.Parse(years_index[i].ToString())] = (double)0.0;
                }
                #endregion

                #region Create MediaPlanItem for total
                for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
                {
                    if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                        oTab[TOTAL_LINE_INDEX, mpi] = new MediaPlanItemIds(-1);
                    else
                        oTab[TOTAL_LINE_INDEX, mpi] = new MediaPlanItem(-1);
                }
                #endregion

                Int64 currentL3PDMIndex = 0;
                Int64 currentL2PDMIndex = 0;
                Int64 currentL1PDMIndex = 0;
                Int64 currentL1Index = 2;
                Int64 currentL2Index = 0;
                Int64 currentL3Index = 1;
                Int64 currentL4Index = 1;
                bool isPeriodN = false;
                int indexPeriod = -1;
                int indexPeriodComparative = -1;
                int numberOflineToAdd = 0;
                Dictionary< CstWeb.CustomerSessions.Unit ,double>  unitDictionary = new Dictionary<CstWeb.CustomerSessions.Unit, double>();
                Dictionary<CstWeb.CustomerSessions.Unit, double> unitComparativeDictionary = new Dictionary<CstWeb.CustomerSessions.Unit, double>();
                CellIdsNumber unitIds = null;
                CstWeb.CustomerSessions.Unit selectedUnit = _session.GetSelectedUnit().Id;
                string unitAlias = SQLGenerator.GetUnitAlias(_session);
                DataRow currentRow = null;

                foreach (DataRow currentRowLevels in dtLevels.Rows)
                {
                    isPeriodN = dt.Rows.Count > indexPeriod + 1
                           && ((nbLevels >= 1 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(1)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(1)])) || nbLevels < 1)
                           && ((nbLevels >= 2 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(2)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(2)])) || nbLevels < 2)
                           && ((nbLevels >= 3 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(3)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(3)])) || nbLevels < 3)
                           && ((nbLevels >= 4 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(4)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(4)])) || nbLevels < 4);

                    #region New L1

                    if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRowLevels, 1, detailLevel))
                    {
                        // Next L2 is new
                        newL2 = true;

                        // PDM
                        if (oldIdL1 != long.MinValue)
                        {
                            for (int i = 0; i < currentL2PDMIndex; i++)
                            {
                                selectUnits.ForEach(un =>
                                {                                  

                                    int totalColumnIndex = unitsColumnIndexes[un][TOTAL_COLUMN_INDEX_KEY];
                                    int pdmColumnIndex = unitsColumnIndexes[un][PDM_COLUMN_INDEX_KEY];

                                    SetLevelPDM(un, oTab, tabL2Index[i],  totalColumnIndex, currentL1Index, pdmColumnIndex);


                                    if (WebApplicationParameters.UseComparativeMediaSchedule &&
                                        _session.ComparativeStudy)
                                    {
                                        if (unitsColumnIndexes[un].ContainsKey(TOTAL_COMPARATIVE_COLUMN_INDEX_KEY))
                                        {
                                            int totalComparativeColumnIndex =
                                                unitsColumnIndexes[un][TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                                            int pdmComparativeColumnIndex =
                                                unitsColumnIndexes[un][PDM_COMPARATIVE_COLUMN_INDEX_KEY];

                                            SetLevelPDM(un, oTab, tabL2Index[i],  totalComparativeColumnIndex, currentL1Index, pdmComparativeColumnIndex);                                          
                                        }
                                    }
                                });

                            }
                            tabL2Index = new Int64[nbL2 + 1];
                            currentL2PDMIndex = 0;
                        }
                        // L1 PDMs
                        tabL1Index[currentL1PDMIndex] = currentLineIndex + 1;
                        currentL1PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = GetLevelLabel(currentRowLevels, 1, detailLevel);
                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = GetLevelId(currentRowLevels, 1, detailLevel);

                        if (nbLevels <= 1)
                            if (isPeriodN) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = dt.Rows[indexPeriod + 1]["period_count"].ToString();
                            else oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = "0";

                        //Init years totals
                        InitYearsTotals(years_index, selectUnits, oTab, currentLineIndex);

                        currentL1Index = currentLineIndex;
                        oldIdL1 = GetLevelId(currentRowLevels, 1, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;

                        // Create MediaPlan Items
                        CreateMediaPlanItems(firstPeriodIndex, nbCol, selectUnits, oTab, currentLineIndex);

                    }

                    #endregion

                    #region New L2

                    if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRowLevels, 2, detailLevel) || newL2))
                    {
                        // Next L3 is new
                        newL3 = true;
                        newL2 = false;

                        //Level 3 PDMs
                        if (oldIdL2 != long.MinValue)
                        {
                            for (int i = 0; i < currentL3PDMIndex; i++)
                            {
                                selectUnits.ForEach(un =>
                                {

                                    int totalColumnIndex = unitsColumnIndexes[un][TOTAL_COLUMN_INDEX_KEY];
                                    int pdmColumnIndex = unitsColumnIndexes[un][PDM_COLUMN_INDEX_KEY];

                                    SetLevelPDM(un, oTab, tabL3Index[i],  totalColumnIndex, currentL2Index, pdmColumnIndex);


                                    if (WebApplicationParameters.UseComparativeMediaSchedule &&
                                        _session.ComparativeStudy)
                                    {
                                        if (unitsColumnIndexes[un].ContainsKey(TOTAL_COMPARATIVE_COLUMN_INDEX_KEY))
                                        {
                                            int totalComparativeColumnIndex =
                                                unitsColumnIndexes[un][TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                                            int pdmComparativeColumnIndex =
                                                unitsColumnIndexes[un][PDM_COMPARATIVE_COLUMN_INDEX_KEY];

                                            SetLevelPDM(un, oTab, tabL3Index[i],  totalComparativeColumnIndex, currentL2Index, pdmComparativeColumnIndex);
                                        }
                                    }
                                });
                            }
                            tabL3Index = new Int64[nbL3 + 1];
                            currentL3PDMIndex = 0;
                        }

                        // Prepare L2 PDMs
                        tabL2Index[currentL2PDMIndex] = currentLineIndex + 1;
                        currentL2PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = GetLevelLabel(currentRowLevels, 2, detailLevel);
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = GetLevelId(currentRowLevels, 2, detailLevel);

                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        if (nbLevels <= 2)
                            if (isPeriodN) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = dt.Rows[indexPeriod + 1]["period_count"].ToString();
                            else oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = "0";

                        //Init years totals
                        InitYearsTotals(years_index, selectUnits, oTab, currentLineIndex);
                       
                        currentL2Index = currentLineIndex;
                        oldIdL2 = GetLevelId(currentRowLevels, 2, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;

                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;
                       
                        // Create MediaPlan Items
                        CreateMediaPlanItems(firstPeriodIndex, nbCol, selectUnits, oTab, currentLineIndex);
                    }

                    #endregion

                    #region New L3

                    if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRowLevels, 3, detailLevel) || newL3))
                    {
                        // Next L4 is different
                        newL4 = true;
                        newL3 = false;

                        //  L4 PDMs
                        if (oldIdL3 != long.MinValue)
                        {
                            for (Int64 i = currentL3Index + 1; i <= currentLineIndex; i++)
                            {
                                selectUnits.ForEach(un =>
                                {

                                    int totalColumnIndex = unitsColumnIndexes[un][TOTAL_COLUMN_INDEX_KEY];
                                    int pdmColumnIndex = unitsColumnIndexes[un][PDM_COLUMN_INDEX_KEY];

                                    SetLevelPDM(un, oTab, i,  totalColumnIndex, currentL3Index, pdmColumnIndex);


                                    if (WebApplicationParameters.UseComparativeMediaSchedule &&
                                        _session.ComparativeStudy)
                                    {
                                        if (unitsColumnIndexes[un].ContainsKey(TOTAL_COMPARATIVE_COLUMN_INDEX_KEY))
                                        {
                                            int totalComparativeColumnIndex =
                                                unitsColumnIndexes[un][TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                                            int pdmComparativeColumnIndex =
                                                unitsColumnIndexes[un][PDM_COMPARATIVE_COLUMN_INDEX_KEY];

                                            SetLevelPDM(un, oTab, i,  totalComparativeColumnIndex, currentL3Index, pdmComparativeColumnIndex);
                                        }
                                    }
                                });

                            }
                        }
                        // Prepare L3 PDM
                        tabL3Index[currentL3PDMIndex] = currentLineIndex + 1;
                        currentL3PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = GetLevelLabel(currentRowLevels, 3, detailLevel);
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = GetLevelId(currentRowLevels, 3, detailLevel);

                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = oldIdL2;
                        if (nbLevels <= 3)
                            if (isPeriodN) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = dt.Rows[indexPeriod + 1]["period_count"].ToString();
                            else oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = "0";
                      
                        //Init years totals
                        InitYearsTotals(years_index, selectUnits, oTab, currentLineIndex);

                        currentL3Index = currentLineIndex;
                        oldIdL3 = GetLevelId(currentRowLevels, 3, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;

                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;

                      
                        // Create MediaPlan Items
                        CreateMediaPlanItems(firstPeriodIndex, nbCol, selectUnits, oTab, currentLineIndex);
                    }
                    #endregion

                    #region New L4

                    if (nbLevels >= 4 && (oldIdL4 != GetLevelId(currentRowLevels, 4, detailLevel) || newL4))
                    {
                        newL4 = false;
                        currentLineIndex++;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = GetLevelLabel(currentRowLevels, 4, detailLevel);
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = GetLevelId(currentRowLevels, 4, detailLevel);

                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = oldIdL2;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = oldIdL3;
                        if (nbLevels <= 4)
                            if (isPeriodN) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = dt.Rows[indexPeriod + 1]["period_count"].ToString();
                            else oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = "0";

                        //Init year totals
                        InitYearsTotals(years_index, selectUnits, oTab, currentLineIndex);

                        currentL4Index = currentLineIndex;
                        oldIdL4 = GetLevelId(currentRowLevels, 4, detailLevel);
                        currentDate = 0;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;                     

                        // Create MediaPlan Items
                        CreateMediaPlanItems(firstPeriodIndex, nbCol, selectUnits, oTab, currentLineIndex);

                    }

                    #endregion


                    while (dt.Rows.Count > indexPeriod + 1
                           &&
                           ((nbLevels >= 1 &&
                             dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(1)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(1)])) || nbLevels < 1)
                           &&
                           ((nbLevels >= 2 &&
                             dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(2)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(2)])) || nbLevels < 2)
                           &&
                           ((nbLevels >= 3 &&
                             dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(3)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(3)])) || nbLevels < 3)
                           &&
                           ((nbLevels >= 4 &&
                             dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(4)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(4)])) || nbLevels < 4)
                    )
                    {
                        indexPeriod++;
                        currentRow = dt.Rows[indexPeriod];

                        #region Treat present Period

                        while (periodItemsList[currentDate] != Int64.Parse(currentRow["date_num"].ToString()))
                        {
                            if (oTab[currentLineIndex, firstPeriodIndex + currentDate] == null)
                            {
                                if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                                    oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItemIds(-1);
                                else
                                    oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItem((long)-1);
                            }
                            currentDate++;
                        }

                        // Set periodicity                     
                        bool firstUnit = true;
                        selectUnits.ForEach(ut =>
                        {

                            if (ut == CstWeb.CustomerSessions.Unit.versionNb)
                            {
                                unitIds = new CellIdsNumber();
                                unitIds.Add(currentRow[ut.ToString()].ToString().Split(','));

                                if (firstUnit)
                                {
                                    if (oTab[currentLineIndex, firstPeriodIndex + currentDate] == null)
                                        oTab[currentLineIndex, firstPeriodIndex + currentDate] =
                                            new MediaPlanItemIds(
                                                Math.Max(
                                                    ((MediaPlanItemIds)
                                                        oTab[currentLineIndex, firstPeriodIndex + currentDate]).PeriodicityId,
                                                    Int64.Parse(currentRow["period_count"].ToString())));
                                    else
                                        ((MediaPlanItemIds)oTab[currentLineIndex, firstPeriodIndex + currentDate])
                                            .PeriodicityId = Int64.Parse(currentRow["period_count"].ToString());

                                }
                               

                            }
                            else
                            {
                               

                                if(unitDictionary.ContainsKey(ut))
                                unitDictionary[ut] = double.Parse(currentRow[ut.ToString()].ToString());
                                else unitDictionary.Add(ut, double.Parse(currentRow[ut.ToString()].ToString()));

                                if (firstUnit)
                                {
                                    if (oTab[currentLineIndex, firstPeriodIndex + currentDate] == null)
                                        oTab[currentLineIndex, firstPeriodIndex + currentDate] =
                                            new MediaPlanItem(
                                                Math.Max(
                                                    ((MediaPlanItem)oTab[currentLineIndex, firstPeriodIndex + currentDate])
                                                    .PeriodicityId, Int64.Parse(currentRow["period_count"].ToString())));
                                    else
                                        ((MediaPlanItem)oTab[currentLineIndex, firstPeriodIndex + currentDate])
                                            .PeriodicityId = Int64.Parse(currentRow["period_count"].ToString());
                                }

                               

                            }
                            firstUnit = false;
                        });

                        if (nbLevels >= 4)
                        {
                            AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL4Index, currentRow, firstPeriodIndex, currentDate, unitDictionary, hasAdSpend);
                        }

                        if (nbLevels >= 3)
                        {
                            AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL3Index, currentRow, firstPeriodIndex, currentDate, unitDictionary, hasAdSpend);
                        }

                        if (nbLevels >= 2)
                        {
                            AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL2Index, currentRow, firstPeriodIndex, currentDate, unitDictionary, hasAdSpend);
                        }

                        if (nbLevels >= 1)
                        {
                            AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL1Index, currentRow, firstPeriodIndex, currentDate, unitDictionary, hasAdSpend);
                        }

                        AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentTotalIndex, currentRow, firstPeriodIndex, currentDate, unitDictionary, hasAdSpend);

                        //Years total
                        int k = int.Parse(currentRow["date_num"].ToString().Substring(0, 4));
                        if (years_index.Any())
                        {
                            k = int.Parse(years_index[k].ToString());

                            if (nbLevels >= 4)
                            {
                                AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL4Index, k, currentRow, unitDictionary, hasAdSpend);
                            }
                            if (nbLevels >= 3)
                            {
                                AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL3Index, k, currentRow, unitDictionary, hasAdSpend);
                            }
                            if (nbLevels >= 2)
                            {
                                AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL2Index, k, currentRow, unitDictionary, hasAdSpend);
                            }
                            if (nbLevels >= 1)
                            {
                                AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentL1Index, k, currentRow, unitDictionary, hasAdSpend);
                            }

                            AddUnitValue(selectUnits, unitsColumnIndexes, oTab, currentTotalIndex, k, currentRow, unitDictionary, hasAdSpend);
                        }
                        currentDate++;
                        oldCurrentDate = currentDate;

                        while (oldCurrentDate < periodItemsList.Count)
                        {
                            if (oTab[currentLineIndex, firstPeriodIndex + currentDate] == null)
                            {
                                if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                                {
                                    oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItemIds(-1);
                                }
                                else
                                {
                                    oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItem(-1);
                                }
                            }
                            oldCurrentDate++;
                        }
                        currentDate = 0;
                        #endregion


                    }

                    while (dtComp != null && dtComp.Rows.Count > indexPeriodComparative + 1
                           &&
                           ((nbLevels >= 1 &&
                             dtComp.Rows[indexPeriodComparative + 1][detailLevel.GetColumnNameLevelId(1)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(1)])) || nbLevels < 1)
                           &&
                           ((nbLevels >= 2 &&
                             dtComp.Rows[indexPeriodComparative + 1][detailLevel.GetColumnNameLevelId(2)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(2)])) || nbLevels < 2)
                           &&
                           ((nbLevels >= 3 &&
                             dtComp.Rows[indexPeriodComparative + 1][detailLevel.GetColumnNameLevelId(3)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(3)])) || nbLevels < 3)
                           &&
                           ((nbLevels >= 4 &&
                             dtComp.Rows[indexPeriodComparative + 1][detailLevel.GetColumnNameLevelId(4)].Equals(
                                 currentRowLevels[detailLevel.GetColumnNameLevelId(4)])) || nbLevels < 4)
                    )
                    {
                        indexPeriodComparative++;

                        #region Treat present Period Comparative

                        if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                        {
                            selectUnits.ForEach(ux =>
                            {
                                if (ux != CstWeb.CustomerSessions.Unit.versionNb)
                                {

                                    if (unitComparativeDictionary.ContainsKey(ux))
                                        unitComparativeDictionary[ux] =
                                            double.Parse(dtComp.Rows[indexPeriodComparative][ux.ToString()].ToString());
                                    else
                                        unitComparativeDictionary.Add(ux,
                                            double.Parse(dtComp.Rows[indexPeriodComparative][ux.ToString()].ToString()));
                                }

                            });

                            if (nbLevels >= 4)
                            {

                                AddUnitComparativeValue(selectUnits, unitsColumnIndexes, oTab, currentL4Index, dtComp,
                                    indexPeriodComparative, unitComparativeDictionary);
                            }
                            if (nbLevels >= 3)
                            {

                                AddUnitComparativeValue(selectUnits, unitsColumnIndexes, oTab, currentL3Index, dtComp,
                                    indexPeriodComparative, unitComparativeDictionary);
                            }
                            if (nbLevels >= 2)
                            {

                                AddUnitComparativeValue(selectUnits, unitsColumnIndexes, oTab, currentL2Index, dtComp,
                                    indexPeriodComparative, unitComparativeDictionary);
                            }
                            if (nbLevels >= 1)
                            {

                                AddUnitComparativeValue(selectUnits, unitsColumnIndexes, oTab, currentL1Index, dtComp,
                                    indexPeriodComparative, unitComparativeDictionary);
                            }

                            AddUnitComparativeValue(selectUnits, unitsColumnIndexes, oTab, currentTotalIndex, dtComp,
                                indexPeriodComparative, unitComparativeDictionary);
                        }

                        #endregion
                    }
                }

                #region Compute PDMs

                if (nbL1 > 0)
                {
                    // PDM L4
                    if (nbLevels >= 4)
                    {                       
                        AddL4PDMValue(selectUnits, unitsColumnIndexes, currentL3Index, currentLineIndex, oTab);

                    }
                    // PDM L3
                    if (nbLevels >= 3)
                    {
                       
                        AddPDMValue(selectUnits, unitsColumnIndexes, currentL3PDMIndex, oTab, tabL3Index, currentL2Index);                        
                    }
                    // PDM L2
                    if (nbLevels >= 2)
                    {

                        AddPDMValue(selectUnits, unitsColumnIndexes, currentL2PDMIndex, oTab, tabL2Index, currentL1Index);
                    }
                    // PDM L1
                    if (nbLevels >= 1)
                    {

                        AddPDMValue(selectUnits, unitsColumnIndexes, currentL1PDMIndex, oTab, tabL1Index, currentTotalIndex);
                    }

                    // PDM Total
                    
                    selectUnits.ForEach(p =>
                    {
                        var unitColumns = unitsColumnIndexes[p];
                        var pdmColumnIndex = unitColumns[PDM_COLUMN_INDEX_KEY];
                        int pdmComparativeColumnIndex = -1;
                        if (isComparativeStudy)
                        {                          
                            pdmComparativeColumnIndex = unitColumns[PDM_COMPARATIVE_COLUMN_INDEX_KEY];
                        }
                        oTab[currentTotalIndex, pdmColumnIndex] = (double)100.0;

                        if (isComparativeStudy)
                        {
                            oTab[currentTotalIndex, pdmComparativeColumnIndex] = (double)100.0;
                        }

                    });
                        
                }

                #endregion

                #region Compute Evol
                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    ComputeEvolution(selectUnits, unitsColumnIndexes, currentTotalIndex, currentLineIndex, oTab, selectedUnit);                   
                }
                #endregion

                #region Periodicity treatement
                MediaPlanItem item = null;
                MediaPlanItem tmp = null;
                MediaPlan.graphicItemType graphicType;

                for (int i = 1; i < nbline; i++)
                {
                    if (oTab[i, 0] != null) if (oTab[i, 0].GetType() == typeof(MemoryArrayEnd)) break;
                    // N1 line
                    if (oTab[i, L1_COLUMN_INDEX] != null) currentL1Index = i;
                    // N2 line
                    if (oTab[i, L2_COLUMN_INDEX] != null) currentL2Index = i;
                    // N3 line
                    if (oTab[i, L3_COLUMN_INDEX] != null) currentL3Index = i;
                    // N4 line
                    if (oTab[i, L4_COLUMN_INDEX] != null) currentL4Index = i;
                    // lower level
                    if ((nbLevels == 1 && currentL1Index == i) || (nbLevels == 2 && currentL2Index == i) ||
                        (nbLevels == 3 && currentL3Index == i) || (nbLevels == 4 && currentL4Index == i))
                    {
                        for (int j = firstPeriodIndex; j < nbCol; j++)
                        {
                            item = ((MediaPlanItem) oTab[i, j]);

                            for (int k = 0; k < item.PeriodicityId && (j + k) < nbCol; k++)
                            {
                                if (k == 0)
                                {
                                    graphicType = MediaPlan.graphicItemType.present;
                                }
                                else
                                {
                                    graphicType = MediaPlan.graphicItemType.extended;
                                }
                                ((MediaPlanItem) oTab[i, j + k]).GraphicItemType = graphicType;
                                if (nbLevels > 3 &&
                                    (tmp = (MediaPlanItem) oTab[currentL3Index, j + k]).GraphicItemType !=
                                    MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (nbLevels > 2 &&
                                    (tmp = (MediaPlanItem) oTab[currentL2Index, j + k]).GraphicItemType !=
                                    MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (nbLevels > 1 &&
                                    (tmp = (MediaPlanItem) oTab[currentL1Index, j + k]).GraphicItemType !=
                                    MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (oTab[TOTAL_LINE_INDEX, j + k] == null)
                                    oTab[TOTAL_LINE_INDEX, j + k] = new MediaPlanItem();
                                if ((tmp = (MediaPlanItem) oTab[TOTAL_LINE_INDEX, j + k]).GraphicItemType !=
                                    MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                            }
                        }
                    }
                }

                #endregion

                #region Ecriture de fin de tableau
                nbCol = oTab.GetLength(1);
                nbline = oTab.GetLength(0);
                if (currentLineIndex + 1 < nbline)
                    oTab[currentLineIndex + 1, 0] = new MemoryArrayEnd();
                #endregion
            }
            finally
            {
                ds?.Dispose();

                dsComp?.Dispose();
            }

            return oTab;
        }

        private void ComputeEvolution(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes, long currentTotalIndex,
            long currentLineIndex, object[,] oTab, CstWeb.CustomerSessions.Unit selectedUnit)
        {
            selectUnits.ForEach(ue =>
            {
                var unitColumns = unitsColumnIndexes[ue];
                var totalColumnIndex = unitColumns[TOTAL_COLUMN_INDEX_KEY];
                int totalComparativeColumnIndex = unitColumns[TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                int evolColumnIndex = unitColumns[EVOL_COLUMN_INDEX_KEY];

                for (long i = currentTotalIndex; i <= currentLineIndex; i++)
                {
                    if (oTab[i, totalColumnIndex] != null)
                    {
                        if (ue == CstWeb.CustomerSessions.Unit.versionNb)
                        {
                            if (oTab[i, totalComparativeColumnIndex] == null ||
                                ((CellIdsNumber) oTab[i, totalComparativeColumnIndex]).Value == 0.0)
                                oTab[i, evolColumnIndex] =
                                    ((CellIdsNumber) oTab[i, totalColumnIndex]).Value *
                                    Double.PositiveInfinity;
                            else
                            {
                                oTab[i, evolColumnIndex] =
                                    ((((CellIdsNumber) oTab[i, totalColumnIndex]).Value /
                                      ((CellIdsNumber) oTab[i, totalComparativeColumnIndex]).Value) - 1) *
                                    100.0;
                            }
                        }
                        else
                        {
                            if (oTab[i, totalComparativeColumnIndex] == null ||
                                (double) oTab[i, totalComparativeColumnIndex] == 0.0)
                                oTab[i, evolColumnIndex] = ((double) oTab[i, totalColumnIndex]) *
                                                           Double.PositiveInfinity;
                            else
                            {
                                oTab[i, evolColumnIndex] = ((((double) oTab[i, totalColumnIndex]) /
                                                             ((double) oTab[i, totalComparativeColumnIndex])) -
                                                            1) * 100.0;
                            }
                        }
                    }
                    else if (oTab[i, totalComparativeColumnIndex] != null)
                    {
                        if (selectedUnit == CstWeb.CustomerSessions.Unit.versionNb)
                            oTab[i, evolColumnIndex] =
                                ((CellIdsNumber) oTab[i, totalComparativeColumnIndex]).Value *
                                Double.NegativeInfinity;
                        else
                            oTab[i, evolColumnIndex] = ((double) oTab[i, totalComparativeColumnIndex]) *
                                                       Double.NegativeInfinity;
                    }
                }
            });
        }

        private void AddPDMValue(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes,
            long currentLevelPDMIndex, object[,] oTab,
            long[] tabLevelIndex, long currentLevelIndex)
        {
            bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule &&
                                      _session.ComparativeStudy;
            selectUnits.ForEach(unt =>
            {
                if (unitsColumnIndexes.ContainsKey(unt))
                {
                    var unitColumns = unitsColumnIndexes[unt];
                    var totalColumnIndex = unitColumns[TOTAL_COLUMN_INDEX_KEY];
                    var pdmColumnIndex = unitColumns[PDM_COLUMN_INDEX_KEY];


                    int totalComparativeColumnIndex = -1;
                    int pdmComparativeColumnIndex = -1;
                    if (isComparativeStudy)
                    {
                        totalComparativeColumnIndex = unitColumns[TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                        pdmComparativeColumnIndex = unitColumns[PDM_COMPARATIVE_COLUMN_INDEX_KEY];
                    }


                    for (int i = 0; i < currentLevelPDMIndex; i++)
                    {
                        if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                        {
                            if (oTab[tabLevelIndex[i], totalColumnIndex] != null &&
                                oTab[currentLevelIndex, totalColumnIndex] != null &&
                                ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value != 0)
                                oTab[tabLevelIndex[i], pdmColumnIndex] =
                                    ((CellIdsNumber) oTab[tabLevelIndex[i], totalColumnIndex]).Value /
                                    ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value * 100.0;
                            else
                                oTab[tabLevelIndex[i], pdmColumnIndex] = 0.0;
                        }
                        else
                        {
                            if (oTab[tabLevelIndex[i], totalColumnIndex] != null &&
                                oTab[currentLevelIndex, totalColumnIndex] != null &&
                                (double) oTab[currentLevelIndex, totalColumnIndex] != 0)
                                oTab[tabLevelIndex[i], pdmColumnIndex] = (double) oTab[tabLevelIndex[i], totalColumnIndex] /
                                                                      (double) oTab[currentLevelIndex, totalColumnIndex] *
                                                                      100.0;
                            else
                                oTab[tabLevelIndex[i], pdmColumnIndex] = 0.0;
                        }
                        if (isComparativeStudy)
                        {
                            if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                            {
                                if (oTab[tabLevelIndex[i], totalComparativeColumnIndex] != null &&
                                    oTab[currentLevelIndex, totalComparativeColumnIndex] != null &&
                                    ((CellIdsNumber) oTab[currentLevelIndex, totalComparativeColumnIndex])
                                    .Value != 0)
                                    oTab[tabLevelIndex[i], pdmComparativeColumnIndex] =
                                        ((CellIdsNumber) oTab[tabLevelIndex[i], totalComparativeColumnIndex]).Value /
                                        ((CellIdsNumber) oTab[currentLevelIndex, totalComparativeColumnIndex])
                                        .Value * 100.0;
                                else
                                    oTab[tabLevelIndex[i], pdmComparativeColumnIndex] = 0.0;
                            }
                            else
                            {
                                if (oTab[tabLevelIndex[i], totalComparativeColumnIndex] != null && oTab[currentLevelIndex, totalComparativeColumnIndex] != null && (double)oTab[currentLevelIndex, totalComparativeColumnIndex] != 0)
                                    oTab[tabLevelIndex[i], pdmComparativeColumnIndex] = (double)oTab[tabLevelIndex[i], totalComparativeColumnIndex] / (double)oTab[currentLevelIndex, totalComparativeColumnIndex] * 100.0;
                                else
                                    oTab[tabLevelIndex[i], pdmComparativeColumnIndex] = 0.0;
                            }
                        }
                    }
                }
            });
        }

        private void AddL4PDMValue(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes, long currentLevelIndex,
            long currentLineIndex,
            object[,] oTab)
        {
            selectUnits.ForEach(unt =>
            {
                if (unitsColumnIndexes.ContainsKey(unt))
                {
                    var unitColumns = unitsColumnIndexes[unt];
                    var totalColumnIndex = unitColumns[TOTAL_COLUMN_INDEX_KEY];
                    var pdmColumnIndex = unitColumns[PDM_COLUMN_INDEX_KEY];
                    bool isComparativeStudy = WebApplicationParameters.UseComparativeMediaSchedule &&
                                              _session.ComparativeStudy;

                    int totalComparativeColumnIndex = -1;
                    int pdmComparativeColumnIndex = -1;
                    if (isComparativeStudy)
                    {
                        totalComparativeColumnIndex = unitColumns[TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];
                        pdmComparativeColumnIndex = unitColumns[PDM_COMPARATIVE_COLUMN_INDEX_KEY];
                    }


                    for (Int64 i = currentLevelIndex + 1; i <= currentLineIndex; i++)
                    {
                        if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                        {
                            if (oTab[i, totalColumnIndex] != null &&
                                oTab[currentLevelIndex, totalColumnIndex] != null &&
                                ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value != 0)
                                oTab[i, pdmColumnIndex] =
                                    ((CellIdsNumber) oTab[i, totalColumnIndex]).Value /
                                    ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value * 100.0;
                            else
                                oTab[i, pdmColumnIndex] = 0.0;
                        }
                        else
                        {
                            if (oTab[i, totalColumnIndex] != null &&
                                oTab[currentLevelIndex, totalColumnIndex] != null &&
                                (double) oTab[currentLevelIndex, totalColumnIndex] != 0)
                                oTab[i, pdmColumnIndex] = (double) oTab[i, totalColumnIndex] /
                                                          (double) oTab[currentLevelIndex, totalColumnIndex] *
                                                          100.0;
                            else
                                oTab[i, pdmColumnIndex] = 0.0;
                        }
                        if (isComparativeStudy)
                        {
                            if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                            {
                                if (oTab[i, totalComparativeColumnIndex] != null &&
                                    oTab[currentLevelIndex, totalComparativeColumnIndex] != null &&
                                    ((CellIdsNumber) oTab[currentLevelIndex, totalComparativeColumnIndex])
                                    .Value != 0)
                                    oTab[i, pdmComparativeColumnIndex] =
                                        ((CellIdsNumber) oTab[i, totalComparativeColumnIndex]).Value /
                                        ((CellIdsNumber) oTab[currentLevelIndex, totalComparativeColumnIndex])
                                        .Value * 100.0;
                                else
                                    oTab[i, pdmComparativeColumnIndex] = 0.0;
                            }
                            else
                            {
                                if (oTab[i, totalComparativeColumnIndex] != null && oTab[currentLevelIndex, totalComparativeColumnIndex] != null && (double)oTab[currentLevelIndex, totalComparativeColumnIndex] != 0)
                                    oTab[i, pdmComparativeColumnIndex] = (double)oTab[i, totalComparativeColumnIndex] / (double)oTab[currentLevelIndex, totalComparativeColumnIndex] * 100.0;
                                else
                                    oTab[i, pdmComparativeColumnIndex] = 0.0;
                            }
                        }
                    }
                }
            });
        }

        private void AddUnitComparativeValue(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes, object[,] oTab,
            long currentLevelIndex, DataTable dtComp, int indexPeriodComparative,
            Dictionary<CstWeb.CustomerSessions.Unit, double> unitComparativeDictionary)
        {
            selectUnits.ForEach(unt =>
            {
                if (unitsColumnIndexes.ContainsKey(unt))
                {
                    var unitColumns = unitsColumnIndexes[unt];
                    var totalComparativeColumnIndex = unitColumns[TOTAL_COMPARATIVE_COLUMN_INDEX_KEY];

                    //Init cell
                    if (oTab[currentLevelIndex, totalComparativeColumnIndex] == null)
                    {
                        if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                            oTab[currentLevelIndex, totalComparativeColumnIndex] = new CellIdsNumber();
                        else
                            oTab[currentLevelIndex, totalComparativeColumnIndex] = (double) 0.0;
                    }

                    //Set cell unit value
                    if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                    {
                        ((CellIdsNumber) oTab[currentLevelIndex, totalComparativeColumnIndex]).Add(
                            dtComp.Rows[indexPeriodComparative][unt.ToString()].ToString()
                                .Split(','));
                    }
                    else
                    {
                        double unit = (unitComparativeDictionary.ContainsKey(unt))
                            ? unitComparativeDictionary[unt]
                            : (double) 0.0;
                        oTab[currentLevelIndex, totalComparativeColumnIndex] =
                            ((double) oTab[currentLevelIndex, totalComparativeColumnIndex])
                            + unit;
                    }
                }
            });
        }

        private void AddUnitValue(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes,
            object[,] oTab, long currentLevelIndex, int k,
            DataRow currentRow, Dictionary<CstWeb.CustomerSessions.Unit, double> unitDictionary, bool hasAdSpend)
        {
            selectUnits.ForEach(unt =>
            {
                if (unitsColumnIndexes.ContainsKey(unt))
                {
                    //Set cell unit value
                    if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                    {
                        ((CellIdsNumber) oTab[currentLevelIndex, k]).Add(
                            currentRow[unt.ToString()].ToString().Split(','));
                    }
                    else
                    {
                        double unit = (unitDictionary.ContainsKey(unt)) ? unitDictionary[unt] : (double) 0.0;
                        oTab[currentLevelIndex, k] = (double) oTab[currentLevelIndex, k] + unit;
                    }
                }
            });

        }

        private void AddUnitValue(List<CstWeb.CustomerSessions.Unit> selectUnits,
            Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes, object[,] oTab,
            long currentLevelIndex,
            DataRow currentRow, int firstPeriodIndex, int currentDate,
            Dictionary<CstWeb.CustomerSessions.Unit, double> unitDictionary, bool hasAdSpend)
        {
            int unitIndex = 0;
            selectUnits.ForEach(unt =>
            {
                if (unitsColumnIndexes.ContainsKey(unt))
                {
                    var unitColumns = unitsColumnIndexes[unt];
                    var totalColumnIndex = unitColumns[TOTAL_COLUMN_INDEX_KEY];

                    //Init cell
                    if (oTab[currentLevelIndex, totalColumnIndex] == null)
                    {
                        if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                            oTab[currentLevelIndex, totalColumnIndex] = new CellIdsNumber();
                        else
                            oTab[currentLevelIndex, totalColumnIndex] = (double) 0.0;
                    }

                    //Set cell unit value
                    if (unt == CstWeb.CustomerSessions.Unit.versionNb)
                    {
                        ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Add(
                            currentRow[unt.ToString()].ToString().Split(','));
                        ((MediaPlanItemIds) oTab[currentLevelIndex, firstPeriodIndex + currentDate]).IdsNumber.Add(
                            currentRow[unt.ToString()].ToString().Split(','));
                    }
                    else
                    {
                        double unit = (unitDictionary.ContainsKey(unt)) ? unitDictionary[unt] : (double) 0.0;
                        var unitSum = (double)oTab[currentLevelIndex, totalColumnIndex] + unit;
                        oTab[currentLevelIndex, totalColumnIndex] = unitSum;
                        ((MediaPlanItem) oTab[currentLevelIndex, firstPeriodIndex + currentDate]).Unit += unit;

                        if (unt == CstWeb.CustomerSessions.Unit.grp && hasAdSpend && _session.SpendsGrp)
                        {
                            var adSpendsUnit = selectUnits[unitIndex - 1];
                            var adSpendsColumns = unitsColumnIndexes[adSpendsUnit];
                            int totalSpendsColumnIndex = adSpendsColumns[TOTAL_COLUMN_INDEX_KEY];
                            var totalSpendsPerGrpColumnIndex = unitsColumnIndexes[unt][SPEND_PER_GRP_COLUMN_INDEX_KEY];
                            if (unitSum > 0)
                                oTab[currentLevelIndex, totalSpendsPerGrpColumnIndex] = (double) oTab[currentLevelIndex, totalSpendsColumnIndex] / unitSum;
                            else
                                oTab[currentLevelIndex, totalSpendsPerGrpColumnIndex] = 0;
                        }
                    }
                }
                unitIndex++;
            });


        }

        private void CreateMediaPlanItems(int firstPeriodIndex, int nbCol, List<CstWeb.CustomerSessions.Unit> selectUnits, object[,] oTab,
            long currentLineIndex)
        {
            for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
            {
                if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                    oTab[currentLineIndex, mpi] = new MediaPlanItemIds(-1);
                else
                    oTab[currentLineIndex, mpi] = new MediaPlanItem(-1);
            }
        }

        private void InitYearsTotals(Dictionary<int, int> yearsIndex, List<CstWeb.CustomerSessions.Unit> selectUnits, object[,] oTab, long currentLineIndex)
        {
            foreach (int i in yearsIndex.Keys)
            {
                if (selectUnits.Contains(CstWeb.CustomerSessions.Unit.versionNb))
                    oTab[currentLineIndex, yearsIndex[i]] = new CellIdsNumber();
                else
                    oTab[currentLineIndex, yearsIndex[i]] = (double) 0.0;
            }
        }

        private void SetLevelPDM(CstWeb.CustomerSessions.Unit un, object[,] oTab, long currentColumnIndex,  int totalColumnIndex, long currentLevelIndex,
            int pdmColumnIndex)
        {
            if (un == CstWeb.CustomerSessions.Unit.versionNb)
            {
                if (oTab[currentColumnIndex, totalColumnIndex] != null &&
                    oTab[currentLevelIndex, totalColumnIndex] != null &&
                    ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value != 0)
                    oTab[currentColumnIndex, pdmColumnIndex] =
                        ((CellIdsNumber) oTab[currentColumnIndex, totalColumnIndex]).Value /
                        ((CellIdsNumber) oTab[currentLevelIndex, totalColumnIndex]).Value * 100.0;
                else
                    oTab[currentColumnIndex, pdmColumnIndex] = 0.0;
            }
            else
            {
                if (oTab[currentColumnIndex, totalColumnIndex] != null &&
                    oTab[currentLevelIndex, totalColumnIndex] != null &&
                    Convert.ToDouble(oTab[currentLevelIndex, totalColumnIndex]) != 0)
                    oTab[currentColumnIndex, pdmColumnIndex] =
                        Convert.ToDouble(oTab[currentColumnIndex, totalColumnIndex]) /
                        Convert.ToDouble(oTab[currentLevelIndex, totalColumnIndex]) * 100.0;
                else
                    oTab[currentColumnIndex, pdmColumnIndex] = 0.0;
            }
        }

        private List<CstWeb.CustomerSessions.Unit> GetAdSpendsUnit()
        {
            List< CstWeb.CustomerSessions.Unit> adSpendsUnits = new List<CstWeb.CustomerSessions.Unit>();
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.euro);
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.usd);
            adSpendsUnits.Add(CstWeb.CustomerSessions.Unit.tl);
            return adSpendsUnits;
        }

        private bool ContainsSpendsUnit(List<CstWeb.CustomerSessions.Unit> list1, List<CstWeb.CustomerSessions.Unit> list2)
        {

            return list1.Intersect(list2).Any();
        }

        private int AddUnitsColumnIndexes(int currentColumnIndex, bool first, Dictionary<CstWeb.CustomerSessions.Unit, Dictionary<string, int>> unitsColumnIndexes,
             CstWeb.CustomerSessions.Unit unit)
        {
            var currentUnitColumnIndexes = new Dictionary<string, int>();

            //Add Total column Indexes
            currentColumnIndex = (first) ? TOTAL_COLUMN_INDEX : ++currentColumnIndex;
            currentUnitColumnIndexes.Add(TOTAL_COLUMN_INDEX_KEY, currentColumnIndex);

            //Add PDM column Indexes
            currentColumnIndex = (first) ? PDM_COLUMN_INDEX : ++currentColumnIndex;
            currentUnitColumnIndexes.Add(PDM_COLUMN_INDEX_KEY, currentColumnIndex);

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                //Add Total Comparative column Indexes                      
                currentColumnIndex = (first) ? TOTAL_COMPARATIVE_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(TOTAL_COMPARATIVE_COLUMN_INDEX_KEY, currentColumnIndex);


                //Add PDM Comparative column Indexes
                currentColumnIndex = (first) ? PDM_COMPARATIVE_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(PDM_COMPARATIVE_COLUMN_INDEX_KEY, currentColumnIndex);

                //Add Evolution column Indexes
                currentColumnIndex = (first) ? EVOL_COLUMN_INDEX : ++currentColumnIndex;
                currentUnitColumnIndexes.Add(EVOL_COLUMN_INDEX_KEY, currentColumnIndex);
            }

            unitsColumnIndexes.Add(unit, currentUnitColumnIndexes);
            return currentColumnIndex;
        }

        #endregion

    }
}
