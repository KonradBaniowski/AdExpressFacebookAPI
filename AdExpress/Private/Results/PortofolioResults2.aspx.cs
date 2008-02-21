#region Informations
// Auteur: A. Obermeyer
// Date de création: 01/12/2004
// Date de modification: 
#endregion

#region Namespace
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using Dundas.Charting.WebControl;
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;

using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results{

	/// <summary>
	///Portefeuille d'un support
	/// </summary>
	public partial class PortofolioResults2 : TNS.AdExpress.Web.UI.ResultWebPage{	

		#region variables
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result="";
		/// <summary>
		/// Date récupérer dans l'url
		/// </summary>
		public string date="";				
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();	
		/// <summary>
		/// Dans le cas où l'on se trouve dans la planche nouveauté
		/// </summary>
		public bool novelty=false;
		/// <summary>
		/// Commentaire Présentation graphique
		/// </summary>
		public string chartTitle="";
		/// <summary>
		/// Commentaire Présentation tableau
		/// </summary>
		public string tableTitle="";
		/// <summary>
		/// Si true affiche la liste pour les niveaux de détail produit
		/// </summary>
		public bool detailProductLevel=false;
		/// <summary>
		/// Si true si média c'est la presse
		/// </summary>
		public bool press=false;
		/// <summary>
		/// libéllé nouveau produit
		/// </summary>
		public string newProductText="";
		/// <summary>
		///  bool précisant si l'on doit afficher les agences médias
		/// </summary>
		public bool displayMediaAgencyList=true;

//		public string testTable2="Empty";
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
		/// Contrôle export des résultats
		/// </summary>
		/// <summary>
		/// Contrôle d'aide
		/// </summary>
		/// <summary>
		/// Contrôle menu d'entête 
		/// </summary>
		/// <summary>
		/// Contrôle de navigation dans un module
		/// </summary>
		/// <summary>
		/// Contrôle texte niveau de détail produit
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Niveau de détails produits
		/// </summary>
		/// <summary>
		/// Choix option nouveau produit
		/// </summary>
			
		/// <summary>
		/// Graphique du portefeuille d'un support
		/// </summary>
		protected TNS.AdExpress.Web.UI.Results.PortofolioChartUI portofolioChart;
		/// <summary>
		/// Annule la personnalisation des éléments de références et concurrents
		/// </summary>
		protected System.Web.UI.WebControls.LinkButton InitializeButton;
		/// <summary>
		/// Texte : agence média
		/// </summary>
		/// <summary>
		/// Controle agence média
		/// </summary>
		
//		/// <summary>
//		/// Controle tableau résultat
//		/// </summary>
//		protected TNS.FrameWork.WebResultUI.WebControlResultTable webControlResultTable;
		/// <summary>
		/// Annule la personnalisation des éléments de références et concurrents
		/// </summary>
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public PortofolioResults2():base(){
			date=HttpContext.Current.Request.QueryString.Get("date");			
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
		protected void Page_Load(object sender, System.EventArgs e) {
			try{
				
				#region Gestion du flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null) {
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=recallWebControl.ID){
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
				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0){
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Choix affichage graphique ou tableau ( planche structure)
				chartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
				tableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);							
								
				#endregion	
		

				#region Agence média
				displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
				#endregion

				#region Initialisation de preformatedProductDetail
				if(_webSession.PreformatedProductDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct
					|| !_webSession.ReachedModule
					) {
					_webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;	 
				}
				// Initialisation de l'unité pour le cas de la presse
				if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.press.GetHashCode().ToString()
					&& !_webSession.ReachedModule
					){
					_webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
				}
				// Création de la liste nouveaux produit (dans la pige ou dans le support)
				if(!IsPostBack){
					newProductRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1421,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support.GetHashCode().ToString()));
					newProductRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1422,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.pige.GetHashCode().ToString()));
					newProductRadioButtonList.Items[0].Selected=true;
					newProductRadioButtonList.CssClass="txtNoir11";			
				}
				newProductText=GestionWeb.GetWebWord(1449,_webSession.SiteLanguage);
				try{
					newProductRadioButtonList.Items.FindByValue(newProductRadioButtonList.SelectedItem.Value).Selected=true;
				}
				catch(Exception){
					if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY ){
						newProductRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support.GetHashCode().ToString()).Selected=true;
					}
				}
				#endregion

				#region Sélection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
				#endregion

				#region Outdoor prerequisites
				if((DBClassificationConstantes.Vehicles.names.outdoor==vehicleName)&&
					((_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA)||
					(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY)||
					(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE))) {
					_webSession.CurrentTab=TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS;
					_webSession.Save();
				}				
				#endregion	

				#region Choix de la planche courante
				switch(_webSession.CurrentTab) {					
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						DetailProductLevelWebControl2.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;
						
						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:	
						DetailProductLevelWebControl2.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;

						#region Test
							
						//							ExportWebControl1.ExcelFormat=true;
						//							ExportWebControl1.ExportExcelUrl="/Private/Results/Excel/PortofolioDetailMediaPopUp.aspx?idSession="+_webSession.IdSession;
						#endregion
						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;	
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						detailProductLevel=true;
						DetailProductLevelWebControl2.Visible=true;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;
						if(_webSession.PreformatedProductDetail==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product){
							_webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup;
							_webSession.Save();
						}					
						if(DBClassificationConstantes.Vehicles.names.press==vehicleName){
							ResultsOptionsWebControl1.InsertOption=true;																	
						}			
						else ResultsOptionsWebControl1.InsertOption=false;
						
						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
						novelty=true;
						newProductRadioButtonList.CssClass="txtNoir11Bold";
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;
						if(newProductRadioButtonList.Items.FindByValue(newProductRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support.GetHashCode().ToString()){
							_webSession.NewProduct=TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support;
						}
						else if(newProductRadioButtonList.Items.FindByValue(newProductRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.pige.GetHashCode().ToString()){
							_webSession.NewProduct=TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.pige;
						}

						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
						}
						_webSession.Save();
				
						DetailProductLevelWebControl2.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;						
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						DetailProductLevelWebControl2.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;		
						ResultsOptionsWebControl1.Percentage=false;
						press=true;
						
						#region Choix affichage graphique ou tableau ( planche structure)
						chartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
						tableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);

						
			
						#endregion

						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;
						
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
						detailProductLevel=true;
						DetailProductLevelWebControl2.Visible=true;
						ResultsOptionsWebControl1.UnitOption=true;
						ResultsOptionsWebControl1.Percentage=true;
						if(_webSession.PreformatedProductDetail==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product){
							_webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup;
						}					
						if(DBClassificationConstantes.Vehicles.names.press==vehicleName){
							ResultsOptionsWebControl1.InsertOption=true;																	
						}			
						else ResultsOptionsWebControl1.InsertOption=false;
						_webSession.Save();

						break;

					default:					
						break;
				}
				#endregion
						
				// Calcul du résultat pour portefeuille
				//result=WebBF.Results.PortofolioSystem.GetAlertHtml(this.Page,_webSession);
				// Nouveau Code
				// UI Origine			
//				testTable2 = TNS.AdExpress.Web.Rules.Results.PortofolioRules.GetFormattedTableForGenericUITest(_webSession).Render();
				
					
				
				
				
				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				_siteLanguage=_webSession.SiteLanguage;
			
				#endregion

				#region Définition de la page d'aide
				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"PortofolioResultsHelp.aspx";
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
			
				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;
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

		#region Prérender
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) 
		{
			base.OnPreRender (e);

			

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() 
		{			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ExportWebControl1.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			recallWebControl.CustomerWebSession=_webSession;
			DetailProductLevelWebControl2.WebSession=_webSession;
			InitializeProductWebControl.CustomerWebSession=_webSession;	
			MediaAgencyYearWebControl1.WebSession=_webSession;
			
			return tmp;
		}
		#endregion

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
		private void InitializeComponent(){

		}
		#endregion					

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return "";
		}
		#endregion

	}
}
