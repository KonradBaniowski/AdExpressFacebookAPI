#region Information
/*
Author : B. Masson
Creation : 25/08/2005
Last modification:
	26/08/2005 par B.Masson
*/
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

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Domain.Translation;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Pop Up justificatif presse
	/// </summary>
	public partial class APPMProof : TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region Variables
		/// <summary>
		/// Résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession="";
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia="";
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct="";
		/// <summary>
		/// Date
		/// </summary>
		private string _date="";
		/// <summary>
		/// Date parution
		/// </summary>
		private string _dateParution="";
		/// <summary>
		/// Date faciale
		/// </summary>
		private string _dateCover = "";
		/// <summary>
		/// Page
		/// </summary>
		private string _page="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public APPMProof():base(){			
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion
		
		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Page</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Variables
				System.Text.StringBuilder HtmlTxt = new System.Text.StringBuilder(3000);
				#endregion

				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion

				#region Résultat
				try{
					//Récupération des paramètres de l'url
					_idMedia	= Page.Request.QueryString.Get("idMedia");
					_idProduct	= Page.Request.QueryString.Get("idProduct");
					_date		= Page.Request.QueryString.Get("date");
					//_dateParution = Page.Request.QueryString.Get("dateParution");
					_dateCover = Page.Request.QueryString.Get("dateParution"); ;
					_page		= Page.Request.QueryString.Get("page");
                    _dataSource = _webSession.Source;
				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
				}
				result = APPMSystem.GetProofHtml(this.Page, _dataSource, _webSession, Int64.Parse(_idMedia), Int64.Parse(_idProduct), int.Parse(_date), int.Parse(_dateCover), _page);
				#endregion

				#region MAJ Session
				//Sauvegarde de la session
				_webSession.Save();
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
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
