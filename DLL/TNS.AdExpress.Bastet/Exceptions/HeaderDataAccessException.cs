#region Informations
// Auteur: B. Masson
// Date de création: 16/11/2005
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement du xml pour le header
	/// </summary>
	public class HeaderDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public HeaderDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public HeaderDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public HeaderDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
