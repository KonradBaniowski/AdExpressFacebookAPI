#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//	30/12/2004 A. Obermeyer Intégration de WebPage
//	K.Shehzad 09/09/2005 modifications for the case when there no element is selected (chronopress)
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
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Sélection Produits pour les pages plans médias concurrentielles
	/// </summary>
	public partial class CompetitorAdvertiserSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
		/// <summary>
		/// Contrôle affichant le Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle En tête de page
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
		/// Texte : recherche par groupe de sociétés
		/// </summary>
		/// <summary>
		/// Bouton radion groupe de société
		/// </summary>
		/// <summary>
		/// Texte : recherche par annonceur
		/// </summary>
		/// <summary>
		/// Bouton radion annonceur
		/// </summary>
		/// <summary>
		/// Texte : recherche par référence
		/// </summary>
		/// <summary>
		/// bouton radio référence
		/// </summary>
		/// <summary>
		/// Sauvegarde univers
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
		/// Contrôle : liste des annonceurs 
		/// </summary>
		/// <summary>
		/// bouton ok
		/// </summary>
		/// <summary>
		/// Texte : recherche par famille
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdexpressSector;
		/// <summary>
		/// Bouton radio famille
		/// </summary>
		protected System.Web.UI.WebControls.RadioButton RadiobuttonSector;
		/// <summary>
		/// Texte : recherche par classe
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdexpresstextSubSector;
		/// <summary>
		/// Bouton radio classe
		/// </summary>
		protected System.Web.UI.WebControls.RadioButton RadiobuttonSubSector;
		/// <summary>
		/// Texte : recherche par groupe
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdexpresstextGroup;
		/// <summary>
		/// Bouton radio groupe
		/// </summary>
		protected System.Web.UI.WebControls.RadioButton RadiobuttonGroup;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <summary>
		/// Bouton suivant
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// identifiant univers
		/// </summary>
		/// <summary>
		/// Text
		/// </summary>
		/// <summary>
		/// Initialisation globale
		/// </summary>
		/// <summary>
		/// Chargement de l'univers
		/// </summary>
		/// <summary>
		/// Radio boutton marque
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// fermeture du flash d'attente
		/// </summary>
		public string divClose;		
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniverses="";
		/// <summary>
		/// Type d'évènement (bouton ok, valider...)
		/// </summary>
		protected int eventButton;
		/// <summary>
		/// texte
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
		/// Menu contextuel
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
		public CompetitorAdvertiserSelection():base(){						
		}
		#endregion

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

				#region Rights for brand and holding company
				//For controlling access to brands and holding company according to the right flags 
				if(!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				{
					RadiobuttonBrand.Enabled=false;
					AdexpressBrandText.Enabled=false;					
					cssBrand="txtGris11";
				}
				if(!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY))
				{
					radioButtonHoldingCompany.Enabled=false;
					AdExpressHolding.Enabled=false;
					cssHoldingCompany="txtGris11";
				}
				#endregion

				#region Flash pour patienter
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null)
				{
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=validateButton.ID && nomInput!=NextImageButtonRollOverWebControl.ID  && nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
						Page.Response.Write(LoadingSystem.GetHtmlCloseDiv());					
					}
				}
				#endregion			
			
				#region Langues et textes
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				_webSession.Save();
				initializeImageButtonRollOverWebControl1.Text=GestionWeb.GetWebWord(974,_webSession.SiteLanguage);
				initializeAllImageButtonRollOverWebControl.Text=GestionWeb.GetWebWord(979,_webSession.SiteLanguage);
				#endregion
 
				#region Evènemment
				// Connaître le boutton qui a été cliquer 
				eventButton=0;

				if(HttpContext.Current.Request.QueryString.Get("saveUnivers")!= null){
					eventButton=int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));
				
				}

				// Boutton valider
				if(Request.Form.Get("__EVENTTARGET")=="validateButton" ){
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
				else if(Request.Form.Get("__EVENTTARGET")=="initializeImageButtonRollOverWebControl1" || Request.Form.Get("__EVENTTARGET")=="initializeAllImageButtonRollOverWebControl" || Request.Form.Get("__EVENTTARGET")==null ){
					if(eventButton!=6){
						eventButton=-1;
					}
				}
					// bouton suivant
				else if(Request.Form.Get("__EVENTTARGET")=="NextImageButtonRollOverWebControl"){
					eventButton=-2;
				}
				#endregion

				#region Gestion affichage des options de sélection du niveau de nomenclature 
				if(_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE){
					ShowNomenclatureOptions(_webSession);
					if(!Page.IsPostBack)radiobuttonProduct.Checked=true;
					_webSession.CurrentUniversProduct =  new System.Windows.Forms.TreeNode("produit");
					_webSession.CurrentUniversAdvertiser=new System.Windows.Forms.TreeNode("advertiser");
				}
				#endregion
			
				#region Chargement univers
				// Bouton Charger 
				if(eventButton==4){
					if(!loadUniversAdvertiser()){
						eventButton=-1;
						this.loadImageButtonRollOverWebControl_Click(null,null);
					
					}
				}
				#endregion
			
				#region Initialisation
				if(Request.Form.Get("__EVENTTARGET")==null && eventButton!=6) {
					this.initializeAllImageButtonRollOverWebControl_Click(null,null);	 
				}
				#endregion

				#region Caractères accuentués
				// Remplace les caractères accuentués
				textBoxAdvertiserChoice.Text=TNS.AdExpress.Web.Functions.CheckedText.NewText(textBoxAdvertiserChoice.Text);
				#endregion

				#region Initialisation du composant Recall
				//if(IsPostBack || eventButton==6){
				if(IsPostBack || eventButton==3){			
					// Initialisation du composant liste des annonceurs
					AdvertiserSelectionWebControl1.HoldingCompanyBool=radioButtonHoldingCompany.Checked;
					AdvertiserSelectionWebControl1.AdvertiserBool=radiobuttonAdvertiser.Checked;
					AdvertiserSelectionWebControl1.BrandBool=RadiobuttonBrand.Checked;
					AdvertiserSelectionWebControl1.ProductBool=radiobuttonProduct.Checked;
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
//				if(_webSession.IsTargetSelected())linkToShow.Add(6);
//				RecallWebControl1.LinkToShow=linkToShow;
//				
//					//if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected() && _webSession.isDatesSelected())RecallWebControl1.CanGoToResult=true;
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"AdvertiserCompetitorSelectionHelp.aspx";
				#endregion

				#region Affichage des boutons Valider et Enregistrer
				//Boutons valider et Enregistrer invisible lors du premier affichage
				if(!IsPostBack){
					saveUniverseImageButtonRollOverWebControl.Visible=false;
					validateButton.Visible=false;
					
//					if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected() && _webSession.isDatesSelected())
//						//RecallWebControl1.CanGoToResult=false;
//						;

					NextImageButtonRollOverWebControl.Visible=false;
					AdExpressText6.Visible=false;
					universAdExpressText.Visible=false;
					idUnivers.Visible=false;
				}
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

				// bouton sauvegarder
				saveUniverseImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				saveUniverseImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
			
				// bouton suivant
				if(_webSession.CurrentModule != WebConstantes.Module.Name.BILAN_CAMPAGNE){
					NextImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/suivant_up.gif";
					NextImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/suivant_down.gif";
				}
				else{
					NextImageButtonRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/bt_enregistrer_univers_up.gif";
					NextImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/bt_enregistrer_univers_down.gif";
				}

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
			
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				// Gestion lorsqu'on clique sur entrée
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",TNS.AdExpress.Web.Functions.Script.TrapEnter("okButtonRollOverWebControl2"));
				}

				// Sélection/désélection de tous les fils (cas 2 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
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
	
				//Raccourci resultat bilan de campagne
//				if(_webSession.LastReachedResultUrl.Length>0 &&  _webSession.IsTargetSelected() && _webSession.isDatesSelected() && _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
//					;
//					//RecallWebControl1.CanGoToResult=true;

				//Annuler l'univers de version pour APPM
				if(_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE){
					_webSession.IdSlogans = new ArrayList();
                    _webSession.SloganIdZoom = long.MinValue;
					_webSession.Save();
				}
				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
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
			LoadableUniversWebControl1.ListBranchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser.GetHashCode().ToString()+","+TNS.AdExpress.Constantes.Classification.Branch.type.brand.GetHashCode().ToString();
			MenuWebControl2.CustomerWebSession = _webSession;
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
				// Affichage des boutons si existance d'annonceurs
				if(IsPostBack){
					if(AdvertiserSelectionWebControl1.DsListAdvertiser!=null){
						if(AdvertiserSelectionWebControl1.DsListAdvertiser.Tables[0].Rows.Count==0){
							saveUniverseImageButtonRollOverWebControl.Visible=false;
							NextImageButtonRollOverWebControl.Visible=false;
							validateButton.Visible=false;
//							if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected() && _webSession.isDatesSelected())
//								;//RecallWebControl1.CanGoToResult=false;
							AdExpressText6.Visible=false;
							universAdExpressText.Visible=false;
							idUnivers.Visible=false;

						}
						else{
							saveUniverseImageButtonRollOverWebControl.Visible=true;
							if(_webSession.CompetitorUniversAdvertiser.Count>=4 
								|| (_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE
								&& _webSession.CompetitorUniversAdvertiser.Count>=1
								)){
								NextImageButtonRollOverWebControl.Visible=false;
							
							}else{
								NextImageButtonRollOverWebControl.Visible=true;
							}
							if(_webSession.CompetitorUniversAdvertiser.Count>0 
								|| _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE){
								validateButton.Visible=true;
//								if(_webSession.LastReachedResultUrl.Length>0 &&
//									(_webSession.isMediaSelected() || _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
//									&& _webSession.isDatesSelected())
//									;//RecallWebControl1.CanGoToResult=true;
							}else{
								validateButton.Visible=false;
//								if(_webSession.LastReachedResultUrl.Length>0 && 
//									(_webSession.isMediaSelected()||_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
//									&& _webSession.isDatesSelected())
//									;//RecallWebControl1.CanGoToResult=false;
							}
							AdExpressText6.Visible=true;
							universAdExpressText.Visible=true;
							idUnivers.Visible=true;
						
						}
					}else{
						saveUniverseImageButtonRollOverWebControl.Visible=false;
						validateButton.Visible=false;	
//						if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected() && _webSession.isDatesSelected())
//							;//RecallWebControl1.CanGoToResult=false;
						AdExpressText6.Visible=false;
						NextImageButtonRollOverWebControl.Visible=false;
						universAdExpressText.Visible=false;
						idUnivers.Visible=false;
					}


				}

				// Bouton Charger
				//if(eventButton==4 || eventButton==6){
				if(eventButton==4 || eventButton==3){
					saveUniverseImageButtonRollOverWebControl.Visible=true;
					AdExpressText6.Visible=true;
					if(_webSession.CompetitorUniversAdvertiser.Count>=4 || 
						(_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE
						&& _webSession.CompetitorUniversAdvertiser.Count>=1
						)){
						NextImageButtonRollOverWebControl.Visible=false;
					}else{
						NextImageButtonRollOverWebControl.Visible=true;
					}
					if(_webSession.CompetitorUniversAdvertiser.Count>0 ||
						_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE
						){
						validateButton.Visible=true;
//						if(_webSession.LastReachedResultUrl.Length>0 && 
//							(_webSession.isMediaSelected() || _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
//							&& _webSession.isDatesSelected())
//							;//RecallWebControl1.CanGoToResult=true;
					}else{
						validateButton.Visible=false;
//						if(_webSession.LastReachedResultUrl.Length>0 &&
//							(_webSession.isMediaSelected()||_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
//							&& _webSession.isDatesSelected())
//							;//RecallWebControl1.CanGoToResult=false;
					}

					universAdExpressText.Visible=true;
					idUnivers.Visible=true;

				}

				//Contrôle Recall
				if(eventButton==5){
					this.validateButton_Click(null,null);
				}
			
				//if(eventButton==6){
				if(eventButton==3){
					#region Désactivation des boutons
					radioButtonHoldingCompany.Enabled=false;
					radiobuttonAdvertiser.Enabled=false;
					RadiobuttonBrand.Enabled=false;
					radiobuttonProduct.Enabled=false;
					radioButtonHoldingCompany.Checked=false;
					radiobuttonAdvertiser.Checked=false;
					radiobuttonProduct.Checked=false;
					RadiobuttonBrand.Checked=false;

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
					}
					#endregion

					// Affichage du nom Univers
					if(_webSession.CurrentModule != WebConstantes.Module.Name.BILAN_CAMPAGNE){
						int number=_webSession.CompetitorUniversAdvertiser.Count+1;
						idUnivers.Text=""+GestionWeb.GetWebWord(977,_webSession.SiteLanguage)+""+number.ToString();
					}
					else{
						if(_webSession.CompetitorUniversAdvertiser.Count>=1){
							idUnivers.Text=""+GestionWeb.GetWebWord(1678,_webSession.SiteLanguage);
						}
						else{
							idUnivers.Text=""+GestionWeb.GetWebWord(1677,_webSession.SiteLanguage);
						}
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

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
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

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
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
				// no element selected
				if(AdvertiserSelectionWebControl1.NbElement==3 && _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
				{
					Response.Write("<script language=javascript>");
					Response.Write("alert(\""+GestionWeb.GetWebWord(391,_webSession.SiteLanguage)+"\");");
					Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
				else
				{
					if(idUnivers.Text.Length>0)
					{
						if(_webSession.CompetitorUniversAdvertiser[idUnivers.Text]==null)
						{
							if(_webSession.CompetitorUniversAdvertiser.Count<5)
							{
								if(AdvertiserSelectionWebControl1.NbElement==1 || AdvertiserSelectionWebControl1.NbElement==3)
								{

									if(_webSession.CompetitorUniversAdvertiser.Count>=1 || _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
									{
										if(AdvertiserSelectionWebControl1.NbElement!=3)
										{
											int i=1;
											while(_webSession.CompetitorUniversAdvertiser[i]!=null)
											{

												if(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[i]).NameCompetitorAdvertiser==idUnivers.Text)
												{
													idUnivers.Text=idUnivers.Text+"-"+_webSession.CompetitorUniversAdvertiser.Count.ToString();
													i=0;
												}					
												i++;
											}
									
											idUnivers.Text=idUnivers.Text.Replace("'"," ");
											int numberCompetitorAdvertiser=_webSession.CompetitorUniversAdvertiser.Count+1;
											_webSession.CompetitorUniversAdvertiser.Add(numberCompetitorAdvertiser,new TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser(idUnivers.Text,(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone()));
											_webSession.Save();		
											_webSession.Source.Close();
											Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
											//Response.Redirect("VehicleSelection.aspx?idSession="+_webSession.IdSession+"");
										}
										else if(_webSession.CompetitorUniversAdvertiser.Count==1 && AdvertiserSelectionWebControl1.NbElement==3)
										{
											// Erreur : 1 élément
											Response.Write("<script language=javascript>");
											Response.Write("alert(\""+GestionWeb.GetWebWord(391,_webSession.SiteLanguage)+"\");");
											Response.Write("history.go(-1);");
											Response.Write("</script>");
										}
										else 
										{
											_webSession.Source.Close();
											Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
										}
									}
										// Sélectionner au moins 2 univers
									else
									{
										Response.Write("<script language=javascript>");
										Response.Write("alert(\""+GestionWeb.GetWebWord(989,_webSession.SiteLanguage)+"\");");
										Response.Write("history.go(-1);");
										Response.Write("</script>");
									}
								}
								else if (AdvertiserSelectionWebControl1.NbElement==2)
								{
									// Erreur : Nbre d'élément < 1000
									Response.Write("<script language=javascript>");
                                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
									Response.Write("history.go(-1);");
									Response.Write("</script>");
								}
							}
							else
							{							
								Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
							}
						}
							//Nom d'univers déjà existant
						else
						{
							Response.Write("<script language=javascript>");
							Response.Write("alert(\""+GestionWeb.GetWebWord(923,_webSession.SiteLanguage)+"\");");
							Response.Write("history.go(-1);");
							Response.Write("</script>");
						}
					}
						// Text univers null
					else
					{
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(976,_webSession.SiteLanguage)+"\");");
						Response.Write("history.go(-1);");
						Response.Write("</script>");
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

		#region Boutton Enregistrer
		/// <summary>
		/// Bouton Enregistrer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{

				TNS.AdExpress.Constantes.Classification.Branch.type branchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
				if(AdvertiserSelectionWebControl1.NbElement==1){
			
					Int64 idUniverseClientDescription=-1;

					if(radioButtonHoldingCompany.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.HOLDING_ADVERTISER;
						branchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
					}
					else if(radiobuttonAdvertiser.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.ADVERTISER_PRODUCT;
						branchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
					}
					else if(RadiobuttonBrand.Checked)
					{
						idUniverseClientDescription=WebConstantes.LoadableUnivers.BRAND_PRODUCT;
						branchType=TNS.AdExpress.Constantes.Classification.Branch.type.brand;
					}
					else if(radiobuttonProduct.Checked){
						idUniverseClientDescription=WebConstantes.LoadableUnivers.PRODUCT;
						branchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
					}
				
					//	Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"saveUnivers","<script language=\"JavaScript\" type=\"text/JavaScript\"> window.open('/Private/Universe/UniverseSavePopUp.aspx?idSession="+_webSession.IdSession+"', '', \"toolbar=0, directories=0, status=0, menubar=0, width=450, height=300, scrollbars=1, location=0, resizable=1\");</script>");
					// saveScript="<script language=\"JavaScript\" type=\"text/JavaScript\"> window.open('/Private/Universe/UniverseSavePopUp.aspx?idSession="+_webSession.IdSession+"&brancheType="+TNS.AdExpress.Constantes.Classification.Branch.type.advertiser+"', '', \"toolbar=0, directories=0, status=0, menubar=0, width=450, height=300, scrollbars=1, location=0, resizable=1\");</script>";
					saveScript=WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType,idUniverseClientDescription);					
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
					System.Windows.Forms.TreeNode treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
					_webSession.SelectionUniversAdvertiser=treeNodeUniverse;
					_webSession.Save();
					idUnivers.Text=TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetUniverse(idUniverse,_webSession);
				
					int i=1;
					selectionnedUnivers=true;
					while(_webSession.CompetitorUniversAdvertiser[i]!=null){

						if(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[i]).NameCompetitorAdvertiser==idUnivers.Text){
							idUnivers.Text=idUnivers.Text+"-"+_webSession.CompetitorUniversAdvertiser.Count.ToString();
							i=0;
						}					
						i++;
					}

					#region Désactivation des boutons
					radioButtonHoldingCompany.Checked=false;
					radiobuttonAdvertiser.Checked=false;
					RadiobuttonBrand.Checked=false;
					radiobuttonProduct.Checked=false;
					radioButtonHoldingCompany.Enabled=false;
					radiobuttonAdvertiser.Enabled=false;
					RadiobuttonBrand.Enabled=false;
					radiobuttonProduct.Enabled=false;

					if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {  
						radioButtonHoldingCompany.Checked=true;
						radioButtonHoldingCompany.Enabled=true;
					}			
			
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {  
						radiobuttonAdvertiser.Checked=true;
						radiobuttonAdvertiser.Enabled=true;

					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
					{  
						RadiobuttonBrand.Checked=true;
						RadiobuttonBrand.Enabled=true;

					}
					else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
						|| ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {  
						radiobuttonProduct.Checked=true;
						radiobuttonProduct.Enabled=true;
					}	
					#endregion
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
			if(radioButtonHoldingCompany.Checked){
				radioButtonHoldingCompany.Enabled=true;				
			}
			else if(radiobuttonAdvertiser.Checked){
				radiobuttonAdvertiser.Enabled=true;				
			}
			else if(RadiobuttonBrand.Checked)
			{
				RadiobuttonBrand.Enabled=true;				
			}
			else if(radiobuttonProduct.Checked){
				radiobuttonProduct.Enabled=true;
			}

			#endregion

			// Affichage du nom Univers
			if(_webSession.CurrentModule != WebConstantes.Module.Name.BILAN_CAMPAGNE){
				int number=_webSession.CompetitorUniversAdvertiser.Count+1;
				idUnivers.Text=""+GestionWeb.GetWebWord(977,_webSession.SiteLanguage)+""+number.ToString();
			}
			else{
				if(_webSession.CompetitorUniversAdvertiser.Count>=1){
					idUnivers.Text=""+GestionWeb.GetWebWord(1678,_webSession.SiteLanguage);
				}
				else{
					idUnivers.Text=""+GestionWeb.GetWebWord(1677,_webSession.SiteLanguage);
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

		#region Nouvelle Recherche
		/// <summary>
		/// Nouvelle Recherche
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void initializeImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			
			#region Activation des boutons
			if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				RadiobuttonBrand.Enabled=true;
			if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY))
				radioButtonHoldingCompany.Enabled=true;
			
			//radioButtonHoldingCompany.Enabled=true;
			radiobuttonAdvertiser.Enabled=true;
			//RadiobuttonBrand.Enabled=true;
			radiobuttonProduct.Enabled=true;
			#endregion
			
			textBoxAdvertiserChoice.Text="";
		}
		#endregion

		#region Bouton suivant
		/// <summary>
		/// Bouton suivant
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void NextImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
				if(idUnivers.Text.Length>0){
				
					if(_webSession.CompetitorUniversAdvertiser[idUnivers.Text]==null){
						if(AdvertiserSelectionWebControl1.NbElement==1){
						

							int i=1;
							while(_webSession.CompetitorUniversAdvertiser[i]!=null){

								if(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[i]).NameCompetitorAdvertiser==idUnivers.Text){
									idUnivers.Text=idUnivers.Text+"-"+_webSession.CompetitorUniversAdvertiser.Count.ToString();
									i=0;
								}					
								i++;
							}
							idUnivers.Text=idUnivers.Text.Replace("'"," ");
							int numberCompetitorAdvertiser=_webSession.CompetitorUniversAdvertiser.Count+1;
							_webSession.CompetitorUniversAdvertiser.Add(numberCompetitorAdvertiser,new TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser(idUnivers.Text,(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone()));
							_webSession.Save();

							#region Activation des boutons
							if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
								RadiobuttonBrand.Enabled=true;
							if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY))
								radioButtonHoldingCompany.Enabled=true;
			
							//radioButtonHoldingCompany.Enabled=true;
							radiobuttonAdvertiser.Enabled=true;
							
							//RadiobuttonBrand.Enabled=true;
							radiobuttonProduct.Enabled=true;
							#endregion

							textBoxAdvertiserChoice.Text="";
					
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
					}else{
						// Erreur : Univers porte déjà ce nom
						Response.Write("<script language=javascript>");
						Response.Write("alert(\""+GestionWeb.GetWebWord(923,_webSession.SiteLanguage)+"\");");
						Response.Write("history.go(-1);");
						Response.Write("</script>");
					}
				}
				else{
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(976,_webSession.SiteLanguage)+"\");");
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

		#region Remise à zéro
		/// <summary>
		/// Remise à zéro de la hashTable competitorUniversAdvertiser
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void initializeAllImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{
				#region Activation des boutons
				//holding company rights
				if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY))								
					radioButtonHoldingCompany.Enabled=true;
				radiobuttonAdvertiser.Enabled=true;
				//Brand rights
				if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))						
					RadiobuttonBrand.Enabled=true;
				radiobuttonProduct.Enabled=true;
				#endregion
			
				textBoxAdvertiserChoice.Text="";			
				// On vide la hashTable contenant les arbres
				_webSession.CompetitorUniversAdvertiser.Clear();
				_webSession.Save();
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
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

		#region Selection level options
		/// <summary>
		/// Selection level configuration depending on the module
		/// </summary>
		/// <param name="_webSession">Client Session</param>
		private void ShowNomenclatureOptions(WebSession _webSession){
			switch(_webSession.CurrentModule){
				case WebConstantes.Module.Name.BILAN_CAMPAGNE :
					//Radio buttons
					radioButtonHoldingCompany.Visible=false;

					//Radio buttons labels
					AdExpressHolding.Visible=false;
					
					break;
				default : 
					break;
			}
		}
		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			//this.validateButton_Click(sender,e);
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
