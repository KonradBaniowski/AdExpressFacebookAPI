using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Unencrypted Slot
    /// </summary>
    public class UnencryptedSlot {

        #region Variables
        /// <summary>
        /// Id
        /// </summary>
        private int _id;
        /// <summary>
        /// Slot Start
        /// </summary>
        private DateTime _slotStart;
        /// <summary>
        /// Slot End
        /// </summary>
        private DateTime _slotEnd;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="slotStart">Slot Start</param>
        /// <param name="slotEnd">Slot End</param>
        public UnencryptedSlot(int id, DateTime slotStart, DateTime slotEnd) {
            _id = id;
            _slotStart = slotStart;
            _slotEnd = slotEnd;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Id
        /// </summary>
        public int Id {
            get { return (_id); }
        }
        /// <summary>
        /// Get Slot Start
        /// </summary>
        public DateTime SlotStart {
            get { return (_slotStart); }
        }
        /// <summary>
        /// Get Slot End
        /// </summary>
        public DateTime SlotEnd {
            get { return (_slotEnd); }
        }
        #endregion

    }
}
