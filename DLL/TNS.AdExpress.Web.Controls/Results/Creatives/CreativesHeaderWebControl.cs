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

using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Controls.Headers;
using DBClassifCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Controls.Results.Creatives {


	///<summary>
	/// Control to drive the display of a set of creatives filtered by period and vehicle
	/// </summary>
	///  <author>G Ragneau</author>
	///  <since>09/08/2007</since>
	///  <stereotype>control</stereotype>
    [ToolboxData("<{0}:CreativesHeaderWebControl runat=server></{0}:CreativesHeaderWebControl>")]
    public class CreativesHeaderWebControl : WebControl {

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
            set{
                _webSession = value;
                _subPeriodSelectionWebControl.WebSession = _webSession;
            }
        }
        /// <summary>
        /// Vehicle List
        /// </summary>
		protected IList<long> _vehicles = new List<long>();
        /// <summary>
        /// Get / Set list of vehicles
        /// </summary>
		public IList<long> Vehicles {
            get {
                return _vehicles;
            }
            set {
                _vehicles = value;
            }        
        }
        /// <summary>
        /// Cuurent Vehicle Id
        /// </summary>
		protected long _idVehicle = -1;
        /// <summary>
        /// Get / Set current vehicle id
        /// </summary>
		public long IdVehicle {
            get {
                return _idVehicle;
            }
            set {
                _idVehicle = value;
            }
        }
        /// <summary>
        /// Zomm on this specific date
        /// </summary>
        protected string _zoom = string.Empty;
        /// <summary>
        /// Get / Set current zoomdate
        /// </summary>
        public string ZoomDate {
            get {
                return _zoom;
            }
            set {
                _zoom = value;
                _subPeriodSelectionWebControl.ZoomDate = _zoom;
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
            }
        }
        /// <summary>
        /// Name of the javascript variable containing the id of the current vehicle
        /// </summary>
        protected string _vehicleContainerName = string.Empty;
        /// <summary>
        /// Get / set the Name of the javascript variable containing the id of the current vehicle
        /// </summary>
        public string VehicleContainerName {
            get {
                return _vehicleContainerName;
            }
            set {
                _vehicleContainerName = value;
            }
        }
        /// <summary>
        /// Name of the javascript variable containing the current period
        /// </summary>
        protected string _periodContainerName = string.Empty;
        /// <summary>
        /// Get / set the Name of the javascript variable containing the current period
        /// </summary>
        public string PeriodContainerName {
            get {
                return _periodContainerName;
            }
            set {
                _periodContainerName = value;
                _subPeriodSelectionWebControl.PeriodContainerName = _periodContainerName;
            }
        }
        /// <summary>
        /// Name of the javascript function to call to refresh data
        /// </summary>
        protected string _javascriptRefresh = string.Empty;
        /// <summary>
        /// Get / set the javascript function to call to refresh data
        /// </summary>
        public string JavascriptRefresh {
            get {
                return _javascriptRefresh;
            }
            set {
                _javascriptRefresh = value;
                _subPeriodSelectionWebControl.JavascriptRefresh = _javascriptRefresh;
            }
        }
        /// <summary>
        /// Specify if the control must call the javascript refreshing at the control initiation
        /// </summary>
        protected bool _autoInitRefresh = true;
        /// <summary>
        /// Specify if the control must call the javascript refreshing at the control initiation
        /// </summary>
        public bool AutoInitRefresh
        {
            get {
                return _autoInitRefresh;
            }
            set {
                _autoInitRefresh = value;
            }
        }
        /// <summary>
        /// SubPeriodSelection Component
        /// </summary>
        SubPeriodSelectionWebControl _subPeriodSelectionWebControl = null;
        /// <summary>
        /// Specify if "All periods" optrion is allowed for more than 4 month
        /// </summary>
        protected bool _isAllPeriodIsRestrictTo4Month = true;
        /// <summary>
        /// Get / Set "All periods" optrion is allowed for more than 4 month
        /// </summary>
        public bool IsAllPeriodIsRestrictTo4Month {
            get {
                return _isAllPeriodIsRestrictTo4Month;
            }
            set {
                _isAllPeriodIsRestrictTo4Month = value;
                _subPeriodSelectionWebControl.IsAllPeriodIsRestrictTo4Month = _isAllPeriodIsRestrictTo4Month;
            }
        }

	    protected List<string> _activePeriods = null;
        /// <summary>
        /// Get /Set Active Periods
        /// </summary>
	    public List<string> ActivePeriods {
            get { return _activePeriods; }
	   
	        set
	        {
	            _activePeriods = value;
	            _subPeriodSelectionWebControl.ActivePeriods = value;
	        }
	    }
        #endregion

        #region Constructor

	    /// <summary>
	    /// Default Builder
	    /// </summary>
	    public CreativesHeaderWebControl()
	    {
	        //_subPeriodSelectionWebControl = new SubPeriodSelectionWebControl {ActivePeriods = ActivePeriods,UseCookie = false};
	        _subPeriodSelectionWebControl = new SubPeriodSelectionWebControl();
	    }

	    #endregion

        #region RenderContents
        /// <summary>
        /// Render Control
        /// </summary>
        /// <param name="output">output</param>
        protected override void RenderContents(HtmlTextWriter output) {

            StringBuilder sb = new StringBuilder();

            sb.Append("<table width=\"100%\">");

            sb.Append("<tr><td>");
            
            #region vehicles
            //vehicles
            sb.Append("<script language=javascript>");
            sb.AppendFormat("\r\n {0} = -1;", this._vehicleContainerName);
            sb.AppendFormat("\r\nvar call_count_{0} = {1};", this.ID, ((_autoInitRefresh)?1:0));
            sb.AppendFormat("\r\nvar current_vehicle_{0}_caption = null;", this.ID);
            sb.AppendFormat("\r\nfunction setVehicle_{0}(id, captionid)", this.ID);
            sb.Append("\r\n{");
            sb.AppendFormat("\r\nif ({0} != id)", this._vehicleContainerName);
            sb.Append("\r\n{");
            sb.AppendFormat("\r\n{0} = id", this._vehicleContainerName);
            sb.AppendFormat("\r\nif (current_vehicle_{0}_caption != null)", this.ID);
            sb.AppendFormat("\r\n\tcurrent_vehicle_{0}_caption.className = '';", this.ID);
            sb.AppendFormat("\r\ncurrent_vehicle_{0}_caption = document.getElementById(captionid);", this.ID);
            sb.AppendFormat("\r\ncurrent_vehicle_{0}_caption.className = 'selected';", this.ID);
            sb.AppendFormat("\r\nif (call_count_{0} > 0)", this.ID);
            sb.Append("\r\n{");
            sb.AppendFormat("\r\n{0}();", this._javascriptRefresh);
            sb.Append("\r\n}");
            sb.Append("\r\n}");
            sb.AppendFormat("\r\n call_count_{0} = 1;", this.ID);
            sb.Append("\r\n}");
            sb.Append("</script><div class=\"shadetabs\"><ul>");

            string vehicle = string.Empty;
            for (int i = 0; i < _vehicles.Count; i++) {

                #region Vehicle Label
                switch (VehiclesInformation.DatabaseIdToEnum(_vehicles[i])) {
                    case DBClassifCst.Vehicles.names.newspaper:
                        vehicle = GestionWeb.GetWebWord(2620, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.magazine:
                        vehicle = GestionWeb.GetWebWord(2621, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.press: 
                        vehicle = GestionWeb.GetWebWord(1298, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.pressClipping:
                        vehicle = GestionWeb.GetWebWord(2955, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.internationalPress:
                        vehicle = GestionWeb.GetWebWord(646, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.radio:
                        vehicle = GestionWeb.GetWebWord(644, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.radioGeneral:
                        vehicle = GestionWeb.GetWebWord(2630, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.radioSponsorship:
                        vehicle = GestionWeb.GetWebWord(2632, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.radioMusic:
                        vehicle = GestionWeb.GetWebWord(2631, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.tv:
                        vehicle = GestionWeb.GetWebWord(1300, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.tvGeneral:
                        vehicle = GestionWeb.GetWebWord(2633, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.tvClipping:
                        vehicle = GestionWeb.GetWebWord(2956, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.tvSponsorship:
                        vehicle = GestionWeb.GetWebWord(2634, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.tvAnnounces:
                        vehicle = GestionWeb.GetWebWord(2635, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.tvNonTerrestrials:
                        vehicle = GestionWeb.GetWebWord(2636, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.others:
                        vehicle = GestionWeb.GetWebWord(647, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.outdoor:
                        vehicle = GestionWeb.GetWebWord(1302, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.instore:
                        vehicle = GestionWeb.GetWebWord(2665, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.indoor:
                        vehicle = GestionWeb.GetWebWord(2644, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.adnettrack:
                        vehicle = GestionWeb.GetWebWord(648, _webSession.SiteLanguage); 
                        break;
                    case DBClassifCst.Vehicles.names.directMarketing:
                    case DBClassifCst.Vehicles.names.mailValo:
                        vehicle = GestionWeb.GetWebWord(2989, _webSession.SiteLanguage); 
                        break;                                         
                    case DBClassifCst.Vehicles.names.internet:
                    case DBClassifCst.Vehicles.names.czinternet:
                        vehicle = GestionWeb.GetWebWord(1301, _webSession.SiteLanguage); 
                        break;
					case DBClassifCst.Vehicles.names.evaliantMobile:
						vehicle = GestionWeb.GetWebWord(2577, _webSession.SiteLanguage);
						break;
                    case DBClassifCst.Vehicles.names.cinema:
                        vehicle = GestionWeb.GetWebWord(2726, _webSession.SiteLanguage);
                        break;
                    case DBClassifCst.Vehicles.names.editorial:
                        vehicle = GestionWeb.GetWebWord(2801, _webSession.SiteLanguage);
                        break;
                        
                }
                #endregion

                if ((i == 0 && _idVehicle < 0) || _idVehicle==_vehicles[i])
                    sb.AppendFormat("<li class=\"selected\" id=\"vehicle_{1}_{2}\"><a href=\"javascript:setVehicle_{2}({1},'vehicle_{1}_{2}');\">{0}</a></li>", vehicle, _vehicles[i], this.ID);
                else
                    sb.AppendFormat("<li id=\"vehicle_{1}_{2}\"><a href=\"javascript:setVehicle_{2}({1},'vehicle_{1}_{2}');\">{0}</a></li>", vehicle, _vehicles[i], this.ID);
			}
            #endregion

            sb.AppendFormat("</ul></div></td></tr>", _vehicles.Count);

            #region Period Rendering
            sb.Append("<tr><td>");

            output.WriteLine(sb.ToString());

            _subPeriodSelectionWebControl.RenderControl(output);

            sb = new StringBuilder();

            sb.Append("<br/></td></tr>");
            #endregion

            //set current vehicle
            sb.Append("<script language=javascript>");
            sb.AppendFormat("\r\n setVehicle_{0}({1},'vehicle_{1}_{0}')"
                , this.ID
                , (_idVehicle>=0)?_idVehicle:_vehicles[0]);
            sb.Append("</script>");

            
            sb.Append("</table>");

            output.Write(sb.ToString());
        }
        #endregion

    }
}
