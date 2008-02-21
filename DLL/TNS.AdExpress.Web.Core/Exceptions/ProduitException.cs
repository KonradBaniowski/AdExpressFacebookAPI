#region Informations
// Author: G. Facon
// Creation date: 07/12/2004 
// Modification date: 07/12/2004 (G. Facon)
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{

	/// <summary>
	/// Thrown when is impossible to initialize product classification items Lists used in AdExpress
	/// </summary>
	public class ProduitException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ProduitException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ProduitException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public ProduitException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
