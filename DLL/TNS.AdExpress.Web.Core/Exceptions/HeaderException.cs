#region Informations
// Author: 
// Creation date: 
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Thrown when is impossible to load the data relating to the page headings
	/// </summary>
	public class HeaderException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public HeaderException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public HeaderException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public HeaderException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
