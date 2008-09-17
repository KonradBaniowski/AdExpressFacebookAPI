#region Informations
// Auteur: B. Masson
// Date de création: 02/03/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception d'accès des données des types de client
	/// </summary>
	public class CustomerTypeDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CustomerTypeDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CustomerTypeDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CustomerTypeDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
