#region Informations
// Author: Y. R'kaina
// Creation date: 14/09/2009
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.Domain.Exceptions {

    class ReportingSystemException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ReportingSystemException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ReportingSystemException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public ReportingSystemException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}
