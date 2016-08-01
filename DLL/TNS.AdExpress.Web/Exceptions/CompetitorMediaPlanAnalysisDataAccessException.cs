#region Informations
// Auteur: ?
// Date de cr�ation:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la g�n�ration des donn�es pour l'analyse plan m�dia concurentiel
	/// </summary>
	public class CompetitorMediaPlanAnalysisDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorMediaPlanAnalysisDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorMediaPlanAnalysisDataAccessException(string message):base(message){	
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorMediaPlanAnalysisDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}