#region Informations
// Author: G. Facon 
// Creation date: 15/06/2004 
// Modification date: 15/06/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Thrown when problem occurred during connections with the database in management of the universes
	/// </summary>
	public class MySessionDataAccessException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public MySessionDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public MySessionDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public MySessionDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
