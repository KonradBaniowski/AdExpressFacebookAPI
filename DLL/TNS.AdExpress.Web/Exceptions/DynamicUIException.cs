#region Informations
// Auteur: G. Facon 
// Date de création: 09/11/2004 
// Date de modification: 09/11/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DynamicUIException thrown whenever an error occured while generating UI for dynamic analysis module
	/// </summary>
	public class DynamicUIException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DynamicUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DynamicUIException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DynamicUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
