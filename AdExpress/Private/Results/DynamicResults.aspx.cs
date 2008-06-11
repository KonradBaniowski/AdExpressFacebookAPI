#region Information
/*Auteur : G. Facon
  Cr�� le :  08/11/2004
  Date de modification : 
	30/12/2004	D. Mussuma	Int�gration de WebPage
	19/11/2006	G. Facon	Ajout composant G�neric d�tail Level
*/
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.WebResultUI;


#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Affiche les tableaux dynamiques
	/// </summary>
	public partial class DynamicResults : TNS.AdExpress.Web.UI.ResultWebPage{	

		#region variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options des r�sultats
		/// </summary>
		/// <summary>
		/// Contr�le passerelle vres les autres modules
		/// </summary>
		/// <summary>
		/// Contr�le menu d'ent�te 
		/// </summary>
		/// <summary>
		/// Contr�le information
		/// </summary>
		/// <summary>
		/// Contr�le niveau de d�tail g�n�rique
		/// </summary>

		/// <summary>
		/// Composant tableau de r�sultat
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Liste des annonceurs s�lectionn�s
		/// </summary>
		protected string listAdvertiserUI="";
		/// <summary>
		/// Script de gestion du rappel annonceur
		/// </summary>
		public string scriptAdvertiserRemind;		
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Niveau de d�tail produit
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Results.DetailProductLevelWebControl DetailProductLevelWebControl2;
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Texte niveau de d�tail produit
		/// </summary>
		/// <summary>
		/// Annnule la personnalisation des �l�ments de r�f�rences et concurrents
		/// </summary>
		/// <summary>
		/// Liste des ann�es pour les agences m�dias
		/// </summary>
		/// <summary>
		/// Texte Agence M�dia
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		
	
		/// <summary>
		/// Affiche les ann�es disponibles pour les agences m�dia
		/// </summary>
		public bool displayMediaAgencyList=true;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public DynamicResults():base(){			
		}
		#endregion
	
		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page : 
		///		Flash d'attente
		///		Initialisation des connections � la BD
		///		Redirection en cas changement d'un crit�re de s�lection
		///		Traduction du site
		///		Extraction du code HTML r�pondant � la s�lection utilisateur
		///		MAJ dans la session de la derni�re page de r�sultats atteinte lors de la navigation
		/// </summary>		
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{				
				#region Gestion du flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
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

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion
			
				#region Agence m�dia
				//displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
				#endregion
				
				#region S�lection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				#endregion				

				#region choix du type d'encarts
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName 
					|| DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
					ResultsOptionsWebControl1.InsertOption=true;																	
				}			
				else ResultsOptionsWebControl1.InsertOption=false;
			
				#endregion
								
				#region R�sultat
				//Code html des r�sultats
				result="";
				//TODO:AD
				// Initialisation de preformatedProductDetail
				if(_webSession.PreformatedProductDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct
					|| !_webSession.ReachedModule
					){
					_webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;	 
				}	
				// Initialisation de l'unit� pour le cas de la presse
				if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.press.GetHashCode().ToString()
					&& !_webSession.ReachedModule
					){
					_webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
				}
				if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode().ToString()
					&& !_webSession.ReachedModule
					){
					_webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
				}
				
				if(_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.DynamicAnalysis.SYNTHESIS){
					ResultsOptionsWebControl1.Percentage=false;
					_webSession.PDM=false;					
				}
				#endregion
			
				#region MAJ _webSession
				_webSession.ReachedModule=true;
				#endregion	

                //#region S�lection du vehicle
                //string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                //DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
                //if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
                //#endregion


			}			
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public static void TNS.AdExpress.Web.DataAccess.Functions.closeDataBase(WebSession _webSession)
		/// </remarks>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion
		
		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'�v�nement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			//DetailProductLevelWebControl2.WebSession=_webSession;
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			//MediaAgencyYearWebControl1.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			_genericMediaLevelDetailSelectionWebControl.CustomerWebSession=_webSession;
            _genericColumnLevelDetailSelectionWebControl1.CustomerWebSession = _webSession;

			resultwebcontrol1.CustomerWebSession = _webSession;

			return tmp;
		}
		#endregion

		#region Pr�render
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

				#region S�lection du vehicle
				string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				#endregion

				TNS.AdExpress.Domain.Level.DetailLevelItemInformation columnDetailLevel = (TNS.AdExpress.Domain.Level.DetailLevelItemInformation)_webSession.GenericColumnDetailLevel.Levels[0];
				if ((vehicleName == DBClassificationConstantes.Vehicles.names.press || vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress) && columnDetailLevel.Id == TNS.AdExpress.Domain.Level.DetailLevelItemInformation.Levels.media)
					resultwebcontrol1.NbTableBeginningLinesToRepeat = 2;
				else resultwebcontrol1.NbTableBeginningLinesToRepeat = 1;
			
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
	
		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output){
			base.Render(output);
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent() {
            this.Unload += new System.EventHandler(this.Page_UnLoad);          

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
