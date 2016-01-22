#region Informations
// Author: B.Masson
// Creation date: 26/01/2007
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{

	/// <summary>
	/// Thrown when is impossible to get the data of a module group
	/// </summary>
	public class ModuleCategoryListXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ModuleCategoryListXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ModuleCategoryListXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error messager</param>
		/// <param name="innerException">Inner Exception</param>
        public ModuleCategoryListXLException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}
