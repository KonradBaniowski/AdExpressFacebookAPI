using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
	class SearchLevelDataAccessException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public SearchLevelDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public SearchLevelDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public SearchLevelDataAccessException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}
