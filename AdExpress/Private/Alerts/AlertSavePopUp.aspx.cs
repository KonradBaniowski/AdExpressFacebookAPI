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
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFunctions = TNS.AdExpress.Web.Functions;
using FrameworkDB = TNS.FrameWork.DB.Common;
using TNS.AdExpress.Alerts;

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

                this.ddlPeriodicityType.Attributes.Add("onchange", "onPeriodicityChanged(this);");

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
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(2579, _webSession.SiteLanguage), "10"));
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(2580, _webSession.SiteLanguage), "20"));
                    this.ddlPeriodicityType.Items.Add(new ListItem(GestionWeb.GetWebWord(1294, _webSession.SiteLanguage), "30"));
                }
                #endregion

                #region Initialisation Periodicity Type Weekly
                this.lnkMonday.Text = GestionWeb.GetWebWord(1554, _webSession.SiteLanguage);
                this.lnkMonday.Attributes.Add("onclick", "onPeriodicityParameterClicked(1, this);");
                this.lnkTuesday.Text = GestionWeb.GetWebWord(1555, _webSession.SiteLanguage);
                this.lnkTuesday.Attributes.Add("onclick", "onPeriodicityParameterClicked(2, this);");
                this.lnkWednesday.Text = GestionWeb.GetWebWord(1556, _webSession.SiteLanguage);
                this.lnkWednesday.Attributes.Add("onclick", "onPeriodicityParameterClicked(3, this);");
                this.lnkThursday.Text = GestionWeb.GetWebWord(1557, _webSession.SiteLanguage);
                this.lnkThursday.Attributes.Add("onclick", "onPeriodicityParameterClicked(4, this);");
                this.lnkFriday.Text = GestionWeb.GetWebWord(1558, _webSession.SiteLanguage);
                this.lnkFriday.Attributes.Add("onclick", "onPeriodicityParameterClicked(5, this);");
                this.lnkSaturday.Text = GestionWeb.GetWebWord(1559, _webSession.SiteLanguage);
                this.lnkSaturday.Attributes.Add("onclick", "onPeriodicityParameterClicked(6, this);");
                this.lnkSunday.Text = GestionWeb.GetWebWord(1560, _webSession.SiteLanguage);
                this.lnkSunday.Attributes.Add("onclick", "onPeriodicityParameterClicked(7, this);");

                this.lblIntroWeekly.Text = GestionWeb.GetWebWord(2582, _webSession.SiteLanguage);
                this.lblIntroMonthly.Text = GestionWeb.GetWebWord(2583, _webSession.SiteLanguage);
                #endregion

                #region Gestion des cookies

                #region Cookies enregistrement des préférences

                //Vérifie si le navigateur accepte les cookies
				if(Request.Browser.Cookies){

                    WebFunctions.Cookies.ManageEmailListCookie(this.Page, false, this.tbxMail.Text);

					cbxRegisterMail.Text = GestionWeb.GetWebWord(2117,_webSession.SiteLanguage);
					cbxRegisterMail.CssClass = "txtViolet11Bold";
					
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

		/// <summary>
		/// Femeture de la fenêtre
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void closeRollOverWebControl_Click(object sender, System.EventArgs e) {
			this.ClientScript.RegisterClientScriptBlock(this.GetType(),"closeScript",WebFunctions.Script.CloseScript());
		}

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
					Int64 idStaticNavSession = 0;

                    DataAccessLayer layer = NyxConfiguration.GetDataAccessLayer(NyxDataAccessLayer.Alert);
                    FrameworkDB.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                    IAlertDAL alertDAL = (TNS.AdExpress.Alerts.IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null, null);

                    alertDAL.InsertAlertData(this.tbxFileName.Text, _webSession,
                                             (TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity)Enum.Parse(typeof(TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity), this.ddlPeriodicityType.SelectedValue),
                                             int.Parse(this.hiddenPeriodicityValue.Value), this.tbxMail.Text, _webSession.CustomerLogin.IdLogin);

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

