using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
    /// <summary>
    /// Active Banners Format management
    /// </summary>
    class ActiveBannersFormatListDataAccessException : BaseException{
        
        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ActiveBannersFormatListDataAccessException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ActiveBannersFormatListDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public ActiveBannersFormatListDataAccessException(string message, System.Exception innerException) : base(message, innerException)
        {
		}
		#endregion

    }
}
