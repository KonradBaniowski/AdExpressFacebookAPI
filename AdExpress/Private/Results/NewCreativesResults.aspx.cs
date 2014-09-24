#region Informations
// Auteur: B.Masson
// Date de création: 30/09/2008
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

using Domain = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using System.Reflection;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;

using NewCreatives = TNS.AdExpressI.NewCreatives;
using TNS.AdExpress.DataAccess;
using TNS.AdExpress.Domain.Web;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// New creatives result page
	/// </summary>
    public partial class NewCreativesResults : TNS.AdExpress.Web.UI.ResultWebPage {

        #region Variables
        /// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();				
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
        public NewCreativesResults(): base() {			
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

                #region Avaliant Country Access List
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new Exception("Uncorrect Media Selection"));
                VehicleInformation vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
                if (WebApplicationParameters.ApplyEvaliantCountryAccess && vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack 
                    && (_webSession.EvaliantCountryAccessList == null || _webSession.EvaliantCountryAccessList.Length == 0))
                    _webSession.EvaliantCountryAccessList = RightDAL.GetEvaliantCountryRights(_webSession.Source, _webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], _webSession.CustomerLogin[CustomerRightConstante.type.mediaException], _webSession.DataLanguage);
                #endregion

                #region Url Suivante
                if (_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion

                DetailPeriodWebControl1.WebSession = _webSession;

                //#region Period Detail
                //if(!IsPostBack)
                //    PeriodDetailWebControl1.Select(_webSession.DetailPeriod);
                //else 
                //    _webSession.DetailPeriod = PeriodDetailWebControl1.SelectedValue;
                //#endregion

                //#region Résultat
                //// Affichage de tous les produits
                //if(Request.Form.Get("__EVENTTARGET")=="InitializeButton"){
                //    _webSession.CurrentUniversProduct.Nodes.Clear();
                //    _webSession.SelectionUniversProduct.Nodes.Clear();
                //    _webSession.Save();
                //}
                //_webSession.Save();
                //#endregion
						
				#region Textes et Langage du site
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion
			
				#region Scripts
                //// Ouverture de la popup chemin de fer
                //if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioCreation")) {
                //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"portofolioCreation",TNS.AdExpress.Web.Functions.Script.PortofolioCreation());
                //}
				//if (!Page.ClientScript.IsClientScriptBlockRegistered("openCreation"))
                //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openCreation",WebFunctions.Script.OpenCreation());
                //// Ouverture de la popup detail portefeuille
                //if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioDetailMedia")) {
                //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"portofolioDetailMedia",TNS.AdExpress.Web.Functions.Script.PortofolioDetailMedia());
                //}	
				#endregion              
                
                //#region Sector list
                //DataTable dt = new TNS.AdExpress.DataAccess.Classification.ProductBranch.AllSectorLevelListDataAccess(_webSession.DataLanguage, _webSession.Source).GetDataTable;
                //SectorWebControl1.Session = _webSession;
                //SectorWebControl1.DataTable = dt;
                //SectorWebControl1.LanguageCode = _webSession.SiteLanguage;
                //#endregion

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
            MenuWebControl2.CustomerWebSession = _webSession;

            ResultsOptionsWebControl1.CustomerWebSession = _webSession;

            //PeriodDetailWebControl1.Session = _webSession;
            //PeriodDetailWebControl1.LanguageCode = _webSession.SiteLanguage;
            //_genericMediaLevelDetailSelectionWebControl.CustomerWebSession = _webSession;
            _ResultWebControl.CustomerWebSession = _webSession;
            
            // Period
            //PeriodDetailWebControl1.Visible = true;
            //_webSession.DetailPeriod = PeriodDetailWebControl1.SelectedValue;
            if(_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly) {
                if(WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                    < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)) {
                    _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                }
            }
			return tmp;
		}
		#endregion

		#region PreRender
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
