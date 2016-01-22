#region Informations
// Auteur: G. Ragneau
// Date de création: 24/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Appm.Exceptions{
	/// <summary>
	/// Exception
	/// </summary>
	public class AppmPdfException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AppmPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AppmPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public AppmPdfException(string message, System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}