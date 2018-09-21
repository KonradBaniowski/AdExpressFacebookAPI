#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion


using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain;
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

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.Portofolio.Engines
{
    /// <summary>
    /// Compute media insertion detail's results
    /// </summary>
    public class InsertionDetailEngine : Engine
    {

        #region Constantes
        protected const char SEPARATOR = '°';
        protected const string CARRIAGE_RETURN = "<br/>";
        protected const string EXCEL_CARRIAGE_RETURN = "<br class=\"mso\"/>";
        #endregion

        #region Variables
        /// <summary>
        /// Screen code
        /// </summary>
        protected string _adBreak;
        /// <summary>
        /// Day of Week
        /// </summary>
        protected string _dayOfWeek;
        /// <summary>
        /// Define if show media schedule Link
        /// </summary>
        protected bool _showMediaSchedule = false;
        /// <summary>
        /// Result type
        /// </summary>
        protected bool _excel = false;
        /// <summary>
        /// Define if the study is for all the periods
        /// </summary>
        protected bool _allPeriod = true;       
        /// <summary>
        /// Define if we can show creatives
        /// </summary>
        protected bool _showCreative = false;
        /// <summary>
        /// Define if we can show Mediia Agency
        /// </summary>
        protected bool _showMediaAgency = false;
        /// <summary>
        /// Define if we can show date
        /// </summary>
        protected bool _showDate = true;
        /// <summary>
        /// Define if we can show products
        /// </summary>
        protected bool _showProduct;
        /// <summary>
        /// Column item list
        /// </summary>
        protected List<GenericColumnItemInformation> _columnItemList;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected VehicleInformation _vehicle;

        /// <summary>
        /// Show top diffusion
        /// </summary>
        protected bool _showTopDiffusion = true;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) { }
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
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
            _adBreak = adBreak;
            _dayOfWeek = dayOfWeek;
            _showMediaSchedule = webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null ? true : false;
            _excel = excel;
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
            int iNbLine = 0;
            Assembly assembly;
            Type type;
            bool allPeriod = true;
            bool isDigitalTV = false;
            _vehicle = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            #endregion

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            var parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _adBreak;
            if (!string.IsNullOrEmpty(_adBreak) || !string.IsNullOrEmpty(_dayOfWeek)) _allPeriod = false;
            var portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName),
                _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);


             _showTopDiffusion = CanShowTopDiffusion(_vehicleInformation, portofolioDAL);

            #region Product detail level (Generic)
            // Initialisation to product
            var levels = new ArrayList {10};
            _webSession.GenericProductDetailLevel = new GenericDetailLevel(levels,
                TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            #endregion

            #region Columns levels (Generic)
            _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicle.DetailColumnId);

            var columnIdList = _columnItemList.Select(column => (int) column.Id).Select(dummy => (long) dummy).ToList();

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
                if (_vehicleInformation.Id == Vehicles.names.press
                    || _vehicleInformation.Id == Vehicles.names.internationalPress
                    || _vehicleInformation.Id == Vehicles.names.newspaper
                    || _vehicleInformation.Id == Vehicles.names.magazine
                    )
                    SetDataTable(dt, _dayOfWeek, _allPeriod);
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

                if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id) && _vehicleInformation.ShowCreations) _showCreative = true;
                // Show media agency

                if (_webSession.CustomerLogin.CustomerMediaAgencyFlagAccess(_vehicleInformation.DatabaseId) && dt.Columns.Contains("advertising_agency"))
                    _showMediaAgency = true;

                //Show diffusion date

                if (!_allPeriod && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    ))
                    _showDate = false;
                //Show column product
                _showProduct = _webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                #endregion

                #region Table nb rows
                iNbLine = dt.Rows.Count;
                #endregion

                #region Initialisation of result table
                try
                {
                    headers = new Headers();
                    _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicle.DetailColumnId);

                    foreach (GenericColumnItemInformation Column in _columnItemList)
                    {

                        switch (Column.Id)
                        {
                            case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                            case GenericColumnItemInformation.Columns.visual://Visual press
                                if (_showCreative)
                                    headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.agenceMedia://media agency
                                if (_showMediaAgency)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.planMedia://Plan media
                                if (_showMediaSchedule)
                                    headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.dateDiffusion:
                            case GenericColumnItemInformation.Columns.dateParution:
                                if (_showDate)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.topDiffusion:
                                if (_showTopDiffusion)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.product:
                                if (_showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                                break;
                            default:
                                if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, Column.Id))
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

                SetResultTable(dt, tab);

            }

            return tab;
        }

        #endregion

        #region CanShowTopDiffusion

        /// <summary>
        /// Can Show Top Diffusion
        /// </summary>
        /// <param name="vehicleInformation"></param>
        /// <param name="portofolioDAL"></param>
        /// <returns></returns>
        protected bool CanShowTopDiffusion(VehicleInformation vehicleInformation, IPortofolioDAL portofolioDAL)
        {
            string idCategories = string.Empty;
            if (vehicleInformation.Id == Vehicles.names.tv
                && !_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
            {
                idCategories = Lists
                                  .GetIdList(WebCst.GroupList.ID.category, WebCst.GroupList.Type.digitalTv);
            }

            MediaItemsList tntMediaItems = null;
            if (Media.Contains(WebCst.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION))
                tntMediaItems = Media.GetItemsList(WebCst.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION);

            if (tntMediaItems != null && !string.IsNullOrEmpty(tntMediaItems.CategoryList))
            {
                idCategories = !string.IsNullOrEmpty(idCategories)
                                   ? string.Format("{0},{1}", idCategories, tntMediaItems.CategoryList)
                                   : tntMediaItems.CategoryList;
            }

            if (!string.IsNullOrEmpty(idCategories))
                _showTopDiffusion = !portofolioDAL.IsMediaBelongToCategory(_idMedia, idCategories);

            if (_showTopDiffusion && tntMediaItems != null && !string.IsNullOrEmpty(tntMediaItems.MediaList))
            {
                _showTopDiffusion =
                    !new List<string>(tntMediaItems.MediaList.Split(',')).ConvertAll(Convert.ToInt64).Contains(_idMedia);
            }

            return _showTopDiffusion;
        }

        #endregion

        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override string BuildHtmlResult()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }
        #endregion

        #region Compute DataTable for Press and   interntional Press
        /// <summary>
        /// Adapte data for vehcilce Press and interntional press
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>DataTable</returns>
        protected virtual void SetDataTable(DataTable dt, string dayOfWeek, bool allPeriod)
        {

            #region Variables
            Int64 idOldLine = -1;
            Int64 idLine = -1;
            DataRow oldRow = null;
            int iLine = 0;
            ArrayList indexLines = new ArrayList();
            #endregion

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0 && dt.Columns.Contains("id_advertisement"))
            {

                #region Parcours du tableau
                foreach (DataRow row in dt.Rows)
                {

                    if (dayOfWeek == row["date_media_num"].ToString() || allPeriod)
                    {

                        idLine = (long)row["id_advertisement"];

                        if (idLine != idOldLine)
                        {
                            idOldLine = idLine;
                            oldRow = row;
                        }
                        else
                        {
                            if (oldRow["location"].ToString().Length > 0 && row["location"].ToString().Length > 0)
                                oldRow["location"] = oldRow["location"].ToString() + "-" + row["location"].ToString();
                            else if (oldRow["location"].ToString().Length == 0 && row["location"].ToString().Length > 0)
                                oldRow["location"] = row["location"].ToString();
                            indexLines.Add(iLine);
                        }
                    }

                    iLine++;
                }
                #endregion

                indexLines.Reverse();
                //suppress rows
                foreach (int index in indexLines)
                    dt.Rows.Remove(dt.Rows[index]);

            }
        }
        #endregion

        #region SplitStringValue
        /// <summary>
        /// Split string value
        /// </summary>
        /// <param name="s">string</param>
        /// <returns></returns>
        protected virtual string SplitStringValue(string s)
        {
            string[] sArr = s.Split(SEPARATOR);
            string stringValue = string.Empty;
            for (int z = 0; z < sArr.Length; z++)
            {
                stringValue += sArr[z].ToString();
                if (sArr.Length > 1 && z < sArr.Length - 1)
                    stringValue += (_excel) ? EXCEL_CARRIAGE_RETURN : CARRIAGE_RETURN;
            }
            s = stringValue;
            return s;
        }
        #endregion

        #region SetResultTable
        /// <summary>
        /// SetResultTable
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="tab">Result table</param>
        protected virtual void SetResultTable(DataTable dt, ResultTable tab)
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
                                        if (  row[Column.DataBaseField].ToString().Length > 0 )
                                        {
                                            // Creation
                                            files = row[Column.DataBaseField].ToString().Split(',');
                                            foreach (string str in files)
                                            {
                                                if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                                    listVisual += string.Format("/ImagesPresse/{0}/{1}/{2},", _idMedia, row["date_media_num"]                                                                               
                                                                                , str);
                                                else listVisual += string.Format("/ImagesPresse/{0}/{1}/{2},", _idMedia, row["date_cover_num"]                                                                               
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
                                case GenericColumnItemInformation.Columns.dateMediaNum:
                                case GenericColumnItemInformation.Columns.Day:
                                case GenericColumnItemInformation.Columns.Month:
                                case GenericColumnItemInformation.Columns.Year:
                                case GenericColumnItemInformation.Columns.MonthYear:
                                case GenericColumnItemInformation.Columns.DayName:
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
                                        var columnLabel = string.IsNullOrEmpty(Column.DataBaseAliasField)
                                            ? row[Column.DataBaseField]
                                            : row[Column.DataBaseAliasField];
                                        curCell.SetCellValue(GetColumnValue(Column, columnLabel));
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

        #region GetColumnValue
        /// <summary>
        /// Get column value depending on if the value has a separator or not
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="value">Value of the cell</param>
        /// <returns>Value</returns>
        /// <remarks>We have add this method to solve cobranding problem for Russia</remarks>
        protected object GetColumnValue(GenericColumnItemInformation column, object value)
        {

            string s = string.Empty;
            if (column.IsContainsSeparator)
            {
                if (value != null)
                    s = SplitStringValue(value.ToString());
                else
                    s = string.Empty;
                return s;
            }
            else
                return value;

        }

        protected override GridResult BuildGridResult()
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
