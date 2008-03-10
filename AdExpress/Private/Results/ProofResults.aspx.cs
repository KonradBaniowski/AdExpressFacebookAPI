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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebFunctions=TNS.AdExpress.Web.Functions;

using TNS.FrameWork.WebResultUI;

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Liste des justiifcatifs presse
	/// </summary>
	public partial class ProofResults  : TNS.AdExpress.Web.UI.ResultWebPage{

		#region Variables
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region variables MMI
		/// <summary>
		/// Contrôle Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle d'information sur les  options
		/// </summary>
		/// <summary>
		/// Contrôle menu d'entête 
		/// </summary>

		/// <summary>
		/// Menu contextuel
		/// </summary>
		
		/// <summary>
		/// Composant générique d'affichage des résultats
		/// </summary>

//		/// <summary>
//		/// Bouton de validation
//		/// </summary>
//		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl okImageButton;
		/// <summary>
		/// Annule sélection produit
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public ProofResults():base(){			
		}
		#endregion

		#region Evènements

		#region chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			#region Gestion du flash d'attente
			if(Page.Request.Form.GetValues("__EVENTTARGET")!=null) {
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput!=MenuWebControl2.ID){
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
			}
			else{
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
			}
			#endregion

			#region Url Suivante
			if(_nextUrl.Length!=0){
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			#endregion

			#region Textes et Langage du site
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;
			#endregion

			#region Option affiner version 
			if(!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(_webSession) || !WebFunctions.MediaDetailLevel.HasSloganRight(_webSession)){//droits affiner univers Versions				
				InitializeProductWebControl1.Enabled = false;
				MenuWebControl2.ForbidOptionPages = true;
				_webSession.IdSlogans = new ArrayList();
				
			}else{				
				MenuWebControl2.ForbidOptionPages = false;
			}
			#endregion

			_webSession.Save();
		}
		#endregion

		#region Prérender
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			try{

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
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

		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'évènement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;			
			MenuWebControl2.CustomerWebSession = _webSession;
			_resultWebControl.CustomerWebSession = _webSession;
			InitializeProductWebControl1.CustomerWebSession	= _webSession;	
			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
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

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return MenuWebControl2.NextUrl;
		}
		#endregion
	}
}
