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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.UI.Results;
#endregion

namespace AdExpress.Public{
	/// <summary>
	/// Page publique du détail du chemin de fer d'un support utilisé par les alertes push mail
	/// </summary>
	public partial class PortofolioCreationMedia : System.Web.UI.Page{
		
		#region Variables
		/// <summary>
		/// Identifiant du média
		/// </summary>
		protected string idMedia="";
		/// <summary>
		/// Date de publication du support
		/// </summary>
		protected string date="";
		/// <summary>
		/// Date de parution du support
		/// </summary>
		protected string parution="";
		/// <summary>
		/// Nom du support
		/// </summary>
		protected string nameMedia="";
		/// <summary>
		/// Resultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage=33;
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
			try{
				if(Page.Request.QueryString.Get("idMedia")!=null) idMedia = Page.Request.QueryString.Get("idMedia");
				if(Page.Request.QueryString.Get("date")!=null) date = Page.Request.QueryString.Get("date");
                if (Page.Request.QueryString.Get("parution") != null) date = Page.Request.QueryString.Get("parution");
				if(Page.Request.QueryString.Get("nameMedia")!=null) nameMedia = Page.Request.QueryString.Get("nameMedia");
				if(Page.Request.QueryString.Get("siteLanguage") == null) _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
				else _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());

				if(idMedia.Length>0 && date.Length>0 && nameMedia.Length>0){
					result=PortofolioUI.GetPortofolioCreationMedia(date,parution,idMedia,nameMedia,_siteLanguage);
				}

				#region Scripts
				// Ouverture de la popup une création
				if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioOneCreation")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "portofolioOneCreation", TNS.AdExpress.Web.Functions.Script.PortofolioOneCreation());
				}
				#endregion

			}
			catch(System.Exception et){
				//Response.Redirect("/Public/Message.aspx?msgTxt="+et.Message.Replace("&"," ")+"&_siteLanguage="+_siteLanguage);
				Response.Redirect("/Public/Message.aspx?msgCode=5&siteLanguage="+_siteLanguage);
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
