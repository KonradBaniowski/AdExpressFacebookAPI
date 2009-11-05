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
	public class ModulesListXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ModulesListXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ModulesListXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public ModulesListXLException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion
	}
}
