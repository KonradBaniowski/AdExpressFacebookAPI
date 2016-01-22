#region Informations
// Author: 
// Creation date: 
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// Thrown when is impossible to load modules and modules groups data information
	/// </summary>
	public class ModuleException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ModuleException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ModuleException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public ModuleException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
