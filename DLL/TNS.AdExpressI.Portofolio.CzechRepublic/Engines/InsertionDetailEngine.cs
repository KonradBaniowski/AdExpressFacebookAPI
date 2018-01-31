#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.CzechRepublic.Engines
{
  
          /// <summary>
    /// CzechRepublic InsertionDetail engine
    /// </summary>
    public class InsertionDetailEngine : AbstractResult.InsertionDetailEngine {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, string adBreak, string dayOfWeek, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd,adBreak, dayOfWeek, excel)
        {           
        }
        #endregion

        #region Abstract methods implementation

        #region ComputeResultTable
        /// <summary>
        /// Get portofolio media detail insertions 
        /// </summary>
        /// <returns>Result table</returns>
        protected override ResultTable ComputeResultTable()
        {

            #region Variables
            ResultTable tab = null;
            DataSet ds;
            DataTable dt = null;
            Headers headers;
            List<GenericColumnItemInformation> columnItemList;
            int iCurLine = 0;
            int iNbLine = 0;
            Assembly assembly;
            Type type;
            bool allPeriod = true;
            bool isDigitalTV = false;
            VehicleInformation vehicle = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            #endregion

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _adBreak;
            if ((_adBreak != null && _adBreak.Length > 0) || (_dayOfWeek != null && _dayOfWeek.Length > 0)) allPeriod = false;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            string idTNTCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebCst.GroupList.ID.category, WebCst.GroupList.Type.digitalTv);
            if (idTNTCategory != null && idTNTCategory.Length > 0)
                isDigitalTV = portofolioDAL.IsMediaBelongToCategory(_idMedia, idTNTCategory);

            #region Product detail level (Generic)
            // Initialisation to product
            ArrayList levels = new ArrayList();
            levels.Add(10);
            _webSession.GenericProductDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            #endregion

            #region Columns levels (Generic)
            columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(vehicle.DetailColumnId);

            List<Int64> columnIdList = new List<Int64>();
            foreach (GenericColumnItemInformation Column in columnItemList)
                columnIdList.Add((int)Column.Id);

            _webSession.GenericInsertionColumns = new GenericColumns(columnIdList);
            _webSession.Save();
            #endregion

            #region Data loading
            try
            {
                ds = portofolioDAL.GetInsertionData(); //portofolioDAL.GetGenericDetailMedia();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Error while extracting portofolio media detail data", err));
            }
            #endregion

            #region Press and Internatioanl Press cases
            try
            {
                if ( _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    )
                    SetDataTable(dt, _dayOfWeek, allPeriod);
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Error while deleting rows (case of press) fro portofolio media detail", err));
            }
            #endregion

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {

                #region Rigths management des droits
                // Show creatives
                bool showCreative = false;
                if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id) && _vehicleInformation.ShowCreations) showCreative = true;
                // Show media agency
                bool showMediaAgency = false;
                if (_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_MEDIA_AGENCY) && dt.Columns.Contains("advertising_agency"))
                {
                    showMediaAgency = true;
                }
                //Show diffusion date
                bool showDate = true;
                if (!allPeriod && (
                     _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    ))
                    showDate = false;
                //Show column product
                bool showProduct = _webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                #endregion

                #region Table nb rows
                iNbLine = dt.Rows.Count;
                #endregion

                #region Initialisation of result table
                try
                {
                    headers = new Headers();
                    columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(vehicle.DetailColumnId);

                    foreach (GenericColumnItemInformation Column in columnItemList)
                    {

                        switch (Column.Id)
                        {
                            case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                            case GenericColumnItemInformation.Columns.visual://Visual press
                                if (showCreative)
                                    headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.agenceMedia://media agency
                                if (showMediaAgency)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.planMedia://Plan media
                                if (_showMediaSchedule)
                                    headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.dateDiffusion:
                            case GenericColumnItemInformation.Columns.dateParution:
                                if (showDate)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.topDiffusion:
                                if (!isDigitalTV)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.product:
                                if (showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(vehicle.DetailColumnId, Column.Id))
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            default:
                                if (WebApplicationParameters.GenericColumnsInformation.IsVisible(vehicle.DetailColumnId, Column.Id))
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                        }

                    }

                    tab = new ResultTable(iNbLine, headers);
                }
                catch (System.Exception err)
                {
                    throw (new PortofolioException("Error while initiating headers of portofolio media detail", err));
                }
                #endregion

                #region Generation of table
                string[] files;
                string listVisual = "";
                int iCurColumn = 0;
                Cell curCell = null;
                string date = "";
                string dateMediaNum = "";
                DateTime dateMedia;

                try
                {

                    // assembly loading
                    assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
                    if (_mediaList == null)
                    {
                        try
                        {
                            string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                            if (mediaList != null && mediaList.Length > 0)
                                _mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(mediaList, (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
                        }
                        catch { }
                    }
                    foreach (DataRow row in dt.Rows)
                    {

                        #region Initialisation of dateMediaNum
                        switch (_vehicleInformation.Id)
                        {                            
                            case DBClassificationConstantes.Vehicles.names.newspaper:
                            case DBClassificationConstantes.Vehicles.names.magazine:                           
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

                        if (_dayOfWeek == dateMediaNum || allPeriod)
                        {

                            tab.AddNewLine(LineType.level1);
                            iCurColumn = 1;
                            foreach (GenericColumnItemInformation Column in columnItemList)
                            {
                                switch (Column.Id)
                                {
                                    case GenericColumnItemInformation.Columns.visual://Visual press
                                        if (showCreative)
                                        {
                                            if (row[Column.DataBaseField].ToString().Length > 0)
                                            {
                                                // Creation
                                                files = row[Column.DataBaseField].ToString().Split(',');
                                                foreach (string str in files)
                                                {
                                                    //listVisual += "/ImagesPresse/" + _idMedia + "/" + row["date_media_num"] + "/" + str + ",";
                                                    listVisual += "/ImagesPresse/" + str + ",";
                                                }
                                                if (listVisual.Length > 0)
                                                {
                                                    listVisual = listVisual.Substring(0, listVisual.Length - 1);
                                                }
                                                tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(listVisual, _webSession);
                                                listVisual = "";
                                            }
                                            else
                                                tab[iCurLine, iCurColumn++] = new CellPressCreativeLink("", _webSession);
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                                        if (showCreative)
                                        {
                                            switch (_vehicleInformation.Id)
                                            {
                                                case DBClassificationConstantes.Vehicles.names.radio:
                                                    tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                                    break;
                                                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                                                    tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                                    break;
                                                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                                                    tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                                    break;
                                                case DBClassificationConstantes.Vehicles.names.radioMusic:
                                                    tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
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
                                        if (showMediaAgency)
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
                                        if (showDate)
                                        {
                                            type = assembly.GetType(Column.CellType);
                                            curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                            curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                            date = row[Column.DataBaseField].ToString();
                                            if (date.Length > 0)
                                                curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
                                            else
                                                curCell.SetCellValue(null);
                                            tab[iCurLine, iCurColumn++] = curCell;
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.topDiffusion:
                                    case GenericColumnItemInformation.Columns.idTopDiffusion:
                                        if (!isDigitalTV)
                                        {
                                            if (row[Column.DataBaseField].ToString().Length > 0)
                                                tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
                                            else
                                                tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(0);
                                            curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.product:
                                        if (showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(vehicle.DetailColumnId, Column.Id))
                                        {
                                            type = assembly.GetType(Column.CellType);
                                            curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                            curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                            curCell.SetCellValue(row[Column.DataBaseField]);
                                            tab[iCurLine, iCurColumn++] = curCell;
                                        }
                                        break;
                                    default:
                                        if (WebApplicationParameters.GenericColumnsInformation.IsVisible(vehicle.DetailColumnId, Column.Id))
                                        {
                                            type = assembly.GetType(Column.CellType);
                                            curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                            curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                            curCell.SetCellValue(row[Column.DataBaseField]);
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
                #endregion

            }

            return tab;
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
            string[] files;
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
                if (_mediaList == null)
                {
                    try
                    {
                        string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                        if (mediaList != null && mediaList.Length > 0)
                            _mediaList = new List<string>(mediaList).ConvertAll(Convert.ToInt64);
                    }
                    catch { }
                }


                //string blur = TNS.AdExpress.Web.Functions.Rights.HasPressCopyright(_idMedia) ? string.Empty : "blur/";
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

                    if (_dayOfWeek == dateMediaNum || _allPeriod)
                    {

                        tab.AddNewLine(LineType.level1);
                        iCurColumn = 1;
                        foreach (GenericColumnItemInformation Column in _columnItemList)
                        {
                            switch (Column.Id)
                            {
                                case GenericColumnItemInformation.Columns.visual://Visual press
                                    if (_showCreative)
                                    {
                                        if (row[Column.DataBaseField].ToString().Length > 0)
                                        {
                                            // Creation
                                            files = row[Column.DataBaseField].ToString().Split(',');
                                            foreach (string str in files)
                                            {
                                                if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                                    listVisual += string.Format("/ImagesPresse/{0}/},", row["visual"]
                                                                                , str);
                                                else listVisual += string.Format("/ImagesPresse/{0}/,", row["visual"]
                                                                                , str);
                                            }
                                            if (listVisual.Length > 0)
                                            {
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
                                    if (_showCreative)
                                    {
                                        switch (_vehicleInformation.Id)
                                        {
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
                                    if (_showDate)
                                    {
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
                                    if (_showTopDiffusion)
                                    {
                                        long TOP_DIFFUSION_VISIBILITY_DATE = 20170904;
                                        var MEDIA_IDS_TOP_DIFFUSION_VISIBILITY = new List<long> { 3168, 18560, 18564, 24982 };
                                        long dateNum = Convert.ToInt64(row["date_media_num"]);
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


                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        if (row[Column.DataBaseField].ToString().Length > 0
                                            && !(MEDIA_IDS_TOP_DIFFUSION_VISIBILITY.Contains(idMedia) && dateNum < TOP_DIFFUSION_VISIBILITY_DATE))
                                            curCell = new CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
                                        else
                                            curCell = new CellEmpty();//new CellAiredTime(0);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.product:
                                    if (_showProduct && WebApplicationParameters.
                                        GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
                                    {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        curCell.SetCellValue(GetColumnValue(Column, row[Column.DataBaseField]));
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                default:
                                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
                                    {
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
            catch (System.Exception err)
            {
                throw (new PortofolioException("Error while generating result table of portofolio media detail", err));
            }
        }
        #endregion

        #endregion
    }
}
