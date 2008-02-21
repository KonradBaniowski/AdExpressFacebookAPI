#region Informations
// Auteur: G. RAGNEAU
// Date de cr�ation: 17/07/2006
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// VersionDataAccessException thrown whenever an error occured in VersionDataAccess
	/// </summary>
	public class VersionDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VersionDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public VersionDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public VersionDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
