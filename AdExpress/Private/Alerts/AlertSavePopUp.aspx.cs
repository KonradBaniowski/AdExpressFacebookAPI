#region Informations
// Auteur: D. Mussuma
// Date de création: 02/02/2007
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.DataAccess.Classification.ProductBranch;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFunctions = TNS.AdExpress.Web.Functions;
using FrameworkDB = TNS.FrameWork.DB.Common;
using TNS.Ares.Alerts;
using TNS.Ares.Domain.LS;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Alerts
{
	/// <summary>
	/// Page de demande d'export de résultat au format PDF
	/// </summary>
	public partial class AlertSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage { 

		#region Variables

		#region variables fiche justificatives
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct = null;
		/// <summary>
		/// Date faciale
		/// </summary>
		private string _dateCover = null;
		/// <summary>
		/// Date parution
		/// </summary>
		private string _dateParution = null;
		/// <summary>
		/// Page
		/// </summary>
		private string _pageNumber = null;
        /// <summary>
        /// Is Quota Full
        /// </summary>
        private bool _isQuotaFull;
		#endregion

        #region Variables Plan média
        /// <summary>
        /// Zoom parameters in Media Plan
        /// </summary>
        string zoomDate = string.Empty;
        #endregion

        #endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
        public AlertSavePopUp()
            : base()
        {			
	}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>		
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{
                if (_isQuotaFull) return;
                this.ddlPeriodicityType.Attributes.Add("onchange", "onPeriodicityChanged(this);");
                string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

				#region Paramètres pour les fiches justificatives
				//Récupération des paramètres de l'url
				if(_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.JUSTIFICATIFS_PRESSE ){
					_idMedia	= Page.Request.QueryString.Get("idmedia");
					_idProduct	= Page.Request.QueryString.Get("idproduct");
					_dateCover = Page.Request.QueryString.Get("dateCover");
					_dateParution = Page.Request.QueryString.Get("dateParution");
					_pageNumber		= Page.Request.QueryString.Get("page");
				}
				#endregion

                #region Paramètres Plans média
                zoomDate = Page.Request.QueryString.Get("zoomDate");
                #endregion

                #region Initialisation de la liste
                if (this.ddlPeriodicityType.Items.Count == 0)
                {
                    // Il faudra vérifier si chaque type d'alerte est
                    // disponible pour le pays ou le site est déployé
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(2579, _webSession.SiteLanguage), TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Daily.GetHashCode().ToString()));
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(2580, _webSession.SiteLanguage), TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Weekly.GetHashCode().ToString()));
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(1294, _webSession.SiteLanguage), TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Monthly.GetHashCode().ToString()));
                }
                #endregion

                #region Initialisation images path for calendar
                string baseImgPath = "/App_Themes/" + themeName + @"/Images/Culture/AlertCalendar/";

                this.lnkMonday.ImageUrl     = baseImgPath + "DayLab_1.gif";
                this.lnkTuesday.ImageUrl    = baseImgPath + "DayLab_2.gif";
                this.lnkWednesday.ImageUrl  = baseImgPath + "DayLab_3.gif";
                this.lnkThursday.ImageUrl   = baseImgPath + "DayLab_4.gif";
                this.lnkFriday.ImageUrl     = baseImgPath + "DayLab_5.gif";
                this.lnkSaturday.ImageUrl   = baseImgPath + "DayLab_6.gif";
                this.lnkSunday.ImageUrl     = baseImgPath + "DayLab_7.gif";

                this.lnkMonday.Attributes.Add("onclick", "onPeriodicityParameterClicked(1, this, '" + baseImgPath + "');");
                this.lnkTuesday.Attributes.Add("onclick", "onPeriodicityParameterClicked(2, this, '" + baseImgPath + "');");
                this.lnkWednesday.Attributes.Add("onclick", "onPeriodicityParameterClicked(3, this, '" + baseImgPath + "');");
                this.lnkThursday.Attributes.Add("onclick", "onPeriodicityParameterClicked(4, this, '" + baseImgPath + "');");
                this.lnkFriday.Attributes.Add("onclick", "onPeriodicityParameterClicked(5, this, '" + baseImgPath + "');");
                this.lnkSaturday.Attributes.Add("onclick", "onPeriodicityParameterClicked(6, this, '" + baseImgPath + "');");
                this.lnkSunday.Attributes.Add("onclick", "onPeriodicityParameterClicked(7, this, '" + baseImgPath + "');");

                this.lblIntroWeekly.Text = GestionWeb.GetWebWord(2582, _webSession.SiteLanguage);
                this.lblIntroMonthly.Text = GestionWeb.GetWebWord(2583, _webSession.SiteLanguage);

                this.day1.ImageUrl = baseImgPath + "1.gif";
                this.day2.ImageUrl = baseImgPath + "2.gif";
                this.day3.ImageUrl = baseImgPath + "3.gif";
                this.day4.ImageUrl = baseImgPath + "4.gif";
                this.day5.ImageUrl = baseImgPath + "5.gif";
                this.day6.ImageUrl = baseImgPath + "6.gif";
                this.day7.ImageUrl = baseImgPath + "7.gif";
                this.day8.ImageUrl = baseImgPath + "8.gif";
                this.day9.ImageUrl = baseImgPath + "9.gif";
                this.day10.ImageUrl = baseImgPath + "10.gif";
                this.day11.ImageUrl = baseImgPath + "11.gif";
                this.day12.ImageUrl = baseImgPath + "12.gif";
                this.day13.ImageUrl = baseImgPath + "13.gif";
                this.day14.ImageUrl = baseImgPath + "14.gif";
                this.day15.ImageUrl = baseImgPath + "15.gif";
                this.day16.ImageUrl = baseImgPath + "16.gif";
                this.day17.ImageUrl = baseImgPath + "17.gif";
                this.day18.ImageUrl = baseImgPath + "18.gif";
                this.day19.ImageUrl = baseImgPath + "19.gif";
                this.day20.ImageUrl = baseImgPath + "20.gif";
                this.day21.ImageUrl = baseImgPath + "21.gif";
                this.day22.ImageUrl = baseImgPath + "22.gif";
                this.day23.ImageUrl = baseImgPath + "23.gif";
                this.day24.ImageUrl = baseImgPath + "24.gif";
                this.day25.ImageUrl = baseImgPath + "25.gif";
                this.day26.ImageUrl = baseImgPath + "26.gif";
                this.day27.ImageUrl = baseImgPath + "27.gif";
                this.day28.ImageUrl = baseImgPath + "28.gif";
                this.day29.ImageUrl = baseImgPath + "29.gif";
                this.day30.ImageUrl = baseImgPath + "30.gif";
                this.day31.ImageUrl = baseImgPath + "31.gif";

                this.day1.Attributes.Add("onclick", "onPeriodicityParameterClicked(1, this, '" + baseImgPath + "');");
                this.day2.Attributes.Add("onclick", "onPeriodicityParameterClicked(2, this, '" + baseImgPath + "');");
                this.day3.Attributes.Add("onclick", "onPeriodicityParameterClicked(3, this, '" + baseImgPath + "');");
                this.day4.Attributes.Add("onclick", "onPeriodicityParameterClicked(4, this, '" + baseImgPath + "');");
                this.day5.Attributes.Add("onclick", "onPeriodicityParameterClicked(5, this, '" + baseImgPath + "');");
                this.day6.Attributes.Add("onclick", "onPeriodicityParameterClicked(6, this, '" + baseImgPath + "');");
                this.day7.Attributes.Add("onclick", "onPeriodicityParameterClicked(7, this, '" + baseImgPath + "');");
                this.day8.Attributes.Add("onclick", "onPeriodicityParameterClicked(8, this, '" + baseImgPath + "');");
                this.day9.Attributes.Add("onclick", "onPeriodicityParameterClicked(9, this, '" + baseImgPath + "');");
                this.day10.Attributes.Add("onclick", "onPeriodicityParameterClicked(10, this, '" + baseImgPath + "');");
                this.day11.Attributes.Add("onclick", "onPeriodicityParameterClicked(11, this, '" + baseImgPath + "');");
                this.day12.Attributes.Add("onclick", "onPeriodicityParameterClicked(12, this, '" + baseImgPath + "');");
                this.day13.Attributes.Add("onclick", "onPeriodicityParameterClicked(13, this, '" + baseImgPath + "');");
                this.day14.Attributes.Add("onclick", "onPeriodicityParameterClicked(14, this, '" + baseImgPath + "');");
                this.day15.Attributes.Add("onclick", "onPeriodicityParameterClicked(15, this, '" + baseImgPath + "');");
                this.day16.Attributes.Add("onclick", "onPeriodicityParameterClicked(16, this, '" + baseImgPath + "');");
                this.day17.Attributes.Add("onclick", "onPeriodicityParameterClicked(17, this, '" + baseImgPath + "');");
                this.day18.Attributes.Add("onclick", "onPeriodicityParameterClicked(18, this, '" + baseImgPath + "');");
                this.day19.Attributes.Add("onclick", "onPeriodicityParameterClicked(19, this, '" + baseImgPath + "');");
                this.day20.Attributes.Add("onclick", "onPeriodicityParameterClicked(20, this, '" + baseImgPath + "');");
                this.day21.Attributes.Add("onclick", "onPeriodicityParameterClicked(21, this, '" + baseImgPath + "');");
                this.day22.Attributes.Add("onclick", "onPeriodicityParameterClicked(22, this, '" + baseImgPath + "');");
                this.day23.Attributes.Add("onclick", "onPeriodicityParameterClicked(23, this, '" + baseImgPath + "');");
                this.day24.Attributes.Add("onclick", "onPeriodicityParameterClicked(24, this, '" + baseImgPath + "');");
                this.day25.Attributes.Add("onclick", "onPeriodicityParameterClicked(25, this, '" + baseImgPath + "');");
                this.day26.Attributes.Add("onclick", "onPeriodicityParameterClicked(26, this, '" + baseImgPath + "');");
                this.day27.Attributes.Add("onclick", "onPeriodicityParameterClicked(27, this, '" + baseImgPath + "');");
                this.day28.Attributes.Add("onclick", "onPeriodicityParameterClicked(28, this, '" + baseImgPath + "');");
                this.day29.Attributes.Add("onclick", "onPeriodicityParameterClicked(29, this, '" + baseImgPath + "');");
                this.day30.Attributes.Add("onclick", "onPeriodicityParameterClicked(30, this, '" + baseImgPath + "');");
                this.day31.Attributes.Add("onclick", "onPeriodicityParameterClicked(31, this, '" + baseImgPath + "');");
                #endregion

                #region Gestion des cookies

                #region Cookies enregistrement des préférences

                //Vérifie si le navigateur accepte les cookies
				if(Request.Browser.Cookies){

                    WebFunctions.Cookies.ManageEmailListCookie(this.Page, false, this.tbxMail.Text);

					cbxRegisterMail.Text = GestionWeb.GetWebWord(2117,_webSession.SiteLanguage);
					
					HttpCookie isRegisterEmailForRemotingExport = null, savedEmailForRemotingExport = null;
					cbxRegisterMail.Visible = true; //RegisterMailLabel.Visible = true;

					if(!Page.IsPostBack){
						WebFunctions.Cookies.LoadSavedEmailForRemotingExport(Page,isRegisterEmailForRemotingExport, savedEmailForRemotingExport,cbxRegisterMail,tbxMail);
					}
				}else cbxRegisterMail.Visible = false; // = RegisterMailLabel.Visible = false;

				#endregion

				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		
		}
		#endregion

        #region Button Close
        /// <summary>
		/// Femeture de la fenêtre
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void closeRollOverWebControl_Click(object sender, System.EventArgs e) {
			this.ClientScript.RegisterClientScriptBlock(this.GetType(),"closeScript",WebFunctions.Script.CloseScript());
        }
        #endregion

        #region Button Validate
        /// <summary>
		/// Lancer une génération
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void validateRollOverWebControl_Click(object sender, System.EventArgs e) {
			string fileName=tbxFileName.Text;
			string mail=tbxMail.Text;
            try
            {
				if(fileName==null || mail==null || fileName.Length==0 || mail.Length==0) {
					this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(1748,_siteLanguage)));
                    this.ddlPeriodicityType.SelectedIndex = 0;
                }
				else if (!WebFunctions.CheckedText.CheckedMailText(mail.Split(";".ToCharArray()))) {
					this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(2041,_siteLanguage)));
                    this.ddlPeriodicityType.SelectedIndex = 0;
                } 
                else if((TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity)Enum.Parse(typeof(TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity), this.ddlPeriodicityType.SelectedValue)==TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Weekly && !(int.Parse(this.hiddenPeriodicityValue.Value)>0 && int.Parse(this.hiddenPeriodicityValue.Value)<8)){
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", WebFunctions.Script.Alert(GestionWeb.GetWebWord(2645, _siteLanguage)));
                    this.ddlPeriodicityType.SelectedIndex = 0;
                }
                else if ((TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity)Enum.Parse(typeof(TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity), this.ddlPeriodicityType.SelectedValue) == TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Monthly && !(int.Parse(this.hiddenPeriodicityValue.Value) > 0 && int.Parse(this.hiddenPeriodicityValue.Value) < 32)) {
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", WebFunctions.Script.Alert(GestionWeb.GetWebWord(2645, _siteLanguage)));
                    this.ddlPeriodicityType.SelectedIndex = 0;
                }
				else {

					#region Gestion des cookies
					
					#region Cookies enregistrement des préférences
				
					//Vérifie si le navigateur accepte les cookies
					if(Request.Browser.Cookies){						
						 WebFunctions.Cookies.SaveEmailForRemotingExport(Page,mail,cbxRegisterMail);						
					}
					#endregion

					#endregion

					_webSession.ExportedPDFFileName = fileName;
					string[] mails=new string[1];
					mails[0]=mail;
					_webSession.EmailRecipient=mails;

                    DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                    FrameworkDB.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                    IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
                    if (alertDAL.GetAlerts(_webSession.CustomerLogin.IdLogin).Count >= _webSession.CustomerLogin.GetNbAlertsAdExpress()) {
                        Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(2615, _siteLanguage)));
                        return;
                    }
                    else {
                        Int64 idAlertSchedule = -1;
                        // Loading alerts and binding them
                        TNS.Alert.Domain.AlertHourCollection alertHourCollection = alertDAL.GetAlertHours();
                        for (int i = 0; i < alertHourCollection.Count; i++) {
                            if (alertHourCollection[i].HoursSchedule.Ticks == (new TimeSpan(18, 0, 0)).Ticks) {
                                idAlertSchedule = alertHourCollection[i].IdAlertSchedule;
                            }
                        }

                        alertDAL.InsertAlertData(this.tbxFileName.Text, _webSession.ToBinaryData(), _webSession.CurrentModule,
                                                 (TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity)Enum.Parse(typeof(TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity), this.ddlPeriodicityType.SelectedValue),
                                                 int.Parse(this.hiddenPeriodicityValue.Value), this.tbxMail.Text, _webSession.CustomerLogin.IdLogin, idAlertSchedule);
                        Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(2660, _webSession.SiteLanguage)));
                    }
					closeRollOverWebControl_Click(this, null);
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}

        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
            FrameworkDB.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
            IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);

            if (alertDAL.GetAlerts(_webSession.CustomerLogin.IdLogin).Count >= _webSession.CustomerLogin.GetNbAlertsAdExpress()) {
                Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(2615, _siteLanguage)));
                _isQuotaFull = true;
                return;
            }
            else {
                _isQuotaFull = false;
            }
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
           
		}
		#endregion

        #region Identifiant des éléments de la nomenclature produit
        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        private void SetProduct(int id, int level)
        {
            System.Windows.Forms.TreeNode tree = new System.Windows.Forms.TreeNode();
            switch ((DetailLevelItemInformation.Levels)_webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level))
            {
                case DetailLevelItemInformation.Levels.sector:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id, (new PartialSectorLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.sector, tree);
                    break;
                case DetailLevelItemInformation.Levels.subSector:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id, (new PartialSubSectorLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subsector, tree);
                    break;
                case DetailLevelItemInformation.Levels.group:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id, (new PartialGroupLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.group, tree);
                    break;
                case DetailLevelItemInformation.Levels.segment:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id, (new PartialSegmentLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.segment, tree);
                    break;
                case DetailLevelItemInformation.Levels.product:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id, (new PartialProductLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product, tree);
                    break;
                case DetailLevelItemInformation.Levels.advertiser:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id, (new PartialAdvertiserLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.advertiser, tree);
                    break;
                case DetailLevelItemInformation.Levels.brand:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id, (new PartialBrandLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.brand, tree);
                    break;
                case DetailLevelItemInformation.Levels.holdingCompany:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id, (new PartialHoldingCompanyLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.holding_company, tree);
                    break;
            }
        }
        #endregion

    }
}

