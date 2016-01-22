#region Informations
// Auteur: Y. Rkaina
// Date de création: 28/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Mnevis.Exceptions
{
	/// <summary>
	/// Description résumée de MnevisConfigDataAccessException.
	/// </summary>
	public class MnevisConfigDataAccessException:BaseException {

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MnevisConfigDataAccessException():base() {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public MnevisConfigDataAccessException(string message):base(message) {
	    }

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public MnevisConfigDataAccessException(string message, System.Exception innerException):base(message,innerException) {
		}


		#endregion
	}
}
