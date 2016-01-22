#region Informations
// Auteur: D. Mussuma
// Date de création: 26/04/2012
#endregion

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Apis.Exceptions{
	/// <summary>
	/// Exception
	/// </summary>
	public class ApisPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ApisPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ApisPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public ApisPdfException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
