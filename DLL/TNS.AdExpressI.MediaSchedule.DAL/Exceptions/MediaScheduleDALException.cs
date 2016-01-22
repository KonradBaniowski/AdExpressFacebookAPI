#region Information
/*
 * Author : G Ragneau
 * Created On : 05/05/2008
 * Modification:
 *      Date - Author - Description
 * 
 * 
 * */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.MediaSchedule.DAL.Exceptions {
	/// <summary>
	/// Media Schedule Data access exception
	/// </summary>
	public class MediaScheduleDALException : BaseException{

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public MediaScheduleDALException():base(){
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public MediaScheduleDALException(string message):base(message){
		}

		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
        public MediaScheduleDALException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}
