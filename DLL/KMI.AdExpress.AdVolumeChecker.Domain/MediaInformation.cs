using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Media Information : TF1, M6
    /// </summary>
    public class MediaInformation {

        #region Variables
        /// <summary>
        /// Media Id
        /// </summary>
        private Int64 _id;
        /// <summary>
        /// Media Label
        /// </summary>
        private string _label;
        /// <summary>
        /// Top diffusion duration by days
        /// </summary>
        private Dictionary<string, Dictionary<string, Int64>> _topDiffusionByDays;
        /// <summary>
        /// List containing the day/slot for which we need to display a spot detail
        /// </summary>
        private List<string> _spotDetails;
        /// <summary>
        /// Duration Limit
        /// </summary>
        private Int64 _durationLimit;
        /// <summary>
        /// Average Duration Limit
        /// </summary>
        private Int64 _averageDurationLimit;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Media id</param>
        /// <param name="label">Media label</param>
        public MediaInformation(Int64 id, string label, Int64 durationLimit, Int64 averageDurationLimit) {
            _id = id;
            _label = label;
            _durationLimit = durationLimit;
            _averageDurationLimit = averageDurationLimit;
            InitTopDiffusionByDays();
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Media Id
        /// </summary>
        public Int64 Id {
            get { return (_id); }
        }
        /// <summary>
        /// Get Media Label
        /// </summary>
        public string Label {
            get { return (_label); }
        }
        /// <summary>
        /// Get Duration Limit
        /// </summary>
        public Int64 DurationLimit {
            get { return (_durationLimit); }
        }
        /// <summary>
        /// Get Average Duration Limit
        /// </summary>
        public Int64 AverageDurationLimit {
            get { return (_averageDurationLimit); }
        }
        /// <summary>
        /// Get Top Diffusion By Days
        /// </summary>
        public Dictionary<string, Dictionary<string, Int64>> TopDiffusionByDays {
            get { return (_topDiffusionByDays); }
        }
        /// <summary>
        /// Get Spot Details
        /// </summary>
        public List<string> SpotDetails {
            get { return (_spotDetails); }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Init Top Diffusion By Days
        /// </summary>
        private void InitTopDiffusionByDays() {

            List<string> daysOfWeek = new List<string>(new string[] { "SEMAINE", "LUNDI", "MARDI", "MERCREDI", "JEUDI", "VENDREDI", "SAMEDI", "DIMANCHE" });
            List<string> topDiffusionSlots = new List<string>(new string[] { "03H - 04H", "04H - 05H", "05H - 06H", "06H - 07H", "07H - 08H", "08H - 09H", "09H - 10H", "10H - 11H", "11H - 12H", "12H - 13H", "13H - 14H", "14H - 15H", "15H - 16H", "16H - 17H", "17H - 18H", "18H - 19H", "19H - 20H", "20H - 21H", "21H - 22H", "22H - 23H", "23H - 24H", "24H - 01H", "01H - 02H", "02H - 03H", "TOTAL", "MOYENNE" });
            Dictionary<string, Int64> topDiffusionInformations;
            _topDiffusionByDays = new Dictionary<string, Dictionary<string, long>>();
            _spotDetails = new List<string>();

            foreach (string day in daysOfWeek) {
                topDiffusionInformations = new Dictionary<string, long>();
                foreach (string slot in topDiffusionSlots)
                    topDiffusionInformations.Add(slot,0);

                _topDiffusionByDays.Add(day, topDiffusionInformations);
            }
        
        }
        #endregion

        #region Public Methods

        #region SetTopDiffusionByDays
        /// <summary>
        /// Set Top Diffusion By Days
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void SetTopDiffusionByDays(DataSet ds) {

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {

                foreach (DataRow dr in ds.Tables[0].Rows) {

                    string day = dr["JOUR"].ToString().TrimEnd();
                    string slot = dr["TRANCHE_HORAIRE"].ToString();
                    Int64 duration = Int64.Parse(dr["duration"].ToString());

                    if (duration > _durationLimit)
                        _spotDetails.Add(day + "_" + slot);

                    _topDiffusionByDays[day][slot] = duration;
                    _topDiffusionByDays["SEMAINE"][slot] += duration;
                    _topDiffusionByDays[day]["TOTAL"] += duration;
                    _topDiffusionByDays["SEMAINE"]["TOTAL"] += duration;
                    _topDiffusionByDays[day]["MOYENNE"] += duration;
                    _topDiffusionByDays["SEMAINE"]["MOYENNE"] += duration;
                }
            }

        }
        #endregion

        #region SetTopDiffusionDetailByDays
        /// <summary>
        /// Set Top Diffusion Detail By Days
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void SetTopDiffusionDetailByDays(DataSet ds, DataSet dsPreviousLastTopDiffusion) {

            string day = string.Empty;
            string slot = string.Empty;
            Int64 duration;

            if (dsPreviousLastTopDiffusion != null && dsPreviousLastTopDiffusion.Tables.Count > 0 && dsPreviousLastTopDiffusion.Tables[0] != null && dsPreviousLastTopDiffusion.Tables[0].Rows.Count > 0) {

                DataRow dr = dsPreviousLastTopDiffusion.Tables[0].Rows[dsPreviousLastTopDiffusion.Tables[0].Rows.Count - 1];
                day = dr["JOUR"].ToString().TrimEnd();
                slot = dr["TRANCHE_HORAIRE"].ToString();
                TopDiffusionManagement topDiffusion = new TopDiffusionManagement(int.Parse(dr["TOP_DIFFUSION"].ToString()), int.Parse(dr["duration"].ToString()), day, slot);
                duration = topDiffusion.GetDuration();

                if (topDiffusion.Exceeding) {

                    day = "LUNDI";
                    slot = "03H - 04H";
                    duration = topDiffusion.GetExceedingDuration();
                    SetTopDiffusionByDays(day, slot, duration);
                }
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {

                foreach (DataRow dr in ds.Tables[0].Rows) {

                    day = dr["JOUR"].ToString().TrimEnd();
                    slot = dr["TRANCHE_HORAIRE"].ToString();
                    TopDiffusionManagement topDiffusion = new TopDiffusionManagement(int.Parse(dr["TOP_DIFFUSION"].ToString()), int.Parse(dr["duration"].ToString()), day, slot);

                    duration = topDiffusion.GetDuration();

                    SetTopDiffusionByDays(day, slot, duration);

                    if (topDiffusion.Exceeding) {

                        day = topDiffusion.GetDay();

                        if (day.Length > 0) {
                            slot = topDiffusion.GetNextSlot();

                            duration = topDiffusion.GetExceedingDuration();

                            SetTopDiffusionByDays(day, slot, duration);
                        }

                    }
                }
            }

        }
        #endregion

        #region SetTopDiffusionByDays
        /// <summary>
        /// Set Top Diffusion By Days
        /// </summary>
        /// <param name="day">Day</param>
        /// <param name="slot">Slot</param>
        /// <param name="duration">Duration</param>
        private void SetTopDiffusionByDays(string day, string slot, Int64 duration) {

            _topDiffusionByDays[day][slot] += duration;
            _topDiffusionByDays["SEMAINE"][slot] += duration;
            _topDiffusionByDays[day]["TOTAL"] += duration;
            _topDiffusionByDays["SEMAINE"]["TOTAL"] += duration;
            _topDiffusionByDays[day]["MOYENNE"] += duration;
            _topDiffusionByDays["SEMAINE"]["MOYENNE"] += duration;

            if (_topDiffusionByDays[day][slot] > _durationLimit)
                if (!_spotDetails.Contains(day + "_" + slot))
                    _spotDetails.Add(day + "_" + slot);
        
        }
        #endregion

        #endregion

    }
}
