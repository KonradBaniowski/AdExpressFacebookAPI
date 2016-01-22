#region Informations
// Auteur: D. V. Mussuma
// Date de création: 29/05/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Tefnout.Exceptions{
	/// <summary>
	/// Classe d'exception d'accès des données Appm
	/// </summary>
	public class TefnoutDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TefnoutDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TefnoutDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TefnoutDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
