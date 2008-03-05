#region Informations
// Auteur: G. Facon
// Date de création: 30/06/2004
// Date de modification: 06/07/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.Date;
using CustomerWebConstantes=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Calendrier afficher en Années,mois,semaines
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:DayCalendarWebControl runat=server></{0}:DayCalendarWebControl>")]
	public class DayCalendarWebControl : System.Web.UI.WebControls.WebControl{

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
		#endregion

		#region Variables
		/// <summary>
		/// Année du début de calendrier
		/// </summary>
		protected int startMonth;
		/// <summary>
		/// Année du fin de calendrier
		/// </summary>
		protected int stopMonth;
		/// <summary>
		/// Année sélectionnée
		/// </summary>
		protected int selectedMonth;
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
		protected TNS.AdExpress.Web.Controls.Selections.DayCalendarWebControl.Type calendarType=Type.unknown;
		/// <summary>
		/// Calendrier en mémoire
		/// </summary>
		private DayCalendar memoryCalendar=null;
		/// <summary>
		/// Etendu d'une période
		/// </summary>
		private int _periodLength = 3;
        /// <summary>
        /// Border color
        /// </summary>
        private string _tableBorderColor = "#9D9885";
        /// <summary>
        /// Table color
        /// </summary>
        private string _tableColor = "#FFFFFF";
        /// <summary>
        /// Font color
        /// </summary>
        private string _fontColor = "#644883";
        /// <summary>
        /// Table color for selected date
        /// </summary>
        private string _tableColorSelectedDate = "#644883";
        /// <summary>
        /// Font color for selected date
        /// </summary>
        private string _fontColorSelectedDate = "#FFFFFF";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DayCalendarWebControl(){
			this.selectedMonth=int.Parse(MonthString.GetYYYYMM(DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString()));
		}
		#endregion
	
		#region Accesseurs
		/// <summary>
		/// Ontient et définit le mois à afficher
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("200406")] 
		public int SelectedMonth{
			get{return selectedMonth;}
			set{
				selectedMonth = value;
				if(memoryCalendar==null)memoryCalendar=new DayCalendar(value); 
				else memoryCalendar.YearMonth=value;
			}
		}

		/// <summary>
		/// Ontient et définit la langue à utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public int Language{
			get{return language;}
			set{language = value;}
		}


		/// <summary>
		/// Ontient et définit le type de Calendrier (date de début ou date de fin)
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public DayCalendarWebControl.Type CalendarType{
			get{return calendarType;}
			set{calendarType = value;}
		}

		/// <summary>
		/// Obtient la période sélectionnée
		/// </summary>
		public int SelectedDate{
			get{return(this.selectedDate);}
		}


		/// <summary>
		/// Obtient/Définit l'étendu d'une période
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("3")] 
		public int PeriodLength{
			get{return _periodLength;}
			set{
				_periodLength = value;				
			}
		}

        /// <summary>
        /// Get or Set Table border color
        /// </summary>
        public string TableBorderColor {
            get { return _tableBorderColor; }
            set { _tableBorderColor = value; }
        }

        /// <summary>
        /// Get or Set Table color
        /// </summary>
        public string TableColor {
            get { return _tableColor; }
            set { _tableColor = value; }
        }

        /// <summary>
        /// Get or Set Font color
        /// </summary>
        public string FontColor {
            get { return _fontColor; }
            set { _fontColor = value; }
        }

        /// <summary>
        /// Get or Set Table color for selected date
        /// </summary>
        public string TableColorSelectedDate {
            get { return _tableColorSelectedDate; }
            set { _tableColorSelectedDate = value; }
        }

        /// <summary>
        /// Get or Set Font color for selected date
        /// </summary>
        public string FontColorSelectedDate {
            get { return _fontColorSelectedDate; }
            set { _fontColorSelectedDate = value; }
        }
		#endregion

		#region Evènements

		#region Initialisation
		/// <summary>
		/// Initialisation de l'objet
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
			DateTime dt=DateTime.Now;
			DateTime dtEnd=DateTime.Now;
//			dt=dt.AddMonths(-3);
			dt=dt.AddMonths(-_periodLength);
			startMonth=int.Parse(MonthString.GetYYYYMM(dt.Year.ToString()+dt.Month.ToString()));
			if(DateTime.Now.Month==12)dtEnd=dtEnd.AddMonths(1);
			stopMonth=int.Parse(MonthString.GetYYYYMM(dtEnd.Year.ToString()+dtEnd.Month.ToString()));
			selectedDate=-1;
			if (this.ViewState["selectedDate"]!=null){
				selectedDate=(int)this.ViewState["selectedDate"];
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
			}
			if (this.ViewState["selectedMonth"]!=null){
				selectedMonth=(int)this.ViewState["selectedMonth"];
			}
			if (Page.IsPostBack){
				// TEST A FAIRE POUR VERIFIER QUE L'ON PREND LE BON CALENDARMONTH
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput==this.ID){
					string valueInput=Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
					switch(valueInput.Length){
						case 6:
							selectedMonth=int.Parse(valueInput);
							ViewState.Add("selectedMonth",selectedMonth);
							//memoryCalendar=new YearMonthWeekCalendar(this.selectedYear);
							break;
						case 8:
							selectedDate=int.Parse(valueInput);
							ViewState.Add("selectedDate",selectedDate);
							if (this.ViewState["yearSelected"]!=null){
							
							}
							break;
					}
				}	
			}
		}
		#endregion

		#region PréRender
		
		#endregion

		#region Affichage
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			string dateSelectedString="&nbsp;";
            string themeName = WebApplicationParameters.Themes[language].Name;
			StringBuilder htmlBuilder=new StringBuilder(6500);
			if(isDateSelected()){
				dateSelectedString=DateString.YYYYMMDDToDD_MM_YYYY(selectedDate,language);
			}

			htmlBuilder.Append("<table cellspacing=1 cellpadding=0 border=0 bgcolor="+_tableBorderColor+">");
			string tmpHTML="";
			
			// Titre
			string titleImage=this.getTitle();
			if(titleImage.Length>0){
                tmpHTML = "<td colspan=7><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + titleImage + "\"></td>";
			}
			else{
			tmpHTML="<td colspan=7>&nbsp;</td>";
			}

			// Flèche
			string left_arrow_HTML;
			string right_arrow_HTML;
			string nextMonth,previousMonth;
			if(selectedMonth>startMonth){
				if(int.Parse(this.selectedMonth.ToString().Substring(4,2))==01)	previousMonth=(int.Parse(this.selectedMonth.ToString().Substring(0,4))-1).ToString()+"12";
				else previousMonth=(this.selectedMonth-1).ToString();
                left_arrow_HTML = "<td align=\"left\"><a href=\"javascript:__doPostBack('" + this.ID + "','" + previousMonth + "')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Arrow_left_up.gif\"></a></td>";
			}
			else
				left_arrow_HTML="<td><img width=\"11px\" src=\"/Images/Common/pixel.gif\"></td>";
			if(selectedMonth<stopMonth){
				if(int.Parse(this.selectedMonth.ToString().Substring(4,2))==12)	nextMonth=(int.Parse(this.selectedMonth.ToString().Substring(0,4))+1).ToString()+"01";
				else nextMonth=(this.selectedMonth+1).ToString();
                right_arrow_HTML = "<td align=\"right\"><a href=\"javascript:__doPostBack('" + this.ID + "','" + nextMonth + "')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Arrow_right_up.gif\"></a></td>";
			}
			else
				right_arrow_HTML="<td><img width=\"11px\" src=\"/Images/Common/pixel.gif\"></td>";


			htmlBuilder.Append("<tr bgcolor="+_tableColor+">");
			htmlBuilder.Append(tmpHTML);
			htmlBuilder.Append("</tr><tr>");
			// Sélection des mois
			htmlBuilder.Append("<td colspan=7>");
			htmlBuilder.Append("<table width=\"100%\"cellspacing=0 cellpadding=0 border=0 bgcolor="+_tableColor+">");
			htmlBuilder.Append("<tr>");
			htmlBuilder.Append(left_arrow_HTML);
			htmlBuilder.Append("<td colspan=3 align=\"center\"><font size=1 color="+_fontColor+" face=\"Arial\">"+MonthString.Get(int.Parse(this.selectedMonth.ToString().Substring(4,2)),language,0)+"&nbsp;"+this.selectedMonth.ToString().Substring(0,4)+"</font></td>");
			htmlBuilder.Append(right_arrow_HTML);
			htmlBuilder.Append("</tr>");
			htmlBuilder.Append("</table>");
			htmlBuilder.Append("</td>");
			htmlBuilder.Append("</tr>");
			// Nom de colonnes
			htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_1.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_2.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_3.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_4.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_5.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_6.gif\" border=0></td>");
            htmlBuilder.Append("<td><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Day_7.gif\" border=0></td>");
			htmlBuilder.Append("</tr>");
			int i,j;
			if(memoryCalendar==null) memoryCalendar=new DayCalendar(this.selectedMonth);
			int[,] days=this.memoryCalendar.DaysTable;
			for(i=0;i<days.GetLength(0);i++){
				htmlBuilder.Append("<tr>");
				for(j=0;j<days.GetLength(1);j++){
					if(days[i,j]!=0)
                        htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('" + this.ID + "','" + (this.selectedMonth).ToString() + days[i, j].ToString("00") + "')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + days[i, j] + ".gif\"></a></td>");
					else
						htmlBuilder.Append("<td bgcolor=\"#9378B3\"><img width=\"27\" height=\"13\" src=\"/Images/Common/pixel.gif\"></td>");
				}
				htmlBuilder.Append("</tr>");
			}
			htmlBuilder.Append("<tr><td colspan=7  bgcolor="+_tableColorSelectedDate+"><font size=1 color="+_fontColorSelectedDate+" face=\"Arial\">"+dateSelectedString+"</font></td></tr>");
			htmlBuilder.Append("</table>");
			output.Write(htmlBuilder.ToString());
		}
		#endregion

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Donne le titre du calendar
		/// </summary>
		/// <returns></returns>
		private string getTitle(){
			switch(this.calendarType){
				case TNS.AdExpress.Web.Controls.Selections.DayCalendarWebControl.Type.dateBegin:
					return(DayCalendarWebControl.TITLE_DATE_BEGIN_PICTURE);
				case TNS.AdExpress.Web.Controls.Selections.DayCalendarWebControl.Type.dateEnd:
					return(DayCalendarWebControl.TITLE_DATE_END_PICTURE);
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
	}
}

