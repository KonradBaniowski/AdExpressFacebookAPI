#region Informations
// Auteur: G. Facon 
// Date de création: 02/06/2004 
// Date de modification: 02/06/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'analyse plan média
	/// </summary>
	public class MediaPlanAlertDataAccessException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPlanAlertDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPlanAlertDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPlanAlertDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
