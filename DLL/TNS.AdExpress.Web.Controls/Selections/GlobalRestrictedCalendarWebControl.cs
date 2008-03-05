#region Informations
// Auteur: Y. Rkaina
// Date de création: 30/10/2007
// Date de modification: 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Classification.DB;


namespace TNS.AdExpress.Web.Controls.Selections {
    /// <summary>
    /// Calendrier global avec limitation de la période
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GlobalRestrictedCalendarWebControl runat=server></{0}:GlobalRestrictedCalendarWebControl>")]
    public class GlobalRestrictedCalendarWebControl : GlobalCalendarWebControl {

        #region Variables
        /// <summary>
        /// Année sélectionnée
        /// </summary>
        protected int _selectedYear = DateTime.Now.Year;
        /// <summary>
        /// Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        protected DateTime _firstDayNotEnable;
        /// <summary>
        /// Le vehicle sélectionné
        /// </summary>
        protected long _selectedVehicle;
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _customerWebSession = null; 
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient et définit l'année à afficher
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("2007")]
        public int SelectedYear {
            get { return _selectedYear; }
            set { _selectedYear = value;}
        }
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession {
            get { return (_customerWebSession); }
            set { _customerWebSession = value; }
        }
        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation de l'objet
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            _startYear = _selectedYear - 2;
            if (DateTime.Now.Month == 12) _stopYear = (DateTime.Now.AddYears(1)).Year;
            else {
                _stopYear = DateTime.Now.Year;
            }
            _selectedVehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID;
            _firstDayNotEnable = GetFirstDayNotEnabled();
        }
        #endregion

        #endregion

        #region Méthode interne

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected override string Initialisation() {
            StringBuilder js = new StringBuilder();

            js.Append(base.Initialisation());
            js.Append("\r\n\n\t /// Chargement des images");
            js.AppendFormat("\r\n\t var dayImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=31;i++){");
            js.AppendFormat("\r\n\t dayImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t dayImageListNC_{0}[i].src = '/Images/" + _language.ToString() + "/GlobalCalendar/' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var monthImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=12;i++){");
            js.AppendFormat("\r\n\t monthImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t monthImageListNC_{0}[i].src = '/Images/" + _language.ToString() + "/GlobalCalendar/Month_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var trimesterImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=4;i++){");
            js.AppendFormat("\r\n\t trimesterImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t trimesterImageListNC_{0}[i].src = '/Images/" + _language.ToString() + "/GlobalCalendar/Trim_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var semesterImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=1;i<=2;i++){");
            js.AppendFormat("\r\n\t semesterImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t semesterImageListNC_{0}[i].src = '/Images/" + _language.ToString() + "/GlobalCalendar/Sem_' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");
            js.AppendFormat("\r\n\t var yearImageListNC_{0} = new Array();", this.ID);
            js.Append("\r\n\t for(i=" + _startYear + ";i<=" + _stopYear + ";i++){");
            js.AppendFormat("\r\n\t yearImageListNC_{0}[i] = new Image();", this.ID);
            js.AppendFormat("\r\n\t yearImageListNC_{0}[i].src = '/Images/" + _language.ToString() + "/GlobalCalendar/' + i + 'NC.gif';", this.ID);
            js.Append("\r\n\t }");

            return (js.ToString());
        }
        #endregion

        #region GetVariables
        /// <summary>
        /// renvoie la déclaration des variables
        /// </summary>
        /// <returns></returns>
        protected override string GetVariables() {
            StringBuilder js = new StringBuilder();

            js.Append(base.GetVariables());
            // Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
            js.AppendFormat("\r\n\t var firstDayNotEnable_{0}='"+_firstDayNotEnable.Year.ToString()+ _firstDayNotEnable.Month.ToString("00")+_firstDayNotEnable.Day.ToString("00")+"';", this.ID);

            return (js.ToString());
        }
        #endregion

        #region PeriodPrintYear
        /// <summary>
        /// coloriage d'une année
        /// </summary>
        /// <returns>Script de la fonction</returns>
        protected override string PeriodPrintYear() {
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
        protected override string PeriodPrintSemester() {
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
        protected override string PeriodPrintTrimester() {
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
        protected override string PeriodPrintMonth() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintMonth(year, month, init, periodType){");

            js.Append("\r\n\t var dayStr='', monthStr='';");
            js.Append("\r\n\t var dImageList = new Array();");
            js.Append("\r\n\t var dImageListNC = new Array();");
            js.Append("\r\n\t var mImageList = new Array();");
            js.AppendFormat("\r\n\t var firstDayNotEnable = firstDayNotEnable_{0}.substr(0,6);", this.ID);
            js.Append("\r\n\t\t monthStr=month+'';");
            js.Append("\r\n\t\t if(monthStr.length==1)monthStr = '0'+month;");
            js.Append("\r\n\t var dateMonth = year+monthStr;");
            js.Append("\r\n\t var dateDay;");
            js.Append("\r\n\t var enable=1;");

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
            js.Append("\r\n\t if(periodType!='Day')");
            js.Append("\r\n\t\t elementsPeriod['month_'+year+''+month].src=mImageList[parseFloat(month)].src;");

            js.Append("\r\n\t\t for(i=1;i<=elementsYear[year][year+''+monthStr][year+''+monthStr+''+'00'];i++){");
            js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
            js.Append("\r\n\t\t\t dateDay = year+monthStr+dayStr;");
            js.AppendFormat("\r\n\t\t\t if(firstDayNotEnable_{0}<=dateDay && init==1)",this.ID);
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = dImageListNC[i].src;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = dImageList[i].src;");
            js.Append("\r\n\t }");
            js.Append("\r\n\t dImageList = null;");
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
        protected override string PeriodPrintDay() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\n function PeriodPrintDay(dateBegin, dateEnd, year, init, initAll){");

            js.Append("\r\n\t var dayStr='', monthStr=dateBegin.substr(4,2);");
            js.Append("\r\n\t var dayBegin = parseFloat(dateBegin.substr(6,2));");
            js.Append("\r\n\t var dayEnd = parseFloat(dateEnd.substr(6,2));");
            js.Append("\r\n\t var imageList = new Array();");
            js.Append("\r\n\t var imageListNC = new Array();");
            js.AppendFormat("\r\n\t var firstDayNotEnable = firstDayNotEnable_{0};", this.ID);
            js.Append("\r\n\t var dateDay;");

            js.Append("\r\n\t if(init==1){");
            js.AppendFormat("\r\n\t\t imageList = dayImageListI_{0};", this.ID);
            js.AppendFormat("\r\n\t\t imageListNC = dayImageListNC_{0};", this.ID);
            js.Append("\r\n\t }");
            js.Append("\r\n\t else");
            js.AppendFormat("\r\n\t\t imageList = dayImageList_{0};", this.ID);

            js.Append("\r\n\t if(monthStr.length==1)monthStr = '0'+monthStr;");
            js.Append("\r\n\t if(init==1 && initAll==0) dayBegin++;");
            js.Append("\r\n\t\t for(i=dayBegin;i<=dayEnd;i++){");
            js.Append("\r\n\t\t\t dayStr=i+'';");
            js.Append("\r\n\t\t\t if(dayStr.length==1)dayStr='0'+i;");
            js.Append("\r\n\t\t\t dateDay = year+monthStr+dayStr;");
            js.Append("\r\n\t\t\t if(firstDayNotEnable<=dateDay && init==1)");
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = imageListNC[i].src;");
            js.Append("\r\n\t\t\t else");
            js.Append("\r\n\t\t\t\t elementsYear[year][year+''+monthStr][year+''+monthStr+''+dayStr].src = imageList[i].src;");
            js.Append("\r\n\t\t }");
            js.Append("\r\n\t imageList = null;");

            js.Append("\r\n}");

            return (js.ToString());
        }
        #endregion

        #region GetCalendarBottomHtml
        /// <summary>
        /// Renvoie la partie contenant la zone pour l'affichage de la sélection, le boutton init et le boutton valider
        /// </summary>
        /// <returns>Le code HTML</returns>
        protected override string GetCalendarBottomHtml() {

            StringBuilder htmlBuilder = new StringBuilder(1000);

            htmlBuilder.Append("\r\n\t<table border=0 cellspacing=0 cellpadding=0 width=\"100%\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<TD class=\"txtViolet11\" colSpan=\"2\" nowrap>");
            htmlBuilder.Append("\r\n\t " + GestionWeb.GetWebWord(2280, _customerWebSession.SiteLanguage) + "");
            htmlBuilder.Append("\r\n\t\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td class=\"txtGris11Bold\" colspan=\"2\" style=\"HEIGHT: 28px\">");
            htmlBuilder.Append("\r\n\t " + _periodSelectionTitle + "");
            htmlBuilder.Append("\r\n\t\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td style=\"height: 16px\">");
            htmlBuilder.Append("\r\n\t\t&nbsp;");
            htmlBuilder.Append("\r\n\t\t\t<label id=\"Label1\" style=\"display:inline-block;color:#644883;background-color:#E8E4ED;border-color:#A794BE;border-width:1px;border-style:Solid;height:16px;width:180px;vertical-align:top; font-family: Arial, Helvetica, sans-serif;font-size: 11px;font-weight: bold; white-space:nowrap;\"></label>&nbsp;&nbsp;");
            htmlBuilder.Append("\r\n\t\t\t<img id=\"ok\" src=\"/Images/Common/button/initialize_up.gif\" onmouseover=\"ok.src='/Images/Common/button/initialize_down.gif';\" onmouseout=\"ok.src='/Images/Common/button/initialize_up.gif';\" onclick=\"InitAll();\" style=\"cursor:pointer\"/>");
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
            htmlBuilder.Append("\r\n\t<td valign=\"top\" background=\"/Images/Common/dupli_fond.gif\" height=\"25\">");
            htmlBuilder.Append("\r\n\t<table id=\"Table12\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t<td width=\"135\"></td>");
            htmlBuilder.Append("\r\n\t<td><img id=\"valider\" src=\"/Images/" + _language + "/button/valider_up.gif\" onmouseover=\"valider.src='/Images/" + _language + "/button/valider_down.gif';\" onmouseout=\"valider.src='/Images/" + _language + "/button/valider_up.gif';\" onclick=\"__doPostBack('" + this.ID + "','');\" style=\"cursor:pointer\"/></td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t</table>");
            htmlBuilder.Append("\r\n\t</td>");
            htmlBuilder.Append("\r\n\t</tr>");
            htmlBuilder.Append("\r\n\t</table>");

            return (htmlBuilder.ToString());

        }
        #endregion

        #region GetYearHTML
        /// <summary>
        /// Génération d’un calendrier sélectionnable par année, semestre, trimestre, mois, jour pour l’année passée en paramètre
        /// </summary>
        /// <param name="year">L'année du calendrier</param>
        /// <returns>Le code HTML</returns>
        protected override string GetYearHTML(int year) {

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
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Month_" + month + (isMonthLinkEnable ? "":"NC") + ".gif\"></a></td>");
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
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Semester','semester_1_" + year + "','1')\"><img id=\"semester_1_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Sem_1" + (isSemester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('1'," + year + ",'Trimester','trimester_1_" + year + "','1')\"><img id=\"trimester_1_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Trim_1" + (isTrimester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Trimester','trimester_2_" + year + "','2')\"><img id=\"trimester_2_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Trim_2" + (isTrimester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
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
                htmlMonthBuilder.Append("\r\n\t\t\t\t<td align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + month + "'," + year + ",'Month','month_" + currentYearMonth + "','" + month + "')\"><img id=\"month_" + currentYearMonth + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Month_" + month + (isMonthLinkEnable ? "":"NC") + ".gif\"></a></td>");
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
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMSemestre\" colspan=6 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('2'," + year + ",'Semester','semester_2_" + year + "','2')\"><img id=\"semester_2_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Sem_2" + (isSemester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append("\r\n\t<tr>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreL\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('3'," + year + ",'Trimester','trimester_3_" + year + "','3')\"><img id=\"trimester_3_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Trim_3" + (isTrimester1LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t\t<td class=\"SMTrimestreR\" colspan=3 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('4'," + year + ",'Trimester','trimester_4_" + year + "','4')\"><img id=\"trimester_4_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/Trim_4" + (isTrimester2LinkEnable ? "" : "NC") + ".gif\"></a></td>");
            htmlSemesterBuilder.Append("\r\n\t</tr>");

            htmlSemesterBuilder.Append(htmlMonthBuilder.ToString());            
            #endregion

            htmlBuilder.Append("\r\n<table border=0 cellspacing=0 cellpadding=0 width=\"100%\">");
            htmlBuilder.Append("\r\n\t<tr>");
            htmlBuilder.Append("\r\n\t\t<td colspan=12 align=\"center\"><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + year + "'," + year + ",'Year','year_" + year + "','" + year + "')\"><img id=\"year_" + year + "\" border=0 src=\"/Images/" + _language.ToString() + "/GlobalCalendar/" + year.ToString() + ((isSemester1LinkEnable && isSemester2LinkEnable) ? "":"NC") + ".gif\"></a></td>");
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
        /// <param name="isMonthLinkEnable">Mois completement chargé</param>
        /// <returns>Le code HTML</returns>
        protected string GetDays(int yearMonth, ref bool isMonthLinkEnable) {

            DayCalendar dayCalendar = new DayCalendar(yearMonth);
            StringBuilder htmlBuilder = new StringBuilder(6500);

            htmlBuilder.Append("\r\n\t\t\t\t<table cellspacing=1 cellpadding=0 border=0 bgcolor=#FFFFFF>");
            // Noms des colonnes
            htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_1.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_2.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_3.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_4.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_5.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_6.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><img src=\"/Images/" + Language.ToString() + "/GlobalCalendar/Day_7.gif\" border=0></td>");
            htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");

            for (int i = 0; i < dayCalendar.DaysTable.GetLength(0); i++) {

                htmlBuilder.Append("\r\n\t\t\t\t\t<tr>");

                for (int j = 0; j < dayCalendar.DaysTable.GetLength(1); j++) {

                    if (dayCalendar.DaysTable[i, j] != 0) {
                        if (IsDayLinkEnabled(yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00")))
                            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/Images/" + _language.ToString() + "/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + ".gif\"></a></td>");
                        else {
                            htmlBuilder.Append("\r\n\t\t\t\t\t\t<td cellpadding=5><a style=\"outline:none;\" href=\"javascript:SelectedDate('" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "'," + (yearMonth.ToString()).Substring(0, 4) + ",'Day','day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "','" + dayCalendar.DaysTable[i, j] + "')\"><img id=\"day_" + yearMonth.ToString() + dayCalendar.DaysTable[i, j].ToString("00") + "\" border=0 style=\"outline:none;\" src=\"/Images/" + _language.ToString() + "/GlobalCalendar/" + dayCalendar.DaysTable[i, j] + "NC.gif\"></a></td>");
                            isMonthLinkEnable = false;
                        }
                    }
                    else
                        htmlBuilder.Append("\r\n\t\t\t\t\t\t<td bgcolor=\"#A794BE\"><img width=\"17\" height=\"13\" src=\"/Images/" + _language.ToString() + "/GlobalCalendar/pixel.gif\"></td>");
                }
                htmlBuilder.Append("\r\n\t\t\t\t\t</tr>");
            }

            htmlBuilder.Append("\r\n\t\t\t\t</table>");
            return (htmlBuilder.ToString());

        }
        #endregion

        #region SetSelectedId
        /// <summary>
        /// Définit l'élément à afficher
        /// </summary>
        protected override void SetSelectedId() {
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
            bool enabled = false;
            DateTime currentDay = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            int days = 0;
            string lastDate = string.Empty;
           
            switch ((Vehicles.names)_selectedVehicle) {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
                    days = _firstDayNotEnable.Subtract(currentDay).Days;
                    if (days >= 1) return true;
                    break;
                case Vehicles.names.outdoor:
                    firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    days = firstDayOfWeek.Subtract(currentDay).Days;
                    if (days >= 1) return true;
                    break;
                case Vehicles.names.radio:
                case Vehicles.names.tv:
                case Vehicles.names.others:
                    if (!((int)DateTime.Now.DayOfWeek >= 5) && !((int)DateTime.Now.DayOfWeek == 0)) {
                        firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    }
                    days = firstDayOfWeek.Subtract(currentDay).Days;
                    if (days >= 1) return true;
                    break;
                case Vehicles.names.directMarketing:
                    days = _firstDayNotEnable.Subtract(currentDay).Days;
                    if (days >= 1) return true;
                    break;
                case Vehicles.names.internet:
                    days = _firstDayNotEnable.Subtract(currentDay).Days;
                    if (days >= 1) return true;
                    break;
            }

            return enabled;

        }
        #endregion

        #region GetFirstDayNotEnable
        /// <summary>
        /// Renvoie le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        /// <returns>Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées</returns>
        public DateTime GetFirstDayNotEnabled() {
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;

            switch ((Vehicles.names)_selectedVehicle) {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
					lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_customerWebSession, _selectedVehicle, _customerWebSession.Source);
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                    break;
                case Vehicles.names.outdoor:
                    firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    return (firstDayOfWeek);
                case Vehicles.names.radio:
                case Vehicles.names.tv:
                case Vehicles.names.others:
                    if (!((int)DateTime.Now.DayOfWeek >= 5) && !((int)DateTime.Now.DayOfWeek == 0)) {
                        firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    }
                    return (firstDayOfWeek);
                case Vehicles.names.directMarketing:
					lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_customerWebSession, _selectedVehicle, _customerWebSession.Source);
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(7);
                    return firstDayOfWeek;
                case Vehicles.names.internet:
					lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_customerWebSession, _selectedVehicle, _customerWebSession.Source);
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

        #endregion

    }
}
