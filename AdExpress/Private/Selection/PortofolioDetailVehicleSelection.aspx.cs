#region Informations
// Auteur: A. Obermeyer
// Date de création: 255/11/2004
// Date de modification: 
// 31/12/2004 A. Obermeyer Intégration de WebPage
// Date de modification: 
// 20/05/2005 A. Dadouch:Intégration de la sélection par régie;centre d'interet
// Date de modification: 07/06/2005 
// K. Shehzad:option open/close all
//	01/08/2006 Modification FindNextUrl
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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Selections;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Selection{
	/// <summary>
	///Page de sélection d'un unique support
	/// </summary>
	public partial class PortofolioDetailVehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Varaibles MMI
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Bouton OK
		/// </summary>
		/// <summary>
		/// controle : titre du module
		/// </summary>
		/// <summary>
		/// Contrôle : En-tête
		/// </summary>
		/// <summary>
		/// Bouton Annuler la recherche en cours
		/// </summary>
		/// <summary>
		/// Controle liste radio des supports
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Selections.PortofolioDetailVehicleSelectionWebControl AdvertiserSelectionWebControl1;
		/// <summary>
		/// text : Mot clé
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// TextBox : mot clé
		/// </summary>
		/// <summary>
		/// Evènement lancé
		/// </summary>
		public int eventButton=-1;
		/// <summary>
		/// Fermeture du Flash d'attente
		/// </summary>
		public string divClose="";
		/// <summary>
		/// A1
		/// </summary>
		/// <summary>
		/// web control régie
		/// </summary>
		/// <summary>
		/// list régie
		/// </summary>
		protected System.Web.UI.WebControls.RadioButtonList MediaSellerRadioList;
		/// <summary>
		/// boutton ok
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Liste des médias
		/// </summary>
		public string listMedia="";
		

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioDetailVehicleSelection():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
		
				#region Suppression des espaces
				keyWordTextBox.Text=keyWordTextBox.Text.TrimEnd();
				keyWordTextBox.Text=keyWordTextBox.Text.TrimStart();
				#endregion	

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
                for (int i = 0; i < this.Controls.Count; i++) {
                    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
				ModuleTitleWebControl2.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion						

				#region Evènements Boutons
				//_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia;
				// Bouton valider
				if(Request.Form.Get("__EVENTTARGET")=="validImageButtonRollOverWebControl" || Request.Form.Get("__EVENTTARGET")=="MenuWebControl2"){
					eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Bouton ok
				else if(Request.Form.Get("__EVENTTARGET")=="OkImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.OK_EVENT;
				}			
				else if(Request.Form.Get("__EVENTTARGET")=="initializeButton"){
					eventButton=constEvent.eventSelection.INITIALIZE_EVENT;

					// Bouton ok Option
				}else if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){
					if( Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")!=null &&
						Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia.GetHashCode().ToString()){
						_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia;																	
					}
					else if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")!=null && Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia.GetHashCode().ToString()){
						_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia;																		
					}
					else if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")!=null && Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia.GetHashCode().ToString()){
						_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia;																		
					}
					else if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")!=null && Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia.GetHashCode().ToString()){
						_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia;																		
					}
					eventButton=constEvent.eventSelection.OK_OPTION_MEDIA_EVENT;
				}
				else
					_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia;
				#endregion

				#region Gestion Entrer
				//Gestion de l'évènements entrer sur la textBox
				keyWordTextBox.Attributes.Add("onkeydown","javascript:trapEnter();");
				#endregion

				#region Focus
				this.setFocus(keyWordTextBox);
				#endregion

				#region Remplace les caractères accuentués du mot clé
				// Remplace les caractères accuentués
				keyWordTextBox.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(keyWordTextBox.Text);
				#endregion

				#region Liste des Médias
				if(!IsPostBack || eventButton==constEvent.eventSelection.INITIALIZE_EVENT){
					listMedia=TNS.AdExpress.Web.Controls.Selections.PortofolioDetailVehicleSelectionWebControl.listMedia(_webSession,"");
				}
				else if(eventButton==constEvent.eventSelection.OK_EVENT){
					listMedia=TNS.AdExpress.Web.Controls.Selections.PortofolioDetailVehicleSelectionWebControl.listMedia(_webSession,keyWordTextBox.Text);
				
				}else if(eventButton==constEvent.eventSelection.OK_OPTION_MEDIA_EVENT){
					listMedia=TNS.AdExpress.Web.Controls.Selections.PortofolioDetailVehicleSelectionWebControl.listMedia(_webSession,keyWordTextBox.Text);
	
				}else
					listMedia=TNS.AdExpress.Web.Controls.Selections.PortofolioDetailVehicleSelectionWebControl.listMedia(_webSession,keyWordTextBox.Text);
					
				#endregion		

				#region Boutons RollOver
				//if (PortofolioDetailVehicleSelectionWebControl.isEmptyList==false) {
					
				//validImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				if (PortofolioDetailVehicleSelectionWebControl.IsEmptyListPortofolio==true) {
					validImageButtonRollOverWebControl.Visible=false;
				}else
					validImageButtonRollOverWebControl.Visible=true;
				
				// Commentaire lorsque la souris pointe sur les boutons
				initializeButton.ToolTip=GestionWeb.GetWebWord(974,_webSession.SiteLanguage);			
				#endregion	

				#region Script
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				// Evènement lorsqu'on clique sur Entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
				}
				// fermer/ouvrir tous les calques
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
				}
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
				}

				#endregion

				_webSession.Save();
				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					//this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
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
		protected void Page_UnLoad(object sender, System.EventArgs e){

            _webSession.Source.Close();
		}
		#endregion
		
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation de la page de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
            this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//RecallWebControl1.CustomerWebSession=_webSession;
			MediaSellerWebControl2.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;

		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {		
			try{

				if (this.IsPostBack){
					if(_nextUrlOk){
						this.validImageButtonRollOverWebControl_Click(null,null);
					}
				}	
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
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
			
			try{

				#region Variables
				string[] tabParent=null;
				Int64 valueMedia=-1;
				string nameMedia="";
				System.Windows.Forms.TreeNode mediaTree=new System.Windows.Forms.TreeNode();
				System.Windows.Forms.TreeNode tmpNode=null;
				#endregion
			
				#region Parcours du form
				//Parcours des éléments dans form et récupère la valeur dans le champ hidden
				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB") {
						valueMedia=Int64.Parse(tabParent[1]);	
						nameMedia=tabParent[2];
					}
				}
				#endregion			
			
				if(valueMedia!=-1){
			
					#region Création de l'arbre
					// Création de l'arbre que l'on place dans RéférenceUniversMédia
					tmpNode=new System.Windows.Forms.TreeNode(tabParent[2]);						
					tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,valueMedia,nameMedia);
					tmpNode.Checked=true;
					mediaTree.Nodes.Add(tmpNode);
					_webSession.ReferenceUniversMedia=mediaTree;
					_webSession.Save();
					#endregion
	
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				
				}else{
					// Erreur : Il faut sélectionner au moins un élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Fonction
		/// <summary>
		/// Place le focus sur le contrôle sélectionné
		/// </summary>
		/// <param name="champ">Champ sur lequel on veut mettre le focus</param>
		public  void setFocus(System.Web.UI.WebControls.WebControl champ){
			//Champ est le webcontrol sur lequel on veut mettre le focus
			if(!Page.ClientScript.IsClientScriptBlockRegistered("focus")){
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
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.validImageButtonRollOverWebControl_Click(sender,e);
		}

		/// <summary>
		/// Obtient l'url suivante
		/// </summary>
		/// <returns>Url suivante</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion
	
	}
}
