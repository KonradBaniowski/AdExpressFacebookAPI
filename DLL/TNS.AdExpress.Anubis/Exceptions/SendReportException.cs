#region Informations
// Auteur: Y. R'kaina
// Date de création: 08/03/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Description résumée de SendReportException.
	/// </summary>
	public class SendReportException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SendReportException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public SendReportException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SendReportException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
