#region Informations
// Auteur: Guillaume Facon
// Date de création: 13/09/2004
// Date de modification: 13/09/2004
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Fonctions
	/// </summary>
	public class FunctionsDataAccessException:BaseException{
		
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public FunctionsDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public FunctionsDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public FunctionsDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
