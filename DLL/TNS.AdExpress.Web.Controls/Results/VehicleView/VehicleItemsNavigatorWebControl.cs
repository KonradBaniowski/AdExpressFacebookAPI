using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using Portofolio = TNS.AdExpressI.Portofolio;
using System.Reflection;
using System.Data;
using System.Collections;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using System.Globalization;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Results.VehicleView{
    /// <summary>
    /// Items collection navigator
    /// Allow navigation between the collection items
    /// </summary>
    public class VehicleItemsNavigatorWebControl : WebControl{

        #region Variables
        /// <summary>
        /// Items collection
        /// </summary>
        private List<VehicleItemWebControl> _itemsCollection = new List<VehicleItemWebControl>();
        /// <summary>
        /// Displayable Items Number(total items that will be generated in the client side)
        /// </summary>
        private long _displayableItemsNumber = 0;
        /// <summary>
        /// Visible items number (Items that will be shown in the screen)
        /// </summary>
        private long _visibleItemsNumber = 6;
        /// <summary>
        /// Initial position
        /// </summary>
        private long _itemsPositionToShow = 0;
        /// <summary>
        /// Control Name
        /// </summary>
        private string _controlName = "ItemsCollectionNavigator";
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _customerWebSession = null;
        /// <summary>
        /// Visual DataTable
        /// </summary>
        private DataTable _dtVisuel = null;
        /// <summary>
        /// Investment Values
        /// </summary>
        private Hashtable _htValue = null;
        /// <summary>
        /// Media
        /// </summary>
        private string _media = string.Empty;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// If Excel
        /// </summary>
        private bool _excel = false;
        /// <summary>
        /// result type
        /// </summary>
        private int _resultType=0;
        /// <summary>
        /// List of media to test for creative acces (press specific)
        /// </summary>
        private List<long> _mediaList = null;
        /// <summary>
        /// Media Id
        /// </summary>
        private Int64 _idMedia;
        /// <summary>
        /// Is visible
        /// </summary>
        private bool _visible = true;
        /// <summary>
        /// SubPeriodSelection Component
        /// </summary>
        private SubPeriodSelectionWebControl _subPeriodSelectionWebControl = null;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Items collection
        /// </summary>
        public List<VehicleItemWebControl> ItemsCollection{
            get { return _itemsCollection; }
            set { _itemsCollection = value; }
        }
        /// <summary>
        /// Get / Set Displayable Items Number
        /// </summary>
        public long DisplayableItemsNumber {
            get { return _displayableItemsNumber; }
            set { _displayableItemsNumber = value; }
        }
        /// <summary>
        /// Get / Set Visible items number
        /// </summary>
        public long VisibleItemsNumber {
            get { return _visibleItemsNumber; }
            set { _visibleItemsNumber = value; }
        }
        /// <summary>
        /// Get / Set Initial position
        /// </summary>
        public long ItemsPositionToShow {
            get { return _itemsPositionToShow; }
            set { _itemsPositionToShow = value; }
        }
        /// <summary>
        /// Get / Set Control Name
        /// </summary>
        public string ControlName {
            get { return _controlName; }
            set { _controlName = value; }
        }
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession{
            get { return (_customerWebSession); }
            set { 
                _customerWebSession = value;
                _subPeriodSelectionWebControl.WebSession = _customerWebSession;
            }
        }
        /// <summary>
        /// Get / Set Result type
        /// </summary>
        public int ResultType{
            get { return (_resultType); }
            set { _resultType = value; }
        }
        /// <summary>
        /// Get / Set is Excel
        /// </summary>
        public bool Excel{
            get { return (_excel); }
            set { _excel = value; }
        }
        /// <summary>
        /// Get / Set is visible
        /// </summary>
        public bool Visible{
            get { return (_visible); }
            set { _visible = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VehicleItemsNavigatorWebControl(){
            _subPeriodSelectionWebControl = new SubPeriodSelectionWebControl();
            _subPeriodSelectionWebControl.AllPeriodAllowed = false;
            _subPeriodSelectionWebControl.IsZoomEnabled = true;
            _subPeriodSelectionWebControl.ID = "subPeriodSelectionWebControl1";
            _subPeriodSelectionWebControl.PeriodContainerName = "resultParameters.Zoom";
        }
        #endregion

        #region PreRender
		/// <summary>
		/// Vehicle items initialisation
		/// </summary>
		/// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {

            if (_visible){
                string zoomDate = string.Empty;
                Domain.Web.Navigation.Module module = _customerWebSession.CustomerLogin.GetModule(_customerWebSession.CurrentModule);
                object[] parameters = new object[1];
                parameters[0] = _customerWebSession;
                Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                _media = (portofolioResult.GetVehicleViewData(out _dtVisuel, out _htValue));
                _vehicleInformation = GetVehicleInformation();
                _idMedia = GetMediaId();

                DateTime end = Dates.getPeriodBeginningDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType);
                if (_customerWebSession.DetailPeriod == WebCst.CustomerSessions.Period.DisplayLevel.weekly){
                    AtomicPeriodWeek week = new AtomicPeriodWeek(end);
                    zoomDate = string.Format("{0}{1}", week.Year, week.Week.ToString("0#"));
                }
                else{
                    zoomDate = end.ToString("yyyyMM");
                }
                _subPeriodSelectionWebControl.ZoomDate = zoomDate;
            }

        }
        #endregion

        #region Web Control Render

        #region Render
        /// <summary>
        /// Navigator Render
        /// </summary>
        /// <returns>Html code</returns>
        public string Render() {

            #region Variables
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            StringBuilder sb = new StringBuilder(5000);
            string pathWeb = "";
            CoverItemWebControl coverItem = null;
            CoverLinkItemWebControl coverLinkItem = null;
            CoverLinkItemSynthesisWebControl coverLinkItemSynthesis = null;
            VehicleItemWebControl vehicleItem = null;
            List<VehicleItemWebControl> itemsCollection = new List<VehicleItemWebControl>();
            #endregion

            // Vérifie si le client a le droit aux créations
            if (_customerWebSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id)){
                if (!_excel){
                    if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                        || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                        || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                        || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                        ){


                        StringBuilder js = new StringBuilder();
                        string setParameters = string.Empty;
                        _subPeriodSelectionWebControl.JavascriptRefresh = "SetParameters";

                        sb.Append("<table  border=0 cellpadding=0 cellspacing=0 width=\"730px\" height=\"620px\" align=center class=\"paleVioletBackGroundV2 violetBorder\">");
                        //Vehicle view
                        switch (_resultType){
                            case FrameWorkResultConstantes.Portofolio.SYNTHESIS:
                                sb.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + GestionWeb.GetWebWord(1397, _customerWebSession.SiteLanguage) + "</td></tr>");
                                break;
                            case FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA:
                                sb.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc14Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + _media + "</td></tr>");
                                break;
                        }

                        CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_customerWebSession.SiteLanguage].Localization);
                        if (_mediaList == null){
                            try{
                                string[] mediaList = Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                                if (mediaList != null && mediaList.Length > 0)
                                    _mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(mediaList, (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
                            }
                            catch { }
                        }
                        for (int i = 0; i < _dtVisuel.Rows.Count; i++){
                            //date_media_num

                            if (_dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(_dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10){
                                if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                    pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia.ToString() + "/" + _dtVisuel.Rows[i]["date_media_num"].ToString() + "/Imagette/" + WebCst.CreationServerPathes.COUVERTURE + "";
                                else pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia.ToString() + "/" + _dtVisuel.Rows[i]["date_cover_num"].ToString() + "/Imagette/" + WebCst.CreationServerPathes.COUVERTURE + "";
                            }
                            else{
                                pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";
                            }
                            DateTime dayDT = new DateTime(int.Parse(_dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0, 4)), int.Parse(_dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4, 2)), int.Parse(_dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6, 2)));

                            if (_dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(_dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10){

                                if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS){
                                    if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(_idMedia))
                                        coverLinkItemSynthesis = new CoverLinkItemSynthesisWebControl(_dtVisuel.Rows[i]["media"].ToString(), _dtVisuel.Rows[i]["number_page_media"].ToString(), _customerWebSession.IdSession, _idMedia, _dtVisuel.Rows[i]["date_media_num"].ToString(), _dtVisuel.Rows[i]["date_media_num"].ToString());
                                    else
                                        coverLinkItemSynthesis = new CoverLinkItemSynthesisWebControl(_dtVisuel.Rows[i]["media"].ToString(), _dtVisuel.Rows[i]["number_page_media"].ToString(), _customerWebSession.IdSession, _idMedia, _dtVisuel.Rows[i]["date_media_num"].ToString(), _dtVisuel.Rows[i]["date_cover_num"].ToString());
                                    coverItem = new CoverItemWebControl(i + 1, GestionWeb.GetWebWord(1409, _customerWebSession.SiteLanguage), pathWeb, coverLinkItemSynthesis);
                                }
                                else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA){
                                    coverLinkItem = new CoverLinkItemWebControl(_customerWebSession.IdSession, _idMedia, _dtVisuel.Rows[i]["date_media_num"].ToString(), "");
                                    coverItem = new CoverItemWebControl(i + 1, "", pathWeb, coverLinkItem);
                                }
                            }
                            else
                                if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                                    coverItem = new CoverItemWebControl(i + 1, GestionWeb.GetWebWord(1409, _customerWebSession.SiteLanguage), pathWeb, null);
                                else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA)
                                    coverItem = new CoverItemWebControl(i + 1, "", pathWeb, null);
                                

                            if (_htValue.Count > 0){
                                if (_htValue.ContainsKey(_dtVisuel.Rows[i]["date_cover_num"])){
                                    vehicleItem = new VehicleItemWebControl(dayDT, ((string[])_htValue[_dtVisuel.Rows[i]["date_cover_num"]])[1], int.Parse(((string[])_htValue[_dtVisuel.Rows[i]["date_cover_num"]])[0]).ToString("### ### ### ###"), _customerWebSession.SiteLanguage, coverItem);
                                }
                                else{
                                    vehicleItem = new VehicleItemWebControl(dayDT, "0", "0", _customerWebSession.SiteLanguage, coverItem);

                                }
                            }

                            _itemsCollection.Add(vehicleItem);

                        }

                        double groupDivNumber = Math.Ceiling((double)_itemsCollection.Count / _visibleItemsNumber);

                        if (_itemsCollection != null && _itemsCollection.Count > 0)
                        {

                            if (groupDivNumber > 1)
                            {
                                sb.Append("<tr><td>");
                                sb.Append(_subPeriodSelectionWebControl.RenderContents());
                                sb.Append("</td></tr>");
                            }

                            sb.Append("<tr><td>");
                            sb.Append(NavigatorRender());
                            sb.Append("</td></tr>");
                            sb.Append("</table>");
                        }
                        else return "";
                    }
                }
            }

            return sb.ToString();
        }
        #endregion

        #region NavigatorRender
        /// <summary>
        /// Navigator Render
        /// </summary>
        /// <returns>HTML code</returns>
        private string NavigatorRender() {

            StringBuilder sb = new StringBuilder(5000);
            StringBuilder temp = new StringBuilder();
            StringBuilder js = new StringBuilder();
            StringBuilder init = new StringBuilder();
            int coef = 0;
            double groupDivNumber = Math.Ceiling((double)_itemsCollection.Count / _visibleItemsNumber);
            double groupDivNumberInv = groupDivNumber;
            double groupDivNumberInvPrev = groupDivNumberInv;
            List<string> contentBuilder = new List<string>((int)groupDivNumber);
            int index = 1;
            int imageIndex = 1;
            DateTime currMonth;
            DateTime prevMonth = new DateTime(1,1,1);
            int monthDif = 0;

            #region script
            sb.Append("\r\n <script type=\"text/javascript\">   ");

            #region Init
            sb.Append("\n   var imagesName_" + _controlName + " = new Array(" + _itemsCollection.Count + ");    ");
            sb.Append("\n   var currentPanelIndexPrev_" + _controlName + " = " + (groupDivNumber - 2) + ";      ");
            sb.Append("\n   var currentPanelIndexNext_" + _controlName + " = " + (groupDivNumber+1) +  ";       ");
            sb.Append("\n   var initImageIndex_" + _controlName + " = 12;                                       ");
            sb.Append("\n   var indexInf_" + _controlName + " = 0;                                              ");
            sb.Append("\n   var indexSup_" + _controlName + " = 0;                                              ");
            sb.Append("\n   var time_" + _controlName + " = 0;                                                  ");
            foreach (VehicleItemWebControl vehicleItem in _itemsCollection)
                sb.Append("\n imagesName_" + _controlName + "[" + imageIndex++ + "] = '" + vehicleItem.CoverItem.Src + "';");

            sb.Append("\nvar navigatorIndex_" + _controlName + " = " + groupDivNumber + ";");
            sb.Append("\nvar prevNavigatorIndex_" + _controlName + " = -1;");
            sb.Append("\nvar minIndex_" + _controlName + " = 1;");
            sb.Append("\nvar maxIndex_" + _controlName + " = " + groupDivNumber + ";");
            #endregion

            #region Load Complete
            sb.Append("\n   function loadComplete_" + _controlName + "(){                                                           ");
            sb.Append("\n                                                                                                           ");

            if (groupDivNumber == 1)
            {
                sb.Append("\n           document.getElementById('imgNext').style.visibility = 'hidden';                             ");
                sb.Append("\n           document.getElementById('imgPrevious').style.visibility = 'hidden';                         ");
            }
            else
            {
                sb.Append("\n       if(navigatorIndex_" + _controlName + "==" + groupDivNumber + "){                                ");
                sb.Append("\n           document.getElementById('imgNext').style.visibility = 'hidden';                             ");
                sb.Append("\n           document.getElementById('imgPrevious').style.visibility = 'visible';                        ");
                sb.Append("\n       }                                                                                               ");

                sb.Append("\n       else if(navigatorIndex_" + _controlName + "== 1){                                               ");
                sb.Append("\n           document.getElementById('imgPrevious').style.visibility = 'hidden';                         ");
                sb.Append("\n           document.getElementById('imgNext').style.visibility = 'visible';                            ");
                sb.Append("\n       }                                                                                               ");

                sb.Append("\n       else {                                                                                          ");
                sb.Append("\n           document.getElementById('imgPrevious').style.visibility = 'visible';                        ");
                sb.Append("\n           document.getElementById('imgNext').style.visibility = 'visible';                            ");
                sb.Append("\n       }                                                                                               ");
            }

            sb.Append("\n       document.getElementById('item0').innerHTML = document.getElementById('item'+navigatorIndex_" + _controlName + ").innerHTML;   ");
            sb.Append("\n   }                                                                                                       ");
            #endregion

            #region loadImagesPanel
            sb.Append("\n   function loadImagesPanel_" + _controlName + "(currentPanelIndex){                                       ");

            sb.Append("\n       indexInf_" + _controlName + " = imagesToLoad['item'+currentPanelIndex];                             ");
            sb.Append("\n       indexSup_" + _controlName + " = indexInf_" + _controlName + " + 6;                                  ");
            sb.Append("\n       for(var i= indexInf_" + _controlName + "; i<indexSup_" + _controlName + ";i++)                      ");
            sb.Append("\n               if(document.getElementById(i) != null)                                                      ");
            sb.Append("\n                   document.getElementById(i).src=''+imagesName_" + _controlName + "[i]+'';                ");
            sb.Append("\n       imagesLoaded['item'+currentPanelIndex] = 1;                                                         ");
            sb.Append("\n       time_" + _controlName + " = 200;                                                                    ");
            sb.Append("\n   }                                                                                                       ");
            #endregion

            #region SetParameters
            sb.Append("\r\n function SetParameters(){                                                                               ");
            sb.Append("\n       time_" + _controlName + " = 0;                                                                      ");
            sb.Append("\n       if(tabCorres[''+resultParameters.Zoom+''] != null){                                                 ");
            sb.Append("\n           prevNavigatorIndex_" + _controlName + "=navigatorIndex_" + _controlName + ";                    ");
            sb.Append("\n           navigatorIndex_" + _controlName + "=tabCorres[''+resultParameters.Zoom+''].substring(4);        ");
            sb.Append("\n           if(prevNavigatorIndex_" + _controlName + " != navigatorIndex_" + _controlName + ")              ");
            sb.Append("\n               document.getElementById('item0').innerHTML = document.getElementById('itemLoading').innerHTML;  ");
            sb.Append("\n           currentPanelIndexPrev_" + _controlName + " = navigatorIndex_" + _controlName + ";               ");
            sb.Append("\n           currentPanelIndexNext_" + _controlName + " = parseInt(navigatorIndex_" + _controlName + ") + 1; ");
            sb.Append("\n           if(imagesLoaded['item'+currentPanelIndexPrev_" + _controlName + "] == 0)                        ");
            sb.Append("\n               loadImagesPanel_" + _controlName + "(currentPanelIndexPrev_" + _controlName + ");           ");
            sb.Append("\n           currentPanelIndexPrev_" + _controlName + "--;                                                   ");
            sb.Append("\n           if(imagesLoaded['item'+currentPanelIndexPrev_" + _controlName + "] == 0)                        ");
            sb.Append("\n               loadImagesPanel_" + _controlName + "(currentPanelIndexPrev_" + _controlName + ");           ");
            sb.Append("\n           if(imagesLoaded['item'+currentPanelIndexNext_" + _controlName + "] == 0)                        ");
            sb.Append("\n               loadImagesPanel_" + _controlName + "(currentPanelIndexNext_" + _controlName + ");           ");
            sb.Append("\r\n         setTimeout(loadComplete_" + _controlName + ",time_" + _controlName + ");                        ");
            sb.Append("\r\n     }                                                                                                   ");

            sb.Append("\r\n }                                                                                                       ");
            #endregion

            #region nextPos
            sb.Append("\n   function nextPos_" + _controlName + "(){                                                                ");
            sb.Append("\n       time_" + _controlName + " = 0;                                                                      ");

            sb.Append("\n       if(navigatorIndex_" + _controlName + "<maxIndex_" + _controlName + "){                              ");
            sb.Append("\n           prevNavigatorIndex_" + _controlName + "=navigatorIndex_" + _controlName + ";                    ");
            sb.Append("\n           navigatorIndex_" + _controlName + "++;                                                          ");
            sb.Append("\n           currentPanelIndexNext_" + _controlName + " = parseInt(navigatorIndex_" + _controlName + ") + 1; ");
            sb.Append("\n       }                                                                                                   ");
            sb.Append("\n       else return false;                                                                                  ");

            sb.Append("\n       if((imagesLoaded['item'+currentPanelIndexNext_" + _controlName + "] == 0)){ ");
            sb.Append("\n            document.getElementById('item0').innerHTML = document.getElementById('itemLoading').innerHTML; ");
            sb.Append("\n            loadImagesPanel_" + _controlName + "(currentPanelIndexNext_" + _controlName + ");              ");
            sb.Append("\n       }                                                                                                   ");
            sb.Append("\r\n          setTimeout(loadComplete_" + _controlName + ",time_" + _controlName + ");                       ");

            sb.Append("\n       return false;                                                                                       ");
            sb.Append("\n   }                                                                                                       ");
            #endregion

            #region prevPos
            sb.Append("\n   function prevPos_" + _controlName + "(){                                                                ");
            sb.Append("\n       time_" + _controlName + " = 0;  ");

            sb.Append("\n       if(navigatorIndex_" + _controlName + ">minIndex_" + _controlName + "){                              ");
            sb.Append("\n           prevNavigatorIndex_" + _controlName + "=navigatorIndex_" + _controlName + ";                    ");
            sb.Append("\n           navigatorIndex_" + _controlName + "--;                                                          ");
            sb.Append("\n           currentPanelIndexPrev_" + _controlName + " = parseInt(navigatorIndex_" + _controlName + ") - 1; ");
            
            sb.Append("\n       }                                                                                                   ");
            sb.Append("\n       else return false;                                                                                  ");
            
            sb.Append("\n       if((imagesLoaded['item'+currentPanelIndexPrev_" + _controlName + "] == 0)){ ");
            sb.Append("\n            document.getElementById('item0').innerHTML = document.getElementById('itemLoading').innerHTML; ");
            sb.Append("\n            loadImagesPanel_" + _controlName + "(currentPanelIndexPrev_" + _controlName + ");              ");
            sb.Append("\n       }                                                                                                   ");
            sb.Append("\r\n          setTimeout(loadComplete_" + _controlName + ",time_" + _controlName + ");                        ");

            sb.Append("\n       return false;                                                                                       ");
            sb.Append("\n   }                                                                                                       ");
            #endregion

            #region Ready
            sb.Append("\n   $(document).ready(function() {                                                                          ");
            sb.Append("\n       document.getElementById('item0').innerHTML = document.getElementById('itemLoading').innerHTML;      ");
            sb.Append("\n       loadImagesPanel_" + _controlName + "(" + groupDivNumber + ");                                       ");
            sb.Append("\n       loadImagesPanel_" + _controlName + "(" + (groupDivNumber-1) + ");                                   ");
            sb.Append("\r\n     setTimeout(loadComplete_" + _controlName + ",time_" + _controlName + ");                            ");
            sb.Append("\n   });                                                                                                     ");
            #endregion

            sb.Append("</script>");
            #endregion

            #region style
            sb.Append("<style>");

            sb.Append("\n#next{");
            sb.Append("\nfloat:left;");
            sb.Append("\nposition:relative;");
            sb.Append("\n}");

            sb.Append("\n#container{");
            sb.Append("\nfloat:left;");
            sb.Append("\nposition:relative;");
            sb.Append("\n}");

            sb.Append("\n#previous{");
            sb.Append("\nfloat:left;");
            sb.Append("\nposition:relative;");
            sb.Append("\n}");

            sb.Append("\n.item {");
            sb.Append("\nfloat:left;");
            sb.Append("\n}");

            sb.Append("\n.content {");
            sb.Append("\nwidth:664px;");
            sb.Append("\nheight:548px;");
            sb.Append("\nposition:relative;");
            sb.Append("\n}");

            sb.Append("\n.selected {");
            sb.Append("\n}");

            sb.Append("\n.clear {");
            sb.Append("\nclear:both;");
            sb.Append("\n}");

            sb.Append("</style>");
            #endregion

            sb.Append("<div id=\"ItemsCollectionNavigator\">");

            sb.Append("<div id=\"previous\" height=\"548px\" width=\"20px\"><img id=\"imgPrevious\" src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/preview.gif' onMouseOver=\"this.src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/previewUp.gif'\" onMouseOut=\"this.src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/preview.gif'\" style=\"cursor : pointer; margin-top:264px; margin-right:5px;margin-left:5px; visibility:hidden;\" onclick=\"javascript:prevPos_" + _controlName + "();\" /></div>");

            sb.Append("<div id=\"container\">");
            sb.Append("<div id=\"itemLoading\" class=\"item content\" style =\"display : none; text-align:center;\">");
            sb.Append("<img src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/wait.gif' style=\"margin-top:264px;\"/>");
            sb.Append("</div>");
            sb.Append("<div id=\"item0\" class=\"item content\" style =\"text-align:center;\">");
            sb.Append("</div>");
            js.Append("<script>");
            js.Append("var tabCorres = new Array();");
            init.Append("<script>");
            js.Append("var imagesToLoad = new Array();");
            js.Append("var imagesLoaded = new Array();");
            if(_itemsCollection[coef] != null)
                prevMonth= _itemsCollection[coef].ParutionDate.AddMonths(1);
            for (int i = 1; i <= groupDivNumber; i++)
            {
                temp = new StringBuilder();

                if (_itemsCollection[coef] != null)
                {
                    currMonth = _itemsCollection[coef].ParutionDate;
                    if (currMonth.Month != prevMonth.Month)
                    {
                        monthDif = monthDifference(currMonth, prevMonth);
                        if (monthDif > 1)
                            for (int m = 0; m < monthDif; m++)
                                js.Append("tabCorres['" + currMonth.AddMonths(m).ToString("yyyyMM") + "'] = 'item'+" + groupDivNumberInvPrev + ";");

                        js.Append("tabCorres['" + currMonth.ToString("yyyyMM") + "'] = 'item'+" + groupDivNumberInv + ";");
                        groupDivNumberInvPrev = groupDivNumberInv;
                        prevMonth = currMonth;
                    }
                }
                
                init.Append("imagesToLoad['item" + groupDivNumberInv + "'] = "+ (coef+1) +";");
                init.Append("imagesLoaded['item" + groupDivNumberInv + "'] = 0;");

                temp.Append("<div id=\"item" + groupDivNumberInv-- + "\" class=\"item content\" style=\"display : none;\">");

                for (int j = coef; (j < coef + _visibleItemsNumber) && (j < _itemsCollection.Count); j++)
                {
                    temp.Append("<div style=\"float:left; width:220px\" class=\"" + ((index >= 1 && index <= 3) ? (((index++ == 1) ? "portofolioSynthesisBorder" : "portofolioSynthesisBorderRight")) : (((index++ == 4) ? "portofolioSynthesisBorderBottom" : "portofolioSynthesisBorderBottomRight"))) + "\">");
                    temp.Append(_itemsCollection[j].Render());
                    temp.Append("</div>");
                    if (index == 7) index = 1;
                }
                temp.Append("</div>");
                contentBuilder.Add(temp.ToString());
                coef += (int)_visibleItemsNumber;
            }

            if (_itemsCollection[(_itemsCollection.Count - 1)] != null)
            {
                currMonth = _itemsCollection[(_itemsCollection.Count - 1)].ParutionDate;
                if (currMonth.Month != prevMonth.Month)
                {
                    monthDif = monthDifference(currMonth, prevMonth);
                    if (monthDif > 1)
                        for (int m = 0; m < monthDif; m++)
                            js.Append("tabCorres['" + currMonth.AddMonths(m).ToString("yyyyMM") + "'] = 'item'+" + groupDivNumberInvPrev + ";");
                }
            }

            js.Append("</script>"); 
            init.Append("</script>");
            sb.Append(js.ToString());
            sb.Append(init.ToString());

            for (int k = contentBuilder.Count - 1; k >= 0; k--)
                sb.Append(contentBuilder[k].ToString());

            sb.Append("</div>");

            sb.Append("<div id=\"next\" height=\"548px\" width=\"20px\"><img id=\"imgNext\" src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/next.gif' onMouseOver=\"this.src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/nextUp.gif'\" onMouseOut=\"this.src='/App_Themes/" + WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name + "/Images/Common/Button/next.gif'\"  style=\"cursor : pointer; margin-top:264px; margin-right:5px;margin-left:5px; visibility:hidden;\" onclick=\"javascipt:nextPos_" + _controlName + "();\"/></div>");


            sb.Append("</div>");

            return sb.ToString();

        }
        #endregion

        #region Render(HtmlTextWriter output)
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output) {
            if (_visible && _dtVisuel!=null && _htValue!=null){
                output.Write(Render());
            }
        }
        #endregion

        #endregion

        #region Private Methods

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        private string GetVehicle(){
            string vehicleSelection = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new Exception("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        private VehicleInformation GetVehicleInformation(){
            try{
                return (VehiclesInformation.Get(Int64.Parse(GetVehicle())));
            }
            catch (System.Exception err){
                throw (new Exception("Impossible to retreive vehicle selection", err));
            }
        }
        #endregion

        #region Media Selection
        /// <summary>
        /// Get Media Id
        /// </summary>
        /// <returns>Media Id</returns>
        private Int64 GetMediaId(){
            try{
                return (((LevelInformation)_customerWebSession.ReferenceUniversMedia.FirstNode.Tag).ID);
            }
            catch (System.Exception err){
                throw (new Exception("Impossible to retrieve media id", err));
            }
        }
        #endregion

        #region monthDifference
        /// <summary>
        /// Diffrence between two months
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>difference</returns>
        private int monthDifference(DateTime startDate, DateTime endDate){
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
        #endregion

        #endregion
    }
}
