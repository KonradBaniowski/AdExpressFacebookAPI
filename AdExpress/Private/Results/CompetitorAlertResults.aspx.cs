#region Information
/*
 * Auteur :		Arnaud Obermeyer
 * Cr�ation:	04/10/20404
 * Modifications:
 *		05/01/2005	D. V. Mussuma	Int�gration des options encarts pour la presse
 *		24/11/2006	G. Facon		Int�gration des niveaux g�n�riques
 * */
#endregion

#region Using
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
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.PresentAbsent.DAL;
using System.Reflection;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page Alerte concurrentielle
	/// </summary>
    public partial class CompetitorAlertResults : TNS.AdExpress.Web.UI.ResultWebPage {

		#region Variables MMI
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
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Bouton ok
		/// </summary>
		/// <summary>
		/// Texte du d�tail produit
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText detailProductAdExpressText;
		/// <summary>
		/// Initialisation des produits
		/// </summary>
		/// <summary>
		/// Menu contextuel
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
		protected TNS.AdExpress.Web.Controls.Results.ResultWebControl resultwebcontrol1;	
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
		/// Texte : Agence m�dia
		/// </summary>
		/// <summary>
		/// WebControl affichant la liste des ann�es pour les agences m�dias
		/// </summary>
		/// <summary>
		/// Script de gestion du rappel annonceur
		/// </summary>
		public string scriptAdvertiserRemind;
		protected TNS.AdExpress.Web.Controls.Buttons.RedirectWebControl RedirectWebControl1;
		/// <summary>
		/// Bool�en pr�cisant si l'on doit afficher les agences m�dias
		/// </summary>
		public bool displayMediaAgencyList=true;
        /// <summary>
        /// Bool�en pr�cisant si l'on doit afficher les donn�es en PDM ou non
        /// </summary>
        public bool pdm=false;
        /// <summary>
        /// Onglet pr�c�dent
        /// </summary>
        public long previousTab = 0;
        /// <summary>
        /// Vehicle name
        /// </summary>
        public DBClassificationConstantes.Vehicles.names _vehicleName;
		/// <summary>
		/// Vehicle information
		/// </summary>
		public VehicleInformation _vehicleInformation = null;
        #endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public CompetitorAlertResults():base(){
			// Chargement de la Session
			try{				
				idsession=HttpContext.Current.Request.QueryString.Get("idSession");				
			}
			catch(System.Exception ){
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage."));
				Response.Flush();
			}
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
				_nextUrl=this.MenuWebControl2.NextUrl;
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+idsession);
				}
				#endregion

                #region Previous Tab
                if (!IsPostBack) {
                    previousTab = _webSession.CurrentTab;
                    ViewState.Add("previousTab", previousTab);
                }
                else {
                    if (this.ViewState["previousTab"] != null) {
                        previousTab = (long)this.ViewState["previousTab"];
                        ViewState.Add("previousTab", _webSession.CurrentTab);
                    }
                }
                #endregion

                #region pdm
                if (this.ViewState["pdm"] != null) {
                    pdm = (bool)this.ViewState["pdm"];
                }
                if (previousTab != TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.FORCES &&
                    previousTab != TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.POTENTIELS) {

                    if (_webSession.Percentage)
                        pdm = true;
                    else
                        pdm = false;
                }

                ViewState.Add("pdm", pdm);
                #endregion

                #region Validation du menu
                if (Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion

				#region Textes et Langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;			
				#endregion
			
				#region D�finition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"CompetitorAlertResultsHelp.aspx";
				#endregion

                #region Inialisation de l'attribut Percentage
                if (_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.FORCES ||
                    _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.POTENTIELS) {

                    _webSession.Percentage = true;
                    _webSession.Save();
                }
                else {
                    if (pdm) {
                        _webSession.Percentage = true;
                        _webSession.Save();
                    }
                }
                #endregion

				#region S�lection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				VehicleInformation _vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
				if (_vehicleInformation == null) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				#endregion


				#region choix du type d'encarts

				if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id
					|| DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id
                    ) {

					#region Affichage encarts en fonction du module
					switch(_webSession.CurrentModule){
						case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
						case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
						case WebConstantes.Module.Name.ALERTE_POTENTIELS:	
						case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
							ResultsOptionsWebControl1.InsertOption=true;	
							break;
						default :
							ResultsOptionsWebControl1.InsertOption=false;																																	
							break;
					}
					#endregion					

				}			
				else ResultsOptionsWebControl1.InsertOption=false;
			
				#endregion

                #region R�sultat
                //Code html des r�sultats
				result="";
											
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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page:
		///		Fermeture des connections BD
		/// </summary>		
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
			_genericMediaLevelDetailSelectionWebControl.CustomerWebSession=_webSession;
            _genericColumnLevelDetailSelectionWebControl1.CustomerWebSession = _webSession;
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			//MediaAgencyYearWebControl1.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			#region Gestion PDM (on affiche pas PDM si un seul support est selectionn�)
            int positionUnivers=1;
			int nbMedia=0;
			while(_webSession.CompetitorUniversMedia[positionUnivers]!=null){
				nbMedia+=_webSession.GetSelection((System.Windows.Forms.TreeNode) _webSession.CompetitorUniversMedia[positionUnivers],TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess).Split(',').Length;
				positionUnivers++;
			}
            if(positionUnivers==2)
                _webSession.CurrentTab = TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.PORTEFEUILLE;
            
            if(nbMedia==1){
				_webSession.Percentage=false;
				ResultsOptionsWebControl1.Percentage=false;
			}
			#endregion
			
			_resultWebControl.CustomerWebSession=_webSession;

            #region Option autopromo (Evaliant)
            Int64 id = ((LevelInformation)_webSession.SelectionUniversMedia.Nodes[0].Tag).ID;
            if (VehiclesInformation.DatabaseIdToEnum(id) == DBClassificationConstantes.Vehicles.names.adnettrack || VehiclesInformation.DatabaseIdToEnum(id) == DBClassificationConstantes.Vehicles.names.evaliantMobile)
            {
				ResultsOptionsWebControl1.AutopromoEvaliantOption = VehiclesInformation.Get(id).Autopromo; 
            }
            #endregion

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
				TNS.AdExpress.Domain.Level.DetailLevelItemInformation columnDetailLevel = (TNS.AdExpress.Domain.Level.DetailLevelItemInformation)_webSession.GenericColumnDetailLevel.Levels[0];
				string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
				if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				 _vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
				if (_vehicleInformation == null) throw (new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
                if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) && columnDetailLevel.Id == TNS.AdExpress.Domain.Level.DetailLevelItemInformation.Levels.media)
                    _resultWebControl.NbTableBeginningLinesToRepeat = 2;
                else
                {
                    _resultWebControl.NbTableBeginningLinesToRepeat = 1;
                }

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
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output){
			try{

                #region gestion du PDM
                int positionUnivers = 1;
                while (_webSession.CompetitorUniversMedia[positionUnivers] != null) {
                    positionUnivers++;
                }

                TNS.AdExpress.Domain.Web.Navigation.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                object[] param = null;
                if (module.CountryDataAccessLayer == null) throw (new NullReferenceException("DataAccess layer is null for the present/absent result"));
                param = new object[1];
                param[0] = _webSession;
                IPresentAbsentResultDAL presentAbsentResultDal = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                DataSet dsMedia = null;
                dsMedia = presentAbsentResultDal.GetColumnDetails();
                DataTable dtMedia = dsMedia.Tables[0];
                if (dtMedia.Rows != null) {
                    if (dtMedia.Rows.Count == 1 && positionUnivers == 2) {
                        _webSession.Percentage = false;
                        _webSession.Save();
                        ResultsOptionsWebControl1.Percentage = false;
                    }
                }
                #endregion

				int i;
				if(_webSession.CompetitorUniversMedia.Count==1){
					//Supprime l'affichage des onglets communs,absents,exclusifs,synth�se s'il n'ya pas d'univers concurrents
//					for(i=1;i<=3;i++){
					for(i=1;i<=6;i++){
						ResultsOptionsWebControl1.resultsPages.Items.Remove(ResultsOptionsWebControl1.resultsPages.Items.FindByValue(i.ToString()));
					}					
					
				}

                if (_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.FORCES ||
                    _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.POTENTIELS) {

                    if (ResultsOptionsWebControl1.Percentage == true)
                        ResultsOptionsWebControl1.PercentageCheckBox.Enabled = false;
                }

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
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
