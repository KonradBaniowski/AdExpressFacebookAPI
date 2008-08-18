#region Informations
//Auteur : A.Obermeyer
//Date de création : 29/12/2004
//31/12/2004 A. Obermeyer Intégration de WebPage
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
using Oracle.DataAccess.Client;
using System.Globalization;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Customer;
using AdExpressWebControles=TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException=AdExpress.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
namespace AdExpress.Private.Selection{

	/// <summary>
	/// Description résumée de DynamicDateSelection.
	/// </summary>
	public partial class DynamicDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI


		#endregion

		#region Variables
		/// <summary>
		/// Formulaire principale
		/// </summary>
		/// <summary>
		/// Entete
		/// </summary>
		/// <summary>
		/// Titre et description de la page
		/// </summary>
		/// <summary>
		/// Bouton de validation de la sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Bouton de validation de la sélection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Titre sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Calandrier de sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Titre de la partie sélection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Description de la partie avec sauvegarde
		/// </summary>
		/// <summary>
		/// Contrôle txt n dernieres annees
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText yearAdExpressText;
		/// <summary>
		/// Contrôle txt n derniers mois
		/// </summary>
		/// <summary>
		/// Contrôle txt n dernieres semaines
		/// </summary>
		/// <summary>
		/// Contrôle txt "ou bien sur"
		/// </summary>
		/// <summary>
		/// nombre de mois à prendre en compte dans les cas N dernièrs mois
		/// </summary>
		/// <summary>
		/// nombre de semaine à prendre en compte dans les cas N dernières semaines
		/// </summary>
//		/// <summary>
//		/// Text Option année courante
//		/// </summary>
//		public string currentYear;
//		/// <summary>
//		/// Text Option année précédent
//		/// </summary>
//		public string previousYear;
//		/// <summary>
//		/// Text Option mois précédent
//		/// </summary>
//		public string previousMonth;
//		/// <summary>
//		/// Text Option semaine précédente
//		/// </summary>
//		public string txtPreviousWeek; //previousWeek;
		/// <summary>
		/// Contrôle calendrier date de fin de période
		/// </summary>
		/// <summary>
		/// Option de période sélectionné
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Date de début de chargemebt des données
		/// </summary>
		private string downloadBeginningDate="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Informations
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Date de fin de chargement des données
		/// </summary>
		private string downloadEndDate="";
		/// <summary>
		/// Choix semaine précédente
		/// </summary>
		
		/// <summary>
		/// Choix mois précédent
		/// </summary>
	
		/// <summary>
		/// Choix année précédente
		/// </summary>

		/// <summary>
		/// Choix année courante
		/// </summary>


		/// <summary>
		/// Dernier mois (YYYYMM) dont les données sont complètement disponibles
		/// </summary>
		protected string _lastCompleteMonth = null;
		
		/// <summary>
		/// Media sélectionné
		/// </summary>
		protected long _selectedVehicle = -1;
		#endregion
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public DynamicDateSelection():base(){
	}
		#endregion

		#region Evènements

		#region Chargement de la page

		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Définition de la date début Analyse dynamique
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
					this.monthWeekCalendarBeginWebControl.StartYear=DateTime.Now.Year-1;
					this.monthWeekCalendarEndWebControl.StartYear=DateTime.Now.Year-1;
				}
				#endregion
			
				#region Option de période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				#endregion
				
				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				
				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
//				previousYear = GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
//				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
				
//				currentYear=GestionWeb.GetWebWord(1119,_webSession.SiteLanguage);
			
				// Gestion des Calendrier
				this.monthWeekCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthWeekCalendarEndWebControl.Language=_webSession.SiteLanguage;	
//				txtPreviousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);

	
				#endregion
			
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
		
		#region PréRendu de la page

		/// <summary>
		/// Evènement de PréRendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_PreRender(object sender, System.EventArgs e){
			if (this.IsPostBack){

				#region Url Suivante
				
				if(_nextUrlOk){
					if(selectedIndex==6 ||selectedIndex==7) validateButton1_Click(this, null);
					else validateButton2_Click(this, null);
				}
				#endregion
			}
		}

		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {			
		}
		#endregion

		#region Initialisation
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Argument</param>
		override protected void OnInit(EventArgs e){
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
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
           

		}
		#endregion

		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// On l'utilise pour l'initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;	
			monthDateList.WebSession=_webSession;
			weekDateList.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			this.monthWeekCalendarBeginWebControl.IncompleteDatesInRed =  true;			
			this.monthWeekCalendarEndWebControl.IncompleteDatesInRed = true;

				
			previousWeekCheckBox.Language = previousMonthCheckbox.Language = 
			previousYearCheckbox.Language = currentYearCheckbox.Language =_webSession.SiteLanguage;						
			 

			//Les calendriers affiche uniquement des mois pour le média Internet
			if(_webSession.SelectionUniversMedia!=null && _webSession.SelectionUniversMedia.FirstNode !=null){
				 _selectedVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
				if((DBClassificationConstantes.Vehicles.names)_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet){
					
					_lastCompleteMonth = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_webSession,WebConstantes.CustomerSessions.Period.DisplayLevel.monthly,WebConstantes.Module.Type.analysis,_selectedVehicle);
					
#if DEBUG
					_lastCompleteMonth = "200703";
#endif
					if(_lastCompleteMonth!=null && _lastCompleteMonth.Length>0)
						this.monthWeekCalendarBeginWebControl.LastCompleteMonth = _lastCompleteMonth;
					else this.monthWeekCalendarBeginWebControl.AllDatesInRed = true;
					this.monthWeekCalendarBeginWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
					
					if(_lastCompleteMonth!=null && _lastCompleteMonth.Length>0)
						this.monthWeekCalendarEndWebControl.LastCompleteMonth = _lastCompleteMonth;
					else this.monthWeekCalendarEndWebControl.AllDatesInRed = true;
					this.monthWeekCalendarEndWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
					
					//Rendre invisible les options de sélection des semaines pour Intrenet
					weekDateList.Visible = false;
					weekAdExpressText.Visible = false;
					previousWeekCheckBox.Visible = false;
				}
				
			}

			return tmp;
		}
		#endregion

		#region Validation des calendriers
		/// <summary>
		/// Evènement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void validateButton1_Click(object sender, System.EventArgs e) {
			try{
				calendarValidation();
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex){
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}
		}
		#endregion

		#region Validation des dates avec sauvegarde
		/// <summary>
		/// Evènement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void validateButton2_Click(object sender, System.EventArgs e) {
			

			try{
				DateDll.AtomicPeriodWeek week;
				DateTime monthPeriod;

				switch(selectedIndex){
						//Année courante
					case 0:						
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;
						
						//Dates de chargement des données pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){
							if( _lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth.Substring(0,4))==DateTime.Now.Year){
								_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");						
								_webSession.PeriodEndDate = _lastCompleteMonth;
							}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des données pour les autres médias
							if(DateTime.Now.Month==1){
								throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(1612,_webSession.SiteLanguage));														
							}else {
								WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.currentYear);								
								_webSession.PeriodBeginningDate=downloadBeginningDate;						
								_webSession.PeriodEndDate=downloadEndDate;
							}
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;						
						break;

						//N derniers mois
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
						
							//Dates de chargement des données pour Internet
							if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){
								if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0){
									_webSession.PeriodEndDate = _lastCompleteMonth;
									monthPeriod = new DateTime(int.Parse(_lastCompleteMonth.Substring(0,4)),int.Parse(_lastCompleteMonth.Substring(4,2)),01);						
									_webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");	
								}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
							}else{
								//Dates de chargement des données pour les autres médias
								WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
								monthPeriod = new DateTime(int.Parse(downloadBeginningDate.Substring(0,4)),int.Parse(downloadBeginningDate.Substring(4,2)),01);						
								_webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");							
								_webSession.PeriodEndDate = downloadEndDate;
							}
							
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;

						//N dernières semaines
					case 2:
						if(int.Parse(weekDateList.SelectedValue)!=0){
							//dernière semaine chargée
							WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);
							_webSession.PeriodType=CstPeriodType.nLastWeek;
							_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);							
							week = new DateDll.AtomicPeriodWeek(int.Parse(downloadEndDate.Substring(0,4)),int.Parse(downloadEndDate.Substring(4,2)));
							_webSession.PeriodEndDate = downloadEndDate;
							week.SubWeek(_webSession.PeriodLength-1);
							_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							_webSession.DetailPeriod = CstPeriodDetail.weekly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					
						//Année précédente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;

						//Dates de chargement des données pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){						
							if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth.Substring(0,4))>DateTime.Now.AddYears(-2).Year){
								_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
								_webSession.PeriodEndDate = (int.Parse(_lastCompleteMonth.Substring(0,4))==DateTime.Now.Year)? DateTime.Now.AddYears(-1).ToString("yyyy12") : _lastCompleteMonth;
							}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des données pour les autres médias
							WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.previousYear);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;					
						break;

						//Mois précédent
					case 4:
						//Dates de chargement des données pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){						
							if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth) >= int.Parse(DateTime.Now.AddMonths(-1).ToString("yyyyMM")))
								_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
							else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des données pour les autres médias
							WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousMonth,_webSession);												
							_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
							
						}
						_webSession.PeriodType=CstPeriodType.previousMonth;	
						_webSession.PeriodLength=1;
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;

						//Semaine précédente
					case 5:
						WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousWeek,_webSession);
						_webSession.PeriodType=CstPeriodType.previousWeek;
						_webSession.PeriodLength=1;
						week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
						_webSession.PeriodBeginningDate=_webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex){
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");			 								
			}
		}
		#endregion

		#endregion

		#region Méthodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation(){
			// On vérifie que 2 dates ont été sélectionnées
			if(monthWeekCalendarBeginWebControl.isDateSelected() && monthWeekCalendarEndWebControl.isDateSelected()){
				// On teste que les deux dates sont du même type
				if(monthWeekCalendarBeginWebControl.SelectedDateType!=monthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException("Les dates sélectionnées ne sont pas du même type"));
				// On teste que la date de début est inférieur à la date de fin
				if(monthWeekCalendarBeginWebControl.SelectedDate>monthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.AnalyseDateSelectionException("La date de début doit être inférieure à la date de fin"));
				_webSession.PeriodType =monthWeekCalendarBeginWebControl.SelectedDateType;
				switch(monthWeekCalendarBeginWebControl.SelectedDateType){
					case CstPeriodType.dateToDateMonth:
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
					case CstPeriodType.dateToDateWeek:
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException("Le choix de type de sélection de période est incorrect"));
				}
				// On sauvegarde les données
				_webSession.PeriodBeginningDate = monthWeekCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = monthWeekCalendarEndWebControl.SelectedDate.ToString();	
			}
			else{
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}
		}
		#endregion

		

		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
