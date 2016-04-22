#region Information
// Author: Y. Rkaian & D. Mussuma
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;
using DBCst = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Portofolio.VehicleView;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.Classification.Universe;
using System.Linq;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Result;

namespace TNS.AdExpressI.Portofolio
{
    /// <summary>
    /// Portofolio Results
    /// </summary>
    public abstract class PortofolioResults : IPortofolioResults
    {

        #region Constantes
        protected const long TOTAL_LINE_INDEX = 0;
        protected const long DETAILED_PORTOFOLIO_EURO_COLUMN_INDEX = 2;
        protected const long DETAILED_PORTOFOLIO_INSERTION_COLUMN_INDEX = 3;
        protected const long DETAILED_PORTOFOLIO_DURATION_COLUMN_INDEX = 4;
        protected const long DETAILED_PORTOFOLIO_MMC_COLUMN_INDEX = 4;
        protected const long DETAILED_PORTOFOLIO_PAGE_COLUMN_INDEX = 5;
        #endregion

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Vehicle
        /// </summary>
		protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Media Id
        /// </summary>
        protected Int64 _idMedia;
        /// <summary>
        /// Date begin
        /// </summary>
        protected string _periodBeginning;
        /// <summary>
        /// Date end
        /// </summary>
        protected string _periodEnd;
        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Show creations in the result
        /// </summary>
        protected bool _showCreatives;
        /// <summary>
        /// Show insertions in the result
        /// </summary>
        protected bool _showInsertions;
        /// <summary>
        /// Screen code
        /// </summary>
        protected string _adBreak;
        /// <summary>
        /// Day of Week
        /// </summary>
        protected string _dayOfWeek;
        /// <summary>
        /// Table type
        /// </summary>
        protected TNS.AdExpress.Constantes.DB.TableType.Type _tableType;
        /// <summary>
        /// List of media to test for creative acces (press specific)
        /// </summary>
        protected List<long> _mediaList = null;
        /// <summary>
        /// result type
        /// </summary>
        protected int _resultType = 0;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Result type
        /// </summary>
        public int ResultType
        {
            get { return (_resultType); }
            set { _resultType = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
		public PortofolioResults(WebSession webSession)
        {
            if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
            _webSession = webSession;
            try
            {
                // Set Vehicle
                _vehicleInformation = GetVehicleInformation();
                //Set Media Id
                _idMedia = GetMediaId();
                // Period
                _periodBeginning = GetDateBegin();
                _periodEnd = GetDateEnd();
                // Module
                _module = ModulesList.GetModule(webSession.CurrentModule);
                _showCreatives = ShowCreatives();
                _showInsertions = ShowInsertions();
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to set parameters", err));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="adBreak">Screen code</param>
        /// <param name="dayOfWeek">Day of week</param>
        public PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek)
            : this(webSession)
        {
            _adBreak = adBreak;
            _dayOfWeek = dayOfWeek;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>		
        /// <param name="tableType">tableType</param>
        public PortofolioResults(WebSession webSession, TNS.AdExpress.Constantes.DB.TableType.Type tableType)
        {
            if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
            _webSession = webSession;
            try
            {
                // Set Vehicle
                _vehicleInformation = GetVehicleInformation();
                //Set Media Id
                _idMedia = GetMediaId();
                // Module
                _module = ModulesList.GetModule(webSession.CurrentModule);
                //Table type
                _tableType = tableType;
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to set parameters", err));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>		
        /// <param name="resultType">resultType</param>
        public PortofolioResults(WebSession webSession, int resultType)
            : this(webSession)
        {
            _resultType = resultType;

        }
        #endregion

        #region Implementation of abstract methods
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        ///  - SYNTHESIS (only result table)
        /// </summary>
        /// <returns>Result Table</returns>
        public virtual ResultTable GetResultTable()
        {
            Engines.Engine result;
            try
            {
                switch (_webSession.CurrentTab)
                {
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                        result = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _showInsertions, _showCreatives);
                        break;
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        result = new Engines.CalendarEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    default:
                        throw (new PortofolioException("Impossible to identified current tab "));
                }
            }
            catch (Exception err)
            {
                throw (new PortofolioException("Impossible to compute portofolio results", err));
            }

            return result.GetResultTable();
        }
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        ///  - SYNTHESIS (only result table)
        /// </summary>
        /// <returns>Result Table</returns>
        public GridResult GetGridResult()
        {
            ResultTable resultTable = GetResultTable();
            GridResult gridResult = new GridResult();
            string mediaSchedulePath = "/MediaSchedulePopUp";
            string insertionPath = "/Insertions";
            string versionPath = "/Creative";
            int tableWidth = 0;
           
            if (resultTable != null)
            {

                resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
                resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
                object[,] gridData = new object[resultTable.LinesNumber, resultTable.ColumnsNumber]; //+2 car ID et PID en plus  -  //_data.LinesNumber
                List<object> columns = new List<object>();
                List<object> schemaFields = new List<object>();
                List<object> columnsFixed = new List<object>();

                gridResult.HasData = true;

                //Hierachical ids for Treegrid
                columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "ID" });
                columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "PID" });
                List<object> groups = null;

                //Headers
                string colWidth = "200";
                if(_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS)
                    colWidth = "50%";

                if (resultTable.NewHeaders != null)
                {
                    for (int j = 0; j < resultTable.NewHeaders.Root.Count; j++)
                    {
                        groups = null;
                        string colKey = string.Empty;
                        if (resultTable.NewHeaders.Root[j].Count > 0)
                        {
                            groups = new List<object>();

                            int nbGroupItems = resultTable.NewHeaders.Root[j].Count;
                            for (int g = 0; g < nbGroupItems; g++)
                            {
                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "string", width = colWidth });
                                schemaFields.Add(new { name = colKey });
                                tableWidth = tableWidth + 150;
                            }
                           // colKey = string.Format("gr{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label,  group = groups });
                            //schemaFields.Add(new { name = colKey });
                        }
                        else
                        {
                            colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = colWidth });
                            schemaFields.Add(new { name = colKey });
                            if (j == 0) columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });
                            tableWidth = tableWidth + 150;
                        }

                    }
                }


                //table body rows
                for (int i = 0; i < resultTable.LinesNumber; i++) //_data.LinesNumber
                {
                    gridData[i, 0] = i; // Pour column ID
                    gridData[i, 1] = resultTable.GetSortedParentIndex(i); // Pour column PID

                    for (int k = 1; k < resultTable.ColumnsNumber - 1; k++)
                    {
                        var cell = resultTable[i, k];
                        var link = string.Empty;
                        if (cell is CellMediaScheduleLink)
                        {

                            var c = cell as CellMediaScheduleLink;
                            if (c != null)
                            {
                                link = c.GetLink();
                                if (!string.IsNullOrEmpty(link))
                                {
                                    link = string.Format("<center><a href='javascript:window.open(\"{0}?{1}\",  \"_blank\", \"toolbar=no,scrollbars=yes,resizable=yes,top=80,left=100,width=1200,height=700\"); void(0);'><span class='fa fa-search-plus'></span></a></center>"
                                     , mediaSchedulePath
                                     , link);
                                }
                            }
                            gridData[i, k + 1] = link;
                        }
                        else if (cell is CellOneLevelInsertionsLink)
                        {
                            var c = cell as CellOneLevelInsertionsLink;

                            if (c != null)
                            {
                                link = c.GetLink();
                                if (!string.IsNullOrEmpty(link))
                                {
                                    link = string.Format("<center><a href='javascript:window.open(\"{0}?{1}\",  \"_blank\", \"toolbar=no,scrollbars=yes,resizable=yes,top=80,left=100,width=1200,height=700\"); void(0);'><span class='fa fa-search-plus'></span></a></center>"
                             , insertionPath
                             , link);
                                }

                            }
                            gridData[i, k + 1] = link;
                        }
                        else if (cell is CellOneLevelCreativesLink)
                        {
                            var c = cell as CellOneLevelCreativesLink;

                            if (c != null)
                            {
                                link = c.GetLink();
                                if (!string.IsNullOrEmpty(link))
                                {
                                    link = string.Format("<center><a href='javascript:window.open(\"{0}?{1}\",  \"_blank\", \"toolbar=no,scrollbars=yes,resizable=yes,top=80,left=100,width=1200,height=700\"); void(0);'><span class='fa fa-search-plus'></span></a></center>"
                             , versionPath
                             , link);
                                }

                            }
                            gridData[i, k + 1] = link;
                        }
                        else gridData[i, k + 1] = cell.RenderString();
                    }
                }
                if (tableWidth > 920)
                    gridResult.NeedFixedColumns = true;
                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;
                gridResult.ColumnsFixed = columnsFixed;
                gridResult.Data = gridData;
            }
            else
            {
                gridResult.HasData = false;
            }
            return gridResult;

        }

        /// <summary>
        /// Get data for vehicle view
        /// </summary>
        /// <param name="dtVisuel">Visuel information</param>
        /// <param name="htValue">investment values</param>
        /// <returns>Media name</returns>
        public virtual void GetVehicleViewData(out DataTable dtVisuel, out Hashtable htValue)
        {
            Engines.Engine result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
            result.GetVehicleViewData(out dtVisuel, out htValue);
        }


        #region Structure
        /// <summary>
        /// Get Structure html result
        /// </summary>
        /// <param name="excel">True if export excel</param>
        /// <returns>html code</returns>
        public virtual string GetStructureHtml(bool excel)
        {
            Engines.StructureEngine result = null;
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();
                    result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList, excel);
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
                    Dictionary<string, double> hourEndList = new Dictionary<string, double>();
                    GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, excel);
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            return result.GetHtmlResult();
        }


        /// <summary>
        /// Get structure chart data
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetStructureChartData()
        {
            Engines.StructureEngine result = null;

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();
                    result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList, false);
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
                    Dictionary<string, double> hourEndList = new Dictionary<string, double>();
                    GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, false);
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            return result.GetChartData();
        }
        #endregion

        #region HTML detail media
        /// <summary>
        /// Get media detail html
        /// </summary>
        /// <param name="excel">true for excel result</param>
        /// <returns>HTML Code</returns>
        public virtual string GetDetailMediaHtml(bool excel)
        {
            Engines.MediaDetailEngine result = null;
            StringBuilder t = new StringBuilder(5000);

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                    result.GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1837, _webSession.SiteLanguage));
                    return t.ToString();
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel);
                    return result.GetHtmlResult();
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
        }
        #endregion

        #region Insetion detail
        /// <summary>
        /// Get media insertion detail
        /// </summary>
        /// <returns></returns>
        public virtual ResultTable GetInsertionDetailResultTable(bool excel)
        {
            Engines.InsertionDetailEngine result = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _adBreak, _dayOfWeek, excel);
            return result.GetResultTable();
        }
        #endregion

        #region Gets Visula list
        /// <summary>
        /// Get dates parution
        /// </summary>
        /// <param name="beginDate">Begin date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Dates parution</returns>
        public virtual Dictionary<string, string> GetVisualList(string beginDate, string endDate)
        {
            var dic = new Dictionary<string, string>();


            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            var parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = beginDate;
            parameters[4] = endDate;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            var portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                                                                                                    + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                                                                                                                                                                                                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            var ds = portofolioDAL.GetListDate(true, _tableType);

            if (_mediaList == null)
            {
                try
                {
                    string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                    if (mediaList != null && mediaList.Length > 0)
                        _mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(mediaList, Convert.ToInt64));
                }
                catch { }
            }
            dic.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["disponibility_visual"] != DBNull.Value && int.Parse(dr["disponibility_visual"].ToString()) >= 10)
                {
                    if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                        dic.Add(dr["date_media_num"].ToString(), string.Format("{0}/{1}/{2}/Imagette/{3}"
                            , WebCst.CreationServerPathes.IMAGES, _idMedia, dr["date_media_num"].ToString()
                            , WebCst.CreationServerPathes.COUVERTURE));
                    else dic.Add(dr["date_media_num"].ToString(), string.Format("{0}/{1}/{2}/Imagette/{3}"
                        , WebCst.CreationServerPathes.IMAGES, _idMedia, dr["date_cover_num"].ToString()
                        , WebCst.CreationServerPathes.COUVERTURE));
                }
                else
                    dic.Add(dr["date_media_num"].ToString(), "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif");
            }

            return dic;
        }
        #endregion

        /// <summary>
        /// Get vehicle cover items
        /// </summary>
        /// <returns>cover items</returns>
        public virtual List<VehicleItem> GetVehicleItems()
        {
            #region Variables
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            StringBuilder sb = new StringBuilder(5000);
            string pathWeb = "";
            CoverItem coverItem = null;
            CoverLinkItem coverLinkItem = null;
            CoverLinkItemSynthesis coverLinkItemSynthesis = null;
            VehicleItem vehicleItem = null;
            List<VehicleItem> itemsCollection = new List<VehicleItem>();
            DataTable dtVisuel = null;
            Hashtable htValue = null;
            #endregion

            // V�rifie si le client a le droit aux cr�ations
            if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id))
            {

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    )
                {


                    var parameters = new object[1];
                    parameters[0] = _webSession;
                    var portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.
                        CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                        + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                    portofolioResult.GetVehicleViewData(out dtVisuel, out htValue);


                    var cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
                    if (_mediaList == null)
                    {
                        try
                        {
                            string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                            if (mediaList != null && mediaList.Length > 0)
                                _mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(mediaList, Convert.ToInt64));
                        }
                        catch { }
                    }

                    if (dtVisuel != null)
                    {

                        for (int i = 0; i < dtVisuel.Rows.Count; i++)
                        {
                            //date_media_num

                            if (dtVisuel.Rows[i]["disponibility_visual"] != DBNull.Value &&
                                int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10)
                            {
                                if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                    pathWeb = string.Format("{0}/{1}/{2}/Imagette/{3}",
                                        WebCst.CreationServerPathes.IMAGES, _idMedia.ToString(),
                                        dtVisuel.Rows[i]["date_media_num"].ToString()
                                        , WebCst.CreationServerPathes.COUVERTURE);
                                else
                                    pathWeb = string.Format("{0}/{1}/{2}/Imagette/{3}"
                                        , WebCst.CreationServerPathes.IMAGES, _idMedia.ToString(),
                                        dtVisuel.Rows[i]["date_cover_num"].ToString()
                                        , WebCst.CreationServerPathes.COUVERTURE);
                            }
                            else
                            {
                                //pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";
                                pathWeb = "/Content/img/no_visu.jpg";
                            }
                            DateTime dayDT =
                                new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0, 4)),
                                             int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4, 2)),
                                             int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6, 2)));

                            if (dtVisuel.Rows[i]["disponibility_visual"] != DBNull.Value &&
                                int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10)
                            {

                                if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                                {
                                    if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                        coverLinkItemSynthesis =
                                            new CoverLinkItemSynthesis(dtVisuel.Rows[i]["media"].ToString(),
                                                                       dtVisuel.Rows[i]["number_page_media"].ToString(),
                                                                       _webSession.IdSession, _idMedia,
                                                                       dtVisuel.Rows[i]["date_media_num"].ToString(),
                                                                       dtVisuel.Rows[i]["date_media_num"].ToString());
                                    else
                                        coverLinkItemSynthesis =
                                            new CoverLinkItemSynthesis(dtVisuel.Rows[i]["media"].ToString(),
                                                                       dtVisuel.Rows[i]["number_page_media"].ToString(),
                                                                       _webSession.IdSession, _idMedia,
                                                                       dtVisuel.Rows[i]["date_media_num"].ToString(),
                                                                       dtVisuel.Rows[i]["date_cover_num"].ToString());
                                    coverItem = new CoverItem(i + 1,
                                                              GestionWeb.GetWebWord(1409, _webSession.SiteLanguage),
                                                              pathWeb, coverLinkItemSynthesis);
                                }
                                else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA)
                                {
                                    coverLinkItem = new CoverLinkItem(_webSession.IdSession, _idMedia,
                                                                      dtVisuel.Rows[i]["date_media_num"].ToString(), "");
                                    coverItem = new CoverItem(i + 1, "", pathWeb, coverLinkItem);
                                }
                            }
                            else if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                                coverItem = new CoverItem(i + 1, GestionWeb.GetWebWord(1409, _webSession.SiteLanguage),
                                                          pathWeb, null);
                            else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA)
                                coverItem = new CoverItem(i + 1, "", pathWeb, null);


                            if (htValue.Count > 0)
                            {
                                if (htValue.ContainsKey(dtVisuel.Rows[i]["date_cover_num"]))
                                {
                                    vehicleItem = new VehicleItem(dayDT,
                                                                  ((string[])
                                                                   htValue[dtVisuel.Rows[i]["date_cover_num"]])[1],
                                                                  int.Parse(
                                                                      ((string[])
                                                                       htValue[dtVisuel.Rows[i]["date_cover_num"]])[0])
                                                                     .ToString("### ### ### ###"),
                                                                  _webSession.SiteLanguage, coverItem);
                                }
                                else
                                {
                                    vehicleItem = new VehicleItem(dayDT, "0", "0", _webSession.SiteLanguage, coverItem);

                                }
                            }

                            itemsCollection.Add(vehicleItem);

                        }
                    }


                }

            }

            return itemsCollection;
        }

        #endregion

        #region Methods

        #region Dates
        /// <summary>
        /// Get begin date for the 2 module types
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// </summary>
        /// <returns>Begin date</returns>
        protected string GetDateBegin()
        {
            switch (_webSession.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodBeginningDate);
            }
            return (null);
        }

        /// <summary>
        /// Get ending date for the 2 module types
        /// </summary>
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// <returns>Ending date</returns>
        protected string GetDateEnd()
        {
            switch (_webSession.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodEndDate);
            }
            return (null);
        }
        #endregion

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        protected string GetVehicle()
        {
            string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PortofolioException("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        protected VehicleInformation GetVehicleInformation()
        {
            try
            {
                return (VehiclesInformation.Get(Int64.Parse(GetVehicle())));
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to retreive vehicle selection", err));
            }
        }
        #endregion

        #region Media Selection
        /// <summary>
        /// Get Media Id
        /// </summary>
        /// <returns>Media Id</returns>
        protected Int64 GetMediaId()
        {
            try
            {
                //OLD: A tester partout car le support es tmaintenant mis dans _webSession.PrincipalMediaUniverses.
                //if (_webSession.ReferenceUniversMedia != null && _webSession.ReferenceUniversMedia.Nodes.Count > 0)
                //    return (((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID);
                                 
                        var items = _webSession.PrincipalMediaUniverses[0].GetIncludes();
                        return items.First().Get(TNSClassificationLevels.MEDIA).First();

            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to retrieve media id", err));
            }
        }
        #endregion

        #region Insertion and Creations
        /// <summary>
        /// Determine if the result shows the insertion column
        /// </summary>
        /// <returns>True if the Insertion column is shown</returns>
        protected virtual bool ShowInsertions()
        {
            if (!_webSession.CustomerPeriodSelected.IsSliding4M || !_vehicleInformation.ShowInsertions) return (false);
            foreach (DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels)
            {
                if (item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
                    || item.Id.Equals(DetailLevelItemInformation.Levels.product))
                {
                    return (true);
                }
            }
            return (false);
        }
        /// <summary>
        /// Determine if the result shows the creation column
        /// </summary>
        /// <returns>True if the creation column is shown</returns>
        protected virtual bool ShowCreatives()
        {
            if (!_webSession.CustomerPeriodSelected.IsSliding4M ||
                !_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_SLOGAN_ACCESS_FLAG) ||
                !_vehicleInformation.ShowCreations ||
                !_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id)
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internet) return (false);

            foreach (DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels)
            {
                if (item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
                    || item.Id.Equals(DetailLevelItemInformation.Levels.product))
                {
                    return (true);
                }
            }
            return (false);
        }
        #endregion

        /// <summary>
        /// Get ventilation type list
        /// </summary>
        /// <returns>ventilation type list</returns>
        protected virtual List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> GetVentilationTypeList()
        {
            var ventilationTypeList = new List<TNS.AdExpress.Constantes.FrameWork.Results.PortofolioStructure.Ventilation>();
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.format);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.color);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.location);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert);
            return ventilationTypeList;
        }
        /// <summary>
        /// Get hour interval list
        /// </summary>
        /// <param name="hourBeginningList">hour begininng list</param>
        /// <param name="hourEndList">hour end list</param>
        protected virtual void GetHourIntevalList(Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
        {
            if (hourBeginningList == null) hourBeginningList = new Dictionary<string, double>();
            if (hourEndList == null) hourEndList = new Dictionary<string, double>();

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    hourBeginningList.Add("0507", 50000); hourEndList.Add("0507", 70000);
                    hourBeginningList.Add("0709", 70000); hourEndList.Add("0709", 90000);
                    hourBeginningList.Add("0913", 90000); hourEndList.Add("0913", 130000);
                    hourBeginningList.Add("1319", 130000); hourEndList.Add("1319", 190000);
                    hourBeginningList.Add("1924", 190000); hourEndList.Add("1924", 240000);
                    break;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    hourBeginningList.Add("0007", 0); hourEndList.Add("0007", 70000);
                    hourBeginningList.Add("0712", 70000); hourEndList.Add("0712", 120000);
                    hourBeginningList.Add("1214", 120000); hourEndList.Add("1214", 140000);
                    hourBeginningList.Add("1417", 140000); hourEndList.Add("1417", 170000);
                    hourBeginningList.Add("1719", 170000); hourEndList.Add("1719", 190000);
                    hourBeginningList.Add("1922", 190000); hourEndList.Add("1922", 220000);
                    hourBeginningList.Add("2224", 220000); hourEndList.Add("2224", 240000);
                    break;
                default:
                    throw new PortofolioException("GetHourIntevalList(): Vehicle unknown.");
            }
        }

        public GridResult GetStructureGridResult(bool excel)
        {
            Engines.StructureEngine result = null;
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList, excel);
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
                    Dictionary<string, double> hourEndList = new Dictionary<string, double>();
                    GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, excel);
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            return result.GetGridResult();
        }

        public GridResult GetDetailMediaGridResult(bool excel)
        {
            Engines.MediaDetailEngine result = null;
            StringBuilder t = new StringBuilder(5000);

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    //result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                    //TODO : G�rer le lien vars de d�tails insertion
                    //result.GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1837, _webSession.SiteLanguage));
                    //return t.ToString();
                     GridResult gridResult = new GridResult();
                    gridResult.HasData = false;
                    return gridResult;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel);
                    return result.GetGridResult();
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
        }


        private void ComputeGridData(GridResult gridResult, ResultTable _data)
        {
            _data.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            _data.CultureInfo = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            int i, j, k;
            //int creativeIndexInResultTable = -1;
            object[,] gridData = new object[_data.LinesNumber, _data.ColumnsNumber + 2]; //+2 car ID et PID en plus  -  //_data.LinesNumber
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();


            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });

            if (_data.NewHeaders != null)
            {
                for (j = 0; j < _data.NewHeaders.Root.Count; j++)
                {
                    //Key pour "Spot" = 869
                    //key pour "Plan Media du produit" = 1478

                    columns.Add(new { headerText = _data.NewHeaders.Root[j].Label, key = _data.NewHeaders.Root[j].Key, dataType = "string", width = "*" });
                    schemaFields.Add(new { name = _data.NewHeaders.Root[j].Key });
                    columnsFixed.Add(new { columnKey = _data.NewHeaders.Root[j].Key, isFixed = true, allowFixing = true });
                }
            }
            else
            {
                columns.Add(new { headerText = "", key = "Visu", dataType = "string", width = "*" });
                schemaFields.Add(new { name = "Visu" });
            }

            try
            {
                for (i = 0; i < _data.LinesNumber; i++) //_data.LinesNumber
                {
                    gridData[i, 0] = i; // Pour column ID
                    gridData[i, 1] = _data.GetSortedParentIndex(i); // Pour column PID

                    for (k = 1; k < _data.ColumnsNumber - 1; k++)
                    {
                        gridData[i, k + 1] = _data[i, k].RenderString();
                    }
                }
            }
            catch (Exception err)
            {
                throw (new Exception(err.Message));
            }

            //if (columns.Count > 10)
            //    gridResult.NeedFixedColumns = true;
            gridResult.NeedFixedColumns = false;

            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.Data = gridData;
        }


        #region GridResult
        public GridResult GetDetailMediaPopUpGridResult()
        {
            GridResult gridResult = new GridResult();
            gridResult.HasData = false;

            ResultTable _data = GetInsertionDetailResultTable(false);

            if (_data != null)
            {
                ComputeGridData(gridResult, _data);
            }
            else
            {
                gridResult.HasData = false;
                return (gridResult);
            }

            return gridResult;

        }


        public List<GridResult> GetGraphGridResult()
        {
            System.Data.DataTable dt = null;
            List<GridResult> gridResult = new List<GridResult>();
            long oldUnitId = -1;
            Dictionary<long, int> valuesListSize = new Dictionary<long, int>();
            int nbItem = 0, nbChart = 0;
            string[] xValues = null;
            int currentLine = -1;
            List<object> schemaFields = new List<object>();
            int i = -1;

            #region Set Graph values

            dt = GetStructureChartData();

            string idVehicle = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (dt == null || dt.Rows.Count == 0)
            {
                //No data to show
                GridResult gr = new GridResult();
                gridResult.Add(gr);
                gridResult[0].HasData = false;
            }
            else
            {

                string colKey = "LabelKey";
                schemaFields.Add(new { name = colKey });

                string valColKey = "ValKey";
                schemaFields.Add(new { name = valColKey });

                foreach (DataRow dr in dt.Rows)
                {
                    if (oldUnitId != long.Parse(dr["idUnit"].ToString()))
                    {
                        valuesListSize[oldUnitId] = nbItem;
                        nbItem = 0;
                        nbChart++;
                    }
                    nbItem++;
                    oldUnitId = long.Parse(dr["idUnit"].ToString());

                    if (!valuesListSize.ContainsKey(oldUnitId))
                        valuesListSize.Add(oldUnitId, nbItem);
                }
                valuesListSize[oldUnitId] = nbItem; 

                //Set chart values
                oldUnitId = -1;
                foreach (DataRow dr in dt.Rows)
                {
                    if (oldUnitId != long.Parse(dr["idUnit"].ToString()))
                    {
                        i++;
                        var gr = new GridResult();
                        gr.Title = dr["unitLabel"].ToString();
                        xValues = new string[valuesListSize[long.Parse(dr["idUnit"].ToString())]];
                        gr.Data = new object[xValues.Length , 2];
                        gridResult.Add(gr);
                        currentLine = 0;
                    }

                    gridResult[i].Data[currentLine, 0] = dr["chartDataLabel"].ToString();
                    gridResult[i].Data[currentLine, 1] = int.Parse(dr["chartDataValue"].ToString());

                    oldUnitId = long.Parse(dr["idUnit"].ToString());
                    currentLine++;
                    
                }
            }
            #endregion

            gridResult[0].HasData = true;
            gridResult[0].Schema = schemaFields;

            return gridResult;

        }

        #endregion

        #endregion


    }
}
