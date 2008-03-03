#region Informations
// Author: G. Facon
// Creation date: 03/03/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
    /// <summary>
    /// Default connection Exception
    /// </summary>
    public class DefaultConnectionException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public DefaultConnectionException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public DefaultConnectionException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public DefaultConnectionException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}
