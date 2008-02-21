#region Informations
// Auteur: G. Facon
// Date de création: 12/08/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: Création de l'alerte concurrentielle
	/// </summary>
	public class CompetitorAlertCreationsResultsUIException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorAlertCreationsResultsUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorAlertCreationsResultsUIException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorAlertCreationsResultsUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
