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
        protected int _fromDate = 0;
        protected int _toDate = 0;

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
                //if ((value == null || value.Length <= 0) && _customerWebSession != null)
                //{
                //    if (_customerWebSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
                //    {
                //        AtomicPeriodWeek tmp = new AtomicPeriodWeek(new DateTime(int.Parse(_customerWebSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_customerWebSession.PeriodBeginningDate.Substring(4, 2)), int.Parse(_customerWebSession.PeriodBeginningDate.Substring(6, 2))));
                //        value = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                //    }
                //    else
                //    {
                //        value = _customerWebSession.PeriodBeginningDate.Substring(0, 6);
                //    }
                //}
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
                _customerWebSession.DetailLevel = new GenericDetailLevel(new ArrayList());
                List<Int64> genericColumnList = new List<Int64>();
                List<GenericColumnItemInformation> columnItemList = WebApplicationParameters.CreativesDetail.GetDetailColumns(_idVehicle, _customerWebSession.CurrentModule);
                foreach (GenericColumnItemInformation column in columnItemList)
                {
                    genericColumnList.Add((int)column.Id);
                }
                _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);
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

                        ListDictionary mediaImpactedList = WebFct.MediaDetailLevel.GetImpactedMedia(_customerWebSession, long.Parse(filters[0]), long.Parse(filters[1]), long.Parse(filters[2]), long.Parse(filters[3]));	

                        output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
                        output.WriteLine(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(_customerWebSession, false, _fromDate.ToString(), _toDate.ToString(), mediaImpactedList, Convert.ToInt32(_idVehicle)));
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

                        ListDictionary mediaImpactedList = WebFct.MediaDetailLevel.GetImpactedMedia(_customerWebSession, long.Parse(filters[0]), long.Parse(filters[1]), long.Parse(filters[2]), long.Parse(filters[3]));	

                        output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
                        output.WriteLine(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(_customerWebSession, false, _fromDate.ToString(), _toDate.ToString(), mediaImpactedList, Convert.ToInt32(_idVehicle)));
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
                foreach (GenericColumnItemInformation g in columns)
                {
                    if (this._renderType != RenderType.html && (g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.associatedFileMax || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.visual))
                    {
                        continue;
                    }
                    columnsId.Add(g.Id.GetHashCode());
                }
                _customerWebSession.GenericInsertionColumns = new GenericColumns(columnsId);
                data = result.GetCreatives(vehicle, _fromDate, _toDate, _idsFilter, _idUnivers, ZoomDate);
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
            js.Append("\r\n\t obj.PageIndexToto = '" + _pageIndex + "';");
            js.Append("\r\n\t obj.IdUnivers = '" + this._idUnivers + "';");
            js.Append("\r\n\t obj.Zoom = '" + this._zoom + "';");
            js.Append("\r\n\t obj.Filters = '" + this._idsFilter + "';");
            js.Append("\r\n\t obj.IdModule = '" + this._idModule + "';");
            js.Append("\r\n\t obj.CssInfo = '" + this._cssCellInfo + "';");
            js.Append("\r\n\t obj.CreativeConfig = '" + this._isCreativeConfig + "';");
            js.Append("\r\n\t}");
            return (js.ToString());
        }
        /// <summary>
        /// Load specific result parameters
        /// </summary>
        /// <param name="o"></param>
        protected override void LoadResultParameters(JavaScriptObject o)
        {
            if (o != null)
            {
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
    }
}
