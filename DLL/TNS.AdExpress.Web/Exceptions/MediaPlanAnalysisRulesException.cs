#region Informations
// Auteur: G. Facon 
// Date de création: 28/10/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Metier: Classe d'exception de la génération des données pour l'analyse plan média
	/// </summary>
	public class MediaPlanAnalysisRulesException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPlanAnalysisRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPlanAnalysisRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPlanAnalysisRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
