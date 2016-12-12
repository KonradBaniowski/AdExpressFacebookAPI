using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.Classification.Exceptions {
	class UniverseLevelsDataAccessException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public UniverseLevelsDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public UniverseLevelsDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public UniverseLevelsDataAccessException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}
