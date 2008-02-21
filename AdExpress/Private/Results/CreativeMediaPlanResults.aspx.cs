#region Informations
// Auteur: G. Facon
// Date de cr�ation: 25/10/2005
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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page de calendrier d'action d'un plan media
	/// </summary>
	public partial class CreativeMediaPlanResults : TNS.AdExpress.Web.UI.ResultWebPage{
		
		#region Variables
		/// <summary>
		/// Identifiant Session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Liste d'annonceurs
		/// </summary>
		protected string listAdvertiser="";
		/// <summary>
		/// Script qui g�re la s�lection des annonceurs
		/// </summary>
		public string advertiserScript;
		/// <summary>
		/// Texte de l'option "Tout s�lectionner"
		/// </summary>
		public string allChecked;		
//		/// <summary>
//		/// Script de fermeture du flash d'attente
//		/// </summary>
		//public string divClose=LoadingSystem.GetHtmlCloseDiv();		
		/// <summary>
		/// Capture de l'�v�nement responsbale du postBack
		/// </summary>
		protected int  eventButton=0;
		#endregion

		#region Variable MMI
		/// <summary>
		/// Contr�le de texte "Votre s�lection annonceurs"
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Rappel de la s�lection
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CreativeMediaPlanResults():base(){
			idsession=HttpContext.Current.Request.QueryString.Get("idSession");
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
		
				#region Flash d'attente
//				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
//					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
//				}
//				else{
//					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//					Page.Response.Flush();
//				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
				creativeSelectionWebControl.CustomerSession=_webSession;
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				advertiserScript= TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript();
				allChecked=GestionWeb.GetWebWord(856,_webSession.SiteLanguage);			
				#endregion
	

				#region MAJ de la Session
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion
			
				#region Gestion du Contr�le advertiserSelection
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

				#endregion

				#region Calcul du r�sultat
				if(eventButton==9){
					result=MediaPlanAnalysisUI.GetCreativeMediaPlanAnalysisHtmlUI(this,MediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
				}
				#endregion				
			
				#region Script
				// Ouverture/fermeture des fen�tres p�res
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}

				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}

				// S�lection/d�s�lection de tous les fils (cas 2 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
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
				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					//this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}

		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Initialisation
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){    
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// D�termine la valeur de PostBack
		/// Initialise la propri�t� CustomerSession des composants "options de r�sultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

		#region Valider une sous-s�lection d'annonceur
		/// <summary>
		/// Bouton valider au niveau de la sous s�lection d'annonceurs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void validSubSelectionImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		}
		#endregion

		#region Retour � la s�lection originale
		/// <summary>
		/// Gestion du lien retour s�lection initiale
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void firstSubSelectionLinkButton_Click(object sender, System.EventArgs e) {
			try{
				//Copie de l'arbre globale dans le le current
				_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone();
				//Affichage du r�sultat
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
		/// PreRendu De la page
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e){
			try{
				if (IsPostBack){
					if(eventButton==7){	
						if(_webSession.CurrentUniversAdvertiser.FirstNode!=null){
							_webSession.Save();
							// Calcul du r�sultat
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
			return ("");
		}
		#endregion

	}
}
