#region Informations
// Auteur: Y. R'kaina
// Création: 22/10/2007
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using TNS.AdExpress.Web;
using TNS.FrameWork.Date;

using WebCustSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Core {

    /// <summary>
    /// Classe pour la gestion de la période sélectionnée par le client (Composant GlobalDateSelection)
    /// </summary>
    public class CustomerPeriod {

        #region Variables
        /// <summary>
        /// Date de début de la période sélectionnée
        /// </summary>
        private string _startDate = string.Empty;
        /// <summary>
        /// Date de fin de la période sélectionnée
        /// </summary>
        private string _endDate = string.Empty;
        /// <summary>
        /// Type de la période comparative
        /// </summary>
        Constantes.Web.globalCalendar.comparativePeriodType _comparativePeriodType;
        /// <summary>
        /// Type de la disponibilité des données
        /// </summary>
        Constantes.Web.globalCalendar.periodDisponibilityType _periodDisponibilityType;
        /// <summary>
        /// Indique s'il s'agit d'une étude comparative
        /// </summary>
        private bool _withComparativePeriodPersonnalized = false;
        /// <summary>
        /// Indique s'il s'agit d'une étude comparative
        /// </summary>
        private bool _withComparativePeriod = false;
        /// <summary>
        /// Liste des dates de début pour les périodes en jours
        /// </summary>
        private ArrayList _periodDayBegin = null;
        /// <summary>
        /// Liste des dates de fin pour les périodes en jours
        /// </summary>
        private ArrayList _periodDayEnd = null;
        /// <summary>
        /// Liste des dates de début pour les périodes en mois
        /// </summary>
        private ArrayList _periodMonthBegin = null;
        /// <summary>
        /// Liste des dates de fin pour les périodes en mois
        /// </summary>
        private ArrayList _periodMonthEnd = null;
        /// <summary>
        /// Indique si on doit utiliser la table 4M ou non
        /// </summary>
        private bool _is4M = false;
        /// <summary>
        /// Date de début de la période comparative
        /// </summary>
        private string _comparativeStartDate = string.Empty;
        /// <summary>
        /// Date de fin de la période comparative
        /// </summary>
        private string _comparativeEndDate = string.Empty;
        /// <summary>
        /// Liste des dates de début pour les périodes en jours (période comparative)
        /// </summary>
        private ArrayList _comparativePeriodDayBegin = null;
        /// <summary>
        /// Liste des dates de fin pour les périodes en jours (période comparative)
        /// </summary>
        private ArrayList _comparativePeriodDayEnd = null;
        /// <summary>
        /// Liste des dates de début pour les périodes en mois (période comparative)
        /// </summary>
        private ArrayList _comparativePeriodMonthBegin = null;
        /// <summary>
        /// Liste des dates de fin pour les périodes en mois (période comparative)
        /// </summary>
        private ArrayList _comparativePeriodMonthEnd = null;
        #endregion

        #region Accesseurs
        /// <summary>
        /// La date de début
        /// </summary>
        public string StartDate {
            get { return _startDate; }
        }
        /// <summary>
        /// La date de fin
        /// </summary>
        public string EndDate {
            get { return _endDate; }
        }
        /// <summary>
        /// Indique s'il s'agit d'une étude comparative
        /// </summary>
        public bool WithComparativePeriod {
            get { return _withComparativePeriod; }
        }
        /// <summary>
        /// Indique s'il s'agit d'une étude comparative personalized
        /// </summary>
        public bool WithComparativePeriodPersonnalized {
            get { return _withComparativePeriodPersonnalized; }
        }
        /// <summary>
        /// Get le type de la période comparative
        /// </summary>
        public Constantes.Web.globalCalendar.comparativePeriodType ComparativePeriodType {
            get { return _comparativePeriodType; }
        }
        /// <summary>
        /// Get le type de la disponibilité des données
        /// </summary>
        public Constantes.Web.globalCalendar.periodDisponibilityType PeriodDisponibilityType {
            get { return _periodDisponibilityType; }
        }
        /// <summary>
        /// GET la liste des dates de début pour les périodes en jours
        /// </summary>
        public ArrayList PeriodDayBegin {
            get { return _periodDayBegin; }
        }
        /// <summary>
        /// GET la liste des dates de fin pour les périodes en jours
        /// </summary>
        public ArrayList PeriodDayEnd {
            get { return _periodDayEnd; }
        }
        /// <summary>
        /// GET la liste des dates de début pour les périodes en mois
        /// </summary>
        public ArrayList PeriodMonthBegin {
            get { return _periodMonthBegin;  }
        }
        /// <summary>
        /// GET la liste des dates de fin pour les périodes en mois
        /// </summary>
        public ArrayList PeriodMonthEnd {
            get { return _periodMonthEnd; }
        }
        /// <summary>
        /// Indique si on doit utiliser la table 4M ou non
        /// </summary>
        public bool Is4M {
            get { return _is4M; }
        }
        /// <summary>
        /// Indique si on doit utiliser la table 4M ou non pour les quatre derniers mois glissants
        /// </summary>
        public bool IsSliding4M {
            get {
                DateTime DateBegin = new DateTime(int.Parse(_startDate.Substring(0, 4)), int.Parse(_startDate.Substring(4, 2)), int.Parse(_startDate.Substring(6, 2)));
                if (_is4M) return true;
                else if (DateBegin >= DateTime.Now.AddMonths(-4).Date)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Indique si on doit utiliser la table DATA_VEHICLE ou non
        /// </summary>
        public bool IsDataVehicle {
            get {
                if ((_periodDayBegin!=null && _periodDayBegin.Count>0)||(_comparativePeriodDayBegin!=null && _comparativePeriodDayBegin.Count>0))
                    return true;
                else return false;
            }
        }
        /// <summary>
        /// Indique si on doit utiliser la table WEB_PLAN ou non
        /// </summary>
        public bool IsWebPlan {
            get {
                if ((_periodMonthBegin != null && _periodMonthBegin.Count > 0)||(_comparativePeriodMonthBegin != null && _comparativePeriodMonthBegin.Count > 0))
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// La date de début (période comparative)
        /// </summary>
        public string ComparativeStartDate {
            get { return _comparativeStartDate; }
        }
        /// <summary>
        /// La date de fin (période comparative)
        /// </summary>
        public string ComparativeEndDate {
            get { return _comparativeEndDate; }
        }
        /// <summary>
        /// GET la liste des dates de début pour les périodes en jours (période comparative)
        /// </summary>
        public ArrayList ComparativePeriodDayBegin {
            get { return _comparativePeriodDayBegin; }
        }
        /// <summary>
        /// GET la liste des dates de fin pour les périodes en jours (période comparative)
        /// </summary>
        public ArrayList ComparativePeriodDayEnd {
            get { return _comparativePeriodDayEnd; }
        }
        /// <summary>
        /// GET la liste des dates de début pour les périodes en mois (période comparative)
        /// </summary>
        public ArrayList ComparativePeriodMonthBegin {
            get { return _comparativePeriodMonthBegin; }
        }
        /// <summary>
        /// GET la liste des dates de fin pour les périodes en mois (période comparative)
        /// </summary>
        public ArrayList ComparativePeriodMonthEnd {
            get { return _comparativePeriodMonthEnd; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="startDate">La date de début</param>
        /// <param name="endDate">La date de fin</param>
        public CustomerPeriod(string startDate, string endDate) {

            #region Init
            _periodDayBegin = new ArrayList();
            _periodDayEnd = new ArrayList();
            _periodMonthBegin = new ArrayList();
            _periodMonthEnd = new ArrayList();
            #endregion

            DateTime date = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));

            _startDate = startDate;
            _endDate = endDate;
            
            if (date <= DateTime.Now) {
                PeriodSplit(_startDate, _endDate, ref _periodDayBegin, ref _periodDayEnd, ref _periodMonthBegin, ref _periodMonthEnd);
                if (!_is4M)
                    PeriodTreatment(ref _periodDayBegin, ref _periodDayEnd, ref _periodMonthBegin, ref _periodMonthEnd);
            }

        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="startDate">La date de début</param>
        /// <param name="endDate">La date de fin</param>
        /// <param name="comparativeEndDate"></param>
        /// <param name="comparativeStartDate"></param>
        public CustomerPeriod(string startDate, string endDate, string comparativeStartDate, string comparativeEndDate)
            : this(startDate, endDate) {

            #region Init
            _comparativePeriodDayBegin = new ArrayList();
            _comparativePeriodDayEnd = new ArrayList();
            _comparativePeriodMonthBegin = new ArrayList();
            _comparativePeriodMonthEnd = new ArrayList();
            #endregion

            DateTime date = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));

            _comparativePeriodType = TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate;
            _comparativeStartDate = comparativeStartDate;
            _comparativeEndDate = comparativeEndDate;
            _withComparativePeriodPersonnalized = _withComparativePeriod  = true;

            if (date <= DateTime.Now) {
                PeriodSplit(_comparativeStartDate, _comparativeEndDate, ref _comparativePeriodDayBegin, ref _comparativePeriodDayEnd, ref _comparativePeriodMonthBegin, ref _comparativePeriodMonthEnd);
                if (!_is4M)
                    PeriodSplit(_comparativeStartDate, _comparativeEndDate, ref _comparativePeriodDayBegin, ref _comparativePeriodDayEnd, ref _comparativePeriodMonthBegin, ref _comparativePeriodMonthEnd);
            }

        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="startDate">La date de début</param>
        /// <param name="endDate">La date de fin</param>
        /// <param name="withComparativePeriod">Indique s'il s'agit d'une étude comparative</param>
        /// <param name="comparativePeriodType">Type de la période comparative</param>
        public CustomerPeriod(string startDate, string endDate, bool withComparativePeriod, Constantes.Web.globalCalendar.comparativePeriodType comparativePeriodType, Constantes.Web.globalCalendar.periodDisponibilityType periodDisponibilityType):this(startDate, endDate) {

            _comparativePeriodType = comparativePeriodType;
            _periodDisponibilityType = periodDisponibilityType;
            _withComparativePeriod = withComparativePeriod;
            DateTime date = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));

            if (_withComparativePeriod) {

                #region Init
                _comparativePeriodDayBegin = new ArrayList();
                _comparativePeriodDayEnd = new ArrayList();
                _comparativePeriodMonthBegin = new ArrayList();
                _comparativePeriodMonthEnd = new ArrayList();
                #endregion

                SetComparativePeriod();
                if (date <= DateTime.Now) {
                    PeriodSplit(_comparativeStartDate, _comparativeEndDate, ref _comparativePeriodDayBegin, ref _comparativePeriodDayEnd, ref _comparativePeriodMonthBegin, ref _comparativePeriodMonthEnd);
                }
            }
        } 
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="periodDayBegin">Liste des dates de début pour les périodes en jours</param>
        /// <param name="periodDayEnd">Liste des dates de fin pour les périodes en jours</param>
        /// <param name="periodMonthBegin">Liste des dates de début pour les périodes en mois</param>
        /// <param name="periodMonthEnd">Liste des dates de fin pour les périodes en mois</param>
        /// <param name="is4M">Indique si on doit utiliser la table 4M ou non</param>
        public CustomerPeriod(ArrayList periodDayBegin, ArrayList periodDayEnd, ArrayList periodMonthBegin, ArrayList periodMonthEnd, bool is4M, string startDate, string endDate) {
            _periodDayBegin = periodDayBegin;
            _periodDayEnd = periodDayEnd;
            _periodMonthBegin = periodMonthBegin;
            _periodMonthEnd = periodMonthEnd;
            _is4M = is4M;
            _startDate = startDate;
            _endDate = endDate;
        }
        #endregion

        #region Méthodes internes

        #region PeriodSplit
        /// <summary>
        /// Permet la décomposition de la période sélectionnée en plusieur sous périodes
        /// </summary>
        /// <param name="startDate">La date de début</param>
        /// <param name="endDate">La date de fin</param>
        /// <param name="periodDayBegin">Liste des dates de début pour les périodes en jours</param>
        /// <param name="periodDayEnd">Liste des dates de fin pour les périodes en jours</param>
        /// <param name="periodMonthBegin">Liste des dates de début pour les périodes en mois</param>
        /// <param name="periodMonthEnd">Liste des dates de fin pour les périodes en mois</param>
        private void PeriodSplit(string startDate, string endDate, ref ArrayList periodDayBegin, ref ArrayList periodDayEnd, ref ArrayList periodMonthBegin, ref ArrayList periodMonthEnd) {

            #region Variables
            Int32 startDay = Convert.ToInt32(startDate.Substring(6, 2));
            Int32 endDay = Convert.ToInt32(endDate.Substring(6, 2));
            string startMonth = startDate.Substring(0, 6);
            string endMonth = endDate.Substring(0, 6);
            string nextMonth = string.Empty;
            string previousMonth = string.Empty;
            Int32 startYear = Convert.ToInt32(startDate.Substring(0, 4));
            Int32 endYear = Convert.ToInt32(endDate.Substring(0, 4));
            Int32 startMonthInCalendar = Convert.ToInt32(startMonth.Substring(4, 2));
            Int32 endMonthInCalendar = (endYear - startYear) * 12 + Convert.ToInt32(endMonth.Substring(4, 2));
            int periodMonthLength = endMonthInCalendar - startMonthInCalendar;
            Int32 month4M = DateTime.Now.AddMonths(-3).Year * 100 + DateTime.Now.AddMonths(-3).Month;
            #endregion

            #region décomposition

            //Cas des tables 4M
            if (Convert.ToInt32(startMonth) >= month4M) {
                _is4M = true;
            }
            else {

                if (periodMonthLength >= 1) {

                    if (startDay == 1)
                        periodMonthBegin.Add(startDate);
                    else {
                        periodDayBegin.Add(startDate);
                        periodDayEnd.Add(startMonth + GetLastDayOfMonth(startMonth));

                        if (periodMonthLength > 1 || (periodMonthLength == 1 && endDay == GetLastDayOfMonth(endMonth))) {

                            nextMonth = getNextMonth(startMonth);
                            periodMonthBegin.Add(nextMonth + "01");

                        }
                    }

                    if (endDay == GetLastDayOfMonth(endMonth))
                        periodMonthEnd.Add(endDate);
                    else {
                        periodDayBegin.Add(endMonth + "01");
                        periodDayEnd.Add(endDate);

                        if (periodMonthLength > 1 || (periodMonthLength == 1 && startDay == 1)) {

                            previousMonth = getPreviousMonth(endMonth);
                            periodMonthEnd.Add(previousMonth + GetLastDayOfMonth(previousMonth));

                        }
                    }
                }
                else {
                    if (startDay == 1 && endDay == GetLastDayOfMonth(endMonth)) {
                        periodMonthBegin.Add(startDate);
                        periodMonthEnd.Add(endDate);
                    }
                    else {
                        periodDayBegin.Add(startDate);
                        periodDayEnd.Add(endDate);
                    }
                }

            }
            #endregion

        }
        #endregion

        #region Period Treatment
        /// <summary>
        /// Pemret de déterminer quelle table il faut utiliser pour le dernier mois complet
        /// </summary>
        /// <param name="periodDayBegin">Liste des dates de début pour les périodes en jours</param>
        /// <param name="periodDayEnd">Liste des dates de fin pour les périodes en jours</param>
        /// <param name="periodMonthBegin">Liste des dates de début pour les périodes en mois</param>
        /// <param name="periodMonthEnd">Liste des dates de fin pour les périodes en mois</param>
        private void PeriodTreatment(ref ArrayList periodDayBegin, ref ArrayList periodDayEnd, ref ArrayList periodMonthBegin, ref ArrayList periodMonthEnd) {
            
            string date = string.Empty, endMonthYear = string.Empty, previousMonth = string.Empty, previousPreviousMonth = string.Empty;
            Int32 endYear, lastMonthInPeriod, currentMonth, lastDayInPeriod;
            bool oneMonth = false;

            if (periodMonthBegin != null && periodMonthBegin.Count > 0 && periodMonthEnd != null && periodMonthEnd.Count>0) {

                date = (string)periodMonthEnd[0];
                endMonthYear = date.Substring(0, 6);
                endYear = Convert.ToInt32(date.Substring(0, 4));
                lastMonthInPeriod = Convert.ToInt32(date.Substring(4, 2));
                lastDayInPeriod = Convert.ToInt32(date.Substring(6, 2));
                currentMonth = (DateTime.Now.Year - endYear) * 12 + DateTime.Now.Month;

                //Cas d'un seul mois
                if(((string)periodMonthBegin[0]).Substring(0, 6) == ((string)periodMonthEnd[0]).Substring(0, 6))
                    oneMonth = true;

                if (currentMonth - lastMonthInPeriod == 1) {

                    previousMonth = getPreviousMonth(endMonthYear);

                    #region Month period
                    if (oneMonth) {
                        periodMonthBegin = null;
                        periodMonthEnd = null;
                    }
                    else
                        periodMonthEnd[0] = previousMonth + GetLastDayOfMonth(previousMonth);
                    #endregion

                    #region Day period
                    switch (periodDayBegin.Count) {
                        case 2 : 
                            periodDayBegin[1] = endMonthYear + "01";
                            break;
                        case 1 :
                            if (Convert.ToInt32(((string)periodMonthEnd[0]).Substring(0, 6)) < Convert.ToInt32(((string)periodDayBegin[0]).Substring(0, 6)))
                                periodDayBegin[0] = endMonthYear + "01";
                            else
                                periodDayBegin.Add(endMonthYear + "01");
                            break;
                        case 0 :
                            periodDayBegin.Add(endMonthYear + "01");
                            break;
                    }

                    switch (periodDayEnd.Count) { 
                        case 0:
                            periodDayEnd.Add(endMonthYear + GetLastDayOfMonth(endMonthYear));
                            break;
                        case 1:
                            if (Convert.ToInt32(((string)periodMonthEnd[0]).Substring(0, 6)) > Convert.ToInt32(((string)periodDayBegin[0]).Substring(0, 6)))
                                periodDayEnd.Add(endMonthYear + GetLastDayOfMonth(endMonthYear));
                            break;
                    }
                    #endregion

                }

                if ((DateTime.Now.Day == lastDayInPeriod) && (currentMonth - lastMonthInPeriod == 0)) {

                    previousMonth = getPreviousMonth(endMonthYear);
                    previousPreviousMonth = getPreviousMonth(previousMonth);

                    #region Month period
                    if (oneMonth) {
                        periodMonthBegin = null;
                        periodMonthEnd = null;
                    }
                    else
                        periodMonthEnd[0] = previousPreviousMonth + GetLastDayOfMonth(previousPreviousMonth);
                    #endregion

                    #region Day period
                    switch (periodDayBegin.Count) { 
                        case 2:
                            if (oneMonth)
                                periodDayBegin[1] = endMonthYear + "01";
                            else
                                periodDayBegin[1] = previousMonth + "01";
                            break;
                        case 1:
                            if (oneMonth)
                                periodDayBegin[0] = endMonthYear + "01";
                            else
                                periodDayBegin.Add(previousMonth + "01");
                            break;
                        case 0:
                            if (oneMonth)
                                periodDayBegin.Add(endMonthYear + "01");
                            else
                                periodDayBegin.Add(previousMonth + "01");
                            break;
                    }

                    if (periodDayEnd.Count <= 1)
                        periodDayEnd.Add(endMonthYear + GetLastDayOfMonth(endMonthYear));
                    #endregion

                }
            }
        }
        #endregion

        #region Set Comparative Period
        /// <summary>
        /// Initialise la période comparative
        /// </summary>
        private void SetComparativePeriod() {

            switch (_comparativePeriodType) { 
                
                case Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                    _comparativeStartDate = GetPreviousYearDate(_startDate, Constantes.Web.globalCalendar.comparativePeriodType.dateToDate);
                    _comparativeEndDate = GetPreviousYearDate(_endDate, Constantes.Web.globalCalendar.comparativePeriodType.dateToDate);
                    break;
                case Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    _comparativeStartDate = GetPreviousYearDate(_startDate, Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate);
                    _comparativeEndDate = GetPreviousYearDate(_endDate, Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate);
                    break;
            
            }

        }
        #endregion

        #region GetLastDayOfMonth
        /// <summary>
        /// Renvoie le dernier jour du mois passer en paramètre
        /// </summary>
        /// <returns>le dernier jour du mois</returns>
        private Int32 GetLastDayOfMonth(string yearMonth) {

            DateTime firstDayOfMonth = new DateTime(int.Parse(yearMonth.ToString().Substring(0, 4)), int.Parse(yearMonth.ToString().Substring(4, 2)), 1);
            Int32 lastDayOfMonth = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;

            return lastDayOfMonth;

        }
        #endregion

        #region getNextMonth
        /// <summary>
        /// Renvoie le mois prochain
        /// </summary>
        /// <param name="yearMonth">Année + Mois</param>
        /// <returns>Mois prochain</returns>
        private string getNextMonth(string yearMonth){

            DateTime dt = new DateTime(int.Parse(yearMonth.ToString().Substring(0, 4)), int.Parse(yearMonth.ToString().Substring(4, 2)), 1);
            dt = dt.AddMonths(1);
            yearMonth = MonthString.GetYYYYMM(dt.Year.ToString() + dt.Month.ToString());

            return yearMonth;
        }
        #endregion

        #region getPreviousMonth
        /// <summary>
        /// Renvoie le mois précédent
        /// </summary>
        /// <param name="yearMonth">Année + Mois</param>
        /// <returns>Mois précédent</returns>
        private string getPreviousMonth(string yearMonth) {

            DateTime dt = new DateTime(int.Parse(yearMonth.ToString().Substring(0, 4)), int.Parse(yearMonth.ToString().Substring(4, 2)), 1);
            dt = dt.AddMonths(-1);
            yearMonth = MonthString.GetYYYYMM(dt.Year.ToString() + dt.Month.ToString());

            return yearMonth;
        }
        #endregion

        #region GetPreviousYearDate
        /// <summary>
        /// Obtient la date de l'année précédente  sous forme YYYYMMDD
        /// </summary>
        /// <param name="period">date de l'année en cours sous forme YYYYMMDD</param>
        /// <param name="comparativePeriodType">Type de la période comparative</param>
        /// <returns>date de l'année précédente  sous forme YYYYMMDD</returns>
        private string GetPreviousYearDate(string period, Constantes.Web.globalCalendar.comparativePeriodType comparativePeriodType) {

                AtomicPeriodWeek tmpWeek;
                DateTime currentDate;
                int currentDay;

                currentDate = new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));

                switch (comparativePeriodType) {

                    case Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                        currentDate = currentDate.AddYears(-1);
                        return currentDate.ToString("yyyyMMdd");

                    case Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                        currentDay = currentDate.DayOfWeek.GetHashCode();
                        tmpWeek = new AtomicPeriodWeek(currentDate);
                        tmpWeek.SubWeek(52);
                        if (currentDay == 0)
                            currentDate = tmpWeek.FirstDay.AddDays(6);
                        else
                            currentDate = tmpWeek.FirstDay.AddDays(currentDay - 1);

                        return currentDate.ToString("yyyyMMdd");
                }

                return "";

        }
        #endregion

        #endregion
    }
}
