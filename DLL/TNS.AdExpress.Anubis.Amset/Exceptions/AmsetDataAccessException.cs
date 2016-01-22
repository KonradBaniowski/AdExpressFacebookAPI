#region Informations
// Auteur: Y. R'kaina
// Date de création: 05/02/2007
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Amset.Exceptions{
	/// <summary>
	/// Classe d'exception d'accès des données Amset
	/// </summary>
	public class AmsetDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AmsetDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AmsetDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AmsetDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
