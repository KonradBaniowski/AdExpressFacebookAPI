using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.DataLoader;

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
        /// <summary>
        /// Encrypted Media
        /// </summary>
        private bool _encryptedMedia = false;
        /// <summary>
        /// Unencrypted Slots File Path
        /// </summary>
        private string _unencryptedSlotsFilePath = string.Empty;
        /// <summary>
        /// Unencrypted Slots
        /// </summary>
        private Dictionary<string, List<UnencryptedSlot>> _unencryptedSlots = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Media id</param>
        /// <param name="label">Media label</param>
        /// <param name="durationLimit">Duration Limit</param>
        /// <param name="averageDurationLimit">Average Duration Limit</param>
        /// <param name="encryptedMedia">Encrypted Media</param>
        /// <param name="unencryptedSlotsFilePath">Unencrypted Slots File Path</param>
        public MediaInformation(Int64 id, string label, Int64 durationLimit, Int64 averageDurationLimit, bool encryptedMedia, string unencryptedSlotsFilePath) {
            _id = id;
            _label = label;
            _durationLimit = durationLimit;
            _averageDurationLimit = averageDurationLimit;
            _encryptedMedia = encryptedMedia;
            _unencryptedSlotsFilePath = unencryptedSlotsFilePath;
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
        /// <summary>
        /// Get Unencrypted Slots
        /// </summary>
        public Dictionary<string, List<UnencryptedSlot>> UnencryptedSlots {
            get { return (_unencryptedSlots); }
        }
        /// <summary>
        /// Get Encrypted Media
        /// </summary>
        public bool EncryptedMedia {
            get {
                if (_unencryptedSlots == null)
                    return false;
                return (_encryptedMedia); 
            }
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
        public void SetTopDiffusionDetailByDays(DataSet ds, DataSet dsPreviousLastTopDiffusion, DateTime startDate, DateTime endDate) {

            string day = string.Empty;
            string slot = string.Empty;
            int duration;

            if (_encryptedMedia)
                _unencryptedSlots = UnencryptedSlotsDL.Load(_unencryptedSlotsFilePath, startDate, endDate);

            // Détection du dépassement pour la tranche ramenée de la veille de startDate (expl : si startDate = 18/01 alors vérif de la tranche 02-03h du 17/01)
            // Ne peut concerner qu'un dimanche d'où la transposition en Lundi
            // Utile que pour la méthode AdVolumeCheckerDAL.GetPreviousLastTopDiffusionData
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
                    SetTopDiffusionByDays(day, slot, duration, GetTopDiffusionTimeSpan(slot));
                }
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {

                foreach (DataRow dr in ds.Tables[0].Rows) {

                    day = dr["JOUR"].ToString().TrimEnd();
                    slot = dr["TRANCHE_HORAIRE"].ToString();
                    TopDiffusionManagement topDiffusion = new TopDiffusionManagement(int.Parse(dr["TOP_DIFFUSION"].ToString()), int.Parse(dr["duration"].ToString()), day, slot);

                    duration = topDiffusion.GetDuration();

                    // permet le calcul durée par tranche / jour
                    SetTopDiffusionByDays(day, slot, duration, topDiffusion.TopDiffusionTimeSpan);

                    // permet d'indiquer un dépassement de durée sur la tranche concernée
                    if (topDiffusion.Exceeding) {

                        day = topDiffusion.GetDay();

                        if (day.Length > 0) {
                            slot = topDiffusion.GetNextSlot();

                            duration = topDiffusion.GetExceedingDuration();

                            SetTopDiffusionByDays(day, slot, duration, GetTopDiffusionTimeSpan(slot));
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
        private void SetTopDiffusionByDays(string day, string slot, int duration, TimeSpan topDiffusion) {

            //if (!EncryptedMedia || (EncryptedMedia && CheckTopDiffusionInUnencryptedSlot(day, topDiffusion, ref duration))) {
                _topDiffusionByDays[day][slot] += duration;
                _topDiffusionByDays["SEMAINE"][slot] += duration;
                _topDiffusionByDays[day]["TOTAL"] += duration;
                _topDiffusionByDays["SEMAINE"]["TOTAL"] += duration;
                _topDiffusionByDays[day]["MOYENNE"] += duration;
                _topDiffusionByDays["SEMAINE"]["MOYENNE"] += duration;

                if (_topDiffusionByDays[day][slot] > _durationLimit)
                    if (!_spotDetails.Contains(day + "_" + slot))
                        _spotDetails.Add(day + "_" + slot);
            //}

        }
        #endregion

        #region Get Top Diffusion Time Span
        /// <summary>
        /// Get Top Diffusion Time Span
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <returns>Top Diffusion time Span</returns>
        private TimeSpan GetTopDiffusionTimeSpan(string slot) {

            switch (slot) {
                case "03H - 04H": return new TimeSpan(3, 0, 0);
                case "04H - 05H": return new TimeSpan(4, 0, 0);
                case "05H - 06H": return new TimeSpan(5, 0, 0);
                case "06H - 07H": return new TimeSpan(6, 0, 0);
                case "07H - 08H": return new TimeSpan(7, 0, 0);
                case "08H - 09H": return new TimeSpan(8, 0, 0);
                case "09H - 10H": return new TimeSpan(9, 0, 0);
                case "10H - 11H": return new TimeSpan(10, 0, 0);
                case "11H - 12H": return new TimeSpan(11, 0, 0);
                case "12H - 13H": return new TimeSpan(12, 0, 0);
                case "13H - 14H": return new TimeSpan(13, 0, 0);
                case "14H - 15H": return new TimeSpan(14, 0, 0);
                case "15H - 16H": return new TimeSpan(15, 0, 0);
                case "16H - 17H": return new TimeSpan(16, 0, 0);
                case "17H - 18H": return new TimeSpan(17, 0, 0);
                case "18H - 19H": return new TimeSpan(18, 0, 0);
                case "19H - 20H": return new TimeSpan(19, 0, 0);
                case "20H - 21H": return new TimeSpan(20, 0, 0);
                case "21H - 22H": return new TimeSpan(21, 0, 0);
                case "22H - 23H": return new TimeSpan(22, 0, 0);
                case "23H - 24H": return new TimeSpan(23, 0, 0);
                case "24H - 01H": return new TimeSpan(24, 0, 0);
                case "01H - 02H": return new TimeSpan(1, 0, 0);
                case "02H - 03H": return new TimeSpan(2, 0, 0);
                default: return new TimeSpan(0, 0, 0);
            }

        }
        #endregion

        #region Check Top diffusion in Unencrypted Slot
        /// <summary>
        /// Check Top diffusion in Unencrypted Slot
        /// </summary>
        /// <param name="day">Day</param>
        /// <param name="topDiffusion">Top Diffusion</param>
        /// <returns></returns>
        public bool CheckTopDiffusionInUnencryptedSlot(string day, TimeSpan topDiffusion, ref int duration) {

            foreach (UnencryptedSlot slot in _unencryptedSlots[day]) {

                TimeSpan start = new TimeSpan(slot.SlotStart.Hour, slot.SlotStart.Minute, slot.SlotStart.Second);
                TimeSpan end = new TimeSpan(slot.SlotEnd.Hour, slot.SlotEnd.Minute, slot.SlotEnd.Second);
                int totalSeconds = duration;
                int seconds = totalSeconds % 60;
                int minutes = totalSeconds / 60;
                TimeSpan durationTime = new TimeSpan(0, minutes, seconds);
                TimeSpan topDiffusionAfterDuration = topDiffusion + durationTime;

                if (topDiffusion >= start && topDiffusion <= end && topDiffusionAfterDuration >= start && topDiffusionAfterDuration <= end) {
                    return true;
                }
                else if (topDiffusion < start && topDiffusionAfterDuration > start && topDiffusionAfterDuration <= end) {
                    duration = (topDiffusionAfterDuration - start).Seconds;
                    return true;
                }
                else if (topDiffusion >= start && topDiffusion <= end && topDiffusionAfterDuration > end) {
                    duration = (end - topDiffusion).Seconds;
                    return true;
                }
                else if (topDiffusion < start && topDiffusionAfterDuration > end) {
                    duration = (end - start).Seconds;
                    return true;
                }

            }

            return false;
        }
        #endregion

        #endregion

    }
}
