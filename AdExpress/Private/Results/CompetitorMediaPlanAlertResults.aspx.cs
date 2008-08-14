#region Informations
// Auteur: A. Obermeyer
// Date de cr�ation: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Int�gration de WebPage
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
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{

	/// <summary>
	/// Affichage de la page de r�sultat Alerte Plan Media Concurrentiel
	/// </summary>
	public partial class CompetitorMediaPlanAlertResults : TNS.AdExpress.Web.UI.ResultWebPage{

		#region Variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options des r�sultats
		/// </summary>
		/// <summary>
		/// Contr�le passerelle vres les autres modules
		/// </summary>
		/// <summary>
		/// Contr�le menu d'ent�te 
		/// </summary>
		/// <summary>
		/// Contr�le validation d'une sous-s�lection d'annonceurs
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl validSubSelectionImageButtonRollOverWebControl;
		/// <summary>
		/// S2lection des annonceurs
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Selections.AdvertiserSelectionWebControl AdvertiserSelectionWebControl1;

		#endregion
					
		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Liste des annonceurs s�lectionn�s
		/// </summary>
		protected string listAdvertiserUI="";
		/// <summary>
		/// Script de gestion du rappel annonceur
		/// </summary>
		public string scriptAdvertiserRemind;		
		/// <summary>
		/// Contr�le rappel de la s�lection initiale
		/// </summary>
		protected System.Web.UI.WebControls.LinkButton firstSubSelectionLinkButton;
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Bouton qui lance l'�v�nement
		/// </summary>
		protected int  eventButton=0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public CompetitorMediaPlanAlertResults():base(){
		// Chargement de la Session
			try{				
				idsession=HttpContext.Current.Request.QueryString.Get("idSession");
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
				Response.Flush();
			}
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page : 
		///		Flash d'attente
		///		Initialisation des connections � la BD
		///		Redirection en cas changement d'un crit�re de s�lection
		///		Traduction du site
		///		Extraction du code HTML r�pondant � la s�lection utilisateur
		///		MAJ dans la session de la derni�re page de r�sultats atteinte lors de la navigation
		/// </summary>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public static string LoadingSystem.GetHtmlDiv(int language,Page page)
		///		public static void TNS.AdExpress.Web.DataAccess.Functions.closeDataBase(WebSession _webSession)
		///		public static void TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(System.Web.UI.ControlCollection list, int language)
		///		public static string TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript()
		///		public	static string TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(System.Windows.Forms.TreeNode root,bool write,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,bool AllSelection,int SiteLanguage)
		///		public static string TNS.AdExpress.Web.UI.Results.MediaPlanAlertUI.GetMediaPlanAlertUI(Page page,object[,] tab,WebSession _webSession, string zoomDate, string url)
		///		public static object[,] TNS.AdExpress.Web.Rules.Results.MediaPlanAlertRules.GetFormattedTable(WebSession _webSession, string beginningPeriod, string endPeriod)
		///		public static void TNS.AdExpress.Web.Core.Sessions.WebSession.Save()
		/// </remarks>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
		
			try{
			
				#region Gestion du flash d'attente
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
			
				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;
				#endregion
			
				#region D�finition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"CompetitorMediaPlanAlertResultsHelp.aspx";
				#endregion
				
				#region Gestion du Contr�le advertiserSelection
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

				#region R�sultat
				//Code html des r�sultats
				if(eventButton==9){
                    //result=TNS.AdExpress.Web.UI.Results.CompetitorMediaPlanAlertUI.GetMediaPlanAlertUI(this,CompetitorMediaPlanAlertRules.GetFormattedTable(_webSession,_webSession.PeriodBeginningDate, _webSession.PeriodEndDate),_webSession);
				}
				#endregion

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion
		
		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'�v�nement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion
	
		#region Validation de la Sous-s�lection d'annonceur
		/// <summary>
		/// Bouton valider au niveau de la sous s�lection d'annonceurs
		/// </summary>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public	static string TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(System.Windows.Forms.TreeNode root,bool write,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,bool AllSelection,int SiteLanguage)
		///		public static string TNS.AdExpress.Web.UI.Results.MediaPlanAlertUI.GetMediaPlanAlertUI(Page page,object[,] tab,WebSession _webSession, string zoomDate, string url)
		///		public static object[,] TNS.AdExpress.Web.Rules.Results.MediaPlanAlertRules.GetFormattedTable(WebSession _webSession, string beginningPeriod, string endPeriod)
		///		public static void TNS.AdExpress.Web.Core.Sessions.WebSession.Save()
		/// </remarks>
		/// <param name="sender">Objet responsable de l'�v�nement</param>
		/// <param name="e">Param�tres</param>
		private void validSubSelectionImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		}
		#endregion

		#region Retourner � la s�lection originale
		/// <summary>
		/// Gestion du lien retour s�lection initiale
		/// </summary>
		/// <remarks>
		/// Utilise les m�thjodes
		///		public	static string TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(System.Windows.Forms.TreeNode root,bool write,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,bool AllSelection,int SiteLanguage)
		///		public static string TNS.AdExpress.Web.UI.Results.MediaPlanAlertUI.GetMediaPlanAlertUI(Page page,object[,] tab,WebSession _webSession, string zoomDate, string url)
		///		public static object[,] TNS.AdExpress.Web.Rules.Results.MediaPlanAlertRules.GetFormattedTable(WebSession _webSession, string beginningPeriod, string endPeriod)
		///		public static void TNS.AdExpress.Web.Core.Sessions.WebSession.Save()
		/// </remarks>
		/// <param name="sender">Objet responsable de l'�v�nement</param>
		/// <param name="e">Param�tres</param>
		private void firstSubSelectionLinkButton_Click(object sender, System.EventArgs e) {
			try{
				//Copie de l'arbre globale dans le le current
				_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone();
				//Affichage du r�sultat
				result=MediaPlanAlertUI.GetMediaPlanAlertUI(this,MediaPlanAlertRules.GetFormattedTable(_webSession,_webSession.PeriodBeginningDate, _webSession.PeriodEndDate),_webSession);
				_webSession.Save();
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}		
		#endregion

		#region OnPreRender
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
							// Calcul du r�sultat
							result=MediaPlanAlertUI.GetMediaPlanAlertUI(this,MediaPlanAlertRules.GetFormattedTable(_webSession,_webSession.PeriodBeginningDate, _webSession.PeriodEndDate),_webSession);
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

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{    
			this.Unload += new System.EventHandler(this.Page_UnLoad);
           
		}
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
