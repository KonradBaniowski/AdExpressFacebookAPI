#region Informations
// Auteur: B.Masson
// Date de création: 01/06/2006
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
using TNS.AdExpress.Web.UI;
#endregion

namespace AdExpress.Public{
	/// <summary>
	/// Page publique du détail du chemin de fer d'un support utilisé par les alertes push mail
	/// </summary>
	public partial class PortofolioCreationMedia : WebPage{
		
		#region Variables
        /// <summary>
        /// identifiant du support
        /// </summary>
        protected string _idMedia = string.Empty;
        /// <summary>
        /// Nom du support
        /// </summary>
        protected string _nameMedia = "";
        /// <summary>
        /// Date de kiosque
        /// </summary>
        protected string _dateMediaNum = string.Empty;
        /// <summary>
        /// Date de couverture
        /// </summary>
        protected string _dateCoverNum = string.Empty;
		/// <summary>
		/// Resultat
		/// </summary>
		public string result="";
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
            
			try {
                int siteLanguage;
                if(Page.Request.QueryString.Get("idMedia") != null)
                    _idMedia = HttpContext.Current.Request.QueryString.Get("idMedia");
                if(Page.Request.QueryString.Get("dateCoverNum") != null)
                    _dateCoverNum = HttpContext.Current.Request.QueryString.Get("dateCoverNum");
                if(Page.Request.QueryString.Get("dateMediaNum") != null)
                    _dateMediaNum = HttpContext.Current.Request.QueryString.Get("dateMediaNum");
                if(Page.Request.QueryString.Get("nameMedia") != null)
                    _nameMedia = HttpContext.Current.Request.QueryString.Get("nameMedia");
                if(Page.Request.QueryString.Get("siteLanguage") == null)
                    siteLanguage = _siteLanguage;
                else siteLanguage = int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage").ToString());

                if(_idMedia.Length > 0 && _dateCoverNum.Length > 0 && _dateMediaNum.Length > 0 && _nameMedia.Length > 0) {
                    result = PortofolioUI.GetPortofolioCreationMedia(_dateMediaNum, _dateCoverNum, _idMedia, _nameMedia, siteLanguage);
                }

                #region Scripts
                // Ouverture de la popup une création
				if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioOneCreation")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "portofolioOneCreation", TNS.AdExpress.Web.Functions.Script.PortofolioOneCreation());
				}
				#endregion

			}
			catch(System.Exception et){
                Response.Redirect("/Public/Message.aspx?msgCode=5&siteLanguage=" + _siteLanguage);
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <param name="e">arguments</param>
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

	}
}
