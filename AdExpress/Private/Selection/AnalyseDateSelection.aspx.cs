#region Informations
// Auteur: G. Facon 
// Date de création:
// Date de modification: 
//	04/01/05 G. Facon Gestion des erreurs
//	01/08/2006 Modification FindNextUrl
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
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection de dates par mois ou semaine
	/// Cette Page est utilisée pour les analyses
	/// </summary>
	public partial class AnalyseDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
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
		/// nombre d'année à prendre en compte dans les cas N dernières années
		/// </summary>
		/// <summary>
		/// nombre de mois à prendre en compte dans les cas N dernièrs mois
		/// </summary>
		/// <summary>
		/// nombre de semaine à prendre en compte dans les cas N dernières semaines
		/// </summary>
		/// <summary>
		/// Contrôle calendrier date de fin de période
		/// </summary>

		#endregion

		#region Variables
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
//		public string previousWeek;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Informations
		/// </summary>
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
		/// Option de période sélectionné
		/// </summary>
		int selectedIndex=-1;

		
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public  AnalyseDateSelection():base(){
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
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;

				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
//				previousYear = GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
//				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
//				previousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);

				// Gestion des Calendrier
				this.monthWeekCalendarBeginWebControl.Language = _webSession.SiteLanguage;
				this.monthWeekCalendarEndWebControl.Language = _webSession.SiteLanguage;		
				#endregion				

			}
			catch(System.Exception exc) {
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
			try{
				if (this.IsPostBack){
//					if(monthWeekCalendarBeginWebControl.isDateSelected()){
//						switch(monthWeekCalendarBeginWebControl.SelectedDateType){
//							case CstPeriodType.dateToDateMonth:
//								monthWeekCalendarEndWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;
//								break;
//							case CstPeriodType.dateToDateWeek:
//								monthWeekCalendarEndWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.week;
//								break;
//							default:
//								monthWeekCalendarEndWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.all;
//								break;
//						}
//					}
					#region Url Suivante	
					if(_nextUrlOk){
						if(selectedIndex==6 ||selectedIndex==7) validateButton1_Click(this, null);
						else validateButton2_Click(this, null);
					}
					#endregion
				}
			}
			catch(System.Exception exc) {
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
		/// <param name="e">Argument</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {
			if(_webSession.CustomerLogin.Connection!=null){
				if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Open)_webSession.CustomerLogin.Connection.Close();
				_webSession.CustomerLogin.Connection.Dispose();
			}
			_webSession.Save();
		}
		#endregion

		#region Initialisation
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Argument</param>
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

		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// On l'utilise pour l'initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			yearDateList.WebSession=_webSession;
			monthDateList.WebSession=_webSession;
			weekDateList.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			#region Liste des années
			// Pour l'analyse dynamique on ne doit montrer que 2 années
			if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
				this.yearDateList.NbYearsToDisplay=2;
			}
			#endregion
			
			previousWeekCheckBox.Language = previousMonthCheckbox.Language = previousYearCheckbox.Language = _webSession.SiteLanguage;	

			if(_webSession.CurrentModule!=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA){
				string idVehicleList =  _webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			
				//Les calendriers affiche uniquement des mois pour le média Internet
				if(idVehicleList!=null && idVehicleList.Length>0 && idVehicleList.IndexOf(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString())>=0){
				
					this.monthWeekCalendarBeginWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;					
					this.monthWeekCalendarEndWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;
					
					//Rendre invisible les options de sélection des semaines pour Internet
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
				DBFunctions.closeDataBase(_webSession);
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
				switch(selectedIndex){
					case 0:
						if(int.Parse(yearDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastYear;
							_webSession.PeriodLength=int.Parse(yearDateList.SelectedValue);
							_webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - _webSession.PeriodLength).ToString("yyyy01");
							_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
							_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");;
							_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					case 2:
						if(int.Parse(weekDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastWeek;
							_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);
							week = new DateDll.AtomicPeriodWeek(DateTime.Now);
							_webSession.PeriodEndDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							week.SubWeek(_webSession.PeriodLength-1);
							_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							_webSession.DetailPeriod = CstPeriodDetail.weekly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;
						_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate=DateTime.Now.AddYears(-1).ToString("yyyy12");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;					
						break;
					case 4:
						_webSession.PeriodType=CstPeriodType.previousMonth;
						_webSession.PeriodLength=1;
						_webSession.PeriodEndDate=_webSession.PeriodBeginningDate=DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
					case 5:
						_webSession.PeriodType=CstPeriodType.previousWeek;
						_webSession.PeriodLength=1;
						week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
						_webSession.PeriodBeginningDate=_webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(AdExpressException.AnalyseDateSelectionException ex){
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
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion
	}
}