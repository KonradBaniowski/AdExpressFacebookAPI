#region Informations
// Auteur: Y. R'kaina
// Date de création: 09/02/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Aton.Exceptions{
	/// <summary>
	/// Exception
	/// </summary>
	public class AtonConfigDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AtonConfigDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AtonConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public AtonConfigDataAccessException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
