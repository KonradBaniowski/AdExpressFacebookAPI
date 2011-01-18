#region Informations
/*
 * Author : G. Ragneau
 * Created : 21/11/2007
 * Modifications :
 *      Author - Date - Descriptopn
 *      
*/
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Core.Selection
{

    #region MediaSchedulePeriod
    /// <summary>
    /// Media Schedule Period Management regarding to global date selection
    /// </summary>
    public class MediaSchedulePeriod
    {

        #region Constantes
        private const long NANOSECOND_A_DAY = 86400000000000;
        #endregion

        #region Attributes
        /// <summary>
        /// Perdiod Beginning
        /// </summary>
        private DateTime _begin = DateTime.Now;
        /// <summary>
        /// Period End
        /// </summary>
        private DateTime _end = DateTime.Now;
        /// <summary>
        /// SubPeriods granularity
        /// </summary>
        private CstPeriod.DisplayLevel _periodBreakDown = CstPeriod.DisplayLevel.dayly;
        /// <summary>
        /// List of period breakdowns
        /// </summary>
        private List<MediaScheduleSubPeriod> _subPeriods = new List<MediaScheduleSubPeriod>();
        /// <summary>
        /// Whether is comparative period or not 
        /// </summary>
        private bool _isComparativePeriod = false;
        /// <summary>
        /// Comparative Period Type
        /// </summary>
        private CstWeb.globalCalendar.comparativePeriodType _comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
        #endregion

        #region Accessors
        /// <summary>
        /// SubPeriods granularity
        /// </summary>
        public CstPeriod.DisplayLevel PeriodDetailLEvel
        {
            get { return _periodBreakDown; }
        }
        /// <summary>
        /// Get Set of Subperiods composing the global period
        /// </summary>
        public List<MediaScheduleSubPeriod> SubPeriods
        {
            get { return _subPeriods; }
        }
        /// <summary>
        /// Get Period Beginning
        /// </summary>
        public DateTime Begin
        {
            get { return _begin; }
        }
        /// <summary>
        /// Get Period End
        /// </summary>
        public DateTime End
        {
            get { return _end; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="period">period to duplicate</param>
        /// <param name="periodBreakDown">Period breakdown (days, weeks, monthes)</param>
        public MediaSchedulePeriod(CustomerPeriod period, CstPeriod.DisplayLevel periodBreakDown)
            : this(DateString.YYYYMMDDToDateTime(period.StartDate), DateString.YYYYMMDDToDateTime(period.EndDate), periodBreakDown)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="begin">Period Beginning</param>
        /// <param name="end">Period End</param>
        /// <param name="periodBreakDown">Period breakdown (days, weeks, monthes)</param>
        public MediaSchedulePeriod(string begin, string end, CstPeriod.DisplayLevel periodBreakDown)
            : this(DateString.YYYYMMDDToDateTime(begin), DateString.YYYYMMDDToDateTime(end), periodBreakDown)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="begin">Period Beginning</param>
        /// <param name="end">Period End</param>
        /// <param name="periodBreakDown">Period breakdown (days, weeks, monthes)</param>
        public MediaSchedulePeriod(DateTime begin, DateTime end, CstPeriod.DisplayLevel periodBreakDown)
        {
            if (begin.CompareTo(end) > 0) throw new ArgumentException("Period beginning must anterior to period end.");
            _begin = begin.Date;
            _end = end.Date;
            _periodBreakDown = periodBreakDown;
            ComputePeriod();
        }

        /// <summary>
        /// Constructor (Comparative period is used with this constructor)
        /// </summary>
        /// <param name="begin">Period Beginning</param>
        /// <param name="end">Period End</param>
        /// <param name="periodBreakDown">Period breakdown (days, weeks, monthes)</param>
        /// <param name="comparativePeriodType">Type Compartive Period</param>
        public MediaSchedulePeriod(DateTime begin, DateTime end, CstPeriod.DisplayLevel periodBreakDown, CstWeb.globalCalendar.comparativePeriodType comparativePeriodType)
            :this(begin, end, periodBreakDown)
        {
            _isComparativePeriod = true;
            _comparativePeriodType = comparativePeriodType;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determine the subPeriods items
        /// </summary>
        private void ComputePeriod()
        {

            bool is4M = true;
            DateTime today = DateTime.Now.Date;


            MediaScheduleSubPeriod subPeriodMain = null;
            MediaScheduleSubPeriod subPeriodAddOn = null;

            //Check if period is 4M
            if (_begin >= today.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            {
                is4M = true;
                subPeriodAddOn = new MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType.data_4m);
                subPeriodAddOn.AddSubPeriod(_begin, _end);
            }
            else
            {
                //Dayly detail 
                if ((_periodBreakDown == CstPeriod.DisplayLevel.dayly)
                    //uncomplete same month
                    || ((_periodBreakDown == CstPeriod.DisplayLevel.monthly) && (_begin.Year == _end.Year) && (_begin.Month == _end.Month) && ((_begin.Day > 1) || (_end.Day < _begin.AddDays(1 - _begin.Day).AddMonths(1).AddDays(-1).Day)))
                    //uncomplete deux following monthes
                    || ((_periodBreakDown == CstPeriod.DisplayLevel.monthly) && (_begin.AddDays(1 - _begin.Day) == _end.AddDays(1 - _end.Day).AddMonths(-1)) && (_begin.Day > 1) && (_end.Day < _end.AddDays(1 - _end.Day).AddMonths(1).AddDays(-1).Day))
                    //uncomplete same week
                    || ((_periodBreakDown == CstPeriod.DisplayLevel.weekly) && ((_begin.DayOfWeek != DayOfWeek.Monday || _end.DayOfWeek != DayOfWeek.Sunday) && GetWeekNumber(_end) - GetWeekNumber(_begin) < 1))
                    //uncomplete two following weeks
                    || ((_periodBreakDown == CstPeriod.DisplayLevel.weekly) && ((_begin.DayOfWeek != DayOfWeek.Monday) && (_end.DayOfWeek != DayOfWeek.Sunday) && GetWeekNumber(_end) - GetWeekNumber(_begin) < 2))
                    )
                {
                    subPeriodAddOn = new MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType.data);
                    subPeriodAddOn.AddSubPeriod(_begin, _end);
                }
                else
                {

                    bool isMonthBD = false;
                    DateTime tmpBegin = _begin;
                    DateTime tmpEnd = _end;
                    subPeriodAddOn = new MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType.data);

                    //at least one subPeriod is a complete week or a complete month
                    if (_periodBreakDown == CstPeriod.DisplayLevel.monthly)
                    {
                        subPeriodMain = new MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType.month);
                        isMonthBD = true;
                    }
                    else
                    {
                        subPeriodMain = new MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType.week);
                        isMonthBD = false;
                    }

                    //Join the day before the first complete month or week
                    if (isMonthBD && tmpBegin.Day != 1)
                    {
                        tmpBegin = tmpBegin.AddMonths(1).AddDays(-tmpBegin.AddMonths(1).Day);
                        subPeriodAddOn.AddSubPeriod(_begin, tmpBegin);
                        tmpBegin = tmpBegin.AddDays(1);
                    }
                    else if (!isMonthBD && tmpBegin.DayOfWeek != DayOfWeek.Monday)
                    {
                        if (tmpBegin.DayOfWeek != DayOfWeek.Sunday)
                            tmpBegin = tmpBegin.AddDays(7 - tmpBegin.DayOfWeek.GetHashCode());
                        subPeriodAddOn.AddSubPeriod(_begin, tmpBegin);
                        tmpBegin = tmpBegin.AddDays(1);
                    }

                    //Join the day after the last complete month or week
                    tmpEnd = _end;
                    if (isMonthBD)
                    {
                        if ((tmpEnd.Day != tmpEnd.AddMonths(1).AddDays(-tmpEnd.AddMonths(1).Day).Day)
                            || InLastTwoMonths(tmpEnd))
                        {
                            //Get Min between the last data update date and the day after the complete period suite
                            tmpEnd = Min(
                                today.AddMonths(-1).AddDays(1 - today.Day),
                                tmpEnd.AddDays(1 - tmpEnd.Day));
                            subPeriodAddOn.AddSubPeriod(tmpEnd, _end);
                            tmpEnd = tmpEnd.AddDays(-1);
                        }
                    }
                    else if (!isMonthBD)
                    {
                        if ((tmpEnd.DayOfWeek != DayOfWeek.Sunday)
                            || InLastTwoMonths(tmpEnd))
                        {

                            //Get Min between the last data update date and the day after the complete period suite
                            tmpEnd = Min(
                                today.AddDays(-35).AddDays(1 - today.DayOfWeek.GetHashCode()),
                                tmpEnd.AddDays(1 - tmpEnd.DayOfWeek.GetHashCode()));
                            subPeriodAddOn.AddSubPeriod(tmpEnd, _end);
                            tmpEnd = tmpEnd.AddDays(-1);
                        }
                    }


                    //Add complete periods suite
                    subPeriodMain.AddSubPeriod(tmpBegin, tmpEnd);

                }

            }
            if (subPeriodAddOn != null && subPeriodAddOn.Items.Count > 0)
                this._subPeriods.Add(subPeriodAddOn);
            if (subPeriodMain != null && subPeriodMain.Items.Count > 0)
                this._subPeriods.Add(subPeriodMain);
        }
        #endregion

        #region Compare two dates and return the smaller
        /// <summary>
        /// Compare two dates and return the smalelr
        /// </summary>
        /// <param name="date1">First Date Param</param>
        /// <param name="date2">Second Date Parm</param>
        /// <returns>Smaller Date</returns>
        public static DateTime Min(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) >= 0)
            {
                return date2;
            }
            return date1;
        }
        #endregion

        #region GetWeekNumber
        /// <summary>
        /// Get Week Number as YYYYWW
        /// </summary>
        /// <param name="date">Day to compute</param>
        /// <returns>YYYYWW</returns>
        public static int GetWeekNumber(DateTime date)
        {
            AtomicPeriodWeek week = new AtomicPeriodWeek(date);
            return week.Year * 100 + week.Week;
        }
        #endregion

        #region If date is in last two months
        /// <summary>
        /// If date is in last two months
        /// </summary>
        /// <param name="date">date</param>
        /// <returns></returns>
        private bool InLastTwoMonths(DateTime date) {

            DateTime today = DateTime.Now.Date;

            if ((date.Month == today.Month && date.Year == today.Year)
                || (date.Month == today.AddMonths(-1).Month && date.Year == today.AddMonths(-1).Year)
                || date > today)
                return true;

            return false;

        }
        #endregion

        #region Get Media Schedule comparative period
        /// <summary>
        /// Get Media Schedule Period Comparative
        /// </summary>
        /// <returns>Media Schedule Period (Null if comparative period is not used (false value)) </returns>
        public MediaSchedulePeriod GetMediaSchedulePeriodComparative() {

            DateTime beginComparative = DateTime.Now;
            DateTime endComparative = DateTime.Now;

            if (_isComparativePeriod) {
                SetComparativePeriod(beginComparative, endComparative);
                return new MediaSchedulePeriod(beginComparative, endComparative, _periodBreakDown);
            }

            return null;
        }
        #endregion

        #region Set Comparative Period
        /// <summary>
        /// Initialise la période comparative
        /// </summary>
        private void SetComparativePeriod(DateTime begin, DateTime end) {

            begin = GetPreviousYearDate(_begin.Date, _comparativePeriodType);
            end = GetPreviousYearDate(_end.Date, _comparativePeriodType);
        }
        #endregion

        #region GetPreviousYearDate
        /// <summary>
        /// Obtient la date de l'année précédente
        /// </summary>
        /// <param name="period">date de l'année en cours</param>
        /// <param name="comparativePeriodType">Type de la période comparative</param>
        /// <returns>date de l'année précédente</returns>
        private DateTime GetPreviousYearDate(DateTime date, Constantes.Web.globalCalendar.comparativePeriodType comparativePeriodType) {

            AtomicPeriodWeek tmpWeek;
            int currentDay;

            switch (comparativePeriodType) {

                case Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                    date = date.AddYears(-1);
                    break;        
                case Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    currentDay = date.DayOfWeek.GetHashCode();
                    tmpWeek = new AtomicPeriodWeek(date);
                    tmpWeek.SubWeek(52);
                    if (currentDay == 0)
                        date = tmpWeek.FirstDay.AddDays(6);
                    else
                        date = tmpWeek.FirstDay.AddDays(currentDay - 1);
                    break;
                default:
                    throw new ArgumentException("Comparative Period Type not valid !!!");
            }

            return date;

        }
        #endregion

    }
    #endregion

    #region MediaScheduleSubPeriod
    /// <summary>
    /// SubPeriod Breakdown entity
    /// </summary>
    public class MediaScheduleSubPeriod
    {

        #region Attributes
        /// <summary>
        /// SubPeriod type
        /// </summary>
        private CstPeriod.PeriodBreakdownType _type;
        /// <summary>
        /// List of items of SubPeriods
        /// </summary>
        private List<PeriodItem> _items = new List<PeriodItem>();
        #endregion

        #region Accessors
        /// <summary>
        /// Get type of subperiod items
        /// </summary>
        public CstPeriod.PeriodBreakdownType SubPeriodType
        {
            get { return _type; }
        }
        /// <summary>
        /// Get list of period items composing the sub period
        /// </summary>
        public List<PeriodItem> Items
        {
            get { return _items; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Period type</param>
        public MediaScheduleSubPeriod(CstPeriod.PeriodBreakdownType type)
        {
            _type = type;
        }
        #endregion

        #region AddSubPeriod
        /// <summary>
        /// Add a SubPeriod 
        /// </summary>
        /// <param name="begin">SubPeriod begin</param>
        /// <param name="end">SubPeriod end</param>
        public void AddSubPeriod(DateTime begin, DateTime end)
        {
            if (_type == CstPeriod.PeriodBreakdownType.month)
            {
                _items.Add(new PeriodItem(begin.Year * 100 + begin.Month, end.Year * 100 + end.Month));
            }
            else if (_type == CstPeriod.PeriodBreakdownType.week)
            {
                _items.Add(new PeriodItem(MediaSchedulePeriod.GetWeekNumber(begin), MediaSchedulePeriod.GetWeekNumber(end)));
            }
            else
            {
                _items.Add(new PeriodItem(begin.Year * 10000 + begin.Month * 100 + begin.Day, end.Year * 10000 + end.Month * 100 + end.Day));
            }
        }
        #endregion

        #region Equals
        /// <summary>
        /// Checks objects are equivalent
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <returns>True if objects are similar</returns>
        public override bool Equals(object obj)
        {
            MediaScheduleSubPeriod m = obj as MediaScheduleSubPeriod;
            if (m == null || this._type != m._type || m._items.Count != this._items.Count)
                return false;
            /*
             * Check contains in both sides as list does not garanty the unicity
             * 
             */
            foreach (PeriodItem p in m._items)
            {
                if (!this._items.Contains(p))
                    return false;
            }

            foreach (PeriodItem p in this._items)
            {
                if (!m._items.Contains(p))
                    return false;
            }

            return true;
        }
        #endregion
    }
    #endregion

    #region Period
    /// <summary>
    /// Period Item is a beginning and a end as YYYYMMDD, YYYYMM, YYYYWW
    /// </summary>
    public class PeriodItem
    {

        #region Attributes
        /// <summary>
        /// Period beginning
        /// </summary>
        private int _begin;
        /// <summary>
        /// Period end
        /// </summary>
        private int _end;

        #endregion

        #region Accessors
        /// <summary>
        /// Period beginning
        /// </summary>
        public int Begin
        {
            get { return _begin; }
        }
        /// <summary>
        /// Period End
        /// </summary>
        public int End
        {
            get { return _end; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="begin">SubPeriod begin as YYYYMMDD, YYYYMM, YYYYWW</param>
        /// <param name="end">SubPeriod end as YYYYMMDD, YYYYMM, YYYYWW</param>
        public PeriodItem(int begin, int end)
        {
            _begin = begin;
            _end = end;
        }
        #endregion

        #region Equals
        /// <summary>
        /// Check equality
        /// </summary>
        /// <param name="obj">object to check</param>
        /// <returns>True if both are equivalent PeriodItem objects</returns>
        public override bool Equals(object obj)
        {
            if (obj is PeriodItem)
            {
                PeriodItem p = (PeriodItem)obj;
                return (p._begin == this._begin && p._end == this._end);
            }
            return false;
        }
        #endregion

    }
    #endregion

}
