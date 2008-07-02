using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Functions;

using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Headers
{
    /// <summary>
    /// Control to display subperiods in the selected period 
    /// </summary>
    [ToolboxData("<{0}:SubPeriodSelectionWebControl runat=server></{0}:SubPeriodSelectionWebControl>")]
    public class SubPeriodSelectionWebControl : WebControl
    {

        #region Constantes
        /// <summary>
        /// Pattern to determine the path to the selected subperiod visual
        /// </summary>
        private const string IMAGE_PATTERN_SELECTED = "/App_Themes/{0}/Images/Culture/GlobalCalendar/{1}s.gif";
        /// <summary>
        /// Pattern to determine the path to the subperiod visual
        /// </summary>
        private const string IMAGE_PATTERN = "/App_Themes/{0}/Images/Culture/GlobalCalendar/{1}.gif";
        /// <summary>
        /// Pattern to determine the path to the subperiod visual when mouse is over
        /// </summary>
        private const string IMAGE_PATTERN_OVER = "/App_Themes/{0}/Images/Culture/GlobalCalendar/{1}r.gif";
        /// <summary>
        /// Number of period visible
        /// </summary>
        private const int NUMBER_PERIOD_VISIBLE = 12;
        private const string SCROLL_CONTENT_CONTAINER_CSS = "scroll_content_container_css";
        private const string SCROLL_CONTENT_CSS = "scroll_content_css";
        private const string SCROLL_BAR_CSS = "scroll_bar_css";
        private const string SCROLL_TRACK_CSS = "scroll_track_css";
        private const string FONT_TITLE = "txtViolet11Bold";
        private const string FONT_PERIOD = "txtViolet11";
        private const int COMPONENT_WIDTH = 250;
        private const string COMPONENT_BACK_GROUND = "violetBackGroundV2";
        private const int TRACK_WIDTH = 50;
        private const int TRACK_HEIGHT = 5;
        private const string WHITE_BACK_GROUND = "whiteBackGround";
        #endregion

        #region Properties
        /// <summary>
        /// Customer WebSession
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Get / Set Customer Session
        /// </summary>
        public WebSession WebSession
        {
            get
            {
                return this._webSession;
            }
            set
            {
                _webSession = value;
            }
        }
        /// <summary>
        /// Specify if zoom navigation is enabled
        /// </summary>
        protected bool _isZoomEnabled = true;
        /// <summary>
        /// Get / Set zoom activation
        /// </summary>
        public bool IsZoomEnabled
        {
            get
            {
                return _isZoomEnabled;
            }
            set
            {
                _isZoomEnabled = value;
            }
        }
        /// <summary>
        /// Specify if "All periods" optrion is allowed
        /// </summary>
        protected bool _isAllPeriodAllowed = true;
        /// <summary>
        /// Get / Set "All periods" optrion is allowed
        /// </summary>
        public bool AllPeriodAllowed
        {
            get
            {
                return _isAllPeriodAllowed;
            }
            set
            {
                _isAllPeriodAllowed = value;
            }
        }        /// <summary>
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
                _zoom = value;
            }
        }
        /// <summary>
        /// Name of the javascript variable containing the current period
        /// </summary>
        protected string _periodContainerName = string.Empty;
        /// <summary>
        /// Get / set the Name of the javascript variable containing the current period
        /// </summary>
        public string PeriodContainerName
        {
            get
            {
                return _periodContainerName;
            }
            set
            {
                _periodContainerName = value;
            }
        }
        /// <summary>
        /// Name of the javascript function to call to refresh data
        /// </summary>
        protected string _javascriptRefresh = string.Empty;
        /// <summary>
        /// Get / set the javascript function to call to refresh data
        /// </summary>
        public string JavascriptRefresh
        {
            get
            {
                return _javascriptRefresh;
            }
            set
            {
                _javascriptRefresh = value;
            }
        }
        #endregion

        #region RenderContents
        /// <summary>
        /// Render Control Contents
        /// </summary>
        /// <param name="output">Html Output</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.WriteLine(RenderContents());
        }
        #endregion

        #region string RenderContents()
        /// <summary>
        /// Generate html code required by the control
        /// </summary>
        /// <returns></returns>
        public string RenderContents()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder tmpSb = new StringBuilder();
            int periodIndex = 0;
            int i = -1;
            string labBegin = _webSession.PeriodBeginningDate;
            string labEnd = _webSession.PeriodEndDate;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

            WebCst.CustomerSessions.Period.Type periodType = _webSession.PeriodType;
            WebCst.CustomerSessions.Period.DisplayLevel periodDisplay = _webSession.DetailPeriod;
            string periodBegin = _webSession.PeriodBeginningDate;
            string periodEnd = _webSession.PeriodEndDate;
            string realPeriodBegin = _webSession.PeriodBeginningDate;
            string realPeriodEnd = _webSession.PeriodEndDate;

            DateTime begin = Dates.getPeriodBeginningDate(realPeriodBegin, _webSession.PeriodType);
            DateTime today = DateTime.Now.Date;
            if (begin < today.AddDays(1 - today.Day).AddMonths(-3))
            {
                _isAllPeriodAllowed = false;
            }

            realPeriodBegin = begin.ToString("yyyyMMdd");
            realPeriodEnd = Dates.getPeriodEndDate(realPeriodEnd, _webSession.PeriodType).ToString("yyyyMMdd");

            if (periodDisplay == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
            {
                periodType = WebCst.CustomerSessions.Period.Type.dateToDateWeek;
                AtomicPeriodWeek tmp = new AtomicPeriodWeek(new DateTime(int.Parse(realPeriodBegin.Substring(0, 4)), int.Parse(realPeriodBegin.Substring(4, 2)), int.Parse(realPeriodBegin.Substring(6, 2))));
                periodBegin = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                tmp = new AtomicPeriodWeek(new DateTime(int.Parse(realPeriodEnd.Substring(0, 4)), int.Parse(realPeriodEnd.Substring(4, 2)), int.Parse(realPeriodEnd.Substring(6, 2))));
                periodEnd = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
            }
            else
            {
                periodType = WebCst.CustomerSessions.Period.Type.dateToDateMonth;
                periodBegin = realPeriodBegin.Substring(0, 6);
                periodEnd = realPeriodEnd.Substring(0, 6);
            }
            //_zoom = (_zoom.Length > 0) ? _zoom : periodBegin;

            //if ((_zoom != periodBegin || _zoom != periodEnd) && _isZoomEnabled)
            #region Period
            if ((_zoom.Length > 0 || _isAllPeriodAllowed) && _isZoomEnabled && (_zoom != periodBegin || _zoom != periodEnd))
            {

                #region Data
                string currentPeriod = periodBegin;
                periodIndex = -1;
                i = -1;
                sb.Append("<script language=javascript>");
                sb.AppendFormat("\r\nvar tab_zooms_{0} = new Array();", this.ID);
                sb.AppendFormat("\r\nvar tab_periodLabel_{0} = new Array();", this.ID);
                sb.AppendFormat("\r\nvar tab_periodImage_{0} = new Array();", this.ID);
                sb.AppendFormat("\r\nvar tab_periodImage_selected_{0} = new Array();", this.ID);
                sb.AppendFormat("\r\nvar tab_periodImage_over_{0} = new Array();", this.ID);

                do
                {

                    i++;

                    AppendPeriod(periodType, currentPeriod, i, sb, tmpSb, ref periodIndex, realPeriodBegin, realPeriodEnd, ref labBegin, ref labEnd);

                    if (periodType != WebCst.CustomerSessions.Period.Type.dateToDateWeek)
                    {
                        currentPeriod = (new DateTime(int.Parse(currentPeriod.Substring(0, 4)), int.Parse(currentPeriod.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM");
                    }
                    else
                    {
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(currentPeriod.Substring(0, 4)), int.Parse(currentPeriod.Substring(4, 2)));
                        tmp.Increment();
                        currentPeriod = string.Format("{0}{1}", tmp.Year, tmp.Week.ToString("0#"));
                    }
                } while (periodEnd != currentPeriod);

                //dernière période
                i++;
                AppendPeriod(periodType, currentPeriod, i, sb, tmpSb, ref periodIndex, realPeriodBegin, realPeriodEnd, ref labBegin, ref labEnd);
                //fin dernière période

                if (_isAllPeriodAllowed)
                {
                    //All periods
                    sb.AppendFormat("\r\ntab_zooms_{0}[{1}] = '';", this.ID, i + 1);
                    sb.AppendFormat("\r\ntab_periodLabel_{0}[{1}] = '{2} {3} {4} {5}';"
                        , this.ID
                        , i + 1
                        , GestionWeb.GetWebWord(896, _webSession.SiteLanguage)
                        , Dates.dateToString(Dates.getPeriodBeginningDate(realPeriodBegin, _webSession.PeriodType), _webSession.SiteLanguage)
                        , GestionWeb.GetWebWord(897, _webSession.SiteLanguage)
                        , Dates.dateToString(Dates.getPeriodEndDate(realPeriodEnd, _webSession.PeriodType), _webSession.SiteLanguage)
                    );
                    sb.AppendFormat("\r\ntab_periodImage_{0}[{1}] = '/App_Themes/"+themeName+"/Images/Common/button/bt_calendar_up.gif';", this.ID, i + 1);
                    sb.AppendFormat("\r\ntab_periodImage_selected_{0}[{1}] = '/App_Themes/"+themeName+"/Images/Common/button/bt_calendar_up.gif';", this.ID, i + 1);
                    sb.AppendFormat("\r\ntab_periodImage_over_{0}[{1}] = '/App_Themes/"+themeName+"/Images/Common/button/bt_calendar_down.gif';", this.ID, i + 1);
                }
                if (periodIndex < 0)
                {
                    if (_isAllPeriodAllowed)
                    {
                        periodIndex = i + 1;
                        labBegin = Dates.dateToString(Dates.getPeriodBeginningDate(realPeriodBegin, _webSession.PeriodType), _webSession.SiteLanguage);
                        labEnd = Dates.dateToString(Dates.getPeriodEndDate(realPeriodEnd, _webSession.PeriodType), _webSession.SiteLanguage);

                    }
                    else
                    {
                        periodIndex = 0;
                        labBegin = Dates.dateToString(Dates.Max(Dates.getZoomBeginningDate(periodBegin, periodType), Dates.getPeriodBeginningDate(realPeriodBegin, WebCst.CustomerSessions.Period.Type.dateToDate)), _webSession.SiteLanguage);
                        labEnd = Dates.dateToString(Dates.Max(Dates.getZoomEndDate(periodBegin, periodType), Dates.getPeriodEndDate(realPeriodEnd, WebCst.CustomerSessions.Period.Type.dateToDate)), _webSession.SiteLanguage);
                    }
                }
                //End all periods

                //periode courante
                sb.AppendFormat("\r\nvar current_period_{0} = {1};", this.ID, periodIndex);
                sb.AppendFormat("\r\n {0} = tab_zooms_{1}[current_period_{1}];", this._periodContainerName, this.ID);
                sb.Append("</script>");
                #endregion

                #region Period management scripts
                sb.Append("<script language=javascript>");

                //function PeriodMouseOver
                sb.AppendFormat("\r\nfunction PeriodMouseOver_{0}(index)", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\tif (current_period_{0} != index) ", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+index).src = tab_periodImage_over_{0}[index];"
                    , this.ID
                    , string.Format(IMAGE_PATTERN_OVER, themeName, "index"));
                sb.AppendFormat("\r\n\t\tdocument.getElementById('periodLabel_{0}').innerHTML = tab_periodLabel_{0}[index];", this.ID);
                sb.Append("\r\n\t}");
                sb.Append("\r\n}");


                //function PeriodMouseOut
                sb.AppendFormat("\r\nfunction PeriodMouseOut_{0}(index)", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\tif (current_period_{0} != index) ", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+index).src = tab_periodImage_{0}[index];"
                    , this.ID);
                sb.Append("\r\n\t}");
                sb.Append("\r\n\telse{");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+index).src = tab_periodImage_selected_{0}[index];"
                    , this.ID);
                sb.Append("\r\n\t}");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('periodLabel_{0}').innerHTML = tab_periodLabel_{0}[current_period_{0}];", this.ID);
                sb.Append("\r\n}");

                //function PeriodSelect
                sb.AppendFormat("\r\nfunction PeriodSelect_{0}(index)", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\tif (current_period_{0} != index) ", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+current_period_{0}).src = tab_periodImage_{0}[current_period_{0}];"
                    , this.ID);
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+index).src = tab_periodImage_selected_{0}[index];"
                    , this.ID);
                sb.AppendFormat("\r\n\t\tcurrent_period_{0} = index; ", this.ID);
                sb.AppendFormat("\r\n\t\t{0} = tab_zooms_{1}[index];", this._periodContainerName, this.ID);
                sb.AppendFormat("\r\n\t\t{0}();", this._javascriptRefresh);
                sb.Append("\r\n\t}");
                sb.Append("\r\n}");

                sb.AppendFormat("\r\nfunction PeriodSelectAll_{0}()", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\tif (current_period_{0} >= 0) ", this.ID);
                sb.Append("{");
                sb.AppendFormat("\r\n\t\tdocument.getElementById('img_{0}_'+current_period_{0}).src = tab_periodImage_{0}[current_period_{0}];"
                    , this.ID);
                sb.AppendFormat("\r\n\t\tcurrent_period_{0} = {1}; ", this.ID, i + 1);
                sb.AppendFormat("\r\n\t\t{0} = tab_zooms_{1}[{2}];", this._periodContainerName, this.ID, i + 1);
                sb.AppendFormat("\r\n\t\t{0}();", this._javascriptRefresh);
                sb.Append("\r\n\t}");
                sb.Append("\r\n}");

                sb.Append("\r\n</script>");
                #endregion

                #region Design
                sb.AppendFormat("<table border=0 cellspacing=0 cellpadding=0 class=\"{0}\" width=\"100%\">", COMPONENT_BACK_GROUND);
                sb.AppendFormat("<tr><td height=\"5\" colspan=\"4\"></td></tr>");

                if (tmpSb.Length > 0)
                {
                    //tmpSb.Append("<img height=\"0\" width=\"0\">");
                    //Titre du composant
                    if (periodDisplay != WebCst.CustomerSessions.Period.DisplayLevel.weekly)
                    {
                        sb.AppendFormat("\r\n<tr height=\"15\"  valign=\"top\"><td nowrap class=\"{1}\" valign=\"top\">&nbsp;{0}&nbsp;</td>", GestionWeb.GetWebWord(2276, _webSession.SiteLanguage), FONT_TITLE);
                    }
                    else
                    {
                        sb.AppendFormat("\r\n<tr height=\"15\" valign=\"top\"><td nowrap class=\"{1}\" valign=\"top\">&nbsp;{0}&nbsp;</td>", GestionWeb.GetWebWord(2277, _webSession.SiteLanguage), FONT_TITLE);
                    }

                    //Current period label
                    if (labBegin != labEnd)
                    {
                        sb.AppendFormat("\r\n<td width=\"150\" nowrap class=\"{5}\" id=\"periodLabel_{4}\" valign=\"top\">{0} {1} {2} {3}</td>",
                            GestionWeb.GetWebWord(896, _webSession.SiteLanguage),
                            labBegin,
                            GestionWeb.GetWebWord(897, _webSession.SiteLanguage),
                            labEnd,
                            this.ID,
                            FONT_PERIOD);
                    }
                    else
                    {
                        sb.AppendFormat("\r\n<td width=\"150\" nowrap class=\"{1}\" id=\"periodLabel_{2}\" valign=\"top\">{0}</td>"
                            , labBegin
                            , FONT_PERIOD
                            , this.ID);
                    }

                    //Period list design
                    if (_isAllPeriodAllowed)
                    {
                        sb.Append("\r\n<td valign=\"top\">&nbsp;&nbsp;&nbsp;</td>");
                        sb.AppendFormat("<td valign=\"top\"><img id=\"img_{0}_{1}\" src=\"/App_Themes/"+themeName+"/Images/Common/button/bt_calendar_up.gif\" onMouseOver=\"javascript:PeriodMouseOver_{0}({1});\" onMouseOut=\"javascript:PeriodMouseOut_{0}({1});\"  onclick=\"javascript:PeriodSelectAll_{0}();\"/></td>"
                            , this.ID
                            , i + 1
                            );
                        //onMouseOver=\"this.src='/Images/Common/button/bt_calendar_down.gif';\" onMouseOut=\"this.src='/Images/Common/button/bt_calendar_up.gif';\" onclick=\"javascript:PeriodSelectAll_{0}();\"/>");
                    }
                    sb.AppendFormat("<td valign=\"top\">&nbsp;&nbsp;&nbsp;</td></td><td width=\"100%\" valign=\"top\"><div width=\"{4}\" id=\"scrollcontentContainer_{2}\" class=\"{1}\"><div id=\"scrollcontent_{2}\" class=\"{3}\">{0}</div></div>",
                        tmpSb.ToString(),
                        SCROLL_CONTENT_CONTAINER_CSS,
                        this.ID,
                        SCROLL_CONTENT_CSS,
                        COMPONENT_WIDTH);

                    sb.AppendFormat("<div id=\"scrollContainer_{1}\" width=\"{3}\" class=\"{0}\"><div width=\"{4}\" id=\"scrollTrack_{1}\" class=\"{2}\" style=\"cursor:pointer;left:0px; top:0px\"></div></div></td></tr>",
                        SCROLL_BAR_CSS, this.ID, SCROLL_TRACK_CSS, COMPONENT_WIDTH, TRACK_WIDTH);

                    sb.AppendFormat("<tr id=\"{0}_bottomLine\" height=\"2\"><td colspan=\"4\"></td></tr>", this.ID);

                    #region Scrolling scripts
                    sb.Append("\r\n<script language=\"javascript\">");
                    sb.AppendFormat("\r\n\tvar theScrollTrack_{0} = document.getElementById(\"scrollTrack_{0}\");", this.ID);
                    sb.AppendFormat("\r\n\tvar theScrollContainer_{0} = document.getElementById(\"scrollContainer_{0}\");", this.ID);
                    sb.AppendFormat("\r\n\tvar theScrollContent_{0} = document.getElementById(\"scrollcontent_{0}\");", this.ID);
                    sb.AppendFormat("\r\n\tvar theScrollContentContainer_{0} = document.getElementById(\"scrollcontentContainer_{0}\");", this.ID);
                    sb.AppendFormat("\r\n\tvar theBottomLine_{0} = document.getElementById(\"{0}_bottomLine\");", this.ID);
                    sb.AppendFormat("\r\n\tvar GECKO = (navigator.product == (\"Gecko\"));");
                    sb.Append("\r\n\tif (GECKO == true)");
                    sb.Append("\r\n\t{");
                    //Firefox
                    sb.AppendFormat("\r\n\tDrag.init(theScrollTrack_{0}, null, 0, theScrollContentContainer_{0}.offsetWidth-theScrollTrack_{0}.offsetWidth, 0, 0);", this.ID);
                    sb.AppendFormat("\r\n\ttheScrollTrack_{0}.onDrag = function(x, y)", this.ID);
                    sb.Append("\r\n\t{");
                    sb.Append("\r\n\tif ( x != 0)");
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = Math.min(0,(x*((parseInt(theScrollContent_{0}.width)-theScrollContentContainer_{0}.offsetWidth)/(theScrollContentContainer_{0}.offsetWidth-theScrollTrack_{0}.offsetWidth))) * (-1)) +'px';", this.ID);
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tif ( parseInt(theScrollTrack_{0}.style.left) == 0)", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = 0 +'px';", this.ID);
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tif ( parseInt(theScrollTrack_{0}.style.left) == (theScrollContentContainer_{0}.offsetWidth-theScrollTrack_{0}.offsetWidth))", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = (theScrollContentContainer_{0}.offsetWidth-parseInt(theScrollContent_{0}.width)) +'px';", this.ID);
                    sb.Append("\r\n\t}");
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tfunction InitScrollBar_{0}()", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\tvar theImg_{0} = document.getElementById(\"img_{0}_{1}\");", this.ID, i);
                    sb.AppendFormat("\r\n\t\ttheScrollContent_{0}.width = ({1}*theImg_{0}.offsetWidth) +'px'", this.ID, i);
                    sb.AppendFormat("\r\n\t\tif ( parseInt(theScrollContent_{0}.width) <= theScrollContentContainer_{0}.offsetWidth)", this.ID);
                    sb.Append("\r\n\t\t{");
                    sb.AppendFormat("\r\n\t\t\ttheScrollContainer_{0}.style.visibility = 'hidden';", this.ID);
                    sb.AppendFormat("\r\n\t\t\ttheBottomLine_{0}.style.height = '0px';", this.ID);
                    sb.Append("\r\n\t\t}else{");
                    sb.AppendFormat("\r\n\t\t\ttheScrollContainer_{0}.style.visibility = 'visible';", this.ID);
                    sb.AppendFormat("\r\n\t\t\ttheBottomLine_{0}.style.height = '2px';", this.ID);
                    sb.AppendFormat("\r\n\t\t\tvar posInit_{0} = Math.max(0,{1} * (theScrollContentContainer_{0}.offsetWidth-theScrollTrack_{0}.offsetWidth) / {2});", this.ID, periodIndex, i);
                    sb.AppendFormat("\r\n\t\t\ttheScrollTrack_{0}.style.left = parseInt(posInit_{0})+'px';", this.ID);
                    sb.AppendFormat("\r\n\t\t\ttheScrollTrack_{0}.onDrag(posInit_{0},0);", this.ID);
                    sb.Append("\r\n\t\t}");
                    sb.Append("\r\n\t}"); 
                    sb.Append("\r\n\t}");
                    sb.Append("\r\n\telse");
                    sb.Append("\r\n\t{");
                    //IE
                    //toto
                    sb.AppendFormat("\r\n\ttheScrollTrack_{0}.onDragEnd = function(x, y)", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\tif ( theScrollTrack_{0}.style.left == \"0px\")", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = 0 +\"px\";", this.ID);
                    sb.Append("\r\n\t}");
                    sb.Append("\r\n\t}");
                    //toto
                    sb.AppendFormat("\r\n\tDrag.init(theScrollTrack_{0}, null, 0, theScrollContentContainer_{0}.width-theScrollTrack_{0}.width, 0, 0);", this.ID);
                    sb.AppendFormat("\r\n\ttheScrollTrack_{0}.onDrag = function(x, y)", this.ID);
                    sb.Append("\r\n\t{");
                    sb.Append("\r\n\tif ( x != 0)");
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = Math.min(0,(x*((theScrollContent_{0}.offsetWidth-theScrollContentContainer_{0}.width)/(theScrollContentContainer_{0}.width-theScrollTrack_{0}.width))) * (-1)) +\"px\";", this.ID);
                    //(-7 * x / Math.abs(x))+
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tif ( theScrollTrack_{0}.style.left == \"0px\")", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = 0 +\"px\";", this.ID);

                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tif ( theScrollTrack_{0}.left == (theScrollContentContainer_{0}.width-theScrollTrack_{0}.width))", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\ttheScrollContent_{0}.style.left = (theScrollContentContainer_{0}.width-theScrollContent_{0}.width) +\"px\";", this.ID);

                    sb.Append("\r\n\t}");
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tfunction InitScrollBar_{0}()", this.ID);
                    sb.Append("\r\n\t{");
                    sb.AppendFormat("\r\n\t\tif ( theScrollContent_{0}.offsetWidth <= theScrollContentContainer_{0}.width)", this.ID);
                    sb.Append("\r\n\t\t{");
                    sb.AppendFormat("\r\n\t\t\ttheScrollContainer_{0}.style.display = 'none';", this.ID);
                    sb.AppendFormat("\r\n\t\t\ttheBottomLine_{0}.style.height = '0px';", this.ID);
                    sb.Append("\r\n\t\t}else{");
                    sb.AppendFormat("\r\n\t\t\ttheScrollContainer_{0}.style.display = '';", this.ID);
                    sb.AppendFormat("\r\n\t\t\ttheBottomLine_{0}.style.height = '2px';", this.ID);
                    sb.AppendFormat("\r\n\t\t\tvar posInit_{0} = Math.max(0,{1} * (theScrollContentContainer_{0}.width-theScrollTrack_{0}.width) / {2});", this.ID, periodIndex, i);
                    //sb.AppendFormat("\r\n\t\t\talert('posInit:' + posInit_{0} + ' - theScrollContentContainer_{0}.width: ' + theScrollContentContainer_{0}.width + ' - theScrollTrack_{0}.width:' + theScrollTrack_{0}.width + ' - periodIndex:' + {1} + ' - total:' + {2});", this.ID, periodIndex, i);
                    //sb.AppendFormat("\r\n\t\t\talert('image courante:' + imgCurrent_{0}.offsetLeft);", this.ID, periodIndex, i);
                    sb.AppendFormat("\r\n\t\t\ttheScrollTrack_{0}.style.left = posInit_{0}+\"px\";", this.ID);

                    sb.AppendFormat("\r\n\t\t\ttheScrollTrack_{0}.onDrag(posInit_{0},0);", this.ID);
                    //sb.AppendFormat("\r\n\t\t\talert('image courante:' + imgCurrent_{0}.offsetLeft);", this.ID, periodIndex, i);
                    sb.Append("\r\n\t\t}");
                    sb.Append("\r\n\t}");
                    sb.Append("\r\n\t}");



                    sb.AppendFormat("\r\n\tfunction Init_{0}()", this.ID);
                    sb.Append("\r\n\t{");
                    sb.Append("\r\n\t\tif (window.addEventListener){");
                    sb.AppendFormat("\r\n\t\t\twindow.addEventListener(\"load\", InitScrollBar_{0}, false);", this.ID);
                    sb.Append("\r\n\t\t}else if (window.attachEvent){");
                    sb.AppendFormat("\r\n\t\t\twindow.attachEvent(\"onload\", InitScrollBar_{0});", this.ID);
                    sb.Append("\r\n\t\t}");
                    sb.Append("\r\n\t}");
                    sb.AppendFormat("\r\n\tInit_{0}();", this.ID);
                    sb.AppendFormat("\r\n\tInitScrollBar_{0}();", this.ID);
                    sb.Append("\r\n</script>");
                    #endregion

                }

                #endregion

            }
            else
            {
                sb.AppendFormat("<table border=0 cellspacing=0 cellpadding=0 class=\"{0}\" width=\"100%\">", WHITE_BACK_GROUND);

                if (_webSession.PeriodType == WebCst.CustomerSessions.Period.Type.dateToDate) {
                    labBegin = Dates.dateToString(Dates.getPeriodBeginningDate(realPeriodBegin, WebCst.CustomerSessions.Period.Type.dateToDate), _webSession.SiteLanguage);
                    labEnd = Dates.dateToString(Dates.getPeriodEndDate(realPeriodEnd, WebCst.CustomerSessions.Period.Type.dateToDate), _webSession.SiteLanguage);
                }
                else {
                    labBegin = Dates.dateToString(Dates.getPeriodBeginningDate(periodBegin, periodType), _webSession.SiteLanguage);
                    labEnd = Dates.dateToString(Dates.getPeriodEndDate(periodEnd, periodType), _webSession.SiteLanguage);
                }
                if (labBegin != labEnd)
                {
                    sb.AppendFormat("\r\n<tr valign=\"top\"><td witdh=\"1\" class=\"txtViolet12Bold valign=\"top\">{0} {1} {2} {3}</td></tr>",
                        GestionWeb.GetWebWord(896, _webSession.SiteLanguage),
                        labBegin,
                        GestionWeb.GetWebWord(897, _webSession.SiteLanguage),
                        labEnd);
                }
                else
                {
                    sb.AppendFormat("<tr valign=\"top\"><td witdh=\"1\" class=\"txtViolet12Bold\" valign=\"top\">{0}</td></tr>", labBegin);
                }
                sb.AppendFormat("<tr id=\"{0}_bottomLine\"><td height=\"2\"></td></tr>", this.ID);
            }
            sb.Append("</table>");
            #endregion


            return sb.ToString();
        }
        #endregion

        #region AppendPeriod
        private void AppendPeriod(WebCst.CustomerSessions.Period.Type periodType, string currentPeriod, int i, StringBuilder sb, StringBuilder tmpSb, ref int periodIndex, string globalDateBegin, string globalDateEnd, ref string labBegin, ref string labEnd)
        {
            string tmpBegin = string.Empty;
            string tmpEnd = string.Empty;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;


            sb.AppendFormat("\r\ntab_zooms_{0}[{1}] = '{2}';", this.ID, i, currentPeriod);
            if (globalDateBegin.Length > 0)
            {
                tmpBegin = Dates.dateToString(
                    Dates.Max(Dates.getZoomBeginningDate(currentPeriod, periodType),
                    Dates.getPeriodBeginningDate(globalDateBegin, WebCst.CustomerSessions.Period.Type.dateToDate))
                    , _webSession.SiteLanguage);
            }
            else
            {
                tmpBegin = Dates.dateToString(Dates.getZoomBeginningDate(currentPeriod, periodType), _webSession.SiteLanguage);
            }
            if (globalDateEnd.Length > 0)
            {
                tmpEnd = Dates.dateToString(
                    Dates.Min(Dates.getZoomEndDate(currentPeriod, periodType),
                    Dates.getPeriodEndDate(globalDateEnd, WebCst.CustomerSessions.Period.Type.dateToDate))
                    , _webSession.SiteLanguage);
            }
            else
            {
                tmpEnd = Dates.dateToString(Dates.getZoomEndDate(currentPeriod, periodType), _webSession.SiteLanguage);
            }
            if (tmpEnd != tmpBegin)
            {
                sb.AppendFormat("\r\ntab_periodLabel_{0}[{1}] = '{2} {3} {4} {5}';", this.ID, i,
                GestionWeb.GetWebWord(896, _webSession.SiteLanguage),
                tmpBegin,
                GestionWeb.GetWebWord(897, _webSession.SiteLanguage),
                tmpEnd);
            }
            else
            {
                sb.AppendFormat("\r\ntab_periodLabel_{0}[{1}] = '{2}';", this.ID, i, tmpBegin);
            }
            sb.AppendFormat("\r\ntab_periodImage_{0}[{1}] = '{2}';",
                this.ID, i,
                string.Format(IMAGE_PATTERN, themeName, Int32.Parse(currentPeriod.Substring(4, 2))));
            sb.AppendFormat("\r\ntab_periodImage_selected_{0}[{1}] = '{2}';",
                this.ID, i,
                string.Format(IMAGE_PATTERN_SELECTED, themeName, Int32.Parse(currentPeriod.Substring(4, 2))));
            sb.AppendFormat("\r\ntab_periodImage_over_{0}[{1}] = '{2}';",
                this.ID, i,
                string.Format(IMAGE_PATTERN_OVER, themeName, Int32.Parse(currentPeriod.Substring(4, 2))));
            if (_zoom == currentPeriod)
            {
                //style=\"display:none\" 
                tmpSb.AppendFormat("<img id=img_{2}_{3} src=\"" + IMAGE_PATTERN_SELECTED + "\" onMouseOver=\"javascript:PeriodMouseOver_{2}({3});\" onMouseOut=\"javascript:PeriodMouseOut_{2}({3});\" style=\"cursor:pointer;\" onclick=\"javascript:PeriodSelect_{2}({3});\"/>"
                    , themeName
                    , Int32.Parse(currentPeriod.Substring(4, 2))
                    , this.ID, i);
            }
            else
            {
                tmpSb.AppendFormat("<img id=img_{2}_{3} src=\"" + IMAGE_PATTERN + "\" onMouseOver=\"javascript:PeriodMouseOver_{2}({3});\" onMouseOut=\"javascript:PeriodMouseOut_{2}({3});\" style=\"cursor:pointer;\" onclick=\"javascript:PeriodSelect_{2}({3});\"/>"
                    , themeName
                    , Int32.Parse(currentPeriod.Substring(4, 2))
                    , this.ID, i);
          
            }

            if (_zoom == currentPeriod)
            {
                periodIndex = i;
                labEnd = tmpEnd;
                labBegin = tmpBegin;
            }


        }
        #endregion
    }
}
