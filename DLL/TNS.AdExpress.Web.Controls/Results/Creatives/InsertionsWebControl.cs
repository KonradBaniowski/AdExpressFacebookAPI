#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 08/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxPro;

using WebCst = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebCtrlFct = TNS.AdExpress.Web.Controls.Functions;
using WebFct = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Common.Results.Creatives;
using FrmFct = TNS.FrameWork.WebResultUI.Functions;
using TNS.FrameWork.WebResultUI.TableControl;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using System.Collections;
using System.Collections.Specialized;
using TNS.AdExpressI.Insertions.Cells;

namespace TNS.AdExpress.Web.Controls.Results.Creatives {


	///<summary>
	/// Control to display a set of insertions, vehicle by vehicle, depending on a univers of products and media.
	/// </summary>
	/// <author>G Ragneau</author>
	/// <since>09/08/2007</since>
	/// <stereotype>container</stereotype>
    [ToolboxData("<{0}:InsertionsWebControl runat=server></{0}:InsertionsWebControl>")]
    public class InsertionsWebControl : ResultWebControl {


        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 120;
        /// <summary>
        /// Header managing result parameters
        /// </summary>
        protected CreativesHeaderWebControl _header = new CreativesHeaderWebControl();
        /// <summary>
        /// Columns Management Control
        /// </summary>
        protected GenericDetailSelectionWebControl _columns = new GenericDetailSelectionWebControl();
        /// <summary>
        /// Period beginning
        /// </summary>
        protected int _fromDate = 0;
        /// <summary>
        /// Perid End
        /// </summary>
        protected int _toDate = 0;
        /// <summary>
        /// Customer Filters values
        /// </summary>
        protected Dictionary<GenericColumnItemInformation.Columns, List<string>> _customFilterValues = new Dictionary<GenericColumnItemInformation.Columns,List<string>>();
        /// <summary>
        /// Available Filters values
        /// </summary>
        protected Dictionary<GenericColumnItemInformation.Columns, List<string>> _availableFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();
        #endregion

        #region Properties
        /// <summary>
        /// Get / Set Customer Session
        /// </summary>
        public WebSession WebSession
        {
            get
            {
                return this._customerWebSession;
            }
            set
            {
                _customerWebSession = value;
                this._header.WebSession = value;
                this._columns.CustomerWebSession = value;
                this._columns.Language = value.SiteLanguage;
            }
        }
        /// <summary>
        /// Vehicle List
        /// </summary>
        protected IList<VehicleInformation> _vehicles = new List<VehicleInformation>();
        /// <summary>
        /// Get / Set list of vehicles
        /// </summary>
        public IList<VehicleInformation> Vehicles
        {
            get
            {
                return _vehicles;
            }
            set
            {
                _vehicles = value;
                List<long> ids = new List<long>();
                foreach (VehicleInformation v in value)
                {
                    ids.Add(v.DatabaseId);
                }
                this._header.Vehicles = ids;
                if (this.IdVehicle <= 0 && this.Vehicles.Count >0)
                {
                    this.IdVehicle = this.Vehicles[0].DatabaseId;
                }

            }
        }
        /// <summary>
        /// Cuurent Vehicle Id
        /// </summary>
        protected long _idVehicle = -1;
        /// <summary>
        /// Get / Set current vehicle id
        /// </summary>
        public long IdVehicle
        {
            get
            {
                return _idVehicle;
            }
            set
            {
                _idVehicle = value;
                this._header.IdVehicle = value;
                this._columns.IdVehicleFromTab = value;
            }
        }
        /// <summary>
        /// Cuurent Page Index
        /// </summary>
        protected int _pageIndex = 1;
        /// <summary>
        /// Get / Set current vehicle id
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }
        /// <summary>
        /// Curent Univers Id
        /// </summary>
        protected int _idUnivers = -1;
        /// <summary>
        /// Get / Set current univers id
        /// </summary>
        public int IdUnivers
        {
            get
            {
                return _idUnivers;
            }
            set
            {
                _idUnivers = value;
            }
        }
        /// <summary>
        /// Zomm on this specific date
        /// </summary>
        protected string _zoom = string.Empty;
        /// <summary>
        /// Get / Set current zoomdate
        /// </summary>
        public string ZoomDate
        {
            get
            {
                return _zoom;
            }
            set
            {
                if ((value == null || value.Length <= 0) && _customerWebSession != null) {
                    if (_customerWebSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly) {
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(new DateTime(int.Parse(_customerWebSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_customerWebSession.PeriodBeginningDate.Substring(4, 2)), int.Parse(_customerWebSession.PeriodBeginningDate.Substring(6, 2))));
                        value = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                    }
                    else {
                        value = _customerWebSession.PeriodBeginningDate.Substring(0, 6);
                    }
                }
                _zoom = value;
                this._header.ZoomDate = _zoom;
                this._columns.ZoomDate = _zoom;

            }
        }
        /// <summary>
        /// Ids Filters
        /// </summary>
        protected string _idsFilter = string.Empty;
        /// <summary>
        /// Get / Set ids Filters
        /// </summary>
        public string IdsFilter
        {
            get
            {
                return _idsFilter;
            }
            set
            {
                _idsFilter = value;
                this._header.IdsFilter = _idsFilter;
            }
        }
        /// <summary>
        /// Javascript refresh
        /// </summary>
        protected string _javaScriptRefresh = string.Empty;
        /// <summary>
        /// Get / Set Javascript method to call to refresh the page
        /// </summary>
        public string JavaScriptRefresh
        {
            get
            {
                return _javaScriptRefresh;
            }
            set
            {
                _javaScriptRefresh = value;
                this._header.JavascriptRefresh = value;
            }
        }
        /// <summary>
        /// Curent Module Id
        /// </summary>
        protected Int64 _idModule = -1;
        /// <summary>
        /// Get / Set current Module id
        /// </summary>
        public Int64 IdModule
        {
            get
            {
                return _idModule;
            }
            set
            {
                _idModule = value;
            }
        }
        /// <summary>
        /// Couche d'accès aux règles métiers
        /// </summary>
        IInsertionsResult _rulesLayer = null;
        /// <summary>
        /// Get / Set Style of Information cells
        /// </summary>
        public string CssCellInfo
        {
            get
            {
                return _cssCellInfo;
            }
            set
            {
                _cssCellInfo = value;
            }
        }
        /// <summary>
        /// Style of Information cells
        /// </summary>
        protected string _cssCellInfo = string.Empty;
        /// <summary>
        /// Get / Set Creative/Insertion parameter
        /// </summary>
        public bool IsCreativeConfig
        {
            get
            {
                return _isCreativeConfig;
            }
            set
            {
                _isCreativeConfig = value;
            }
        }
        /// <summary>
        /// Creative/Insertion parameter
        /// </summary>
        protected bool _isCreativeConfig = false;
        #endregion

        #region Constructor
        public InsertionsWebControl()
        {
        }
        #endregion

        #region OnInit
        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if (this._renderType != RenderType.html)
            {
                base.OnInit(e);
                return;
            }
            if (this._javaScriptRefresh.Length <= 0)
            {
                this._javaScriptRefresh = string.Format("get_{0}", this.ID);
            }
            object[] param = new object[2];
            param[0] = _customerWebSession;
            param[1] = _idModule;
            _rulesLayer = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "TNS.AdExpressI.Insertions.Default.dll", "TNS.AdExpressI.Insertions.Default.InsertionsResult", false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);

            this._header.JavascriptRefresh = this.JavaScriptRefresh;
            this._header.ID = string.Format("{0}_header", this.ID);
            this._header.PeriodContainerName = "resultParameters.Zoom";
            this._header.VehicleContainerName = "resultParameters.IdVehicle";
            this._header.AutoInitRefresh = false;
            this.Vehicles = _rulesLayer.GetPresentVehicles(_idsFilter, this._idUnivers, this._isCreativeConfig);
            if (this.Vehicles.Count <= 0)
            {
                return;
            }
            if (!this._isCreativeConfig)
            {
                this.Controls.Add(_columns);
            }
            else
            {
                List<Int64> genericColumnList = new List<Int64>();
                List<GenericColumnItemInformation> columnItemList = WebApplicationParameters.CreativesDetail.GetDetailColumns(_idVehicle, _customerWebSession.CurrentModule);
                foreach (GenericColumnItemInformation column in columnItemList)
                {
                    genericColumnList.Add((int)column.Id);
                }
                _customerWebSession.GenericCreativesColumns = new GenericColumns(genericColumnList);
                _customerWebSession.Save();

            }
            base.OnInit(e);
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// OnLoad Evzent Handling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openDownload")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openDownload", WebFct.Script.OpenDownload());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openPressCreation", WebFct.Script.OpenPressCreation());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openGad")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openGad", WebFct.Script.OpenGad());
            base.OnLoad(e);
        }
        #endregion

        #region Filters Scripts
        protected virtual void RenderFiltersScripts(HtmlTextWriter output)
        {

            #region Find Filter
            output.WriteLine("\r\n<SCRIPT language=javascript>\r\n<!--");
            output.WriteLine("\r\n\tfunction FindValue(tab, value){");
            output.WriteLine("\r\n\t\t var i = -1;");
            output.WriteLine("\r\n\t\t for (var j=0;j<tab.length;j=j+1){");
            output.WriteLine("\r\n\t\t\t if(tab[j] == value){");
            output.WriteLine("\r\n\t\t\t\t return j;");
            output.WriteLine("\r\n\t\t\t }");
            output.WriteLine("\r\n\t\t }");
            output.WriteLine("\r\n\t\t return(i);");
            output.WriteLine("\r\n\t}");
            output.WriteLine("\r\n-->\r\n</SCRIPT>");
            #endregion

            #region Init filters
            output.WriteLine("\r\n<SCRIPT language=javascript>\r\n<!--");
            output.WriteLine("\r\n\tfunction InitFilters(){");
            output.WriteLine("\r\n\t\t resultParameters.MyFiltersId = new Array();");
            output.WriteLine("\r\n\t\t resultParameters.MyFiltersValues = new Array();");
            output.WriteLine("\r\n\t}");
            output.WriteLine("\r\n-->\r\n</SCRIPT>");
            #endregion

            #region Apply Filters
            output.WriteLine("\r\n<SCRIPT language=javascript>\r\n<!--");
            output.WriteLine("\r\n\tfunction ApplyFilters(){");
            output.WriteLine("\r\n\t\t get_{0}();", this.ID);
            output.WriteLine("\r\n\t}");
            output.WriteLine("\r\n-->\r\n</SCRIPT>");
            #endregion

            #region AddFilter
            output.WriteLine("\r\n<SCRIPT language=javascript>\r\n<!--");
            output.WriteLine("\r\n\tfunction AddFilter(filterId, filterValue){");
            output.WriteLine("\r\n\t\t var fIndex = FindValue(resultParameters.MyFiltersId, filterId);");
            output.WriteLine("\r\n\t\t if (fIndex < 0){");
            output.WriteLine("\r\n\t\t resultParameters.MyFiltersId.push(filterId); ");
            output.WriteLine("\r\n\t\t resultParameters.MyFiltersValues.push(new Array()); ");
            output.WriteLine("\r\n\t\t fIndex = resultParameters.MyFiltersValues.length-1; ");
            output.WriteLine("\r\n\t\t }");
            output.WriteLine("\r\n\t\t resultParameters.MyFiltersValues[fIndex].push(filterValue); ");
            output.WriteLine("\r\n\t}");
            output.WriteLine("\r\n-->\r\n</SCRIPT>");
            #endregion

            #region RemoveFilter
            output.WriteLine("\r\n<SCRIPT language=javascript>\r\n<!--");
            output.WriteLine("\r\n\tfunction RemoveFilter(filterId, filterValue){");
            output.WriteLine("\r\n\t\t var fIndex = FindValue(resultParameters.MyFiltersId, filterId); ");
            output.WriteLine("\r\n\t\t if (fIndex > -1){");
            output.WriteLine("\r\n\t\t\t var vIndex = FindValue(resultParameters.MyFiltersValues[fIndex], filterValue); ");
            output.WriteLine("\r\n\t\t\t if (vIndex > -1){");
            output.WriteLine("\r\n\t\t\t\t resultParameters.MyFiltersValues[fIndex].splice(vIndex,1); ");
            output.WriteLine("\r\n\t\t\t }");
            output.WriteLine("\r\n\t\t }");
            output.WriteLine("\r\n\t}");
            output.WriteLine("\r\n-->\r\n</SCRIPT>");
            #endregion

        }
        #endregion

        #region RenderContents
        /// <summary>
        /// Render COntrol
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output)
        {
            switch (_renderType)
            {
                case RenderType.html:
                    if (Vehicles.Count > 0)
                    {
                        if (_isCreativeConfig)
                        {
                            this.PageSizeCookieName = TNS.AdExpress.Constantes.Web.Cookies.CurrentPageSizeCreatives;
                        }
                        RenderFiltersScripts(output);
                        output.WriteLine(this.AjaxEventScript());

                        output.WriteLine("<table align=\"center\" class=\"whiteBackGround\" cellpadding=\"0\" cellspacing=\"2\" border=\"0\" >");
                        output.WriteLine("<tr width=\"100%\"><td width=\"100%\">");
                        _header.RenderControl(output);
                        output.WriteLine("</td></tr>");
                        if (!_isCreativeConfig)
                        {
                            output.WriteLine("<tr width=\"100%\" align=\"left\"><td width=\"100%\">");
                            _columns.RenderControl(output);
                            output.WriteLine("</td></tr>");
                        }
                        output.WriteLine("<tr width=\"100%\"><td width=\"100%\">");
                        base.Render(output);
                        output.WriteLine("</td></tr>");

                        output.WriteLine("</table>");
                    }
                    else
                    {
                        output.WriteLine("<table align=\"center\" valign=\"middle\" class=\"error\">");
                        output.WriteLine("<tr width=\"100%\" class=\"error\"><td width=\"100%\" class=\"error\">");
                        output.WriteLine(GestionWeb.GetWebWord(2106, _customerWebSession.SiteLanguage));
                        output.WriteLine("</td></tr>");
                        output.WriteLine("</table>");
                    } 
                    break;
                case RenderType.rawExcel:
                    _data = GetResultTable(_customerWebSession);
                    if (_data != null)
                    {
                        string[] filters = new string[5] { "-1", "-1", "-1", "-1", "-1" };
                        string[] tmp = _idsFilter.Split(',');
                        Array.Copy(tmp, filters, tmp.Length);

                        ListDictionary filtertList = GetFilters(filters);

                        output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
                        output.WriteLine(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(_customerWebSession, false, _fromDate.ToString(), _toDate.ToString(), filtertList, Convert.ToInt32(_idVehicle)));
                        output.WriteLine(GetRawExcel());
                        output.WriteLine(detailSelectionWebControl.GetFooter());
                    }
                    break;
                case RenderType.excel:
                    _data = GetResultTable(_customerWebSession);
                    if (_data != null)
                    {
                        string[] filters = new string[5] { "-1", "-1", "-1", "-1", "-1" };
                        string[] tmp = _idsFilter.Split(',');
                        Array.Copy(tmp, filters, tmp.Length);

                        ListDictionary filtertList = GetFilters(filters);

                        output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
                        output.WriteLine(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(_customerWebSession, false, _fromDate.ToString(), _toDate.ToString(), filtertList, Convert.ToInt32(_idVehicle)));
                        output.WriteLine(base.GetExcel());
                        output.WriteLine(detailSelectionWebControl.GetFooter());
                    }
                    break;
            }

        }
        #endregion

        #region GetResultTable
        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="session">User session</param>
        /// <returns>Data</returns>
        protected override ResultTable GetResultTable(WebSession session)
        {
            Domain.Web.Navigation.Module module = session.CustomerLogin.GetModule(this._idModule);
            VehicleInformation vehicle = VehiclesInformation.Get(_idVehicle);
            string filters = string.Empty;
            ResultTable data = null;


            string message = string.Empty;
            if (vehicle.Id == CstDBClassif.Vehicles.names.outdoor && !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))
            {
                message = GestionWeb.GetWebWord(1882, _customerWebSession.SiteLanguage);
            }
            if (!this._isCreativeConfig && vehicle.Id == CstDBClassif.Vehicles.names.internet)
            {
                message = GestionWeb.GetWebWord(2244, _customerWebSession.SiteLanguage);
            }
            if (message.Length > 0)
            {
                data = new ResultTable(1, 1);
                data.AddNewLine(LineType.total);
                CellLabel c = new CellLabel(message);
                c.CssClass = "error";
                this.CssLTotal = "error";
                data[0, 1] = c;
                return data;
            }


             //date
            WebCst.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
            string periodBegin = _customerWebSession.PeriodBeginningDate;
            string periodEnd = _customerWebSession.PeriodEndDate;

            if (ZoomDate != null && ZoomDate.Length > 0)
            {
                if (_customerWebSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
                {
                    periodType = WebCst.CustomerSessions.Period.Type.dateToDateWeek;
                }
                else
                {
                    periodType = WebCst.CustomerSessions.Period.Type.dateToDateMonth;
                }
                _fromDate = Convert.ToInt32(
                    WebFct.Dates.Max(WebFct.Dates.getZoomBeginningDate(ZoomDate, periodType),
                        WebFct.Dates.getPeriodBeginningDate(periodBegin, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
                _toDate = Convert.ToInt32(
                    WebFct.Dates.Min(WebFct.Dates.getZoomEndDate(ZoomDate, periodType),
                        WebFct.Dates.getPeriodEndDate(periodEnd, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
            }
            else
            {
                _fromDate = Convert.ToInt32(WebFct.Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                _toDate = Convert.ToInt32(WebFct.Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
            }

            object[] param = new object[2];
            param[0] = session;
            param[1] = module.Id;
            IInsertionsResult result = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "TNS.AdExpressI.Insertions.Default.dll", "TNS.AdExpressI.Insertions.Default.InsertionsResult", false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
            if (_isCreativeConfig)
            {
                List<GenericColumnItemInformation> columns = WebApplicationParameters.CreativesDetail.GetDetailColumns(vehicle.DatabaseId, module.Id);
                List<Int64> columnsId = new List<long>();
                List<GenericColumnItemInformation> columnFilters = new List<GenericColumnItemInformation>();
                Int64 idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, module.Id);
                foreach (GenericColumnItemInformation g in columns)
                {
                    if (this._renderType != RenderType.html && (g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.associatedFileMax || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.visual))
                    {
                        continue;
                    }
                    columnsId.Add(g.Id.GetHashCode());
                    if (WebApplicationParameters.GenericColumnsInformation.IsFilter(idColumnsSet, g.Id)){
                        columnFilters.Add(g);
                        _availableFilterValues.Add(g.Id, new List<string>());
                    }
                }
                _customerWebSession.GenericCreativesColumns = new GenericColumns(columnsId);
                GenericDetailLevel saveLevels = _customerWebSession.DetailLevel;
                _customerWebSession.DetailLevel = new GenericDetailLevel(new ArrayList());
                data = result.GetCreatives(vehicle, _fromDate, _toDate, _idsFilter, _idUnivers, ZoomDate);
                _customerWebSession.DetailLevel = saveLevels;
                if (data != null)
                {
                    BuildCustomFilter(ref data, columnFilters);
                }

            }
            else
            {
                if (this._renderType != RenderType.html){
                    List<GenericColumnItemInformation> columns = _customerWebSession.GenericInsertionColumns.Columns;
                    List<Int64> columnIds = new List<Int64>();
                    foreach (GenericColumnItemInformation g in columns)
                    {
                        if (g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.associatedFileMax || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.visual)
                        {
                            continue;
                        }
                        columnIds.Add(g.Id.GetHashCode());
                    }
                    _customerWebSession.GenericInsertionColumns = new GenericColumns(columnIds);
                }

                data = result.GetInsertions(vehicle, _fromDate, _toDate, _idsFilter, _idUnivers, ZoomDate);
            }

            if (_isCreativeConfig)
            {
                this._cssLHeader = string.Empty;
                this._cssL4 = _cssCellInfo;
                this._highlightBackgroundColorL4 = string.Empty;
            }
            else
            {
                if (data != null && data.NewHeaders == null && _cssCellInfo != null && _cssCellInfo.Length > 0)
                {
                    switch (vehicle.Id)
                    {
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.outdoor:
                        case CstDBClassif.Vehicles.names.directMarketing:
                            this._cssLHeader = string.Empty;
                            this._cssL4 = _cssCellInfo;
                            this._highlightBackgroundColorL4 = string.Empty;
                            break;
                        default:
                            break;
                    }
                }
            }

			if(data != null)
                data.CultureInfo = WebApplicationParameters.AllowedLanguages[session.SiteLanguage].CultureInfo;

            return data;
        }
        #endregion

        #region Build Custom Filters
        protected virtual void BuildCustomFilter(ref ResultTable data, List<GenericColumnItemInformation> columnFilters)
        {

            if (columnFilters == null || columnFilters.Count <= 0)
            {
                return;
            }

            #region Filter data and build available filters
            string value = string.Empty;
            string[] values = null;
            char[] sep = new char[1] { ',' };
            bool match = true;

            //Set available filters
            for (int i = 0; i < data.LinesNumber; i++)
            {
                foreach (GenericColumnItemInformation g in columnFilters)
                {

                    value = ((CellCreativesInformation)data[i, 1]).GetValue(g);
                    values = value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (!_availableFilterValues[g.Id].Contains(values[j]))
                        {
                            _availableFilterValues[g.Id].Add(values[j]);
                        }
                    }


                }
            }
            foreach (GenericColumnItemInformation g in columnFilters)
            {
                if (_customFilterValues.ContainsKey(g.Id) && _customFilterValues[g.Id].Count > 0)
                {
                    foreach (string s in _customFilterValues[g.Id])
                    {
                        if (!_availableFilterValues[g.Id].Contains(s))
                        {
                            _availableFilterValues[g.Id].Add(s);
                        }

                    }
                }
            }

            //Check custom filters
            int doNotMatch = 0;
            for (int i = 0; i < data.LinesNumber; i++)
            {
                match = true;
                foreach (GenericColumnItemInformation g in columnFilters)
                {

                    value = ((CellCreativesInformation)data[i, 1]).GetValue(g);
                    values = value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (_availableFilterValues[g.Id].Contains(values[j]) && _customFilterValues.ContainsKey(g.Id) && _customFilterValues[g.Id].Count > 0 && !_customFilterValues[g.Id].Contains(values[j]))
                        {
                            match = false;
                        }
                    }


                }
                if (!match)
                {
                    data.SetLineStart(new LineHide(data.GetLineStart(i).LineType), i);
                    doNotMatch++;
                }

            }
            if (doNotMatch == data.LinesNumber)
            {
                data = new ResultTable(1, 1);
                data.AddNewLine(LineType.total);
                CellLabel c = new CellLabel(GestionWeb.GetWebWord(2543, _customerWebSession.SiteLanguage));
                c.CssClass = "error";
                this.CssLTotal = "error";
                data[0, 1] = c;
            }
            #endregion

            #region Build Filters Html Code
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            StringBuilder str = new StringBuilder();
            str.AppendFormat("<tr id=\"filters_{0}\" style=\"display:none;\" ><td colSpan=\"2\"><table>", this.ID);
            bool checke = false;
            foreach (GenericColumnItemInformation c in columnFilters)
            {
                if (_availableFilterValues.ContainsKey(c.Id) && _availableFilterValues[c.Id].Count > 0)
                {
                    str.AppendFormat("<tr><th colSpan=\"4\">{0}</th></tr>", GestionWeb.GetWebWord(c.WebTextId, _customerWebSession.SiteLanguage));
                    _availableFilterValues[c.Id].Sort();                    
                    for (int i = 0; i < _availableFilterValues[c.Id].Count; i++)
                    {
                        if ((i % 4) == 0)
                        {
                            if (i > 0)
                            {
                                str.Append("</tr>");
                            }
                            str.Append("<tr>");
                        }
                        checke = checke || (_customFilterValues.ContainsKey(c.Id) && _customFilterValues[c.Id].Contains(_availableFilterValues[c.Id][i]));
                        str.AppendFormat("<td><input {4} id=\"{2}_{0}_{3}\" type=\"checkbox\" onclick=\"if(this.checked){{AddFilter({0},'{1}');}}else {{RemoveFilter({0},'{1}');}};\" ><label for=\"{2}_{0}_{3}\">{1}</label></td>"
                            , c.Id.GetHashCode(), _availableFilterValues[c.Id][i], this.ID, i
                            , (_customFilterValues.ContainsKey(c.Id)&&_customFilterValues[c.Id].Contains(_availableFilterValues[c.Id][i]))? "checked":string.Empty );

                    }
                }
            }
            str.AppendFormat("<tr><td colSpan=\"4\">&nbsp;</td></tr><tr><td colSpan=\"4\" align=\"right\"><a onclick=\"ApplyFilters();\" onmouseover=\"filterButton_{0}.src='/App_Themes/{1}/Images/Common/Button/appliquer_down.gif';\" onmouseout=\"filterButton_{0}.src = '/App_Themes/{1}/Images/Common/Button/appliquer_up.gif';\"><img src=\"/App_Themes/{1}/Images/Common/Button/appliquer_up.gif\" border=0 name=filterButton_{0}></a>", this.ID, themeName);
            str.AppendFormat("  <a onclick=\"{{InitFilters();ApplyFilters();}}\" onmouseover=\"filterInitButton_{0}.src='/App_Themes/{1}/Images/Common/Button/reinitialiser_down.gif';\" onmouseout=\"filterInitButton_{0}.src = '/App_Themes/{1}/Images/Common/Button/reinitialiser_up.gif';\"><img src=\"/App_Themes/{1}/Images/Common/Button/reinitialiser_up.gif\" border=0 name=filterInitButton_{0}></a></td></tr>", this.ID, themeName);
            str.Append("</table></td></tr></table><br>");

            str.Insert(0, string.Format("<table class=\"creativeFilterBox\" border=\"0\" cellSpacing=\"0\" cellPadding=\"0\"><tr style=\"cursor:hand;\" onclick=\"if(filters_{1}.style.display == 'none'){{filters_{1}.style.display = '';}}else {{filters_{1}.style.display = 'none';}};\"><td class=\"{2}\">{0}</td><td align=\"right\" class=\"arrowBackGround\"></td></tr>", GestionWeb.GetWebWord(518, _customerWebSession.SiteLanguage), this.ID, checke ? "pinkTextColor" : string.Empty));


            _optionHtml = str.ToString();
            #endregion
        }
        #endregion

        #region Result Parameters
        /// <summary>
        /// Set Result Parameters
        /// </summary>
        /// <returns></returns>
        protected override string SetResultParametersScript()
        {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\tfunction SetResultParameters(obj){");
            js.Append("\r\n\t obj.IdVehicle = '" + this.IdVehicle + "';");
            js.Append("\r\n\t obj.IdPreviousVehicle = '" + this.IdVehicle + "';");
            js.Append("\r\n\t obj.PageIndexToto = '" + _pageIndex + "';");
            js.Append("\r\n\t obj.IdUnivers = '" + this._idUnivers + "';");
            js.Append("\r\n\t obj.Zoom = '" + this._zoom + "';");
            js.Append("\r\n\t obj.Filters = '" + this._idsFilter + "';");
            js.Append("\r\n\t obj.IdModule = '" + this._idModule + "';");
            js.Append("\r\n\t obj.CssInfo = '" + this._cssCellInfo + "';");
            js.Append("\r\n\t obj.CreativeConfig = '" + this._isCreativeConfig + "';");

            js.Append("\r\n\t obj.MyFiltersId = new Array();");
            js.Append("\r\n\t obj.MyFiltersValues = new Array();");

            js.Append("\r\n\t}");
            return (js.ToString());
        }
        /// <summary>
        /// Load specific result parameters
        /// </summary>
        /// <param name="o"></param>
        protected override void LoadResultParameters(JavaScriptObject o)
        {
            Int64 previousVehicle = -1;
            if (o != null)
            {
                if (o.Contains("IdPreviousVehicle"))
                {
                    previousVehicle = Convert.ToInt64(o["IdPreviousVehicle"].Value.Replace("\"", ""));
                }
                if (o.Contains("IdVehicle"))
                {
                    this._idVehicle = Convert.ToInt64(o["IdVehicle"].Value.Replace("\"", ""));
                }
                if (o.Contains("PageIndexToto"))
                {
                    this._pageIndex = Convert.ToInt32(o["PageIndexToto"].Value.Replace("\"", ""));
                }
                if (o.Contains("IdUnivers"))
                {
                    this._idUnivers = Convert.ToInt32(o["IdUnivers"].Value.Replace("\"", ""));
                }
                if (o.Contains("Zoom"))
                {
                    this._zoom = o["Zoom"].Value.Replace("\"", "");
                }
                if (o.Contains("Filters"))
                {
                    this._idsFilter = o["Filters"].Value.Replace("\"", "");
                }
                if (o.Contains("CssInfo"))
                {
                    this._cssCellInfo = o["CssInfo"].Value.Replace("\"", "");
                }
                if (o.Contains("IdModule"))
                {
                    this._idModule = Convert.ToInt64(o["IdModule"].Value.Replace("\"", ""));
                }
                if (o.Contains("CreativeConfig"))
                {
                    this._isCreativeConfig = Convert.ToBoolean(o["CreativeConfig"].Value.Replace("\"", ""));
                }
                
            }
            if (previousVehicle != _idVehicle)
            {
                _availableFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();
                _customFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();
            }
            else
            {
                Int32[] o1 = (Int32[])AjaxPro.JavaScriptDeserializer.Deserialize(o["MyFiltersId"], typeof(Int32[]));
                AjaxPro.JavaScriptArray o2 = (AjaxPro.JavaScriptArray)AjaxPro.JavaScriptDeserializer.Deserialize(o["MyFiltersValues"], typeof(AjaxPro.JavaScriptArray));
                _customFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();
                List<string> l = null;
                string[] t = null;
                for (int i = 0; i < o1.Length; i++)
                {
                    l = new List<string>();
                    _customFilterValues.Add((GenericColumnItemInformation.Columns)o1[i], l);
                    t = (string[])AjaxPro.JavaScriptDeserializer.Deserialize(o2[i], typeof(string[]));
                    for (int j = 0; j < t.Length; j++)
                    {
                        l.Add(t[j]);
                    }
                }
            }
        }
        #endregion

        #region GetRawExcel
        /// <summary>
        /// Génère le code html destinée à un fichier excel brut
        /// </summary>
        /// <returns>Code html</returns>
        public new string GetRawExcel()
        {

            #region Tri des données
            if (this._data != null)
            {
                int iCol = (int)this._data.GetHeadersIndexInResultTable(this._sSortKey);
                if (iCol >= 0 && !this._sortOrder.Equals(ResultTable.SortOrder.NONE))
                {
                    this._data.Sort(this._sortOrder, iCol);
                }
            }
            #endregion

            StringBuilder output = new StringBuilder(10000);
            int i = 0, j = 0;
            InitCss();

            #region Process html code

            output.Append("<table border=1>");

            #region Headers
            output.Append("<tr>");
            bool b = false;

            foreach (DetailLevelItemInformation d in _customerWebSession.DetailLevel.Levels)
            {
                Header tmp = new Header(GestionWeb.GetWebWord(d.WebTextId, _customerWebSession.SiteLanguage), d.Id.GetHashCode(), _cssLHeader);
                tmp.RenderExcel(output, ref b, null, 1, null);
            }

            string s = string.Empty;
            if (_data.NewHeaders != null)
            {
                s = _data.NewHeaders.RenderExcel(_cssLHeader);
                s = s.Replace("<tr>", string.Empty);
            }
            output.Append(s);
            #endregion

            #region table body
            try
            {
                //Get lower level
                LineType dataLineType = LineType.level1;
                foreach (LineType l in _data.LinesStart.Keys)
                {
                    if (l.GetHashCode() <= dataLineType.GetHashCode() && l != LineType.header && l != LineType.nbParution && l != LineType.total)
                    {
                        dataLineType = l;
                    }
                }
                //Render lower levels
                LineType cType = LineType.header;
                List<ICell> parents = new List<ICell>();
                List<LineType> parentType = new List<LineType>();
                LineStart lStart;
                for (i = 0; i < _data.LinesNumber; i++)
                {
                    lStart = _data.GetLineStart(i);
                    cType = lStart.LineType;
                    if (cType != dataLineType)
                    {
                        if (!parentType.Contains(cType))
                        {
                            parentType.Add(cType);
                            parents.Add(null);
                        }                        
                        parents[parentType.IndexOf(cType)] = _data[i, 1];
                        continue;
                    }
                    output.Append(lStart.RenderExcel(string.Empty));
                    //parents
                    foreach (ICell c in parents)
                    {
                        output.Append(c.RenderExcel(lStart.CssClass));
                    }
                    //data line
                    for (j = 1; j < _data.ColumnsNumber ; j++)
                    {
                        output.Append(_data[i, j].RenderExcel(lStart.CssClass));
                    }


                }

            }
            catch (System.Exception err)
            {
                throw (new System.Exception(err.Message + " Impossible de rendre la cellule [" + i + "," + j + "]"));
            }
            #endregion

            output.Append("</table>");

            #endregion

            return (output.ToString());

        }
        #endregion

        #region GetFilters
        protected ListDictionary GetFilters(string[] filters)
        {
            Domain.Web.Navigation.Module module = _customerWebSession.CustomerLogin.GetModule(this._idModule);
            ListDictionary filtersList = new ListDictionary();
            if (filters != null && filters.Length > 0)
            {
                GenericDetailLevel detailLevels = null;
                switch (module.Id)
                {
                    case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
                    case WebCst.Module.Name.NEW_CREATIVES:
                    case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                        detailLevels = _customerWebSession.GenericProductDetailLevel;
                        break;
                    case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
                    case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case WebCst.Module.Name.ANALYSE_DES_PROGRAMMES:
                        detailLevels = _customerWebSession.GenericMediaDetailLevel;
                        break;
                }

                for (int i = 0; i < detailLevels.GetNbLevels; i++)
                {
                    DetailLevelItemInformation cLevel = (DetailLevelItemInformation)detailLevels.Levels[i];
                    if (filters[i] != "-1" || (_idVehicle == VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.adnettrack) && cLevel.Id == DetailLevelItemInformation.Levels.slogan && filters[i] != "-1"))
                    {
                        filtersList.Add(cLevel.DataBaseIdField, filters[i]);
                    }
                }
            }
            return filtersList;
        }
        #endregion

    }
}
