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

namespace TNS.AdExpress.Web.Controls.Results.Creatives {


	///<summary>
	/// Control to display a set of creations, vehicle by vehicle, depending on a univers of products and media.
	/// </summary>
	/// <author>G Ragneau</author>
	/// <since>09/08/2007</since>
	/// <stereotype>container</stereotype>
    [ToolboxData("<{0}:CreativesWebControl runat=server></{0}:CreativesWebControl>")]
    public class CreativesWebControl : TNS.FrameWork.WebResultUI.TableControl.PaginedTableWebControl {


        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 120;
        /// <summary>
        /// Header managing result parameters
        /// </summary>
        protected CreativesHeaderWebControl _header = new CreativesHeaderWebControl();
        #endregion

        #region Properties
        /// <summary>
        /// Customer WebSession
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Get / Set Customer Session
        /// </summary>
        public WebSession WebSession {
            get {
                return this._webSession;
            }
            set {
                _webSession = value;
                this._header.WebSession = value;
            }
        }
        /// <summary>
        /// Vehicle List
        /// </summary>
        protected IList<int> _vehicles = new List<int>();
        /// <summary>
        /// Get / Set list of vehicles
        /// </summary>
        public IList<int> Vehicles {
            get {
                return _vehicles;
            }
            set {
                _vehicles = value;
                this._header.Vehicles = value;
            }
        }
        /// <summary>
        /// Cuurent Vehicle Id
        /// </summary>
        protected int _idVehicle = -1;
        /// <summary>
        /// Get / Set current vehicle id
        /// </summary>
        public int IdVehicle {
            get {
                return _idVehicle;
            }
            set {
                _idVehicle = value;
                this._header.IdVehicle = value;
            }
        }
        /// <summary>
        /// Cuurent Page Index
        /// </summary>
        protected int _pageIndex = 1;
        /// <summary>
        /// Get / Set current vehicle id
        /// </summary>
        public int PageIndex {
            get {
                return _pageIndex;
            }
            set {
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
        public int IdUnivers {
            get {
                return _idUnivers;
            }
            set {
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
                if ((value == null || value.Length <= 0) && _webSession != null)
                {
                    if (_webSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
                    {
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(new DateTime(int.Parse(_webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.PeriodBeginningDate.Substring(4, 2)), int.Parse(_webSession.PeriodBeginningDate.Substring(6, 2))));
                        value = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                    }
                    else
                    {
                        value = _webSession.PeriodBeginningDate.Substring(0, 6);
                    }
                }
                _zoom = value;
                this._header.ZoomDate = _zoom;
            }
        }
        /// <summary>
        /// Ids Filters
        /// </summary>
        protected string _idsFilter = string.Empty;
        /// <summary>
        /// Get / Set ids Filters
        /// </summary>
        public string IdsFilter {
            get {
                return _idsFilter;
            }
            set {
                _idsFilter = value;
                this._header.IdsFilter = _idsFilter;
            }
        }
        /// <summary>
        /// Curent Module Id
        /// </summary>
        protected Int64 _idModule = -1;
        /// <summary>
        /// Get / Set current Module id
        /// </summary>
        public Int64 IdModule {
            get {
                return _idModule;
            }
            set {
                _idModule = value;
            }
        }
        #endregion

        #region OnInit
        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            this._header.ID = string.Format("{0}_header", this.ID);
            this._header.JavascriptRefresh = string.Format("get_{0}", this.ID);
            this._header.PeriodContainerName = string.Format("o_{0}.Zoom", this.ID);
            this._header.VehicleContainerName = string.Format("o_{0}.IdVehicle", this.ID);
            this.Vehicles = CreativeRules.GetVehicles(_webSession, _idModule, _idsFilter, this._idUnivers);
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// OnLoad Evzent Handling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
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
        protected override void RenderContents(HtmlTextWriter output) {


            if (Vehicles.Count > 0) {
                AjaxScripts(output);
            }

            output.WriteLine("<table align=\"center\" class=\"whiteBackGround\" cellpadding=\"0\" cellspacing=\"2\" border=\"0\" >");

            //header
            if (Vehicles.Count > 0) {
                output.WriteLine("<tr width=\"100%\"><td width=\"100%\">");
                _header.RenderControl(output);
                output.WriteLine("</td></tr>");
            }
            //result
            output.WriteLine("<tr width=\"100%\"><td class=\"datacreativetable\" width=\"100%\" id=\"div_{0}\"></td></tr>", this.ID);
            output.WriteLine("</table>");
            output.WriteLine("<script language=javascript>");
            if (Vehicles.Count > 0) {
                //Refresh data
                output.WriteLine("\t get_{0}()", this.ID);
            }
            else {
                //no data
                output.WriteLine("\t output_{0}=document.getElementById('div_{0}');", this.ID);
                output.WriteLine("\t output_{0}.innerHTML = '<br/><br/>{1}';", this.ID, GetUIEmpty(_webSession.SiteLanguage, 2106));
            }
            output.WriteLine("</script>");

            
        }
        #endregion

        #region Ajax Scripts
        /// <summary>
        /// Generate Javascript required in the page
        /// </summary>
        /// <param name="output"></param>
        protected void AjaxScripts(HtmlTextWriter output) {

            StringBuilder js = new StringBuilder();

            js.Append("\r\n<script language=javascript>\r\n");

            js.AppendLine(FrmFct.Scripts.AjaxProTimeOutScript(this._ajaxProTimeOut));

            #region Cookies Management
            js.AppendLine(FrmFct.Scripts.GetCookie());
            js.AppendLine(FrmFct.Scripts.SetCookie());
            js.AppendLine(FrmFct.Scripts.DeleteCookie());
            #endregion

            #region Stringbuilder
            js.AppendLine(FrmFct.Scripts.StringBuilder());
            #endregion

            #region data
            js.AppendFormat("\r\n var data_{0};", this.ID);
            js.AppendFormat("\r\n var sortIds_{0};", this.ID);
            js.AppendFormat("\r\n var sortLabels_{0};", this.ID);
            js.AppendFormat("\r\n var output_{0};", this.ID);
            //js.AppendFormat("\r\naddEvent(window, \"load\", get_{0});", this.ID);
            #endregion

            #region GetLoading
            js.AppendFormat("\r\n function loading_{0}()", this.ID);
            js.Append("\r\n {");
            js.AppendFormat("\r\n\t return '<div align=\"center\"><img src=\"{0}\"></div>';", this._LoadingImage);
            js.Append("\r\n }");
            #endregion

            #region parameters
            Dictionary<string, object> parameters = new Dictionary<string,object>();
            parameters.Add("IdVehicle", Vehicles[0]);
            parameters.Add("Filters", _idsFilter);
            parameters.Add("Zoom", _zoom);
            parameters.Add("IdUnivers", _idUnivers);
            parameters.Add("IdModule", _idModule);
            parameters.Add("PageIndex", _pageIndex);
            parameters.Add("ID", this.ID);
            parameters.Add("Sort", -1);
            js.AppendLine(FrmFct.Scripts.GetAjaxParametersScripts(this.ID, string.Format("o_{0}", this.ID), parameters));
            #endregion

            #region Pagination
            js.AppendFormat("\r\n var currentPageIndex_{0} = {1};", this.ID, this.PageIndex);
            js.AppendFormat("\r\n var pageCount_{0}  = 0;", this.ID);
            js.AppendFormat("\r\n var pageSize_{0} = 0;", this.ID);
            js.AppendFormat("\r\n var minPageSize_{0} = 0;", this.ID);
            #endregion

            #region Get_{0}()
            js.AppendFormat("\r\n function get_{0}()", this.ID);
            js.Append("\r\n {");
            js.AppendFormat("\r\n\t output_{0}=document.getElementById('div_{0}');", this.ID);
            js.AppendFormat("\r\n\t if (output_{0} != null)", this.ID);
            js.Append("\r\n\t {");
            js.AppendFormat("\r\n\tvar titi = loading_{0}();", this.ID);
            js.AppendFormat("\r\n\t\t output_{0}.innerHTML=titi;", this.ID);

            js.AppendFormat("\r\n\t\t {0}.{1}.GetData('{2}',{3},{4});",
                this.GetType().Namespace,
                this.GetType().Name,
                _webSession.IdSession,
                string.Format("o_{0}", this.ID),
                string.Format("get_{0}_callback", this.ID));
            js.Append("\r\n\t }");
            js.Append("\r\n }");
            #endregion

            #region get_{0}_callback
            js.AppendFormat("\r\n function get_{0}_callback(res)", this.ID);//res.error
            js.Append("\r\n {");
                js.Append("\r\n\t if(res.error != null){ ");
                    js.AppendFormat("\r\n\t\t output_{0}.innerHTML = res.error.Message;", this.ID);
                js.Append("\r\n\t }\r\n");
                js.Append("\r\n\t else if(res.value != null){ ");
                    js.AppendFormat("\r\n\t\t currentPageIndex_{0} = o_{0}.PageIndex;", this.ID);
                    js.AppendFormat("\r\n\t\t o_{0}.PageIndex = 1;", this.ID);
                    js.AppendFormat("\r\n\t\t leftPageIndex_{0}  = 0;", this.ID);
                    js.AppendFormat("\r\n\t\t rightPageIndex_{0}  = 0;", this.ID);
                    js.AppendFormat("\r\n\t\t pageCount_{0}  = 0;", this.ID);

                    //Sauvergarde Nombre de lignes dans cookie
                    js.Append("\r\n\t\t var cook = GetCookie(\"" + TNS.AdExpress.Constantes.Web.Cookies.CreativesPageSize + "\"); ");
                    js.Append("\r\n\t\t if(cook != null){");
                        js.AppendFormat("\r\n\t\t\t pageSize_{0} = cook;", this.ID);
                    js.Append("\r\n\t\t }");
                    js.Append("\r\n\t\t else {");
                        js.AppendFormat("\r\n\t\t\t if( pageSize_{0} <= 0 ) pageSize_{0} = {1};", this.ID, this.DefaultPageSize);
                    js.Append("\r\n\t\t }");
                    js.AppendFormat("\r\n\t\t pageSizeOptionsList_{0} ='{1}';", this.ID, this.PageSizeOptionString);

                    js.AppendFormat("\r\n\t\t data_{0} = res.value[0];", this.ID);
                    js.AppendFormat("\r\n\t\t sortIds_{0} = res.value[1];", this.ID);
                    js.AppendFormat("\r\n\t\t sortLabels_{0} = res.value[2];", this.ID);
                    js.AppendFormat("\r\n\t\t if(data_{0}!=null && data_{0}.length > 0)", this.ID);//Total pages
                    js.Append("\r\n\t\t{");
                        js.AppendFormat("\r\n\t\t\t pageCount_{0} = Math.ceil((data_{0}.length - 1)/pageSize_{0});", this.ID);
                    js.Append("\r\n\t\t }");
                    js.AppendFormat("\r\n\t\t\t if ( currentPageIndex_{0} > pageCount_{0}) currentPageIndex_{0} = 1;", this.ID);

                    //page de résultat
                    js.AppendFormat("\r\n\t\t GetResultPage_{0}();", this.ID);

                    js.Append("\r\n\t\t res = null;");
                js.Append("\r\n\t }");
                js.Append("\r\n\t else{");
                    js.AppendFormat("\r\n\t\t output_{0}.innerHTML='<div align=\"center\" class=\"txtViolet11Bold\">{1}</div>';",
                        this.ID,
                        GestionWeb.GetWebWord(177, _webSession.SiteLanguage));
                js.Append("\r\n\t }\r\n");
            js.Append("\r\n }\r\n");
            #endregion

            #region GetResultTable_{0}
            js.AppendFormat("\r\n function GetResultPage_{0}()", this.ID);
            js.Append("\r\n {");
            js.Append("\r\n\t var barUp=''; ");
            js.Append("\r\n\t var barDown=''; ");
            js.Append("\r\n\t var sb = new StringBuilder();");
            js.Append("\r\n\t var i;");

            js.AppendFormat("\r\n\t barUp=GetBar('up_'); ", this.ID); 
            js.AppendFormat("\r\n\t barDown=GetBar('down_'); ", this.ID);

            js.AppendFormat("\r\n\t var output_header=document.getElementById('{0}_header');", this.ID);

            js.Append("\r\n\t sb.append('<table width=\"'+output_header.scrollWidth+'\"><tr width=\"100%\"><td class=\"nav navBarTop\">');");
            js.Append("\r\n\t sb.append(barUp);");
            js.Append("\r\n\t sb.append('</td></tr>');");

            js.Append("\r\n\t sb.append('<tr width=\"100%\"><td>');");
            js.Append("\r\n\t sb.append('<table class=\"creativesList\" width=\"100%\">');");
            js.AppendFormat("\r\n\t i = currentPageIndex_{0}*pageSize_{0}- pageSize_{0} ; ", this.ID);

            js.AppendFormat("\r\n\t for( i ; i < (currentPageIndex_{0}*pageSize_{0}) && i < data_{0}.length ; i++) ", this.ID);
            js.Append("\r\n\t{");
            js.AppendFormat("\r\n\t\t sb.append(data_{0}[i]);", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t sb.append('</table></td></tr>');");

            js.Append("\r\n\t sb.append('<tr width=\"100%\"><td class=\"nav navBarBottom\">');");
            js.Append("\r\n\t sb.append(barDown);");
            js.Append("\r\n\t sb.append('</td></tr></table>');"); 
            js.Append("\r\nvar toto = sb.toString();");
            js.AppendFormat("\r\n\t output_{0}.innerHTML = toto;", this.ID);
            js.Append("\r\n\t sb=null;");

            js.Append("\r\n }");
            #endregion

            #region Paginate
			js.AppendFormat("\r\n function Paginate_{0}(pageIndex)", this.ID);
            js.Append("\r\n {");
            js.AppendFormat("\r\n\t currentPageIndex_{0} = pageIndex;", this.ID);

			//page de résultat
            js.AppendFormat("\r\n\t output_{0}.innerHTML=loading_{0}();", this.ID);
            js.AppendFormat("\r\n\t setTimeout(\"GetResultPage_{0}(output_{0})\",0);", this.ID);
						
			js.Append("\r\n }");
            #endregion

            #region ChangePageSize
            js.AppendFormat("\r\n\n function ChangePageSize_{0}(pagesizeIndex)", this.ID);
            js.Append("\r\n{");
            js.AppendFormat("\r\n\t pageSize_{0} = pagesizeIndex;", this.ID);

            js.AppendFormat("\r\n\t setCookie(\"{0}\",pagesizeIndex,365); ", TNS.AdExpress.Constantes.Web.Cookies.CreativesPageSize);//

            js.AppendFormat("\r\n\t currentPageIndex_{0}  = 1;", this.ID);
            js.AppendFormat("\r\n\t leftPageIndex_{0} = 0;", this.ID);
            js.AppendFormat("\r\n\t rightPageIndex_{0} = 0;", this.ID);
            js.AppendFormat("\r\n\t if (data_{0} != null && data_{0}.length > 0)", this.ID);//Total pages
            js.Append("\r\n\t {");
            js.AppendFormat("\r\n\t\t pageCount_{0} = Math.ceil((data_{0}.length - 1)/pageSize_{0});", this.ID);
            js.Append("\r\n\t }");

            //page de résultat	
            js.AppendFormat("\r\n\t output_{0}.innerHTML=loading_{0}();", this.ID);
            js.AppendFormat("\r\n\t setTimeout(\"GetResultPage_{0}(output_{0})\",0);", this.ID);
            js.Append("\r\n }");
            #endregion

            #region Images
            js.AppendFormat("\r\n {0}_img_last_out = new Image(); {0}_img_last_out.src ='{1}';\r\n{0}_img_last_in = new Image(); {0}_img_last_in.src ='{2}';\r\n{0}_img_first_out = new Image(); {0}_img_first_out.src ='{3}';\r\n{0}_img_first_in = new Image(); {0}_img_first_in.src ='{4}';\r\n"
                , this.ID
                , this.ButtonLastUp
                , this.ButtonLastDown
                , this.ButtonFirstUp
                , this.ButtonFirstDown
            );
            js.AppendFormat("\r\n {0}_img_next_out = new Image(); {0}_img_next_out.src ='{1}';\r\n{0}_img_next_in = new Image(); {0}_img_next_in.src ='{2}';\r\n{0}_img_previous_out = new Image(); {0}_img_previous_out.src ='{3}';\r\n{0}_img_previous_in = new Image(); {0}_img_previous_in.src ='{4}';\r\n"
                , this.ID
                , this.ButtonNextUp
                , this.ButtonNextDown
                , this.ButtonPreviousUp
                , this.ButtonPreviousDown
            );

            #endregion

            #region Navigation
            js.Append("\r\n\n function GetBar(name)\r\n {");
            js.Append("\r\n\t var bar = new StringBuilder(); ");

            js.AppendFormat("\r\n\t if( pageCount_{0} > 0 ) ", this.ID);
            js.Append("\r\n\t{");

            //Sélection options taille page
            //js.Append("\r\n\t\t htmlNavigationBar=PageSizeOptions(pageSizeOptionsList,htmlNavigationBar,up); }");
            js.AppendFormat("\r\n\t\t var liste = pageSizeOptionsList_{0}.split(\",\");", this.ID);
            js.AppendFormat("\r\n\t\t minPageSize_{0} = 0;", this.ID);
            js.AppendFormat("\r\n\t\t if (liste.length > 0) minPageSize_{0} = liste[0];", this.ID);
            js.AppendFormat("\r\n\t\t if (data_{0} != null && minPageSize_{0} <= data_{0}.length)", this.ID);
            js.Append("\r\n\t\t {");
            js.AppendFormat("\r\n\t\t\t bar.append('<span> {1} <select name=\"pageSizeOptions_{0}\" id=\"' + name + 'pageSizeOptions_{0}\" onChange=\"ChangePageSize_{0}(this.value)\">');", this.ID, GestionWeb.GetWebWord(2045, _webSession.SiteLanguage));
            js.Append("\r\n\t\t\t var n;");
            js.Append("\r\n\t\t\t for( n = 0 ; n < liste.length ; n++){ ");
            js.AppendFormat("\r\n\t\t\t\t if(pageSize_{0}==liste[n]) ", this.ID);
            js.Append("\r\n\t\t\t\t {");
            js.Append("\r\n\t\t\t\t\t bar.append('<option value=\"' + liste[n] + '\" selected>' + liste[n] + '</option>');");
            js.Append("\r\n\t\t\t\t }else{");
            js.Append("\r\n\t\t\t\t\t bar.append('<option value=\"' + liste[n] + '\">' + liste[n] + '</option>');");
            js.Append("\r\n\t\t\t\t }");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t\t bar.append('</select></span>&nbsp;&nbsp;'); ");
            js.Append("\r\n\t\t }");

            //Première Page 
            js.AppendFormat("\r\n\t\t if(pageCount_{0}> 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.AppendFormat("\r\n\t\t if(currentPageIndex_{0} > 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.Append("\r\n\t\t\t bar.append('<a');");
            js.Append("\r\n\t\t\t bar.append(' onmouseover=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_first_img.src={0}_img_first_in.src;\"');", this.ID);
            js.Append("\r\n\t\t\t bar.append(' onmouseout=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_first_img.src={0}_img_first_out.src;\"');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append(' href=\"javascript:Paginate_{0}(1);\" >');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_first_img\" src=\"{0}\"></a>');", this.ButtonFirstUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else {");
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_first_img\" src=\"{0}\">');", this.ButtonFirstUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t bar.append('&nbsp;&nbsp;');");
            js.Append("\r\n\t\t }");

            //Page précédente
            js.AppendFormat("\r\n\t\t if(pageCount_{0}> 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.AppendFormat("\r\n\t\t var prevIndex = currentPageIndex_{0} - 1;", this.ID);
            js.AppendFormat("\r\n\t\t if(currentPageIndex_{0} > 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.Append("\r\n\t\t  bar.append('<a');");
            js.Append("\r\n\t\t  bar.append(' onmouseover=\"');");
            js.Append("\r\n\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t bar.append('_previous_img.src={0}_img_previous_in.src;\"');", this.ID);
            js.Append("\r\n\t\t\t  bar.append(' onmouseout=\"');");
            js.Append("\r\n\t\t\t  bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_previous_img.src={0}_img_previous_out.src;\"');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append(' href=\"javascript:Paginate_{0}('+prevIndex+');\" >');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_previous_img\" src=\"{0}\"></a>');", this.ButtonPreviousUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else {");
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_previous_img\" src=\"{0}\">');", this.ButtonPreviousUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t bar.append('&nbsp;&nbsp;');");
            js.Append("\r\n\t\t }");

            #region Pages Indexes (Old)
            //Pages Indexes
            //js.AppendFormat("\r\n\t\t if( pageCount_{0}> 1 ) ", this.ID);
            //js.Append("\r\n\t\t {");
            //js.Append("\r\n\t\t\t var i = 1;");
            //js.AppendFormat("\r\n\t\t\t for( i ; i < currentPageIndex_{0}; i++)", this.ID);
            //js.Append("\r\n\t\t\t {");
            //js.AppendFormat("\r\n\t\t\t\t bar.append(' <a class=\"navlink\" href=\"javascript:Paginate_{0}(' + i + ');\">' + i + '</a> ');", this.ID);
            //js.Append("\r\n\t\t\t}");
            //js.AppendFormat("\r\n\t\t\t bar.append(' <font color=\"#FF0099\">'+currentPageIndex_{0}+'</font> ');", this.ID);
            //js.AppendFormat("\r\n\t\t\t i = currentPageIndex_{0} + 1;", this.ID);
            //js.AppendFormat("\r\n\t\t\t for( i ; i <= pageCount_{0}; i++)", this.ID);
            //js.Append("\r\n\t\t\t {");
            //js.AppendFormat("\r\n\t\t\t\t bar.append(' <a class=\"navlink\" href=\"javascript:Paginate_{0}(' + i + ');\">' + i + '</a> ');", this.ID);
            //js.Append("\r\n\t\t\t }");
            //js.Append("\r\n\t\t\t bar.append('&nbsp;&nbsp;');");
            //js.Append("\r\n\t\t }");
            #endregion

            #region Pages Indexes
            js.Append("\r\n\t\t if( pageCount_" + this.ID + "> 1 ) ");
            js.Append("\r\n\t\t {");
            js.Append("\r\n var leftPageIndex  = 0, rightPageIndex  = 0;");
            js.Append("\r\n\t var nbIndexPage = 4; ");
            js.Append("\r\n\t var htmlNavigationBar = ''; ");

                js.Append("\r\n\t if( pageCount_" + this.ID + " <= nbIndexPage )  ");
                js.Append("\r\n\t  nbIndexPage = pageCount_" + this.ID + " - 1; ");

                js.Append("\r\n\t\t htmlNavigationBar += ' <font class=\"pinkTextColor\">'+currentPageIndex_" + this.ID + "+'</font> ';");
                js.Append("\r\n\t\t leftPageIndex = rightPageIndex = currentPageIndex_" + this.ID + ";");
                js.Append("\r\n\t\t while(nbIndexPage>0 && pageCount_" + this.ID + ">1){ ");
                    js.Append("\r\n\t\t\t if( currentPageIndex_" + this.ID + "!=1 && leftPageIndex>1) {");
                        js.Append("\r\n\t\t\t\t leftPageIndex--; ");
                        js.Append("\r\n\t\t\t\t nbIndexPage--; ");
                        js.Append("\r\n\t\t\t\t\t htmlNavigationBar = ' <a class=\"navlink\" href=\"javascript:Paginate_" + this.ID + "('+leftPageIndex+');\">'+leftPageIndex+'</a> '+ htmlNavigationBar ;");
                    js.Append("\r\n\t\t\t }");
                    js.Append("\r\n\t\t\t if( currentPageIndex_" + this.ID + " < pageCount_" + this.ID + " && rightPageIndex < pageCount_" + this.ID + ") {");
                        js.Append("\r\n\t\t\t\t rightPageIndex++; ");
                        js.Append("\r\n\t\t\t\t nbIndexPage--; ");
                        js.Append("\r\n\t\t\t\t\t htmlNavigationBar = htmlNavigationBar +' <a class=\"navlink\" href=\"javascript:Paginate_" + this.ID + "('+rightPageIndex+');\">'+rightPageIndex+'</a> ' ;");
                    js.Append("\r\n\t\t }");
                js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t\t bar.append(htmlNavigationBar);");
            js.Append("\r\n\t\t\t bar.append('&nbsp;&nbsp;');");
            js.Append("\r\n\t\t }");
            #endregion


            //Page suivante
            js.AppendFormat("\r\n\t\t if(pageCount_{0}> 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.AppendFormat("\r\n\t\t var nextIndex = currentPageIndex_{0} + 1;", this.ID);
            js.AppendFormat("\r\n\t\t if(currentPageIndex_{0} < pageCount_{0}) ", this.ID);
            js.Append("\r\n\t\t{");
            js.Append("\r\n\t\t\t bar.append('<a');");
            js.Append("\r\n\t\t\t bar.append(' onmouseover=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_next_img.src={0}_img_next_in.src;\"');", this.ID);
            js.Append("\r\n\t\t\t bar.append(' onmouseout=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_next_img.src={0}_img_next_out.src;\"');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append(' href=\"javascript:Paginate_{0}('+nextIndex+');\" >');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_next_img\" src=\"{0}\"></a>');", this.ButtonNextUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else {");
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_next_img\" src=\"{0}\">');", this.ButtonNextUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t bar.append('&nbsp;&nbsp;');");
            js.Append("\r\n\t\t }");

            //Dernière Page 			
            js.AppendFormat("\r\n\t\t if(pageCount_{0}> 1) ", this.ID);
            js.Append("\r\n\t\t {");
            js.AppendFormat("\r\n\t\t if(currentPageIndex_{0} < pageCount_{0}) ", this.ID);
            js.Append("\r\n\t\t {");
            js.Append("\r\n\t\t\t bar.append('<a');");
            js.Append("\r\n\t\t\t bar.append(' onmouseover=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_last_img.src={0}_img_last_in.src;\"');", this.ID);
            js.Append("\r\n\t\t\t bar.append(' onmouseout=\"');");
            js.Append("\r\n\t\t\t bar.append(name);");
            js.AppendFormat("\r\n\t\t\t bar.append('_last_img.src={0}_img_last_out.src;\"');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append(' href=\"javascript:Paginate_{0}('+pageCount_{0}+');\" >');", this.ID);
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_last_img\" src=\"{0}\"></a>');", this.ButtonLastUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else {");
            js.AppendFormat("\r\n\t\t\t bar.append('<IMG border=0 alt=\"\" name=\"'+name+'_last_img\" src=\"{0}\">');", this.ButtonLastUp);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t }");

            js.Append("\r\n\t }");
            js.Append("\r\n\t return(bar.toString()); ");
            js.Append("\r\n }");
            #endregion

            js.Append("\r\n</script>\r\n");

            output.Write(js.ToString());
        }
        #endregion

        #region Chargement des paramètres AjaxPro.JavaScriptObject
        /// <summary>
        /// Charge les paramètres navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadParams(AjaxPro.JavaScriptObject o) {

            if (o != null) {

                if (o.Contains("IdVehicle")) {
                    _idVehicle = Convert.ToInt32(o["IdVehicle"].Value.Replace("\"", ""));
                }

                if (o.Contains("IdUnivers")) {
                    _idUnivers = Convert.ToInt32(o["IdUnivers"].Value.Replace("\"", ""));
                }

                if (o.Contains("Filters")) {
                    _idsFilter = o["Filters"].Value.Replace("\"", "");
                }

                if (o.Contains("Zoom")) {
                    _zoom = o["Zoom"].Value.Replace("\"", "");
                }

                if (o.Contains("IdModule")) {
                    _idModule = Convert.ToInt64(o["IdModule"].Value.Replace("\"", ""));
                }

                if (o.Contains("PageIndex")) {
                    _pageIndex = Convert.ToInt32(o["PageIndex"].Value.Replace("\"", ""));
                }

                if (o.Contains("ID")) {
                    this.ID = o["ID"].Value.Replace("\"", "");
                }
            }
        }
        #endregion

        #region  [AjaxPro.AjaxMethod] GetData
        /// <summary>
        /// Obtention des tableaux à transmettre côté client
        /// </summary>
        /// <param name="idSession">Identifiant de session utilisateur</param>
        /// <param name="o">Tableaux de paramètres</param>
        /// <returns>Tableau d'objet contenant ls différentes lignes (html) du tableau de résultat</returns>
        [AjaxPro.AjaxMethod]
        public object[] GetData(string idSession, AjaxPro.JavaScriptObject o) {

            object[] result = new object[3];

            try {

                _webSession = (WebSession)WebSession.Load(idSession);
                this.LoadParams(o);

                //date
                int dateBegin = 0;
                int dateEnd = 0;
                WebCst.CustomerSessions.Period.Type periodType = _webSession.PeriodType;
                string periodBegin = _webSession.PeriodBeginningDate;
                string periodEnd = _webSession.PeriodEndDate;

                if (ZoomDate != null && ZoomDate.Length > 0) {
                    if (_webSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly) {
                        periodType = WebCst.CustomerSessions.Period.Type.dateToDateWeek;
                    }
                    else {
                        periodType = WebCst.CustomerSessions.Period.Type.dateToDateMonth;
                    }
                    dateBegin = Convert.ToInt32(
                        WebFct.Dates.Max(WebFct.Dates.getZoomBeginningDate(ZoomDate, periodType),
                            WebFct.Dates.getPeriodBeginningDate(periodBegin, _webSession.PeriodType)).ToString("yyyyMMdd")
                        );
                    dateEnd = Convert.ToInt32(
                        WebFct.Dates.Min(WebFct.Dates.getZoomEndDate(ZoomDate, periodType),
                            WebFct.Dates.getPeriodEndDate(periodEnd, _webSession.PeriodType)).ToString("yyyyMMdd")
                        );
                }
                else {
                    dateBegin = Convert.ToInt32(WebFct.Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                    dateEnd = Convert.ToInt32(WebFct.Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
                }
                //data
                try{
                    string parameters = string.Format("'page=' + currentPageIndex_{0}", this.ID);
                    this._elements = CreativeRules.GetCreatives(_webSession, _idVehicle, _idsFilter, dateBegin, dateEnd, _idUnivers, _zoom,_idModule, parameters);
                }
                catch(UnauthorizedAccessException e){
                    result[0] = new object[1]{GetUIEmpty(_webSession.SiteLanguage, 2254)};
                    return result;
                }


                if (this._elements != null && this._elements.Count > 0) {

                    IList<ITableComparer> sorts = this._elements[0].GetComparers();
                    if (sorts.Count > 0) {
                        object[] sortIds = new object[sorts.Count];
                        object[] sortLabels = new object[sorts.Count];
                        for (int i = 0; i < sorts.Count; i++) {
                            sortIds[i] = sorts[i].GetId();
                            sortLabels[i] = GestionWeb.GetWebWord(sorts[i].GetId(), _webSession.SiteLanguage);
                        }
                        result[1] = sortIds;
                        result[2] = sortLabels;

                    }
                    result[0] = this.GetHtmlLines();
                }
                else {
                    result[0] = new object[1]{GetUIEmpty(_webSession.SiteLanguage, 2106)};
                    return result;
                }

            }
            catch (System.Exception err) {
                string clientErrorMessage = WebCtrlFct.Errors.OnAjaxMethodError(err, this._webSession);
                throw new Exception(clientErrorMessage);
            }
            return (result);
        }
        #endregion

        #region GetUIEmpty
        /// <summary>
        /// Génère le code html précisant qu'il n 'y a pas de données à afficher
        /// </summary>
        /// <param name="language">Langue du site</param>
        /// <param name="code">code traduction</param>
        /// <returns>Code html Généré</returns>
        public static string GetUIEmpty(int language, int code) {
            StringBuilder HtmlTxt = new StringBuilder(10000);

            HtmlTxt.Append("<TR vAlign=\"top\" >");
            HtmlTxt.Append("<TD vAlign=\"top\">");
            HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
            HtmlTxt.Append("<tr><td></td></tr>");
            HtmlTxt.Append("<TR vAlign=\"top\">");
            HtmlTxt.Append("<TD vAlign=\"top\" align=\"center\" height=\"50\" width=\"300\" class=\"txtViolet11Bold\">");
            HtmlTxt.Append(GestionWeb.GetWebWord(code, language));
            HtmlTxt.Append("</TD>");
            HtmlTxt.Append("</TR>");
            HtmlTxt.Append("</TABLE>");
            HtmlTxt.Append("</TD>");
            HtmlTxt.Append("</TR>");

            return HtmlTxt.ToString();
        }
        #endregion
    }
}
