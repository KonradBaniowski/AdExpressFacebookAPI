#region Information
/*
 * auteur : D. V. MUSSUMA
 * créé le : 30/09/2004
 * modifié le : 
 *	30/12/2004  D. Mussuma Intégration de WebPage
 *      12/08/2008 - G Ragneau - Layers Analyse Secto
 * */
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Translation.Functions;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
#endregion

namespace AdExpress.Private.Results
{

    /// <summary>
    ///Résultas des différentes planches indicateurs :
    ///- Saisonnalité : répartition des investissements par média et par annonceurs ou références.
    ///- totalChoice : le top 10 des annonceurs et références ayant le plus investi sur la période sélectionnée
    ///en fonction du média.
    ///- Nouveautés : nouveaux annonceurs ou références sur la période sélectionnée.
    ///- Evolution : annonceurs et références dont l'investissement a le plus augmenté entre la période N-1 et N.
    ///- Stratégie Média : répartion des investissements par média et par annonceurs et références.
    /// </summary>
    public partial class IndicatorSeasonalityResults : TNS.AdExpress.Web.UI.ResultWebPage
    {

        #region variables
        /// <summary>
        /// Code HTML des résultats
        /// </summary>
        public string result;
        /// <summary> 
        /// Script de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// Choix du type de total (bas sélection, famille, marché)
        /// </summary>
        public bool totalChoice = true;
       
        /// <summary>
        /// Commentaire Agrandissement de l'image
        /// </summary>
        public string zoomTitle = "";

        private bool _withAdType = false;

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur : chargement de la session
        /// </summary>
        public IndicatorSeasonalityResults()
            : base()
        {
            // Chargement de la Session			
            _webSession.CurrentModule = CstWeb.Module.Name.INDICATEUR;
        }
        #endregion

        #region Evènements

        #region chargement de la page
        /// <summary>
        /// Chargement de la page
        /// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                #region Gestion du flash d'attente
                if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
                {
                    string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                    if (nomInput != MenuWebControl2.ID)
                    {
                        Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                        Page.Response.Flush();
                    }
                }
                else
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
                #endregion

                #region Url Suivante
                if (_nextUrl.Length != 0)
                {
                    _webSession.Source.Close();
                    Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
                }
                #endregion

                #region Textes et Langage du site
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
                ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191, _webSession.SiteLanguage);
                ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192, _webSession.SiteLanguage);
                zoomTitle = GestionWeb.GetWebWord(1235, _webSession.SiteLanguage);
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                DetailPeriodWebControl1.WebSession = _webSession;

                VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

                if (_webSession.CurrentTab == CstResult.MediaStrategy.MEDIA_STRATEGY)
                {
                    if (vehicleInfo != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null
                        && !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)
                        && !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)
                        && vehicleInfo.Id != TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia)
                    {
                        _webSession.Graphics = false;
                        ResultsOptionsWebControl1.GraphRadioButton.Checked = false;
                        ResultsOptionsWebControl1.GraphRadioButton.Visible = false;
                        ResultsOptionsWebControl1.TableRadioButton.Visible = false;
                        _webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicle;
                        _webSession.Save();
                    }
                }
                else
                {
                    ResultsOptionsWebControl1.GraphRadioButton.Visible = true;
                    ResultsOptionsWebControl1.TableRadioButton.Visible = true;
                }

                if (!IsPostBack)
                {
                    ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
                    ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
                }
                else
                {
                    if (_webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS
                        && (!ResultsOptionsWebControl1.GraphRadioButton.Checked && !ResultsOptionsWebControl1.TableRadioButton.Checked)
                        )
                    {
                        ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
                        ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
                    }
                    else
                    {
                        _webSession.Graphics = ResultsOptionsWebControl1.GraphRadioButton.Checked;
                    }
                }
                
                #region Résultat
                ResultsOptionsWebControl1.ResultFormat = true;
                ResultsOptionsWebControl1.MediaDetailOption = false;
                try
                {
                    switch(_webSession.CurrentTab){
                        case CstResult.SynthesisRecap.SYNTHESIS: //Summary                          
                            ResultsOptionsWebControl1.ResultFormat = false;
                            break;
                        case CstResult.SynthesisRecap.SEASONALITY: //Seasonality
                            if(_webSession.Graphics)
                                ProductClassContainerWebControl1.BigSize = ResultsOptionsWebControl1.IsZoomGraphicChecked;
                            break;
                        case CstResult.SynthesisRecap.PALMARES: //palmares (the coponent detects if it is a grop or not)
                            break;
                        case CstResult.Novelty.NOVELTY: //Novelty
                            break;
                        case CstResult.PalmaresRecap.EVOLUTION: //EVOL
                            if(_webSession.Graphics){
                                // Cas année N-2
                                DateTime PeriodBeginningDate = FctUtilities.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                                if (((PeriodBeginningDate.Year == (System.DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1))) && DateTime.Now.Year <= _webSession.DownLoadDate)
                                    || ((PeriodBeginningDate.Year == (System.DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear)) && DateTime.Now.Year > _webSession.DownLoadDate)
                                    )
                                {
                                    _webSession.Graphics = false;
                                }
                            }
                            break;
                        case CstResult.MediaStrategy.MEDIA_STRATEGY:
                            ResultsOptionsWebControl1.MediaDetailOption = true;
                            if(_webSession.Graphics){
                                if(vehicleInfo != null){
                                    // si WebSession est au niveau media on se met au niveau categorie/media
                                    if(_webSession.PreformatedMediaDetail == CstPreformatedDetail.PreformatedMediaDetails.vehicle && vehicleInfo.Id != CstDBClassif.Vehicles.names.plurimedia){
                                        _webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory;
                                    }
                                }
                    }
                            break;
                    }
                }
                catch (System.Exception){
                    result = noResult("");
                }
                #endregion

                #region MAJ _webSession
                _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                _webSession.ReachedModule = true;               
                #endregion

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region OnLoadComplete
        /// <summary>
        /// OnLoadComplete
        /// </summary>
        /// <param name="e">Args</param>
        protected override void  OnLoadComplete(EventArgs e){
            _webSession.Save();
 	        base.OnLoadComplete(e);
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
			ModuleBridgeWebControl1.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
            ProductClassContainerWebControl1.WebSession = _webSession;
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
            ResultPageInformation resultPageInformation = module.GetResultPageInformation(_webSession.CurrentTab);
            ResultsOptionsWebControl1.UnitOption = resultPageInformation.CanDisplayUnitOption;
            //ResultsOptionsWebControl1.CampaignTypeOption = resultPageInformation.CanDisplayCampaignType;
           
			return tmp;
		}
		#endregion

        #region Aucun Résultat
        /// <summary>
        /// Affichage d'un message d'erreur
        /// </summary>
        /// <returns></returns>
        private string noResult(string message)
        {
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
            t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
            t.Append(GestionWeb.GetWebWord(177, _webSession.SiteLanguage) + " " + message);
            t.Append("</td></tr></table>");
            return t.ToString();
        }
        #endregion

        #region OnPreInit
        /// <summary>
        /// OnPreInit
        /// </summary>
        /// <param name="e">Args</param>
        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);
        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
		/// Initialisation
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


        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            
                // Suppime les items inutiles de la list "niveau media" pour le graphe de strategie Media
                if (_webSession.CurrentTab == CstResult.PalmaresRecap.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked)
                {
                    VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                    if (vehicleInfo != null)
                    {
                        switch (vehicleInfo.Id)
                        {
                            case CstDBClassif.Vehicles.names.tv:
                            case CstDBClassif.Vehicles.names.tvGeneral:
                            case CstDBClassif.Vehicles.names.tvSponsorship:
                            case CstDBClassif.Vehicles.names.tvAnnounces:
                            case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                            case CstDBClassif.Vehicles.names.radio:
                            case CstDBClassif.Vehicles.names.radioGeneral:
                            case CstDBClassif.Vehicles.names.radioMusic:
                            case CstDBClassif.Vehicles.names.radioSponsorship:
                            case CstDBClassif.Vehicles.names.outdoor:
                            case CstDBClassif.Vehicles.names.indoor:
                            case CstDBClassif.Vehicles.names.instore: 
                            case CstDBClassif.Vehicles.names.mediasTactics:
                            case CstDBClassif.Vehicles.names.mobileTelephony:
                            case CstDBClassif.Vehicles.names.emailing:
                                ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
                                break;
                            case CstDBClassif.Vehicles.names.press:
                            case CstDBClassif.Vehicles.names.newspaper:
                            case CstDBClassif.Vehicles.names.magazine:
                            case CstDBClassif.Vehicles.names.internet:
                            case CstDBClassif.Vehicles.names.czinternet:
                                ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
                                break;
                            case CstDBClassif.Vehicles.names.cinema:
                                result = noResult("");
                                break;
                            case CstDBClassif.Vehicles.names.plurimedia:
                                ResultsOptionsWebControl1.mediaDetail.Enabled = true;
                                break;
                            default:
                                ResultsOptionsWebControl1.mediaDetail.Enabled = false;
                                break;
                        }
                    }
                }
                base.Render(output);
           
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Get next Url from contextual menu
        /// </summary>
        /// <returns></returns>
        protected override string GetNextUrlFromMenu()
        {
            return this.MenuWebControl2.NextUrl;
        }
        #endregion

        #region SetAdvertisementTypeOptions
        ///// <summary>
        ///// Indicate if customer can refine ad type
        ///// </summary>		
        //private void SetAdvertisementTypeOptions()
        //{   //TODO : A déplacer dans resultoptionswebcontrol pour fusion Dev Trunk
        //    //TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
        //    //ArrayList detailSelections = ((ResultPageInformation)module.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
        //    //if (detailSelections.Contains(CstWeb.DetailSelection.Type.advertisementType.GetHashCode()))
        //    //{
        //    //    //InitializeProductWebControl1.Visible = true;
        //    //    ResultsOptionsWebControl1.InitializeAdvertisementTypeOption = true;
        //    //    _withAdType = true;
        //    //}
        //}
        #endregion

    }
}
