#region Information
// Auteur A.Obermeyer
// Date de création : 26/10/04
//date de modification : 30/12/2004  D. Mussuma Intégration de WebPage
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
#endregion

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	///Export Excel pour les tableaux dynamiques.
	/// </summary>
	public partial class DynamicTables : TNS.AdExpress.Web.UI.ExcelWebPage{
		
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DynamicTables():base(){			
		}
		#endregion

		#region Evènements

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

				#region Calcul du résultat
				result="<table><tr><td bgcolor=\"#ffffff\">"+TNS.AdExpress.Web.UI.Results.DynamicTablesUI.GetDynamicTableExcelUI(_webSession)+"</table></td></tr></table>";
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
		private void Page_UnLoad(object sender, System.EventArgs e){
			DBFunctions.closeDataBase(_webSession);
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
           
		}
		#endregion

		#endregion

	}
}
