#region Informations
// Auteur: A. Obermeyer
// Date de création: 03/12/2004
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
using TNS.AdExpress.Web.UI.Results;

using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;
#endregion

namespace AdExpress.Private.Results{

	/// <summary>
	/// Pop up affichant une image dans le chemin de fer
	/// </summary>
	public partial class PortofolioCreationOneMediaPopUp : System.Web.UI.Page{

		#region Variables		
		/// <summary>
		/// Identifiant Media
		/// </summary>
		protected string idMedia;
		/// <summary>
		/// Date de publication
		/// </summary>
		protected string date;
		/// <summary>
		/// Nom du fichier
		/// </summary>
		protected string fileName1;
		/// <summary>
		/// Nom du fichier
		/// </summary>
		protected string fileName2;
		/// <summary>
		/// Affichage du résultat
		/// </summary>
		public string result;
		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioCreationOneMediaPopUp():base(){						
			idMedia=HttpContext.Current.Request.QueryString.Get("idMedia");
			date=HttpContext.Current.Request.QueryString.Get("date");
			fileName1=HttpContext.Current.Request.QueryString.Get("fileName1");
			fileName2=HttpContext.Current.Request.QueryString.Get("fileName2");
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			
            //result= PortofolioUI.GetPortofolioOneCreationMedia(date,idMedia,fileName1,fileName2);		
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			zoommediapageswebControl1.IdMedia = long.Parse(idMedia);
			zoommediapageswebControl1.FileName1 = fileName1;
			zoommediapageswebControl1.FileName2 = fileName2;
			zoommediapageswebControl1.DateCover = date;

			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
