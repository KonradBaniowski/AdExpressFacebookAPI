#region Informations
// Auteur: G. Facon
// Date de cr�ation: 12/06/2006 
// Date de modification: 
#endregion

#region Using
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
using BusinessFacadeResults=TNS.AdExpress.Web.BusinessFacade.Results;
#endregion

namespace AdExpress.Private.Results.ValueExcel{
	/// <summary>
	/// Description r�sum�e de selectionModule.
	/// </summary>
	public partial class ZoomMediaPlanAnalysisResults :  TNS.AdExpress.Web.UI.PrivateWebPage{	
		
		#region Variables
		//		/// <summary>
		//		/// Session du client
		//		/// </summary>
		//		protected WebSession _webSession;
		/// <summary>
		/// Contr�le txt titre du module
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleTitleWebControl Moduletitlewebcontrol2;
		/// <summary>
		/// Contr�le optrions de r�sultats (unit�, tableau)
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ResultsOptionsWebControl ResultsOptionsWebControl1;
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Contr�le txt description du r�sultat du module
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText mainDescAdexpresstext;
		/// <summary>
		/// Contr�le menu d'ent�te
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HeaderWebControl HeaderWebControl1;
		//		/// <summary>
		//		/// Langage du site
		//		/// </summary>
		//		public int _siteLanguage;
		#endregion

		#region Ev�nements

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ZoomMediaPlanAnalysisResults():base(){
			// Chargement de la Session
			_webSession=(WebSession)WebSession.Load(HttpContext.Current.Request.QueryString.Get("idSession"));
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

			#region Textes et langage du site
			//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
			//Langue du site
			_siteLanguage=_webSession.SiteLanguage;
			#endregion

			#region R�sultats
			try{
				result = BusinessFacadeResults.MediaPlanSystem.GetExcelUI(this,this._webSession,this._dataSource,Page.Request.QueryString.Get("zoomDate"),Page.Request.Url.AbsolutePath,true);
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
			}
			#endregion

		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){
            _webSession.Source.Close();
		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// DeterminePostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
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

	}
}
