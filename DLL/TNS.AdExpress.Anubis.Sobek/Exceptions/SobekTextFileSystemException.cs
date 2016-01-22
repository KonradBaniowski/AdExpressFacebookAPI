#region Informations
// Auteur: D. V. Mussuma, Y. Rkaina
// Date de création: 23/05/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Sobek.Exceptions{
	/// <summary>
	/// Description résumée de SobekTextFileSystemException.
	/// </summary>
	public class SobekTextFileSystemException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SobekTextFileSystemException():base(){
	    }

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SobekTextFileSystemException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SobekTextFileSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
