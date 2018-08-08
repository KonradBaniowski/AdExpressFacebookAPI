#region using
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;
using System.Globalization;

using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstFrameWorkResult = TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpressI.MediaSchedule.Exceptions;
using TNS.AdExpressI.MediaSchedule.Style;

using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.FrameWork;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Results;

using TNS.AdExpressI.MediaSchedule.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

using Aspose.Cells;
using TNS.AdExpressI.MediaSchedule.Functions;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Web.Core.Utilities;

#endregion

namespace TNS.AdExpressI.MediaSchedule
{
    public abstract partial class MediaScheduleResults
    {
        #region IMediaScheduleResults Membres
        /// <summary>
        /// Get HTML code for an raw excel export of the media schedule
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetRawExcel(bool withValues)
        {
            _isCreativeDivisionMS = false;
            _showValues = withValues;
            _isExcelReport = true;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = ((!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.evaliantMobile && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.internet)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);
            _style = new ExcelMediaScheduleStyle();
            return ComputeDesignRawExcel(ComputeData());
        }

        /// <summary>
        /// Get HTML code for an excel export of the media schedule
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetExcelHtml(bool withValues)
        {
            _isCreativeDivisionMS = false;
            _showValues = withValues;
            _isExcelReport = true;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = ((!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.evaliantMobile && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.internet)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);
            _style = new ExcelMediaScheduleStyle();
            return ComputeDesign(ComputeData());
        }

        /// <summary>
        /// Get HTML code for an excel export of the media schedule dedicated to creative division
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetExcelHtmlCreativeDivision(bool withValues)
        {
            _isCreativeDivisionMS = true;
            _showValues = withValues;
            _isExcelReport = true;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = ((!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.evaliantMobile && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.internet)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);
            _style = new ExcelMediaScheduleStyle();
            return ComputeDesign(ComputeData());
        }

        /// <summary>
        /// Get Excel for an excel export by Anubis of the media schedule
        /// </summary>
        public virtual void GetRawData(Workbook excel, TNS.FrameWork.WebTheme.Style style)
        {
            _isCreativeDivisionMS = false;
            _showValues = false;
            _allowTotal = false;
            _allowPdm = false;
            ComputeDesignExcel(ComputeData(), excel, style);
        }
        #endregion

        #region Design table

        /// <summary>
        /// Provide html Code to present raw excel Media Schedule
        /// </summary>
        /// <param name="data">Preformated Data</param>
        /// <returns>HTML code</returns>
        protected virtual MediaScheduleData ComputeDesignRawExcel(object[,] data)
        {
            var t = new StringBuilder();

            var oMediaScheduleData = new MediaScheduleData();
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            IFormatProvider fp =
                (!_isExcelReport || _isCreativeDivisionMS) ? WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo
                : WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfoExcel;

            #region Theme Name
            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            #endregion

            #region No data
            if (data.GetLength(0) == 0)
            {
                oMediaScheduleData.HTMLCode = string.Empty;
                return (oMediaScheduleData);
            }
            #endregion

            #region Init Variables
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
                    firstPeriodIndex = CstFrameWorkResult.MediaSchedule.EVOL_COLUMN_INDEX + 1;
                }
                else
                {
                    firstPeriodIndex =  CstFrameWorkResult.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
                }
            }
            else
            {
                firstPeriodIndex =  CstFrameWorkResult.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
            }

            firstPeriodIndex += nbColYear;

            int nbColTab = data.GetLength(1);
            int nbPeriod = nbColTab - firstPeriodIndex - 1;
            int nbline = data.GetLength(0);

            try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }
            catch (System.Exception) { }
            oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            bool isExport = _isExcelReport || _isPDFReport;
            int labColSpan = (isExport && !_allowTotal) ? 2 : 1;
            UnitInformation unit = UnitsInformation.Get(_session.Unit);
            #endregion

            #region Rappel de sélection
            // TODO : Commented temporarily for new AdExpress
            //if (_isExcelReport)
            //{
            //    if (_isCreativeDivisionMS)
            //    {
            //        t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
            //    }
            //    else
            //    {
            //        if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE)
            //        {
            //            t.Append(FctExcel.GetLogo(_session));
            //            if (VehiclesInformation.Contains(_vehicleId) && (VehiclesInformation.Get(_vehicleId).Id == CstDBClassif.Vehicles.names.adnettrack || VehiclesInformation.Get(_vehicleId).Id == CstDBClassif.Vehicles.names.internet))
            //            {
            //                t.Append(FctExcel.GetExcelHeaderForAdnettrackMediaPlanPopUp(_session, false, Zoom, (int)_session.DetailPeriod));
            //            }
            //            else
            //            {
            //                try
            //                {
            //                    t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
            //                }
            //                catch (Exception)
            //                {
            //                    t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
            //                }
            //            }
            //        }
            //        else
            //        {
            //            t.Append(FctExcel.GetAppmLogo(_session));
            //            t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474, _session.SiteLanguage)));
            //        }
            //    }
            //}
            #endregion

            GenericDetailLevel detailLevel = GetDetailsLevelSelected();

            #region Columns

            #region basic columns (product, total, PDM, version, insertion, years totals)
            int rowSpanNb = 3;
            if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            {
                rowSpanNb = 2;
            }
            t.Append("<table id=\"calendartable\" border=1 cellpadding=0 cellspacing=0>\r\n\t<tr>");


            //Add Classification labels in columns
            for (int l = 1; l <= detailLevel.GetNbLevels; l++)
            {
                t.AppendFormat("\r\n\t\t<td colSpan=\"{2}\" rowspan=\"{1}\" width=\"250px\">{0}</td>"
               , TNS.FrameWork.Convertion.ToHtmlString(GestionWeb.GetWebWord(detailLevel[l].WebTextId, _session.SiteLanguage))
               , rowSpanNb
               , labColSpan);
            }

            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
            {
                if (_allowTotal)
                {
                    MediaSchedulePeriod compPeriod = _period.GetMediaSchedulePeriodComparative();
                    t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}", TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(compPeriod.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern), rowSpanNb);

                    int nbtot;
                    if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                        nbtot = Units.ConvertUnitValueToString(((CellIdsNumber)data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit, fp).Length;
                    else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                    {
                        nbtot = Units.ConvertUnitValueToString(data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit, fp).Length;
                    }
                    else
                        nbtot = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX])).Length;

                    int nbSpace = (nbtot - 1) / 3;
                    int nbCharTotal = nbtot + nbSpace - 5;
                    if (nbCharTotal < 5) nbCharTotal = 0;
                    for (int h = 0; h < nbCharTotal; h++)
                    {
                        t.Append("&nbsp;");
                    }
                    t.Append("</td>");
                }
                //PDM
                if (_allowPdm)
                {
                    t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", GestionWeb.GetWebWord(806, _session.SiteLanguage), rowSpanNb);
                }
            }


            // Total Column
            if (_allowTotal)
            {
                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)                    
                    t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}", TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.Begin, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern) + " - <br/>" + TNS.AdExpress.Web.Core.Utilities.Dates.DateToString(_period.End, _session.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.shortDatePattern), rowSpanNb);
                else t.AppendFormat("\r\n\t\t<td rowspan={1} >{0}",GestionWeb.GetWebWord(805, _session.SiteLanguage), rowSpanNb);

          
                int nbtot;
                if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                    nbtot = Units.ConvertUnitValueToString(((CellIdsNumber)data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit, fp).Length;
                else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                {
                    nbtot = Units.ConvertUnitValueToString(data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
                }
                else
                    nbtot = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX])).Length;

                int nbSpace = (nbtot - 1) / 3;
                int nbCharTotal = nbtot + nbSpace - 5;
                if (nbCharTotal < 5) nbCharTotal = 0;
                for (int h = 0; h < nbCharTotal; h++)
                {
                    t.Append("&nbsp;");
                }
                t.Append("</td>");
            }
            //PDM
            if (_allowPdm)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>",  GestionWeb.GetWebWord(806, _session.SiteLanguage), rowSpanNb);
                
            }
            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", GestionWeb.GetWebWord(1212, _session.SiteLanguage), rowSpanNb);
               
               
            }
            //Version
            if (_allowVersion)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(1994, _session.SiteLanguage), rowSpanNb);
            }
            // Insertions
            if (_allowInsertions)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].WebTextId, _session.SiteLanguage), rowSpanNb);
            }
            if (!WebApplicationParameters.UseComparativeMediaSchedule)
            {
                // Years necessary if the period consists of several years
                for (int k = firstPeriodIndex - nbColYear; k < firstPeriodIndex && _allowTotal; k++)
                {
                    t.AppendFormat("\r\n\t\t<td rowspan=\"{1}\" >{0}</td>", data[0, k], rowSpanNb);
                   
                }
            }
            #endregion

            #region Period
            nbPeriod = 0;
            int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            StringBuilder periods = new StringBuilder();
            StringBuilder headers = new StringBuilder();
            string periodClass;
            string link = string.Empty;
            System.Uri uri = new Uri(_session.LastWebPage);
            link = uri.AbsolutePath;

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
                                headers.AppendFormat("<td colspan={0} >&nbsp;</td>", nbPeriod);
                             
                            else
                                headers.AppendFormat("<td colspan={0} >{1}</td>", nbPeriod, prevPeriod);
                                
                            nbPeriod = 0;
                            prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));
                        }

                        switch (_period.PeriodDetailLEvel)
                        {
                            case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                #region Period Color Management
                                // First Period or last period is incomplete
                                periodClass = _style.CellPeriod;
                                if ((j == firstPeriodIndex && _period.Begin.Day != 1)
                                   || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
                                {
                                    periodClass = _style.CellPeriodIncomplete;
                                }
                                #endregion

                                if (!isExport)
                                {
                                    periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{2}?idSession={3}&zoomDate={4}\">&nbsp;{5}&nbsp;</td>"
                                        , periodClass
                                        , _style.CellPeriod
                                        , link
                                        , _session.IdSession
                                        , data[0, j]
                                        , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));
                                }
                                else
                                {
                                    periods.AppendFormat("<td >&nbsp;{0}&nbsp;</td>"                                    
                                       , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));
                                   
                                }
                                break;
                            case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                #region Period Color Management
                                periodClass = _style.CellPeriod;
                                if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
                                   || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
                                {
                                    periodClass = _style.CellPeriodIncomplete;
                                }
                                #endregion

                                if (!isExport && !IsCreativeDivisionMS)
                                {
                                    periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{0}?idSession={1}&zoomDate={2}\">&nbsp;{3}&nbsp;</a></td>"                                     
                                       , link
                                       , _session.IdSession
                                       , data[0, j]
                                       , data[0, j].ToString().Substring(4, 2));

                                   
                                }
                                else
                                {
                                    periods.AppendFormat("<td >&nbsp;{0}&nbsp;</td>"                           
                                      , data[0, j].ToString().Substring(4, 2));                                
                                }
                                break;

                        }
                        nbPeriod++;
                    }
                    // Compute last date
                    if (nbPeriod < 3)
                        headers.AppendFormat("<td colspan={0} >&nbsp;</td>", nbPeriod);
                     
                    else
                        headers.AppendFormat("<td colspan={0}>{1}</td>", nbPeriod, prevPeriod);

                    t.AppendFormat("{0}</tr>", headers);

                    t.AppendFormat("<tr>{0}</tr>", periods);

                    oMediaScheduleData.Headers = t.ToString();

                    // t.Append("\r\n\t<tr>");

                    break;
                case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                    StringBuilder days = new StringBuilder();
                    periods.Append("<tr>");
                    days.Append("\r\n\t<tr>");
                    DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
                    prevPeriod = currentDay.Month;
                    currentDay = currentDay.AddDays(-1);
                    for (int j = firstPeriodIndex; j < nbColTab; j++)
                    {
                        currentDay = currentDay.AddDays(1);
                        if (currentDay.Month != prevPeriod)
                        {
                            if (nbPeriod >= 8)
                                headers.AppendFormat("<td colspan=\"{0}\" align=center>{1}</td>", nbPeriod, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")));
                            else
                                headers.AppendFormat("<td colspan=\"{0}\" align=center>&nbsp;</td>", nbPeriod);
                            nbPeriod = 0;
                            prevPeriod = currentDay.Month;
                        }
                        nbPeriod++;
                        //Period Number
                        periods.AppendFormat("<td nowrap>&nbsp;{0}&nbsp;</td>",currentDay.ToString("dd"));

                        //Period day
                        if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                            days.AppendFormat("<td >{0}</td>",  DayString.GetCharacters(currentDay, cultureInfo, 1));
                        else
                            days.AppendFormat("<td >{0}</td>", DayString.GetCharacters(currentDay, cultureInfo, 1));

                    }
                    if (nbPeriod >= 8)
                        headers.AppendFormat("<td colspan=\"{0}\" align=center>{1}</td>", nbPeriod, TNS.FrameWork.Convertion.ToHtmlString(TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.ToString("yyyyMM"))));
                    else
                        headers.AppendFormat("<td colspan=\"{0}\"  align=center>&nbsp;</td>", nbPeriod);

                    periods.Append("</tr>");
                    days.Append("</tr>");
                    headers.Append("</tr>");

                    t.Append(headers);
                    t.Append(periods);
                    t.Append(days);

                    oMediaScheduleData.Headers = t.ToString();
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
                var classifLabels = new string[detailLevel.GetNbLevels];

                for (i = 1; i < nbline; i++)
                {

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
                            else if (_isPDFReport && VehiclesInformation.Contains(_vehicleId))
                            {
                                switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
                                {
                                    case CstDBClassif.Vehicles.names.directMarketing:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, cssClasse));
                                        break;
                                    case CstDBClassif.Vehicles.names.outdoor:
                                    case CstDBClassif.Vehicles.names.dooh:
                                    case CstDBClassif.Vehicles.names.indoor:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, cssClasse));
                                        break;
                                    case CstDBClassif.Vehicles.names.instore:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportInstoreVersionItem(sloganId, cssClasse));
                                        break;
                                    default:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, cssClasse));
                                        break;
                                }

                            }
                            colorItemIndex++;
                        }
                        if (sloganId != 0 && !oMediaScheduleData.VersionsDetail.ContainsKey(sloganId))
                        {
                            if (_allowVersion)
                            {
                                oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                            }
                            else if (_isPDFReport && VehiclesInformation.Contains(_vehicleId))
                            {
                                switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
                                {
                                    case CstDBClassif.Vehicles.names.directMarketing:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                    case CstDBClassif.Vehicles.names.outdoor:
                                    case CstDBClassif.Vehicles.names.dooh:
                                    case CstDBClassif.Vehicles.names.indoor:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                    case CstDBClassif.Vehicles.names.instore:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportInstoreVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                    default:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                }

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
                            case CstFrameWorkResult.MediaSchedule.L1_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    if (i == CstFrameWorkResult.MediaSchedule.TOTAL_LINE_INDEX)
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
                                    classifLabels[0] = Convert.ToString(data[i, j]);
                                    AppenLabelTotalPDM(data, t, i, cssClasse, cssClasseNb, j, string.Empty, labColSpan, fp, unit,classifLabels,1);

                                    if (_allowVersion)
                                    {
                                        if (i != CstFrameWorkResult.MediaSchedule.TOTAL_LINE_INDEX && !IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L1_COLUMN_INDEX))
                                        {
                                            AppendCreativeLink(data, t, themeName, i, cssClasse, j);
                                        }
                                        else
                                        {
                                            t.AppendFormat("<td align=\"center\"></td>");
                                        }

                                    }
                                    if (_allowInsertions)
                                    {
                                        if (i != CstFrameWorkResult.MediaSchedule.TOTAL_LINE_INDEX && !IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L1_COLUMN_INDEX))
                                        {
                                            AppendInsertionLink(data, t, themeName, i, cssClasse, j);
                                        }
                                        else
                                        {
                                            t.AppendFormat("<td align=\"center\"></td>");
                                        }
                                    }
                                    //Totals
                                    if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                    {
                                        for (int k = 1; k <= nbColYear; k++)
                                        {
                                            AppendYearsTotal(data, t, i, cssClasseNb, (j + firstPeriodIndex - nbColYear - 1) + k, fp, unit);
                                        }
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 2
                            case CstFrameWorkResult.MediaSchedule.L2_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    classifLabels[1] = Convert.ToString(data[i, j]);
                                    AppenLabelTotalPDM(data, t, i, _style.CellLevelL2, _style.CellLevelL2Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 2);
                                    if (_allowVersion)
                                    {
                                        if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L2_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL2, j);
                                        else t.AppendFormat("<td align=\"center\" ></td>");
                                    }
                                    if (_allowInsertions)
                                    {
                                        if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L2_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL2, j);
                                        else t.AppendFormat("<td align=\"center\" ></td>");
                                    }
                                    if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                    {
                                        for (int k = 1; k <= nbColYear; k++)
                                        {
                                            AppendYearsTotal(data, t, i, _style.CellLevelL2Nb, j + (firstPeriodIndex - nbColYear - 2) + k, fp, unit);
                                        }
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case CstFrameWorkResult.MediaSchedule.L3_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    classifLabels[2] = Convert.ToString(data[i, j]);
                                    AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 3);
                                    if (_allowVersion)
                                    {
                                        if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L3_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL3, j);
                                        else t.AppendFormat("<td align=\"center\"></td>");
                                    }
                                    if (_allowInsertions)
                                    {
                                        if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L3_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL3, j);
                                        else t.AppendFormat("<td align=\"center\"></td>");
                                    }
                                    if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                    {
                                        for (int k = 1; k <= nbColYear; k++)
                                        {
                                            AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear - 3) + k, fp, unit);
                                        }
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 4
                            case CstFrameWorkResult.MediaSchedule.L4_COLUMN_INDEX:
                                classifLabels[3] = Convert.ToString(data[i, j]);
                                AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, string.Empty, labColSpan, fp, unit, classifLabels, 4);
                                if (_allowVersion)
                                {
                                    if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L4_COLUMN_INDEX)) AppendCreativeLink(data, t, themeName, i, _style.CellLevelL4, j);
                                    else t.AppendFormat("<td align=\"center\" ></td>");
                                }
                                if (_allowInsertions)
                                {
                                    if (!IsAgencyLevelType(CstFrameWorkResult.MediaSchedule.L4_COLUMN_INDEX)) AppendInsertionLink(data, t, themeName, i, _style.CellLevelL4, j);
                                    else t.AppendFormat("<td align=\"center\"></td>");
                                }
                                if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                {
                                    for (int k = 1; k <= nbColYear; k++)
                                    {
                                        AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear - 4) + k, fp, unit);
                                    }
                                }
                                j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                break;
                            #endregion

                            #region Other
                            default:
                                if (data[i, j] == null)
                                {
                                    t.AppendFormat("<td >&nbsp;</td>");
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
                                                    t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItemIds)data[i, j]).IdsNumber.Value, _session.Unit, fp));
                                                else if (_isCreativeDivisionMS || !IsExcelReport || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                                                    t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, Units.ConvertUnitValueToString(((MediaPlanItem)data[i, j]).Unit, _session.Unit, fp));
                                                else
                                                    t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, string.Format(fp, unit.StringFormat, ((MediaPlanItem)data[i, j]).Unit));
                                            }
                                            else
                                            {
                                                t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, stringItem);
                                            }
                                            break;
                                        case DetailledMediaPlan.graphicItemType.extended:
                                            t.AppendFormat("<td class=\"{0}\">&nbsp;</td>", cssExtendedClass);
                                            break;
                                        default:
                                            t.AppendFormat("<td class=\"{0}\"nbsp;</td>","NpX");
                                            break;
                                    }
                                }
                                break;
                            #endregion

                        }
                    }
                    t.Append("</tr>");
                    #endregion

                }
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleException("Error i=" + i, err));
            }
            t.Append("</table>");
            #endregion

            // Release table
            data = null;
            // TODO : Commented temporarily for new AdExpress
            //if (_isExcelReport && !_isCreativeDivisionMS)
            //{
            //    t.Append(FctExcel.GetFooter(_session));
            //}

            oMediaScheduleData.HTMLCode = t.ToString();
            return oMediaScheduleData;

        }




        #endregion
        #region Excel design Table
        /// <summary>
        /// Provide Excel page to present Media Schedule for ANUBIS
        /// </summary>
        /// <param name="data">Preformated Data</param>
        /// <param name="excel">Object Excel for compute a page lan media</param>
        protected virtual void ComputeDesignExcel(object[,] data, Workbook excel, TNS.FrameWork.WebTheme.Style styleExcel)
        {

            if (data.GetLength(0) != 0)
            {

                #region Init Variables
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                UnitInformation unitInformation = _session.GetSelectedUnit();

                #region Excel
                Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                Cells cells = sheet.Cells;
                string formatTotal = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(unitInformation.Format);
                string formatPdm = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(EXCEL_PATTERN_NAME_PERCENTAGE);

                bool premier = true;
                int header = 1;
                string prevYearString = string.Empty;
                int nbMaxRowByPage = 42;
                int s = 1;
                int cellRow = 5;
                int startIndex = cellRow;
                int upperLeftColumn = 10;
                string vPageBreaks = "";
                double columnWidth = 0, indexLogo = 0, index;
                bool verif = true;
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
                string presentstyle = string.Empty;
                string extendedStyle = string.Empty;
                string style = string.Empty;
                string styleNb = string.Empty;
                string stylePdmNb = string.Empty;
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
                        firstPeriodIndex = CstFrameWorkResult.MediaSchedule.EVOL_COLUMN_INDEX + 1;
                    }
                    else
                    {
                        firstPeriodIndex =  CstFrameWorkResult.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
                    }
                }
                else
                {
                    firstPeriodIndex =  CstFrameWorkResult.MediaSchedule.L4_ID_COLUMN_INDEX + 1;
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
                cells.Merge(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(804, _session.SiteLanguage), cellRow - 1, colSupport, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
                cells[cellRow - 1, colSupport].Style.HorizontalAlignment = TextAlignmentType.Left;
                cells[cellRow - 1, colSupport].Style.VerticalAlignment = TextAlignmentType.Top;
                styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                {
                    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colSupport);
                }
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
                    cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(805, _session.SiteLanguage), cellRow - 1, colTotal, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
                    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colSupport);
                    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    {
                        styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotal);
                    }
                    int nbtot = Units.ConvertUnitValueToString(data[1, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit, fp).Length;
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
                    cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
                    WorkSheet.PutCellValue(excel, cells, GestionWeb.GetWebWord(806, _session.SiteLanguage), cellRow - 1, colPdm, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
                    styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colPdm);
                    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    {
                        styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colPdm);
                    }
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
                        cells.Merge(cellRow - 1, colTotalYears + l, rowSpanNb, labColSpan);
                        styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow, colTotalYears + l);
                        if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                        {
                            styleExcel.GetTag("MediaPlanCellTitle").SetStyleExcel(excel, cells, cellRow + 1, colTotalYears + l);
                        }
                        WorkSheet.PutCellValue(excel, cells, data[0, k], cellRow - 1, colTotalYears + l, colFirstMediaPlan, "MediaPlanCellTitle", null, styleExcel);
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
                string periodStyle = string.Empty;
                switch (_period.PeriodDetailLEvel)
                {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                        prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++)
                        {
                            if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
                            {
                                cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                                if (nbPeriod < 3)
                                    WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
                                else
                                    WorkSheet.PutCellValue(excel, cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);

                                styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel)
                            {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    #region Period Color Management
                                    // First Period or last period is incomplete
                                    periodStyle = "MediaPlanCellPeriod";
                                    if ((j == firstPeriodIndex && _period.Begin.Day != 1)
                                       || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
                                    {
                                        periodStyle = "MediaPlanCellPeriodIncomplete";
                                    }
                                    #endregion

                                    WorkSheet.PutCellValue(excel, cells, MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null, styleExcel);
                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    #region Period Color Management
                                    periodStyle = "MediaPlanCellPeriod";
                                    if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
                                       || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
                                    {
                                        periodStyle = "MediaPlanCellPeriodIncomplete";
                                    }
                                    #endregion

                                    WorkSheet.PutCellValue(excel, cells, int.Parse(data[0, j].ToString().Substring(4, 2)), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null, styleExcel);
                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }
                        // Compute last date
                        cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                        if (nbPeriod < 3)
                            WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);
                        else
                            WorkSheet.PutCellValue(excel, cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);

                        for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
                        {
                            cells[startIndex - 1, k].Style.Number = 1;
                            if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
                                cells[startIndex, k].Style.Number = 1;
                        }

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
                                cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                                if (nbPeriod >= 8)
                                    WorkSheet.PutCellValue(excel, cells, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
                                else
                                    WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear1", null, styleExcel);
                                styleExcel.GetTag("MediaPlanCellYear1").SetStyleExcel(excel, cells, startIndex - 1, nbColTabFirst + nbPeriod);
                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            WorkSheet.PutCellValue(excel, cells, currentDay.ToString("dd"), startIndex, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPeriod", null, styleExcel);
                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDayWE", null, styleExcel);
                            else
                                WorkSheet.PutCellValue(excel, cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellDay", null, styleExcel);
                        }


                        cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                        if (nbPeriod >= 8)
                            WorkSheet.PutCellValue(excel, cells, TNS.AdExpress.Web.Core.Utilities.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);
                        else
                            WorkSheet.PutCellValue(excel, cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, "MediaPlanCellYear", null, styleExcel);


                        for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++)
                        {
                            cells[startIndex, k].Style.Number = 1;
                        }


                        break;

                }
                #endregion

                #endregion

                #region init Row Media Shedule
                cellRow++;

                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    cellRow++;
                #endregion

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

                            presentstyle = _session.SloganColors[sloganId].ToString();
                            extendedStyle = _session.SloganColors[sloganId].ToString();
                            stringItem = "x";
                        }
                        else
                        {
                            presentstyle = "MediaPlanCellPresent";
                            extendedStyle = "MediaPlanCellExtended";
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
                                case CstFrameWorkResult.MediaSchedule.L1_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {

                                        if (data[i, j].GetType() == typeof(MemoryArrayEnd))
                                        {
                                            i = int.MaxValue - 2;
                                            j = int.MaxValue - 2;
                                            break;
                                        }

                                        #region Style define
                                        if (i == CstFrameWorkResult.MediaSchedule.TOTAL_LINE_INDEX)
                                        {
                                            style = "MediaPlanCellLevelTotal";
                                            styleNb = "MediaPlanCellLevelTotalNb";
                                            stylePdmNb = "MediaPlanCellLevelTotalPdmNb";
                                        }
                                        else
                                        {
                                            style = "MediaPlanCellLevelL1";
                                            styleNb = "MediaPlanCellLevelL1Nb";
                                            stylePdmNb = "MediaPlanCellLevelL1PdmNb";
                                        }
                                        #endregion

                                        #region Label
                                        WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null, styleExcel);
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal, styleExcel);
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, CstFrameWorkResult.MediaSchedule.PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm, styleExcel);
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                        {
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, j + (firstPeriodIndex - nbColYear - 1) + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal, styleExcel);
                                        }
                                        #endregion

                                        j = j + (firstPeriodIndex - nbColYear - 1) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case CstFrameWorkResult.MediaSchedule.L2_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {

                                        #region Style define
                                        if (premier)
                                        {
                                            style = "MediaPlanCellLevelL2_1";
                                            styleNb = "MediaPlanCellLevelL2_1Nb";
                                            stylePdmNb = "MediaPlanCellLevelL2_1PdmNb";
                                        }
                                        else
                                        {
                                            style = "MediaPlanCellLevelL2_2";
                                            styleNb = "MediaPlanCellLevelL2_2Nb";
                                            stylePdmNb = "MediaPlanCellLevelL2_2PdmNb";
                                        }
                                        #endregion

                                        #region Label
                                        WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null, styleExcel);
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal, styleExcel);
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, CstFrameWorkResult.MediaSchedule.PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm, styleExcel);
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++)
                                        {
                                            WorkSheet.PutCellValue(excel, cells, ((double)data[i, j + (firstPeriodIndex - nbColYear - 2) + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal, styleExcel);
                                        }
                                        #endregion

                                        premier = !premier;

                                        j = j + (firstPeriodIndex - nbColYear - 2) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case CstFrameWorkResult.MediaSchedule.L3_COLUMN_INDEX:
                                    if (data[i, j] != null)
                                    {
                                        WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL3", null, styleExcel);
                                        /*
                                        AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
                                        */
                                        if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                        {
                                            for (int k = 1; k <= nbColYear; k++)
                                            {
                                                //AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + (firstPeriodIndex - nbColYear-3) + k);
                                            }
                                        }
                                        j = j + (firstPeriodIndex - nbColYear - 3) + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case CstFrameWorkResult.MediaSchedule.L4_COLUMN_INDEX:
                                    WorkSheet.PutCellValue(excel, cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, "MediaPlanCellLevelL4", null, styleExcel);
                                    //AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);
                                    if (!WebApplicationParameters.UseComparativeMediaSchedule)
                                    {
                                        for (int k = 1; k <= nbColYear; k++)
                                        {
                                            //AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + (firstPeriodIndex - nbColYear-4) + k);
                                        }
                                    }
                                    j = j + (firstPeriodIndex - nbColYear - 4) + nbColYear;
                                    break;
                                #endregion

                                #region Other
                                default:
                                    if (data[i, j] == null)
                                    {
                                        WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);
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
                                                    WorkSheet.PutCellValue(excel, cells, ((MediaPlanItem)data[i, j]).Unit, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", formatTotal, styleExcel);
                                                }
                                                else
                                                {
                                                    WorkSheet.PutCellValue(excel, cells, stringItem, cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellPresent", null, styleExcel);
                                                }
                                                break;
                                            case DetailledMediaPlan.graphicItemType.extended:
                                                WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellExtended", null, styleExcel);
                                                break;
                                            default:
                                                WorkSheet.PutCellValue(excel, cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, "MediaPlanCellNotPresent", null, styleExcel);
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
                    throw (new MediaScheduleException("Error i=" + i, err));
                }
                #endregion

                #region Mise en forme de la page

                #region Ajustement de la taile des cellules en fonction du contenu
                sheet.AutoFitColumn(colSupport);

                for (int c = colFirstMediaPlan; c <= (nbColTabCell + 1 - colFirstMediaPlan); c++)
                {
                    if (_showValues)
                    {
                        sheet.AutoFitColumn(c);
                    }
                    else
                    {
                        cells.SetColumnWidth((byte)c, 2);
                    }
                }

                #endregion

                if (_session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.monthly)
                {
                    for (index = 0; index < 30; index++)
                    {
                        columnWidth += cells.GetColumnWidth((byte)index);
                        if ((columnWidth < 124) && verif)
                            indexLogo++;
                        else
                            verif = false;
                    }
                    upperLeftColumn = (int)indexLogo - 1;
                    vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), data.GetLength(0) + 3, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), styleExcel);
                }
                else
                {
                    if (nbColTabCell > 44)
                    {
                        upperLeftColumn = nbColTabCell - 4;
                        vPageBreaks = cells[cellRow, nbColTabCell - 4].Name;
                    }
                    else
                    {
                        for (index = 0; index < 30; index++)
                        {
                            columnWidth += cells.GetColumnWidth((byte)index);
                            if ((columnWidth < 124) && verif)
                                indexLogo++;
                            else
                                verif = false;
                        }
                        upperLeftColumn = (int)indexLogo - 1;
                        vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                    }

                    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), ref s, upperLeftColumn, header.ToString(), styleExcel);
                }
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// Append Label, Total and PDM
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="cssClasseNb">Line syle for numbers</param>
        /// <param name="col">Column to consider</param>
        /// <param name="padding">Stirng to insert before Label</param>
        protected virtual void AppenLabelTotalPDM(object[,] data, StringBuilder t, int line, string cssClasse, string cssClasseNb, int col, string padding, int labColSpan, IFormatProvider fp, UnitInformation unit,string[] classifLabels,int currentLevel)
        {
            if (_session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                t.AppendFormat("\r\n\t<tr>");
                for (int i = 1; i <= classifLabels.Length; i++)
                {
                    t.AppendFormat("\r\n\t\t<td colSPan=\"{0}\">{1}{2}</td>"
                                   , labColSpan
                                   , padding
                                   ,
                                   (i > currentLevel
                                        ? GestionWeb.GetWebWord(1401, _session.SiteLanguage)
                                        : classifLabels[i - 1]));

                }

                 
                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    if (_allowTotal)
                    {
                        if (data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                            t.AppendFormat("<td >{0}</td>"                               
                                , Units.ConvertUnitValueToString(((CellIdsNumber)data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX]).Value, _session.Unit, fp));//class=\"{0}\"
                        else
                            t.AppendFormat("<td >&nbsp;</td>");//class=\"{0}\"
                    }
                    if (_allowPdm)
                    {
                        t.AppendFormat("<td >{0}</td>"                          
                            , string.Format(fp, "{0:percentWOSign}", data[line, CstFrameWorkResult.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]));//class=\"{0}\"
                    }
                }
                if (_allowTotal)
                {
                    if (data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX] != null)
                        t.AppendFormat("<td>{0}</td>"                          
                            , Units.ConvertUnitValueToString(((CellIdsNumber)data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX]).Value, _session.Unit, fp));
                    else
                        t.AppendFormat("<td>&nbsp;</td>");
                }
                if (_allowPdm)
                {
                    t.AppendFormat("<td >{0}</td>"                       
                        , string.Format(fp, "{0:pdm}", data[line, CstFrameWorkResult.MediaSchedule.PDM_COLUMN_INDEX]));
                }
            }
            else
            {
                t.AppendFormat("\r\n\t<tr>");
                 for (int i = 1; i <= classifLabels.Length;i++ )
                 {
                     t.AppendFormat("\r\n\t\t<td colSPan=\"{0}\">{1}{2}</td>"
                                    , labColSpan
                                    , padding
                                    ,
                                    (i > currentLevel
                                         ? GestionWeb.GetWebWord(1401, _session.SiteLanguage)
                                         : classifLabels[i - 1]) );

                 }

                if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy)
                {
                    if (_allowTotal)
                    {
                        string s = string.Empty;
                        if (data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX] != null)
                        {
                            if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                            {
                                s = Units.ConvertUnitValueToString(data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX], _session.Unit, fp).Trim();
                            }
                            else
                            {
                                s = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COMPARATIVE_COLUMN_INDEX])).Trim();
                            }
                        }
                        else s = "&nbsp;";

                        t.AppendFormat("<td >{0}</td>"                           
                            , s);
                    }
                    if (_allowPdm)
                    {
                        t.AppendFormat("<td >{0}</td>"                          
                            , string.Format(fp, "{0:percentWOSign}", data[line, CstFrameWorkResult.MediaSchedule.PDM_COMPARATIVE_COLUMN_INDEX]));
                    }
                }
                if (_allowTotal)
                {
                    string s = string.Empty;
                    if (!_isExcelReport || _isCreativeDivisionMS || unit.Id != CstWeb.CustomerSessions.Unit.duration)
                    {
                        s = Units.ConvertUnitValueToString(data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX], _session.Unit, fp).Trim();
                    }
                    else
                    {
                        s = string.Format(fp, unit.StringFormat, Convert.ToDouble(data[line, CstFrameWorkResult.MediaSchedule.TOTAL_COLUMN_INDEX])).Trim();
                    }

                    t.AppendFormat("<td>{0}</td>"                      
                        , s);
                }
                if (_allowPdm)
                {
                    t.AppendFormat("<td>{0}</td>"                       
                        , string.Format(fp, "{0:pdm}", data[line, CstFrameWorkResult.MediaSchedule.PDM_COLUMN_INDEX]));

                }
            }
            if (WebApplicationParameters.UseComparativeMediaSchedule && _session.ComparativeStudy && _allowTotal)
            {
                //Evol
                StringBuilder str = new StringBuilder();
                double evol = (double)data[line, CstFrameWorkResult.MediaSchedule.EVOL_COLUMN_INDEX];
                if (evol != 0)
                {
                    if (Double.IsInfinity(evol))
                    {
                       
                        //str.Append((evol < 0) ? "-" : "+");
                    }
                    else if (Double.IsNaN(evol))
                    {
                        str.Append("&nbsp;");
                    }
                    else if (evol == 0)
                    {
                        str.Append("0");
                    }
                    else
                    {
                        str.Append(string.Format(fp, "{0:percentWOSign}", evol));
                    }
                }
                else
                {
                    str.Append("&nbsp;");
                }
               
                //Evol
                t.AppendFormat("<td nowrap=\"nowrap\">{0}</td>"
                       , str.ToString());
            }
        }

    }
}
