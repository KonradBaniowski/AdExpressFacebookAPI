#region Informations
// Auteur: D. V. Mussuma
// Date de création: 29/05/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Satet.Exceptions{
	/// <summary>
	/// Classe d'exception d'accès des données Appm
	/// </summary>
	public class SatetDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SatetDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SatetDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SatetDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
