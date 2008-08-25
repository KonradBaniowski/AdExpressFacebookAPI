#region Informations
// Auteur: 
// Date de création: 
// Date de modification: 
//		05/01/2005	D. V. Mussuma	Intégration des options encarts pour la presse
//		01/12/2006	G. Facon		Résultats et niveaux génériques
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
#endregion

namespace AdExpress.Private.Results{

	/// <summary>
	/// Page Alerte de potentiels.
	/// </summary>
	public partial class MarketShareResults : TNS.AdExpress.Web.UI.ResultWebPage{
		
//        #region Variables MMI
//        /// <summary>
//        /// Titre du module
//        /// </summary>
//        /// <summary>
//        /// Contrôle : Options de résultats
//        /// </summary>
//        /// <summary>
//        /// Text Niveau de détails produits
//        /// </summary>
//        protected TNS.AdExpress.Web.Controls.Translation.AdExpressText detailProductAdExpressText;
//        /// <summary>
//        /// Niveau de détails produits
//        /// </summary>
//        protected TNS.AdExpress.Web.Controls.Results.DetailProductLevelWebControl DetailProductLevelWebControl2;
//        /// <summary>
//        /// Bouton ok
//        /// </summary>
//        /// <summary>
//        /// Navigation entre modules
//        /// </summary>
//        /// <summary>
//        /// Contrôle En-tête
//        /// </summary>
//        /// <summary>
//        /// Initialisation de l'univers produits
//        /// </summary>
//        #endregion

//        #region Variables
//        /// <summary>
//        /// Identifiant de session
//        /// </summary>
//        public string idsession;	
//        /// <summary>
//        /// Code HTML du résultat
//        /// </summary>
//        public string result="";
//        /// <summary>
//        /// Texte Agence média
//        /// </summary>
//        /// <summary>
//        /// Controle agence média
//        /// </summary>
//        /// <summary>
//        /// Script de fermeture du flash d'attente
//        /// </summary>
//        public string divClose=LoadingSystem.GetHtmlCloseDiv();
//        /// <summary>
//        /// Menu contextuel
//        /// </summary>
//        /// <summary>
//        /// Bool précisant si l'on doit afficher la liste des années pour les agences médias
//        /// </summary>
//        public bool displayMediaAgencyList=true;
//        #endregion

//        #region Constructeur
//        /// <summary>
//        /// Constructeur, chargement de la session
//        /// </summary>
//        public MarketShareResults():base(){
			
//            // Chargement de la Session
//            try{				
//                idsession=HttpContext.Current.Request.QueryString.Get("idSession");
//            }
//            catch(System.Exception ){
//                Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
//                Response.Flush();
//            }
//        }
//        #endregion

//        #region Evènements

//        #region Chargement de la page
//        /// <summary>
//        /// Chargement de la page
//        /// </summary>
//        /// <param name="sender">Objet qui lance l'évènement</param>
//        /// <param name="e">Arguments</param>
//        protected void Page_Load(object sender, System.EventArgs e){
		
//            try{

//                #region Gestion du flash d'attente
//                if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
//                    string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
//                    if(nomInput!=MenuWebControl2.ID){
//                        Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//                        Page.Response.Flush();
//                    }
//                }
//                else{
//                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//                    Page.Response.Flush();
//                }
//                #endregion

//                #region Url Suivante
////				_nextUrl=this.recallWebControl.NextUrl;
//                if(_nextUrl.Length!=0){
//                    _webSession.Source.Close();
//                    Response.Redirect(_nextUrl+"?idSession="+idsession);
//                }			
//                #endregion

//                #region Définition de la page d'aide
////				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"MarketShareResultsHelp.aspx";
//                #endregion

//                #region Textes et Langage du site
//                //TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
//                Moduletitlewebcontrol2.CustomerWebSession=_webSession;
//                ModuleBridgeWebControl1.CustomerWebSession=_webSession;
//                InformationWebControl1.Language = _webSession.SiteLanguage;
////				ExportWebControl1.CustomerWebSession=_webSession;			
//                #endregion			

//                #region Inialisation de l'attribut Percentage 
//                _webSession.Percentage=true;
//                _webSession.Save();
//                #endregion

//                #region Sélection du vehicle
//                string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
//                DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//                if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
//                #endregion

//                #region Agence média
//                displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
//                #endregion

//                #region choix du type d'encarts
						
//                if(DBClassificationConstantes.Vehicles.names.press==vehicleName 
//                    || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){

//                    #region Affichage encarts en fonction du module
//                    switch(_webSession.CurrentModule){
//                        case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
//                        case WebConstantes.Module.Name.ALERTE_POTENTIELS:
//                        case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
//                        case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
//                            ResultsOptionsWebControl1.InsertOption=true; 																																	
//                            break;
//                        default :
//                            ResultsOptionsWebControl1.InsertOption=false;	
//                            break;
//                    }
//                    #endregion					

//                }			
//                else ResultsOptionsWebControl1.InsertOption=false;
			
//                #endregion

//                #region Initialisation de preformatedProductDetail et de l'unité dans le cas de la presse
//                // Initialisation de preformatedProductDetail				
//                if(_webSession.PreformatedProductDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct
//                    || !_webSession.ReachedModule
//                    ) {			
//                    _webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;	 
//                }

//                if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.press.GetHashCode().ToString()
//                    && !_webSession.ReachedModule
//                    ){
//                    _webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
//                }
//                if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode().ToString()
//                    && !_webSession.ReachedModule
//                    ){
//                    _webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
//                }
//                #endregion

//                #region Résultat	
//                //result= WebBF.Results.MarketShareSystem.GetHtml(Page,_webSession);		
//                #endregion

//                #region MAJ _webSession
//                _webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
//                _webSession.ReachedModule=true;
//                _webSession.Save();
//                #endregion
//            }
//            catch(System.Exception exc){
//                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
//                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
//                }
//            }
//        }
//        #endregion

//        #region Déchargement de la page
//        /// <summary>
//        /// Evènement de déchargement de la page
//        /// </summary>
//        /// <param name="sender">Objet qui lance l'évènement</param>
//        /// <param name="e">Arguments</param>
//        protected void Page_UnLoad(object sender, System.EventArgs e){
			
//        }
//        #endregion

//        #region DeterminePostBackMode
//        /// <summary>
//        /// Evaluation de l'évènement PostBack:
//        ///		base.DeterminePostBackMode();
//        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
//        /// </summary>
//        /// <returns></returns>
//        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
//            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
//            ResultsOptionsWebControl1.CustomerWebSession = _webSession;
//            InitializeProductWebControl1.CustomerWebSession=_webSession;
//            MediaAgencyYearWebControl1.WebSession=_webSession;
//            MenuWebControl2.CustomerWebSession = _webSession;
//            _resultWebControl.CustomerWebSession=_webSession;
//            _genericMediaLevelDetailSelectionWebControl.CustomerWebSession=_webSession;
			
//            return tmp;
//        }
//        #endregion

//        #region Code généré par le Concepteur Web Form
//        /// <summary>
//        /// Evènement d'initialisation
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        override protected void OnInit(EventArgs e){
//            //
//            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
//            //
//            InitializeComponent();
//            base.OnInit(e);
//        }
		
//        /// <summary>
//        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
//        /// le contenu de cette méthode avec l'éditeur de code.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.Unload += new System.EventHandler(this.Page_UnLoad);
           
//        }
//        #endregion

//        #region Prérender
//        /// <summary>
//        /// OnPreRender
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnPreRender(EventArgs e) {
//            base.OnPreRender (e);
//            try{

//                #region MAJ _webSession
//                _webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
//                _webSession.Save();
//                #endregion

//                //				ResultTable resultTable=TNS.AdExpress.Web.BusinessFacade.Results.CompetitorSystem.GetHtml(_webSession);
//                //				WebControlResultTable1.Data=resultTable;
			
//            }
//            catch(System.Exception exc){
//                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
//                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
//                }
//            }
//        }
//        #endregion
		
//        #region Render
//        /// <summary>
//        /// Render
//        /// </summary>
//        /// <param name="output">Sortie</param>
//        protected override void Render(HtmlTextWriter output){

//            ResultsOptionsWebControl1.PercentageCheckBox.Enabled=false;
//            base.Render(output);
//        }
//        #endregion

//        #endregion

//        #region Implémentation méthodes abstraites
//        /// <summary>
//        /// Retrieve next Url from the menu
//        /// </summary>
//        /// <returns>Next Url</returns>
//        protected override string GetNextUrlFromMenu(){
//            return(this.MenuWebControl2.NextUrl);
//        }
//        #endregion
	}
}
