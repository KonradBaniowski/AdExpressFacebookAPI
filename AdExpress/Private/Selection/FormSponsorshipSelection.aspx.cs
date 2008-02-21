#region Informations
// Auteur: Y. R'kaina
// Date de création: 28/11/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Rules.Customer;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;

namespace AdExpress.Private.Selection 
{
	/// <summary>
	/// Classe permettant l'affichage des genres d'émissions / émissions.
	/// </summary>
	public partial class FormSponsorshipSelection : TNS.AdExpress.Web.UI.SelectionWebPage 
	{
		#region Constantes
		/// <summary>
		/// L'Id de la sub section qui represente la page ProgramType
		/// </summary>
		protected const int PROGRAM_TYPE_ID = 7;
		#endregion
		
		#region Variables MMI
		/// <summary>
		/// Contrôle affichant le Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle En tête de page
		/// </summary>
		/// <summary>
		/// Texte : vos univers 
		/// </summary>
		/// <summary>
		/// Bouton chargement d'un univers
		/// </summary>
		/// <summary>
		/// Texte : Forme de parrainage
		/// </summary>
		/// <summary>
		/// Texte : Saississez votre mot-clé
		/// </summary>
		/// <summary>
		/// Texte box : Pour la saisie du forme de parrainage (mot-clé)
		/// </summary>
		/// <summary>
		/// Sauvegarde d'un univers
		/// </summary>
		/// <summary>
		/// Bouton valider 
		/// </summary>
		/// <summary>
		/// Controle redirect
		/// </summary>
		/// <summary>
		/// A1
		/// </summary>
		/// <summary>
		/// bouton ok
		/// </summary>
		/// <summary>
		/// Contrôle : liste des formes de parrainage
		/// </summary>
		/// <summary>
		/// contrôle Recall
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.RecallWebControl RecallWebControl1;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Bouton initialisation
		/// </summary>
		/// <summary>
		/// Contrôle aide
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HelpWebControl helpWebControl;
		/// <summary>
		/// Chargement d'un univers
		/// </summary>
		/// <summary>
		/// Bouton genre d'émission
		/// </summary>
		/// <summary>
		/// Main Menu
		/// </summary>
		/// <summary>
		/// Contrôle information
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// fermeture du flash d'attente
		/// </summary>
		public string divClose="";
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniverses="";
		/// <summary>
		/// Type d'évènement (bouton ok, valider...)
		/// </summary>
		protected int eventButton;	
		/// <summary>
		/// script ouvrant la popup permettant d'enregistrer un univers
		/// </summary>
		public string saveScript;
		/// <summary>
		/// Identifiant de la session
		/// </summary>
		public string sessionId="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public FormSponsorshipSelection():base() {
		}
		#endregion

		#region Evénements

		#region Chargement de la page
		/// <summary>
		/// Load
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try {
				sessionId=_webSession.IdSession;
				
				#region Flash pour patienter
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null) {

					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=validateButton.ID && nomInput!=MenuWebControl2.ID && nomInput!=programTypeImageButtonRollOverWebControl.ID) {
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
						divClose=LoadingSystem.GetHtmlCloseDiv();					
					}
				}
				#endregion			
			
				#region Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				#endregion
						 
				#region Evènemment
				// Connaître le boutton qui a été cliqué 
				eventButton=0;
				if(HttpContext.Current.Request.QueryString.Get("saveUnivers")!= null) {

					eventButton=int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));
				}
				// Boutton valider
				if(Request.Form.Get("__EVENTTARGET")=="validateButton") {

					eventButton=1;
					//eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Boutton ok
				else if(Request.Form.Get("__EVENTTARGET")=="okButtonRollOverWebControl2") {

					eventButton=2;
					//eventButton=constEvent.eventSelection.OK_EVENT;
				}
					// Boutton sauvegarder
				else if(Request.Form.Get("__EVENTTARGET")=="saveUniverseImageButtonRollOverWebControl") {

					eventButton=3;
					//eventButton=constEvent.eventSelection.SAVE_EVENT;
				}
					// Boutton Charger
				else if(Request.Form.Get("__EVENTTARGET")=="loadImageButtonRollOverWebControl") {

					eventButton=4;
					//eventButton=constEvent.eventSelection.LOAD_EVENT;
				}
					// Controle Recall
				else if(Request.Form.Get("__EVENTTARGET")=="MenuWebControl2") {

					eventButton=5;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="programTypeImageButtonRollOverWebControl") {

					eventButton=6;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="initializeImageButtonRollOverWebControl1") {

					eventButton=-1;
					//eventButton=constEvent.eventSelection.INITIALIZE_EVENT;
				}

				#endregion

				#region Chargement Univers
				// Bouton Charger 
				if(eventButton==4) {
					//if(eventButton==constEvent.eventSelection.LOAD_EVENT) {
					if(!loadUniversAdvertiser()) {

						eventButton=-1;
						//eventButton=constEvent.eventSelection.INITIALIZE_EVENT;
						loadImageButtonRollOverWebControl_Click(null,null);
					}
				}
				#endregion
			
				#region Remplace les caractères accuentués
				textBoxProgramTypeChoice.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(textBoxProgramTypeChoice.Text);
				#endregion

				#region Définition du contrôle ProgramTypeWebControl1 
				AdvertiserSelectionWebControl1.WebSession=_webSession;
				AdvertiserSelectionWebControl1.EventButton=eventButton;	
				AdvertiserSelectionWebControl1.SponsorshipFormText=textBoxProgramTypeChoice.Text;	
				if(eventButton!=4) {
					//if((eventButton!=4)&&(eventButton!=constEvent.eventSelection.VALID_EVENT)&&(eventButton!=constEvent.eventSelection.SAVE_EVENT)&&(eventButton!=6)) {
					_webSession.SelectionUniversSponsorshipForm=new System.Windows.Forms.TreeNode("sponsorshipform");
				}
				#endregion
			
				#region Init ModuleTitleWebControl1 & InformationWebControl1
				ModuleTitleWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region Rollover des boutons
				//Button valider
				validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				
				//Button Ok
				okButtonRollOverWebControl2.ImageUrl="/Images/Common/button/ok_up.gif";
				okButtonRollOverWebControl2.RollOverImageUrl="/Images/Common/button/ok_down.gif";			

				//Button enregistrer
				saveUniverseImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				saveUniverseImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";

				//Button forme de parrainage
				programTypeImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/bt_emissions_up.gif";
				programTypeImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/bt_emissions_down.gif";			

				initializeImageButtonRollOverWebControl1.ToolTip=GestionWeb.GetWebWord(1358,_webSession.SiteLanguage);
				#endregion

				#region Gestion Entrer
				//Gestion de l'évènements entrer sur la textBox
				textBoxProgramTypeChoice.Attributes.Add("onkeydown","javascript:trapEnter();");
				#endregion

				#region Focus sur la checkbox 
				this.setFocus(textBoxProgramTypeChoice);		
				#endregion
			
				#region Script
				//Gestion de la sélection d'un radiobutton dans la liste des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"InsertIdMySession4",TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
				}
	
				// Gestion lorsqu'on clique sur entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("okButtonRollOverWebControl2"));
				}	
			
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
			
				// Sélection/désélection de tous les fils (cas 2 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				// Sélection de tous les fils
				if (!Page.ClientScript.IsClientScriptBlockRegistered("Integration")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Integration",TNS.AdExpress.Web.Functions.Script.Integration());
				}
				// Sélection de tous les éléments (cas 1 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllLevelSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllLevelSelection",TNS.AdExpress.Web.Functions.Script.AllLevelSelection());
				}			
				#endregion		
			
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//RecallWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.ListBranchType=TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm.GetHashCode().ToString();
			MenuWebControl2.CustomerWebSession=_webSession;
			return tmp;
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			try {

				#region Affichage des boutons si existance d'annonceurs
				if(IsPostBack) {
					if(AdvertiserSelectionWebControl1.DsListSponsorshipForm!=null) {
						if(AdvertiserSelectionWebControl1.DsListSponsorshipForm.Tables[0].Rows.Count==0) {
							saveUniverseImageButtonRollOverWebControl.Visible=false;
							programTypeImageButtonRollOverWebControl.Visible=false;
							validateButton.Visible=false;
						}
						else {
							saveUniverseImageButtonRollOverWebControl.Visible=true;
							programTypeImageButtonRollOverWebControl.Visible=true;
							validateButton.Visible=true;
						}
					}
					else {
						saveUniverseImageButtonRollOverWebControl.Visible=false;
						programTypeImageButtonRollOverWebControl.Visible=false;
						validateButton.Visible=false;				
					}
				}
				#endregion

				#region Bouton Charger
				if(eventButton==4 || eventButton==3) {
					//if(eventButton==4 || eventButton==constEvent.eventSelection.SAVE_EVENT) {
					saveUniverseImageButtonRollOverWebControl.Visible=true;
					programTypeImageButtonRollOverWebControl.Visible=true;
					validateButton.Visible=true;	
				}
				#endregion

				#region Contrôle Recall
				if(eventButton==5) {
					menuWebControl_Click();
				}	
				#endregion
				
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
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
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
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

		#region Boutton Valider
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try {

				_webSession.CurrentUniversSponsorshipForm=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversSponsorshipForm.Clone();
				_webSession.Save(); 

				if(AdvertiserSelectionWebControl1.NbElement==1) {
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				}
				else if (AdvertiserSelectionWebControl1.NbElement==2) {
					// Erreur : Nbre d'élément < 1000
					Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else {
					// Erreur : 1 élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(2059,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Menu Web Click
		/// <summary>
		/// Menu Web Click
		/// </summary>
		private void menuWebControl_Click() {
			try {

				_webSession.CurrentUniversSponsorshipForm=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversSponsorshipForm.Clone();
				_webSession.Save();

				if(AdvertiserSelectionWebControl1.NbElement==1) {
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				}
				else if (AdvertiserSelectionWebControl1.NbElement==2) {
					// Erreur : Nbre d'élément < 1000
					Response.Write("<script language=javascript>");
                    Response.Write("alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else {
					// Erreur : 1 élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(2059,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Boutton Genres d'émissions / émissions
		/// <summary>
		/// Bouton Forme de parrainage
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void programTypeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try {

				_webSession.CurrentUniversSponsorshipForm=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversSponsorshipForm.Clone();
				_webSession.Save();

				if(AdvertiserSelectionWebControl1.NbElement==1) {
					DBFunctions.closeDataBase(_webSession);
					_nextUrl = this._currentModule.GetSubSectionURL(PROGRAM_TYPE_ID,Page.Request.Url.AbsolutePath,false); 
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				}
				else if (AdvertiserSelectionWebControl1.NbElement==2) {
					// Erreur : Nbre d'élément < 1000
					Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else {
					// Erreur : 1 élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(2059,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Boutton Enregistrer
		/// <summary>
		/// Bouton Enregistrer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			Int64 idUniverseClientDescription=-1;
			TNS.AdExpress.Constantes.Classification.Branch.type branchType=TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm;
			
			idUniverseClientDescription=WebConstantes.LoadableUnivers.SPONSORSHIP_FORM;
			branchType=TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm;
			
			if(AdvertiserSelectionWebControl1.NbElement==1) {

				saveScript=WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType,idUniverseClientDescription);		
			}
			else if (AdvertiserSelectionWebControl1.NbElement==2) {
				// Erreur : Nbre d'élément < 1000
				Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
				Response.Write("history.go(-1);");
				Response.Write("</script>");
			}
			else {
				// Erreur : 1 élément
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(2059,_webSession.SiteLanguage)+"\");");
				Response.Write("history.go(-1);");
				Response.Write("</script>");
			}
		}
		#endregion
		
		#region Boutton charger
		/// <summary>
		/// Bouton chargement
		/// </summary>
		protected bool loadUniversAdvertiser() {
			
			string[] tabParent=null;
			Int64 idUniverse=0;	
			bool selectionnedUnivers=false;

			foreach (string currentKey in Request.Form.AllKeys) {

				tabParent=currentKey.Split('_');
				if(tabParent[0]=="UNIVERSE") {
							
					idUniverse=Int64.Parse(tabParent[1]);		
				}
			}
			
			if(idUniverse!=0) {

				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {

					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
			
				System.Windows.Forms.TreeNode treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
				_webSession.SelectionUniversSponsorshipForm=treeNodeUniverse;
				_webSession.Save();
				selectionnedUnivers=true;
			}
			
			return selectionnedUnivers; 
		}
		#endregion

		#region Boutton ok
		/// <summary>
		/// Bouton OK
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void okButtonRollOverWebControl2_Click(object sender, System.EventArgs e) {
		}
		#endregion	

		#region Nouvelle Recherche
		/// <summary>
		/// Nouvelle Recherche
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void initializeImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			textBoxProgramTypeChoice.Text="";
		}
		#endregion

		#region Charger
		/// <summary>
		/// Chargement de l'univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			if(eventButton!=4) {
				//if(eventButton!=constEvent.eventSelection.LOAD_EVENT) {
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(926,_webSession.SiteLanguage)+"\");");
				Response.Write("history.go(-1);");
				Response.Write("</script>");
			}
		}
		#endregion

		#endregion

		#region Implémentation méthodes abstarites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e) {
			//this.validateButton_Click(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu() {
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

		#region Fonction
		/// <summary>
		/// Place le focus sur le contrôle sélectionné
		/// </summary>
		/// <param name="champ">Champ sur lequel on veut mettre le focus</param>
		public  void setFocus(System.Web.UI.WebControls.WebControl champ) {
			//Champ est le webcontrol sur lequel on veut mettre le focus
			if(!Page.ClientScript.IsClientScriptBlockRegistered("focus")) {
				
				string s;
				s = "<script language= \"javascript\">document.getElementById('" + 
					champ.ClientID + "').focus()</script>"; 
			
				Page.ClientScript.RegisterStartupScript(this.GetType(),"focus", s);
			}
		}
		/// <summary>
		/// Gestion affichage des options de sélection du niveau de nomenclature
		///  en fonction du module.
		/// </summary>
		/// <param name="_webSession">session du client</param>
		private void ShowNomenclatureOptions(WebSession _webSession) {
		}
		#endregion
	}
}
