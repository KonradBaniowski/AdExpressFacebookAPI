#region Informations
// Auteur: B. Masson 
// Date de création: 27/09/2004 
// Date de modification: 27/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Gad
	/// </summary>
	public class GadDataAccessException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GadDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GadDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public GadDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
