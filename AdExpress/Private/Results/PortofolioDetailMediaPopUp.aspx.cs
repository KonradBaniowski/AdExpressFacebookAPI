#region Informations
// Auteur: A. Obermeyer
// Date de création: 14/12/2004
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

using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

#endregion

namespace AdExpress.Private.Results {
	/// <summary>
	/// Pop Up affiche le détail média d'un portefeuille
	/// </summary>
	public partial class PortofolioDetailMediaPopUp : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// Date récupérer dans l'url
		/// </summary>
		public string idMedia="";
		/// <summary>
		/// Dans le cas de la radio-tv
		/// On précise le jour de la semaine
		/// </summary>
		public string dayOfWeek="";
		/// <summary>
		/// Code écran
		/// </summary>
		public string code_ecran="";		
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public PortofolioDetailMediaPopUp():base(){
			// Chargement de la Session
			try{				
				idMedia=HttpContext.Current.Request.QueryString.Get("idMedia");
				dayOfWeek=HttpContext.Current.Request.QueryString.Get("dayOfWeek");
				code_ecran=HttpContext.Current.Request.QueryString.Get("ecran");
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
				Response.Flush();
			}		
		}
		#endregion	

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			
			#region Gestion du flash d'attente
			Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
			Page.Response.Flush();			
			#endregion

			InformationWebControl1.Language = _webSession.SiteLanguage;

            #region Old Code
            //DataSet dsTable = null;

            //if (WebFunctions.CheckedText.IsStringEmpty(code_ecran) || WebFunctions.CheckedText.IsStringEmpty(dayOfWeek)) {
            //    dsTable = TNS.AdExpress.Web.DataAccess.Results.PortofolioDetailMediaDataAccess.GetDetailMedia(_webSession, ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID, _webSession.PeriodBeginningDate, _webSession.PeriodEndDate, code_ecran, false);
            //    result = TNS.AdExpress.Web.UI.Results.PortofolioUI.GetHTMLDetailMediaPopUpUI(Page, _webSession, dsTable.Tables[0], ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID, code_ecran, dayOfWeek, false, false);
            //}
            //else {
            //    dsTable = TNS.AdExpress.Web.DataAccess.Results.PortofolioDetailMediaDataAccess.GetDetailMedia(_webSession, ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID, _webSession.PeriodBeginningDate, _webSession.PeriodEndDate, code_ecran, true);
            //    result = TNS.AdExpress.Web.UI.Results.PortofolioUI.GetHTMLDetailMediaPopUpUI(Page, _webSession, dsTable.Tables[0], ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID, code_ecran, dayOfWeek, false, true);
            //} 
            #endregion
            
            MenuWebControl2.ForcePrint = "/Private/Results/Excel/PortofolioDetailMediaPopUp.aspx?idSession="
				+ this._webSession.IdSession + "&ecran=" + code_ecran + "&dayOfWeek=" + dayOfWeek;
	
			#region Script
			// Ouverture de la popup création d'une image
            //if (!Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) {
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openPressCreation",TNS.AdExpress.Web.Functions.Script.OpenPressCreation());
            //}
            //if (!Page.ClientScript.IsClientScriptBlockRegistered("OpenDownload")) {
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"OpenDownload",TNS.AdExpress.Web.Functions.Script.OpenDownload());
            //}
			#endregion

		}
		#endregion

		#region Determine PostBack
		/// <summary>
		/// Determine Post Back
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection ret = base.DeterminePostBackMode ();

            ArrayList columnItemList = new ArrayList();

			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;

            PortofolioDetailMediaResultWebControl1.MediaId = HttpContext.Current.Request.QueryString.Get("idMedia");
            PortofolioDetailMediaResultWebControl1.DayOfWeek = HttpContext.Current.Request.QueryString.Get("dayOfWeek");
            PortofolioDetailMediaResultWebControl1.AdBreak = HttpContext.Current.Request.QueryString.Get("ecran");

            PortofolioDetailMediaResultWebControl1.CustomerWebSession = _webSession;

            #region Niveau de détail produit (Generic)
            // Initialisation à produit
			ArrayList levels=new ArrayList();
			levels.Add(10);
			_webSession.GenericProductDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
			//_webSession.Save();
			#endregion

			return ret ;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
