#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
    /// <summary>
    /// The value of the xml attribute is null
    /// </summary>
    public class XmlNullValueException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public XmlNullValueException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public XmlNullValueException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public XmlNullValueException(string message, System.Exception innerException): base(message, innerException){
		}
		#endregion

    }
}
