#region Informations
// Auteur: G. Facon
// Date de cr�ation: 10/05/2004
// Date de modification: 
//   - 02/06/2005 D. V. Mussuma :  option d'affichage des mois et semaines dont les donn�es sont completement disponibles.
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;
using CustomerWebConstantes=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Calendrier afficher en Ann�es,mois,semaines
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MonthWeekCalendarWebControl runat=server></{0}:MonthWeekCalendarWebControl>")]
	public class MonthWeekCalendarWebControl : System.Web.UI.WebControls.WebControl{

		#region Enum�rateur
		/// <summary>
		/// Type du calendrier
		/// Cette �num�rateur sert � d�terminer le titre du calendrier
		/// </summary>
		public enum Type{
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

/*		#region Constantes
		/// <summary>
		/// Titre du d�but de p�riode en fran�ais
		/// </summary>
		private const string TITLE_DATE_BEGIN_PICTURE="Starting_date.gif";
		/// <summary>
		/// Titre de la fin de p�riode en fran�ais
		/// </summary>
		private const string TITLE_DATE_END_PICTURE="Ending_date.gif";
		/// <summary>
		/// Couleur de la bordure
		/// </summary>
		private const string TABLE_BORDER_COLOR="#9D9885";
		/// <summary>
		/// Couleur de fond
		/// </summary>
		private const string TABLE_COLOR="#FFFFFF";
		/// <summary>
		/// Couleur de fond de la date s�lectionn�e
		/// </summary>
		private const string TABLE_COLOR_SELECTED_DATE="#644883";
		/// <summary>
		/// Couleur de fond du titre dans les colonnes
		/// </summary>
		private const string TITLE_BACKGROUND_COLOR="#644883";
		/// <summary>
		/// Couleur de la font
		/// </summary>
		private const string FONT_COLOR="#644883";
		/// <summary>
		/// Couleur de la font de la date s�lectionn�e
		/// </summary>
		private const string FONT_COLOR_SELECTED_DATE="#FFFFFF";
		/// <summary>
		/// Couleur de fond quand il n'y a pas de date
		/// </summary>
		private const string TABLE_COLOR_NO_DATE="#9378B3";

		#endregion
 */

		#region Variables
		/// <summary>
		/// Type d'affichage
		/// </summary>
		private MonthWeekCalendarWebControl.Display display=MonthWeekCalendarWebControl.Display.all;
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
		protected TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Type calendarType=Type.unknown;
		/// <summary>
		/// Calendrier en m�moire
		/// </summary>
		private YearMonthWeekCalendar memoryCalendar=null;
		/// <summary>
		/// G�re l'affichage des mois et semaines dont les donn�es sont completement disponibles.
		/// </summary>
		protected bool _blockIncompleteDates = false;
		/// <summary>
		/// G�re l'affichage des mois et semaines dont les donn�es sont completement disponibles.
		/// Indique les semaines et mois imcomplets en rouge
		/// </summary>
		protected bool _incompleteDatesInRed = false;

		/// <summary>
		/// Dernier mois (YYYYMM) dont les donn�es sont compl�tement disponibles
		/// </summary>
		protected string _lastCompleteMonth = null;

		/// <summary>
		/// Indique si toutes les dates sont en rouges. 
		/// Par exemple lorsque l'on sait qu'on a aucune date compl�te
		/// </summary>
		protected bool _allDatesInRed = false;
        /// <summary>
        /// Vehicle information
        /// </summary>
        protected VehicleInformation _vehicleInformation = null;
        /// <summary>
        /// Current module ID
        /// </summary>
        protected long _currentModule = -1;
		#endregion

        #region Propri�t�
        /// <summary>
        /// Titre du d�but de p�riode en fran�ais
        /// </summary>
        private string _titleDateBeginPicture = "Starting_date.gif";
        /// <summary>
        /// Obtient et d�finit le Titre de d�but de p�riode � afficher
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("Starting_date.gif")]
        public string TitleDateBeginPicture {
            get { return _titleDateBeginPicture; }
            set {_titleDateBeginPicture = value; }
        }

        /// <summary>
        /// Titre de la fin de p�riode
        /// </summary>
        private string _titleDateEndPicture = "Ending_date.gif";
        /// <summary>
        /// Obtient et d�finit le Titre de fin de p�riode � afficher
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("Ending_date.gif")]
        public string TitleDateEndPicture {
            get { return _titleDateEndPicture; }
            set { _titleDateEndPicture = value; }
        }

        /// <summary>
        /// Couleur de la bordure
        /// </summary>
        private string _tableBorderColor = "#9D9885";
        /// <summary>
        /// Obtient et d�finit la Couleur de la bordure
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#9D9885")]
        public string TableBorderColor {
            get { return _tableBorderColor; }
            set { _tableBorderColor = value; }
        }

        /// <summary>
        /// Couleur de fond
        /// </summary>
        private string _tableColor = "#FFFFFF";
        /// <summary>
        /// Obtient et d�finit la Couleur de fond
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#FFFFFF")]
        public string TableColor {
            get { return _tableColor; }
            set { _tableColor = value; }
        }

        /// <summary>
        /// Couleur de fond de la date s�lectionn�e
        /// </summary>
        private string _tableColorSelectedDate = "#644883";
        /// <summary>
        /// Obtient et d�finit la Couleur de fond de la date s�lectionn�e
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#644883")]
        public string TableColorSelectedDate {
            get { return _tableColorSelectedDate; }
            set { _tableColorSelectedDate = value; }
        }

        /// <summary>
        /// Couleur de fond du titre dans les colonnes
        /// </summary>
        private string _titleBackGroundColor = "#644883";
        /// <summary>
        /// Obtient et d�finit la Couleur de fond du titre dans les colonnes
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#644883")]
        public string TitleBackGroundColor {
            get { return _titleBackGroundColor; }
            set { _titleBackGroundColor = value; }
        }

        /// <summary>
        /// Couleur de la font
        /// </summary>
        private string _fontColor = "#644883";
        /// <summary>
        /// Obtient et d�finit la Couleur de la font
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#644883")]
        public string FontColor {
            get { return _fontColor; }
            set { _fontColor = value; }
        }


        /// <summary>
        /// Couleur de la font
        /// </summary>
        private string _fontColorSelectedDate = "#FFFFFF";
        /// <summary>
        /// Obtient et d�finit la Couleur de la font
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#FFFFFF")]
        public string FontColorSelectedDate {
            get { return _fontColorSelectedDate; }
            set { _fontColorSelectedDate = value; }
        }


        /// <summary>
        /// Couleur de la font
        /// </summary>
        private string _tableColorNoDate = "#9378B3";
        /// <summary>
        /// Obtient et d�finit la Couleur de la font
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("#9378B3")]
        public string TableColorNoDate {
            get { return _tableColorNoDate; }
            set { _tableColorNoDate = value; }
        }
        #endregion

        #region Accesseurs
        /// <summary>
		/// Obtient et d�finit l'ann�e de d�but
		/// </summary>
		public int StartYear{
			get{return startYear;}
			set{startYear = value;}
		}

		/// <summary>
		/// Obtient et d�finit l'ann�e � afficher
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
		/// Obtient et d�finit la langue � utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public int Language{
			get{return language;}
			set{language = value;}
		}


		/// <summary>
		/// Obtient et d�finit le type de calendrier
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public MonthWeekCalendarWebControl.Type CalendarType{
			get{return calendarType;}
			set{calendarType = value;}
		}

		/// <summary>
		/// D�finit le niveau d'affichage
		/// </summary>
		public MonthWeekCalendarWebControl.Display DisplayType{
			set{this.display = value;}
		}

		/// <summary>
		/// Obtient le type de p�riode s�lectionn�
		/// </summary>
		public CustomerWebConstantes.Period.Type SelectedDateType{
			get{return(this.selectedDateType);}
		}

		/// <summary>
		/// Obtient la p�riode s�lectionn�e
		/// </summary>
		public int SelectedDate{
			get{return(this.selectedDate);}
		}

		/// <summary>
		/// G�re l'affichage d'un mois ou d'une semaine complete
		/// Bloque les dates (couleur gris + pas de s�lection possibles 
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(false)] 
		public bool BlockIncompleteDates{
			get{
				return _blockIncompleteDates;}
			set{
				if(_incompleteDatesInRed)throw(new System.MemberAccessException("blockIncompleteDates ne peut �tre � true si incompleteDatesInRed l'est d�j�"));
				_blockIncompleteDates=value;}
		}
		/// <summary>
		///G�re l'affichage d'un mois ou d'une semaine complete (donn�ees completement disponibles) 
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(false)] 
		public bool IncompleteDatesInRed{
			get{
				return _incompleteDatesInRed;}
			set{
				if(_blockIncompleteDates)throw(new System.MemberAccessException("incompleteDatesInRed ne peut �tre � true si _blockIncompleteDates l'est d�j�"));
				_incompleteDatesInRed=value;}
		}

		/// <summary>
		/// D�finit le dernier mois (YYYYMM) dont les donn�es sont compl�tement disponibles
		/// </summary>
		public string LastCompleteMonth{
			set{this._lastCompleteMonth = value;}
		}

		/// <summary>
		/// Indique si toutes les dates sont en rouges. 
		/// Par exemple lorsque l'on sait qu'on a aucune date compl�te
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(false)] 
		public bool AllDatesInRed{
			
			set{
				_allDatesInRed = value;
			}
		}

        /// <summary>
        /// Vehicle information
        /// </summary>
        public VehicleInformation VehicleInformation
        {

            set
            {
                _vehicleInformation = value;
            }
        }
        /// <summary>
        /// Current Module
        /// </summary>
        public long CurrentModule
        {

            set
            {
                _currentModule = value;
            }
        }
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MonthWeekCalendarWebControl(){
			this.selectedYear=DateTime.Now.Year;
			
		}
		#endregion
		
		#region Ev�nements

		#region Initialisation
		/// <summary>
		/// Initialisation de l'objet
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
			//option des mois ou semaines completes � afficher
			//ann�e  N-1 devient ann�e courante si le mois actuel est janvier
			if(_blockIncompleteDates || _incompleteDatesInRed){               
				if(_lastCompleteMonth!=null)this.selectedYear = int.Parse(_lastCompleteMonth.Substring(0,4));
				else if(IsStartWithPreviousYear(_blockIncompleteDates))this.selectedYear=DateTime.Now.Year-1; 
              
			}
            int nbYears = WebApplicationParameters.DataNumberOfYear - 1;
            startYear = selectedYear - nbYears;
			if(DateTime.Now.Month==12)stopYear=(DateTime.Now.AddYears(1)).Year;
			else {
				stopYear=DateTime.Now.Year;
				//option des mois ou semaines completes � afficher
				//ann�e  N-1 devient ann�e courante si le mois actuel est janvier
				if(_blockIncompleteDates || _incompleteDatesInRed){
					if(_lastCompleteMonth!=null)this.selectedYear = int.Parse(_lastCompleteMonth.Substring(0,4));
					else if(IsStartWithPreviousYear(_blockIncompleteDates))stopYear=DateTime.Now.Year-1; 
				}
			}
            //Patch Finland pour le Tableau de bord PRESSE
            if (WebApplicationParameters.CountryCode.Trim().Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND)
                && ( TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE == _currentModule
                || TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_TELEVISION == _currentModule
                || TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_RADIO == _currentModule
                 )
                && (_vehicleInformation != null && TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList.ContainsKey(_vehicleInformation.Id))
                && _blockIncompleteDates  )            
            {                
                    DateTime dat = TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList[_vehicleInformation.Id];
                    stopYear = selectedYear = dat.Year;
                    startYear = selectedYear - nbYears;
                    _lastCompleteMonth = String.Format("{0:yyyyMM}", dat);                 
               
            }
			selectedDate=-1;
			if (this.ViewState["selectedDate"]!=null){
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
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[language].Name;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[language].Localization);
			StringBuilder htmlBuilder=new StringBuilder(10500);
			string dateSelectedString="&nbsp;";
			string colSpan="";
			if(isDateSelected()){
				switch(selectedDateType){
					case CustomerWebConstantes.Period.Type.dateToDateMonth:
						dateSelectedString=MonthString.GetCharacters(int.Parse(selectedDate.ToString().Substring(4,2)),cultureInfo,12)+"&nbsp;"+selectedYear.ToString();
						break;
					case CustomerWebConstantes.Period.Type.dateToDateWeek:
						AtomicPeriodWeek week=new AtomicPeriodWeek(int.Parse(selectedDate.ToString().Substring(0,4)),int.Parse(selectedDate.ToString().Substring(4,2)));
						DateTime dateBegin=week.FirstDay;
						DateTime dateEnd=week.FirstDay.AddDays(6);
						dateSelectedString=dateBegin.Day.ToString()+"/"+dateBegin.Month.ToString()+"/"+dateBegin.Year.ToString()+" - "+dateEnd.Day.ToString()+"/"+dateEnd.Month.ToString()+"/"+dateEnd.Year.ToString();
						break;
				}
			}
			
			htmlBuilder.Append("<!--Debut Calendrier--->");
			htmlBuilder.Append("<table cellspacing=1 cellpadding=0 border=0 bgcolor="+_tableBorderColor+">");
			string tmpHTML="";
			string titleImage=this.getTitle();
			// Titre
			colSpan = (display==MonthWeekCalendarWebControl.Display.week)?"colspan=5":"";
			if(titleImage.Length>0){
                tmpHTML = "<td " + colSpan + "><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/" + titleImage + "\"></td>";
			}
			else{
			tmpHTML="<td "+colSpan+">&nbsp;</td>";
			}

			if(display!=MonthWeekCalendarWebControl.Display.all){
				htmlBuilder.Append("<tr bgcolor="+_tableColor+">");
				htmlBuilder.Append(tmpHTML);
				htmlBuilder.Append("</tr>");
			}
			// Fl�ches
			string left_arrow_HTML;
			string right_arrow_HTML;
			if(selectedYear>startYear)
				left_arrow_HTML="<td align=\"left\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear-1).ToString()+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Arrow_left_up.gif\"></a></td>";
			else
                left_arrow_HTML = "<td><img width=\"11px\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>";
			if(selectedYear<stopYear)
				right_arrow_HTML="<td align=\"right\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear+1).ToString()+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Arrow_right_up.gif\"></a></td>";
			else
                right_arrow_HTML = "<td><img width=\"11px\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>";

			htmlBuilder.Append("<tr bgcolor="+_tableColor+">");
			if(display==MonthWeekCalendarWebControl.Display.all)
				htmlBuilder.Append(tmpHTML);

			//S�lection des ann�es
			colSpan = (display!=MonthWeekCalendarWebControl.Display.month)?"colspan=5":"";
			htmlBuilder.Append("<td "+colSpan+">");
			htmlBuilder.Append("<table width=\"100%\"cellspacing=0 cellpadding=0 border=0 bgcolor="+_tableColor+">");
			htmlBuilder.Append("<tr>");
			htmlBuilder.Append(left_arrow_HTML);
			htmlBuilder.Append("<td colspan=3 align=\"center\"><font size=1 color="+_fontColor+" face=\"Arial\">"+this.selectedYear.ToString()+"</font></td>");
			htmlBuilder.Append(right_arrow_HTML);
			htmlBuilder.Append("</tr>");
			htmlBuilder.Append("</table>");
			htmlBuilder.Append("</td>");
			htmlBuilder.Append("</tr>");

			//Titre des colonnes
			htmlBuilder.Append("<tr bgcolor=\""+_titleBackGroundColor+"\" >");
			
			if(display==MonthWeekCalendarWebControl.Display.month || display==MonthWeekCalendarWebControl.Display.all)
			htmlBuilder.Append("<td align=\"center\"><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/monthTitle.gif\" border=0></td>");
			
			if(display==MonthWeekCalendarWebControl.Display.week || display==MonthWeekCalendarWebControl.Display.all)
			htmlBuilder.Append("<td colspan=5 align=\"center\"><img src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/weekTitle.gif\" border=0></td>");
			
			htmlBuilder.Append("</tr>");


			int i,j;
			if(memoryCalendar==null) memoryCalendar = new YearMonthWeekCalendar(this.selectedYear);
			int[,] weeks=this.memoryCalendar.WeeksTable;
			for(i=0;i<weeks.GetLength(1);i++){
				htmlBuilder.Append("<tr>");
				if(display!=MonthWeekCalendarWebControl.Display.week){
					if( !_blockIncompleteDates && !_incompleteDatesInRed ) 
						htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+".gif\"></a></td>");
					else {
						if(_incompleteDatesInRed){
							//option d'affichage des mois incomplets en rouge
							if(IsMonthLinkEnabled(i+1)) 
								htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+".gif\"></a></td>");
							else 
								htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+"NC.gif\"></a></td>");
						}
						else{
							//option d'affichage uniquement des mois complets(donn�es enti�rement disponibles)
							if(IsMonthLinkEnabled(i+1)) 
								htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+".gif\"></a></td>");
							else 
								htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+"G.gif\"></td>");
						}
					}
				}
//				else htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/Month_"+(i+1).ToString()+".gif\"></td>");
				
				for(j=0;j<weeks.GetLength(0);j++){
					if(display!=MonthWeekCalendarWebControl.Display.month){
						if(weeks[j,i]!=0){ //&& display!=MonthWeekCalendarWebControl.Display.month){
							if(!_blockIncompleteDates && !_incompleteDatesInRed )
								htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+weeks[j,i].ToString("00")+"00')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+".gif\"></a></td>");							
							else{
								if(_incompleteDatesInRed){
									//option d'affichage des semaines incompletes en rouge
									if(IsWeekLinkEnabled(int.Parse(weeks[j,i].ToString()),this.selectedYear) ) 
										htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+weeks[j,i].ToString("00")+"00')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+".gif\"></a></td>");
									else 
										htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+weeks[j,i].ToString("00")+"00')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+"NC.gif\"></a></td>");
								}
								else{
									//option d'affichage uniquement des semaines completes(donn�es enti�rement disponibles)
									if(IsWeekLinkEnabled(int.Parse(weeks[j,i].ToString()),this.selectedYear) ) 
										htmlBuilder.Append("<td><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+weeks[j,i].ToString("00")+"00')\"><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+".gif\"></a></td>");
									else 
										htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+"G.gif\"></a></td>");
								}
							}
						}else htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+".gif\"></td>");
					}
//					else htmlBuilder.Append("<td><img border=0 src=\"/App_Themes/" + themeName + "/Images/Culture/Calendar/"+weeks[j,i]+".gif\"></td>");
				}
				htmlBuilder.Append("</tr>");
			}
			switch(display){
				case Display.week :
					colSpan = "colspan=5";break;					
				case Display.all :
					colSpan = "colspan=6";break;
				default :
					colSpan = "";break;
			}
			htmlBuilder.Append("<tr><td "+colSpan+"  bgcolor="+_tableColorSelectedDate+"><font size=1 color="+_fontColorSelectedDate+" face=\"Arial\">"+dateSelectedString+"</font></td></tr>");
			htmlBuilder.Append("</table>");
			htmlBuilder.Append("<!--Fin Calendrier--->");
			output.Write(htmlBuilder.ToString());
		}
		#endregion

		#endregion

		#region M�thodes internes
		/// <summary>
		/// Donne le titre du calendar
		/// </summary>
		/// <returns></returns>
		private string getTitle(){
			switch(this.calendarType){
				case TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Type.dateBegin:
					return(this._titleDateBeginPicture);
				case TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Type.dateEnd:
					return(this._titleDateEndPicture);
			}
			return("");
		}
		#endregion

		#region M�thode externes

		/// <summary>
		/// Indique si une date est s�lectionn�e
		/// </summary>
		/// <returns>True si une date est s�lectionn�e, false sinon</returns>
		public bool isDateSelected(){
			if(selectedDate>0) return(true);
			else return(false);
		}

		/// <summary>
		/// G�re l'activaton du lien permettant de s�lectionner un mois.
		/// </summary>
		/// <param name="month">mois</param>
		/// <returns>vrai si le mois est s�lectionnable</returns>
		public bool IsMonthLinkEnabled(int month){
			bool enabled =false;
			double currentWeek = ((double)DateTime.Now.Day/(double)7);
			currentWeek=Math.Ceiling(currentWeek);
			AtomicPeriodWeek week=new AtomicPeriodWeek(DateTime.Now);			
			DateTime dateBegin=week.FirstDay;
			AtomicPeriodWeek previousWeek;	
			
			if(_lastCompleteMonth == null && !_allDatesInRed){
				#region Verifie si les donn�es du mois sont completes en fonction de la date actuelle
				if(this.selectedYear==DateTime.Now.Year && month==DateTime.Now.Month-1 && DateTime.Now.Month>1){
					//1ere semaine du mois pr�c�dent le mois en cours
					if(currentWeek==1 && week.FirstDay.Month>month 
						&& (int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
						|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())==0)){
						enabled = true;
					}
						//2�me semaine du mois pr�c�dent le mois en cours
					else if(month==DateTime.Now.Month-1 && currentWeek==2){
						if((int) DateTime.Now.DayOfWeek>=5 || (int) DateTime.Now.DayOfWeek==0)
							enabled = true;
						else {
							previousWeek=new AtomicPeriodWeek(DateTime.Now.AddDays(-7));
							if(previousWeek.FirstDay.Month==previousWeek.LastDay.Month)
								enabled = true;
						}
						//Plus de 2 semaines du mois pr�c�dent le mois en cours
					}else if(currentWeek>2){
						enabled = true;
					}
				}
					//Mois trait� inf�rieur au mois pr�c�dent le mois en cours
				else if(month<DateTime.Now.Month-1 && this.selectedYear==DateTime.Now.Year)
					enabled =true;
					//Mois trait� est le 12�me mois de l'ann�e pr�c�dent l'ann� actuelle
				else if(this.selectedYear==DateTime.Now.Year-1 && month==12){	
					if( DateTime.Now.Month==1){
						//1ere ou 2�me semaine ann�e courante
						if((week.Week==1 ) && week.FirstDay.Month==week.LastDay.Month &&
							(int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
							|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())==0)){
							enabled = true;								
							//Plus de 2 semaines du mois pr�c�dent le mois en cours
						}else if(week.Week==2 && ((int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
							|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())==0)
							|| (week.FirstDay.AddDays(-7).Month==week.LastDay.AddDays(-7).Month))
							){
							enabled = true;
						}else if(week.Week>2 && week.Week!=52 && week.Week!=53){
							enabled = true;
						}	
					}else enabled = true;
					//	Ann�e s�lectionn� est N-1 et mois trait�<12, ou ann�e s�lectionn�e est N-2
				}else if((this.selectedYear==DateTime.Now.Year-1 && month<12) || (this.selectedYear<DateTime.Now.Year-1)){
					enabled = true;
				}
				#endregion
			
			}else{
				#region Verifie si les donn�es du mois sont completes � partir de la derni�re date de disponibilit� (_lastCompleteMonth)
				string tempDate = this.selectedYear.ToString() + ((month.ToString().Length==1)?"0"+month.ToString() : month.ToString());
				enabled = ( (_lastCompleteMonth != null && int.Parse(tempDate)<= int.Parse(_lastCompleteMonth)) && !_allDatesInRed) ? true : false;
				#endregion
			}
			return enabled;
		}
		
		/// <summary>
		/// G�re l'activaton du lien permettant de s�lectionner une semaine.
		/// </summary>
		/// <param name="week">semaine</param>
		/// <param name="year">ann�e </param>
		/// <returns>vrai si la semaine est s�lectionnable</returns>
		public bool IsWeekLinkEnabled(int week,int year){
			bool enabled = false;						
			AtomicPeriodWeek currentWeek=new AtomicPeriodWeek(DateTime.Now);
			AtomicPeriodWeek targetWeek=new AtomicPeriodWeek(year,week);
			int numberWeek=currentWeek.Week;
			int days=0;			
			if(!_allDatesInRed){
				days = DateTime.Now.Subtract(targetWeek.LastDay).Days;
				if(days>=5)return true;
			}
			return enabled;
		}

		/// <summary>
		/// L'ann�e pr�c�dente est la l'ann�e de r�f�rence 
		/// du calendrier, si le mois de l'ann�e actuelle est janvier et que nous sommes en dessous de 5 jours
		/// apr�s la premi�re semaine complete.
		/// </summary>
		/// <param name="showOnlyCompleteDate">vrai s'il ne faut afficher que des mois ou semaines completes.</param>
		/// <returns>vrai si calendrier d�but avec l'ann�e pr�c�dente</returns>
		public bool IsStartWithPreviousYear(bool showOnlyCompleteDate){
			AtomicPeriodWeek week=new AtomicPeriodWeek(DateTime.Now);
			DateTime dateBegin=week.FirstDay;
			
			AtomicPeriodWeek week2;
					
			if(showOnlyCompleteDate){
				if(DateTime.Now.Month==1 && ( week.Week==1 || week.Week==52 || week.Week==53))
					return true;
				else if(DateTime.Now.Month==1 && (week.Week==2 || week.Week==3)){
					week2=new AtomicPeriodWeek(DateTime.Now.Year,week.Week-1);
					if( week2.FirstDay.Month==week2.LastDay.Month 
						&& (int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
						|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())== 0)){
						return false;
					}else return true;				
				}
				else return false;
			}else return false;
		}
		#endregion
	}
}

