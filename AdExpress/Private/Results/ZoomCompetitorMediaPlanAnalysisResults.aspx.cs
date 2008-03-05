#region Informations
// Auteur:
// Date de création: 
// Date de modification: 
//		03/01/2005 A. Obermeyer Intégration de WebPage
//		10/08/2006 - G Ragneau - Contextual menu integration
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

using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{
	/// <summary>
	/// ZoomCompetitorMediaPlanAnalysisResults display monthly competitor analysis detail 
	/// </summary>
	public partial class ZoomCompetitorMediaPlanAnalysisResults : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region variables MMI
		/// <summary>
		/// Contrôle Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle Options de résultats (unité, tableau)
		/// </summary>
		/// <summary>
		/// Contrôle txt desciption du résultat
		/// </summary>
		/// <summary>
		/// Contrôle menu d'entête
		/// </summary>
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region Variables	
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession="";
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Période de zoom (semaine, mois)
		/// </summary>
		public string zoomDate="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ZoomCompetitorMediaPlanAnalysisResults():base(){			
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
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

				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion
				
				#region Textes et langage AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region Résultat
				try{
					result=CompetitorMediaPlanAlertUI.GetMediaPlanAlertUI(this,CompetitorMediaPlanAlertRules.GetFormattedTable(_webSession, Page.Request.QueryString.Get("zoomDate")),_webSession,Page.Request.QueryString.Get("zoomDate"),Page.Request.Url.AbsolutePath);
				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
				}
				#endregion
			
				#region MAJ Session
				//Sauvegarde de la session
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

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){		
		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// DeterminePostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();

			zoomDate=Page.Request.QueryString.Get("zoomDate");

			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
		
			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;
			MenuWebControl2.ForceHelp = WebConstantes.Links.HELP_FILE_PATH+"ZoomMediaCompetitorPlanAnalysisResultsHelp.aspx";
			MenuWebControl2.ForcePrint = "/Private/Results/Excel/ZoomCompetitorMediaPlanAnalysisResults.aspx?idSession=" + this._webSession.IdSession + "&zoomDate=" + zoomDate;

			return tmp;
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
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

	
	}
}
