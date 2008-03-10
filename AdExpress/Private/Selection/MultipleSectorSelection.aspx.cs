#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/02/2005
// Date de modification:  24/02/2005  D. V. Mussuma Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;

#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection des familles
	/// </summary>
	public partial class MultipleSectorSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
		
		#region Variables MMI
		/// <summary>
		/// Texte "Module X"
		/// </summary>
		/// <summary>
		/// Entete flash et menu
		/// </summary>
		/// <summary>
		/// Control de sélection des Familles
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Bouton sauvegarde univers
		/// </summary>
		/// <summary>
		/// Chargement de l'univers
		/// </summary>
		/// <summary>
		/// Bouton chargement d'un univers
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose="";
		/// <summary>
		/// Script
		/// </summary>
		public string Script="";
		/// <summary>
		/// Evènement lancé
		/// </summary>
		public int eventButton=-1;		
		/// <summary>
		/// Code html bouton precedent
		/// </summary>
		public string previousButton="";	
		/// <summary>
		/// Type de branche au niveau de la nomenclature
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.Branch.type branchType=TNS.AdExpress.Constantes.Classification.Branch.type.product;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniverses="";

		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MultipleSectorSelection ():base(){
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

				#region Flash pour patienter
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=validateButton.ID && nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
						Page.Response.Write(LoadingSystem.GetHtmlCloseDiv());
						//Response.Redirect(this.ViewState["url"].ToString());
					}
				}
				#endregion	

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);							
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;												
				#endregion

				#region Rollover des boutons
				validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton.ToolTip=GestionWeb.GetWebWord(1536,_webSession.SiteLanguage);

				//Boutons univers :
				loadImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/charger_up.gif";
				loadImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/charger_down.gif";

				// bouton sauvegarder
				saveUniverseImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				saveUniverseImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
			
			
				#endregion

				#region évènemment
				// Connaître le boutton qui a été cliquer 
				eventButton=0;			

				// Boutton valider
				if(Request.Form.Get("__EVENTTARGET")=="validateButton" ){
					eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Boutton ok
				else if(Request.Form.Get("__EVENTTARGET")=="okButtonRollOverWebControl2"){
					eventButton=2;
				}
					// Boutton sauvegarder
				else if(Request.Form.Get("__EVENTTARGET")=="saveUniverseImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.SAVE_EVENT;
				}
					// Boutton Charger
				else if(Request.Form.Get("__EVENTTARGET")=="loadImageButtonRollOverWebControl"){
					eventButton=constEvent.eventSelection.LOAD_EVENT;
				}
					// Controle Recall
				else if(Request.Form.Get("__EVENTTARGET")=="MenuWebControl2"){
					eventButton=constEvent.eventSelection.VALID_EVENT;
				}
					// Bouton OK du pop up qui enregistre l'univers
				else if(HttpContext.Current.Request.QueryString.Get("saveUnivers")!= null){
					//				eventButton=int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));
					eventButton=constEvent.eventSelection.OK_POP_UP_EVENT;
				}		
				#endregion

				#region Chargement univers
				// Bouton Charger 
				if(eventButton==constEvent.eventSelection.LOAD_EVENT){
					if(!loadUniversProduct()){
						eventButton=-1;
						this.loadImageButtonRollOverWebControl_Click(null,null);
						
					}
				}
				#endregion						

				#region Définition de la page d'aide
				//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"MultipleSectorSelectionHelp.aspx";
				#endregion					

				#region Définition du contrôle sectorSelectionWebControl
				sectorSelectionWebControl.WebSession=_webSession;
				sectorSelectionWebControl.Title = GestionWeb.GetWebWord(1535,_webSession.SiteLanguage);
				sectorSelectionWebControl.EventButton=eventButton;				
				#endregion

				#region Script
				//Gestion de la sélection d'un radiobutton dans la liste des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"InsertIdMySession4",TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
				}				
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
			
				// Gestion lorsqu'on clique sur entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
				}	

				//				// Sélection/désélection de tous les fils (cas 2 niveau)
				//				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
				//					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
				//				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}

				#endregion

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
				if(!sectorSelectionWebControl.hasRecords){
					validateButton.Visible=false;
					saveUniverseImageButtonRollOverWebControl.Visible=false;
				}
			}
			catch(System.Exception exc) {
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
		/// Evènement d'initialisation de la page 
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
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			LoadableUniversWebControl1.CustomerWebSession=_webSession;
			sectorSelectionWebControl.WebSession=_webSession;				
			LoadableUniversWebControl1.ListBranchType=TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();
			LoadableUniversWebControl1.SelectionPage = true;
			LoadableUniversWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.product;
			LoadableUniversWebControl1.ForGenericUniverse = true;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;

		}
		#endregion

		#region Bouton Valider
		/// <summary>
		/// Valide la sélection
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			//COmmentaires à conserver en vue d'une amélioration ==> recocher les cases déjà cochées en cas d'erreur			
			//_webSession.CurrentUniversProduct=TreeBuilding();
			_webSession.PrincipalProductUniverses = GetUniverseSelection();
				//if(_webSession.CurrentUniversProduct.Nodes.Count >0 ){
			if (_webSession.PrincipalProductUniverses.Count > 0) {
				//redirection
				_webSession.Save();
				_webSession.Source.Close();
				//Redirection Page suivante
				if (_nextUrl.Length > 0) { // if(sender!=null && _nextUrl.Length>0)					
					Response.Redirect(this._nextUrl + "?idSession=" + _webSession.IdSession);
				}
			}
			else {
				Script = "javascript:alert('" + GestionWeb.GetWebWord(1537, _webSession.SiteLanguage) + "');";
			}
			_webSession.Save();
		}
		#endregion

		#region Construction de l'arbre
		/// <summary>
		/// Fonction qui construit un arbre à partir des éléments sélectionnés dans le webControl
		/// sectorSelectionWebControl
		/// </summary>
		/// <returns></returns>
		private System.Windows.Forms.TreeNode TreeBuilding(){
			System.Windows.Forms.TreeNode sectors = new System.Windows.Forms.TreeNode("sector");
			System.Windows.Forms.TreeNode tmp;
			foreach(ListItem item in this.sectorSelectionWebControl.Items){
				if (item.Selected){
					tmp = new System.Windows.Forms.TreeNode(item.Text);
					tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.sectorAccess,Int64.Parse(item.Value),item.Text);
					tmp.Checked = true;
					sectors.Nodes.Add(tmp);
				}
			}
			return sectors;
		}
		#endregion

		#region Get Universe selection
		/// <summary>
		/// Get universe builded from selected items
		/// </summary>
		/// <returns>product universe</returns>
		private Dictionary<int, AdExpressUniverse> GetUniverseSelection() {

			System.Collections.Generic.Dictionary<int, AdExpressUniverse> universeDictionary = new System.Collections.Generic.Dictionary<int, AdExpressUniverse>();
			AdExpressUniverse adExpressUniverse = null;
			List<long> itemList = new List<long>();
			NomenclatureElementsGroup nomenclatureGroup = null;

			foreach (ListItem item in this.sectorSelectionWebControl.Items) {
				if (item.Selected) {
					itemList.Add(Int64.Parse(item.Value));					
				}
			}
			if (itemList.Count > 0) {
				nomenclatureGroup = new NomenclatureElementsGroup(0, AccessType.includes);
				nomenclatureGroup.AddItems(TNSClassificationLevels.SECTOR,itemList);
				adExpressUniverse = new AdExpressUniverse(Dimension.product);
				adExpressUniverse.AddGroup(0, nomenclatureGroup);
				universeDictionary.Add(0, adExpressUniverse);
			}
			return universeDictionary;
		}

		#endregion 

		#region Bouton Enregistrer
		/// <summary>
		/// Bouton Enregistrer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{

				TNS.AdExpress.Constantes.Classification.Branch.type branchType=TNS.AdExpress.Constantes.Classification.Branch.type.product;
				System.Collections.Generic.Dictionary<int, AdExpressUniverse> universeDictionary = GetUniverseSelection();
				//if(sectorSelectionWebControl.NbElement==1){
				if (universeDictionary != null && universeDictionary.Count > 0) {
			
					Int64 idUniverseClientDescription=-1;
					branchType=TNS.AdExpress.Constantes.Classification.Branch.type.product;
					//idUniverseClientDescription=WebConstantes.LoadableUnivers.SECTOR;
					idUniverseClientDescription = WebConstantes.LoadableUnivers.GENERIC_UNIVERSE;
					
					//_webSession.CurrentUniversProduct=TreeBuilding();
					_webSession.PrincipalProductUniverses = universeDictionary;
					_webSession.Save();
					
					this.sectorSelectionWebControl.LoadSession=true;
//					validateButton_Click(null,null);									
					Script=WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType,idUniverseClientDescription);					
				}				
				else{
					// Erreur 
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(1537,_webSession.SiteLanguage)+"\");");
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
		/// Bouton chargement
		/// </summary>
		protected bool loadUniversProduct(){
			
			string[] tabParent=null;
			Int64 idUniverse=0;
			bool selectionnedUnivers=false;
			foreach (string currentKey in Request.Form.AllKeys){
				tabParent=currentKey.Split('_');
				if(tabParent[0]=="UNIVERSE") {							
					idUniverse=Int64.Parse(tabParent[1]);		
				}
			}
			if(idUniverse!=0){
				//System.Windows.Forms.TreeNode treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
				//_webSession.CurrentUniversProduct=treeNodeUniverse;
				System.Collections.Generic.Dictionary<int, AdExpressUniverse> universeDictionary = (Dictionary<int, AdExpressUniverse>)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(idUniverse, _webSession);
				_webSession.PrincipalProductUniverses = universeDictionary;
				_webSession.Save();
				selectionnedUnivers=true;				
			}			

			return selectionnedUnivers;
		}
		#endregion
		
		#region Charger
		/// <summary>
		/// Chargement de l'univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			if(eventButton!=4){
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(926,_webSession.SiteLanguage)+"\");");
				Response.Write("history.go(-1);");
				Response.Write("</script>");
			}
		}
		#endregion
	
		#endregion		

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.validateButton_Click(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
