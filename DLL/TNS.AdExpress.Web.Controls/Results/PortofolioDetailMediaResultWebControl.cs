#region Information
// Auteur : Y. R'kaina
// Cr�� le : 17/08/2007
// Modifi� le :
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.DB.Common;

using Oracle.DataAccess.Client;

using WebRules = TNS.AdExpress.Web.Rules;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;


namespace TNS.AdExpress.Web.Controls.Results {

    /// <summary>
    /// tableau generic pour la page du detail media du portefeuille
    /// </summary>
    public class PortofolioDetailMediaResultWebControl : ResultWebControl {

        #region Variables
        /// <summary>
        /// Rappel de la s�lection
        /// </summary>
        TNS.AdExpress.Web.Controls.Selections.ZoomDetailSelectionWebControl zoomDetailSelectionWebControl = null;
        #endregion

        #region Propri�t�s
        /// <summary>
        /// L'identifiant du media
        /// </summary>
        protected string _mediaId = "";
        /// <summary>
        /// Dans le cas de la radio-tv
        /// On pr�cise le jour de la semaine
        /// </summary>
        public string _dayOfWeek = "";
        /// <summary>
        /// Code �cran
        /// </summary>
        public string _adBreak = "";
        #endregion

        #region Accessors
        /// <summary>
        /// Obtient ou d�finit l'identifiant du media
        /// </summary>
        public string MediaId {
            get { return _mediaId; }
            set { _mediaId = value; }
        }
        /// <summary>
        /// Obtient ou d�finit le jour de la semaine
        /// </summary>
        public string DayOfWeek {
            get { return _dayOfWeek; }
            set { _dayOfWeek = value; }
        }
        /// <summary>
        /// Obtient ou d�finit le Code �cran
        /// </summary>
        public string AdBreak {
            get { return _adBreak; }
            set { _adBreak = value; }
        }
        #endregion

        #region Enregistrement des param�tres de construction du r�sultat
        /// <summary>
        /// G�n�ration du JavaScript pour d�finir les param�tres de r�sultat
        /// </summary>
        /// <returns></returns>
        protected override string SetResultParametersScript() {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\nfunction SetResultParameters(obj){");
            js.Append("\r\n\t obj.MediaId = '" + _mediaId + "';");
            js.Append("\r\n\t obj.DayOfWeek = '" + _dayOfWeek + "';");
            js.Append("\r\n\t obj.AdBreak = '" + _adBreak + "';");
            js.Append("\r\n }");
            return (js.ToString());
        }
        #endregion

        #region Chargement des param�tres AjaxPro.JavaScriptObject
        /// <summary>
        /// Charge les param�tres des r�sultats navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de param�tres javascript</param>
        protected override void LoadResultParameters(AjaxPro.JavaScriptObject o) {
            if (o != null) {
                if (o.Contains("MediaId")) {
                    _mediaId = o["MediaId"].Value;
                    _mediaId = _mediaId.Replace("\"", "");
                }
                if (o.Contains("DayOfWeek")) {
                    _dayOfWeek = o["DayOfWeek"].Value;
                    _dayOfWeek = _dayOfWeek.Replace("\"", "");
                }
                if (o.Contains("AdBreak")) {
                    _adBreak = o["AdBreak"].Value;
                    _adBreak = _adBreak.Replace("\"", "");
                }
            }
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialisation du composant
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            this._sSortKey = _customerWebSession.SortKey;
            this._sortOrder = _customerWebSession.Sorting;
            switch (_renderType) {
                case RenderType.excel:
                    zoomDetailSelectionWebControl = new TNS.AdExpress.Web.Controls.Selections.ZoomDetailSelectionWebControl();
                    break;
            }
        }
        /// <summary>
        /// Initialisation du composant
        /// </summary>
        protected override void DetailSelectionInit() { }
        #endregion

        #region Pr�render
        /// <summary>
        /// Pr�render
        /// </summary>
        /// <param name="e">Arguements</param>
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            switch (_renderType) {
                case RenderType.excel:
                    zoomDetailSelectionWebControl.WebSession = _customerWebSession;
                    zoomDetailSelectionWebControl.PeriodBeginning = _customerWebSession.PeriodBeginningDate;
                    zoomDetailSelectionWebControl.PeriodEnd = _customerWebSession.PeriodEndDate;
                    zoomDetailSelectionWebControl.DateLabel = GestionWeb.GetWebWord(2267, _customerWebSession.SiteLanguage);

                    #region Zoom Date

                    #region Obtention du vehicle
                    string vehicleSelection = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
                    if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
                    DBClassificationConstantes.Vehicles.names vehicle = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
                    #endregion

                    DateTime dayOfWeek = DateTime.Now;

                    if(_dayOfWeek.Length > 0){
                        zoomDetailSelectionWebControl.ShowZoom = true;
                        if (vehicle == DBClassificationConstantes.Vehicles.names.press || vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                            dayOfWeek = new DateTime(int.Parse(_dayOfWeek.ToString().Substring(0, 4)), int.Parse(_dayOfWeek.Substring(4, 2)), int.Parse(_dayOfWeek.ToString().Substring(6, 2)));
                            zoomDetailSelectionWebControl.ZoomDate = WebFunctions.Dates.dateToString(dayOfWeek.Date, _customerWebSession.SiteLanguage);
                        }
                        else {
                            zoomDetailSelectionWebControl.ZoomDate = string.Empty;
                        }
                    }
                    else
                        zoomDetailSelectionWebControl.ShowZoom = false;
                    #endregion

                    zoomDetailSelectionWebControl.OutputType = _renderType;
                    zoomDetailSelectionWebControl.CssLevel1 = CssDetailSelectionL1;
                    zoomDetailSelectionWebControl.CssLevel2 = CssDetailSelectionL2;
                    zoomDetailSelectionWebControl.CssLevel3 = CssDetailSelectionL3;
                    zoomDetailSelectionWebControl.CssRightBorderLevel1 = CssDetailSelectionRightBorderL1;
                    zoomDetailSelectionWebControl.CssRightBorderLevel2 = CssDetailSelectionRightBorderL2;
                    zoomDetailSelectionWebControl.CssRightBorderLevel3 = CssDetailSelectionRightBorderL3;
                    zoomDetailSelectionWebControl.CssRightBottomBorderLevel1 = CssDetailSelectionRightBottomBorderL1;
                    zoomDetailSelectionWebControl.CssRightBottomBorderLevel2 = CssDetailSelectionRightBottomBorderL2;
                    zoomDetailSelectionWebControl.CssRightBottomBorderLevel3 = CssDetailSelectionRightBottomBorderL3;
                    zoomDetailSelectionWebControl.CssTitleGlobal = CssDetailSelectionTitleGlobal;
                    zoomDetailSelectionWebControl.CssTitle = CssDetailSelectionTitle;
                    zoomDetailSelectionWebControl.CssTitleData = CssDetailSelectionTitleData;
                    zoomDetailSelectionWebControl.CssBorderLevel = CssDetailSelectionBordelLevel;
                    break;
            }
        }
        /// <summary>
        /// Pr�render
        /// </summary>
        protected override void DetailSelectionPreRender() {}
        #endregion

        #region Render
        /// <summary> 
        /// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel �crire </param>
        protected override void Render(HtmlTextWriter output) {

            if (_renderType == RenderType.html)
                base.Render(output);
            else {
                if (_customerWebSession == null) {
                    output.WriteLine("<table cellpadding=0 cellspacing=0 bgcolor=#644883>");
                    output.WriteLine("<tr>");
                    output.WriteLine("<td>Tableau r�sultat g�n�rique</td>");
                    output.WriteLine("</tr>");
                    output.WriteLine("</table>");
                }
                _data = GetResultTable(_customerWebSession);
                if (_data != null) {
					output.WriteLine(zoomDetailSelectionWebControl.GetLogo(_customerWebSession));
					output.WriteLine(zoomDetailSelectionWebControl.GetHeader());
					output.WriteLine(base.GetExcel());
					output.WriteLine(zoomDetailSelectionWebControl.GetFooter());
                }
            }
        }
        #endregion

        #region GetResultTable
        /// <summary>
        /// Obtient le tableau de r�sultats
        /// </summary>
        /// <param name="customerWebSession">Session du client</param>
        /// <returns>tableau de r�sultats</returns>
        protected override ResultTable GetResultTable(WebSession customerWebSession) {

            if (customerWebSession.CustomerLogin.Connection == null) {
                TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(customerWebSession.CustomerLogin.OracleConnectionString));
                customerWebSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
            }

            return WebRules.Results.PortofolioRules.GetPortofolioDetailMediaResultTable(customerWebSession, _dayOfWeek, _adBreak);

        }
        #endregion

        #region Chargement du code html du d�tail de s�lection
        /// <summary>
        /// Chargement du code html du d�tail de s�lection
        /// </summary>
        /// <returns></returns>
        protected override string LoadHtmlDetailSelection(AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters, AjaxPro.JavaScriptObject sortParameters) {
           
            try {
           
                DateTime dayOfWeek=DateTime.Now;

                // Chargement des param�tres javascript
                this.LoadResultParameters(resultParameters);
                this.LoadStyleParameters(styleParameters);
                this.LoadSortParameters(sortParameters);

                // Composant DetailSelctionWebControl pour la r�cup�ration du code html
                TNS.AdExpress.Web.Controls.Selections.ZoomDetailSelectionWebControl zoomDetailSelectionWebControl = new TNS.AdExpress.Web.Controls.Selections.ZoomDetailSelectionWebControl();
                zoomDetailSelectionWebControl.WebSession = _customerWebSession;
                zoomDetailSelectionWebControl.PeriodBeginning = _customerWebSession.PeriodBeginningDate;
                zoomDetailSelectionWebControl.PeriodEnd = _customerWebSession.PeriodEndDate;
                zoomDetailSelectionWebControl.DateLabel = GestionWeb.GetWebWord(2267, _customerWebSession.SiteLanguage);

                #region Zoom Date

                #region Obtention du vehicle
                string vehicleSelection = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
                DBClassificationConstantes.Vehicles.names vehicle = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
                #endregion

                if (_dayOfWeek.Length > 0) {
                    zoomDetailSelectionWebControl.ShowZoom = true;
                    if (vehicle == DBClassificationConstantes.Vehicles.names.press || vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                        dayOfWeek = new DateTime(int.Parse(_dayOfWeek.ToString().Substring(0, 4)), int.Parse(_dayOfWeek.Substring(4, 2)), int.Parse(_dayOfWeek.ToString().Substring(6, 2)));
                        zoomDetailSelectionWebControl.ZoomDate = WebFunctions.Dates.dateToString(dayOfWeek.Date, _customerWebSession.SiteLanguage);
                    }
                    else {
                        zoomDetailSelectionWebControl.ZoomDate = string.Empty;
                    }
                }
                else
                    zoomDetailSelectionWebControl.ShowZoom = false;
                #endregion

                zoomDetailSelectionWebControl.OutputType = _renderType;
                zoomDetailSelectionWebControl.CssLevel1 = CssDetailSelectionL1;
                zoomDetailSelectionWebControl.CssLevel2 = CssDetailSelectionL2;
                zoomDetailSelectionWebControl.CssLevel3 = CssDetailSelectionL3;
                zoomDetailSelectionWebControl.CssRightBorderLevel1 = CssDetailSelectionRightBorderL1;
                zoomDetailSelectionWebControl.CssRightBorderLevel2 = CssDetailSelectionRightBorderL2;
                zoomDetailSelectionWebControl.CssRightBorderLevel3 = CssDetailSelectionRightBorderL3;
                zoomDetailSelectionWebControl.CssRightBottomBorderLevel1 = CssDetailSelectionRightBottomBorderL1;
                zoomDetailSelectionWebControl.CssRightBottomBorderLevel2 = CssDetailSelectionRightBottomBorderL2;
                zoomDetailSelectionWebControl.CssRightBottomBorderLevel3 = CssDetailSelectionRightBottomBorderL3;
                zoomDetailSelectionWebControl.CssTitleGlobal = CssDetailSelectionTitleGlobal;
                zoomDetailSelectionWebControl.CssTitle = CssDetailSelectionTitle;
                zoomDetailSelectionWebControl.CssTitleData = CssDetailSelectionTitleData;
                zoomDetailSelectionWebControl.CssBorderLevel = CssDetailSelectionBordelLevel;
                return (zoomDetailSelectionWebControl.GetHeader());
            }
            catch (System.Exception err) {
                return (OnAjaxMethodError(err, this._customerWebSession));
            }
        
        }
        #endregion

    }
}
