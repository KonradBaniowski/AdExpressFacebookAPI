#region Informations
// Auteur: A. Obermeyer
// Date de création: 3/12/2004
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
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;

using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;
#endregion

namespace AdExpress.Private.Results{

	/// <summary>
	/// Pop up affichant le détail du chemin de fer d'un support
	/// </summary>
	public partial class PortofolioCreationMediaPopUp :  TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// identifiant du média
		/// </summary>
		protected string idMedia;
		/// <summary>
		/// Date de publication du support
		/// </summary>
		protected string date;
		/// <summary>
		/// Date de cover du support
		/// </summary>
		protected string dateCover;
		/// <summary>
		/// Nom du support
		/// </summary>
		protected string nameMedia="";
		/// <summary>
		/// Resultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Nombre de page
		/// </summary>
		protected string nbrePages="";
		/// <summary>
		/// Ancre du numéro de page pour le positionnement de la publicité dans son contexte
		/// </summary>
		public string _pageAnchor=null;

        /// <summary>
        /// Sub folder name of magazine pages
        /// </summary>
        public string _subFolder = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioCreationMediaPopUp():base(){			
			idMedia=HttpContext.Current.Request.QueryString.Get("idMedia");
			date=HttpContext.Current.Request.QueryString.Get("date");
			dateCover = HttpContext.Current.Request.QueryString.Get("parution");
			nameMedia=HttpContext.Current.Request.QueryString.Get("nameMedia");
			nbrePages=HttpContext.Current.Request.QueryString.Get("nbrePages");
            _pageAnchor = HttpContext.Current.Request.QueryString.Get("pageAnchor");
            _subFolder = HttpContext.Current.Request.QueryString.Get("subfolder");
		}
		#endregion
		
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
				
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			displaymediaPageswebcontrol1.CustomerWebSession = _webSession;
			displaymediaPageswebcontrol1.IdMedia = long.Parse(idMedia);
			displaymediaPageswebcontrol1.NameMedia = nameMedia;
			displaymediaPageswebcontrol1.NbPages = nbrePages;
			displaymediaPageswebcontrol1.DateParution = date;
			displaymediaPageswebcontrol1.DateCover = dateCover;
            displaymediaPageswebcontrol1.PageAnchor = _pageAnchor;
		    displaymediaPageswebcontrol1.SubFolder = _subFolder;
			return tmp;
		}
		#endregion


		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
