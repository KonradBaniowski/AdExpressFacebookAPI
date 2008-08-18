#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page de résultat Plan Média Concurrentiel
	/// </summary>
	public partial class CompetitorMediaPlanResults : TNS.AdExpress.Web.UI.ResultWebPage{

		#region Variables
		/// <summary>
		/// Identifiant Session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Liste d'annonceurs
		/// </summary>
		protected string listAdvertiser="";
		/// <summary>
		/// Script qui gère la sélection des annonceurs
		/// </summary>
		public string advertiserScript;
		/// <summary>
		/// Texte de l'option "Tout sélectionner"
		/// </summary>
		public string allChecked;		
		/// <summary>
		/// Bouton "Revenir à la sélection originale"
		/// </summary>
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();		
		/// <summary>
		/// Bouton évènement
		/// </summary>
		protected int  eventButton=0;
		#endregion

		#region Variable MMI
		/// <summary>
		/// Contrôle du titre du module
		/// </summary>
		/// <summary>
		/// Contrôle des options d'analyse 
		///</summary>
		/// <summary>
		/// Contrôle de la navigation inter module (n'est pas utilisé)
		/// </summary>
		/// <summary>
		/// Contrôle de texte "Votre sélection annonceurs"
		/// </summary>
		/// <summary>
		/// Contrôle du bouton de la sous sélection
		/// </summary>
		/// <summary>
		/// Contrôle du header d'AdExpress
		/// </summary>
		/// <summary>
		/// Contrôle Annonceurs
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorMediaPlanResults():base(){
			idsession=HttpContext.Current.Request.QueryString.Get("idSession");
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

				#region Flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				#region Url Suivante
//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+idsession);
				}			
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
			
				advertiserScript= TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript();
				allChecked=GestionWeb.GetWebWord(856,_webSession.SiteLanguage);	
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;
				//Bouton valider dans la sous sélection
				validSubSelectionImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				validSubSelectionImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
				//Lien Retour arbre initial
				firstSubSelectionLinkButton.Text=GestionWeb.GetWebWord(877,_webSession.SiteLanguage);
				#endregion
				
				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"CompetitorMediaPlanResultsHelp.aspx";
				#endregion

				#region MAJ de la Session
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion
			
				#region Gestion du Contrôle advertiserSelection
				//Bouton valider
				if(Request.Form.Get("__EVENTTARGET")=="validSubSelectionImageButtonRollOverWebControl"){
					eventButton=7;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="firstSubSelectionLinkButton"){
					eventButton=8;
				}
				else{
					eventButton=9;
				}

				AdvertiserSelectionWebControl1.ButtonTarget=eventButton;
				AdvertiserSelectionWebControl1.WebSession=_webSession;
				//MAJ DM
				//if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException){
				//    AdvertiserSelectionWebControl1.HoldingCompanyBool=true;
				//}
				//else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException){
				//    AdvertiserSelectionWebControl1.AdvertiserBool=true;
				//}
				//else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException){
				//    AdvertiserSelectionWebControl1.ProductBool=true;
				//}
				//else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException){
				//    AdvertiserSelectionWebControl1.SectorBool=true;
				//}
				//else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException){
				//    AdvertiserSelectionWebControl1.SubSectorBool=true;
				//}
				//else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException){
				//    AdvertiserSelectionWebControl1.GroupBool=true;
				//}
				#endregion

				#region Calcul du résultat		
				if(eventButton==9){
					result=CompetitorMediaPlanAnalysisUI.GetMediaPlanAnalysisHtmlUI(this,CompetitorMediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
				}
				#endregion
			
				#region Script
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
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

		#region Initialisation
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
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
		
		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de PostBack
		/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Valider une sous-sélection d'annonceur
		/// <summary>
		/// Bouton valider au niveau de la sous sélection d'annonceurs
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void validSubSelectionImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {	
		}
		#endregion

		#region Retour à la sélection originale
		/// <summary>
		/// Gestion du lien retour sélection initiale
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void firstSubSelectionLinkButton_Click(object sender, System.EventArgs e) {
			try{
				//Copie de l'arbre globale dans le le current
				_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone();
				//Affichage du résultat
				result=MediaPlanAnalysisUI.GetMediaPlanAnalysisHtmlUI(this,MediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
				_webSession.Save();
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
		protected override void OnPreRender(EventArgs e){
			try{
				if (IsPostBack){
					if(eventButton==7){	
						if(_webSession.CurrentUniversAdvertiser.FirstNode!=null){
							_webSession.Save();
							// Calcul du résultat
							result=MediaPlanAnalysisUI.GetMediaPlanAnalysisHtmlUI(this,MediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
						}else{
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(878,_webSession.SiteLanguage)+"\");");					
							Response.Write("</script>");
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

		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return MenuWebControl2.NextUrl;
		}
		#endregion
	}
}
