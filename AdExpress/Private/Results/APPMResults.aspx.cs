#region Informations
// Auteur: K. Shehzad
// Date de création: 11/07/2005
// Date de modification: 
#endregion

#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Level;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Functions;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page d'affichage des résultats du bilan de campagne.
	/// </summary>
    public partial class APPMResults : TNS.AdExpress.Web.UI.BaseResultWebPage {

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
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";
		/// <summary>
		/// Affiche les graphiques
		/// </summary>
		public bool displayChart=false;
		/// <summary>
		///  booléen précisant si l'on doit afficher les agences médias
		/// </summary>
		public bool displayMediaAgencyList=false;
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string SetZoom = "";
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string zoomButton = "";
        /// <summary>
        /// Analysis Period
        /// </summary>
        protected MediaSchedulePeriod _period;
        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _zoom;
        /// <summary>
        /// Initial Period Detail Saving
        /// </summary>
        ConstantesPeriod.DisplayLevel _savePeriod = ConstantesPeriod.DisplayLevel.monthly;
        #endregion
		
		#region variables MMI
		/// <summary>
		/// texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext2;
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try {



				#region Url Suivante
				if(_nextUrl.Length!=0) {
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Textes et Langage du site
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				_siteLanguage=_webSession.SiteLanguage;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				//mediaAgencyText.Language = _webSession.SiteLanguage;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                HeaderWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region Planche des graphiques

				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan 
					||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan 
					||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.interestFamily){

					#region Choix affichage graphique ou tableau ( planche Périodicité du plan)
					ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
					ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);		
					#endregion	

					if(!IsPostBack){
						ResultsOptionsWebControl1.GraphRadioButton.Checked =_webSession.Graphics; 
						ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
					}else{
						_webSession.Graphics=ResultsOptionsWebControl1.GraphRadioButton.Checked;
						_webSession.Save();
					}
					if(_webSession.Graphics){
						ResultsOptionsWebControl1.GraphRadioButton.Checked=true;
					}else{
						ResultsOptionsWebControl1.TableRadioButton.Checked=true;
					}
				
				}
				#endregion	
				
                AppmContainerWebControl1.Source = _webSession.Source;	
				
				#region Niveau de détail media (Generic)
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion ){
					_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan;
					#region Niveau de détail media (Generic) pour le calendriers d'action par version
					// Initialisation à media\catégorie\Support\Version
					ArrayList levels=new ArrayList();
					levels.Add(1);
					levels.Add(2);
					levels.Add(3);
					levels.Add(6);
					_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					#endregion
				}else{
					_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;

                    if (_webSession.CurrentTab != TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan)
                    {
                        // Initialisation à media\catégorie
                        ArrayList levels = new ArrayList();
                        levels.Add(1);
                        levels.Add(2);
                        _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    }
                    else
                    {
                        //MediaCategoriSupport
                        ArrayList levels = new ArrayList();
                        levels.Add(2);
                        levels.Add(3);
                        _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

                    }
				}
				#endregion
				
				#region Option affiner version 
				if (!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(_webSession) || !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)
					|| _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion){//droits affiner univers Versions				
					InitializeProductWebControl1.Visible = false;
					MenuWebControl2.ForbidOptionPages = true;
					_webSession.IdSlogans=new ArrayList();
				
				}else{
					InitializeProductWebControl1.Visible = true;
					MenuWebControl2.ForbidOptionPages = false;
				}
				#endregion

                #region Period Detail
                if (_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan &&_zoom != null && _zoom != string.Empty)
                {
                    zoomButton = string.Format("<tr class=\"whiteBackGround\" ><td align=\"left\"><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"30\" height=\"8\" VIEWASTEXT><param name=movie value=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\"><param name=quality value=\"high\"><param name=menu value=\"false\"><embed src=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\" width=\"30\" height=\"8\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" menu=\"false\"></embed></object><a class=\"roll06\" href=\"/Private/Results/APPMResults.aspx?idSession={0}\">{2}</a></td></tr><tr><td class=\"whiteBackGround\" height=\"5\"></td></tr>",
                        _webSession.IdSession,
                        _webSession.SiteLanguage,
                        GestionWeb.GetWebWord(2309, _webSession.SiteLanguage));

                }
                #endregion

                _webSession.Save();
			

			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
			
		}
		
		#endregion

		#region PreRender
		/// <summary>
		/// to calculate the result for the page
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnPreRender(EventArgs e)
		{

			
			try{
				
				#region MAJ _webSession
                if (_zoom != null && _zoom != string.Empty)
                {
                    _webSession.DetailPeriod = _savePeriod;
                }
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

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns>collections triées de valeurs</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() 
		{			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			//MediaAgencyYearWebControl1.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
            AppmContainerWebControl1.CustomerWebSession = _webSession;
            GenericMediaScheduleWebControl1.CustomerWebSession = _webSession;
            GenericMediaScheduleWebControl1.Module = ModulesList.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);
            SubPeriodSelectionWebControl1.WebSession = _webSession;
            _zoom = Page.Request.QueryString.Get("zoomDate");

            #region Result params
            if ((Page.Request.Form.GetValues("_resultsPages") == null && _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan)
                || (Page.Request.Form.GetValues("_resultsPages") != null && Int64.Parse(Page.Request.Form.GetValues("_resultsPages")[0]) == TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan)
                )
            {
                GenericMediaScheduleWebControl1.Visible = true;
                AppmContainerWebControl1.Visible = false;



                #region Period Detail
                if (_zoom != null && _zoom != string.Empty)
                {
                    if (Page.Request.Form.GetValues("zoomParam") != null && Page.Request.Form.GetValues("zoomParam")[0].Length > 0)
                    {
                        _zoom = Page.Request.Form.GetValues("zoomParam")[0];
                    }
                    MenuWebControl2.UrlPameters = string.Format("zoomDate={0}", _zoom);
                    MenuWebControl2.ForbidSave = true;
                    SubPeriodSelectionWebControl1.Visible = true;
                    SubPeriodSelectionWebControl1.AllPeriodAllowed = false;
                    _savePeriod = _webSession.DetailPeriod;
                    _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                    GenericMediaScheduleWebControl1.ZoomDate = _zoom;
                    SubPeriodSelectionWebControl1.ZoomDate = _zoom;
                    SubPeriodSelectionWebControl1.JavascriptRefresh = "SetZoom";
                    SubPeriodSelectionWebControl1.PeriodContainerName = GenericMediaScheduleWebControl1.ZoomDateContainer;
                    zoomParam.Value = _zoom;

                    #region SetZoom
                    StringBuilder js = new StringBuilder();
                    js.Append("\r\n<script type=\"text/javascript\">");
                    js.Append("\r\nfunction SetZoom(){");
                    js.AppendFormat("\r\n\tif ({0} == '')", SubPeriodSelectionWebControl1.PeriodContainerName);
                    js.Append("\r\n\t{");
                    js.AppendFormat("\r\n\t\tdocument.location='/Private/Results/MediaPlanResults.aspx?idSession={0}'", _webSession.IdSession);
                    js.Append("\r\n\t}");
                    js.Append("\r\n\telse {");
                    js.AppendFormat("\r\n\t\tvar date = document.getElementById(\"zoomParam\").value;");
                    js.AppendFormat("\r\n\t\t{0}();", GenericMediaScheduleWebControl1.RefreshDataMethod);
                    js.AppendFormat("\r\n\t\texportMenu.items.printMenuItem.actionOnClick = exportMenu.items.printMenuItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                    js.AppendFormat("\r\n\t\tmenu.items.selectedItem.actionOnClick = menu.items.selectedItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                    js.AppendFormat("\r\n\t\tdocument.getElementById(\"zoomParam\").value = {0};", SubPeriodSelectionWebControl1.PeriodContainerName);
                    js.Append("\r\n\t}");
                    js.Append("\r\n}");
                    js.Append("\r\n</script>");
                    SetZoom = js.ToString();
                    #endregion

                    #region script
                    if (!this.ClientScript.IsClientScriptBlockRegistered("SetZoom")) this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetZoom", SetZoom);
                    #endregion

                }
                else
                {
                    SubPeriodSelectionWebControl1.Visible = false;

                }
                #endregion


            }
            else
            {
                GenericMediaScheduleWebControl1.Visible = false;
                SubPeriodSelectionWebControl1.Visible = false;
                AppmContainerWebControl1.Visible = true;
                //Conteneur des composants de l'APPM
                AppmContainerWebControl1.AppmImageType = ChartImageType.Flash;
            }
            #endregion


			InitializeProductWebControl1.CustomerWebSession	=_webSession;		
			
			_webSession.Save();
			return tmp;
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
			
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			AppmOptionsWebControl();

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
		
		#region méthodes internes

		#region webcontrols to show and hide
		/// <summary>
		/// Contrôles fils à afficher dans le contrôle du choix des options 
		/// </summary>
		private void AppmOptionsWebControl()
		{

			switch (_webSession.CurrentTab){
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.interestFamily:					
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan:
					ResultsOptionsWebControl1.UnitOptionAppm=true;					
					ResultsOptionsWebControl1.ResultFormat=true;
					_webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis :
					//displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
					ResultsOptionsWebControl1.ProductsOption=true;	
					_webSession.Graphics =false;
                    if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.kEuro) {
                        //unité en euro pour cette planche
                        _webSession.Unit = WebConstantes.CustomerSessions.Unit.euro;
                        _webSession.Save();
                    }
                    _webSession.Graphics = false;
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan:
                    if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.kEuro) {
                        //unité en euro pour cette planche
                        _webSession.Unit = WebConstantes.CustomerSessions.Unit.euro;
                        _webSession.Save();
                    }
//					graphes=true;
					_webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					ResultsOptionsWebControl1.ResultFormat=true;
					break;				
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan:	
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.supportPlan:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.affinities:
                    if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.kEuro) {
                        //unité en euro pour ces planches

                        _webSession.Unit = WebConstantes.CustomerSessions.Unit.euro;
                        _webSession.Save();
                    }
					_webSession.Graphics =false;
					_webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					break;
			}
		}
		#endregion

		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve Next Url from Contextual Menu
		/// </summary>
		/// <returns>Next URL</returns>
		protected override string GetNextUrlFromMenu() {
			return MenuWebControl2.NextUrl;
		}

		#endregion
	}
}
