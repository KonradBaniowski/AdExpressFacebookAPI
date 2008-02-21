#region Informations
// Auteur: G. Ragneau
// Date de création: 24/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception lors de l'exécution d'une fontions tools
	/// </summary>
	public class FunctionsException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public FunctionsException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public FunctionsException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public FunctionsException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion
	}
}