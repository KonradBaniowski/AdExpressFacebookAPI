using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Portofolio.Exceptions;

namespace TNS.AdExpressI.Portofolio.Ireland.Engines {
    public class InsertionDetailEngine : TNS.AdExpressI.Portofolio.Engines.InsertionDetailEngine {

        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation,
                                     long idMedia, string periodBeginning, string periodEnd,
                                     string adBreak, string dayOfWeek, bool excel) :
            base(
                                         webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, adBreak,
                                         dayOfWeek, excel) {
        }

        #region SetResultTable
        /// <summary>
        /// SetResultTable
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="tab">Result table</param>
        protected override void SetResultTable(DataTable dt, ResultTable tab) {

            string dateMediaNum = string.Empty;
            DateTime dateMedia;
            int iCurLine = 0;
            int iCurColumn = 0;
            string[] files;
            string listVisual = "";
            Cell curCell = null;
            string date = "";
            string temp = string.Empty;
            Assembly assembly;
            Type type;

            try {
                // assembly loading
                assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
                if (_mediaList == null) {
                    try {
                        string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                        if (mediaList != null && mediaList.Length > 0)
                            _mediaList = new List<string>(mediaList).ConvertAll(Convert.ToInt64);
                    }
                    catch { }
                }
                foreach (DataRow row in dt.Rows) {

                    #region Initialisation of dateMediaNum
                    switch (_vehicleInformation.Id) {
                        case DBClassificationConstantes.Vehicles.names.press:
                        case DBClassificationConstantes.Vehicles.names.newspaper:
                        case DBClassificationConstantes.Vehicles.names.magazine:
                        case DBClassificationConstantes.Vehicles.names.internationalPress:
                            dateMediaNum = row["date_media_num"].ToString();
                            break;
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.radio:
                        case DBClassificationConstantes.Vehicles.names.radioGeneral:
                        case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                        case DBClassificationConstantes.Vehicles.names.radioMusic:
                        case DBClassificationConstantes.Vehicles.names.others:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                            dateMedia = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
                            dateMediaNum = dateMedia.DayOfWeek.ToString();
                            break;
                    }
                    #endregion

                    if (_dayOfWeek == dateMediaNum || _allPeriod) {

                        tab.AddNewLine(LineType.level1);
                        iCurColumn = 1;
                        foreach (GenericColumnItemInformation Column in _columnItemList) {
                            switch (Column.Id) {
                                case GenericColumnItemInformation.Columns.visual://Visual press
                                    if (_showCreative) {
                                        if (row[Column.DataBaseField].ToString().Length > 0) {
                                            // Creation
                                            files = row[Column.DataBaseField].ToString().Split(',');
                                            foreach (string str in files) {
                                                if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                                    listVisual += string.Format("/ImagesPresse/{0}/{1}/{2},", _idMedia, row["date_media_num"], str);
                                                else listVisual += string.Format("/ImagesPresse/{0}/{1}/{2},", _idMedia, row["date_cover_num"], str);
                                            }
                                            if (listVisual.Length > 0) {
                                                listVisual = listVisual.Substring(0, listVisual.Length - 1);
                                            }
                                            tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(listVisual, _webSession);
                                            listVisual = "";
                                        }
                                        else
                                            tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(string.Empty, _webSession);
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                                    if (_showCreative) {
                                        switch (_vehicleInformation.Id) {
                                            case Vehicles.names.press:
                                                tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(row["associated_file"].ToString(), _webSession);
                                                break;
                                            case Vehicles.names.radio:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(),
                                                    _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                                break;
                                            case Vehicles.names.radioMusic:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(),
                                                    _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
                                                break;
                                            case Vehicles.names.radioSponsorship:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(),
                                                    _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                                break;
                                            case Vehicles.names.radioGeneral:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString()
                                                    , _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                                break;
                                            case Vehicles.names.tv:
                                            case Vehicles.names.tvGeneral:
                                            case Vehicles.names.tvSponsorship:
                                            case Vehicles.names.tvAnnounces:
                                            case Vehicles.names.tvNonTerrestrials:
                                            case Vehicles.names.others:
                                                if (row[Column.DataBaseField].ToString().Length > 0)
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(
                                                        Convert.ToString(row[Column.DataBaseField]), _webSession, _vehicleInformation.Id.GetHashCode());
                                                else
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(string.Empty,
                                                        _webSession, _vehicleInformation.Id.GetHashCode());

                                                break;
                                        }
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.agenceMedia://Media agncy
                                    if (_showMediaAgency) {
                                        tab[iCurLine, iCurColumn++] = new CellLabel(row["advertising_agency"].ToString());
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.planMedia://Plan media
                                    if (_showMediaSchedule)
                                        tab[iCurLine, iCurColumn++] = new CellInsertionMediaScheduleLink(_webSession, Convert.ToInt64(row["id_product"]), 1);
                                    break;
                                case GenericColumnItemInformation.Columns.dateParution:// Parution Date and  diffusion Date
                                case GenericColumnItemInformation.Columns.dateDiffusion:
                                    if (_showDate) {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        date = row[Column.DataBaseField].ToString();
                                        if (date.Length > 0)
                                            curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)),
                                                int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
                                        else
                                            curCell.SetCellValue(null);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.topDiffusion:
                                case GenericColumnItemInformation.Columns.idTopDiffusion:
                                    if (_showTopDiffusion) {
                                        if (row[Column.DataBaseField].ToString().Length > 0)
                                            tab[iCurLine, iCurColumn++] = new CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
                                        else
                                            tab[iCurLine, iCurColumn++] = new CellAiredTime(0);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.product:
                                    if (_showProduct && WebApplicationParameters.
                                        GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id)) {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        curCell.SetCellValue(GetColumnValue(Column, row[Column.DataBaseField]));
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                default:
                                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id)) {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        curCell.SetCellValue(GetColumnValue(Column, row[Column.DataBaseField]));
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                            }
                        }
                        iCurLine++;
                    }

                }
            }
            catch (System.Exception err) {
                throw (new PortofolioException("Error while generating result table of portofolio media detail", err));
            }
        }
        #endregion

    }
}
