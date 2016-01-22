#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 14/02/2007 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la g�n�ration des donn�es pour l'alerte mail des cr�ations 
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
