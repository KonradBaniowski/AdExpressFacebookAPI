#region Informations
// Auteur: Y. R'kaina
// Date de création: 08/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Amset.Exceptions{
	/// <summary>
	/// Description résumée de AmsetExcelSystemException.
	/// </summary>
	public class AmsetExcelSystemException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AmsetExcelSystemException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AmsetExcelSystemException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public AmsetExcelSystemException(string message, System.Exception innerException):base(message,innerException){
		}	
		#endregion
	}
}
