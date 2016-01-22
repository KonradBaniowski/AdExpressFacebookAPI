#region Informations
// Auteur: A.DADOUCH
// Date de création:22/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Rules: Info/News
	/// </summary>
	public class PdfFilesSystemException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PdfFilesSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PdfFilesSystemException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PdfFilesSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
