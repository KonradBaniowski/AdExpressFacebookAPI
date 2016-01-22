#region Informations
// Auteur: D. Mussuma
// Date de création: 14/02/2007
// Date de modification :
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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion


namespace AdExpress.Private.Alerts{

	/// <summary>
	/// Page publique du détail des créations d'un support utilisé par les alertes mail
	/// </summary>
	public partial class CreationsResults: System.Web.UI.Page{

		#region Variables
		/// <summary>
		/// Identifiant du média
		/// </summary>
		protected string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		protected string _idProduct = null;
		/// <summary>
		/// Date de début
		/// </summary>
		protected string _dateBegin = null;
		/// <summary>
		/// Date de Fin
		/// </summary>
		protected string _dateEnd = null;
		/// <summary>
		///Clé d'authentification
		/// </summary>
		protected string _authentificationKey = null;		
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage = 33;
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{
				
				if(Page.Request.QueryString.Get("siteLanguage") == null) _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
				else _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());

				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_siteLanguage,Page));
				Page.Response.Flush();
				#endregion	
			}
			catch(System.Exception ){
				Response.Redirect("/Public/Message.aspx?msgCode=5&_siteLanguage="+_siteLanguage);
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
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