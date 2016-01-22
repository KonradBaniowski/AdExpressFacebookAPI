#region Informations
// Auteur: Y. R'kaina
// Date de création: 09/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Aton.Exceptions{
	/// <summary>
	/// Description résumée de AtonPdfException.
	/// </summary>
	public class AtonPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AtonPdfException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AtonPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public AtonPdfException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
