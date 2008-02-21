
#region Informations
// Auteur: D. Mussuma
// Date de création : 15/01/2007
// Date de modification : 
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

using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Alerts{
	/// <summary>
	/// Description résumée de AlertsDownloadCreationsPopUp.
	/// </summary>
	public partial class AlertsDownloadCreationsPopUp : System.Web.UI.Page{
		
		/// <summary>
		/// Titre de la PopUp
		/// </summary>
		public string title="";

		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose =  LoadingSystem.GetHtmlCloseDiv();

		/// <summary>
		/// Affiche les créations
		/// </summary>

		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage = 33;

		/// <summary>
		/// Chargement
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string[] parametersList = null;
			if(Page.Request.QueryString.Get("parameters") == null){
				_siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
			}
			else{
				parametersList = Page.Request.QueryString.Get("parameters").ToString().Split(',');
				if(parametersList!=null && parametersList.Length>=6)
				_siteLanguage = int.Parse(parametersList[5].ToString());
				else _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
			}

			//Titre de la popUp
			title = GestionWeb.GetWebWord(876, _siteLanguage);

			
			#region Flash d'attente
			Page.Response.Write(LoadingSystem.GetHtmlDiv(_siteLanguage,Page));
			Page.Response.Flush();
			#endregion	
		}

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initiailisation
		/// </summary>
		/// <param name="e">arguments</param>
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
	}
}
