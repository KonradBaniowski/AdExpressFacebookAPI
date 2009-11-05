#region Informations
// Auteur: A. Obermeyer
// Date de création: 01/12/2004
// Date de modification: 
// 19/09/2006: D. V. Mussuma | Ajout du niveau de détail produit

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

using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;
#endregion

namespace AdExpress.Private.Results{

	/// <summary>
	///Portefeuille d'un support
	/// </summary>
    public partial class PortofolioResults : TNS.AdExpress.Web.UI.ResultWebPage {	

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
		/// Si true affiche la liste pour les niveaux de détail produit
		/// </summary>
		public bool detailProductLevel=false;
		/// <summary>
		/// libéllé nouveau produit
		/// </summary>
		public string newProductText="";
		/// <summary>
		///  bool précisant si l'on doit afficher les agences médias
		/// </summary>
		public bool displayMediaAgencyList=true;

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
		/// Choix option nouveau produit
		/// </summary>
		/// <summary>
		/// Graphique du portefeuille d'un support
		/// </summary>
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
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Annule la personnalisation des éléments de références et concurrents
		/// </summary>
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public PortofolioResults():base(){
			date=HttpContext.Current.Request.QueryString.Get("date");			
		}
		#endregion		

		#region Evènements

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);
            Int64 tabSelected;
            try {
                tabSelected = Int64.Parse(Page.Request.Form.GetValues("_resultsPages")[0]);
            }
            catch (System.Exception err) {
                tabSelected = _webSession.CurrentTab;
            }
            switch (tabSelected) {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                    _resultWebControl.SkinID = "portofolioSynthesisResultTable";
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    _resultWebControl.SkinID = "portofolioResultTable";
                    break;
            }
        }
        #endregion

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
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null)
				{
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
			
				#region Choix affichage graphique ou tableau ( planche structure)
				ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
				ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);
				ResultsOptionsWebControl1.ResultFormat = false;
				#endregion	

				#region Agence média
				//displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
				#endregion

				// Initialisation de preformatedProductDetail
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
				if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode().ToString()
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

				#region Sélection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
				#endregion

				#region Outdoor prerequisites
				if((DBClassificationConstantes.Vehicles.names.outdoor==vehicleName)&&
					((_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA)||
					(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY)||
					(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE)))		
					
				{
					_webSession.CurrentTab=TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS;
					_webSession.Save();
				}				
				#endregion	

				//Choix de la planche courante
				//portofolioChart.Visible = false;
				portofolioChartWebControl1.Visible = false;
				_resultWebControl.Visible = false;
				switch(_webSession.CurrentTab)
				{					
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						_genericMediaLevelDetailSelectionWebControl.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;
                        _resultWebControl.Visible = true;
                        _resultWebControl.ShowContainer = false;
                        //_resultWebControl.SkinID = "portofolioSynthesisResultTable"; 
                        detailProductLevel = true;
						
						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:	
						_genericMediaLevelDetailSelectionWebControl.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;

						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;	
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						_genericMediaLevelDetailSelectionWebControl.Visible=true;
						ResultsOptionsWebControl1.UnitOption=false;	
						ResultsOptionsWebControl1.Percentage=false;
						_resultWebControl.Visible = true;
						detailProductLevel=true;
				
						if(DBClassificationConstantes.Vehicles.names.press==vehicleName
							|| DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
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
						_webSession.CustomerLogin.ClearModulesList();
						_webSession.Save();
				
						_genericMediaLevelDetailSelectionWebControl.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;						
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						_genericMediaLevelDetailSelectionWebControl.Visible=false;
						ResultsOptionsWebControl1.InsertOption=false;
						ResultsOptionsWebControl1.UnitOption=false;		
						ResultsOptionsWebControl1.Percentage=false;
						ResultsOptionsWebControl1.ResultFormat = true;

						#region Choix affichage graphique ou tableau ( planche structure)
						ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
						ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);

						if(!IsPostBack){				
							ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
							ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
						}else{
							_webSession.Graphics=ResultsOptionsWebControl1.GraphRadioButton.Checked;
							_webSession.Save();
						}
						if(_webSession.Graphics){
							//portofolioChart.Visible = true;
							portofolioChartWebControl1.Visible = true;
							ResultsOptionsWebControl1.GraphRadioButton.Checked = true;
						}else{
							ResultsOptionsWebControl1.TableRadioButton.Checked = true;
						}
						#endregion

						if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
							//unité en euro pour cette planche
							_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
							_webSession.Save();
						}
						break;
						
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
						detailProductLevel=true;
						_genericMediaLevelDetailSelectionWebControl.Visible=true;
						ResultsOptionsWebControl1.UnitOption=true;
						ResultsOptionsWebControl1.Percentage=true;
						_resultWebControl.Visible = true;
						if(DBClassificationConstantes.Vehicles.names.press==vehicleName 
							|| DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
							ResultsOptionsWebControl1.InsertOption=true;																	
						}			
						else ResultsOptionsWebControl1.InsertOption=false;
						_webSession.Save();
						break;

					default:					
						break;
				}	
						
				#region Textes et Langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				_siteLanguage=_webSession.SiteLanguage;
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
			_genericMediaLevelDetailSelectionWebControl.CustomerWebSession=_webSession;
			InitializeProductWebControl.CustomerWebSession=_webSession;	
			MenuWebControl2.CustomerWebSession = _webSession;
			_resultWebControl.CustomerWebSession = _webSession;

			portofolioChartWebControl1.CustomerWebSession = _webSession;
			portofolioChartWebControl1.TypeFlash = true;

		 

			return tmp;
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			try{
                Domain.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                object[] parameters = new object[1];
                parameters[0] = _webSession;
                Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
				
                switch(_webSession.CurrentTab) {

					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = portofolioResult.GetVehicleViewHtml(false, FrameWorkConstantes.Portofolio.SYNTHESIS); 
                        break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        result = portofolioResult.GetDetailMediaHtml(false);
                        break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
                        //result = WebBF.Results.PortofolioSystem.GetAlertHtml(this.Page, _webSession);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						if (!_webSession.Graphics)
							result = portofolioResult.GetStructureHtml(false);
						else {
							//portofolioChartWebControl2 = new TNS.AdExpress.Web.Controls.Results.PortofolioChartWebControl();
							//portofolioChartWebControl2.CustomerWebSession = _webSession;
							//portofolioChartWebControl2.TypeFlash = true;
						}
						break;
					default:					
						break;
				}


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
			base.OnPreRender (e);
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
			return MenuWebControl2.NextUrl;
		}
		#endregion

	}
}
