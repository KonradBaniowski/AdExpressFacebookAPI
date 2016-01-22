using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Utilities;
namespace TNS.AdExpressI.VP
{
    public class VeillePromoScheduleData
    {
        ///<summary>
        /// List of week with index in table result
        /// </summary>
        protected   Dictionary<long, long> _weekList = new Dictionary<long, long>();

        protected string _startDate = string.Empty;

        protected string _endDate = string.Empty;

        protected List<object[]> data = null;

        List<object[]> _data = new List<object[]>();

        public VeillePromoScheduleData(string startDate, string endDate, int nbLevels)
        {
            _startDate = startDate;
            _endDate = endDate;

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                //Get week list 
                DateTime periodBegin = DateString.YYYYMMDDToDateTime(startDate);
                DateTime periodEnd = DateString.YYYYMMDDToDateTime(endDate);

                //Set date list dictionary[ date week, index positon in result]
                AtomicPeriodWeek currentWeek = new AtomicPeriodWeek(periodBegin);
                AtomicPeriodWeek endWeek = new AtomicPeriodWeek(periodEnd);

                endWeek.Increment();

                int colPosition = nbLevels * 2;
                while (!(currentWeek.Week == endWeek.Week && currentWeek.Year == endWeek.Year))
                {
                    int month = Dates.GetMonthFromWeek(currentWeek.Year, currentWeek.Week);
                    _weekList.Add((currentWeek.Year * 100 + month ) *100 + currentWeek.Week, colPosition);
                    currentWeek.Increment();                    
                    colPosition++;
                }
            }
        }

        ///<summary>
        /// Get list of weeks in vp schedule
        /// </summary>
        public Dictionary<long, long> WeekList
        {
            get { return (_weekList); }
        }

        ///<summary>
        /// Get list of creatives presented in media schedule
        /// </summary>
        public List<object[]> Data
        {
            get { return (_data); }
        }
    }
}
