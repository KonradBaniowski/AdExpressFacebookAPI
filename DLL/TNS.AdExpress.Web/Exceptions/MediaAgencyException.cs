#region Informations
// Auteur : A.Obermeyer
// Date de création : 19/04/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{

	/// <summary>
	/// Classe d'exception pour le dataAccess agence média
	/// </summary>
	public class MediaAgencyException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaAgencyException():base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaAgencyException(string message):base(message){			
		}
	
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaAgencyException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
