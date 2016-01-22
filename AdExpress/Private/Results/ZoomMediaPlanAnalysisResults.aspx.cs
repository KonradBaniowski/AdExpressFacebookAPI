#region Informations
// Auteur: G. Facon
// Date de cr�ation: 20/04/2006 (Nouvelle version) 
// Date de modification: 
#endregion

#region using
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
using ModuleName=TNS.AdExpress.Constantes.Web.Module.Name;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using BusinessFacadeResults=TNS.AdExpress.Web.BusinessFacade.Results;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page de zoom sur un plan m�dia
	/// </summary>
	public partial class ZoomMediaPlanAnalysisResults : TNS.AdExpress.Web.UI.PrivateWebPage{	
	
		#region Variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options de r�sultats (unit�, tableau)
		/// </summary>
		/// <summary>
		/// Contr�le menu d'ent�te
		/// </summary>
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession="";		
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();

		#endregion

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// P�riode de zoom (semaine, mois)
		/// </summary>
		public string zoomDate="";
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// link to the main page of plan media
		/// </summary>
		public string href="/Private/Results/MediaPlanResults.aspx?idSession";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ZoomMediaPlanAnalysisResults():base(){			
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
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

				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion
				
				#region Textes et langage AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

                #region Script
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenCreatives", WebFunctions.Script.OpenCreatives());
                #endregion

				#region D�finition de la page d'aide
				//helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ZoomMediaPlanAnalysisResultsHelp.aspx";
				#endregion

				#region R�sultat
				try{
                    this._dataSource = _webSession.Source;
					if(_webSession.CurrentModule==ModuleName.BILAN_CAMPAGNE){
						href="/Private/Results/APPMResults.aspx?idSession";	
						ResultsOptionsWebControl1.UnitOption=false;
						ResultsOptionsWebControl1.Visible=false;
					}
					
					result = BusinessFacadeResults.MediaPlanSystem.GetHtml(this,this._webSession,this._dataSource,Page.Request.QueryString.Get("zoomDate"),Page.Request.Url.AbsolutePath);
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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//Initialisation de la propro�t� session de chaque composant
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			zoomDate=Page.Request.QueryString.Get("zoomDate");

			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;
			MenuWebControl2.ForceHelp = WebConstantes.Links.HELP_FILE_PATH+"ZoomMediaPlanAnalysisResultsHelp.aspx";
			MenuWebControl2.ForcePrint = "/Private/Results/Excel/ZoomMediaPlanAnalysisResults.aspx?idSession=" + this._webSession.IdSession +  "&zoomDate=" + zoomDate;
			if(_webSession.CurrentModule!=ModuleName.BILAN_CAMPAGNE)
			MenuWebControl2.ForceExcelUnit = "/Private/Results/ValueExcel/ZoomMediaPlanAnalysisResults.aspx?idSession=" + this._webSession.IdSession +  "&zoomDate=" + zoomDate;
			return tmp;
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
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

	}
}
