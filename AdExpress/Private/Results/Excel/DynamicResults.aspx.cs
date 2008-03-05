#region Information
// Auteur B.Masson
// Date de cr�ation : 15/11/04
//date de modification : 30/12/2004  D. Mussuma Int�gration de WebPage
#endregion

#region Namespace
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
using TNS.FrameWork.WebResultUI;
#endregion

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Export Excel pour l'analyse dynamique.
	/// </summary>
	public partial class DynamicResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables MMI
		/// <summary>
		/// R�sultat
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DynamicResults():base(){		
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{			

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
				#endregion

				#region Calcul du r�sultat
				//result=TNS.AdExpress.Web.BusinessFacade.Results.DynamicSystem.GetExcel(Page,_webSession);
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
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'�v�nement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns>PostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			ResultWebControl1.CustomerWebSession=_webSession;
			return tmp;
		}
		#endregion

		#region Pr�render
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			try{

				#region MAJ _webSession
				//_webSession.Save();
				#endregion
			
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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

		#endregion

	}
}
