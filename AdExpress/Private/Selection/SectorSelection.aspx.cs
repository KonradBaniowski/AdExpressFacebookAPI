#region Informations
// Auteur: 
// Date de création: 
// Date de modification: 
//	31/12/2004 A. Obermeyer Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
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

namespace AdExpress.Private.Selection {
	/// <summary>
	/// Classe permettant l'affichage des annonceurs où des familles.
	/// </summary>
	public partial class SectorSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
		
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
		/// Texte : annonceur
		/// </summary>
		/// <summary>
		/// textBox pour la sélection de l'annonceur 
		/// </summary>
		/// <summary>
		/// Texte : Saississez votre mot-clé
		/// </summary>
		/// <summary>
		/// Texte : vous pouvez accélérer votre recherche
		/// </summary>
		/// <summary>
		/// Bouton radion annonceur
		/// </summary>
		/// <summary>
		/// bouton radio référence
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
		/// Contrôle : liste des annonceurs 
		/// </summary>
		/// <summary>
		/// bouton ok
		/// </summary>
		/// <summary>
		/// Texte : recherche par famille
		/// </summary>
		/// <summary>
		/// Bouton radio famille
		/// </summary>
		/// <summary>
		/// Texte : recherche par classe
		/// </summary>
		/// <summary>
		/// Bouton radio classe
		/// </summary>
		/// <summary>
		/// Texte : recherche par groupe
		/// </summary>
		/// <summary>
		/// Bouton radio groupe
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Bouon initialisation
		/// </summary>
		/// <summary>
		/// Chargement d'un univers
		/// </summary>
		/// <summary>
		/// bouton radio choix marque
		/// </summary>
		
		/// <summary>
		/// Bouton radion groupe de société
		/// </summary>
		
		#endregion
		
		#region Variables
		/// <summary>
		/// fermeture du flash d'attente
		/// </summary>
		public string divClose;		
	//	protected System.Web.UI.HtmlControls.HtmlAnchor A1;		
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniverses="";
		/// <summary>
		/// Type d'évènement (bouton ok, valider...)
		/// </summary>
		protected int eventButton;
		/// <summary>
		/// A1
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Information
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// script ouvrant la popup permettant d'enregistrer un univers
		/// </summary>
		public string saveScript;
		/// <summary>
		/// css class for brand text
		/// </summary>
		public string cssBrand;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// css class for holding  company text
		/// </summary>
		public string cssHoldingCompany;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SectorSelection():base(){						
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Load
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
				cssBrand="txtViolet11";
				cssHoldingCompany="txtViolet11";
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

				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);

                _webSession.CurrentUniversProduct = new System.Windows.Forms.TreeNode("produit");
				_webSession.Save();
 
				#region Evènemment
				// Connaître le boutton qui a été cliqué 
				eventButton=0;

				if(HttpContext.Current.Request.QueryString.Get("saveUnivers")!= null){
					eventButton=int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));
				}
				// Boutton valider
				if(Request.Form.Get("__EVENTTARGET")=="validateButton"){
					eventButton=1;
				}
				// Boutton ok
				else if(Request.Form.Get("__EVENTTARGET")=="okButtonRollOverWebControl2"){
					eventButton=2;
				}
				// Boutton sauvegarder
				else if(Request.Form.Get("__EVENTTARGET")=="saveUniverseImageButtonRollOverWebControl"){
					eventButton=3;
				}
				// Boutton Charger
				else if(Request.Form.Get("__EVENTTARGET")=="loadImageButtonRollOverWebControl"){
					eventButton=4;
				}
					// Controle Recall
				else if(Request.Form.Get("__EVENTTARGET")=="MenuWebControl2"){
					eventButton=5;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="initializeImageButtonRollOverWebControl1"){
					eventButton=-1;
				}
				#endregion
			
				#region Chargement de l'univers
				// Bouton Charger 
				if(eventButton==4){
					if(!loadUniversAdvertiser()){
						eventButton=-1;
						loadImageButtonRollOverWebControl_Click(null,null);
					}
				}
				#endregion
			
				#region Affichage des boutons valider et enregistrer
				//Boutons valider et Enregistrer invisible lors du premier affichage
				if(!IsPostBack){
					saveUniverseImageButtonRollOverWebControl.Visible=false;
					validateButton.Visible=false;
					AdExpressText6.Visible=false;
				}	
				#endregion		

				// Remplace les caractères accuentués
				textBoxAdvertiserChoice.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(textBoxAdvertiserChoice.Text);
			
				#region Initialisation du composant Recall
				//if(IsPostBack || eventButton==6){	
				if(IsPostBack || eventButton==3){			
					// Initialisation du composant liste des annonceurs
					if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES){
						AdvertiserSelectionWebControl1.HoldingCompanyBool=radioButtonHoldingCompany.Checked;
						AdvertiserSelectionWebControl1.AdvertiserBool=radiobuttonAdvertiser.Checked;
						AdvertiserSelectionWebControl1.ProductBool=radiobuttonProduct.Checked;
					}
					AdvertiserSelectionWebControl1.SectorBool=RadiobuttonSector.Checked;
					AdvertiserSelectionWebControl1.SubSectorBool=RadiobuttonSubSector.Checked;
					AdvertiserSelectionWebControl1.GroupBool=RadiobuttonGroup.Checked;
					AdvertiserSelectionWebControl1.AdvertiserText=textBoxAdvertiserChoice.Text;
					AdvertiserSelectionWebControl1.WebSession=_webSession;
					AdvertiserSelectionWebControl1.ButtonTarget=eventButton;		
			
				}
				#endregion

				#region Url Suivante
//				_nextUrl=this.RecallWebControl1.NextUrl;
//				if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
				#endregion

				#region Rappel
//				// Rappel
//				ArrayList linkToShow=new ArrayList();
//				if(_webSession.isMediaSelected())linkToShow.Add(3);
//				if(_webSession.isDatesSelected())linkToShow.Add(4);
//				RecallWebControl1.LinkToShow=linkToShow;
//				if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected() && _webSession.isDatesSelected())RecallWebControl1.CanGoToResult=true;
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"SectorSelectionHelp.aspx";
				#endregion
					
				ModuleTitleWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;

				#region Rollover des boutons
				validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";

				okButtonRollOverWebControl2.ImageUrl="/Images/Common/button/ok_up.gif";
				okButtonRollOverWebControl2.RollOverImageUrl="/Images/Common/button/ok_down.gif";

				//Boutons univers :
				loadImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/charger_up.gif";
				loadImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/charger_down.gif";

				saveUniverseImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				saveUniverseImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
			

				#endregion

				#region Gestion Entrer
				//Gestion de l'évènements entrer sur la textBox
				textBoxAdvertiserChoice.Attributes.Add("onkeydown","javascript:trapEnter();");				
				#endregion

				//Focus sur la checkbox 
				this.setFocus(textBoxAdvertiserChoice);			
			
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
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("okButtonRollOverWebControl2"));
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

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//RecallWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.CustomerWebSession=_webSession;
			LoadableUniversWebControl1.ListBranchType=TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();
			if(_webSession.CurrentModule==WebConstantes.Module.Name.DONNEES_DE_CADRAGE){
				LoadableUniversWebControl1.SelectionPage=true;
			}
			MenuWebControl2.CustomerWebSession = _webSession;
			
			if(_webSession.CurrentModule!=WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES){
				radiobuttonAdvertiser.Visible =false;	
				AdExpressAdvertiser.Visible = false;
				radiobuttonProduct.Visible = false;	
				AdExpressProduct.Visible =false;
				RadiobuttonBrand.Visible = false;
				brandText.Visible = false;
				radioButtonHoldingCompany.Visible = false;
				AdExpressHolding.Visible = false;
			}
	
			return tmp;
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			
			// Affichage des boutons si existance d'annonceurs
			if(IsPostBack){
				if(AdvertiserSelectionWebControl1.DsListAdvertiser!=null){
					if(AdvertiserSelectionWebControl1.DsListAdvertiser.Tables[0].Rows.Count==0){
						saveUniverseImageButtonRollOverWebControl.Visible=false;
						validateButton.Visible=false;
						AdExpressText6.Visible=false;
					}
					else{
						saveUniverseImageButtonRollOverWebControl.Visible=true;
						validateButton.Visible=true;
						AdExpressText6.Visible=true;
					}
				}else{
					saveUniverseImageButtonRollOverWebControl.Visible=false;
					validateButton.Visible=false;				
					AdExpressText6.Visible=false;
				}
			}
			// Bouton Charger			
			if(eventButton==4 || eventButton==3){
				saveUniverseImageButtonRollOverWebControl.Visible=true;
				validateButton.Visible=true;	
				AdExpressText6.Visible=true;

			}
			//Contrôle Recall
			if(eventButton==5){
				this.validateButton_Click(null,null);
			}
			
			//if(eventButton==6){
			if(eventButton==3){
				#region Désactivation des boutons				
				
				RadiobuttonSector.Enabled=false;
				RadiobuttonSubSector.Enabled=false;
				RadiobuttonGroup.Enabled=false;				
				RadiobuttonSector.Checked=false;
				RadiobuttonSubSector.Checked=false;
				RadiobuttonGroup.Checked=false;					
				radioButtonHoldingCompany.Checked=false;
				radiobuttonAdvertiser.Checked=false;
				radiobuttonProduct.Checked=false;

				if(_webSession.SelectionUniversAdvertiser.FirstNode!=null){
					if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ){  
						radioButtonHoldingCompany.Enabled=true;
						radioButtonHoldingCompany.Checked=true;
					}			
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ){  
						radiobuttonAdvertiser.Enabled=true;
						radiobuttonAdvertiser.Checked=true;
					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ){  
						RadiobuttonBrand.Enabled=true;
						RadiobuttonBrand.Checked=true;
					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ){  
						radiobuttonProduct.Enabled=true;
						radiobuttonProduct.Checked=true;
					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ){  
						RadiobuttonSector.Enabled=true;	
						RadiobuttonSector.Checked=true;		
					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ){  
						RadiobuttonSubSector.Enabled=true;
						RadiobuttonSubSector.Checked=true;
					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ){  
						RadiobuttonGroup.Enabled=true;
						RadiobuttonGroup.Checked=true;
					}
				}
				#endregion
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
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

		#region Boutton Valider
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			
			try{
				_webSession.CurrentUniversProduct=_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone();
				_webSession.Save();
				if(AdvertiserSelectionWebControl1.NbElement==1){
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
					//Response.Redirect("VehicleSelection.aspx?idSession="+_webSession.IdSession+"");
				}
				else if (AdvertiserSelectionWebControl1.NbElement==2){
					// Erreur : Nbre d'élément < 1000
					Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else{
					// Erreur : 1 élément
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(391,_webSession.SiteLanguage)+"\");");
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

		#region Boutton Enregistrer
		/// <summary>
		/// Bouton Enregistrer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{
				if(AdvertiserSelectionWebControl1.NbElement==1){				
					Int64 idUniverseClientDescription=-1;
					if(radioButtonHoldingCompany.Checked==true){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.HOLDING_ADVERTISER;
						
					}
					else if(radiobuttonAdvertiser.Checked==true){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.ADVERTISER_PRODUCT;
						
					}
					else if(RadiobuttonBrand.Checked==true) {
						idUniverseClientDescription=WebConstantes.LoadableUnivers.BRAND_PRODUCT;
						
					}
					else if(radiobuttonProduct.Checked==true){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.PRODUCT;
						
					}
					else if(RadiobuttonSector.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.SECTOR_SUBSECTOR;
					}
					else if(RadiobuttonSubSector.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.SUBSECTOR_GROUP;
					}
					else if(RadiobuttonGroup.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.GROUP;
					}					
					saveScript=WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, TNS.AdExpress.Constantes.Classification.Branch.type.product,idUniverseClientDescription);								
				}
				else if (AdvertiserSelectionWebControl1.NbElement==2){
					// Erreur : Nbre d'élément < 1000
					Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else{
					// Erreur : 1 élément
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(391,_webSession.SiteLanguage)+"\");");
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
		
		#region Boutton charger
		/// <summary>
		/// Bouton chargement
		/// </summary>
		protected bool loadUniversAdvertiser(){
			
			

			#region Variables
			string[] tabParent=null;
			Int64 idUniverse=0;	
			bool selectionnedUnivers=false;
			#endregion

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
				_webSession.SelectionUniversAdvertiser=treeNodeUniverse;
				_webSession.Save();
			
			
				#region Désactivation des boutons
				radioButtonHoldingCompany.Enabled=false;
				radioButtonHoldingCompany.Checked=false;
				RadiobuttonBrand.Enabled=false;
				RadiobuttonBrand.Checked=false;
				radiobuttonAdvertiser.Checked=false;
				radiobuttonAdvertiser.Enabled=false;
				radiobuttonProduct.Enabled=false;
				radiobuttonProduct.Checked=false;
				RadiobuttonSector.Enabled=false;
				RadiobuttonSubSector.Enabled=false;
				RadiobuttonGroup.Enabled=false;
				RadiobuttonSector.Checked=false;
				RadiobuttonSubSector.Checked=false;
				RadiobuttonGroup.Checked=false;
			
				if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {  
				
					radioButtonHoldingCompany.Enabled=true;
					radioButtonHoldingCompany.Checked=true;
				}			
			
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {  
					radiobuttonAdvertiser.Enabled=true;
					radiobuttonAdvertiser.Checked=true;

				}
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) {  
					RadiobuttonBrand.Enabled=true;
					RadiobuttonBrand.Checked=true;

				}
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {  
					radiobuttonProduct.Enabled=true;
					radiobuttonProduct.Checked=true;
				}
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {  
					RadiobuttonSector.Enabled=true;	
					RadiobuttonSector.Checked=true;
		
				}
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {  
					RadiobuttonSubSector.Enabled=true;
					RadiobuttonSubSector.Checked=true;
				}
				else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
					|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {  
					RadiobuttonGroup.Enabled=true;
					RadiobuttonGroup.Checked=true;

				}
				#endregion

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
			
			#region Désactivation des boutons
			
			radioButtonHoldingCompany.Enabled=false;
			radiobuttonAdvertiser.Enabled=false;
			RadiobuttonBrand.Enabled=false;
			radiobuttonProduct.Enabled=false;
			RadiobuttonSector.Enabled=false;
			RadiobuttonSubSector.Enabled=false;
			RadiobuttonGroup.Enabled=false;
			
			if(radioButtonHoldingCompany.Checked){
				radioButtonHoldingCompany.Enabled=true;				
			}
			else if(radiobuttonAdvertiser.Checked){
				radiobuttonAdvertiser.Enabled=true;				
			}
			else if(RadiobuttonBrand.Checked) {
				RadiobuttonBrand.Enabled=true;				
			}
			else if(radiobuttonProduct.Checked){
				radiobuttonProduct.Enabled=true;
			}
			else if(RadiobuttonSector.Checked){
				RadiobuttonSector.Enabled=true;				
			}
			else if(RadiobuttonSubSector.Checked){
				RadiobuttonSubSector.Enabled=true;
			}
			else if(RadiobuttonGroup.Checked){
				RadiobuttonGroup.Enabled=true;
			}
			
			#endregion

		}
		#endregion

		#region Fonction
		/// <summary>
		/// Place le focus sur le contrôle sélectionné
		/// </summary>
		/// <param name="champ">webcontrol sur lequel on veut mettre le focus</param>
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

		#region Nouvelle Recherche
		/// <summary>
		/// Nouvelle Recherche
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void initializeImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			
			#region Activation des boutons
			if(_webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE]!=null)
				RadiobuttonBrand.Enabled=true;
			if(_webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_HOLDING_COMPANY]!=null)
				radioButtonHoldingCompany.Enabled=true;
			
			//radioButtonHoldingCompany.Enabled=true;
			radiobuttonAdvertiser.Enabled=true;
			//RadiobuttonBrand.Enabled=true;
			radiobuttonProduct.Enabled=true;
			RadiobuttonSector.Enabled=true;
			RadiobuttonSubSector.Enabled=true;
			RadiobuttonGroup.Enabled=true;
			#endregion
			textBoxAdvertiserChoice.Text="";
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
//			this.validateButton_Click(sender,e);
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
