#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 12/12/2006
// Date de modification: 
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
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;

namespace AdExpress.Private.Selection
{
	/// <summary>
	/// Page de affiner univers des supports
	/// </summary>
	public partial class RefineMediaUniverseSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
			
		#region Varaibles MMI
		/// <summary>
		/// Contr�le : ModuleTitle
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleTitleWebControl ModuleTitleWebControl1;
		/// <summary>
		/// Contr�le : aide
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HelpWebControl helpWebControl;
		/// <summary>
		/// Contr�le En-t�te
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
		/// TextBox mot-cl�
		/// </summary>
		/// <summary>
		/// Bouton : valider
		/// </summary>
	
		/// <summary>
		/// Bouton : prochain
		/// </summary>
		/// <summary>
		/// Contr�le : titre du module
		/// </summary>
		/// <summary>
		/// Contr�le En t�te
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
		/// Contr�le chargement d'un univers
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Sauvegarde d'un univers
		/// </summary>
		/// <summary>
		/// S�lection des annonceurs
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Ev�nement lanc�
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
		/// Contr�le d'information
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
		public RefineMediaUniverseSelection():base(){			
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

                VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

				#region Initialssation de la s�lection supports
				if(!Page.IsPostBack){
					_webSession.CompetitorUniversMedia = new Hashtable(5);
					 _webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("Media");
				}
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				ModuleTitleWebControl2.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
                HeaderWebControl2.Language = _webSession.SiteLanguage;
                AdExpressText2.Language = _webSession.SiteLanguage;
                
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
						
				#region Ev�nements Boutons
				
				// Bouton valider
				if(Request.Form.Get("__EVENTTARGET")=="validImageButtonRollOverWebControl" 					
					|| Request.Form.Get("__EVENTTARGET")==MenuWebControl2.ID
					){
					eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Bouton ok
				else if(Request.Form.Get("__EVENTTARGET")=="OkImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.OK_EVENT;

					// Bouton ok Option
				}else if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){

                    if (Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2") != null) {
                        foreach (DetailLevelItemInformation.Levels currentLevel in vehicleInformation.MediaSelectionParentsItemsEnumList)
                            if (Page.Request.Form.GetValues("mediaDetail_MediaSellerWebControl2")[0].ToString() == "MediaSeller_" + currentLevel.GetHashCode())
                                _webSession.MediaSelectionParent = currentLevel;
                    }

					eventButton=constEvent.eventSelection.OK_OPTION_MEDIA_EVENT;

					
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
                    _webSession.MediaSelectionParent = vehicleInformation.DefaultMediaSelectionParent;

				#endregion

				#region D�finition du contr�le DetailVehicleSelectionWebControl1 
				AdvertiserSelectionWebControl1.EventButton=eventButton;
				AdvertiserSelectionWebControl1.KeyWord=keyWordTextBox.Text;
				#endregion


				#region Remplace les caract�res accuentu�s du mot cl�
				// Remplace les caract�res accuentu�s
				keyWordTextBox.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(keyWordTextBox.Text);
				#endregion

				#region Boutons RollOver
				//validImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";		
									
				//saveImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				//saveImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
				//loadImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/charger_up.gif";
				//loadImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/charger_down.gif";								

				#region test
				if(!Page.IsPostBack){
					validImageButtonRollOverWebControl.Visible=true;					
					saveImageButtonRollOverWebControl.Visible=true;
				
				}																

				#endregion

				// Commentaire lorsque la souris pointe sur les boutons
				initializeButton.ToolTip=GestionWeb.GetWebWord(974,_webSession.SiteLanguage);
				initializeAllButton.ToolTip=GestionWeb.GetWebWord(979,_webSession.SiteLanguage);
			
				#endregion

				#region Gestion Entrer
				//Gestion de l'�v�nements entrer sur la textBox
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
				//Gestion de la s�lection d'un radiobutton dans la liste des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"InsertIdMySession4",TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
				}				
				// Ouverture/fermeture des fen�tres p�res
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
			
				// Gestion lorsqu'on clique sur entr�e
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
				}	

				// S�lection/d�s�lection de tous les fils (cas 2 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				// S�lection de tous les fils
				if (!Page.ClientScript.IsClientScriptBlockRegistered("Integration")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Integration",TNS.AdExpress.Web.Functions.Script.Integration());
				}
				// S�lection de tous les �l�ments (cas 1 niveau)
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

		#region Code g�n�r� par le Concepteur Web Form

		/// <summary>
		/// Initlaisation
		/// </summary>
		/// <param name="e">arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent() {
           

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

			//Liste des univers			
			if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==DBClassificationConstantes.Vehicles.names.press.GetHashCode()
                ||((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationConstantes.Vehicles.names.newspaper.GetHashCode()
                ||((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationConstantes.Vehicles.names.magazine.GetHashCode()
                ){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.radio.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.tv.GetHashCode()){
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES ||
					_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS )
					branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaTvSponsorship;
				else
					branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv;
			}
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaOutdoor;
			}
            else if (((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)DBClassificationConstantes.Vehicles.names.instore.GetHashCode()) {
                branchType = TNS.AdExpress.Constantes.Classification.Branch.type.mediaInstore;
            }
			else if(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID==(long)DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternationalPress;
			}
            else if (((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)DBClassificationConstantes.Vehicles.names.cinema.GetHashCode()){
                branchType = TNS.AdExpress.Constantes.Classification.Branch.type.mediaCinema;
            }


			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();

			MediaSellerWebControl2.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.ListBranchType=branchType.GetHashCode().ToString();
			MenuWebControl2.CustomerWebSession = _webSession;
			AdvertiserSelectionWebControl1.CustomerWebSession=_webSession;
			return tmp;

		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
		
			try{
				
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_POTENTIELS || 
					_webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_POTENTIELS){
					if(_webSession.mediaUniversNumber()==0){
						validImageButtonRollOverWebControl.Visible=false;						
					}else if(_webSession.mediaUniversNumber()==1){						
						validImageButtonRollOverWebControl.Visible=true;
					}
				}
				
				#region affichage des bouttons
				if(Page.IsPostBack){
					switch(_webSession.CurrentModule){
						case WebConstantes.Module.Name.ALERTE_POTENTIELS:
						case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
					
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								validImageButtonRollOverWebControl.Visible=false;								
								saveImageButtonRollOverWebControl.Visible=false;							
							}
							else{								
								saveImageButtonRollOverWebControl.Visible=true;
							}							
							break;

						case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								validImageButtonRollOverWebControl.Visible=false;							
								saveImageButtonRollOverWebControl.Visible=false;							
							}
							else{
								validImageButtonRollOverWebControl.Visible=true;								
								saveImageButtonRollOverWebControl.Visible=true;
							}							
							break;					
						default:
						
							if (DetailVehicleSelectionWebControl.IsEmptyList==true) {
								validImageButtonRollOverWebControl.Visible=false;								
								saveImageButtonRollOverWebControl.Visible=false;							
							}
							else{
								validImageButtonRollOverWebControl.Visible=true;								
								saveImageButtonRollOverWebControl.Visible=true;
							}							
							break;
					}
				}

				#endregion		
			
								
				//Gestion page suivante
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

		#region Bouton Valider
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void validImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{

				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL && AdvertiserSelectionWebControl1.NbElement!=constEvent.error.MEDIA_SELECTED_ALREADY){
					int numberCompetitorMedia=_webSession.CompetitorUniversMedia.Count+1;
					_webSession.CompetitorUniversMedia.Add(numberCompetitorMedia,(System.Windows.Forms.TreeNode)_webSession.CurrentUniversMedia.Clone());
					_webSession.Save();
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				
				}else if (AdvertiserSelectionWebControl1.NbElement==constEvent.error.CHECKBOX_NULL) {
					// Erreur : Il faut s�lectionner au moins un �l�ment
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else if(AdvertiserSelectionWebControl1.NbElement==constEvent.error.MEDIA_SELECTED_ALREADY){
					// Erreur : Les supports ont d�j� �t� s�lectionn�s
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
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void saveImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{			
				if(AdvertiserSelectionWebControl1.NbElement!=constEvent.error.CHECKBOX_NULL){
					Int64 idUniverseClientDescription=WebConstantes.LoadableUnivers.CATEGORY_MEDIA; 	

					saveScript = WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType,idUniverseClientDescription);
				}else{
					// Erreur : Il faut s�lectionner au moins un m�dia
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
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
				
		#endregion

		#region Fonction
		/// <summary>
		/// Place le focus sur le contr�le s�lectionn�
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

		#region Impl�mentation m�thodes abstraites
		/// <summary>
		/// Validation de la s�lection de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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

	}
}
