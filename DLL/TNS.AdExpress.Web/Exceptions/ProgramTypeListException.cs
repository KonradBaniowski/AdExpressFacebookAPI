#region Informations
// Auteur: Y. R'kaina
// Date de création:
// Date de modification: 28/11/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// Description résumée de ProgramTypeListException.
	/// </summary>
	public class ProgramTypeListException:BaseException {

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProgramTypeListException():base() {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProgramTypeListException(string message):base(message) {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ProgramTypeListException(string message,System.Exception innerException):base(message,innerException) {
		}
		#endregion

	}
}
