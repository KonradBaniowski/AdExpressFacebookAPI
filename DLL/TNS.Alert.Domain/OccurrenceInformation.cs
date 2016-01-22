using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Alert.Domain {

    public class OccurrenceInformation {

        #region Variables
        /// <summary>
        /// Nb day of expiration
        /// </summary>
        private Int32 _dayExpiration = -1;
        /// <summary>
        /// Use for Delete or not in Garbage
        /// </summary>
        private bool _delete = false;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dayExpiration">Nb day of expiration</param>
        /// <param name="delete">Use for Delete or not in Garbage</param>
        public OccurrenceInformation(Int32 dayExpiration, bool delete) {
            if (dayExpiration <= 0) throw new ArgumentException("dayExpiration parameter can't to be negative or equal to 0 in OccurrenceInformation");
            this._dayExpiration = dayExpiration;
            this._delete = delete;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Nb day of expiration to Add on date Current
        /// </summary>
        public Int32 DayExpiration {
            get { return (this._dayExpiration); }
        }
        /// <summary>
        /// Get Use for Delete or not in Garbage
        /// </summary>
        public bool Delete {
            get { return (this._delete); }
        }
        #endregion

        
    }
}
