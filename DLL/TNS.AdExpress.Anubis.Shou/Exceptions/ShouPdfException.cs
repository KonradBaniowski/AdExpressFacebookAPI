#region Informations
// Auteur: D. Mussuma
// Date de création: 02/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Shou.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class ShouPdfException : BaseException {

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ShouPdfException():base()	{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ShouPdfException(string message):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public ShouPdfException(string message, System.Exception innerException):base(message,innerException)  {
		}


		#endregion
	}
}
