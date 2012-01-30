﻿#region Informations
// Auteur: D. Mussuma
// Date de création: 12/12/2008
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Selections;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using DALClassif = TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Domain.Layers;

/// <summary>
///Page de sélection d'un unique support
/// </summary>
public partial class Private_Selection_Russia_PortofolioMediaSelection : TNS.AdExpress.Web.UI.SelectionWebPage {
	
	#region Variables
	/// <summary>
	/// TextBox : mot clé
	/// </summary>
	/// <summary>
	/// Evènement lancé
	/// </summary>
	public int eventButton = -1;
	/// <summary>
	/// Fermeture du Flash d'attente
	/// </summary>
	public string divClose = "";

	/// <summary>
	/// boutton ok
	/// </summary>
	/// <summary>
	/// Menu contextuel
	/// </summary>
	/// <summary>
	/// Liste des médias
	/// </summary>
	public string listMedia = "";

	#endregion

	#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
	public Private_Selection_Russia_PortofolioMediaSelection()
		: base() {
		}
	#endregion


		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try {

				VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

				#region Suppression des espaces
				keyWordTextBox.Text = keyWordTextBox.Text.TrimEnd();
				keyWordTextBox.Text = keyWordTextBox.Text.TrimStart();
				#endregion

				#region Textes et langage du site				
				ModuleTitleWebControl2.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				GenericDetailVehicleSelectionWebControl1.CustomerWebSession = _webSession;
				#endregion

				#region Evènements Boutons
				// Bouton valider
				if (Request.Form.Get("__EVENTTARGET") == "validImageButtonRollOverWebControl" || Request.Form.Get("__EVENTTARGET") == "MenuWebControl2") {
					eventButton = constEvent.eventSelection.VALID_EVENT;
				}
				// Bouton ok
				else if (Request.Form.Get("__EVENTTARGET") == "OkImageButtonRollOverWebControl") {
					eventButton = constEvent.eventSelection.OK_EVENT;
				}
				else if (Request.Form.Get("__EVENTTARGET") == "initializeButton") {
					eventButton = constEvent.eventSelection.INITIALIZE_EVENT;

					// Bouton ok Option
				}
				else if (Request.Form.Get("__EVENTTARGET") == "okImageButton") {

					if (Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2") != null) {
						foreach (DetailLevelItemInformation.Levels currentLevel in vehicleInformation.MediaSelectionParentsItemsEnumList)
							if (Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString() == "MediaSeller_" + currentLevel.GetHashCode())
								_webSession.MediaSelectionParent = currentLevel;
					}

					eventButton = constEvent.eventSelection.OK_OPTION_MEDIA_EVENT;
				}
				//Bouton ok (selection generique)
				else if (Request.Form.Get("__EVENTTARGET") == "okDetailMediaImageButton") {
					eventButton = constEvent.eventSelection.OK_OPTION_MEDIA_EVENT;
					_webSession.MediaSelectionParent = vehicleInformation.DefaultMediaSelectionParent;
				}
				else
					_webSession.MediaSelectionParent = vehicleInformation.DefaultMediaSelectionParent;
				#endregion

				#region Gestion Entrer
				//Gestion de l'évènements entrer sur la textBox
				keyWordTextBox.Attributes.Add("onkeydown", "javascript:trapEnter();");
				#endregion

				#region Focus
				this.setFocus(keyWordTextBox);
				#endregion

				#region Remplace les caractères accuentués du mot clé
				// Remplace les caractères accuentués
				keyWordTextBox.Text = TNS.AdExpress.Web.Functions.CheckedText.NewText(keyWordTextBox.Text);
				#endregion

				#region Liste des Médias
				if (!IsPostBack || eventButton == constEvent.eventSelection.INITIALIZE_EVENT) {
					GenericDetailVehicleSelectionWebControl1.KeyWord = "";
				}
				else if (eventButton == constEvent.eventSelection.OK_EVENT) {
					GenericDetailVehicleSelectionWebControl1.KeyWord = keyWordTextBox.Text;

				}
				else if (eventButton == constEvent.eventSelection.OK_OPTION_MEDIA_EVENT) {
					GenericDetailVehicleSelectionWebControl1.KeyWord = keyWordTextBox.Text;

				}
				else
					GenericDetailVehicleSelectionWebControl1.KeyWord = keyWordTextBox.Text;

				#endregion

				#region Boutons RollOver				
				if (PortofolioDetailVehicleSelectionWebControl.IsEmptyListPortofolio == true) {
					validImageButtonRollOverWebControl.Visible = false;
				}
				else
					validImageButtonRollOverWebControl.Visible = true;

				// Commentaire lorsque la souris pointe sur les boutons
				initializeButton.ToolTip = GestionWeb.GetWebWord(974, _webSession.SiteLanguage);
				#endregion

				#region Script
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				// Evènement lorsqu'on clique sur Entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "trapEnter", TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
				}
				// fermer/ouvrir tous les calques
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ExpandColapseAllDivs", TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
				}
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
				}

				#endregion

				_webSession.Save();

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {

			_webSession.Source.Close();
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation de la page de la page
		/// </summary>
		/// <param name="e">Arguments</param>
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
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();	
		    try{
			MenuWebControl2.CustomerWebSession = _webSession;
			_genericMediaDetailSelectionWebControl.CustomerWebSession = _webSession;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
			return tmp;

		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			try {

				if (this.IsPostBack) {
					if (_nextUrlOk) {
						this.validImageButtonRollOverWebControl_Click(null, null);
					}
				}
			}
			catch (System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
				}
			}
		}
		#endregion

		#region Valider
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void validImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {

			try {

				#region Variables
				string[] tabParent = null;
				Int64 valueMedia = -1;
				string nameMedia = "";
				System.Windows.Forms.TreeNode mediaTree = new System.Windows.Forms.TreeNode();
				System.Windows.Forms.TreeNode tmpNode = null;
				#endregion

				#region Parcours du form
				//Parcours des éléments dans form et récupère la valeur dans le champ hidden
				foreach (string currentKey in Request.Form.AllKeys) {
					tabParent = currentKey.Split('_');
					if (tabParent[0] == "CKB") {
						valueMedia = Int64.Parse(tabParent[1]);
					}
				}
				#endregion

				if (valueMedia != -1) {

					#region Création de l'arbre
					// Création de l'arbre que l'on place dans RéférenceUniversMédia
                    CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelListRussia];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                    object[] param = new object[2];
                    param[0] = _webSession.CustomerDataFilters.DataSource;
                    param[1] = _webSession.DataLanguage;
                    TNS.AdExpressI.Classification.DAL.Russia.ClassificationLevelListDALRussia levels = new TNS.AdExpressI.Classification.DAL.Russia.ClassificationLevelListDALRussia(TNS.AdExpress.Constantes.Classification.DB.Table.name.media.ToString(), valueMedia.ToString(), _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
 
					//DALClassif.MediaBranch.PartialMediaListDataAccess mediaLabelList = new DALClassif.MediaBranch.PartialMediaListDataAccess(valueMedia.ToString(), _webSession.DataLanguage, _webSession.Source);
                    if (levels != null) nameMedia = levels[valueMedia];
					tmpNode = new System.Windows.Forms.TreeNode(nameMedia);
					tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess, valueMedia, nameMedia);
					tmpNode.Checked = true;
					mediaTree.Nodes.Add(tmpNode);
					_webSession.ReferenceUniversMedia = mediaTree;
					_webSession.Save();
					#endregion

					_webSession.Source.Close();
					Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");

				}
				else {
					// Erreur : Il faut sélectionner au moins un élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\"" + GestionWeb.GetWebWord(1052, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			catch (System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
				}
			}
		}
		#endregion

		#region Fonction
		/// <summary>
		/// Place le focus sur le contrôle sélectionné
		/// </summary>
		/// <param name="champ">Champ sur lequel on veut mettre le focus</param>
		public void setFocus(System.Web.UI.WebControls.WebControl champ) {
			//Champ est le webcontrol sur lequel on veut mettre le focus
			if (!Page.ClientScript.IsClientScriptBlockRegistered("focus")) {
				string s;
				s = "<script language= \"javascript\">document.getElementById('" +
					champ.ClientID + "').focus()</script>";

				Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
			}
		}

		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Validation de la sélection de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e) {
			this.validImageButtonRollOverWebControl_Click(sender, e);
		}

		/// <summary>
		/// Obtient l'url suivante
		/// </summary>
		/// <returns>Url suivante</returns>
		protected override string GetNextUrlFromMenu() {
			return (this.MenuWebControl2.NextUrl);
		}
		#endregion
}
