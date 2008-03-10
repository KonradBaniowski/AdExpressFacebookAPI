#region Informations
// Auteur: K. Shehzad
// Date de cr�ation: 11/07/2005
// Date de modification: 
#endregion

#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
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
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Level;

#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page d'affichage des r�sultats du bilan de campagne.
	/// </summary>
	public partial class APPMResults : TNS.AdExpress.Web.UI.ResultWebPage{

		#region variables
		/// <summary>
		/// Code HTML des r�sultats
		/// </summary>
		public string result="";
		/// <summary>
		/// Date r�cup�rer dans l'url
		/// </summary>
		public string date="";				
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();	
//		/// <summary>
//		/// Commentaire Pr�sentation graphique
//		/// </summary>
//		public string chartTitle="";
//		/// <summary>
//		/// Commentaire Pr�sentation tableau
//		/// </summary>
//		public string tableTitle="";
		/// <summary>
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";
		/// <summary>
		/// Affiche les graphiques
		/// </summary>
		public bool displayChart=false;
		/// <summary>
		///  bool�en pr�cisant si l'on doit afficher les agences m�dias
		/// </summary>
		public bool displayMediaAgencyList=false;
//		/// <summary>
//		///  bool�en pr�cisant si l'on doit afficher la selection des graphes
//		/// </summary>
//		public bool graphes=false;
		#endregion
		
		#region variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options des r�sultats
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Texte : agence m�dia
		/// </summary>
//		/// <summary>
//		/// Choix pr�sentation graphique
//		/// </summary>
//		protected System.Web.UI.WebControls.RadioButton graphRadioButton;
//		/// <summary>
//		/// Choix pr�sentation tableau
//		/// </summary>
//		protected System.Web.UI.WebControls.RadioButton tableRadioButton;		
//		/// <summary>
//		/// Graphique du APPM
//		/// </summary>
//		public TNS.AdExpress.Web.UI.Results.APPM.APPMChartUI APPMChart;
		/// <summary>
		/// Contr�le de la barre d'en-t�te
		/// </summary>
		/// <summary>
		/// texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext2;
		/// <summary>
		/// Conteneur des composants destin�s � l'APPM.
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Annule s�lection produit
		/// </summary>
		/// <summary>
		/// Contr�le du choix de l'ann�e des agences m�dia
		/// </summary>
		

		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une m�thode contenue dans UI est appel�
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try {

				#region Gestion du flash d'attente
				//				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null)
				//				{
				//					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				//					if(nomInput!=recallWebControl.ID)
				//					{
				//						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				//						Page.Response.Flush();
				//					}
				//				}
				//				else
				//				{
				//					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				//					Page.Response.Flush();
				//				}
				#endregion

				#region Url Suivante
				//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0) {
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Agence m�dia
				//				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis)
				//				{
				//					displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
				//					//displayMediaAgencyList=false;
				//
				//				}
			
				#endregion

				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				_siteLanguage=_webSession.SiteLanguage;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region D�finition de la page d'aide
				//			    helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"APPMResultsHelp.aspx";
				#endregion

				#region Planche des graphiques

				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan 
					||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan 
					||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.interestFamily){

					#region Choix affichage graphique ou tableau ( planche P�riodicit� du plan)
					//				chartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
					ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
					//				tableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);	
					ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);		
					//				graphRadioButton.Visible=false;
					//				tableRadioButton.Visible=false;						
					#endregion	

					if(!IsPostBack){
						//					graphRadioButton.Checked=_webSession.Graphics;
						//					tableRadioButton.Checked=!_webSession.Graphics;
						ResultsOptionsWebControl1.GraphRadioButton.Checked =_webSession.Graphics; 
						ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
					}else{
						//					_webSession.Graphics=graphRadioButton.Checked;
						_webSession.Graphics=ResultsOptionsWebControl1.GraphRadioButton.Checked;
						_webSession.Save();
					}
					if(_webSession.Graphics){
						//					ExportWebControl1.JpegFormatFromWebPage=true;
						//					graphRadioButton.Checked=true;
						ResultsOptionsWebControl1.GraphRadioButton.Checked=true;
					}else{
						//					tableRadioButton.Checked=true;
						ResultsOptionsWebControl1.TableRadioButton.Checked=true;
						//					ExportWebControl1.JpegFormatFromWebPage=false;
					}
					//				if(graphRadioButton.Checked){
					//				if(ResultsOptionsWebControl1.GraphRadioButton.Checked){
					//					displayChart=true;
					//				}
				
				}
				#endregion	
				
				AppmContainerWebControl1.Source = this._dataSource;	
				
				#region Niveau de d�tail media (Generic)
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion ){
					_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan;
					#region Niveau de d�tail media (Generic) pour le calendriers d'action par version
					// Initialisation � media\cat�gorie\Support\Version
					ArrayList levels=new ArrayList();
					levels.Add(1);
					levels.Add(2);
					levels.Add(3);
					levels.Add(6);
					_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					#endregion
				}else{
					_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					
					// Initialisation � media\cat�gorie
					ArrayList levels=new ArrayList();
					levels.Add(1);
					levels.Add(2);
					_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					
				}
				#endregion
				
				#region Option affiner version 
				if(!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(_webSession) || !WebFunctions.MediaDetailLevel.HasSloganRight(_webSession)
					|| _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion){//droits affiner univers Versions				
					InitializeProductWebControl1.Visible = false;
					MenuWebControl2.ForbidOptionPages = true;
					_webSession.IdSlogans=new ArrayList();
				
				}else{
					InitializeProductWebControl1.Visible = true;
					MenuWebControl2.ForbidOptionPages = false;
				}
				#endregion

				_webSession.Save();
			
				#region MAJ _webSession
				//in prerender
				//				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				//				_webSession.ReachedModule=true;
				//				_webSession.Save();
				#endregion


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
				#region r�sultat
				//result=WebBF.Results.APPMSystem.GetHtml(this.Page,this.APPMChart,_webSession,this._dataSource);			
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

			base.OnPreRender (e);
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns>collections tri�es de valeurs</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() 
		{			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			MediaAgencyYearWebControl1.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			#region chargement de l'univers
						
			//_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;			

			#endregion

			//Conteneur des composants de l'APPM
			AppmContainerWebControl1.CustomerWebSession = _webSession;
			AppmContainerWebControl1.AppmImageType = ChartImageType.Flash;
			
			InitializeProductWebControl1.CustomerWebSession	=_webSession;		
			
			_webSession.Save();
			return tmp;
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
			
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			AppmOptionsWebControl();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent()
		{
           

		}
		#endregion
		
		#region m�thodes internes

		#region webcontrols to show and hide
		/// <summary>
		/// Contr�les fils � afficher dans le contr�le du choix des options 
		/// </summary>
		private void AppmOptionsWebControl()
		{

			switch (_webSession.CurrentTab){
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.interestFamily:					
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan:
					ResultsOptionsWebControl1.UnitOptionAppm=true;					
//					graphes=true;
					ResultsOptionsWebControl1.ResultFormat=true;
					_webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis :
					displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();
					ResultsOptionsWebControl1.ProductsOption=true;	
					if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
						//unit� en euro pour cette planche
						_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
						_webSession.Save();
					}
					_webSession.Graphics =false;

					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan:
					if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
						//unit� en euro pour cette planche
						_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
						_webSession.Save();
					}
//					graphes=true;
					_webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					ResultsOptionsWebControl1.ResultFormat=true;
					break;				
//				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.locationPlanType:
//					ResultsOptionsWebControl1.UnitOptionAppm=true;
////					graphes=false;
//					ResultsOptionsWebControl1.ResultFormat=false;
//					_webSession.Graphics =false;
//					break;	
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlan:	
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.supportPlan:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.affinities:
					if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
						//unit� en euro pour ces planches

					_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
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
