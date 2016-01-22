#region Informations
// Auteur: D. Mussuma
// Date de création: 14/02/2007 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'alerte mail des créations 
	/// </summary>
	public class AlertsInsertionsCreationsDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AlertsInsertionsCreationsDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AlertsInsertionsCreationsDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AlertsInsertionsCreationsDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
