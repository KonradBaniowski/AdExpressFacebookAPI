#region Informations
// Auteur: G. Facon
// Date de création: 01/08/2007
// Date de modification: 24/09/2007 Y. R'kaina
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.FrameWork.Date;
using CustomerWebConstantes = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Web;
using Obout = OboutInc.SlideMenu;

namespace TNS.AdExpress.Web.Controls.Selections{
    /// <summary>
    /// Calendrier Global
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GlobalCalendarWebControl runat=server></{0}:GlobalCalendarWebControl>")]
    public class GlobalCalendarWebControl : WebControl{

        #region enum
        /// <summary>
        /// Type de la période
        /// </summary>
        public enum selectedDateT {
            /// <summary>
            /// Day
            /// </summary>
            day = 1,
            /// <summary>
            /// DayToDay
            /// </summary>
            dayToDay = 2,
            /// <summary>
            /// Month
            /// </summary>
            month = 3,
            /// <summary>
            /// MonthToMonth
            /// </summary>
            monthToMonth = 4,
            /// <summary>
            /// Trimester
            /// </summary>
            trimester = 5,
            /// <summary>
            /// TrimesterToTrimester
            /// </summary>
            trimesterToTrimester = 6,
            /// <summary>
            /// Semester
            /// </summary>
            semester = 7,
            /// <summary>
            /// SemesterToSemester
            /// </summary>
            semesterToSemester = 8,
            /// <summary>
            /// Year
            /// </summary>
            year = 9,
            /// <summary>
            /// YearToYear
            /// </summary>
            yearToYear = 10
        }
        /// <summary>
        /// Trimestre
        /// </summary>
        public enum Trimester {
            /// <summary>
            /// Premier trimestre
            /// </summary>
            first = 1,
            /// <summary>
            /// Deuxième trimestre
            /// </summary>
            second = 2,
            /// <summary>
            /// Troisième trimestre
            /// </summary>
            third = 3,
            /// <summary>
            /// Quatrième trimestre
            /// </summary>
            fourth = 4
        }
        /// <summary>
        /// Semestre
        /// </summary>
        public enum Semester {
            /// <summary>
            /// Premier semestre
            /// </summary>
            first = 1,
            /// <summary>
            /// Deuxième semestre
            /// </summary>
            second = 2
        }
        #endregion

        #region variables
        /// <summary>
        /// Année du début de calendrier
        /// </summary>
        protected int _startYear;
        /// <summary>
        /// Année du fin de calendrier
        /// </summary>
        protected int _stopYear;
        /// <summary>
        /// Date de début
        /// </summary>
        protected int _selectedStartDate;
        /// <summary>
        /// Date de fin
        /// </summary>
        protected int _selectedEndDate;
        /// <summary>
        /// Type de date sélectionnée (dateTodate, dateTodateMonth ...)
        /// </summary>
        protected CustomerSessions.Period.Type _selectedDateType;
        /// <summary>
        /// Indique si le calendrier contient des périodes en grisées
        /// </summary>
        protected bool _isRestricted = false;
        /// <summary>
        /// Année sélectionnée
        /// </summary>
        protected int _selectedYear = DateTime.Now.Year;
        /// <summary>
        /// Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        protected DateTime _firstDayNotEnable;
        /// <summary>
        /// Langue utilisé
        /// </summary>
        protected int _language = 33;
        /// <summary>
        /// Slide Menu
        /// </summary>
        protected OboutInc.SlideMenu.SlideMenu osm;
        /// <summary>
        /// Titre du label de la période sélectionnée
        /// </summary>
        protected string _periodSelectionTitle = "";
        /// <summary>
        /// Informe le client que la période en grisée correspond à une période qui ne contient pas des données chargées
        /// </summary>
        protected string _periodRestrictedLabel = "";
        /// <summary>
        /// Css file path
        /// </summary>
        protected string _cssPath = "";
        /// <summary>
        /// Calendar theme name
        /// </summary>
        protected string _themeName = "";
		/// <summary>
		/// Precise if calendar contained parution dates
		/// </summary>
		protected bool _withParutionDates = false;
		/// <summary>
		/// Parution date list dictionary
		/// <remarks>
		/// - key =parution date
		/// - value = magazine cover's url
		/// </remarks>
		/// </summary>
		protected Dictionary<string,string> _parutionDateList = null;
		/// <summary>
		/// ID div contanining cover
		/// </summary>
		protected string _idDivCover = "";
		/// <summary>
		/// ID visual cover
		/// </summary>
		protected string _idVisualCover = "";
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient et définit l'année à afficher
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("2007")]
        public int SelectedYear {
            get { return _selectedYear; }
            set { _selectedYear = value; }
        }
        /// <summary>
        /// Set / Get L'année du début de calendrier
        /// </summary>
        public int StartYear {
            get { return _startYear; }
            set { _startYear = value; }
        }
        /// <summary>
        /// Set / Get L'année de fin du calendrier
        /// </summary>
        public int StopYear {
            get { return _stopYear; }
            set { _stopYear = value; }
        }
        /// <summary>
        /// Obtient la date de début
        /// </summary>
        public int SelectedStartDate {
            get { return (_selectedStartDate); }
        }
        /// <summary>
        /// Obtient la date de fin
        /// </summary>
        public int SelectedEndDate {
            get { return (_selectedEndDate); }
        }
        /// <summary>
        /// Obtient le type de période sélectionné
        /// </summary>
        public CustomerSessions.Period.Type SelectedDateType {
            get { return (_selectedDateType); }
        }
        /// <summary>
        /// Set / Get la langue utilisée
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("33")]
        public int Language {
            get { return _language; }
            set { _language = value; }
        }
        /// <summary>
        /// Set / Get le titre du label de la période sélectionnée
        /// </summary>
        public string PeriodSelectionTitle {
            get { return _periodSelectionTitle; }
            set { _periodSelectionTitle = value; }
        }
        /// <summary>
        /// Set / Get le label informant que la période en grisée correspond à une période qui ne contient pas des données chargées
        /// </summary>
        public string PeriodRestrictedLabel {
            get { return _periodRestrictedLabel; }
            set { _periodRestrictedLabel = value; }
        }
        /// <summary>
        /// Set / Get si le calendrier contient des périodes en grisées
        /// </summary>
        public bool IsRestricted {
            get { return _isRestricted; }
            set { _isRestricted = value; }
        }
        /// <summary>
        /// Set / Get Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        public DateTime FirstDayNotEnable {
            get { return _firstDayNotEnable; }
            set { _firstDayNotEnable = value; }
        }
        /// <summary>
        /// Set / Get Css file path
        /// </summary>
        public string CssPath {
            get { return _cssPath; }
            set { _cssPath = value; }
        }
        /// <summary>
        /// Set / Get Calendar theme name
        /// </summary>
        public string ThemeName {
            get { return _themeName; }
            set { _themeName = value; }
        }
		/// <summary>
		/// Get / Set if calendar manages parution dates
		/// </summary>
		public bool WithParutionDates {
			get { return _withParutionDates; }
			set { _withParutionDates = value; }
		}
		/// <summary>
		/// Get / Set parution date list
		/// </summary>
		public Dictionary<string,string> ParutionDateList {
			get { return _parutionDateList; }
			set { _parutionDateList = value; }
		}
		/// <summary>
		/// Get / Set ID div cover
		/// </summary>
		public string IdDivCover {
			get { return _idDivCover; }
			set { _idDivCover = value; }
		}
		/// <summary>
		/// Get / Set ID visual cover
		/// </summary>
		public string IdVisualCover {
			get { return _idVisualCover; }
			set { _idVisualCover = value; }
		}
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public GlobalCalendarWebControl() {
            _stopYear = DateTime.Now.Year;
        }
        #endregion

        #region Scripts

        #region CalendarScript
        /// <summary>
        /// Génération du javascript pour la gestion du calendrier
        /// </summary>
        /// <param name="output">Html text writer</param>
        protected void CalendarScript(HtmlTextWriter output) {

            StringBuilder js = new StringBuilder();

            js.Append("\r\n<script language=javascript>\r\n");

            #region Css links
            //js.Append(CssLinks());
            #endregion

            #region Initialisation
            js.Append(Initialisation());
            #endregion

            #region Variables
            js.Append(GetVariables());
            #endregion

            #region SelectedDate
            js.Append(SelectedDate());
            #endregion

            #region Init selectedItemIndex
            js.Append(InitSelectedItemIndex());
            #endregion

            #region PostBack
            js.Append(PostBack());
            #endregion

            #region memoryDispose
            js.Append(memoryDispose());
            #endregion

            #region Init
            js.Append(Init());
            #endregion

            #region InitAll
            js.Append(InitAll());
            #endregion

            #region CompareDate
            js.Append(compareDate());
            #endregion

            #region CompareLastDate
            js.Append(CompareLastDate());
            #endregion

            #region GetDateLabel
            js.Append(GetDateLabel());
            #endregion

            #region SetPeriodPrint
            js.Append(SetPeriodPrint());
            #endregion

            #region SetPeriodPrintSingle
            js.Append(SetPeriodPrintSingle());
            #endregion

            #region PeriodPrintYearToYear
            js.Append(PeriodPrintYearToYear());
            #endregion

            #region PeriodPrintSemesterToSemester
            js.Append(PeriodPrintSemesterToSemester());
            #endregion

            #region PeriodPrintTrimesterToTrimester
            js.Append(PeriodPrintTrimesterToTrimester());
            #endregion

            #region PeriodPrintMonthToMonth
            js.Append(PeriodPrintMonthToMonth());
            #endregion

            #region PeriodPrintDayToDay
            js.Append(PeriodPrintDayToDay());
            #endregion

            #region PeriodPrintYear
            if(IsRestricted)
                js.Append(PeriodRestrictedPrintYear());
            else
                js.Append(PeriodPrintYear());
            #endregion

            #region PeriodPrintSemester
            if(IsRestricted)
                js.Append(PeriodRestrictedPrintSemester());
            else
                js.Append(PeriodPrintSemester());
            #endregion

            #region PeriodPrintTrimester
            if(IsRestricted)
                js.Append(PeriodRestrictedPrintTrimester());
            else
                js.Append(PeriodPrintTrimester());
            #endregion

            #region PeriodPrintMonth
            if(IsRestricted)
                js.Append(PeriodRestrictedPrintMonth());
            else
                js.Append(PeriodPrintMonth());
            #endregion

            #region PeriodPrintDay
            if(IsRestricted)
                js.Append(PeriodRestrictedPrintDay());
            else
                js.Append(PeriodPrintDay());
            #endregion

            js.Append("\r\n</script>\r\n");

            output.Write(js.ToString());

        }
        #endregion

        #region Functions

        #region Css links
        ///// <summary>
        ///// Css links
        ///// </summary>
        ///// <returns></returns>
        //protected virtual string CssLinks() { 
        //    StringBuilder js = new StringBuilder();
            
        //    js.Append("\r\n\t var GECKO=(navigator.product==(\"Gecko\"));");
			
        //    js.Append("\r\n\t if(GECKO){");
        //    js.Append("\r\n\t\t document.write('<link href=\"/Css/" + _themeName + "/CalendarGecko.css\" rel=\"stylesheet\" type=\"text/css\">');");
        //    js.Append("\r\n}\r\n");

        //    js.Append("\r\n\t else{");
        //    js.Append("\r\n\t\t document.write('<link href=\"/Css/" + _themeName + "/Calendar.css\" rel=\"stylesheet\" type=\"text/css\">');");
        //    js.Append("\r\n\t }");
        //    js.Append("\n\n");

        //    return (js.ToString());
        //}
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string Initialisation() {
            StringBuilder js = new StringBuilder();

            js.AppendFormat("\r\n\t var monthLabelList_{0} = [\"" + GestionWeb.GetWebWord(945, _language) + "\", \"" + GestionWeb.GetWebWord(946, _language) + "\", \"" + GestionWeb.GetWebWord(947, _language) + "\", \"" + GestionWeb.GetWebWord(948, _language) + "\", \"" + GestionWeb.GetWebWord(949, _language) + "\", \"" + GestionWeb.GetWebWord(950, _language) + "\", \"" + GestionWeb.GetWebWord(951, _language) + "\", \"" + GestionWeb.GetWebWord(952, _language) + "\", \"" + GestionWeb.GetWebWord(953, _language) + "\", \"" + GestionWeb.GetWebWord(954, _language) + "\", \"" + GestionWeb.GetWebWord(955, _language) + "\", \"" + GestionWeb.GetWebWord(956, _language) + "\"];", this.ID);
            js.AppendFormat("\r\n\t var trimesterLabelList_{0} = [\"" + GestionWeb.GetWebWord(2303, _language) + "\",\"" + GestionWeb.GetWebWord(2304, _language) + "\",\"" + GestionWeb.GetWebWord(2305, _language) + "\",\"" + GestionWeb.GetWebWord(2306, _language) + "\"];", this.ID);
            js.AppendFormat("\r\n\t var semesterLabelList_{0} =  [\"" + GestionWeb.GetWebWord(2307, _language) + "\",\"" + GestionWeb.GetWebWord(2308, _language) + "\"];", this.ID);
            js.Append("\r\n\n\t /// Chargement des images");
            js.AppendFormat("\r\n\t var dayImageList_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=31;i++){");
            js.AppendFormat("\r\n\t dayImageList_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t dayImageList_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + 's.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var dayImageListI_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=31;i++){");
            js.AppendFormat("\r\n\t dayImageListI_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t dayImageListI_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + '.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var dayImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=31;i++){");
            js.AppendFormat("\r\n\t dayImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t dayImageListNC_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");			
			//Init parutions dates' day images
			if (_withParutionDates) {
				js.AppendFormat("\r\n\t var dayImageListP_{0} = new Array();", this.ID);
				js.Append("\r\n\t for(i=1;i<=31;i++){");
				js.AppendFormat("\r\n\t dayImageListP_{0}[i] = new Image();", this.ID);
				js.AppendFormat("\r\n\t dayImageListP_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + 'p.gif';", this.ID);//TODO replace r by p (for parutions) when parution's images would be created
				js.Append("\r\n\t }");
			}
            js.AppendFormat("\r\n\t var monthImageList_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=12;i++){");
            js.AppendFormat("\r\n\t monthImageList_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t monthImageList_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_' + i + 's.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var monthImageListI_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=12;i++){");
            js.AppendFormat("\r\n\t monthImageListI_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t monthImageListI_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_' + i + '.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var monthImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=12;i++){");
            js.AppendFormat("\r\n\t monthImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t monthImageListNC_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var trimesterImageList_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=4;i++){");
            js.AppendFormat("\r\n\t trimesterImageList_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t trimesterImageList_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_' + i + 's.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var trimesterImageListI_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=4;i++){");
            js.AppendFormat("\r\n\t trimesterImageListI_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t trimesterImageListI_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_' + i + '.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var trimesterImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=4;i++){");
            js.AppendFormat("\r\n\t trimesterImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t trimesterImageListNC_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var semesterImageList_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=2;i++){");
            js.AppendFormat("\r\n\t semesterImageList_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t semesterImageList_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_' + i + 's.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var semesterImageListI_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=2;i++){");
            js.AppendFormat("\r\n\t semesterImageListI_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t semesterImageListI_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_' + i + '.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var semesterImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=2;i++){");
            js.AppendFormat("\r\n\t semesterImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t semesterImageListNC_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var yearImageList_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=" + _startYear + ";i<=" + _stopYear + ";i++){");
            js.AppendFormat("\r\n\t yearImageList_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t yearImageList_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + 's.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var yearImageListI_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=" + _startYear + ";i<=" + _stopYear + ";i++){");
            js.AppendFormat("\r\n\t yearImageListI_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t yearImageListI_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + '.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var yearImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=" + _startYear + ";i<=" + _stopYear + ";i++){");
            js.AppendFormat("\r\n\t yearImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t yearImageListNC_{0}[i].src = '/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");			

			if (_withParutionDates) {
				js.AppendFormat("\r\n\t var parutionDateList_{0} = null;", this.ID);
				if (_parutionDateList != null && _parutionDateList.Count > 0) {
					js.AppendFormat("\r\n\t parutionDateList_{0} = new Array({1});", this.ID, _parutionDateList.Count);					
					foreach (KeyValuePair<string,string> kpv in _parutionDateList) {
						js.AppendFormat("\r\n\t parutionDateList_{0}['{1}'] = {2};", this.ID, kpv.Key, kpv.Key);
					}										
				}
			}
            return (js.ToString());
        }
        #endregion

        #region Variables
        /// <summary>
        /// Variables
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string GetVariables() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n\t /// Variables");
            // Date de début
            js.AppendFormat("\r\n\t var dateBegin_{0}='';", this.ID);
            // Date de fin 
            js.AppendFormat("\r\n\t var dateEnd_{0}='';", this.ID);
            // Date de debut complete
            js.AppendFormat("\r\n\t var longDateBegin_{0}='';", this.ID);
            // Date de fin complete (current)
            js.AppendFormat("\r\n\t var longDateEndCur_{0}='';", this.ID);
            // Date de fin complete (prec)
            js.AppendFormat("\r\n\t var longDateEndPrec_{0}='';", this.ID);
            // L'année de début
            js.AppendFormat("\r\n\t var selectedYear_{0}='';", this.ID);
            // La période sélectionnée (Day, Month, Trimester ....)
            js.AppendFormat("\r\n\t var selectedPeriodType_{0}='';", this.ID);
            // Le type de la date sélectionnée (DayToDay, Month, MonthToMonth ....)
            js.AppendFormat("\r\n\t var selectedDateType_{0}='';", this.ID);
            // L'avant derniere date de fin sélectionnée
            js.AppendFormat("\r\n\t var dateEndPrec_{0} = '';", this.ID);
            // L'année courante
            js.AppendFormat("\r\n\t var yearCur_{0} = '';", this.ID);
            // L'avant derniere année sélectionnée
            js.AppendFormat("\r\n\t var yearPrec_{0} = '';", this.ID);
            // Enum
            js.Append("\r\n\t var  semester = {");
            js.Append("\r\n\t\t semester1 : 1,");
            js.Append("\r\n\t\t semester2 : 2");
            js.Append("\r\n\t };");
            js.Append("\r\n\t var  trimester = {");
            js.Append("\r\n\t\t trimester1 : 1,");
            js.Append("\r\n\t\t trimester2 : 2,");
            js.Append("\r\n\t\t trimester3 : 3,");
            js.Append("\r\n\t\t trimester4 : 4");
            js.Append("\r\n\t };");
            js.Append("\r\n\t var  dateType = {");
            js.Append("\r\n\t\t day : 1,");
            js.Append("\r\n\t\t dayToDay : 2,");
            js.Append("\r\n\t\t month : 3,");
            js.Append("\r\n\t\t monthToMonth : 4,");
            js.Append("\r\n\t\t trimester : 5,");
            js.Append("\r\n\t\t trimesterToTrimester : 6,");
            js.Append("\r\n\t\t semester : 7,");
            js.Append("\r\n\t\t semesterToSemester : 8,");
            js.Append("\r\n\t\t year : 9,");
            js.Append("\r\n\t\t yearToYear : 10");
            js.Append("\r\n\t };");
            if(IsRestricted)
                js.AppendFormat("\r\n\t var firstDayNotEnable_{0}='" + _firstDayNotEnable.Year.ToString() + _firstDayNotEnable.Month.ToString("00") + _firstDayNotEnable.Day.ToString("00") + "';", this.ID);

            return (js.ToString());
        }
        #endregion

        #region SelectedDate
        /// <summary>
        /// Date sélectionnée
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string SelectedDate() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function SelectedDate(date, year, periodType, imageId, replaceImagePathIndex) {");

            js.Append("\r\n\t var periodPrinted=0;");
            js.Append("\r\n\t var initValue = 0, initAllValue=0;");
            js.Append("\r\n\t var compare = 0, compareLast = 0;");
            js.AppendFormat("\r\n\t var oLabel_{0} = document.getElementById('Label1');", this.ID);

            js.AppendFormat("\r\n\n\t if(selectedPeriodType_{0}.length!='' && periodType!=selectedPeriodType_{0})", this.ID);
            js.Append("\r\n\t\t alert('" + GestionWeb.GetWebWord(1854, _language) + "');");
            js.Append("\r\n\t else{");

            js.Append("\r\n\n\t\t if(dateBegin_" + this.ID + ".length==0){");
            js.Append("\r\n\t\t\t Init();");
            js.AppendFormat("\r\n\t\t\t dateBegin_{0} = date;", this.ID);
            js.AppendFormat("\r\n\t\t\t longDateBegin_{0} = year + '' + date;", this.ID);
            js.AppendFormat("\r\n\t\t\t longDateEndCur_{0} = year + '' + date;", this.ID);
            js.Append("\r\n\t\t\t initValue = 0; initAllValue=0;");
            js.Append("\r\n\t\t\t SetPeriodPrintSingle(date, year, periodType, initValue, initAllValue);");
            js.AppendFormat("\r\n\t\t\t oLabel_{0}.innerHTML =  '<font style=\"font-family: Arial, Helvetica, sans-serif;font-size: 11px;font-weight: bold; white-space:nowrap;\">' + '&nbsp;' + GetDateLabel(date,year,periodType) + '</font>'; ", this.ID);
            js.AppendFormat("\r\n\t\t\t selectedPeriodType_{0} = periodType;", this.ID);
            js.AppendFormat("\r\n\t\t\t selectedYear_{0} = year;", this.ID);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else {");
            js.AppendFormat("\r\n\t\t\t longDateEndPrec_{0} = longDateEndCur_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t longDateEndCur_{0} = year + '' + date;", this.ID);
            js.Append("\r\n\t\t\t if(longDateEndPrec_" + this.ID + "!=longDateEndCur_" + this.ID + "){");

            js.AppendFormat("\r\n\t\t\t\t dateEndPrec_{0} = dateEnd_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t\t dateEnd_{0} = date;", this.ID);
            js.AppendFormat("\r\n\t\t\t\t yearPrec_{0} = yearCur_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t\t yearCur_{0} = year;", this.ID);

            js.Append("\r\n\t\t\t\t compare = CompareDate(dateBegin_" + this.ID + ",dateEnd_" + this.ID + ",yearCur_" + this.ID + ",periodType);");
            js.Append("\r\n\t\t\t\t if(compare>=0){");
            js.Append("\r\n\t\t\t\t\t\t if(dateEnd_" + this.ID + ".length>0 && dateEndPrec_" + this.ID + ".length>0){");
            js.Append("\r\n\t\t\t\t\t\t\t compareLast = CompareLastDate(dateEndPrec_" + this.ID + ",dateEnd_" + this.ID + ",yearCur_" + this.ID + ",periodType);");
            js.Append("\r\n\t\t\t\t\t\t\t if(compareLast<0){");
            js.Append("\r\n\t\t\t\t\t\t\t\t initValue = 1; initAllValue=0;");
            js.AppendFormat("\r\n\t\t\t\t\t\t\t\t SetPeriodPrint(dateEnd_{0}, dateEndPrec_{0}, yearCur_{0}, yearPrec_{0}, periodType, initValue, initAllValue, compare);", this.ID);
            js.Append("\r\n\t\t\t\t\t\t\t\t periodPrinted = 1;");
            js.Append("\r\n\t\t\t\t\t\t\t }");
            js.Append("\r\n\t\t\t\t\t\t }");
            js.Append("\r\n\t\t\t\t\t\t compareLast = CompareLastDate(dateEndPrec_" + this.ID + ",dateEnd_" + this.ID + ",yearCur_" + this.ID + ",periodType);");
            js.Append("\r\n\t\t\t\t\t\t if(dateEndPrec_" + this.ID + ".length>0 && compareLast>=0){");
            js.Append("\r\n\t\t\t\t\t\t\t initValue = 0; initAllValue=0;");
            js.Append("\r\n\t\t\t\t\t\t\t SetPeriodPrint(dateEndPrec_" + this.ID + ", dateEnd_" + this.ID + ", yearPrec_" + this.ID + ", yearCur_" + this.ID + ", periodType, initValue, initAllValue, compare);");
            js.Append("\r\n\t\t\t\t\t\t }");
            js.Append("\r\n\t\t\t\t\t\t else if(periodPrinted==0){");
            js.Append("\r\n\t\t\t\t\t\t\t initValue = 0; initAllValue=0;");
            js.Append("\r\n\t\t\t\t\t\t\t SetPeriodPrint(dateBegin_" + this.ID + ", dateEnd_" + this.ID + ", selectedYear_" + this.ID + ", yearCur_" + this.ID + ", periodType, initValue, initAllValue, compare);");
            js.Append("\r\n\t\t\t\t\t\t }");
            js.AppendFormat("\r\n\t\t\t\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t\t\t\t oLabel_{0}.innerHTML = '<font style=\"font-family: Arial, Helvetica, sans-serif;font-size: 11px;font-weight: bold; white-space:nowrap;\">' + '&nbsp;' + GetDateLabel(date,yearCur_{0},periodType) + '</font>'; ", this.ID);
            js.AppendFormat("\r\n\t\t\t\t\t\t else");
            js.AppendFormat("\r\n\t\t\t\t\t\t oLabel_{0}.innerHTML = '<font style=\"font-family: Arial, Helvetica, sans-serif;font-size: 11px;font-weight: bold; white-space:nowrap;\">' + '&nbsp;' + GetDateLabel(dateBegin_{0},selectedYear_{0},periodType) + '</font>'+'&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'+'<font style=\"font-family: Arial, Helvetica, sans-serif;font-size: 11px;font-weight: bold; white-space:nowrap;\">' + GetDateLabel(date,yearCur_{0},periodType) + '&nbsp;</font>'; ", this.ID);
            js.Append("\r\n \t\t\t\t\t\t periodPrinted=0;");
            js.Append("\r\n \t\t\t\t}");
            js.Append("\r\n\t\t\t\t else {");
            js.AppendFormat("\r\n\t\t\t\t\t dateEnd_{0} = dateEndPrec_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t\t\t yearCur_{0} = yearPrec_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t\t\t longDateEndCur_{0} = longDateEndPrec_{0};", this.ID);
            js.Append("\r\n\t\t\t\t\t alert('" + GestionWeb.GetWebWord(46, _language) + "');");
            js.Append("\r\n\t\t\t\t }");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t }");

            js.Append("\r\n\t }");
            js.Append("\r\n\t initSelectedItemIndex();");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region Init selectedItemIndex
        /// <summary>
        /// Initialisation de l'élément sélectionné
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string InitSelectedItemIndex() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n  function initSelectedItemIndex(){");
            js.Append("\r\n\t var strStart, strEnd;");
            js.Append("\r\n\t switch(selectedPeriodType_" + this.ID + "){");
            js.Append("\r\n\t\t case 'Day':");
            js.Append("\r\n\t\t case 'Year':");
            js.AppendFormat("\r\n\t\t\t document.forms[0].dateSelectedItem.value = ''+selectedDateType_{0}+','+dateBegin_{0}+','+dateEnd_{0}+'';", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.Append("\r\n\t\t case 'Trimester':");
            js.Append("\r\n\t\t case 'Semester':");
            js.AppendFormat("\r\n\t\t\t strStart = dateBegin_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t strEnd = dateEnd_{0};", this.ID);
            js.Append("\r\n\t\t\t if(strStart.length==1) strStart = '0' + strStart;");
            js.Append("\r\n\t\t\t if(strEnd.length==1) strEnd = '0' + strEnd;");
            js.AppendFormat("\r\n\t\t\t document.forms[0].dateSelectedItem.value = ''+selectedDateType_{0}+','+selectedYear_{0}+strStart+','+yearCur_{0}+strEnd+'';", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");
            js.Append("\r\n\t }");

            return (js.ToString());
        }
        #endregion

        #region PostBack
        /// <summary>
        /// PostBack
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PostBack() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n  function postBack(){");
            js.Append("\r\n\t var strStart, strEnd;");
            js.Append("\r\n\t switch(selectedPeriodType_" + this.ID + "){");
            js.Append("\r\n\t\t case 'Day':");
            js.Append("\r\n\t\t case 'Year':");
            js.Append("\r\n\t\t\t memoryDispose();");
            js.AppendFormat("\r\n\t\t\t __doPostBack('" + this.ID + "',''+selectedDateType_{0}+','+dateBegin_{0}+','+dateEnd_{0}+'');", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.Append("\r\n\t\t case 'Trimester':");
            js.Append("\r\n\t\t case 'Semester':");
            js.Append("\r\n\t\t\t memoryDispose();");
            js.AppendFormat("\r\n\t\t\t strStart = dateBegin_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t strEnd = dateEnd_{0};", this.ID);
            js.Append("\r\n\t\t\t if(strStart.length==1) strStart = '0' + strStart;");
            js.Append("\r\n\t\t\t if(strEnd.length==1) strEnd = '0' + strEnd;");
            js.AppendFormat("\r\n\t\t\t __doPostBack('" + this.ID + "',''+selectedDateType_{0}+','+selectedYear_{0}+strStart+','+yearCur_{0}+strEnd+'');", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t default:");
            js.Append("\r\n\t\t\t alert('" + GestionWeb.GetWebWord(885, _language) + "');");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");
            js.Append("\r\n\t }");

            return (js.ToString());
        }
        #endregion

        #region memoryDispose
        /// <summary>
        /// Liberation de la mémoire utilisée
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string memoryDispose() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n  function memoryDispose(){");

            DayCalendar dayCalendar;
            string currentYearMonth;
            int length;
            for (int yearIndex = _startYear; yearIndex <= _stopYear; yearIndex++) {
                for (int month = 1; month <= 12; month++) {
                    currentYearMonth = MonthString.GetYYYYMM(yearIndex.ToString() + month.ToString());

                    dayCalendar = new DayCalendar(int.Parse(currentYearMonth));
                    length = 0;
                    for (int i = 0; i < dayCalendar.DaysTable.GetLength(0); i++) {
                        for (int j = 0; j < dayCalendar.DaysTable.GetLength(1); j++) {
                            if (dayCalendar.DaysTable[i, j] != 0) {
                                js.Append("\r\n\t elementsYear[" + yearIndex + "][" + currentYearMonth + "]['" + currentYearMonth + dayCalendar.DaysTable[i, j].ToString("00") + "']=null;");
                                length = dayCalendar.DaysTable[i, j];
                            }
                        }
                    }
                    js.Append("\r\n\t elementsYear[" + yearIndex + "][" + currentYearMonth + "]['" + currentYearMonth + "00']=null;");


                    js.Append("\r\n\t elementsYear[" + yearIndex + "][" + currentYearMonth + "]=null;");
                }
                js.Append("\r\n\t elementsYear[" + yearIndex + "]=null;");

                js.Append("\r\n\t elementsMonth[" + (yearIndex - _startYear) + "]=null;");
            }

            js.Append("\r\n\t for(i=0;i<12;i++){");
            js.Append("\r\n\t\t elementsDay[i]=null;");
            js.Append("\r\n\t }");
            js.Append("\r\n\t for(i=1;i<=31;i++){");
            js.AppendFormat("\r\n\t dayImageList_{0}[i]=null;", this.ID);
            js.AppendFormat("\r\n\t dayImageListI_{0}[i]=null;", this.ID);
			//Empty parution dates table
			if(_withParutionDates)
			js.AppendFormat("\r\n\t dayImageListP_{0}[i]=null;", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t for(i=1;i<=12;i++){");
            js.AppendFormat("\r\n\t monthImageList_{0}[i]=null;", this.ID);
            js.AppendFormat("\r\n\t monthImageListI_{0}[i]=null;", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t for(i=1;i<=4;i++){");
            js.AppendFormat("\r\n\t trimesterImageList_{0}[i]=null;", this.ID);
            js.AppendFormat("\r\n\t trimesterImageListI_{0}[i]=null;", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t for(i=1;i<=2;i++){");
            js.AppendFormat("\r\n\t semesterImageList_{0}[i]=null;", this.ID);
            js.AppendFormat("\r\n\t semesterImageListI_{0}[i]=null;", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t for(i=" + _startYear + ";i<=" + _stopYear + ";i++){");
            js.AppendFormat("\r\n\t yearImageList_{0}[i]=null;", this.ID);
            js.AppendFormat("\r\n\t yearImageListI_{0}[i]=null;", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t monthLabelList_{0} = null;", this.ID);
            js.AppendFormat("\r\n\t trimesterLabelList_{0} = null;", this.ID);
            js.AppendFormat("\r\n\t semesterLabelList_{0} =  null;", this.ID);

            //js.Append("\r\n\t document.location.reload();");

            js.Append("\r\n\t }");

            return (js.ToString());
        }
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string Init() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function Init(){");

            js.AppendFormat("\r\n\t var oLabel_{0} = document.getElementById('Label1');", this.ID);
            js.AppendFormat("\r\n\t dateBegin_{0}='';", this.ID);
            js.AppendFormat("\r\n\t dateEnd_{0}='';", this.ID);
            js.AppendFormat("\r\n\t selectedPeriodType_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t selectedYear_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t oLabel_{0}.innerHTML = '';", this.ID);

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region InitAll
        /// <summary>
        /// Init All
        /// </summary>
        protected virtual string InitAll(){
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function InitAll(){");

            js.AppendFormat("\r\n\t var oLabel_{0} = document.getElementById('Label1');", this.ID);
            js.AppendFormat("\r\n\t if(dateEnd_{0}.length==0)", this.ID);
            js.AppendFormat("\r\n\t\t SetPeriodPrint(dateBegin_{0}, dateBegin_{0}, selectedYear_{0}, selectedYear_{0}, selectedPeriodType_{0}, 1, 1, 0);", this.ID);
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t SetPeriodPrint(dateBegin_{0}, dateEnd_{0}, selectedYear_{0}, yearCur_{0}, selectedPeriodType_{0}, 1, 1, 0);", this.ID);
            js.AppendFormat("\r\n\t\t dateBegin_{0}='';", this.ID);
            js.AppendFormat("\r\n\t\t dateEnd_{0}='';", this.ID);
            js.AppendFormat("\r\n\t\t selectedPeriodType_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t selectedYear_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t dateEndPrec_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t dateEnd_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t yearPrec_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t yearCur_{0} = '';", this.ID);
            js.AppendFormat("\r\n\t\t oLabel_{0}.innerHTML = '';", this.ID);
            js.AppendFormat("\r\n\t\t document.forms[0].dateSelectedItem.value = '';");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region CompareDate
        /// <summary>
        /// Comparaison de dates
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string compareDate() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function CompareDate(dateBegin, dateEnd, year, periodType){");

            js.Append("\r\n\t switch(periodType){");
            js.Append("\r\n\t\t case 'Day':");
            js.Append("\r\n\t\t case 'Year':");
            js.Append("\r\n\t\t\t if(dateBegin < dateEnd)");
            js.Append("\r\n\t\t\t\t return 1;");
            js.Append("\r\n\t\t\t else if(dateBegin == dateEnd)");
            js.Append("\r\n\t\t\t\t return 0;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t return -1;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.Append("\r\n\t\t case 'Trimester':");
            js.Append("\r\n\t\t case 'Semester':");
            js.Append("\r\n\t\t\t if(dateBegin.length==1)dateBegin='0'+dateBegin;");
            js.Append("\r\n\t\t\t if(dateEnd.length==1)dateEnd='0'+dateEnd;");
            js.AppendFormat("\r\n\t\t\t if(selectedYear_{0}+''+dateBegin < year+''+dateEnd)", this.ID);
            js.Append("\r\n\t\t\t\t return 1;");
            js.AppendFormat("\r\n\t\t\t else if(selectedYear_{0}+''+dateBegin == year+''+dateEnd)", this.ID);
            js.Append("\r\n\t\t\t\t return 0;");
            js.AppendFormat("\r\n\t\t\t else if(selectedYear_{0} == year && parseFloat(dateBegin) < parseFloat(dateEnd))", this.ID);
            js.Append("\r\n\t\t\t\t return 1;");
            js.AppendFormat("\r\n\t\t\t else if(selectedYear_{0} == year && parseFloat(dateBegin) == parseFloat(dateEnd))", this.ID);
            js.Append("\r\n\t\t\t\t return 0;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t return -1;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region CompareLastDate
        /// <summary>
        /// comparaison de la dernière date
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string CompareLastDate() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function CompareLastDate(dateBegin, dateEnd, year, periodType){");

            js.Append("\r\n\t switch(periodType){");
            js.Append("\r\n\t\t case 'Day':");
            js.Append("\r\n\t\t case 'Year':");
            js.Append("\r\n\t\t\t if(dateBegin < dateEnd)");
            js.Append("\r\n\t\t\t\t return 1;");
            js.Append("\r\n\t\t\t else if(dateBegin == dateEnd)");
            js.Append("\r\n\t\t\t\t return 0;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t return -1;");
            js.Append("\r\n\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.Append("\r\n\t\t case 'Trimester':");
            js.Append("\r\n\t\t case 'Semester':");
            js.Append("\r\n\t\t\t if(dateBegin.length==1)dateBegin='0'+dateBegin;");
            js.Append("\r\n\t\t\t if(dateEnd.length==1)dateEnd='0'+dateEnd;");
            js.AppendFormat("\r\n\t\t\t if(yearPrec_{0}+''+dateBegin < year+''+dateEnd)", this.ID);
            js.Append("\r\n\t\t\t\t return 1;");
            js.AppendFormat("\r\n\t\t\t else if(yearPrec_{0}+''+dateBegin == year+''+dateEnd)", this.ID);
            js.Append("\r\n\t\t\t\t return 0;");
            js.AppendFormat("\r\n\t\t\t else if(yearPrec_{0} == year && parseFloat(dateBegin) < parseFloat(dateEnd))", this.ID);
            js.Append("\r\n\t\t\t\t return 1;");
            js.AppendFormat("\r\n\t\t\t else if(yearPrec_{0} == year && parseFloat(dateBegin) == parseFloat(dateEnd))", this.ID);
            js.Append("\r\n\t\t\t\t return 0;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t return -1;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n }");
            
            return (js.ToString());
        }
        #endregion

        #region GetDateLabel
        /// <summary>
        /// renvoie le label à afficher
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string GetDateLabel() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function GetDateLabel(date, year, periodType){");

            js.Append("\r\n\t switch(periodType){");
            js.Append("\r\n\t\t case 'Day':");
            
            switch (_language) {
                case 44:
                    js.Append("\r\n\t\t\t return date.substr(4,2) + '/' + date.substr(6,2) + '/' + date.substr(0,4);");
                    break;
                default:
                    js.Append("\r\n\t\t\t return date.substr(6,2) + '/' + date.substr(4,2) + '/' + date.substr(0,4);");
                    break;
            }
                
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.AppendFormat("\r\n\t\t\t return monthLabelList_{0}[date-1]+ ' ' + year;", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Trimester':");
            js.AppendFormat("\r\n\t\t\t return trimesterLabelList_{0}[date-1]+ ' ' + year;", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Semester':");
            js.AppendFormat("\r\n\t\t\t return semesterLabelList_{0}[date-1]+ ' ' + year;", this.ID);
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Year':");
            js.Append("\r\n\t\t\t return date;");
            js.Append("\r\n\t }");

            js.Append("}");

            return (js.ToString());
        }
        #endregion

        #region SetPeriodPrint
        /// <summary>
        /// Coloriage du calendrier en fonction d'une période
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string SetPeriodPrint() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function SetPeriodPrint(dateBegin, dateEnd, yearBegin, yearEnd, periodType, init, initAll, compare){");

            js.Append("\r\n\t switch(periodType){");
            js.Append("\r\n\t\t case 'Day':");
            js.Append("\r\n\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.day;", this.ID);
            js.Append("\r\n\t\t\t else");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.dayToDay;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintDayToDay(dateBegin, dateEnd, yearEnd, init, periodType, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.Append("\r\n\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.month;", this.ID);
            js.Append("\r\n\t\t\t else");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.monthToMonth;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintMonthToMonth(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Trimester':");
            js.Append("\r\n\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.trimester;", this.ID);
            js.Append("\r\n\t\t\t else");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.trimesterToTrimester;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintTrimesterToTrimester(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Semester':");
            js.Append("\r\n\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.semester;", this.ID);
            js.Append("\r\n\t\t\t else");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.semesterToSemester;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintSemesterToSemester(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Year':");
            js.Append("\r\n\t\t\t if(compare==0)");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.year;", this.ID);
            js.Append("\r\n\t\t\t else");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.yearToYear;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintYearToYear(yearBegin, yearEnd, init, periodType, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("}");

            return (js.ToString());
        }
        #endregion

        #region SetPeriodPrintSingle
        /// <summary>
        /// Coloriage du calendrier pour un jour, un mois, un trimestre, un semestre ou une année
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string SetPeriodPrintSingle() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function SetPeriodPrintSingle(date, year, periodType, init, initAll){");

            js.Append("\r\n\t switch(periodType){");
            js.Append("\r\n\t\t case 'Day':");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.day;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintDay(date, date, year, init, initAll);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Month':");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.month;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintMonth(year, date, init,periodType);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Trimester':");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.trimester;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintTrimester(year, date, init,periodType);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Semester':");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.semester;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintSemester(year, date, init, periodType);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case 'Year':");
            js.AppendFormat("\r\n\t\t\t selectedDateType_{0} = dateType.year;", this.ID);
            js.Append("\r\n\t\t\t PeriodPrintYear(year, init, periodType);");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintYearToYear
        /// <summary>
        /// Coloriage d'une période allant d'une année à une autre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintYearToYear() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintYearToYear(yearBegin, yearEnd, init, periodType, initAll){");

            js.Append("\r\n\t var yearBeginIndex = 0;");
            js.Append("\r\n\t if(initAll==1) yearBeginIndex = yearBegin;");
            js.Append("\r\n\t else yearBeginIndex = yearBegin+1;");

            js.Append("\r\n\t for(yearIndex=yearBeginIndex;yearIndex<=yearEnd;yearIndex++){");
            js.Append("\r\n\t\t PeriodPrintYear(yearIndex, init,periodType);");
            js.Append("\r\n\t }");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintSemesterToSemester
        /// <summary>
        /// Coloriage d'une période allant d'un semestre à un autre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintSemesterToSemester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintSemesterToSemester(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll){");

            js.Append("\r\n\t for(yearIndex=yearBegin;yearIndex<=yearEnd;yearIndex++){");
            js.Append("\r\n\t\t if(yearBegin==yearEnd){");
            js.Append("\r\n\t\t\t if(initAll==1) PeriodPrintSemester(yearIndex, dateBegin, init, periodType);");
            js.Append("\r\n\t\t\t PeriodPrintSemester(yearIndex, dateEnd, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearBegin){");
            js.Append("\r\n\t\t\t if(initAll==1) {");
            js.Append("\r\n\t\t\t\t PeriodPrintSemester(yearIndex, 1, init, periodType);");
            js.Append("\r\n\t\t\t\t PeriodPrintSemester(yearIndex, 2, init, periodType);");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t\t if(parseFloat(dateBegin)==1)");
            js.Append("\r\n\t\t\t\t PeriodPrintSemester(yearIndex, 2, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearEnd){");
            js.Append("\r\n\t\t\t PeriodPrintSemester(yearIndex, 1, init, periodType);");
            js.Append("\r\n\t\t\t if(parseFloat(dateEnd)==2)");
            js.Append("\r\n\t\t\t\t PeriodPrintSemester(yearIndex, 2, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else{");
            js.Append("\r\n\t\t\t PeriodPrintSemester(yearIndex, 1, init, periodType);");
            js.Append("\r\n\t\t\t PeriodPrintSemester(yearIndex, 2, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t }");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintTrimesterToTrimester
        /// <summary>
        /// Coloriage d'une période allant d'un trimestre à un autre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintTrimesterToTrimester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintTrimesterToTrimester(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll){");
            js.Append("\r\n\t var i=0;");
            js.Append("\r\n\t var beginIndex=0;");
            js.Append("\r\n\t for(yearIndex=yearBegin;yearIndex<=yearEnd;yearIndex++){");
            js.Append("\r\n\t\t if(yearBegin==yearEnd){");
            js.Append("\r\n\t\t\t if(initAll==1) beginIndex = parseFloat(dateBegin);");
            js.Append("\r\n\t\t\t else beginIndex = parseFloat(dateBegin)+1;");
            js.Append("\r\n\t\t\t for(i=beginIndex;i<=parseFloat(dateEnd);i++)");
            js.Append("\r\n\t\t\t\t PeriodPrintTrimester(yearIndex, i, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearBegin){");
            //js.Append("\r\n\t\t\t if(parseFloat(dateBegin)!=4){");
            js.Append("\r\n\t\t\t\t if(initAll==1) beginIndex = parseFloat(dateBegin);");
            js.Append("\r\n\t\t\t\t else beginIndex = parseFloat(dateBegin)+1;");
            js.Append("\r\n\t\t\t\t for(i=beginIndex;i<=4;i++)");
            js.Append("\r\n\t\t\t\t\t PeriodPrintTrimester(yearIndex, i, init, periodType);");
            //js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearEnd){");
            js.Append("\r\n\t\t\t for(i=1;i<=parseFloat(dateEnd);i++)");
            js.Append("\r\n\t\t\t\t PeriodPrintTrimester(yearIndex, i, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else{");
            js.Append("\r\n\t\t\t for(i=1;i<=4;i++)");
            js.Append("\r\n\t\t\t\t PeriodPrintTrimester(yearIndex, i, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t }");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintMonthToMonth
        /// <summary>
        /// Coloriage d'une période allant d'un mois à un autre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintMonthToMonth() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintMonthToMonth(dateBegin, dateEnd, yearBegin, yearEnd, init, periodType, initAll){");

            js.Append("\r\n\t var monthIndexBegin, monthIndexEnd;");

            js.Append("\r\n\t for(yearIndex=yearBegin;yearIndex<=yearEnd;yearIndex++){");
            js.Append("\r\n\t\t if(yearBegin==yearEnd){");
            js.Append("\r\n\t\t\t if(initAll==1) monthIndexBegin=parseFloat(dateBegin);");
            js.Append("\r\n\t\t\t else monthIndexBegin=parseFloat(dateBegin)+1;");
            js.Append("\r\n\t\t\t monthIndexEnd=parseFloat(dateEnd);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearBegin){");
            js.Append("\r\n\t\t\t if(initAll==1) monthIndexBegin=parseFloat(dateBegin);");
            js.Append("\r\n\t\t\t else monthIndexBegin=parseFloat(dateBegin)+1;");
            js.Append("\r\n\t\t\t monthIndexEnd=12;");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else if(yearIndex==yearEnd){");
            js.Append("\r\n\t\t\t monthIndexBegin=1;");
            js.Append("\r\n\t\t\t monthIndexEnd=parseFloat(dateEnd);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else{");
            js.Append("\r\n\t\t\t monthIndexBegin=1;");
            js.Append("\r\n\t\t\t monthIndexEnd=12;");
            js.Append("\r\n\t\t }");

            js.Append("\r\n\t\t for(month=monthIndexBegin;month<=monthIndexEnd;month++)");
            js.Append("\r\n\t\t PeriodPrintMonth(yearIndex, month, init, periodType);");
            js.Append("\r\n\t }");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintDayToDay
        /// <summary>
        /// Coloriage d'une période allant d'un jour à un autre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintDayToDay() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintDayToDay(dateBegin, dateEnd, year, init, periodType, initAll){");
			
            js.Append("\r\n\t var dayBegin, dayEnd, monthBegin, monthEnd, yearBegin, yearEnd, dayStr, monthStr;");
            js.Append("\r\n\t var monthYearBegin, monthYearEnd, monthIndexBegin, monthIndexEnd;");

            js.Append("\r\n\t dayBegin = parseFloat(dateBegin.substr(6,2));");
            js.Append("\r\n\t dayEnd = parseFloat(dateEnd.substr(6,2));");
            js.Append("\r\n\t monthBegin = parseFloat(dateBegin.substr(4,2));");
            js.Append("\r\n\t monthEnd = parseFloat(dateEnd.substr(4,2));");
            js.Append("\r\n\t yearBegin = parseFloat(dateBegin.substr(0,4));");
            js.Append("\r\n\t yearEnd = parseFloat(dateEnd.substr(0,4));");

            js.Append("\r\n\t monthYearBegin = dateBegin.substr(0,6);");
            js.Append("\r\n\t monthYearEnd = dateEnd.substr(0,6);");

            js.Append("\r\n\t if(monthYearBegin==monthYearEnd)");
            js.Append("\r\n\t\t PeriodPrintDay(dateBegin, dateEnd, year, init, initAll);");
            js.Append("\r\n\t else if(monthYearBegin<monthYearEnd){");
            js.Append("\r\n\t\t PeriodPrintDay(dateBegin, monthYearBegin+elementsYear[dateBegin.substr(0,4)][monthYearBegin][monthYearBegin+'00'], dateBegin.substr(0,4), init, initAll);");
            js.Append("\r\n\t\t for(yearIndex=yearBegin;yearIndex<=year;yearIndex++){");
            js.Append("\r\n\t\t\t if(year==yearBegin){");
            js.Append("\r\n\t\t\t\t monthIndexBegin=monthBegin+1;");
            js.Append("\r\n\t\t\t\t monthIndexEnd=monthEnd;");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t\t else if(yearIndex==yearBegin){");
            js.Append("\r\n\t\t\t\t monthIndexBegin=monthBegin+1;");
            js.Append("\r\n\t\t\t\t monthIndexEnd=13;");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t\t else if(yearIndex==yearEnd){");
            js.Append("\r\n\t\t\t\t monthIndexBegin=1;");
            js.Append("\r\n\t\t\t\t monthIndexEnd=monthEnd;");
            js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t\t\t else{");
            js.Append("\r\n\t\t\t\t monthIndexBegin=1;");
            js.Append("\r\n\t\t\t\t monthIndexEnd=13;");
            js.Append("\r\n\t\t\t }");

            js.Append("\r\n\t\t\t for(month=monthIndexBegin;month<monthIndexEnd;month++)");
            js.Append("\r\n\t\t\t\t PeriodPrintMonth(yearIndex, month, init, periodType);");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t PeriodPrintDay(monthYearEnd+'01', dateEnd, year, init, 1);");
            js.Append("\r\n\t }");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintYear
        /// <summary>
        /// coloriage d'une année
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintYear() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintYear(year, init, periodType){");

            js.Append("\r\n\t var dayStr, monthStr;");
            js.Append("\r\n\t var yImageList = new Array();");

            js.Append("\r\n\t if(init==1)");
            js.AppendFormat("\r\n\t\t yImageList = yearImageListI_{0};", this.ID);
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t yImageList = yearImageList_{0};", this.ID);

            js.Append("\r\n\t elementsPeriod['year_'+year].src=yImageList[parseFloat(year)].src;");

            js.Append("\r\n\t for(semesterIndex=1;semesterIndex<=2;semesterIndex++)");
            js.Append("\r\n\t\t PeriodPrintSemester(year, semesterIndex, init, periodType);");

            js.Append("\r\n\t yImageList = null;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodRestrictedPrintYear
        /// <summary>
        /// coloriage d'une année
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodRestrictedPrintYear() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintYear(year, init, periodType){");

            js.Append("\r\n\t var dayStr, monthStr;");
            js.Append("\r\n\t var yImageList = new Array();");
            js.Append("\r\n\t var enableSemester = 1;");
            js.Append("\r\n\t var enableYear = 1;");

            js.Append("\r\n\t for(semesterIndex=1;semesterIndex<=2;semesterIndex++){");
            js.Append("\r\n\t\t enableSemester = PeriodPrintSemester(year, semesterIndex, init, periodType);");
            js.Append("\r\n\t\t if(enableSemester==0)");
            js.Append("\r\n\t\t\t enableYear=0;");
            js.Append("\r\n\t\t }");

            js.Append("\r\n\t if(init==1)");
            js.Append("\r\n\t\t if(enableYear==0)");
            js.AppendFormat("\r\n\t\t\t yImageList = yearImageListNC_{0};", this.ID);
            js.Append("\r\n\t\t else");
            js.AppendFormat("\r\n\t\t yImageList = yearImageListI_{0};", this.ID);
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t yImageList = yearImageList_{0};", this.ID);

            js.Append("\r\n\t elementsPeriod['year_'+year].src=yImageList[parseFloat(year)].src;");

            js.Append("\r\n\t yImageList = null;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintSemester
        /// <summary>
        /// Coloriage d'un semestre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintSemester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintSemester(year, date, init, periodType){");
            js.Append("\r\n\t var dayStr, monthStr, trimesterBeginIndex, trimesterEndIndex;");
            js.Append("\r\n\t var sImageList = new Array();");

            js.Append("\r\n\t if(init==1){");
            js.AppendFormat("\r\n\t\t sImageList = semesterImageListI_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t sImageList = semesterImageList_{0};", this.ID);
            js.Append("\r\n\t }");

            js.Append("\r\n\t switch(parseFloat(date)){");
            js.Append("\r\n\t\t case semester.semester1:");
            js.Append("\r\n\t\t\t trimesterBeginIndex = 1;");
            js.Append("\r\n\t\t\t trimesterEndIndex = 2;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case semester.semester2:");
            js.Append("\r\n\t\t\t trimesterBeginIndex = 3;");
            js.Append("\r\n\t\t\t trimesterEndIndex = 4;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n\t elementsPeriod['semester_'+year+''+date].src=sImageList[parseFloat(date)].src;");

            js.Append("\r\n\t for(trimesterIndex=trimesterBeginIndex;trimesterIndex<=trimesterEndIndex;trimesterIndex++){");
            js.Append("\r\n\t\t PeriodPrintTrimester(year, ''+trimesterIndex+'', init, periodType);");
            js.Append("\r\n\t }");
            js.Append("\r\n\t sImageList = null;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodRestrictedPrintSemester
        /// <summary>
        /// Coloriage d'un semestre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodRestrictedPrintSemester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintSemester(year, date, init, periodType){");
            js.Append("\r\n\t var dayStr, monthStr, trimesterBeginIndex, trimesterEndIndex;");
            js.Append("\r\n\t var sImageList = new Array();");
            js.Append("\r\n\t var enableTrimester = 1;");
            js.Append("\r\n\t var enableSemester = 1;");

            js.Append("\r\n\t switch(parseFloat(date)){");
            js.Append("\r\n\t\t case semester.semester1:");
            js.Append("\r\n\t\t\t trimesterBeginIndex = 1;");
            js.Append("\r\n\t\t\t trimesterEndIndex = 2;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case semester.semester2:");
            js.Append("\r\n\t\t\t trimesterBeginIndex = 3;");
            js.Append("\r\n\t\t\t trimesterEndIndex = 4;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n\t for(trimesterIndex=trimesterBeginIndex;trimesterIndex<=trimesterEndIndex;trimesterIndex++){");
            js.Append("\r\n\t\t enableTrimester = PeriodPrintTrimester(year, ''+trimesterIndex+'', init, periodType);");
            js.Append("\r\n\t\t if(enableTrimester==0)");
            js.Append("\r\n\t\t\t enableSemester=0;");
            js.Append("\r\n\t }");

            js.Append("\r\n\t if(init==1){");
            js.Append("\r\n\t\t if(enableSemester==0)");
            js.AppendFormat("\r\n\t\t\t sImageList = semesterImageListNC_{0};", this.ID);
            js.Append("\r\n\t\t else");
            js.AppendFormat("\r\n\t\t\t sImageList = semesterImageListI_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t sImageList = semesterImageList_{0};", this.ID);
            js.Append("\r\n\t }");

            js.Append("\r\n\t elementsPeriod['semester_'+year+''+date].src=sImageList[parseFloat(date)].src;");

            js.Append("\r\n\t sImageList = null;");
            js.Append("\r\n\t return enableSemester;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintTrimester
        /// <summary>
        /// Coriage d'un trimestre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintTrimester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintTrimester(year, date, init, periodType){");
            js.Append("\r\n\t var dayStr, monthStr, monthBeginIndex, monthEndIndex;");
            js.Append("\r\n\t var tImageList = new Array();");

            js.Append("\r\n\t if(init==1){");
            js.AppendFormat("\r\n\t\t tImageList = trimesterImageListI_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t tImageList = trimesterImageList_{0};", this.ID);
            js.Append("\r\n\t }");

            js.Append("\r\n\t switch(parseFloat(date)){");
            js.Append("\r\n\t\t case trimester.trimester1:");
            js.Append("\r\n\t\t\t monthBeginIndex = 1;");
            js.Append("\r\n\t\t\t monthEndIndex = 3;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester2:");
            js.Append("\r\n\t\t\t monthBeginIndex = 4;");
            js.Append("\r\n\t\t\t monthEndIndex = 6;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester3:");
            js.Append("\r\n\t\t\t monthBeginIndex = 7;");
            js.Append("\r\n\t\t\t monthEndIndex = 9;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester4:");
            js.Append("\r\n\t\t\t monthBeginIndex = 10;");
            js.Append("\r\n\t\t\t monthEndIndex = 12;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n\t elementsPeriod['trimester_'+year+''+date].src=tImageList[parseFloat(date)].src;");

            js.Append("\r\n\t for(month=monthBeginIndex;month<=monthEndIndex;month++){");
            js.Append("\r\n\t\t PeriodPrintMonth(year, month, init, periodType);");
            js.Append("\r\n\t }");
            js.Append("\r\n\t tImageList = null;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodRestrictedPrintTrimester
        /// <summary>
        /// Coriage d'un trimestre
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodRestrictedPrintTrimester() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintTrimester(year, date, init, periodType){");
            js.Append("\r\n\t var dayStr, monthStr, monthBeginIndex, monthEndIndex;");
            js.Append("\r\n\t var tImageList = new Array();");
            js.Append("\r\n\t var enableMonth = 1;");
            js.Append("\r\n\t var enableTrimester = 1;");

            js.Append("\r\n\t switch(parseFloat(date)){");
            js.Append("\r\n\t\t case trimester.trimester1:");
            js.Append("\r\n\t\t\t monthBeginIndex = 1;");
            js.Append("\r\n\t\t\t monthEndIndex = 3;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester2:");
            js.Append("\r\n\t\t\t monthBeginIndex = 4;");
            js.Append("\r\n\t\t\t monthEndIndex = 6;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester3:");
            js.Append("\r\n\t\t\t monthBeginIndex = 7;");
            js.Append("\r\n\t\t\t monthEndIndex = 9;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t\t case trimester.trimester4:");
            js.Append("\r\n\t\t\t monthBeginIndex = 10;");
            js.Append("\r\n\t\t\t monthEndIndex = 12;");
            js.Append("\r\n\t\t\t break;");
            js.Append("\r\n\t }");

            js.Append("\r\n\t for(month=monthBeginIndex;month<=monthEndIndex;month++){");
            js.Append("\r\n\t\t enableMonth = PeriodPrintMonth(year, month, init, periodType);");
            js.Append("\r\n\t\t if(enableMonth==0)");
            js.Append("\r\n\t\t\t enableTrimester=0");
            js.Append("\r\n\t }");

            js.Append("\r\n\t if(init==1){");
            js.Append("\r\n\t\t if(enableTrimester==0)");
            js.AppendFormat("\r\n\t\t\t tImageList = trimesterImageListNC_{0};", this.ID);
            js.Append("\r\n\t\t else");
            js.AppendFormat("\r\n\t\t\t tImageList = trimesterImageListI_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t tImageList = trimesterImageList_{0};", this.ID);
            js.Append("\r\n\t }");

            js.Append("\r\n\t elementsPeriod['trimester_'+year+''+date].src=tImageList[parseFloat(date)].src;");

            js.Append("\r\n\t tImageList = null;");
            js.Append("\r\n\t return enableTrimester;");
            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintMonth
        /// <summary>
        /// Coloriage d'un mois
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodPrintMonth() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintMonth(year, month, init, periodType){");

            js.Append("\r\n\t var dayStr='', monthStr='';");
            js.Append("\r\n\t var dImageList = new Array();");
			js.Append("\r\n\t var dImageListP = new Array();");
            js.Append("\r\n\t var mImageList = new Array();");

            js.Append("\r\n\t if(init==1){");
            js.AppendFormat("\r\n\t\t dImageList = dayImageListI_{0};", this.ID);
            js.AppendFormat("\r\n\t\t mImageList = monthImageListI_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t dImageList = dayImageList_{0};", this.ID);
            js.AppendFormat("\r\n\t\t mImageList = monthImageList_{0};", this.ID);
            js.Append("\r\n\t }");
			if (_withParutionDates) js.AppendFormat("\r\n\t dImageListP = dayImageListP_{0};", this.ID);
            js.Append("\r\n\t if(periodType!='Day')");
            js.Append("\r\n\t\t elementsPeriod['month_'+year+''+month].src=mImageList[parseFloat(month)].src;");
            js.Append("\r\n\t\t monthStr=month+'';");
            js.Append("\r\n\t\t if(monthStr.length==1)monthStr = '0'+month;");
            js.Append("\r\n\t\t for(i=1;i<=elementsYear[year][year+''+monthStr][year+''+monthStr+''+'00'];i++){");
            js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
			//Set parutions images 
			if (_withParutionDates) {
				js.AppendFormat("\r\n\t if(init==1 && && parutionDateList_{0} != null && parutionDateList_{0}[year+monthStr+dayStr] != null && typeof(parutionDateList_{0}[year+monthStr+dayStr]) != 'undefined')", this.ID);
				js.Append("\r\n\t\t\t {");
				js.Append("\r\n\t\t\t\t	elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = dImageListP[i].src;");
				js.Append("\r\n\t\t\t }");
				js.Append("\r\n\t\t\t else { ");
			}
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = dImageList[i].src;");
			if (_withParutionDates) js.Append("\r\n\t\t\t }");
            js.Append("\r\n\t }");
            js.Append("\r\n\t dImageList = null;");
			js.Append("\r\n\t dImageListP = null;");
            js.Append("\r\n\t mImageList = null;");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodRestrictedPrintMonth
        /// <summary>
        /// Coloriage d'un mois
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected virtual string PeriodRestrictedPrintMonth() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintMonth(year, month, init, periodType){");

            js.Append("\r\n\t var dayStr='', monthStr='';");
            js.Append("\r\n\t var dImageList = new Array();");
            js.Append("\r\n\t var dImageListNC = new Array();");
			js.Append("\r\n\t var dImageListP = new Array();");
            js.Append("\r\n\t var mImageList = new Array();");
            js.AppendFormat("\r\n\t var firstDayNotEnable = firstDayNotEnable_{0}.substr(0,6);", this.ID);
            js.Append("\r\n\t\t monthStr=month+'';");
            js.Append("\r\n\t\t if(monthStr.length==1)monthStr = '0'+month;");
            js.Append("\r\n\t var dateMonth = year+monthStr;");
            js.Append("\r\n\t var dateDay;");
            js.Append("\r\n\t var enable=1;");
			js.Append("\r\n\t var withParutionDates = false;");			

            js.Append("\r\n\t if(init==1){");
            js.Append("\r\n\t\t if(firstDayNotEnable<=dateMonth){");
            js.AppendFormat("\r\n\t\t\t dImageListNC = dayImageListNC_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t dImageList = dayImageListI_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t mImageList = monthImageListNC_{0};", this.ID);
            js.Append("\r\n\t\t\t enable = 0;");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t\t else{");
            js.AppendFormat("\r\n\t\t\t dImageList = dayImageListI_{0};", this.ID);
            js.AppendFormat("\r\n\t\t\t mImageList = monthImageListI_{0};", this.ID);
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t }");
            js.Append("\r\n\t else{");
            js.AppendFormat("\r\n\t\t dImageList = dayImageList_{0};", this.ID);
            js.AppendFormat("\r\n\t\t mImageList = monthImageList_{0};", this.ID);
            js.Append("\r\n\t }");
			if (_withParutionDates) js.AppendFormat("\r\n\t dImageListP = dayImageListP_{0};", this.ID);
            js.Append("\r\n\t if(periodType!='Day')");
            js.Append("\r\n\t\t elementsPeriod['month_'+year+''+month].src=mImageList[parseFloat(month)].src;");

            js.Append("\r\n\t\t for(i=1;i<=elementsYear[year][year+''+monthStr][year+''+monthStr+''+'00'];i++){");			
			js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
            js.Append("\r\n\t\t\t dateDay = year+monthStr+dayStr;");
			//Get parution images dates
			if (_withParutionDates) {
				js.AppendFormat("\r\n\t\t\t if(init==1 && parutionDateList_{0} != null && parutionDateList_{0}[''+dateDay+''] != null && typeof(parutionDateList_{0}[''+dateDay+'']) != 'undefined')", this.ID);
				js.Append("\r\n\t\t\t {");
				js.Append("\r\n\t\t\t\t	withParutionDates = true;");
				js.Append("\r\n\t\t\t }");
			}
            js.AppendFormat("\r\n\t\t\t if(firstDayNotEnable_{0}<=dateDay && init==1)", this.ID);
			js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = (withParutionDates) ? dImageListP[i].src : dImageListNC[i].src;");
            js.Append("\r\n\t\t\t else");
			js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = (withParutionDates) ? dImageListP[i].src : dImageList[i].src;");
			js.Append("\r\n\t\t\t withParutionDates = false;");
			js.Append("\r\n\t }");
            js.Append("\r\n\t dImageList = null;");
			js.Append("\r\n\t dImageListP = null;");
            js.Append("\r\n\t mImageList = null;");
            js.Append("\r\n\t return enable;");

            js.Append("\r\n }");

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintDay
        /// <summary>
        /// Coloriage d'un jour
        /// </summary>
        /// <returns>script de la fonction</returns>
        protected virtual string PeriodPrintDay() {
            StringBuilder js = new StringBuilder();
            
            js.Append("\r\n\n function PeriodPrintDay(dateBegin, dateEnd, year, init, initAll){");
			
            js.Append("\r\n\t var dayStr='', monthStr=dateBegin.substr(4,2);");
            js.Append("\r\n\t var dayBegin = parseFloat(dateBegin.substr(6,2));");
            js.Append("\r\n\t var dayEnd = parseFloat(dateEnd.substr(6,2));");
            js.Append("\r\n\t var imageList = new Array();");
			js.Append("\r\n\t var imageListP = new Array();");

            js.Append("\r\n\t if(init==1)");
            js.AppendFormat("\r\n\t\t imageList = dayImageListI_{0};", this.ID);
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t imageList = dayImageList_{0};", this.ID);
			if (_withParutionDates) js.AppendFormat("\r\n\t\t imageListP = dayImageListP_{0};", this.ID);

            js.Append("\r\n\t if(monthStr.length==1)monthStr = '0'+monthStr;");
            js.Append("\r\n\t if(init==1 && initAll==0) dayBegin++;");
            js.Append("\r\n\t\t for(i=dayBegin;i<=dayEnd;i++){");
            js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
			//Set parutions images 
			if (_withParutionDates) {
				js.AppendFormat("\r\n\t if(init==1 && parutionDateList_{0} != null && parutionDateList_{1}[year+monthStr+dayStr] != null && typeof(parutionDateList_{2}[year+monthStr+dayStr]) != 'undefined')", this.ID, this.ID, this.ID);
				js.Append("\r\n\t\t\t {");
				js.Append("\r\n\t\t\t\t	elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = imageListP[i].src;");
				js.Append("\r\n\t\t\t }");
				js.Append("\r\n\t\t\t else { ");
			}
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = imageList[i].src;");
			if (_withParutionDates) js.Append("\r\n\t\t\t }");
			js.Append("\r\n\t\t }");
            js.Append("\r\n\t imageList = null;");
			js.Append("\r\n\t imageListP = null;");

            js.Append("\r\n}"); 
            
            return (js.ToString());
        }
        #endregion

        #region PeriodRestrictedPrintDay
        /// <summary>
        /// Coloriage d'un jour
        /// </summary>
        /// <returns>script de la fonction</returns>
        protected virtual string PeriodRestrictedPrintDay() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintDay(dateBegin, dateEnd, year, init, initAll){");			

            js.Append("\r\n\t var dayStr='', monthStr=dateBegin.substr(4,2);");
            js.Append("\r\n\t var dayBegin = parseFloat(dateBegin.substr(6,2));");
            js.Append("\r\n\t var dayEnd = parseFloat(dateEnd.substr(6,2));");
            js.Append("\r\n\t var imageList = new Array();");
            js.Append("\r\n\t var imageListNC = new Array();");
			js.Append("\r\n\t var imageListP = new Array();");
            js.AppendFormat("\r\n\t var firstDayNotEnable = firstDayNotEnable_{0};", this.ID);
            js.Append("\r\n\t var dateDay;");
			js.Append("\r\n\t var withParutionDates = false;");			

            js.Append("\r\n\t if(init==1){");
            js.AppendFormat("\r\n\t\t imageList = dayImageListI_{0};", this.ID);
            js.AppendFormat("\r\n\t\t imageListNC = dayImageListNC_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t imageList = dayImageList_{0};", this.ID);
			if (_withParutionDates) js.AppendFormat("\r\n\t\t imageListP = dayImageListP_{0};", this.ID);

            js.Append("\r\n\t if(monthStr.length==1)monthStr = '0'+monthStr;");
            js.Append("\r\n\t if(init==1 && initAll==0) dayBegin++;");
            js.Append("\r\n\t\t for(i=dayBegin;i<=dayEnd;i++){");
            js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
            js.Append("\r\n\t\t\t dateDay = year+monthStr+dayStr;");

			//Get parution images dates
			if (_withParutionDates) {
				js.AppendFormat("\r\n\t\t\t if(init==1 && parutionDateList_{0} != null && parutionDateList_{1}[''+dateDay+''] != null && typeof(parutionDateList_{2}[''+dateDay+'']) != 'undefined')", this.ID, this.ID, this.ID);
				js.Append("\r\n\t\t\t {");
				js.Append("\r\n\t\t\t\t	withParutionDates = true;");
				js.Append("\r\n\t\t\t }");				
			}
            js.Append("\r\n\t\t\t if(firstDayNotEnable<=dateDay && init==1)");
			js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = (withParutionDates && init==1) ? imageListP[i].src : imageListNC[i].src;");
            js.Append("\r\n\t\t\t else");
			js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = (withParutionDates && init==1) ? imageListP[i].src : imageList[i].src;");
			js.Append("\r\n\t\t\t withParutionDates = false;");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t imageList = null;");
			js.Append("\r\n\t imageListP = null;");

            js.Append("\r\n}");

            return (js.ToString());
        }
        #endregion
		
		#endregion

		#region ImagesLoadScript
		/// <summary>
        /// Génération du javascript pour le chargement des images
        /// </summary>
        /// <param name="output">Html text writer</param>
        protected void ImagesLoadScript(HtmlTextWriter output) {

            StringBuilder js = new StringBuilder();
            DayCalendar dayCalendar;
            string currentYearMonth;
            int length;

            js.Append("\r\n<script language=javascript>\r\n");

            js.Append("\r\n\t var elementsYear = new Array(3); \r");
            js.Append("\r\n\t var elementsMonth = new Array(3); \r");
            js.Append("\r\n\t var elementsDay = new Array(12); \r");

            for (int yearIndex = _startYear; yearIndex <= _stopYear; yearIndex++) {

                js.Append("\r\n\t elementsMonth[" + (yearIndex - _startYear) + "] = new Array(12); \r");
                for (int month = 1; month <= 12; month++) {

                    currentYearMonth = MonthString.GetYYYYMM(yearIndex.ToString() + month.ToString());
                    dayCalendar = new DayCalendar(int.Parse(currentYearMonth));
                    length = 0;
                    js.Append("\r\n\t elementsDay[" + (month - 1) + "] = new Array(32); \r");
                    for (int i = 0; i < dayCalendar.DaysTable.GetLength(0); i++) {
                        for (int j = 0; j < dayCalendar.DaysTable.GetLength(1); j++) {
                            if (dayCalendar.DaysTable[i, j] != 0) {
                                js.Append("\r\n\t elementsDay[" + (month - 1) + "]['" + currentYearMonth + dayCalendar.DaysTable[i, j].ToString("00") + "'] = document.getElementById('day_" + currentYearMonth + dayCalendar.DaysTable[i, j].ToString("00") + "');");
                                length = dayCalendar.DaysTable[i, j];
                            }
                        }
                    }
                    js.Append("\r\n\t elementsDay[" + (month - 1) + "]['" + currentYearMonth + "00'] = " + length + ";");
                    js.Append("\r\n\t elementsMonth[" + (yearIndex - _startYear) + "]['" + currentYearMonth + "'] = elementsDay[" + (month - 1) + "]; \r");
                }

                js.Append("\r\n\t elementsYear['" + yearIndex + "'] = elementsMonth[" + (yearIndex - _startYear) + "]; \r");

            }
            int monthIndexB = 1, monthIndexE = 3;
            int trimesterIndexB, trimesterIndexE;
            js.Append("\r\n\n\t var elementsPeriod = new Array(); \r");
            for (int yearIndex = _startYear; yearIndex <= _stopYear; yearIndex++) {
                js.Append("\r\n\t elementsPeriod['year_" + yearIndex + "'] = document.getElementById('year_" + yearIndex + "');");
                trimesterIndexB = 1; trimesterIndexE = 2;
                monthIndexB = 1; monthIndexE = 3;
                for (int semester = 1; semester <= 2; semester++) {
                    js.Append("\r\t elementsPeriod['semester_" + yearIndex + semester + "'] = document.getElementById('semester_" + semester + "_" + yearIndex + "');");
                    for (int trimester = trimesterIndexB; trimester <= trimesterIndexE; trimester++) {
                        js.Append("\r\t elementsPeriod['trimester_" + yearIndex + trimester + "'] = document.getElementById('trimester_" + trimester + "_" + yearIndex + "');");
                        for (int month = monthIndexB; month <= monthIndexE; month++) {
                            currentYearMonth = MonthString.GetYYYYMM(yearIndex.ToString() + month.ToString());
                            js.Append("\r\t elementsPeriod['month_" + yearIndex + month + "'] = document.getElementById('month_" + currentYearMonth + "');");
                        }
                        monthIndexB += 3; monthIndexE += 3;
                    }
                    trimesterIndexB += 2; trimesterIndexE += 2;
                }
            }
            js.Append("\r\n</script>\r\n");

            output.Write(js.ToString());
        }
        #endregion

        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
		/// Init event
		/// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {			
            base.OnInit(e);
        }
        #endregion

        #region Chargement
        /// <summary>
        /// Evènement de chargement du composant
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnLoad(EventArgs e) {

            base.OnLoad(e);
            // Ouverture/fermeture des fenêtres pères
            if (!Page.ClientScript.IsClientScriptBlockRegistered("Old")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Old", "<script language=\"JavaScript\">var ancien='Parent_" + _selectedYear.ToString() + "Content'; var ouvert=true;</script>");
            }
            // Ouverture/fermeture des fenêtres pères
            if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideCalendar")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideCalendar", TNS.AdExpress.Web.Functions.Script.ShowHideCalendar());
            }


            if (Page.IsPostBack) {
                string valueInput = Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
                string eventTaregt = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                string dateSelectedItem = "";
                if (Page.Request.Form.GetValues("dateSelectedItem") != null) dateSelectedItem = Page.Request.Form.GetValues("dateSelectedItem")[0];
                string[] parameters = dateSelectedItem.Split(',');
                string type = string.Empty, dateStart = string.Empty, dateEnd = string.Empty;

                type = parameters[0];
                if (type.Length > 0 && !type.Equals("-1")) {
                    switch ((selectedDateT)Convert.ToInt32(type)) {
                        case selectedDateT.day:
                            dateStart = parameters[1];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = Convert.ToInt32(dateStart);
                            _selectedEndDate = Convert.ToInt32(dateStart);
                            break;
                        case selectedDateT.dayToDay:
                            dateStart = parameters[1];
                            dateEnd = parameters[2];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = Convert.ToInt32(dateStart);
                            _selectedEndDate = Convert.ToInt32(dateEnd);
                            break;
                        case selectedDateT.month:
                            dateStart = parameters[1];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(dateStart);
                            _selectedEndDate = GetLastDayOfMonth(dateStart);
                            break;
                        case selectedDateT.monthToMonth:
                            dateStart = parameters[1];
                            dateEnd = parameters[2];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(dateStart);
                            _selectedEndDate = GetLastDayOfMonth(dateEnd);
                            break;
                        case selectedDateT.trimester:
                            dateStart = parameters[1];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(GetFirstMonthOfTrimester(dateStart));
                            _selectedEndDate = GetLastDayOfMonth(GetLastMonthOfTrimester(dateStart));
                            break;
                        case selectedDateT.trimesterToTrimester:
                            dateStart = parameters[1];
                            dateEnd = parameters[2];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(GetFirstMonthOfTrimester(dateStart));
                            _selectedEndDate = GetLastDayOfMonth(GetLastMonthOfTrimester(dateEnd));
                            break;
                        case selectedDateT.semester:
                            dateStart = parameters[1];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(GetFirstMonthOfTrimester(GetFirstTrimesterOfSemester(dateStart)));
                            _selectedEndDate = GetLastDayOfMonth(GetLastMonthOfTrimester(GetLastTrimesterOfSemester(dateStart)));
                            break;
                        case selectedDateT.semesterToSemester:
                            dateStart = parameters[1];
                            dateEnd = parameters[2];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = GetFirstDayOfMonth(GetFirstMonthOfTrimester(GetFirstTrimesterOfSemester(dateStart)));
                            _selectedEndDate = GetLastDayOfMonth(GetLastMonthOfTrimester(GetLastTrimesterOfSemester(dateEnd)));
                            break;
                        case selectedDateT.year:
                            dateStart = parameters[1];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = (Convert.ToInt32(dateStart) * 10000) + 101;
                            _selectedEndDate = (Convert.ToInt32(dateStart) * 10000) + 1231;
                            break;
                        case selectedDateT.yearToYear:
                            dateStart = parameters[1];
                            dateEnd = parameters[2];
                            _selectedDateType = CustomerSessions.Period.Type.dateToDate;
                            _selectedStartDate = (Convert.ToInt32(dateStart) * 10000) + 101;
                            _selectedEndDate = (Convert.ToInt32(dateEnd) * 10000) + 1231;
                            break;
                    }
                }
            }
        }
        #endregion

        #region OnPrerender
        /// <summary>
        /// OnPreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {

            /*osm = new OboutInc.SlideMenu.SlideMenu();
            osm.ID = "GlobalCalendar";
            osm.Height = -1;
            osm.MinimumChildrenHeight = 180;
            base.Width = 800;
            osm.CSSPath = _cssPath;
            osm.CSSMenu = "SMMenu";
            osm.CSSParent = "SMParent";
            osm.CSSParentOver = "SMParentOver";
            osm.CSSParentSelected = "SMParentSelected";
            osm.CSSChild = "SMChild";
            osm.CSSChildOver = "SMChildOver";
            osm.CSSChildSelected = "SMChildSelected";
            osm.Speed = 100;

            for (int yearIndex = _startYear; yearIndex <= _stopYear; yearIndex++) {
                osm.AddParent("Parent_" + yearIndex.ToString(), "<table border=0 cellspacing=0 cellpadding=0 width=\"100%\"><tr><td class=\"SMTitle\">" + yearIndex.ToString() + "</td><td class=\"arrowBackGround\"></td></tr></table>");
                if(IsRestricted)
                    osm.AddChild(yearIndex.ToString(), GetRestrictedYearHTML(yearIndex));
                else
                    osm.AddChild(yearIndex.ToString(), GetYearHTML(yearIndex));
            }

            //Ouverture de l'année par défaut
            SetSelectedId();

            this.Controls.Add(osm);*/

        }
        #endregion

        #region RenderContents
        /// <summary>
        /// Render Control
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output) {
            base.Render(output);
            string display = "none";
            StringBuilder html = new StringBuilder(10000);
            html.Append("<table width=\"800\" cellspacing=0 cellpadding=0 class=\"SMMenu\">");
            html.Append("<tr>");
            html.Append("<td>");
            for (int yearIndex = _startYear; yearIndex <= _stopYear; yearIndex++) {
                if (yearIndex == _selectedYear) display = "";
                else if (display.Length == 0) display = "none";
                html.Append("<table class=\"SMParent\" border=0 cellspacing=0 cellpadding=0 width=\"100%\" style=\"cursor:pointer;\"><tr onClick=\"javascript:ShowHideCalendar('Parent_" + yearIndex.ToString() + "');\"><td style=\"font-size-adjust: 0.58;font-family: Arial;font:bold 12px arial;color: #644882;\">" + yearIndex.ToString() + "</td><td><IMG height=\"15\" align=\"right\" src=\"/App_Themes/"+_themeName+"/Images/Culture/GlobalCalendar/bt_arrow_down.gif\" width=\"15\"></td></tr></table>");
                html.Append("<div style=\"padding:0;margin:0; display :" + display + "; \" id=\"Parent_" + yearIndex.ToString() + "Content\">");
                if (IsRestricted)
                    html.Append(GetRestrictedYearHTML(yearIndex));
                else
                    html.Append(GetYearHTML(yearIndex));
                html.Append("</div>");
            }
            html.Append("");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            output.Write(html.ToString());
            output.Write(GetCalendarBottomHtml());
            ImagesLoadScript(output);
            CalendarScript(output);
        }
        #endregion

        #endregion

        #region Méthode interne

        #region GetCalendarBottomHtml
        /// <summary>
        /// Renvoie la partie contenant la zone pour l'affichage de la sélection, le boutton init et le boutton valider
        /// </summary>
        /// <returns>Le code HTML</returns>
        protected virtual string GetCalendarBottomHtml() {

            StringBuilder htmlBuilder = new StringBuilder(1000);

            htmlBuilder.Append("\r\n\t<table border=0 cellspacing=0 cellpadding=0 width=\"100%\">");
            if (_periodRestrictedLabel.Length > 0 && IsRestricted) {
                htmlBuilder.Append("\r\n\t<tr>");
                htmlBuilder.Append("\r\n\t\t<TD class=\"txtViolet11\" colSpan=\"2\" nowrap>");
                htmlBuilder.Append("\r\n\t " + _periodRestrictedLabel + "");
                htmlBuilder.Append("\r\n\t\t</td>");
                htmlBuilder.Append("\r\n\t</tr>");
            }
            if (_periodSelectionTitle.Length > 0) {
                htmlBuilder.Append("\r\n\t<tr>");
                htmlBuilder.Append("\r\n\t\t<td class=\"txtGris11Bold\" colspan=\"2\" style=\"HEIGHT: 28px\">");
                htmlBuilder.Append("\r\n\t " + _periodSelectionTitle + "");
                htmlBuilder.Append("\r\n\t\t</td>");
                htmlBuilder.Append("\r\n\t</tr>");
            }
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td style=\"height: 16px\">");
            htmlBuilder.Append("\r\n\t\t\t<label id=\"Label1\" class=\"SMLabel\"></label><label style=\"display:block;float:left;\">&nbsp;&nbsp;&nbsp;</label>");
            htmlBuilder.Append("\r\n\t\t\t<img id=\"ok\" src=\"/App_Themes/" + _themeName + "/Images/Common/button/initialize_up.gif\" onmouseover=\"ok.src='/App_Themes/" + _themeName + "/Images/Common/button/initialize_down.gif';\" onmouseout=\"ok.src='/App_Themes/" + _themeName + "/Images/Common/button/initialize_up.gif';\" onclick=\"InitAll();\" style=\"cursor:pointer;display:block;float:left;\"/>");
            htmlBuilder.Append("\r\n\t\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t<td>");
            htmlBuilder.Append("\r\n\t&nbsp;");
            htmlBuilder.Append("\r\n\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t</table>");
            htmlBuilder.Append("\r\n\t<table id=\"Table11\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t<td valign=\"top\" class=\"imageBackGround\" height=\"25\">");
            htmlBuilder.Append("\r\n\t<table id=\"Table12\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t<td width=\"135\"></td>");
            htmlBuilder.Append("\r\n\t<td><img id=\"valider\" src=\"/App_Themes/" + _themeName + "/Images/Culture/button/valider_up.gif\" onmouseover=\"valider.src='/App_Themes/" + _themeName + "/Images/Culture/button/valider_down.gif';\" onmouseout=\"valider.src='/App_Themes/" + _themeName + "/Images/Culture/button/valider_up.gif';\" onclick=\"PostBack('" + this.ID + "');\" style=\"cursor:pointer\"/></td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t</table>");
            htmlBuilder.Append("\r\n\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t</table>");

            return(htmlBuilder.ToString());

        }
        #endregion

        #region GetYearHTML
        /// <summary>
        /// Génération d’un calendrier sélectionnable par année, semestre, trimestre, mois, jour pour l’année passée en paramètre
        /// </summary>
        /// <param name="year">L'année du calendrier</param>
        /// <returns>Le code HTML</returns>
        protected virtual string GetYearHTML(int year) {

            string currentYearMonth;
            StringBuilder htmlBuilder = new StringBuilder(1000);
            htmlBuilder.Append("\r\n<table class=\"SMChild\" border=0 cellspacing=0 cellpadding=0 width=\"100%\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td colspan=12 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + year + "'," + year + ",'Year','year_" + year + "','" + year + "')\"><img id=\"year_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + year.ToString() + ".gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");

            #region Semestre 1
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Semester','semester_1_" + year + "','1')\"><img id=\"semester_1_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_1.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");

            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Trimester','trimester_1_" + year + "','1')\"><img id=\"trimester_1_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_1.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Trimester','trimester_2_" + year + "','2')\"><img id=\"trimester_2_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_2.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");

            htmlBuilder.Append("\r\n\t<tr>");
            for (int month = 1; month <= 6; month++) {

                currentYearMonth = MonthString.GetYYYYMM(year.ToString() + month.ToString());

                htmlBuilder.Append("\r\n\t<td " + (month == 3 ? "class=\"SMMonth\"" : "") + ((month != 3) ? "class=\"SMTdPadding\"" : "") + ">");
                htmlBuilder.Append("\r\n\t\t<table border=0 cellspacing=0 cellpadding=0 style=\"padding-top:4px;\">");
                htmlBuilder.Append("\r\n\t\t\t<tr>");
                htmlBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_" + month + ".gif\"></a></td>");
                htmlBuilder.Append("\r\n\t\t\t</tr>");
                htmlBuilder.Append("\r\n\t\t\t<tr>");
                htmlBuilder.Append("\r\n\t\t\t\t<td colspan=12 align=\"center\">");
                htmlBuilder.Append(GetDays(int.Parse(currentYearMonth)));
                htmlBuilder.Append("\r\n\t\t\t\t</td>");
                htmlBuilder.Append("\r\n\t\t\t</tr>");
                htmlBuilder.Append("\r\n\t\t</table>");
                htmlBuilder.Append("\r\n\t</td>");

            }
            htmlBuilder.Append("\r\n\t</tr>");
            #endregion

            #region Semestre 2
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Semester','semester_2_" + year + "','2')\"><img id=\"semester_2_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_2.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('3'," + year + ",'Trimester','trimester_3_" + year + "','3')\"><img id=\"trimester_3_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_3.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('4'," + year + ",'Trimester','trimester_4_" + year + "','4')\"><img id=\"trimester_4_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_4.gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");

            for (int month = 7; month <= 12; month++) {

                currentYearMonth = MonthString.GetYYYYMM(year.ToString() + month.ToString());

                htmlBuilder.Append("\r\n\t<td " + (month == 9 ? "class=\"SMMonth\"" : "") + ((month != 9) ? "class=\"SMTdPadding\"" : "") + ">");
                htmlBuilder.Append("\r\n\t\t<table border=0 cellspacing=0 cellpadding=0 style=\"padding-top:4px;\">");
                htmlBuilder.Append("\r\n\t\t\t<tr>");
                htmlBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_" + month + ".gif\"></a></td>");
                htmlBuilder.Append("\r\n\t\t\t</tr>");
                htmlBuilder.Append("\r\n\t\t\t<tr>");
                htmlBuilder.Append("\r\n\t\t\t\t<td colspan=12 align=\"center\">");
                htmlBuilder.Append(GetDays(int.Parse(currentYearMonth)));
                htmlBuilder.Append("\r\n\t\t\t\t</td>");
                htmlBuilder.Append("\r\n\t\t\t</tr>");
                htmlBuilder.Append("\r\n\t\t</table>");
                htmlBuilder.Append("\r\n\t</td>");

            }
            htmlBuilder.Append("\r\n\t</tr>");
            #endregion

            htmlBuilder.Append("\r\n</table>");

            return (htmlBuilder.ToString());

        }
        #endregion

        #region GetRestrictedYearHTML
        /// <summary>
        /// Génération d’un calendrier sélectionnable par année, semestre, trimestre, mois, jour pour l’année passée en paramètre
        /// </summary>
        /// <param name="year">L'année du calendrier</param>
        /// <returns>Le code HTML</returns>
        protected virtual string GetRestrictedYearHTML(int year) {

            #region Variables
            string currentYearMonth;
            StringBuilder htmlBuilder = new StringBuilder(1000);
            StringBuilder htmlSemesterBuilder = new StringBuilder(1000);
            StringBuilder htmlMonthBuilder = new StringBuilder(1000);
            StringBuilder htmlDayBuilder = new StringBuilder(1000);
            bool isSemester1LinkEnable = true;
            bool isSemester2LinkEnable = true;
            bool isTrimester1LinkEnable = true;
            bool isTrimester2LinkEnable = true;
            bool isMonthLinkEnable = true;
            #endregion

            #region Semestre 1
            htmlMonthBuilder.Append("\r\n\t<tr>");
            for (int month = 1; month <= 6; month++) {

                currentYearMonth = MonthString.GetYYYYMM(year.ToString() + month.ToString());

                htmlDayBuilder.Append(GetDays(int.Parse(currentYearMonth), ref isMonthLinkEnable));

                htmlMonthBuilder.Append("\r\n\t<td " + (month == 3 ? "class=\"SMMonth\"" : "") + ((month != 3) ? "class=\"SMTdPadding\"" : "") + ">");
                htmlMonthBuilder.Append("\r\n\t\t<table border=0 cellspacing=0 cellpadding=0 style=\"padding-top:4px;\">");
                htmlMonthBuilder.Append("\r\n\t\t\t<tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_" + month + (isMonthLinkEnable ? "" : "NC") + ".gif\"></a></td>");
                htmlMonthBuilder.Append("\r\n\t\t\t</tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t<tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td colspan=12 align=\"center\">");
                htmlMonthBuilder.Append(htmlDayBuilder.ToString());
                htmlMonthBuilder.Append("\r\n\t\t\t\t</td>");
                htmlMonthBuilder.Append("\r\n\t\t\t</tr>");
                htmlMonthBuilder.Append("\r\n\t\t</table>");
                htmlMonthBuilder.Append("\r\n\t</td>");

                if (month <= 3) {
                    if (!isMonthLinkEnable) {
                        isSemester1LinkEnable = false;
                        isTrimester1LinkEnable = false;
                        isMonthLinkEnable = true;
                    }
                }
                else {
                    if (!isMonthLinkEnable) {
                        isSemester1LinkEnable = false;
                        isTrimester2LinkEnable = false;
                        isMonthLinkEnable = true;
                    }
                }
                htmlDayBuilder.Length = 0;

            }
            htmlMonthBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Semester','semester_1_" + year + "','1')\"><img id=\"semester_1_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_1" + (isSemester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Trimester','trimester_1_" + year + "','1')\"><img id=\"trimester_1_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_1" + (isTrimester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Trimester','trimester_2_" + year + "','2')\"><img id=\"trimester_2_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_2" + (isTrimester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append(htmlMonthBuilder.ToString());
            #endregion

            #region Initialisation
            htmlMonthBuilder.Length = 0;
            htmlDayBuilder.Length = 0;
            isTrimester1LinkEnable = true;
            isTrimester2LinkEnable = true;
            #endregion

            #region Semestre 2
            htmlMonthBuilder.Append("\r\n\t<tr>");

            for (int month = 7; month <= 12; month++) {

                currentYearMonth = MonthString.GetYYYYMM(year.ToString() + month.ToString());

                htmlDayBuilder.Append(GetDays(int.Parse(currentYearMonth), ref isMonthLinkEnable));

                htmlMonthBuilder.Append("\r\n\t<td " + (month == 9 ? "class=\"SMMonth\"" : "") + ((month != 9) ? "class=\"SMTdPadding\"" : "") + ">");
                htmlMonthBuilder.Append("\r\n\t\t<table border=0 cellspacing=0 cellpadding=0 style=\"padding-top:4px;\">");
                htmlMonthBuilder.Append("\r\n\t\t\t<tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Month_" + month + (isMonthLinkEnable ? "" : "NC") + ".gif\"></a></td>");
                htmlMonthBuilder.Append("\r\n\t\t\t</tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t<tr>");
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td colspan=12 align=\"center\">");
                htmlMonthBuilder.Append(htmlDayBuilder.ToString());
                htmlMonthBuilder.Append("\r\n\t\t\t\t</td>");
                htmlMonthBuilder.Append("\r\n\t\t\t</tr>");
                htmlMonthBuilder.Append("\r\n\t\t</table>");
                htmlMonthBuilder.Append("\r\n\t</td>");

                if (month <= 9) {
                    if (!isMonthLinkEnable) {
                        isSemester2LinkEnable = false;
                        isTrimester1LinkEnable = false;
                        isMonthLinkEnable = true;
                    }
                }
                else {
                    if (!isMonthLinkEnable) {
                        isSemester2LinkEnable = false;
                        isTrimester2LinkEnable = false;
                        isMonthLinkEnable = true;
                    }
                }

                htmlDayBuilder.Length = 0;
            }
            htmlMonthBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Semester','semester_2_" + year + "','2')\"><img id=\"semester_2_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Sem_2" + (isSemester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('3'," + year + ",'Trimester','trimester_3_" + year + "','3')\"><img id=\"trimester_3_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_3" + (isTrimester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('4'," + year + ",'Trimester','trimester_4_" + year + "','4')\"><img id=\"trimester_4_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Trim_4" + (isTrimester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append(htmlMonthBuilder.ToString());
            #endregion

            htmlBuilder.Append("\r\n<table class=\"SMChild\" border=0 cellspacing=0 cellpadding=0 width=\"100%\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td colspan=12 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + year + "'," + year + ",'Year','year_" + year + "','" + year + "')\"><img id=\"year_" + year + "\" border=0 src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + year.ToString() + ((isSemester1LinkEnable && isSemester2LinkEnable) ? "" : "NC") + ".gif\"></a></td>");
            htmlBuilder.Append("\r\n\t</tr>");

            htmlBuilder.Append(htmlSemesterBuilder.ToString());

            htmlBuilder.Append("\r\n</table>");

            return (htmlBuilder.ToString());

        }
        #endregion

        #region GetDays
        /// <summary>
        /// Génération d’un calendrier par mois
        /// </summary>
        /// <param name="yearMonth">L'année et mois du calendrier</param>
        /// <returns>Le code HTML</returns>
        protected virtual string GetDays(int yearMonth) {

            DayCalendar dayCalendar = new DayCalendar(yearMonth);
            StringBuilder htmlBuilder = new StringBuilder(6500);

            htmlBuilder.Append("\r\n\t\t\t\t<table cellspacing=1 cellpadding=0 border=0 class=\"whiteBackGround\">");
            // Noms des colonnes
            htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_1.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_2.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_3.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_4.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_5.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_6.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_7.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");

            for (int i = 0; i < dayCalendar.DaysTable.GetLength(0); i++) {

                htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");

                for (int j = 0; j < dayCalendar.DaysTable.GetLength(1); j++) {

                    if (dayCalendar.DaysTable[i, j] != 0) {
                        htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + ".gif\"></a></td>");
                    }
                    else
                        htmlBuilder.Append("\r\n\t\t\t\t\t\t<td class=\"violetBackGroundV4\"><img width=\"17\" height=\"13\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/pixel.gif\"></td>");
                }
                htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");
            }

            htmlBuilder.Append("\r\n\t\t\t\t</table>");
            return (htmlBuilder.ToString());

        }
        #endregion

        #region GetRestrictedDays
        /// <summary>
        /// Génération d’un calendrier par mois
        /// </summary>
        /// <param name="yearMonth">L'année et mois du calendrier</param>
        /// <param name="isMonthLinkEnable">Mois completement chargé</param>
        /// <returns>Le code HTML</returns>
        protected string GetDays(int yearMonth, ref bool isMonthLinkEnable) {

            DayCalendar dayCalendar = new DayCalendar(yearMonth);
            StringBuilder htmlBuilder = new StringBuilder(6500);

            htmlBuilder.Append("\r\n\t\t\t\t<table cellspacing=1 cellpadding=0 border=0 class=\"whiteBackGround\">");
            // Noms des colonnes
            htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_1.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_2.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_3.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_4.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_5.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_6.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/Day_7.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");
			string pathWeb = "";
            for (int i = 0; i < dayCalendar.DaysTable.GetLength(0); i++) {

                htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");

                for (int j = 0; j < dayCalendar.DaysTable.GetLength(1); j++) {

                    if (dayCalendar.DaysTable[i, j] != 0) {
						
						if (_withParutionDates && IsParutionDate(yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00"))) {
							pathWeb = _parutionDateList[yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00")];
							htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\" onmouseout=\""+_idVisualCover+".src = '/App_Themes/" + _themeName + "/Images/Common/vide.gif';"+_idDivCover+".style.display='none';\" onmouseover=\""+_idVisualCover+".src = '" + pathWeb + "';"+_idDivCover+".style.display='block';\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + "p.gif\"></a></td>");
						}
                        else if (IsDayLinkEnabled(yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00"))){
                            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + ".gif\"></a></td>");
						
						}else {
							htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + "NC.gif\"></a></td>");
							isMonthLinkEnable = false;
						}
                    }
                    else
                        htmlBuilder.Append("\r\n\t\t\t\t\t\t<td class=\"violetBackGroundV4\"><img width=\"17\" height=\"13\" src=\"/App_Themes/" + _themeName + "/Images/Culture/GlobalCalendar/pixel.gif\"></td>");
                }
                htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");
            }

            htmlBuilder.Append("\r\n\t\t\t\t</table>");
            return (htmlBuilder.ToString());

        }
        #endregion

        #region Date manager
        /// <summary>
        /// Renvoie le premier jour du mois
        /// </summary>
        /// <param name="yearMonth">Mois et année</param>
        private Int32 GetFirstDayOfMonth(string yearMonth) {

            DateTime firstDayOfMonth = new DateTime(int.Parse(yearMonth.ToString().Substring(0, 4)), int.Parse(yearMonth.ToString().Substring(4, 2)), 1);
            return (Convert.ToInt32(firstDayOfMonth.ToString("yyyyMMdd")));

        }
        /// <summary>
        /// Renvoie le dernier jour du mois
        /// </summary>
        /// <param name="yearMonth">Mois et année</param>
        private Int32 GetLastDayOfMonth(string yearMonth) {

            DateTime firstDayOfMonth = new DateTime(int.Parse(yearMonth.ToString().Substring(0, 4)), int.Parse(yearMonth.ToString().Substring(4, 2)), 1);
            return (Convert.ToInt32(((firstDayOfMonth.AddMonths(1)).AddDays(-1)).ToString("yyyyMMdd")));

        }
        /// <summary>
        /// Renvoie le premier mois du trimestre
        /// </summary>
        /// <param name="yearTrimester">Trimestre et année</param>
        private string GetFirstMonthOfTrimester(string yearTrimester) {

            int trimester;
            string year = string.Empty;

            trimester = Convert.ToInt16(yearTrimester.Substring(4, 2));
            year = yearTrimester.Substring(0, 4);

            switch ((Trimester)trimester) {
                case Trimester.first:
                    return (year + "01");
                case Trimester.second:
                    return (year + "04");
                case Trimester.third:
                    return (year + "07");
                case Trimester.fourth:
                    return (year + "10");
            }

            return "";
        }
        /// <summary>
        /// Renvoie le dernier mois du trimestre
        /// </summary>
        /// <param name="yearTrimester">Trimestre et année</param>
        private string GetLastMonthOfTrimester(string yearTrimester) {

            int trimester;
            string year = string.Empty;

            trimester = Convert.ToInt16(yearTrimester.Substring(4, 2));
            year = yearTrimester.Substring(0, 4);

            switch ((Trimester)trimester) {
                case Trimester.first:
                    return (year + "03");
                case Trimester.second:
                    return (year + "06");
                case Trimester.third:
                    return (year + "09");
                case Trimester.fourth:
                    return (year + "12");
            }

            return "";
        }
        /// <summary>
        /// Renvoie le premier trimestre du semestre
        /// </summary>
        /// <param name="yearSemester">semestre et année</param>
        private string GetFirstTrimesterOfSemester(string yearSemester) {

            int semester;
            string year = string.Empty;

            semester = Convert.ToInt16(yearSemester.Substring(4, 2));
            year = yearSemester.Substring(0, 4);

            switch ((Semester)semester) {
                case Semester.first:
                    return (year + "01");
                case Semester.second:
                    return (year + "03");
            }

            return "";
        }
        /// <summary>
        /// Renvoie le dernier trimestre du semestre
        /// </summary>
        /// <param name="yearSemester">semestre et année</param>
        private string GetLastTrimesterOfSemester(string yearSemester) {

            int semester;
            string year = string.Empty;

            semester = Convert.ToInt16(yearSemester.Substring(4, 2));
            year = yearSemester.Substring(0, 4);

            switch ((Semester)semester) {
                case Semester.first:
                    return (year + "02");
                case Semester.second:
                    return (year + "04");
            }

            return "";
        }
        #endregion

        #region SetSelectedId
        /// <summary>
        /// Définit l'élément à afficher
        /// </summary>
        protected virtual void SetSelectedId() {
            osm.SelectedId = _selectedYear.ToString();
        }
        #endregion

        #region IsDayLinkEnabled
        /// <summary>
        /// Gère l'activaton du lien permettant de sélectionner un jour.
        /// </summary>
        /// <param name="date">La date courante</param>
        /// <returns>vrai si le jour est sélectionnable</returns>
        public bool IsDayLinkEnabled(string date) {
            DateTime currentDay = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            int days = 0;

            days = _firstDayNotEnable.Subtract(currentDay).Days;
            if (days >= 1) return true;
            return false;

        }
        #endregion

		#region IsParutionDate
		/// <summary>
		/// Get if it's parution date
		/// </summary>
		/// <param name="date">date string</param>
		/// <returns>True if parution date</returns>
		protected bool IsParutionDate(string date) {
			return (_parutionDateList != null && _parutionDateList.Count > 0 && date != null && _parutionDateList.ContainsKey(date.Trim()));			
		}
		#endregion

		#endregion

	}
}
