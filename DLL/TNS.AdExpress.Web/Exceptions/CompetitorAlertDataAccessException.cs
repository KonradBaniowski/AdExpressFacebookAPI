#region Informations
// Auteur: G. Facon 
// Date de création: 13/09/2004 
// Date de modification: 13/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'alert concurrentielle
	/// </summary>
	public class CompetitorAlertDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorAlertDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorAlertDataAccessException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorAlertDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
