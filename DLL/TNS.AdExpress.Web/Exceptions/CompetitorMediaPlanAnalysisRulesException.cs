#region Informations
// Auteur: G.Facon
// Date de création:
// Date de modification: 23/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la classe metier pour le plan média concurentiel
	/// </summary>
	public class CompetitorMediaPlanAnalysisRulesException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorMediaPlanAnalysisRulesException():base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorMediaPlanAnalysisRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorMediaPlanAnalysisRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
