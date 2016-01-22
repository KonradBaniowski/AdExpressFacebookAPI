#region Informations
// Auteur: D. Mussuma
// Date de création: 02/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Shou.Exceptions
{
	/// <summary>
	/// Gère les exceptions de la classe de configuration
	/// </summary>
	public class ShouConfigDataAccessException : BaseException {

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ShouConfigDataAccessException():base() {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ShouConfigDataAccessException(string message):base(message) {
	    }

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public ShouConfigDataAccessException(string message, System.Exception innerException):base(message,innerException) {
		}


		#endregion
	}
}
