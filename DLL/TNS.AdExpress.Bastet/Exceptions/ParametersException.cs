#region Informations
// Auteur: B. Masson
// Date de cr�ation: 16/11/2005
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception d'acc�s des donn�es de liste de logins
	/// </summary>
	public class ParametersException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ParametersException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ParametersException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ParametersException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
