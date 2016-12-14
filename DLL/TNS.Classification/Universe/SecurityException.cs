#region Informations
// Author: G. Facon
// Creation date: 26/10/2007
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.Classification.Universe {

	/// <summary>
	/// Security Exception
	/// </summary>
	public class SecurityException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public SecurityException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public SecurityException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public SecurityException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}