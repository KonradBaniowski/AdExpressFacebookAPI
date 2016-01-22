#region Informations
// Auteur: D.V. Mussuma et K.Shehzad
// Date de création: 05/07/2005
#endregion
#define DEBUG
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;
using CustomerWebConstantes=TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.Collections.Generic;


namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Contrôle qui fournit les N derniers mois et/ou les N dernières semaines.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:LastMonthWeekCalendarWebControl runat=server></{0}:LastMonthWeekCalendarWebControl>")]
	public class LastMonthWeekCalendarWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Enumérateur
		/// <summary>
		/// Type du calendrier
		/// Cette énumérateur sert à déterminer le titre du calendrier
		/// </summary>
		public enum Type{
			/// <summary>
			/// Indéfini
			/// </summary>
			unknown,
			/// <summary>
			/// Date de début
			/// </summary>
			dateBegin,
			/// <summary>
			/// Date de fin
			/// </summary>
			dateEnd
		}

		/// <summary>
		/// Type d'affichage
		/// </summary>
		public enum Display{
			/// <summary>
			/// Mois et semaines
			/// </summary>
			all,
			/// <summary>
			/// mois
			/// </summary>
			month,
			/// <summary>
			/// semaine
			/// </summary>
			week
		}
		#endregion

		#region Constantes
		/// <summary>
		/// Titre du début de période en français
		/// </summary>
		private const string TITLE_DATE_BEGIN_PICTURE="Starting_date.gif";
		/// <summary>
		/// Titre de la fin de période en français
		/// </summary>
		private const string TITLE_DATE_END_PICTURE="Ending_date.gif";
		/// <summary>
		/// Couleur de la bordure
		/// </summary>
		private string _table_border_color="#9D9885";
		/// <summary>
		/// Couleur de fond
		/// </summary>
        private string _table_color = "#FFFFFF";
		/// <summary>
		/// Couleur de fond de la date sélectionnée
		/// </summary>
        private string _table_color_selected_date = "#644883";
		/// <summary>
		/// Couleur de fond du titre dans les colonnes
		/// </summary>
		private string _title_background_color="#644883";
		/// <summary>
		/// Couleur de la font
		/// </summary>
		private string _font_color="#644883";
		/// <summary>
		/// Couleur de la font de la date sélectionnée
		/// </summary>
        private string _font_color_selected_date = "#FFFFFF";
		/// <summary>
		/// Couleur de fond quand il n'y a pas de date
		/// </summary>
		private string _table_color_no_date="#9378B3";

		#endregion

		#region Variables
		/// <summary>
		/// Type d'affichage
		/// </summary>
		private LastMonthWeekCalendarWebControl.Display display=LastMonthWeekCalendarWebControl.Display.all;
		/// <summary>
		/// Année du début de calendrier
		/// </summary>
		protected int startYear;
		/// <summary>
		/// Année du fin de calendrier
		/// </summary>
		protected int stopYear;
		/// <summary>
		/// Année sélectionnée
		/// </summary>
		protected int selectedYear=DateTime.Now.Year;
		/// <summary>
		/// Date sélectionnée
		/// </summary>
		protected int selectedDate;
		/// <summary>
		/// Type de date sélectionnée
		/// </summary>
		protected CustomerWebConstantes.Period.Type selectedDateType;
		/// <summary>
		/// Langue utilisé
		/// </summary>
		private int language;
		/// <summary>
		/// type du calendrier, il détermine le titre du calendrier
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Selections.LastMonthWeekCalendarWebControl.Type calendarType=Type.unknown;
		/// <summary>
		/// Calendrier en mémoire
		/// </summary>
		private YearMonthWeekCalendar memoryCalendar=null;
		/// <summary>
		/// Gère l'affichage des mois et semaines dont les données sont completement disponibles.
		/// </summary>
		protected bool showOnlyCompleteDate = false;
		/// <summary>
		/// websession client
		/// </summary>
		protected WebSession webSession=null;
		
		/// <summary>
		/// Nombre de mois à afficher
		/// </summary>
		protected int _numberOfLastMonth;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient et définit l'année de début
		/// </summary>
		public int StartYear{
			get{return startYear;}
			set{startYear = value;}
		}

		/// <summary>
		/// Obtient et définit l'année à afficher
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("2004")] 
		public int SelectedYear{
			get{return selectedYear;}
			set{
				selectedYear = value;
				if(memoryCalendar==null)new YearMonthWeekCalendar(value); 
				else memoryCalendar.Year=value;
			}
		}

		/// <summary>
		/// Obtient et définit la langue à utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public int Language{
			get{return language;}
			set{language = value;}
		}


		/// <summary>
		/// Obtient et définit le type de calendrier
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public	LastMonthWeekCalendarWebControl.Type CalendarType{
			get{return calendarType;}
			set{calendarType = value;}
		}

		/// <summary>
		/// Définit le niveau d'affichage
		/// </summary>
		public LastMonthWeekCalendarWebControl.Display DisplayType{
			set{this.display = value;}
		}

		/// <summary>
		/// Obtient le type de période sélectionné
		/// </summary>
		public CustomerWebConstantes.Period.Type SelectedDateType{
			get{return(this.selectedDateType);}
		}

		/// <summary>
		/// Obtient la période sélectionnée
		/// </summary>
		public int SelectedDate{
			get{return(this.selectedDate);}
		}

		/// <summary>
		///Gère l'affichage d'un mois ou d'une semaine complete (donnéees completement disponibles) 
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(false)] 
		public bool ShowOnlyCompleteDate{
			get{return showOnlyCompleteDate;}
			set{showOnlyCompleteDate=value;}
		}
		/// <summary>
		/// webSession client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		///Nombre de N derniers mois.
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(13)] 
		public int NumberOfLastMonth{
			get{return _numberOfLastMonth;}
			set{_numberOfLastMonth=value;}
		}

        /// <summary>
        /// Obtient et définit la couleur de la bordure
        /// </summary>
        public string TableBorderColor {
            get { return _table_border_color; }
            set { _table_border_color = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de fond
        /// </summary>
        public string TableColor {
            get { return _table_color; }
            set { _table_color = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de fond de la date sélectionnée
        /// </summary>
        public string TableColorSelectedDate {
            get { return _table_color_selected_date; }
            set { _table_color_selected_date = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de fond du titre dans les colonnes
        /// </summary>
        public string TitleBackgroundColor {
            get { return _title_background_color; }
            set { _title_background_color = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de la font
        /// </summary>
        public string FontColor {
            get { return _font_color; }
            set { _font_color = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de la font de la date sélectionnée
        /// </summary>
        public string FontColorSelectedDate {
            get { return _font_color_selected_date; }
            set { _font_color_selected_date = value; }
        }
        /// <summary>
        /// Obtient et définit la couleur de fond quand il n'y a pas de date
        /// </summary>
        public string TableColorNoDate {
            get { return _table_color_no_date; }
            set { _table_color_no_date = value; }
        }
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public LastMonthWeekCalendarWebControl(){
			this.selectedYear=DateTime.Now.Year;
			
		}
		#endregion

		#region Evènements

		#region Init
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
			stopYear=selectedYear;
			startYear=DateTime.Now.AddMonths(-(_numberOfLastMonth-1)).Year;
			selectedDate=-1;
			if (this.ViewState["selectedDate"]!=null){
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
		}

		#endregion

		#region Chargement
		/// <summary>
		/// Evènement de chargement du composant
		/// </summary>
		/// <param name="e">Argument</param>
		protected override void OnLoad(EventArgs e) {
			if (this.ViewState["selectedDate"]!=null){
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
			if (this.ViewState["selectedYear"]!=null){
				selectedYear=(int)this.ViewState["selectedYear"];
			}
			if (Page.IsPostBack){
				//				// TEST A FAIRE POUR VERIFIER QUE L'ON PREND LE BON CALENDARMONTH
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput==this.ID){
					string valueInput=Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
					switch(valueInput.Length){
						case 4:
							selectedYear=int.Parse(valueInput);
							ViewState.Add("selectedYear",selectedYear);
							//memoryCalendar=new YearMonthWeekCalendar(this.selectedYear);
							break;
						case 6:
							selectedDate=int.Parse(valueInput);
							ViewState.Add("selectedDate",selectedDate);
							selectedDateType=CustomerWebConstantes.Period.Type.dateToDateMonth;
							ViewState.Add("selectedDateType",selectedDateType);
							if (this.ViewState["yearSelected"]!=null){
							
							}
							break;
						case 8:
							selectedDate=int.Parse(valueInput.Substring(0,6));
							ViewState.Add("selectedDate",selectedDate);
							selectedDateType=CustomerWebConstantes.Period.Type.dateToDateWeek;
							ViewState.Add("selectedDateType",selectedDateType);
							break;
					}
				}	
			}
		}
		#endregion

		#region Affichage
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			try{

				StringBuilder htmlBuilder=new StringBuilder(10500);
                string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[language].Name;
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[language].Localization);
				string dateSelectedString="&nbsp;";
				if(isDateSelected()){
					switch(selectedDateType){
						case CustomerWebConstantes.Period.Type.dateToDateMonth:
							dateSelectedString=MonthString.GetCharacters(int.Parse(selectedDate.ToString().Substring(4,2)),cultureInfo,12)+"&nbsp;"+selectedDate.ToString().Substring(0,4);
							break;
						case CustomerWebConstantes.Period.Type.dateToDateWeek:
							AtomicPeriodWeek week=new AtomicPeriodWeek(int.Parse(selectedDate.ToString().Substring(0,4)),int.Parse(selectedDate.ToString().Substring(4,2)));
							DateTime dateBegin=week.FirstDay;
							DateTime dateEnd=week.FirstDay.AddDays(6);
							dateSelectedString=dateBegin.Day.ToString()+"/"+dateBegin.Month.ToString()+"/"+dateBegin.Year.ToString()+" - "+dateEnd.Day.ToString()+"/"+dateEnd.Month.ToString()+"/"+dateEnd.Year.ToString();
							break;
					}
				}

				htmlBuilder.Append("<table cellspacing=1 cellpadding=0 border=0 bgcolor="+_table_border_color+">");
				string tmpHTML="";
				string titleImage=this.getTitle();
				// Titre
				if(titleImage.Length>0){
                    tmpHTML = "<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + titleImage + "\" /></td>";
				}
				else{
					tmpHTML="<td>&nbsp;</td>";
				}
				// Flèche
				htmlBuilder.Append("<tr bgcolor="+_table_color+">");
				htmlBuilder.Append(tmpHTML);
				// Sélection des années
				string yearsToDisplay="";
				int startMonth=DateTime.Now.AddMonths(-(_numberOfLastMonth-1)).Month;
				yearsToDisplay=(startMonth.ToString().Length>1? startMonth.ToString() : "0"+startMonth.ToString())+"/"+startYear.ToString()+" - "+(DateTime.Now.Month.ToString().Length>1?DateTime.Now.Month.ToString() : "0"+DateTime.Now.Month.ToString())+"/"+stopYear.ToString();

				htmlBuilder.Append("<td colspan=5>");
				htmlBuilder.Append("<table width=\"100%\"cellspacing=0 cellpadding=0 border=0 bgcolor="+_table_color+">");
				htmlBuilder.Append("<tr>");
				htmlBuilder.Append("<td colspan=3 align=\"center\"><font size=1 color="+_font_color+" face=\"Arial\">"+yearsToDisplay+"</font></td>");
				htmlBuilder.Append("</tr>");
				htmlBuilder.Append("</table>");
				htmlBuilder.Append("</td>");
				htmlBuilder.Append("</tr>");
				//Titre des colonnes
				htmlBuilder.Append("<tr bgcolor=\""+_title_background_color+"\" >");
                htmlBuilder.Append("<td align=\"center\"><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/monthTitle.gif\" border=0 /></td>");
                htmlBuilder.Append("<td colspan=5 align=\"center\"><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/weekTitle.gif\" border=0 /></td>");
				htmlBuilder.Append("</tr>");
				int i,j;
				int[,] weeks;
											
				int currentYear = StartYear;
				int oldYear = StartYear;				
				memoryCalendar=new YearMonthWeekCalendar(StartYear);
				weeks=this.memoryCalendar.WeeksTable;
				for(i=1;i<=NumberOfLastMonth;i++){
					startMonth = DateTime.Now.AddMonths(-(_numberOfLastMonth-i)).Month;
					currentYear=DateTime.Now.AddMonths(-(_numberOfLastMonth-i)).Year;
					if(currentYear!=oldYear){
						memoryCalendar=new YearMonthWeekCalendar(currentYear);
						weeks=this.memoryCalendar.WeeksTable;
					}
					oldYear=currentYear;

					htmlBuilder.Append("<tr>");
					//affichage des mois
					if(display!=LastMonthWeekCalendarWebControl.Display.week){
                        htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('" + this.ID + "','" + (memoryCalendar.Year).ToString() + (startMonth).ToString("00") + "')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_" + (startMonth).ToString() + ".gif\"></a></td>");
					}
					else
                        htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_" + (startMonth).ToString() + ".gif\"></td>");
					//affichage des semaines
					for(j=0;j<weeks.GetLength(0);j++){
						if(weeks[j,startMonth-1]!=0 && display!=LastMonthWeekCalendarWebControl.Display.month){
                            htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('" + this.ID + "','" + (memoryCalendar.Year).ToString() + weeks[j, startMonth - 1].ToString("00") + "00')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + weeks[j, startMonth - 1] + ".gif\"></a></td>");																			
						}
						else
                            htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + weeks[j, startMonth - 1] + ".gif\"></td>");
					}
					htmlBuilder.Append("</tr>");
				}

			

				htmlBuilder.Append("<tr><td colspan=6  bgcolor="+_table_color_selected_date+"><font size=1 color="+_font_color_selected_date+" face=\"Arial\">"+dateSelectedString+"</font></td></tr>");
				htmlBuilder.Append("</table>");
				output.Write(htmlBuilder.ToString());

			}catch(Exception ex){
				throw new TNS.AdExpress.Web.Controls.Exceptions.LastMonthWeekCalendarWebControlException("Impossible d'afficher le calendrier de sélection des N derniers mois ou semaines.",ex);
			}

		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Donne le titre du calendar
		/// </summary>
		/// <returns></returns>
		private string getTitle(){
			switch(this.calendarType){
				case TNS.AdExpress.Web.Controls.Selections.LastMonthWeekCalendarWebControl.Type.dateBegin:
					return(LastMonthWeekCalendarWebControl.TITLE_DATE_BEGIN_PICTURE);
				case TNS.AdExpress.Web.Controls.Selections.LastMonthWeekCalendarWebControl.Type.dateEnd:
					return(LastMonthWeekCalendarWebControl.TITLE_DATE_END_PICTURE);
			}
			return("");
		}


		#endregion

		#region Méthode externes

		/// <summary>
		/// Indique si une date est sélectionnée
		/// </summary>
		/// <returns>True si une date est sélectionnée, false sinon</returns>
		public bool isDateSelected(){
			if(selectedDate>0) return(true);
			else return(false);
		}
		#endregion
		
		#endregion
	}
}
