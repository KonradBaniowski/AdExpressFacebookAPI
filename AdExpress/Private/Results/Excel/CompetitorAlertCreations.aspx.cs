#region Informations
// Auteur: 
// Date de création: 
// Date de modification: 
//		19/12/2004	A. Obermeyer	Intégration de WebPage
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Description résumée de CompetitorAlertCreationsExcel.
	/// </summary>
	public partial class CompetitorAlertCreations : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code html du résultat
		/// </summary>
		public string result = "";		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorAlertCreations():base(){
					
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page:
		///		Chargement de la session
		///		Initialisation de la connection à la base de données
		///		Traduction du site
		///		Génération du code HTML:
		///			Extraction des paramètres de l'URL : ids ==> [vehicle, category, media]
		///			Extraction de zoomDate : Si le paramètre est présent, on effectue les calculs sur la période 
		///			en tenant compte du type de période en session. Si zoomDate est absent dans l'URL on tient 
		///			compte des périodes présentes en session	
		/// </summary>
		/// <remarks>
		/// Page_Load ==> public static string getMediaInsertionsCreationsExcelUI(WebSession _webSession, string idVehicle, string idCategory, string idMedia, string period,Page page)
		/// Page_Load ==> public static string getMediaInsertionsCreationsExcelUI(WebSession _webSession, string idVehicle, string idCategory, string idMedia, Page page)
		/// </remarks>
		/// <param name="sender">Objet responsable de l'appel de la fonction</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{

                Response.ContentType = "application/vnd.ms-excel";

				#region Resultat
				string[] ids=null;			
				try{
					//Récupération du media et du vehicule
					ids = Page.Request.QueryString.Get("ids").Split(',');
					result = CompetitorAlertCreationsResultsUI.GetCompetitorAlertCreationsExcelUI(_webSession,ids[0],Int64.Parse(ids[1]),int.Parse(ids[2]),this.Page);
				
				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
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

	}
}
