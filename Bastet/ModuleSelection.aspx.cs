#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
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
using TNS.Isis.Right.Common;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Common.Headers;
using System.Threading;

namespace BastetWeb{
	/// <summary>
	/// Page du choix de module (stats ou indicateurs)
	/// </summary>
	public partial class ModuleSelection : PrivateWebPage{

		#region Variables MMI
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
            try {
                // Placer ici le code utilisateur pour initialiser la page
                HeaderWebControl1.LanguageId = _siteLanguage;
                HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.generic;

                Hashtable headers = HeaderList.List;
                if (headers != null
                    && headers.ContainsKey(HeaderWebControl1.Type_de_page.ToString())
                    && headers[HeaderWebControl1.Type_de_page.ToString()] is Header
                    && ((Header)headers[HeaderWebControl1.Type_de_page.ToString()]).MenuItems != null) {
                    if (((Header)headers[HeaderWebControl1.Type_de_page.ToString()]).MenuItems.Count == 1) {
                        Response.Redirect((((HeaderMenuItem)((Header)headers[HeaderWebControl1.Type_de_page.ToString()]).MenuItems[0]).TargetUrl));
                        return;
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception er) {
                this.OnError(new ErrorEventArgs(this, er));
            }
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{    

		}
		#endregion

		#endregion

	}
}
