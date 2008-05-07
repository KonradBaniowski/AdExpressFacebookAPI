#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.MediaSchedule.Exceptions{
	/// <summary>
	/// Media Schedule Report Exception
	/// </summary>
	public class MediaScheduleException : BaseException{

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public MediaScheduleException():base(){
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public MediaScheduleException(string message):base(message){
		}

		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
        public MediaScheduleException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}
