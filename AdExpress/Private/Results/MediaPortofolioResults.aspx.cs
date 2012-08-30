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
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
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

using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;
using System.Reflection;

#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Affiche les résultats (tableaux et graphiques) du portefeuille
	/// d'un support
	/// </summary>
    public partial class MediaPortofolioResults : TNS.AdExpress.Web.UI.ResultWebPage {		
	
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

		#region On PreInit
		/// <summary>
		/// On preinit event
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreInit(EventArgs e) {
			base.OnPreInit(e);
			Int64 tabSelected;

			try {
                //Set default tab if necessary
                ChangeCurrentTab();

				tabSelected = Int64.Parse(Page.Request.Form.GetValues("_resultsPages")[0]);

                
			}
			catch{
				tabSelected = _webSession.CurrentTab;
			}

            switch (tabSelected)
            {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                    _ResultWebControl.SkinID = "portofolioSynthesisResultTable";
                    _ResultWebControl.ShowContainer = false;
                    //TODO : A vérifier pour fusion Dev Trunk
                    _ResultWebControl.UseLimitation = false;
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    _ResultWebControl.SkinID = "portofolioResultTable";
                    //TODO : A vérifier pour fusion Dev Trunk                    
                    _ResultWebControl.UseLimitation = true;
                    break;
                default:
                    _ResultWebControl.UseLimitation = true;
                    break;
            }
			
		}
		#endregion

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

				#region Choix affichage graphique ou tableau ( planche structure)
				ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191, _webSession.SiteLanguage);
				ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192, _webSession.SiteLanguage);
				ResultsOptionsWebControl1.ResultFormat = false;
				#endregion	

				#region Sélection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName= VehiclesInformation.DatabaseIdToEnum(Int64.Parse(vehicleSelection));
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
				#endregion

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
				
				// Choix de la planche à afficher
				_ResultWebControl.Visible = false;
                vehicleItemsNavigatorWebControl1.Visible = false;
				portofolioChartWebControl1.Visible = false;
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName 
					|| DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName
                    || DBClassificationConstantes.Vehicles.names.newspaper == vehicleName
                    || DBClassificationConstantes.Vehicles.names.magazine == vehicleName)
                {
					ResultsOptionsWebControl1.InsertOption=true;	
				}			
				else 
                    ResultsOptionsWebControl1.InsertOption=false;
				
                switch(_webSession.CurrentTab){					
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						_ResultWebControl.Visible = true;// false;
                        ResultsOptionsWebControl1.UnitOption = false;
                        ResultsOptionsWebControl1.InsertOption = false;
                        vehicleItemsNavigatorWebControl1.Visible = true;
                        vehicleItemsNavigatorWebControl1.ResultType = TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS;
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        ResultsOptionsWebControl1.UnitOption = false;
                        vehicleItemsNavigatorWebControl1.Visible = true;
                        vehicleItemsNavigatorWebControl1.ResultType = TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA;
                        break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						_ResultWebControl.Visible = true;
                        ResultsOptionsWebControl1.UnitOption = false;
						break;								
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						ResultsOptionsWebControl1.InsertOption = false;
						ResultsOptionsWebControl1.UnitOption = false;
						ResultsOptionsWebControl1.Percentage = false;
						ResultsOptionsWebControl1.ResultFormat = true;

						#region Choix affichage graphique ou tableau ( planche structure)
						ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191, _webSession.SiteLanguage);
						ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192, _webSession.SiteLanguage);

						if (!IsPostBack) {
							ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
							ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
						}
						else {
							_webSession.Graphics = ResultsOptionsWebControl1.GraphRadioButton.Checked;
							_webSession.Save();
						}
						if (_webSession.Graphics) {
							//portofolioChart.Visible = true;
							portofolioChartWebControl1.Visible = true;
							ResultsOptionsWebControl1.GraphRadioButton.Checked = true;
						}
						else {
							ResultsOptionsWebControl1.TableRadioButton.Checked = true;
						}
						#endregion					
						break;

					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
						ResultsOptionsWebControl1.UnitOption = true;
						ResultsOptionsWebControl1.Percentage = true;
						_ResultWebControl.Visible = true;
						if (DBClassificationConstantes.Vehicles.names.press == vehicleName
							|| DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName
                            || DBClassificationConstantes.Vehicles.names.newspaper == vehicleName
                            || DBClassificationConstantes.Vehicles.names.magazine == vehicleName)
                        {
							ResultsOptionsWebControl1.InsertOption = true;
						}
						else ResultsOptionsWebControl1.InsertOption = false;
                        _webSession.ReachedModule = true;
						_webSession.Save();
						break;

					default:					
						break;
				}	
				#endregion
						
				#region Textes et Langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

               
				#region Scripts
				// Ouverture de la popup chemin de fer
				if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioCreation")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"portofolioCreation",TNS.AdExpress.Web.Functions.Script.PortofolioCreation());
				}					
				//if (!Page.ClientScript.IsClientScriptBlockRegistered("openCreation"))Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openCreation",WebFunctions.Script.OpenCreation());
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
            try
            {
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.SelectedMediaUniverse = GetSelectedUniverseMedia();
			MenuWebControl2.CustomerWebSession = _webSession;
			_ResultWebControl.CustomerWebSession = _webSession;
            vehicleItemsNavigatorWebControl1.CustomerWebSession = _webSession;
            vehicleItemsNavigatorWebControl1.Excel = false;

			portofolioChartWebControl1.CustomerWebSession = _webSession;
			portofolioChartWebControl1.TypeFlash = true;

            // Option autopromo (Evaliant)
            string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
            DBClassificationConstantes.Vehicles.names vehicleName = VehiclesInformation.DatabaseIdToEnum(Int64.Parse(vehicleSelection));
            if (vehicleName == DBClassificationConstantes.Vehicles.names.adnettrack || vehicleName == DBClassificationConstantes.Vehicles.names.evaliantMobile)
				ResultsOptionsWebControl1.AutopromoEvaliantOption = VehiclesInformation.Get(vehicleName).Autopromo; 
            else
                ResultsOptionsWebControl1.AutopromoEvaliantOption = false;

            #region Campaign type option
            ResultsOptionsWebControl1.CampaignTypeOption = WebApplicationParameters.AllowCampaignTypeOption;
            #endregion

            //Set Advertisement Type Options
//TODO: A Vérifier pour fusion dev trunk
            //SetAdvertisementTypeOptions();
			
			}			
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
			return tmp;
		}
		#endregion

		#region PreRender
		protected override void OnPreRender(EventArgs e) {
			
			try{

                Domain.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                object[] parameters = new object[1];
                parameters[0] = _webSession;

                Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

                switch (_webSession.CurrentTab)
                {
                   
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        result = portofolioResult.GetDetailMediaHtml(false);
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                        if (!_webSession.Graphics)
                            result = portofolioResult.GetStructureHtml(false);
                        break;

                }
                base.OnPreRender(e);

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
			//ChangeCurrentTab();
			//_webSession.Save();
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

		/// <summary>
		/// Set synthesis tab as default in some cases
		/// </summary>
		protected void ChangeCurrentTab() {

			#region VehicleInformation
			VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
			#endregion

			switch (vehicleInformation.Id) {
				case ClassificationCst.DB.Vehicles.names.outdoor :
                case ClassificationCst.DB.Vehicles.names.instore:
                case ClassificationCst.DB.Vehicles.names.indoor:
				case ClassificationCst.DB.Vehicles.names.cinema :
					if (_webSession.CurrentTab == FrameWorkConstantes.Portofolio.NOVELTY || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.DETAIL_MEDIA || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.STRUCTURE || (_webSession.CurrentTab == FrameWorkConstantes.Portofolio.CALENDAR && !_webSession.CustomerPeriodSelected.IsSliding4M)) {
						_webSession.CurrentTab = FrameWorkConstantes.Portofolio.SYNTHESIS;
						_webSession.Save();
					}
					break;
				case ClassificationCst.DB.Vehicles.names.directMarketing:
				case ClassificationCst.DB.Vehicles.names.internet:
                case ClassificationCst.DB.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
					if ((_webSession.CurrentTab == FrameWorkConstantes.Portofolio.NOVELTY || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.DETAIL_MEDIA || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.STRUCTURE || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.CALENDAR)) {
						_webSession.CurrentTab = FrameWorkConstantes.Portofolio.SYNTHESIS;
						_webSession.Save();
					}
					break;
				case ClassificationCst.DB.Vehicles.names.others:
				case ClassificationCst.DB.Vehicles.names.tv:
                case ClassificationCst.DB.Vehicles.names.tvGeneral:
                case ClassificationCst.DB.Vehicles.names.tvSponsorship:
                case ClassificationCst.DB.Vehicles.names.tvAnnounces:
                case ClassificationCst.DB.Vehicles.names.tvNonTerrestrials:
				case ClassificationCst.DB.Vehicles.names.radio:
                case ClassificationCst.DB.Vehicles.names.radioGeneral:
                case ClassificationCst.DB.Vehicles.names.radioSponsorship:
                case ClassificationCst.DB.Vehicles.names.radioMusic:
				case ClassificationCst.DB.Vehicles.names.press:
                case ClassificationCst.DB.Vehicles.names.newspaper:
                case ClassificationCst.DB.Vehicles.names.magazine:
				case ClassificationCst.DB.Vehicles.names.internationalPress:
					if (!_webSession.CustomerPeriodSelected.IsSliding4M && (_webSession.CurrentTab == FrameWorkConstantes.Portofolio.NOVELTY || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.DETAIL_MEDIA || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.STRUCTURE || _webSession.CurrentTab == FrameWorkConstantes.Portofolio.CALENDAR)) {
						_webSession.CurrentTab = FrameWorkConstantes.Portofolio.SYNTHESIS;
						_webSession.Save();
					}
					break;
				default: throw new WebExceptions.PortofolioSystemException(" Vehicle unknown.");
			}
		}

		#region Get selected universe media
		/// <summary>
		/// Get selected media universe
		/// </summary>
		/// <returns></returns>
		protected MediaItemsList GetSelectedUniverseMedia() {
			MediaItemsList selectedUniverseMedia = new MediaItemsList();
			// vehicle Selected
			selectedUniverseMedia.VehicleList = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
			return selectedUniverseMedia;
		}
		#endregion

        #region SetAdvertisementTypeOptions
        ///// <summary>
        ///// Indicate if customer can refine ad type
        ///// </summary>	
        //private void SetAdvertisementTypeOptions()
        //{   //TODO:A vérifier pour fusion Dev Trunk : déplacer dans resultoptionswebcontrol
        //    TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
        //    ArrayList detailSelections = ((ResultPageInformation)module.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
        //    if (detailSelections.Contains(WebConstantes.DetailSelection.Type.advertisementType.GetHashCode()))
        //    {
        //        InitializeProductWebControl1.Visible = true;
        //        InitializeProductWebControl1.InitializeAdvertisementType = true;
        //    }
        //}
        #endregion

    }
}
