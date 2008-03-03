#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
    /// <summary>
    /// Invalid xml attribute value
    /// </summary>
    public class InvalidXmlValueException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public InvalidXmlValueException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public InvalidXmlValueException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public InvalidXmlValueException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}

