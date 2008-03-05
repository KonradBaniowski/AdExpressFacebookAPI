#region Informations
// Auteur: G. Facon 
// Date de cr�ation:
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
	/// Page de s�lection de dates par mois ou semaine
	/// Cette Page est utilis�e pour les analyses
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
		/// Bouton de validation de la s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Bouton de validation de la s�lection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Titre s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Calandrier de s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Titre de la partie s�lection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Description de la partie avec sauvegarde
		/// </summary>
		/// <summary>
		/// Contr�le txt n dernieres annees
		/// </summary>
		/// <summary>
		/// Contr�le txt n derniers mois
		/// </summary>
		/// <summary>
		/// Contr�le txt n dernieres semaines
		/// </summary>
		/// <summary>
		/// Contr�le txt "ou bien sur"
		/// </summary>
		/// <summary>
		/// nombre d'ann�e � prendre en compte dans les cas N derni�res ann�es
		/// </summary>
		/// <summary>
		/// nombre de mois � prendre en compte dans les cas N derni�rs mois
		/// </summary>
		/// <summary>
		/// nombre de semaine � prendre en compte dans les cas N derni�res semaines
		/// </summary>
		/// <summary>
		/// Contr�le calendrier date de fin de p�riode
		/// </summary>

		#endregion

		#region Variables
//		/// <summary>
//		/// Text Option ann�e pr�c�dent
//		/// </summary>
//		public string previousYear;
//		/// <summary>
//		/// Text Option mois pr�c�dent
//		/// </summary>
//		public string previousMonth;
//		/// <summary>
//		/// Text Option semaine pr�c�dente
//		/// </summary>
//		public string previousWeek;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Informations
		/// </summary>
		/// <summary>
		/// Choix semaine pr�c�dente
		/// </summary>
		
		/// <summary>
		/// Choix mois pr�c�dent
		/// </summary>
	
		/// <summary>
		/// Choix ann�e pr�c�dente
		/// </summary>
		/// <summary>
		/// Option de p�riode s�lectionn�
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

		#region Ev�nements

		#region Chargement de la page

		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region D�finition de la date d�but Analyse dynamique
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
					this.monthWeekCalendarBeginWebControl.StartYear=DateTime.Now.Year-1;
					this.monthWeekCalendarEndWebControl.StartYear=DateTime.Now.Year-1;
				}
				#endregion
			
				#region Option de p�riode s�lectionn�e
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
		
		#region Pr�Rendu de la page

		/// <summary>
		/// Ev�nement de Pr�Rendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Argument</param>
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

			#region Liste des ann�es
			// Pour l'analyse dynamique on ne doit montrer que 2 ann�es
			if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
				this.yearDateList.NbYearsToDisplay=2;
			}
			#endregion
			
			previousWeekCheckBox.Language = previousMonthCheckbox.Language = previousYearCheckbox.Language = _webSession.SiteLanguage;	

			if(_webSession.CurrentModule!=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA){
				string idVehicleList =  _webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			
				//Les calendriers affiche uniquement des mois pour le m�dia Internet
				if(idVehicleList!=null && idVehicleList.Length>0 && idVehicleList.IndexOf(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString())>=0){
				
					this.monthWeekCalendarBeginWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;					
					this.monthWeekCalendarEndWebControl.DisplayType = AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;
					
					//Rendre invisible les options de s�lection des semaines pour Internet
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
		/// Ev�nement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
		/// Ev�nement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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

		#region M�thodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation(){
			// On v�rifie que 2 dates ont �t� s�lectionn�es
			if(monthWeekCalendarBeginWebControl.isDateSelected() && monthWeekCalendarEndWebControl.isDateSelected()){
				// On teste que les deux dates sont du m�me type
				if(monthWeekCalendarBeginWebControl.SelectedDateType!=monthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException("Les dates s�lectionn�es ne sont pas du m�me type"));
				// On teste que la date de d�but est inf�rieur � la date de fin
				if(monthWeekCalendarBeginWebControl.SelectedDate>monthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.AnalyseDateSelectionException("La date de d�but doit �tre inf�rieure � la date de fin"));
				_webSession.PeriodType =monthWeekCalendarBeginWebControl.SelectedDateType;
				switch(monthWeekCalendarBeginWebControl.SelectedDateType){
					case CstPeriodType.dateToDateMonth:
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
					case CstPeriodType.dateToDateWeek:
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException("Le choix de type de s�lection de p�riode est incorrect"));
				}
				// On sauvegarde les donn�es
				_webSession.PeriodBeginningDate = monthWeekCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = monthWeekCalendarEndWebControl.SelectedDate.ToString();	
			}
			else{
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}
		}
		#endregion

		#endregion

		#region Impl�mentation m�thodes abstraites
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion
	}
}