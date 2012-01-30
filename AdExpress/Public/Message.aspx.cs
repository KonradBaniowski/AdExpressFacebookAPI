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

using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace AdExpress{
	/// <summary>
	/// Page pour afficher des messages (d'erreur ou non).
	/// On peut preciser plusieurs parametres dans l uirl
	/// msgCode permet de specifier un code message a recuperé
	/// msgTxt permet de faire passer un message a afficher directement (message plut^pot du a des exceptions nojn gérées)
	/// back permet d'indiquer le nombre de page a retourner en arriere apres.
	/// title permet de mettre un titre dans la barre de la fenêtre si désiré. Si title n'est pas précisé
	/// et qu'un message spécifique est demandé sous forme de code, un titre par defaut aest applique.
	/// Si le message est un txt et que le titre n est pas precise, il ny en a pas
	/// </summary>
	public partial class Message : WebPage{
		
		#region Variables
		///// <summary>
		///// ? 
		/////</summary>
		//protected System.Web.UI.HtmlControls.HtmlForm Form2;
		///// <summary>
		///// Message a afficher
		///// </summary>
		//protected System.Web.UI.WebControls.Label msg;
		/// <summary>
		/// Nombre de page de retour en arrière
		/// </summary>
		public int backPageNb = 2;
		/// <summary>
		/// Titre de la page
		/// </summary>
		public string title="";
		#endregion
	
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation de s controls d ela page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Chargement de la page

		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e) {
			if(Page.Request.QueryString.Get("back")!=null) 
                backPageNb = int.Parse(Page.Request.QueryString.Get("back").ToString());

			if(Page.Request.QueryString.Get("msgTxt")!=null) 
                msg.Text = Page.Request.QueryString.Get("msgTxt").ToString();
			else{
				if (Page.Request.QueryString.Get("msgCode")!=null) 
                    msg.Text = getText(int.Parse(Page.Request.QueryString.Get("msgCode").ToString()));
				else 
                    msg.Text = getText(0);
			}
			
			if(Page.Request.QueryString.Get("title")!=null) 
                title = Page.Request.QueryString.Get("title").ToString();
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Obtient le texte correspondand à l'erreur
		/// </summary>
		/// <param name="code">Numéro d'erreur</param>
		/// <returns>Texte correspondand à l'erreur</returns>
		private string getText(int code){
			switch (code){
				case 1:
					title=GestionWeb.GetWebWord(887,_siteLanguage);
					return GestionWeb.GetWebWord(1485,_siteLanguage);
				case 2:
					title=GestionWeb.GetWebWord(888,_siteLanguage);
					return GestionWeb.GetWebWord(1486,_siteLanguage);
				case 3:
					title=GestionWeb.GetWebWord(887,_siteLanguage);
					return GestionWeb.GetWebWord(1487,_siteLanguage);
				case 4:
					title=GestionWeb.GetWebWord(887,_siteLanguage);
					return GestionWeb.GetWebWord(1488,_siteLanguage);
				case 5:
					title=GestionWeb.GetWebWord(887,_siteLanguage);
                    return GestionWeb.GetWebWord(1489, _siteLanguage) + "<br/><a class=\"Tips1\" href=\"/index.aspx\">" + GestionWeb.GetWebWord(2929, _siteLanguage) + "</a>";
				default: 
					title=GestionWeb.GetWebWord(887,_siteLanguage);
					return GestionWeb.GetWebWord(1490,_siteLanguage);
			}
		}
		#endregion

	}
}
