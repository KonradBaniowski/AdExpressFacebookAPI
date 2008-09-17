#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement de la configuration du plugin Geb (GebConfigDataAccess)
	/// </summary>
	public class GebConfigDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GebConfigDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GebConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public GebConfigDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
