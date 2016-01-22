#region Informations
// Author: Y. R'kaina
// Creation date: 08/09/2009
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Date.Exception {
    /// <summary>
    /// Active Media management
    /// </summary>
    class LastAvailableDateException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public LastAvailableDateException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public LastAvailableDateException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public LastAvailableDateException(string message, System.Exception innerException) : base(message, innerException)
        {
		}
		#endregion

    }
}
