#region Information
// Auteur : Guillaume Facon
// Création : 18/04/2006 (Refonte de l'ancienne version)
// Modification :
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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Description résumée de test.
	/// </summary>
	public partial class MediaPlanAlertResults : TNS.AdExpress.Web.UI.ResultWebPage{

		#region Variables	
		/// <summary>
		/// Nécessaire pour savor quel est l evenement levé
		/// </summary>
		protected int  eventButton=0;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Titre du module
		/// </summary>
		/// <summary>
		/// Option du résultat
		/// </summary>
		/// <summary>
		/// Pont vers un autre résultat
		/// </summary>
		/// <summary>
		/// En-tête de la page
		/// </summary>
		/// <summary>
		/// Validation de la présentation
		/// </summary>
		/// <summary>
		/// Annule sélection produit
		/// </summary>
		/// <summary>
		/// Result WebControl
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Contrôle informations
		/// </summary>
		/// <summary>
		/// Niveaux de détail du plan media (regroupements)
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeurs
		/// </summary>
		public MediaPlanAlertResults():base(){
			
		}
		#endregion

		#region Evènements

		#region Load
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Url Suivante
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
	
				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				//Bouton valider dans la sous sélection
				okImageButton.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				okImageButton.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
				#endregion
	
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection				tmp = base.DeterminePostBackMode();

			GenericMediaLevelDetailSelectionWebControl1.CustomerWebSession	=	_webSession;
			ResultsOptionsWebControl1.CustomerWebSession					=	_webSession;
			InitializeProductWebControl1.CustomerWebSession					=	_webSession;
			AlertMediaPlanResultWebControl1.CustomerWebSession				=	_webSession;
			MenuWebControl2.CustomerWebSession								=	_webSession;

			SetSloganUniverseOptions();

			return(tmp);
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

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return MenuWebControl2.NextUrl;
		}

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
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

		#region Méthodes internes
		/// <summary>
		/// Indique si le client peut affiner l'univers de versions
		/// </summary>		
		private void SetSloganUniverseOptions(){
//			string idVehicleList =  _webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);						
			if ((!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(_webSession) || !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)) //droits affiner univers Versions
					){//|| (idVehicleList!=null && idVehicleList.Length>0 && idVehicleList.IndexOf(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString())>=0)//Interdire  affiner versions pour le média Internet
				
				InitializeProductWebControl1.Visible = false;
				MenuWebControl2.ForbidOptionPages = true;
			}						
		}
		#endregion
	}
}
