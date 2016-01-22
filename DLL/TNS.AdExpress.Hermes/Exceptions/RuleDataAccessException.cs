#region Informations
// Auteur : B.Masson
// Date de cr�ation : 12/02/2007
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Hermes.Exceptions{
	/// <summary>
	/// Classe d'exception des acc�s aux donn�es pour les r�gles
	/// </summary>
	public class RuleDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RuleDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RuleDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public RuleDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
