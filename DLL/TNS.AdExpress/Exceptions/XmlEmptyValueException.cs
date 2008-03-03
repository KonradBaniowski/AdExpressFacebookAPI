#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
    /// <summary>
    /// The value of the xml attribute is empty
    /// </summary>
    public class XmlEmptyValueException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public XmlEmptyValueException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public XmlEmptyValueException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public XmlEmptyValueException(string message, System.Exception innerException): base(message, innerException){
		}
		#endregion

    }
}

