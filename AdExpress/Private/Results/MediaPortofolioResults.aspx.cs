#region Informations
// Auteur: D. V. Mussuma
// Date de création: 23/12/2004
//date de modification : 30/12/2004  D. Mussuma Intégration de WebPage
#endregion

#region namespace
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
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using Dundas.Charting.WebControl;
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Affiche les résultats (tableaux et graphiques) du portefeuille
	/// d'un support
	/// </summary>
	public partial class MediaPortofolioResults :  TNS.AdExpress.Web.UI.ResultWebPage{		
	
		#region variables publiques
		/// <summary>
		/// Date récupérer dans l'url
		/// </summary>
		public string date="";		
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();				
		/// <summary>
		/// Si true si média c'est la presse
		/// </summary>
		public bool press=false;
		/// <summary>
		/// Si true si la planche est un détail de portefeuille
		/// </summary>
		public bool _genericProductLevel = false;
		/// <summary>
		/// libellé nouveau produit
		/// </summary>
		public string newProductText="";
		/// <summary>
		/// Bool indiquant si l'on doit afficher la liste des agences médias
		/// </summary>
		public bool	displayMediaAgencyList=true;
		#endregion

		#region variables MMI
		/// <summary>
		/// Contrôle Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle Options des résultats
		/// </summary>
		/// <summary>
		/// Contrôle passerelle vers les autres modules
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleBridgeWebControl ModuleBridgeWebControl1;
		/// <summary>
		/// Contrôle menu d'entête 
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Annule la personnalisation des éléments de références ou concurrents
		/// </summary>
		/// <summary>
		/// Texte Agence média
		/// </summary>
		/// <summary>
		/// Conrole agence média
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public MediaPortofolioResults():base(){			
			date=HttpContext.Current.Request.QueryString.Get("date");
		}
		#endregion		

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
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
		
				#region Sélection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
				#endregion

				//#region Agence média
				//displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
				//#endregion

				#region Option encart
				ResultsOptionsWebControl1.InsertOption=false;											
				#endregion

				#region Résultat
				// Affichage de tous les produits
				if(Request.Form.Get("__EVENTTARGET")=="InitializeButton"){
					_webSession.CurrentUniversProduct.Nodes.Clear();
					_webSession.SelectionUniversProduct.Nodes.Clear();
					_webSession.Save();
				}

				//Choix de la planche à afficher
				_ResultWebControl.Visible = false;
				_genericProductLevel = false;
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName 
					|| DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
					ResultsOptionsWebControl1.InsertOption=true;	
				}			
				else ResultsOptionsWebControl1.InsertOption=false;	
				switch(_webSession.CurrentTab){					
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						_ResultWebControl.Visible = false;
						_genericProductLevel = false;
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						_ResultWebControl.Visible = true;
						_genericProductLevel = true;
						break;				
					default:					
						break;
				}	
				#endregion
						
				#region Textes et Langage du site
                for (int i = 0; i < this.Controls.Count; i++) {
                    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion
			
				#region Scripts
				// Ouverture de la popup chemin de fer
				if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioCreation")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"portofolioCreation",TNS.AdExpress.Web.Functions.Script.PortofolioCreation());
				}					
				if (!Page.ClientScript.IsClientScriptBlockRegistered("openCreation"))Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openCreation",WebFunctions.Script.OpenCreation());
				// Ouverture de la popup detail portefeuille
				if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioDetailMedia")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"portofolioDetailMedia",TNS.AdExpress.Web.Functions.Script.PortofolioDetailMedia());
				}	
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
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;			
			InitializeProductWebControl1.CustomerWebSession=_webSession;	
			//MediaAgencyYearWebControl1.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			_genericMediaLevelDetailSelectionWebControl.CustomerWebSession = _webSession;
			_ResultWebControl.CustomerWebSession = _webSession;

			return tmp;
		}
		#endregion

		#region PreRender
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

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output">sortie html</param>
		protected override void Render(HtmlTextWriter output){
			// Calcul du résultat pour portefeuille
			if (!_ResultWebControl.Visible)
				result=WebBF.Results.PortofolioSystem.GetHtml(this.Page,_webSession);
			//id Média
			string idVehicle=_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			
			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.press :
				case DBClassificationConstantes.Vehicles.names.internationalPress :
				case DBClassificationConstantes.Vehicles.names.internet:
                case DBClassificationConstantes.Vehicles.names.directMarketing:
					break;
				case DBClassificationConstantes.Vehicles.names.radio :					
				case DBClassificationConstantes.Vehicles.names.tv :
				case DBClassificationConstantes.Vehicles.names.others :	
				case DBClassificationConstantes.Vehicles.names.outdoor:
					ResultsOptionsWebControl1.resultsPages.Items.Remove(ResultsOptionsWebControl1.resultsPages.Items.FindByValue("5"));				
					break;
				default :				
					throw new  WebExceptions.PortofolioSystemException("Le cas de ce média n'est pas gérer.");
			}			
			base.Render(output);
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <param name="e">arguments</param>
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
