#region Informations
// Auteur: A. Obermeyer & D. V. Mussuma
// Date de création : 8/02/2005
// Date de modification :
// 
#endregion

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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Constantes.Customer;
//using AdExpressException=AdExpress.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{

	/// <summary>
	/// Page de résultat du module Tendance
	/// </summary>
	public partial class TendenciesResult : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables MMI
		/// <summary>
		/// Contrôle : Titre du Module
		/// </summary>
		/// <summary>
		/// Contrôle : Options
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ResultsOptionsWebControl ResultsOptionsWebControl1;
		/// <summary>
		/// bouton de validation des options
		/// </summary>
		/// <summary>
		/// module bridge
		/// </summary>
		/// <summary>
		/// Contrôle En-tête
		/// </summary>
		/// <summary>
		/// Contrôle options des tendances
		/// </summary>
		/// <summary>
		/// Contrôle choix dates mensuelles
		/// </summary>
		/// <summary>
		/// Contrôle choix dates hebdomadaires
		/// </summary>
		/// <summary>
		/// texte
		/// </summary>
		/// <summary>
		/// texte
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Résultat HTML
		/// </summary>
		public string result="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur, chargement de la session
		/// </summary>
		public TendenciesResult():base(){
			// On réinitialise en KEuro car d'anciennes sessions peuvent être en Euro
			_webSession.Unit = WebConstantes.CustomerSessions.Unit.kEuro;
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
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
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}			
				#endregion

				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;
				#endregion

				#region Sélection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
				#endregion

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;			
				#endregion			

				#region sélection de la période
				//initialisation de la péiode sélectionnée
				SelectedPeriod(vehicleName);
				if(vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
					&& vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress){
					//Chargement de la période sélectionnée
					LoadSelectedPeriod(vehicleName);
				}
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"TendenciesResultHelp.aspx";
				#endregion
				
				_webSession.Save();

				#region Traitement du cas presse
				if(vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
					|| vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress){
					ResultsDateListWebControl1.Enabled=false;
					ResultsDateListWebControl1.SelectedIndex=0;
					ResultsDateListWebControl2.Enabled=false;
					ResultsDateListWebControl2.SelectedIndex=0;						
				}
				#endregion
 
				result=TNS.AdExpress.Web.UI.Results.TendenciesUI.GetHTMLTendenciesUI(_webSession,vehicleName,false);					
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
		
		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'évènement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();		
			ResultsDashBoardOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			ResultsDateListWebControl1.WebSession=_webSession;
			ResultsDateListWebControl2.WebSession=_webSession;			
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
            this.Unload += new System.EventHandler(this.Page_UnLoad);         

		}
		#endregion

		#region Méthodes internes

		#region période sélectionnée
		/// <summary>
		/// Sélection de la période d'étude
		/// </summary>
		private void SelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName){	
			try{			
				if(!Page.IsPostBack || (vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press) 
					|| (vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress)){
					//Chargement des périodes
					if((_webSession.PeriodBeginningDate=="" || _webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate)
						){						
						setYearlyPeriod();
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=true;
					}
					else if(!isPeriodSelected(Page) && Page.IsPostBack ){
						if(vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
							|| vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress)
							setYearlyPeriod();						
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
						throw(new System.Exception(" Vous devez sélectionner une période pour faire une analyse."));
					}
					else if(vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
						|| vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress){
						setYearlyPeriod();
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=true;
	
					}
					
				}else{
					if(!isPeriodSelected(Page)){
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
                        throw (new System.Exception(" Vous devez sélectionner une période pour faire une analyse."));						
					}
				}					
			}											
			catch(Exception e){				
				Response.Write("<script language=\"JavaScript\">alert('"+e.Message+"');</script>");
			}					
		}
		#endregion
		
		#region chargement de la période sélectionnée
		/// <summary>
		/// Récupère la période sélectionnée
		/// </summary>
		private void LoadSelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName){
			#region sélection de la période	
			if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){											
				//Sélection période cumulée			
				if((vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press || vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress)  
					||( Page.Request.Form.GetValues("_cumulPeriodCheckBox")!=null && Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0]!=null && !Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0].ToString().Equals("0"))){					
					setYearlyPeriod();
					ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=true;
				}									
				//Sélection période mensuelle
				else if((vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press && vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress) 
					&& Page.Request.Form.GetValues("ResultsDateListWebControl1")!=null && Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]!=null && !Page.Request.Form.GetValues("ResultsDateListWebControl1")[0].ToString().Equals("0")){
					_webSession.PeriodBeginningDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();
					_webSession.PeriodEndDate =  Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();											
					_webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
					_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
					ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
				}			
				//Sélection période hebdomadaire
				else if((vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press && vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress) 
					&& Page.Request.Form.GetValues("ResultsDateListWebControl2")!=null && Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]!=null && !Page.Request.Form.GetValues("ResultsDateListWebControl2")[0].ToString().Equals("0")){
					_webSession.PeriodBeginningDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]).ToString();
					_webSession.PeriodEndDate =  Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]).ToString();					
					_webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.weekly;
					_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
					ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
				}
			}			
			#endregion
			_webSession.Save();
		}
		#endregion

		/// <summary>
		/// Vérifie si l'utilisateur à sélectionné une période
		/// </summary>
		/// <returns>vrai si une période a été sélectionnée</returns>
		private  bool isPeriodSelected(System.Web.UI.Page Page){
			try{
				if( (Page.Request.Form.GetValues("_cumulPeriodCheckBox")!=null && Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0]!=null) 
					||( Page.Request.Form.GetValues("ResultsDateListWebControl1")!=null && Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]!=null && !Page.Request.Form.GetValues("ResultsDateListWebControl1")[0].ToString().Equals("0"))
					|| (Page.Request.Form.GetValues("ResultsDateListWebControl2")!=null && Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]!=null && !Page.Request.Form.GetValues("ResultsDateListWebControl2")[0].ToString().Equals("0"))
					)return true;
				}catch(Exception)
				{ return false;}
					
			return false;
		}
		/// <summary>
		/// Met la période cumulée dans la session cliente
		/// </summary>
		private void setYearlyPeriod(){
			string periodBeginningDate="01";	
			int week_N =0;
			_webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.yearly;
			AtomicPeriodWeek currentPeriod1 = new AtomicPeriodWeek(DateTime.Now);			
			currentPeriod1.SubWeek(2);
			week_N = currentPeriod1.Week;
			if(week_N>0){																						
				_webSession.PeriodBeginningDate = DateTime.Now.Year.ToString()+periodBeginningDate;
				_webSession.PeriodEndDate = currentPeriod1.Week.ToString().Length==1?currentPeriod1.Year.ToString()+"0"+currentPeriod1.Week.ToString() : currentPeriod1.Year.ToString()+currentPeriod1.Week.ToString();					
				_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
			}else{
				_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString()+periodBeginningDate;
				currentPeriod1= new AtomicPeriodWeek(DateTime.Now.AddYears(-1));
				_webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString()+currentPeriod1.Week;
				_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
			}
			_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.cumlDate;
			_webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
			_webSession.PeriodBeginningDate=TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE;
			_webSession.PeriodEndDate=TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE;
			_webSession.Save();
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
