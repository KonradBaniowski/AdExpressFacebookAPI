#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 14/11/2005
// Date de modification: 
#endregion

#region Namespaces
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
using TNS.Isis.Right.Common;
using TNS.FrameWork.Date;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet.Web;
#endregion

namespace BastetWeb{
	/// <summary>
	/// Page de gestion de période
	/// </summary>
	public partial class DateSelection : PrivateWebPage{

		#region Variables
		/// <summary>
		/// Option de période sélectionné
		/// </summary>
		int selectedIndex = -1;

		/// <summary>
		/// Gestion des emails
		/// </summary>
		public string _email_manage = string.Empty;
		/// <summary>
		/// Gestion de la période
		/// </summary>
		public string _period_manage = string.Empty;
		/// <summary>
		/// Gestion des logins
		/// </summary>
		public string _login_manage = string.Empty;
		/// <summary>
		/// validation
		/// </summary>
		public string _validation = string.Empty;
		/// <summary>
		/// Sélection de périodes calendaires
		/// </summary>
		public string _label_calendar_period_selection = string.Empty;
		/// <summary>
		/// Sélection de périodes glissantes
		/// </summary>
		public string _label_rolling_period_selection = string.Empty;
		/// <summary>
		/// Au cours de(s) :
		/// </summary>
		public string _label_in_the_last = string.Empty;
		/// <summary>
		/// derniers mois
		/// </summary>
		public string _label_n_last_month = string.Empty;
		/// <summary>
		/// dernières semaines
		/// </summary>
		public string _label_n_last_week = string.Empty;
		/// <summary>
		/// Ou bien sur :
		/// </summary>
		public string _label_or_in_the_last = string.Empty;
		/// <summary>
		/// l'année en cours
		/// </summary>
		public string _label_current_year = string.Empty;
		/// <summary>
		/// La date de fin ne peut pas être supérieure à la date de début
		/// </summary>
		public string _msg_err_dates = string.Empty;
		/// <summary>
		/// Il est nécessaire de sélectionner deux dates pour valider
		/// </summary>
		public string _msg_err_select_two_date_for_validation = string.Empty;
		/// <summary>
		/// Vous devez sélectionner une période
		/// </summary>
		public string _msg_err_select_period = string.Empty;
		/// <summary>
		/// Identifiant du module
		/// </summary>
		public int _moduleId = 1;
		#endregion

		#region Variables MMI
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
                
                dateBeginCalendar.DateChanged += new OboutInc.Calendar2.DateChangedEventHandler(dateBeginCalendar_SelectionChanged);
                dateEndCalendar.DateChanged += new OboutInc.Calendar2.DateChangedEventHandler(dateEndCalendar_SelectionChanged);

				#region QueryString
				if(Page.Request.QueryString.Get("moduleId")!= null){
					_moduleId	= int.Parse(Page.Request.QueryString.Get("moduleId").ToString());
				}
				#endregion

				#region Vérification des éléments en session
				if(Session[TNS.AdExpress.Bastet.WebSession.LOGIN] == null) throw(new SystemException("Aucun login en session"));
				switch(_moduleId){
					case 1: // Stats
                        if (Session[TNS.AdExpress.Bastet.WebSession.MAILS] == null) throw (new SystemException("Aucun email en session"));
						break;
				}
				#endregion


				HeaderWebControl1.ActiveMenu = _moduleId;
                HeaderWebControl1.LanguageId = _siteLanguage;
                HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.generic;
                dateBeginCalendar.CultureName = TNS.AdExpress.Bastet.Web.WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.Name;
                dateEndCalendar.CultureName = TNS.AdExpress.Bastet.Web.WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.Name;

                //dateBeginCalendar.DateFormat = WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.GetFormatPatternFromStringFormat("{0:ddMMyyyy}");
                dateBeginCalendar.DateFormat = WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.GetFormatPatternFromStringFormat("{0:calendar}");
                dateEndCalendar.DateFormat = WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.GetFormatPatternFromStringFormat("{0:calendar}");

                dateBeginLabel.Text = dateBeginCalendar.SelectedDate.ToString(WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.GetFormatPatternFromStringFormat("{0:shortdatepattern}"));
                dateEndLabel.Text = dateEndCalendar.SelectedDate.ToString(WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.GetFormatPatternFromStringFormat("{0:shortdatepattern}")); ;

				#region Listes déroulantes des périodes
				int nbMonths = 15;
				int nbWeeks = 53;
				switch(_moduleId){
					case 1: // STATS > Mois et semaines coulants
						nbMonths = 15;
						nbWeeks = 53;
						break;
					case 2: // INDICATEURS > Mois et semaines de l'année précédente + Mois et semaines de l'année en cours
						nbMonths = 12 + int.Parse(DateTime.Now.ToString("MM"));
						int weekNum = 0;
						if(DateTime.Now.Month==12 && DateTime.Now.Day==31)
							weekNum = WeekNumber(DateTime.Now.AddDays(-2)); // -2 car le 31 peut faire parti de la 1ère semaine de l'année suivante
						else
							weekNum = WeekNumber(DateTime.Now);						
						nbWeeks = 53 + weekNum;
						break;
				}

				if(!IsPostBack){
					// N derniers mois
					monthDateList.Items.Clear();
					monthDateList.Items.Add(new System.Web.UI.WebControls.ListItem("----","0"));
					for(int i=1 ; i <= nbMonths ; i++){
						monthDateList.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(),i.ToString()));
					}
					// N dernières semaines
					weekDateList.Items.Clear();
					weekDateList.Items.Add(new System.Web.UI.WebControls.ListItem("----","0"));
					for(int i=1 ; i <= nbWeeks ; i++){
						weekDateList.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(),i.ToString()));
					}
				}
				#endregion

				#region Option de période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				#endregion	

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new ErrorEventArgs(this, exc));
				}
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
            base.OnInit(e);
			InitializeComponent();
			InitializeComponentCustom();

            if (!this.IsPostBack) {
                DateTime dateTimeBegin = DateTime.Now;
                DateTime dateTimeEnd = DateTime.Now;
                if (Session[TNS.AdExpress.Bastet.WebSession.DATE_BEGIN] != null && Session[TNS.AdExpress.Bastet.WebSession.DATE_BEGIN] != null) {
                    dateTimeBegin = (DateTime)Session[TNS.AdExpress.Bastet.WebSession.DATE_BEGIN];
                    dateTimeEnd = (DateTime)Session[TNS.AdExpress.Bastet.WebSession.DATE_END];
                }
                else {
                    dateTimeBegin = DateTime.Now;
                    dateTimeEnd = DateTime.Now;
                }

                dateBeginCalendar.DateMax=DateTime.Now;
                dateEndCalendar.DateMax=DateTime.Now;
                dateBeginCalendar.SelectedDate = dateTimeBegin;
                dateEndCalendar.SelectedDate = dateTimeEnd;

            }
			
		}

		/// <summary>
		/// Initialisation des composants
		/// </summary>
		private void InitializeComponentCustom(){

			// Etapes durant navigation dans le site
            _email_manage = GestionWeb.GetWebWord(6, _siteLanguage);
            _period_manage = GestionWeb.GetWebWord(7, _siteLanguage);
            _login_manage = GestionWeb.GetWebWord(8, _siteLanguage);
            _validation = GestionWeb.GetWebWord(9, _siteLanguage);

			// Boutons
            this.validateCalendarButton.Text = GestionWeb.GetWebWord(1, _siteLanguage);
            this.validateButton.Text = GestionWeb.GetWebWord(1, _siteLanguage);

			// Textes
            _label_calendar_period_selection = GestionWeb.GetWebWord(776, _siteLanguage);
            _label_rolling_period_selection = GestionWeb.GetWebWord(777, _siteLanguage);
            _label_in_the_last = GestionWeb.GetWebWord(785, _siteLanguage);
            _label_n_last_month = GestionWeb.GetWebWord(15, _siteLanguage);
            _label_n_last_week = GestionWeb.GetWebWord(16, _siteLanguage);
            _label_or_in_the_last = GestionWeb.GetWebWord(786, _siteLanguage);
            _label_current_year = GestionWeb.GetWebWord(18, _siteLanguage);

			// Messages
            _msg_err_dates = GestionWeb.GetWebWord(30, _siteLanguage);
            _msg_err_select_two_date_for_validation = GestionWeb.GetWebWord(886, _siteLanguage);
            _msg_err_select_period = GestionWeb.GetWebWord(885, _siteLanguage);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){    

		}
		#endregion

		#region Validation des périodes calendaires
		/// <summary>
		/// Valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateCalendarButton_Click(object sender, System.EventArgs e) {
            try {
                DateTime dateBegin = dateBeginCalendar.SelectedDate;
                DateTime dateEnd = dateEndCalendar.SelectedDate;

                if (dateBegin.Ticks <= dateEnd.Ticks) {
                    // Ajout des dates de début et de fin dans la session
                    Session.Add("DateBegin", dateBegin);
                    Session.Add("DateEnd", dateEnd);

                    // Redirection
                    switch (_moduleId) {
                        case 1: // Stats
                            Response.Redirect("LoginSelection.aspx");
                            break;
                        case 2: // Indicateurs
                            Response.Redirect("/Indicators/IndicatorsResult.aspx");
                            break;
                        default:
                            Response.Write("<script _siteLanguage=Javascript>alert('Module invalide');</script>");
                            break;
                    }
                }
                else {
                    // Javascript Erreur : La date de fin ne peut pas être supérieure à la date de début
                    Response.Write("<script _siteLanguage=Javascript>");
                    Response.Write("alert('" + _msg_err_dates + "');");
                    Response.Write("</script>");
                }
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new ErrorEventArgs(this, exc));
                }
            }
		}
		#endregion

		#region Validation de périodes glissante
		/// <summary>
		/// Valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{

				#region Variables
				AtomicPeriodWeek week;
				DateTime dateBeginSelected;
                DateTime dateEndSelected;
				#endregion

				#region Calcul des dates
				switch(selectedIndex){
					case 0:	// N derniers mois
						if(int.Parse(monthDateList.SelectedValue)!=0){
							dateEndSelected = Tools.EndMonth(DateTime.Now);
                            DateTime dateBeginYearMonthSelected = DateTime.Now.AddMonths(1 - int.Parse(monthDateList.SelectedValue));
                            dateBeginSelected = new DateTime(dateBeginYearMonthSelected.Year, dateBeginYearMonthSelected.Month, 1);

							// Ajout des dates de début et de fin dans la session
							Session.Add("DateBegin", dateBeginSelected);
							Session.Add("DateEnd", dateEndSelected);
						}
						else
							// Vous devez sélectionner une période
							Response.Write("<script _siteLanguage=Javascript>alert('"+_msg_err_select_period+"');</script>");
						break;
					case 1:	// N dernières semaines
						if(int.Parse(weekDateList.SelectedValue)!=0){
							week = new AtomicPeriodWeek(DateTime.Now);
							dateEndSelected = week.LastDay;
							int nb = int.Parse(weekDateList.SelectedValue)-1;
							week.SubWeek(nb);
							dateBeginSelected = week.FirstDay;

							// Ajout des dates de début et de fin dans la session
							Session.Add("DateBegin", dateBeginSelected);
							Session.Add("DateEnd", dateEndSelected);
						}
						else 
							// Vous devez sélectionner une période
							Response.Write("<script _siteLanguage=Javascript>alert('"+_msg_err_select_period+"');</script>");
						break;
					case 2:	// Année en cours
                        dateBeginSelected = new DateTime(DateTime.Now.Year, 1, 1);
						dateEndSelected = DateTime.Now.AddDays(-1);

						// Ajout des dates de début et de fin dans la session
						Session.Add("DateBegin", dateBeginSelected);
						Session.Add("DateEnd", dateEndSelected);
						break;
					default:
						// Vous devez sélectionner une période
						Response.Write("<script _siteLanguage=Javascript>alert('"+_msg_err_select_period+"');</script>");
						break;
				}
				#endregion

				#region Redirection
				switch(_moduleId){
					case 1: // Stats
						Response.Redirect("LoginSelection.aspx");
						break;
					case 2: // Indicateurs
						Response.Redirect("/Indicators/IndicatorsResult.aspx");
						break;
					default:
						Response.Write("<script _siteLanguage=Javascript>alert('Module invalide');</script>");
						break;
				}
				#endregion

				selectedIndex = selectedIndex;
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new ErrorEventArgs(this, exc));
				}
			}
		}
		#endregion

		#region SelectionChanged des calendriers
		/// <summary>
		/// Affiche la date de début au moment de la sélection
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void dateBeginCalendar_SelectionChanged(object sender, System.EventArgs e) {
			dateBeginLabel.Text = dateBeginCalendar.SelectedDate.ToString("dd/MM/yyyy");
		}

		/// <summary>
		/// Affiche la date de fin au moment de la sélection
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void dateEndCalendar_SelectionChanged(object sender, System.EventArgs e) {
			dateEndLabel.Text = dateEndCalendar.SelectedDate.ToString("dd/MM/yyyy");
		}
		#endregion

		#endregion

		#region Méthode privée
		/// <summary>
		/// Méthode pour calculer le numéro de semaine d'une date
		/// </summary>
		/// <param name="dt">DateTime</param>
		/// <returns>Numéro de semaine</returns>
		private int WeekNumber(DateTime dt){
			int yyyy = dt.Year; 
			int mm = dt.Month;
			int dd = dt.Day; 
			// Declare other required variables
			int jan1WeekDay; 
			int weekNumber = 0;
			int weekDay = 0; 
			int i, j, k, l, m, n;
			int[] mnth = new int[12] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 }; 
			int yearNumber;
			// Set DayofYear Number for yyyy mm dd
			int dayOfYearNumber = dd + mnth[mm - 1]; 
			// Increase of Dayof Year Number by 1, if year is leapyear and month is february
			if ((DateTime.IsLeapYear(yyyy)) && (mm == 2)) dayOfYearNumber += 1; 
			// Find the Jan1WeekDay for year 
			i = (yyyy - 1) % 100;
			j = (yyyy - 1) - i;
			k = i + i / 4;
			jan1WeekDay = 1 + (((((j / 100) % 4) * 5) + k) % 7);

			// Calcuate the WeekDay for the given date
			l = dayOfYearNumber + (jan1WeekDay - 1);
			weekDay = 1 + ((l - 1) % 7);

			// Find if the date falls in YearNumber set WeekNumber to 52 or 53
			if ((dayOfYearNumber <= (8 - jan1WeekDay)) && (jan1WeekDay > 4)) {
				yearNumber = yyyy - 1;
				if ((jan1WeekDay == 5) || ((jan1WeekDay == 6) && (jan1WeekDay > 4))) weekNumber = 53;else weekNumber = 52; 
			}
			else yearNumber = yyyy;
			
			// Set WeekNumber to 1 to 53 if date falls in YearNumber
			if (yearNumber == yyyy) {
				if (DateTime.IsLeapYear(yyyy) == true) m = 366; 
				else m = 365;
				if ((m - dayOfYearNumber) < (4 - weekDay)) {
					yearNumber = yyyy + 1;
					weekNumber = 1;
				}
			}
			if (yearNumber == yyyy) {
				n = dayOfYearNumber + (7 - weekDay) + (jan1WeekDay - 1);
				weekNumber = n / 7;
				if (jan1WeekDay > 4) weekNumber -= 1; 
			}
			return (weekNumber); 
		}
		#endregion

	}
}
