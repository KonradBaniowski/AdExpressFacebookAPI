using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Insertions.Turkey
{
    public class InsertionsResult : Insertions.InsertionsResult
    {
        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId) : base(session, moduleId) { }
        #endregion

        #region SetRawLine
        protected override void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int index1 = -1;
            int index2 = 0;
            long TOP_DIFFUSION_VISIBILITY_DATE = 20170904;
            var MEDIA_IDS_TOP_DIFFUSION_VISIBILITY = new List<long> { 3168, 18560, 18564, 24982 };
            foreach (GenericColumnItemInformation columnItemInformation1 in columns)
            {
                ++index1;
                ++index2;
                if (cells[index1] is CellUnit)
                {
                    int val1 = 1;
                    if (columnItemInformation1.IsSum)
                        val1 = Math.Max(val1, row[divideCol].ToString().Split(',').Length);
                    double num1 = 0.0;
                    if (row[columnsName[index1]] != DBNull.Value)
                        num1 = Convert.ToDouble(row[columnsName[index1]]) / (double)val1;
                    switch (columns[index1].Id)
                    {
                        case GenericColumnItemInformation.Columns.topDiffusion:
                        case GenericColumnItemInformation.Columns.SpotEndTime:
                            if (IsTvVehicle(vehicle))
                            {
                                long num2 = Convert.ToInt64(row[WebApplicationParameters.GenericColumnItemsInformation.Get(89L).DataBaseField].ToString());
                                if (MediasWithoutTopDif != null &&
                                    MediasWithoutTopDif.Contains(
                                        Convert.ToInt64(
                                            row[
                                                WebApplicationParameters.GenericColumnItemsInformation.Get(3L)
                                                    .DataBaseIdField].ToString()))
                                    || CategoriesWithoutTopDif != null && CategoriesWithoutTopDif.Contains(num2))
                                {
                                    num1 = 0.0;
                                }
                                else
                                {
                                    long dateMediaNum = Convert.ToInt64(row["date_media_num"]);
                                    long idMedia = Convert.ToInt64(row["id_media"]);

                                    /*A partir de la date_media 04 / 09, ces chaines 
                                     * 3168 - FRANCE O
                                     * 18560 - HD1
                                     * 18564 - NUMERO 23
                                     * 24982 – RMC DECOUVERTE S
                                     * devront être gérées comme de la pige réelle (catégorie 31), 
                                    donc elles ne constitueront plus un cas particulier. En revanche attention, toutes les lignes qui ont une date_media au 03 / 09 
                                        et avant doivent rester gérées comme aujourd’hui.Le changement dans la règle de transfert ne concerne que les données à partir du 04 / 09.
                                        */
                                    if (MEDIA_IDS_TOP_DIFFUSION_VISIBILITY.Contains(idMedia) && dateMediaNum < TOP_DIFFUSION_VISIBILITY_DATE)
                                        num1 = 0.0;
                                }


                            }
                            break;
                        case GenericColumnItemInformation.Columns.volume:
                            if (!_session.CustomerLogin.CustormerFlagAccess(252L))
                            {
                                num1 = 0.0;

                            }
                            break;
                        case GenericColumnItemInformation.Columns.weight:
                            if (!_session.CustomerLogin.CustormerFlagAccess(256L))
                            {
                                num1 = 0.0;

                            }
                            break;
                    }
                    if (tab[cLine, index2] == null)
                    {
                        if (num1 == 0.0 
                            && (columns[index1].Id == GenericColumnItemInformation.Columns.topDiffusion || columns[index1].Id == GenericColumnItemInformation.Columns.SpotEndTime))
                            tab[cLine, index2] = new CellAiredTime(null);
                        else
                        {
                            if (columns[index1].Id == GenericColumnItemInformation.Columns.topDiffusion
                                || columns[index1].Id == GenericColumnItemInformation.Columns.SpotEndTime)
                            {
                                tab[cLine, index2] = ((CellUnit) cells[index1]).Clone(GetTopDiffusion(num1));
                            }
                            else
                            {
                                tab[cLine, index2] = ((CellUnit) cells[index1]).Clone(num1);
                            }
                        }
                    }
                    else
                    {
                        ((CellUnit)tab[cLine, index2]).Add(num1);
                    }
                }
                else
                {
                    string str = string.Empty;
                    //if (_getInsertionsExcel && columns[index1].Id == GenericColumnItemInformation.Columns.dateMediaNum)
                    if (columns[index1].Id == GenericColumnItemInformation.Columns.dateMediaNum)
                    {
                        int num = Convert.ToInt32(row[columnsName[index1]]);
                        int year = num / 10000;
                        int month = (num - 10000 * year) / 100;
                        int day = num - (10000 * year + 100 * month);
                        tab[cLine, index2] = new CellDate(new DateTime(year, month, day), string.Format("{{0:{0}}}", columnItemInformation1.StringFormat));
                    }
                    else if (columns[index1].Id == GenericColumnItemInformation.Columns.Day 
                        || columns[index1].Id == GenericColumnItemInformation.Columns.Month
                        || columns[index1].Id == GenericColumnItemInformation.Columns.Year
                        || columns[index1].Id == GenericColumnItemInformation.Columns.MonthYear
                        || columns[index1].Id == GenericColumnItemInformation.Columns.DayName)
                    {
                        AdExpressCultureInfo cultureInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
                        int num = Convert.ToInt32(row[columnsName[index1]]);
                        int year = num / 10000;
                        int month = (num - 10000 * year) / 100;
                        int day = num - (10000 * year + 100 * month);
                        var date = new DateTime(year, month, day);
                        
                        switch (columns[index1].Id)
                        {
                            case GenericColumnItemInformation.Columns.Day:
                                tab[cLine, index2] = new CellNumber(day);
                                ((CellNumber)tab[cLine, index2]).StringFormat = string.Format("{{0:{0}}}", "codeEcran");
                                break;
                            case GenericColumnItemInformation.Columns.Year:
                                tab[cLine, index2] = new CellNumber(year);
                                ((CellNumber)tab[cLine, index2]).StringFormat = string.Format("{{0:{0}}}", "codeEcran");
                                break;
                            case GenericColumnItemInformation.Columns.Month:
                            case GenericColumnItemInformation.Columns.MonthYear:
                            case GenericColumnItemInformation.Columns.DayName:
                                tab[cLine, index2] = new CellLabel(date.ToString(cultureInfo.GetFormatPattern(columnItemInformation1.StringFormat),cultureInfo));
                                break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(row[columnsName[index1]].ToString()) &&
                        (columns[index1].Id == GenericColumnItemInformation.Columns.ProgramBeginningTime
                      || columns[index1].Id == GenericColumnItemInformation.Columns.ProgramEndingTime
                      || columns[index1].Id == GenericColumnItemInformation.Columns.CommercialBreakBeginningTime
                      || columns[index1].Id == GenericColumnItemInformation.Columns.CommercialBreakEndingTime))
                    {
                            DateTime date = ToDateTime(row[columnsName[index1]].ToString());
                            tab[cLine, index2] = new CellDate(date, string.Format("{{0:{0}}}", columnItemInformation1.StringFormat));
                    }
                    else
                    {
                        switch (columns[index1].Id)
                        {
                            case GenericColumnItemInformation.Columns.slogan:
                                string label1 = row[columnsName[index1]].ToString();
                                if (!this._session.CustomerLogin.CustormerFlagAccess(221L))
                                    label1 = string.Empty;
                                if (tab[cLine, index2] == null || ((CellLabel)tab[cLine, index2]).Label.Length <= 0)
                                {
                                    tab[cLine, index2] = new CellLabel(label1);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label1);
                                break;
                            case GenericColumnItemInformation.Columns.product:
                                string label2 = row[columnsName[index1]].ToString();
                                if (!this._session.CustomerLogin.CustormerFlagAccess(271L))
                                    label2 = string.Empty;
                                if (tab[cLine, index2] == null || ((CellLabel)tab[cLine, index2]).Label.Length <= 0)
                                {
                                    tab[cLine, index2] = (ICell)new CellLabel(label2);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label2);
                                break;
                            case GenericColumnItemInformation.Columns.associatedFile:
                                switch (vehicle.Id)
                                {
                                    case Vehicles.names.radio:
                                        var sloganField = WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.slogan.GetHashCode()).DataBaseIdField;
                                        tab[cLine, index2] = row[sloganField].ToString().Length <= 0 ? new CellRadioCreativeLink(string.Empty, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio))
                                            : new CellRadioCreativeLink(row[sloganField].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                        break;
                                    case Vehicles.names.tv:
                                    case Vehicles.names.others:
                                        string creative = row[columnsName[index1]].ToString();
                                        tab[cLine, index2] = creative.Length <= 0 ? new CellTvCreativeLink(string.Empty, _session, vehicle.DatabaseId)
                                            : new CellTvCreativeLink(creative, _session, vehicle.DatabaseId);
                                        break;
                                }
                                break;
                            case GenericColumnItemInformation.Columns.dayOfWeek:
                                int num = Convert.ToInt32(row[columnsName[index1]]);
                                int year = num / 10000;
                                int month = (num - 10000 * year) / 100;
                                int day = num - (10000 * year + 100 * month);
                                tab[cLine, index2] = new CellDate(new DateTime(year, month, day), string.Format("{{0:{0}}}", columnItemInformation1.StringFormat));
                                break;
                            case GenericColumnItemInformation.Columns.mailFormat:
                                string label3 = row[columnsName[index1]].ToString() == "20" ?
                                    GestionWeb.GetWebWord(2241L, _session.SiteLanguage) : GestionWeb.GetWebWord(2240L, _session.SiteLanguage);
                                if (tab[cLine, index2] == null)
                                {
                                    tab[cLine, index2] = new CellLabel(label3);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label3);
                                break;
                            case GenericColumnItemInformation.Columns.WeekNumber:
                                var weekNb = row[columnsName[index1]].ToString();
                                if (string.IsNullOrEmpty(weekNb))
                                {
                                    tab[cLine, index2] = new CellLabel("");
                                }
                                else
                                {
                                    tab[cLine, index2] = new CellLabel(weekNb.Insert(4," / "));
                                }
                                break;
                            case GenericColumnItemInformation.Columns.LanguageOfTheAd:
                                var languageId = row[columnsName[index1]].ToString();
                                if (string.IsNullOrEmpty(languageId))
                                {
                                    tab[cLine, index2] = new CellLabel("");
                                }
                                else
                                {
                                    tab[cLine, index2] = new CellLabel(GetLanguageLabel(languageId));
                                }
                                break;
                            default:
                                if (tab[cLine, index2] == null)
                                {
                                    tab[cLine, index2] = new CellLabel(row[columnsName[index1]].ToString());
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, row[columnsName[index1]].ToString());
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        public override GridResult GetInsertionsGridResult(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId,
            string zoomDate)
        {
            GridResult gridResult = new GridResult {HasData = false};
            long nbRows = CountInsertions(vehicle, fromDate, toDate, filters, universId, zoomDate);
            if (nbRows == 0)
            {
                 gridResult = new GridResult { HasData = false };
                return (gridResult);

            }
            if (nbRows >
                Core.MAX_ALLOWED_INSERTION_VERSION_ROWS_NB)
            {
                gridResult.HasData = true;
                gridResult.HasMoreThanMaxRowsAllowed = true;
                return (gridResult);

            }

            int nbLines = 0;
            ResultTable _data = GetInsertions(vehicle, fromDate, toDate, filters, universId, zoomDate);
        

            if (_data != null)
            {
                for (int startline = 0; startline < _data.LinesNumber; startline++)
                {
                    if (!(_data[startline, 0] is LineHide))//Filtre (LineHide)
                    {
                        nbLines++;
                    }
                }

                if (nbLines > Core.MAX_ALLOWED_INSERTION_VERSION_ROWS_NB)
                {
                    gridResult.HasData = true;
                    gridResult.HasMoreThanMaxRowsAllowed = true;
                    return (gridResult);
                }

                ComputeGridData(gridResult, _data, nbLines);
            }
            else
            {
                gridResult.HasData = false;
                return (gridResult);
            }

            return gridResult;
        }

        protected override Insertions.Cells.CellCreativesTvInformation GetCellCreativesTvInformation(VehicleInformation vehicle,
              List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells)
        {
            return new Cells.CellCreativesTvInformation(_session, vehicle, columns, columnsName, cells, _module);
        }

        protected override Insertions.Cells.CellCreativesTvInformation GetCellCreativesTvInformation(VehicleInformation vehicle, List<GenericColumnItemInformation> columns,
            List<string> columnsName, List<Cell> cells, long idColumnsSet)
        {
            return new Cells.CellCreativesTvInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
        }

        private static DateTime ToDateTime(string datetime, char dateSpliter = '-', char timeSpliter = ':',
            char millisecondSpliter = ',')
        {
            datetime = datetime.Trim();
            datetime = datetime.Replace("  ", " ");
            string[] body = datetime.Split(' ');
            string[] date = body[0].Split(dateSpliter);
            int hour = 0, minute = 0, second = 0, millisecond = 0;
            if (body.Length == 2)
            {
                string[] tpart = body[1].Split(millisecondSpliter);
                string[] time = tpart[0].Split(timeSpliter);
                hour = Convert.ToInt32(time[0]);
                minute = Convert.ToInt32(time[1]);
                if (time.Length == 3) second = Convert.ToInt32(time[2]);
                if (tpart.Length == 2) millisecond = Convert.ToInt32(tpart[1]);
            }
            return new DateTime(1970, 1, 1, hour, minute, second, millisecond);
        }

        private string GetLanguageLabel(string languageId)
        {
            switch (languageId)
            {
                case "90":
                    return GestionWeb.GetWebWord(3241, _session.SiteLanguage).ToUpper();
                case "44":
                    return GestionWeb.GetWebWord(2478, _session.SiteLanguage).ToUpper();
                default:
                    return "";
            }
        }

        private double GetTopDiffusion(double value)
        {
            if (value.ToString().Length == 6)
            {
                //Hour
                string h = value.ToString().Substring(0, 2);
                // Minutes Seconds
                string ms = value.ToString().Substring(2, 4);

                // For Turkey 25h = 1h
                if (h == "25")
                    return Convert.ToDouble("1" + ms);

                if (h == "24")
                    return Convert.ToDouble(ms);

                return value;
            }

            return value;
        }
    }
}
