#region Informations
// Author: G. Facon
// Creation date: 29/10/2004 
// Modification date: 29/10/2004 (G. Facon)
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions {
	/// <summary>
	/// Thrown when is impossible to create media Items list used to determine an AdExpress universe
	/// </summary>
	public class MediaListException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public MediaListException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public MediaListException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public MediaListException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
