#region Informations
// Auteur: G. Facon
// Date de création: 05/05/2004
// Date de modification: 10/05/2004
//	08/11/2004  G. Facon Enlever le bouton suivant Lorsque l'on fait une analyse dynamique
//	19/12/2004 A. Obermeyer Intégration de WebPage
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
using TNS.AdExpress.Web.Controls.Selections;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de Sélection des Vehicles
	/// </summary>
	public partial class DetailVehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Constantes
		/// <summary>
		/// L'Id de la sub section qui represente la page ProgramType
		/// </summary>
		protected const int PROGRAM_TYPE_ID = 7;
		/// <summary>
		/// L'Id de la sub section qui represente la page SponsorshipForm
		/// </summary>
		protected const int SPONSORSHIP_FORM_ID = 8;
		#endregion

		#region Varaibles MMI
		/// <summary>
		/// Contrôle : ModuleTitle
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleTitleWebControl ModuleTitleWebControl1;
		/// <summary>
		/// Contrôle : aide
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HelpWebControl helpWebControl;
		/// <summary>
		/// Contrôle En-tête
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HeaderWebControl HeaderWebControl1;
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Bouton Initialisation
		/// </summary>
		/// <summary>
		/// Bouton initialisation de tous les univers
		/// </summary>
		/// <summary>
		/// Chargement de l'univers
		/// </summary>
		/// <summary>
		/// TextBox mot-clé
		/// </summary>
		/// <summary>
		/// Bouton : valider
		/// </summary>
		/// <summary>
		/// Bouton : prochain
		/// </summary>
		/// <summary>
		/// Bouton : prochain
		/// </summary>
		/// <summary>
		/// Contrôle : titre du module
		/// </summary>
		/// <summary>
		/// Contrôle En tête
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText3;
		/// <summary>
		/// TextBox
		/// </summary>
		protected System.Web.UI.WebControls.TextBox universMediaTextBox;
		/// <summary>
		/// Contrôle chargement d'un univers
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Sauvegarde d'un univers
		/// </summary>
		/// <summary>
		/// Sélection des annonceurs
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Evènement lancé
		/// </summary>
		public int eventButton=-1;
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniverses="";
		/// <summary>
		/// Script
		/// </summary>
		public string saveScript="";
		/// <summary>
		/// Fermeture du flash
		/// </summary>
		public string divClose="";
		/// <summary>
		/// A1
		/// </summary>
//		/// <summary>
//		/// A1
//		/// </summary>
//		protected TNS.AdExpress.Web.Controls.Headers.MediaSellerWebControl MediaSellerWebControl;
		/// <summary>
		/// A1
		/// </summary>
		/// <summary>
		/// Bouton Ok
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Contrôle d'information
		/// </summary>
		/// <summary>
		/// Bouton Genre d'émissions
		/// </summary>
		/// <summary>
		/// Bouton Formes de parrainage
		/// </summary>
		/// <summary>
		/// Type de branche au niveau de la nomenclature
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.Branch.type branchType=TNS.AdExpress.Constantes.Classification.Branch.type.media;
		

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DetailVehicleSelection():base(){			
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Initialssation de la sélection supports
				if(!Page.IsPostBack){
					_webSession.CompetitorUniversMedia = new Hashtable(5);
					_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("Media");
				}
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
                for (int i = 0; i < this.Controls.Count; i++) {
                    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
				ModuleTitleWebControl2.CustomerWebSession = _webSession;
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
						
				#region Evènements Boutons
				
				// Bouton valider
				if(Request.Form.Get("__EVENTTARGET")=="validImageButtonRollOverWebControl" 
					|| Request.Form.Get("__EVENTTARGET")=="formSponsoringshipImageButtonRollOverWebControl"
				    || Request.Form.Get("__EVENTTARGET")=="programTypeImageButtonRollOverWebControl"
					|| Request.Form.Get("__EVENTTARGET")==MenuWebControl2.ID
					){
					eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Bouton ok
				else if(Request.Form.Get("__EVENTTARGET")=="OkImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.OK_EVENT;

					// Bouton ok Option
				}else if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){
					
					if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")!=null){
						if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia.GetHashCode().ToString()){
							_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia;																	
						}else if( Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia.GetHashCode().ToString()){
							_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia;																							
						}else if(Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia.GetHashCode().ToString()){
							_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia;																							
						}else if( Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString()=="MediaSeller_"+SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia.GetHashCode().ToString()){
							_webSession.PreformatedMediaDetail = CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia;																		
						}
					}
													

					eventButton=constEvent.eventSelection.OK_OPTION_MEDIA_EVENT;

					// Bouton suivant
				}else if(Request.Form.Get("__EVENTTARGET")=="nextImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.NEXT_EVENT;
				}
					//Bouton Charger
				else if(Request.Form.Get("__EVENTTARGET")=="loadImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.LOAD_EVENT;
				}
					//Bouton Enregistrer
				else if(Request.Form.Get("__EVENTTARGET")=="saveImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.SAVE_EVENT;
				}
					// Bouton OK du pop up qui enregistre l'univers
				else if(HttpContext.Current.Request.QueryString.Get("saveUnivers")!= null){
					eventButton=constEvent.eventSelection.OK_POP_UP_EVENT;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="initializeButton"){
					eventButton=constEvent.eventSelection.INITIALIZE_EVENT;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="initializeAllButton"){
					eventButton=constEvent.eventSelection.ALL_INITIALIZE_EVENT;
				}
				else
					_webSession.PreformatedMediaDetail =CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia;

				#endregion

				#region Définition du contrôle DetailVehicleSelectionWebControl1 
				//				AdvertiserSelectionWebControl1.CustomerWebSession=_webSession;
				AdvertiserSelectionWebControl1.EventButton=eventButton;
				AdvertiserSelectionWebControl1.KeyWord=keyWordTextBox.Text;
				#endregion

				#region Remplace les caractères accuentués du mot clé
				// Remplace les caractères accuentués
				keyWordTextBox.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(keyWordTextBox.Text);
				#endregion

				#region Boutons RollOver
				//validImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";		
			
				//nextImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/suivant_up.gif";
				//nextImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/suivant_down.gif";			
				//saveImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				//saveImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
				//loadImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/charger_up.gif";
				//loadImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/charger_down.gif";
				
				#region Boutons parrainage				
				//programTypeImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/bt_emissions_up.gif";
				//programTypeImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/bt_emissions_down.gif";	
				//formSponsoringshipImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/bt_formes_up.gif";
				//formSponsoringshipImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/bt_formes_down.gif";			
				#endregion

				#region test
				if(!Page.IsPostBack){
					validImageButtonRollOverWebControl.Visible=true;
					nextImageButtonRollOverWebControl.Visible=true;
					saveImageButtonRollOverWebControl.Visible=true;
				
				}										
						

				#endregion

				// Commentaire lorsque la souris pointe sur les boutons
				initializeButton.ToolTip=GestionWeb.GetWebWord(974,_webSession.SiteLanguage);
				initializeAllButton.ToolTip=GestionWeb.GetWebWord(979,_webSession.SiteLanguage);
			
				#endregion

				#region Gestion Entrer
				//Gestion de l'évènements entrer sur la textBox
				keyWordTextBox.Attributes.Add("onkeydown","javascript:trapEnter();");				
				#endregion

				#region Focus sur le textBox
				this.setFocus(keyWordTextBox);
				#endregion

				#region Chargement d'un univers
				if(eventButton==constEvent.eventSelection.LOAD_EVENT){
					string[] tabParent=null;
					Int64 idUniverse=0;
					foreach (string currentKey in Request.Form.AllKeys){
						tabParent=currentKey.Split('_');
						if(tabParent[0]=="UNIVERSE") {							
							idUniverse=Int64.Parse(tabParent[1]);		
						}
					}
					if(idUniverse!=0){
						if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
							Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
						}			
						System.Windows.Forms.TreeNode treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
						_webSession.CurrentUniversMedia=treeNodeUniverse;
						_webSession.Save();			
					}else{
						loadImageButtonRollOverWebControl_Click(null,null);
					}	
				}
				#endregion

				#region Script
				//Gestion de la sélection d'un radiobutton dans la liste des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"InsertIdMySession4",TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
				}				
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
			
				// Gestion lorsqu'on clique sur entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
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

				_webSession.Save();
				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
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
		protected void Page_UnLoad(object sender, System.EventArgs e){

			
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

			//Liste des univers			
			if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==DBClassificationConstantes.Vehicles.names.press.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.radio.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.tv.GetHashCode()){
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES )
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaTvSponsorship;
				else
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaOutdoor;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternationalPress;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet;
			}


			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();

			//RecallWebControl1.CustomerWebSession=_webSession;
			MediaSellerWebControl2.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.ListBranchType=branchType.GetHashCode().ToString();
			MenuWebControl2.CustomerWebSession = _webSession;
			
			//On affiche pas l'icone représentant le racourci vers la page de résultats si les dates sélectionnées sont aux formats autres que mensuels
			//et que le média Internet est sélectionné
			string idVehicleList =  _webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);						
			if(_webSession.isDatesSelected() && idVehicleList!=null && idVehicleList.Length>0 
				&& idVehicleList.IndexOf(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString())>=0
				&& _webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.monthly &&  _webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.yearly){			
				this.MenuWebControl2.ForbidResultIcon = true;									
			}

			AdvertiserSelectionWebControl1.CustomerWebSession=_webSession;
			return tmp;

		}
		#endregion

		#region Bouton suivant
		/// <summary>
		/// Bouton : Page suivante
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void nextImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{
//				if(_webSession.CompetitorUniversMedia!=null && _webSession.CompetitorUniversMedia.Count<5){//5 univers au maximum
					if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MEDIA_SELECTED_ALREADY && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MAX_ELEMENTS){
						int numberCompetitorMedia=_webSession.CompetitorUniversMedia.Count+1;
						_webSession.CompetitorUniversMedia.Add(numberCompetitorMedia,(System.Windows.Forms.TreeNode)_webSession.CurrentUniversMedia.Clone());
						_webSession.Save();				

					}else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.CHECKBOX_NULL){
						// Erreur : Il faut sélectionner au moins un élément
						Response.Write("<script language=javascript>");
						Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
						Response.Write("history.go(-1);");
						Response.Write("</script>");
					}
					else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.MAX_ELEMENTS) {
						// Erreur : Il faut sélectionner moins de 200 éléments
						Response.Write("<script language=javascript>");
                        Response.Write("alert(\"" + GestionWeb.GetWebWord(2264, _webSession.SiteLanguage) + "\");");
						Response.Write("history.go(-1);");
						Response.Write("</script>");
					}
					else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.MEDIA_SELECTED_ALREADY){
						// Erreur : Les supports ont déjà été sélectionnés
						Response.Write("<script language=javascript>");
						Response.Write("alert(\""+GestionWeb.GetWebWord(1475,_webSession.SiteLanguage)+"\");");
						//Response.Write("history.go(-1);");
						Response.Write("</script>");
					}
//				}else{ 
//					// Erreur : sélection de 5 univers au maximum
//					Response.Write("<script language=javascript>");
//					Response.Write("alert(\""+GestionWeb.GetWebWord(978,_webSession.SiteLanguage)+"\");");
//					Response.Write("history.go(-1);");
//					Response.Write("</script>");
//				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
		
			try{
				// Dans l'analyse dynamique, le bouton suivant n'est pas affiché
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
					nextImageButtonRollOverWebControl.Visible=false;
				}
				else if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_POTENTIELS || 
					_webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_POTENTIELS){
					if(_webSession.mediaUniversNumber()==0){
						validImageButtonRollOverWebControl.Visible=false;
						nextImageButtonRollOverWebControl.Visible=true;
					}else if(_webSession.mediaUniversNumber()==1){
						nextImageButtonRollOverWebControl.Visible=false;
						validImageButtonRollOverWebControl.Visible=true;
					}
				}
				else if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES ){
					programTypeImageButtonRollOverWebControl.Visible = true;
					formSponsoringshipImageButtonRollOverWebControl.Visible = true;
					nextImageButtonRollOverWebControl.Visible=false;
				}
				#region affichage des bouttons
				if(Page.IsPostBack){
					switch(_webSession.CurrentModule){
						case WebConstantes.Module.Name.ALERTE_POTENTIELS:
						case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
					
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								validImageButtonRollOverWebControl.Visible=false;
								nextImageButtonRollOverWebControl.Visible=false;
								saveImageButtonRollOverWebControl.Visible=false;							
							}
							else{
								//validImageButtonRollOverWebControl.Visible=true;
								nextImageButtonRollOverWebControl.Visible=true;
								saveImageButtonRollOverWebControl.Visible=true;
							}
//							if(!DetailVehicleSelectionWebControl.LoadData){
//								validImageButtonRollOverWebControl.Visible=false;
//								nextImageButtonRollOverWebControl.Visible=false;
//								saveImageButtonRollOverWebControl.Visible=false;
//							}
							break;

							case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
								if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
									validImageButtonRollOverWebControl.Visible=false;
									nextImageButtonRollOverWebControl.Visible=false;
									saveImageButtonRollOverWebControl.Visible=false;							
								}
								else{
									validImageButtonRollOverWebControl.Visible=true;
									//nextImageButtonRollOverWebControl.Visible=true;
									saveImageButtonRollOverWebControl.Visible=true;
								}
//								if(!DetailVehicleSelectionWebControl.LoadData){
//									validImageButtonRollOverWebControl.Visible=false;
//									nextImageButtonRollOverWebControl.Visible=false;
//									saveImageButtonRollOverWebControl.Visible=false;
//								}
								break;
						case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES :							
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								programTypeImageButtonRollOverWebControl.Visible = false;
								formSponsoringshipImageButtonRollOverWebControl.Visible = false;
							}else{
								programTypeImageButtonRollOverWebControl.Visible = true;
								formSponsoringshipImageButtonRollOverWebControl.Visible = true;
							}
							break;

						default:
						
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								validImageButtonRollOverWebControl.Visible=false;
								nextImageButtonRollOverWebControl.Visible=false;
								saveImageButtonRollOverWebControl.Visible=false;							
							}
							else{
								validImageButtonRollOverWebControl.Visible=true;
								nextImageButtonRollOverWebControl.Visible=true;
								saveImageButtonRollOverWebControl.Visible=true;
							}
//							if(!DetailVehicleSelectionWebControl.LoadData){
//								validImageButtonRollOverWebControl.Visible=false;
//								nextImageButtonRollOverWebControl.Visible=false;
//								saveImageButtonRollOverWebControl.Visible=false;
//							}
							break;
					}
				}

				#endregion		

				#region Affichage du bouton suivant
				if(_webSession.CompetitorUniversMedia.Count>=TNS.AdExpress.Constantes.Web.CompetitorUnivers.MAX_NUMBER_UNIVERS-1){
					nextImageButtonRollOverWebControl.Visible=false;
				}		
				#endregion
								
				//Gestion page suivante
				if (this.IsPostBack){
					if(_nextUrlOk){
						this.validImageButtonRollOverWebControl_Click(null,null);
					}
				}	
				
				//Gestion icone page de resultat				
				if(!this.MenuWebControl2.ForbidResultIcon){
					this.MenuWebControl2.ForbidResultIcon = !validImageButtonRollOverWebControl.Visible;					
				}
				
				//Gestion icone date
				this.MenuWebControl2.ForbidOptionPagesList = null;
				if(_webSession.isDatesSelected() && !validImageButtonRollOverWebControl.Visible){
					ArrayList forbidOptionPagesList = new ArrayList();
					forbidOptionPagesList.Add(4);//Interdit de montrer l'icone date
					this.MenuWebControl2.ForbidOptionPagesList = forbidOptionPagesList;
				}
			
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}			
		}
		#endregion
		
		#region Bouton Valider
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{

				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MEDIA_SELECTED_ALREADY && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MAX_ELEMENTS){
					int numberCompetitorMedia=_webSession.CompetitorUniversMedia.Count+1;
					_webSession.CompetitorUniversMedia.Add(numberCompetitorMedia,(System.Windows.Forms.TreeNode)_webSession.CurrentUniversMedia.Clone());
					_webSession.Save();
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				
				}else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.CHECKBOX_NULL) {
					// Erreur : Il faut sélectionner au moins un élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.MAX_ELEMENTS) {
					// Erreur : Il faut sélectionner moins de 200 éléments
					Response.Write("<script language=javascript>");
                    Response.Write("alert(\"" + GestionWeb.GetWebWord(2264, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.MEDIA_SELECTED_ALREADY){
					// Erreur : Les supports ont déjà été sélectionnés
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1475,_webSession.SiteLanguage)+"\");");
					//Response.Write("history.go(-1);");
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

		#region Bouton sauvegarder
		/// <summary>
		/// Sauvegarde d'un univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void saveImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{			
				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MAX_ELEMENTS){
					Int64 idUniverseClientDescription=WebConstantes.LoadableUnivers.CATEGORY_MEDIA; 	

					saveScript = WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType,idUniverseClientDescription);
				}
				else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.MAX_ELEMENTS){
					// Erreur : Il faut sélectionner moins de 200 éléments
					Response.Write("<script language=javascript>");
                    Response.Write("alert(\"" + GestionWeb.GetWebWord(2264, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else{
					// Erreur : Il faut sélectionner au moins un média
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
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

		#region Bouton charger
		/// <summary>
		/// Chargement d'un univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{		
				if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.LOAD_NOT_POSSIBLE){
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1086,_webSession.SiteLanguage)+"\");");
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
		
		#region Boutons ok
		/// <summary>
		/// Ok
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void OkImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
		}

		/// <summary>
		/// Ok
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void okImageButton_Click(object sender, System.EventArgs e) {
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

		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Validation de la sélection de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			
		}

		/// <summary>
		/// Obtient l'url suivante
		/// </summary>
		/// <returns>Url suivante</returns>
		protected override string GetNextUrlFromMenu(){						
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

		#region Bouton Emissions
		/// <summary>
		///Bouton accès genres émissions
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void programTypeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MEDIA_SELECTED_ALREADY){
					int numberCompetitorMedia=_webSession.CompetitorUniversMedia.Count+1;
					_webSession.CompetitorUniversMedia.Add(numberCompetitorMedia,(System.Windows.Forms.TreeNode)_webSession.CurrentUniversMedia.Clone());
					_webSession.Save();
					_webSession.Source.Close();
					_nextUrl = this._currentModule.GetSubSectionURL(PROGRAM_TYPE_ID,Page.Request.Url.AbsolutePath,true); 
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				
				}else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.CHECKBOX_NULL) {
					// Erreur : Il faut sélectionner au moins un élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.MEDIA_SELECTED_ALREADY){
					// Erreur : Les supports ont déjà été sélectionnés
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1475,_webSession.SiteLanguage)+"\");");
					//Response.Write("history.go(-1);");
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

		#region Bouton formes de parrainage
		/// <summary>
		///Bouton accès formes de parrainage émissions
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void formSponsoringshipImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MEDIA_SELECTED_ALREADY){
					int numberCompetitorMedia=_webSession.CompetitorUniversMedia.Count+1;
					_webSession.CompetitorUniversMedia.Add(numberCompetitorMedia,(System.Windows.Forms.TreeNode)_webSession.CurrentUniversMedia.Clone());
					_webSession.Save();
					_webSession.Source.Close();
					_nextUrl = this._currentModule.GetSubSectionURL(SPONSORSHIP_FORM_ID,Page.Request.Url.AbsolutePath,true); 
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				
				}else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.CHECKBOX_NULL) {
					// Erreur : Il faut sélectionner au moins un élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.MEDIA_SELECTED_ALREADY){
					// Erreur : Les supports ont déjà été sélectionnés
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1475,_webSession.SiteLanguage)+"\");");
					//Response.Write("history.go(-1);");
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

	}
}
