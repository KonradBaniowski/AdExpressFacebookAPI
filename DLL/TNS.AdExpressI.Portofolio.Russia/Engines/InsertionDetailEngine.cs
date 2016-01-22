#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion


using System;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.Russia.Engines {
	/// <summary>
	/// Compute media insertion detail's results
	/// </summary>
	public class InsertionDetailEngine : TNS.AdExpressI.Portofolio.Engines.InsertionDetailEngine {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">vehicle Information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
        /// <param name="adBreak">Ad break</param>
        /// <param name="dayOfWeek">Day of week</param>
        /// <param name="excel">Excel</param>
		public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, string adBreak, string dayOfWeek, bool excel)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, adBreak, dayOfWeek, excel) {
		}
		#endregion

        #region SetResultTable
        /// <summary>
        /// SetResultTable
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="tab">Result table</param>
        protected override void SetResultTable(DataTable dt, ResultTable tab)
        {

            string dateMediaNum = string.Empty;
            DateTime dateMedia;
            int iCurLine = 0;
            int iCurColumn = 0;
            string listVisual = "";
            Cell curCell = null;
            string date = "";
            string temp = string.Empty;
            Assembly assembly;
            Type type;

            try
            {
                // assembly loading
                assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
                foreach (DataRow row in dt.Rows)
                {

                    #region Initialisation of dateMediaNum
                    switch (_vehicleInformation.Id)
                    {
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

                    if (CanAddLine(dateMediaNum,row))
                    {

                        tab.AddNewLine(LineType.level1);
                        iCurColumn = 1;
                        foreach (GenericColumnItemInformation Column in _columnItemList)
                        {
                            switch (Column.Id)
                            {
                               
                                case GenericColumnItemInformation.Columns.associatedFile://Visual Pres/radio/tv
                                    if (_showCreative)
                                    {
                                        switch (_vehicleInformation.Id)
                                        {
                                            case DBClassificationConstantes.Vehicles.names.press :
                                                if (row[Column.DataBaseField] != System.DBNull.Value && row[Column.DataBaseField].ToString().Length > 0
                                            && row.Table.Columns.Contains("id_slogan") && row["id_slogan"] != System.DBNull.Value)
                                                {
                                                    // Creation
                                                    Int64 advertisementId = Convert.ToInt64(row["id_slogan"].ToString());
                                                    int directoryName = (int)(advertisementId / 10000) * 10000;
                                                    string[] files = row["associated_file"].ToString().Split(',');
                                                    foreach (string s in files)
                                                    {
                                                        listVisual += string.Format("{0}/{1}/{2}", CstWeb.CreationServerPathes.IMAGES, directoryName, s) + ",";
                                                    }
                                                    listVisual = listVisual.Substring(0, listVisual.Length - 1);
                                                    tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(listVisual, _webSession);
                                                    listVisual = "";
                                                }
                                                else
                                                    tab[iCurLine, iCurColumn++] = new CellPressCreativeLink("", _webSession);
                                                break;
                                            case DBClassificationConstantes.Vehicles.names.radio:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                                break;
                                            case DBClassificationConstantes.Vehicles.names.radioMusic:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
                                                break;
                                            case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                                break;
                                            case DBClassificationConstantes.Vehicles.names.radioGeneral:
                                                tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                                break;
                                            case DBClassificationConstantes.Vehicles.names.tv:
                                            case DBClassificationConstantes.Vehicles.names.tvGeneral:
                                            case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                                            case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                                            case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                                            case DBClassificationConstantes.Vehicles.names.others:
                                                if (row[Column.DataBaseField].ToString().Length > 0)
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(Convert.ToString(row[Column.DataBaseField]), _webSession, _vehicleInformation.Id.GetHashCode());
                                                else
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(string.Empty, _webSession, _vehicleInformation.Id.GetHashCode());

                                                break;
                                        }
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.agenceMedia://Media agncy
                                    if (_showMediaAgency)
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel(row["advertising_agency"].ToString());
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.planMedia://Plan media
                                    if (_showMediaSchedule)
                                        tab[iCurLine, iCurColumn++] = new CellInsertionMediaScheduleLink(_webSession, Convert.ToInt64(row["id_product"]), 1);
                                    break;
                                case GenericColumnItemInformation.Columns.dateParution:// Parution Date and  diffusion Date
                                case GenericColumnItemInformation.Columns.dateDiffusion:
                                case GenericColumnItemInformation.Columns.firstIssueDate:
                                case GenericColumnItemInformation.Columns.firstDateParution:
                                    //if (_showDate)
                                    //{
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        date = row[Column.DataBaseField].ToString();
                                        if (date.Length > 0)
                                            curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
                                        else
                                            curCell.SetCellValue(null);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    //}
                                    break;
                                case GenericColumnItemInformation.Columns.topDiffusion:
                                case GenericColumnItemInformation.Columns.idTopDiffusion:
                                    if (_showTopDiffusion)
                                    {
                                        if (row[Column.DataBaseField].ToString().Length > 0)
                                            tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
                                        else
                                            tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(0);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.product:
                                    if (_showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
                                    {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        curCell.SetCellValue(GetColumnValue(Column, row[Column.DataBaseField]));
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                default:
                                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
                                    {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        if(!string.IsNullOrEmpty(Column.StringFormat))
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
            catch (System.Exception err)
            {
                throw (new PortofolioException("Error while generating result table of portofolio media detail", err));
            }
        }
        #endregion


        protected bool CanAddLine(string dateMediaNum, DataRow row)
        {
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    if (_dayOfWeek == dateMediaNum || _allPeriod)
                    {
                        return true;
                    }
                    return false;
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
                     string screenCode = row["screencode"].ToString();

                   
                    if ((_dayOfWeek == dateMediaNum && _adBreak == screenCode) || _allPeriod)
                    {
                        return true;
                    }
                    return false;
                    break;
                default: return false;
            }
            
            return false;
        }

	}
}
