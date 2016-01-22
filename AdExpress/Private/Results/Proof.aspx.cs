#region Information
/* Author : D. Mussuma
Creation : 31/01/2007
Last modification:
*/
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

using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Pop Up justificatif presse
	/// </summary>
	public partial class Proof :  TNS.AdExpress.Web.UI.PrivateWebPage{
		
	
		#region Variables
		
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Contrôle qui affiche la fiche justificative
		/// </summary>

		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Proof():base(){			
		
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Page</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try {
				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
				Page.Response.Flush();
				#endregion

				if (proofresultwebcontrol1.IsVisualExist) {
					MenuWebControl2.ForcePdfExportResult = "/Private/MyAdExpress/PdfSavePopUp.aspx?idSession="
						+ this._webSession.IdSession + "&idmedia=" + proofresultwebcontrol1.IdMedia
						+ "&idproduct=" + proofresultwebcontrol1.IdProduct + "&dateParution=" + proofresultwebcontrol1.DateMediaNum
						+ "&dateCover=" + proofresultwebcontrol1.DateFacial + "&page=" + proofresultwebcontrol1.PageNumber;

				}
				
			}
			catch (System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
				}
			}

		}
		#endregion

		protected override void Render(HtmlTextWriter output){
			if (proofresultwebcontrol1.IsVisualExist) {
				MenuWebControl2.ForcePdfExportResult = "/Private/MyAdExpress/PdfSavePopUp.aspx?idSession="
					+ this._webSession.IdSession + "&idmedia=" + proofresultwebcontrol1.IdMedia
                    + "&idproduct=" + proofresultwebcontrol1.IdProduct + "&dateParution=" + proofresultwebcontrol1.DateMediaNum
					+ "&dateCover=" + proofresultwebcontrol1.DateFacial + "&page=" + proofresultwebcontrol1.PageNumber;
			}
			else MenuWebControl2.Visible = false;

			base.Render(output);

		}
		
		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'évènement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			
			proofresultwebcontrol1.CustomerWebSession = _webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			MenuWebControl2.ForbidHelpPages = true;
		


			return tmp;
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
           
		}
		#endregion

		
	}
}
