#region Informations
// Auteur: D.V. MuSSUMA
// Date de cr�ation: 21/09/2004
// Date de modification: 21/09/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;
using CustomerWebConstantes=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Calendrier afficher en Ann�es,mois
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MonthCalendarWebControl runat=server></{0}:MonthCalendarWebControl>")]
	public class MonthCalendarWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Enum�rateur
		/// <summary>
		/// Type du calendrier
		/// Cette �num�rateur sert � d�terminer le titre du calendrier
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Ind�fini
			/// </summary>
			unknown,
			/// <summary>
			/// Date de d�but
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
		public enum Display
		{
//			/// <summary>
//			/// Mois et semaines
//			/// </summary>
//			all,
			/// <summary>
			/// mois
			/// </summary>
			month,
//			/// <summary>
//			/// semaine
//			/// </summary>
//			week
		}
		#endregion

		#region Constantes
		/// <summary>
		/// Titre du d�but de p�riode en fran�ais
		/// </summary>
		private const string TITLE_DATE_BEGIN_PICTURE="Starting_date.gif";
		/// <summary>
		/// Titre de la fin de p�riode en fran�ais
		/// </summary>
		private const string TITLE_DATE_END_PICTURE="Ending_date.gif";
		#endregion

		#region Variables
		/// <summary>
		/// Type d'affichage
		/// </summary>
		private MonthCalendarWebControl.Display display=MonthCalendarWebControl.Display.month;
		/// <summary>
		/// Ann�e du d�but de calendrier
		/// </summary>
		protected int startYear;
		/// <summary>
		/// Ann�e du fin de calendrier
		/// </summary>
		protected int stopYear;
		/// <summary>
		/// Ann�e s�lectionn�e
		/// </summary>
		protected int selectedYear=DateTime.Now.Year;
		/// <summary>
		/// Date s�lectionn�e
		/// </summary>
		protected int selectedDate;
		/// <summary>
		/// Type de date s�lectionn�e
		/// </summary>
		protected CustomerWebConstantes.Period.Type selectedDateType;

		/// <summary>
		/// Langue utilis�
		/// </summary>
		private int language;
		/// <summary>
		/// type du calendrier, il d�termine le titre du calendrier
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type calendarType=Type.unknown;
		/// <summary>
		/// Calendrier en m�moire
		/// </summary>
		private YearMonthCalendar memoryCalendar=null;
		/// <summary>
		/// Date de chargement pour les recaps
		/// </summary>
		protected int downloadDate=0;

        /// <summary>
        /// Table border color
        /// </summary>
        protected string _tableBorderColor = "#9D9885";
        /// <summary>
        /// Table color
        /// </summary>
        protected string _tableColor = "#FFFFFF";
        /// <summary>
        /// Table color selected date
        /// </summary>
        protected string _tableColorSelectedDate = "#644883";
        /// <summary>
        /// Title background color
        /// </summary>
        protected string _titleBackgroundColor = "#644883";
        /// <summary>
        /// Font color
        /// </summary>
        protected string _fontColor = "#644883";
        /// <summary>
        /// Font color selected date
        /// </summary>
        protected string _fontColorSelectedDate = "#FFFFFF";
        /// <summary>
        /// Table color for no date
        /// </summary>
        protected string _tableColorNoDate = "#9378B3";
        /// <summary>
        /// Td background color
        /// </summary>
        protected string _tdBgcolor = "#FFFFFF";
		#endregion

		#region Accesseurs
		/// <summary>
		/// Ontient et d�finit l'ann�e � afficher
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("2005")] 
		public int SelectedYear
		{
			get{return selectedYear;}
			set
			{
				selectedYear = value;
				if(memoryCalendar==null)new YearMonthCalendar(value); 
				else memoryCalendar.Year=value;
			}
		}

		/// <summary>
		/// Ontient et d�finit la langue � utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public int Language
		{
			get{return language;}
			set{language = value;}
		}
		
	
		/// <summary>
		/// Obtient ou d�finit la webSession 
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Obtient ou d�finit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Ontient et d�finit le type de calendrier
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public MonthCalendarWebControl.Type CalendarType
		{
			get{return calendarType;}
			set{calendarType = value;}
		}

		/// <summary>
		/// D�finit le niveau d'affichage
		/// </summary>
		public MonthCalendarWebControl.Display DisplayType
		{
			set{this.display = value;}
		}

		/// <summary>
		/// Obtient le type de p�riode s�lectionn�
		/// </summary>
		public CustomerWebConstantes.Period.Type SelectedDateType
		{
			get{return(this.selectedDateType);}
		}

		/// <summary>
		/// Obtient la p�riode s�lectionn�e
		/// </summary>
		public int SelectedDate
		{
			get{return(this.selectedDate);}
		}

        /// <summary>
        /// Get or Set table border color
        /// </summary>
        public string TableBorderColor {
            get { return _tableBorderColor; }
            set { _tableBorderColor = value; }
        }
        /// <summary>
        /// Get or Set table color
        /// </summary>
        public string TableColor {
            get { return _tableColor; }
            set { _tableColor = value; }
        }
        /// <summary>
        /// Get or Set table color selected date
        /// </summary>
        public string TableColorSelectedDate {
            get { return _tableColorSelectedDate; }
            set { _tableColorSelectedDate = value; }
        }
        /// <summary>
        /// Get or Set title background color
        /// </summary>
        public string TitleBackgroundColor {
            get { return _titleBackgroundColor; }
            set { _titleBackgroundColor = value; }
        }
        /// <summary>
        /// Get or Set font color
        /// </summary>
        public string FontColor {
            get { return _fontColor; }
            set { _fontColor = value; }
        }
        /// <summary>
        /// Get or Set color selected date
        /// </summary>
        public string FontColorSelectedDate {
            get { return _fontColorSelectedDate; }
            set { _fontColorSelectedDate = value; }
        }
        /// <summary>
        /// Get or Set table color for no date
        /// </summary>
        public string TableColorNoDate {
            get { return _tableColorNoDate; }
            set { _tableColorNoDate = value; }
        }
        /// <summary>
        /// Get or Set Td background color
        /// </summary>
        public string TdBgcolor {
            get { return _tdBgcolor; }
            set { _tdBgcolor = value; }
        }
		#endregion
		
		#region Ev�nements

		#region Initialisation
		/// <summary>
		/// Initialisation de l'objet
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) 
		{
			base.OnInit (e);
			
			// Attaquer une table
			downloadDate=webSession.DownLoadDate;

			if(downloadDate==DateTime.Now.Year){
				startYear=selectedYear-2;
				stopYear=DateTime.Now.Year;
			}
			else{
				selectedYear=selectedYear-1;
				startYear=selectedYear-2;
				stopYear=DateTime.Now.Year-1;
			}
			selectedDate=-1;
			if (this.ViewState["selectedDate"]!=null)
			{
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
		}

		#endregion

		#region Chargement
		/// <summary>
		/// Ev�nement de chargement du composant
		/// </summary>
		/// <param name="e">Argument</param>
		protected override void OnLoad(EventArgs e) 
		{
			if (this.ViewState["selectedDate"]!=null)
			{
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
			if (this.ViewState["selectedYear"]!=null)
			{
				selectedYear=(int)this.ViewState["selectedYear"];
			}
			if (Page.IsPostBack)
			{
				// TEST A FAIRE POUR VERIFIER QUE L'ON PREND LE BON CALENDARMONTH
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput==this.ID)
				{
					string valueInput=Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
					switch(valueInput.Length)
					{
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
							break;						
					}
				}	
			}
		}
		#endregion

		#region Affichage
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output)
		{
			StringBuilder htmlBuilder=new StringBuilder(10500);
			string dateSelectedString="&nbsp;";
            string themeName = WebApplicationParameters.Themes[language].Name;

			if(isDateSelected())
			{
				dateSelectedString=MonthString.Get(int.Parse(selectedDate.ToString().Substring(4,2)),language,12)+"&nbsp;"+selectedYear.ToString();
				
			}
			htmlBuilder.Append("<table cellspacing=1 cellpadding=0 border=0 bgcolor="+_tableBorderColor+">");
			string tmpHTML="";
			string titleImage=this.getTitle();
			// Titre
			if(titleImage.Length>0)
			{
				tmpHTML="<td colspan=6 align=\"center\"><img src=\"/App_Themes/"+themeName+"/Images/Culture/Calendar/"+titleImage+"\"></td>";
			}
			else
			{
				tmpHTML="<td colspan=6 align=\"center\">&nbsp;</td>";
			}
			// Fl�che
			string left_arrow_HTML;
			string right_arrow_HTML;
			//Old version
			if(selectedYear>startYear)
				left_arrow_HTML="<td align=\"left\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear-1).ToString()+"')\"><img border=0 src=\"/App_Themes/"+themeName+"/Images/Culture/Calendar/Arrow_left_up.gif\"></a></td>";
			else
				left_arrow_HTML="<td><img width=\"11px\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\"></td>";
			if(selectedYear<stopYear)
				right_arrow_HTML="<td align=\"right\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear+1).ToString()+"')\"><img border=0 src=\"/App_Themes/"+themeName+"/Images/Culture/Calendar/Arrow_right_up.gif\"></a></td>";
			else
				right_arrow_HTML="<td><img width=\"11px\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\"></td>";

			

			htmlBuilder.Append("<tr bgcolor="+_tableColor+">");
			htmlBuilder.Append(tmpHTML);
			htmlBuilder.Append("</tr>");
			//Affichage ann�e en cours
			htmlBuilder.Append("<tr>");

			htmlBuilder.Append("<td>");
			htmlBuilder.Append("<table width=\"100%\"cellspacing=0 cellpadding=0 border=0 bgcolor="+_tableColor+">");
			htmlBuilder.Append("<tr>");

			htmlBuilder.Append(left_arrow_HTML);
			htmlBuilder.Append("<td colspan=3 align=\"center\"><font size=1 color="+_fontColor+" face=\"Arial\">"+this.selectedYear.ToString()+"</font></td>");
			htmlBuilder.Append(right_arrow_HTML);

			htmlBuilder.Append("</tr>");
			htmlBuilder.Append("</table>");
			htmlBuilder.Append("</td>");

			htmlBuilder.Append("</tr>");
			//Titre  colonne mois
			htmlBuilder.Append("<tr bgcolor=\""+_titleBackgroundColor+"\" >");
			htmlBuilder.Append("<td colspan=6 align=\"center\"><img src=\"/App_Themes/"+themeName+"/Images/Culture/Calendar/monthTitle.gif\" border=0></td>");			
			htmlBuilder.Append("</tr>");
			int i;
			if(memoryCalendar==null) memoryCalendar=new YearMonthCalendar(this.selectedYear);
			int[] months=this.memoryCalendar.MonthsTable;
			for(i=0;i<months.Length;i++)
			{
				htmlBuilder.Append("<tr>");
				if(display==MonthCalendarWebControl.Display.month) htmlBuilder.Append("<td colspan=6 bgcolor="+_tdBgcolor+"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/App_Themes/"+themeName+"/Images/Culture/Calendar/Month_"+(i+1).ToString()+".gif\"></a></td>");				
				htmlBuilder.Append("</tr>");
			}
			htmlBuilder.Append("<tr><td colspan=6  bgcolor="+_tableColorSelectedDate+"><font size=1 color="+_tableColorSelectedDate+" face=\"Arial\">"+dateSelectedString+"</font></td></tr>");
			htmlBuilder.Append("</table>");
			output.Write(htmlBuilder.ToString());
		}
		#endregion

		#endregion

		#region M�thodes internes
		/// <summary>
		/// Donne le titre du calendar
		/// </summary>
		/// <returns></returns>
		private string getTitle()
		{
			switch(this.calendarType)
			{
				case TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type.dateBegin:
					return(MonthCalendarWebControl.TITLE_DATE_BEGIN_PICTURE);
				case TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type.dateEnd:
					return(MonthCalendarWebControl.TITLE_DATE_END_PICTURE);
			}
			return("");
		}
		#endregion

		#region M�thode externes

		/// <summary>
		/// Indique si une date est s�lectionn�e
		/// </summary>
		/// <returns>True si une date est s�lectionn�e, false sinon</returns>
		public bool isDateSelected()
		{
			if(selectedDate>0) return(true);
			else return(false);
		}

		#endregion

		
	}
}
