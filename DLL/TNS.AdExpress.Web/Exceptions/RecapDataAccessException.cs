#region Informations
// Auteur: G. Facon
// Date de création: 04/01/05
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;


namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	///  Base de données: Analyses sectorielles
	/// </summary>
	public class RecapDataAccessException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RecapDataAccessException(): base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RecapDataAccessException(string message): base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RecapDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
