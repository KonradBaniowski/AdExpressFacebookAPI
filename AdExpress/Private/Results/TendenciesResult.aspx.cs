#region Informations
// Auteur: A. Obermeyer & D. V. Mussuma
// Date de cr�ation : 8/02/2005
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
	/// Page de r�sultat du module Tendance
	/// </summary>
	public partial class TendenciesResult : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables MMI
		/// <summary>
		/// Contr�le : Titre du Module
		/// </summary>
		/// <summary>
		/// Contr�le : Options
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ResultsOptionsWebControl ResultsOptionsWebControl1;
		/// <summary>
		/// bouton de validation des options
		/// </summary>
		/// <summary>
		/// module bridge
		/// </summary>
		/// <summary>
		/// Contr�le En-t�te
		/// </summary>
		/// <summary>
		/// Contr�le options des tendances
		/// </summary>
		/// <summary>
		/// Contr�le choix dates mensuelles
		/// </summary>
		/// <summary>
		/// Contr�le choix dates hebdomadaires
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
		/// R�sultat HTML
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
			// On r�initialise en KEuro car d'anciennes sessions peuvent �tre en Euro
			_webSession.Unit = WebConstantes.CustomerSessions.Unit.kEuro;
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
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

				#region S�lection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				#endregion

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;			
				#endregion			

				#region s�lection de la p�riode
				//initialisation de la p�iode s�lectionn�e
				SelectedPeriod(vehicleName);
				if(vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
					&& vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress){
					//Chargement de la p�riode s�lectionn�e
					LoadSelectedPeriod(vehicleName);
				}
				#endregion

				#region D�finition de la page d'aide
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
		
		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
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
			ResultsDashBoardOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			ResultsDateListWebControl1.WebSession=_webSession;
			ResultsDateListWebControl2.WebSession=_webSession;			
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent(){
            this.Unload += new System.EventHandler(this.Page_UnLoad);         

		}
		#endregion

		#region M�thodes internes

		#region p�riode s�lectionn�e
		/// <summary>
		/// S�lection de la p�riode d'�tude
		/// </summary>
		private void SelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName){	
			try{			
				if(!Page.IsPostBack || (vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press) 
					|| (vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress)){
					//Chargement des p�riodes
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
						throw(new System.Exception(" Vous devez s�lectionner une p�riode pour faire une analyse."));
					}
					else if(vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
						|| vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress){
						setYearlyPeriod();
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=true;
	
					}
					
				}else{
					if(!isPeriodSelected(Page)){
						ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
                        throw (new System.Exception(" Vous devez s�lectionner une p�riode pour faire une analyse."));						
					}
				}					
			}											
			catch(Exception e){				
				Response.Write("<script language=\"JavaScript\">alert('"+e.Message+"');</script>");
			}					
		}
		#endregion
		
		#region chargement de la p�riode s�lectionn�e
		/// <summary>
		/// R�cup�re la p�riode s�lectionn�e
		/// </summary>
		private void LoadSelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName){
			#region s�lection de la p�riode	
			if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){											
				//S�lection p�riode cumul�e			
				if((vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press || vehicleName==TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress)  
					||( Page.Request.Form.GetValues("_cumulPeriodCheckBox")!=null && Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0]!=null && !Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0].ToString().Equals("0"))){					
					setYearlyPeriod();
					ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=true;
				}									
				//S�lection p�riode mensuelle
				else if((vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press && vehicleName!=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress) 
					&& Page.Request.Form.GetValues("ResultsDateListWebControl1")!=null && Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]!=null && !Page.Request.Form.GetValues("ResultsDateListWebControl1")[0].ToString().Equals("0")){
					_webSession.PeriodBeginningDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();
					_webSession.PeriodEndDate =  Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();											
					_webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
					_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
					ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked=false;
				}			
				//S�lection p�riode hebdomadaire
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
		/// V�rifie si l'utilisateur � s�lectionn� une p�riode
		/// </summary>
		/// <returns>vrai si une p�riode a �t� s�lectionn�e</returns>
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
		/// Met la p�riode cumul�e dans la session cliente
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
