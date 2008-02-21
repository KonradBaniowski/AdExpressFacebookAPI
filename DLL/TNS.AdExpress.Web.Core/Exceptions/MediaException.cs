#region Informations
// Author: G. Facon
// Creation date: 29/10/2004 
// Modification date: 29/10/2004 (G. Facon)
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{

	/// <summary>
	/// Thrown when is impossible to initialize media classification items Lists used in AdExpress
	/// </summary>
	public class MediaException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public MediaException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public MediaException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public MediaException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
