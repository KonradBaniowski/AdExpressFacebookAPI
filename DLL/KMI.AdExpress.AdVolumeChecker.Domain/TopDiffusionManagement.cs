using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Top Diffusion Management
    /// </summary>
    public class TopDiffusionManagement {

        #region Variables
        /// <summary>
        /// Hours
        /// </summary>
        private int _hours = 0;
        /// <summary>
        /// Minutes
        /// </summary>
        private int _minutes = 0;
        /// <summary>
        /// Seconds
        /// </summary>
        private int _seconds = 0;
        /// <summary>
        /// Upper Bound
        /// </summary>
        private TimeSpan _upperBound = new TimeSpan(0, 0, 0);
        /// <summary>
        /// Midnight Case
        /// </summary>
        private bool _midnightCase = false;
        /// <summary>
        /// Duration
        /// </summary>
        private int _duration = 0;
        /// <summary>
        /// Exceeding Duration
        /// </summary>
        private int _exceedingDuration = 0;
        /// <summary>
        /// Exceeding
        /// </summary>
        public bool _exceeding = false;
        /// <summary>
        /// Day
        /// </summary>
        private string _day = string.Empty;
        /// <summary>
        /// Last Slot
        /// </summary>
        private bool _lastSlot = false;
        /// <summary>
        /// Slot
        /// </summary>
        private string _slot = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get Exceeding
        /// </summary>
        public bool Exceeding {
            get { return (_exceeding); }
        }
        public TimeSpan TopDiffusionTimeSpan {
            get { return new TimeSpan(_hours, _minutes, _seconds); }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Contructor
        /// </summary>
        public TopDiffusionManagement(int topDiffusion, int duration, string day, string slot) {

            _hours = int.Parse(topDiffusion.ToString("000000").Substring(0, 2));
            _minutes = int.Parse(topDiffusion.ToString("000000").Substring(2, 2));
            _seconds = int.Parse(topDiffusion.ToString("000000").Substring(4, 2));
            _duration = duration;
            _day = day;
            _slot = slot;

            if (_hours < 24)
                _upperBound = new TimeSpan((_hours + 1), 0, 0);
            else
                _midnightCase = true;

            if (_hours == 2)
                _lastSlot = true;

        }
        #endregion

        #region Public Methods

        #region Get Duration
        /// <summary>
        /// Get Duration
        /// </summary>
        /// <returns></returns>
        public int GetDuration() { 
            
            TimeSpan topDiffusionTime = new TimeSpan(_hours, _minutes, _seconds);
            int totalSeconds = _duration;
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;
            TimeSpan durationTime = new TimeSpan(0,minutes, seconds);
            TimeSpan result = topDiffusionTime + durationTime;

            if (!_midnightCase && result > _upperBound) {
                _exceeding = true;
                return Convert.ToInt32((_upperBound - topDiffusionTime).TotalSeconds);
            }
            else
                return _duration;

        }
        #endregion

        #region Get Exceeding Duration
        /// <summary>
        /// Get Exceeding Duration
        /// </summary>
        /// <returns></returns>
        public int GetExceedingDuration() {

            TimeSpan topdiffusionTime = new TimeSpan(_hours, _minutes, _seconds);
            int totalSeconds = _duration;
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;
            TimeSpan durationTime = new TimeSpan(0, minutes, seconds);
            TimeSpan result = topdiffusionTime + durationTime;
            
            _exceedingDuration = Convert.ToInt32((result - _upperBound).TotalSeconds);

            return _exceedingDuration;
        }
        #endregion

        #region GetDay
        /// <summary>
        /// Get Day
        /// </summary>
        /// <returns>Day</returns>
        public string GetDay() {

            if (!_lastSlot)
                return _day;
            else {
                switch (_day) { 
                    case "LUNDI"    : return "MARDI";
                    case "MARDI"    : return "MERCREDI";
                    case "MERCREDI" : return "JEUDI";
                    case "JEUDI"    : return "VENDREDI";
                    case "VENDREDI" : return "SAMEDI";
                    case "SAMEDI"   : return "DIMANCHE";
                    case "DIMANCHE" : return "";
                }
            }

            return "";
        }
        #endregion

        #region GetNextSlot
        /// <summary>
        /// Get Next Slot
        /// </summary>
        /// <returns>Next Slot</returns>
        public string GetNextSlot() {

            switch (_slot) {
                case "03H - 04H": return "04H - 05H";
                case "04H - 05H": return "05H - 06H";
                case "05H - 06H": return "06H - 07H";
                case "06H - 07H": return "07H - 08H";
                case "07H - 08H": return "08H - 09H";
                case "08H - 09H": return "09H - 10H";
                case "09H - 10H": return "10H - 11H";
                case "10H - 11H": return "11H - 12H";
                case "11H - 12H": return "12H - 13H";
                case "12H - 13H": return "13H - 14H";
                case "13H - 14H": return "14H - 15H";
                case "14H - 15H": return "15H - 16H";
                case "15H - 16H": return "16H - 17H";
                case "16H - 17H": return "17H - 18H";
                case "17H - 18H": return "18H - 19H";
                case "18H - 19H": return "19H - 20H";
                case "19H - 20H": return "20H - 21H";
                case "20H - 21H": return "21H - 22H";
                case "21H - 22H": return "22H - 23H";
                case "22H - 23H": return "23H - 24H";
                case "23H - 24H": return "24H - 01H";
                case "24H - 01H": return "01H - 02H";
                case "01H - 02H": return "02H - 03H";
                case "02H - 03H": return "03H - 04H";
            }

            return "";
        }
        #endregion

        #endregion


    }
}
