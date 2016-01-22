using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
    /// <summary>
    /// Vehicle Format List DataAccess Exception
    /// </summary>
    class VehicleFormatListDataAccessException : BaseException{
        
        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public VehicleFormatListDataAccessException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public VehicleFormatListDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public VehicleFormatListDataAccessException(string message, System.Exception innerException) : base(message, innerException)
        {
		}
		#endregion

    }
}
