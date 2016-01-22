#region Informations
// Auteur: B. Masson 
// Date de création: 27/09/2004 
// Date de modification: 27/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// System: Gad
	/// </summary>
	public class GadSystemException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GadSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GadSystemException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public GadSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
