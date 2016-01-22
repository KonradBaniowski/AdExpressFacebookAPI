#region Informations
// Auteur: G. Facon 
// Date de création: 23/11/2004 
// Date de modification: 23/11/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception accès aux fonctionnalités alert concurrentielle
	/// </summary>
	public class CompetitorSystemException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorSystemException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
