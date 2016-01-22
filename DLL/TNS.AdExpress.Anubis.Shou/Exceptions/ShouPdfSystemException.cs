#region Informations
// Auteur: D. Mussuma
// Date de création: 06/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Shou.Exceptions
{
	/// <summary>
	/// Description résumée de ShouPdfSystemException.
	/// </summary>
	public class ShouPdfSystemException : BaseException
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ShouPdfSystemException():base()	{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ShouPdfSystemException(string message):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public ShouPdfSystemException(string message, System.Exception innerException):base(message,innerException)  {
		}


		#endregion
	}
}
