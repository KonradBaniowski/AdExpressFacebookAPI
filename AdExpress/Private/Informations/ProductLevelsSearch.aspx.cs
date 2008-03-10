#region Informations
// Auteur: G. Facon
// Date de création: 13/06/2006
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
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using Classification=TNS.AdExpress.DataAccess.Classification;
using ClassificationTable=TNS.AdExpress.Constantes.Classification.DB.Table;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Informations{
	/// <summary>
	/// Information sur la recherche produit
	/// </summary>
	public partial class ProductLevelsSearch : TNS.AdExpress.Web.UI.PrivateWebPage{

		///<directed>True</directed>
		///  <supplierCardinality>0..1</supplierCardinality>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;

		#region Variables
		/// <summary>
		/// Identifiant de la session
		/// </summary>
		public string sessionIdString;
		/// <summary>
		/// Langue du site
		/// </summary>
		public string siteLanguage="33";
		/// <summary>
		/// Mon à chercher
		/// </summary>
		public string wordToSearch;
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();		
		#endregion

		#region Variables MMI

		///<directed>True</directed>
		///  <supplierCardinality>1</supplierCardinality>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ProductLevelsSearch():base(){
			wordToSearch=HttpContext.Current.Request.QueryString.Get("wordToSearch");
		}
		#endregion

		#region Evènements

		#region Chargement
		/// <summary>
		/// Chargements
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e){
			//AjaxPro.Utility.RegisterTypeForAjax(typeof(ProductLevelsSearch));
			siteLanguage=_webSession.SiteLanguage.ToString();
			sessionIdString=_webSession.IdSession;

			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
			#endregion 
						
			#region Gestion du flash d'attente
			Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));									
			Page.Response.Flush();					
			#endregion
		}
		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de PostBack
		/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			CountItemsInClassificationWebControl1.WordToSearch=wordToSearch;
			CountItemsInClassificationWebControl1.CustomerWebSession=_webSession;
			return(tmp);
		}
		#endregion


		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			InitializeComponent();
			base.OnInit(e);
			
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
           
		}
		#endregion

		#endregion

	}
}
