#region Informations
// Auteur: G.Facon
// Date de cr�ation:
// Date de modification: 23/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la classe metier pour l'alert plan m�dia concurentiel
	/// </summary>
	public class CompetitorMediaPlanAlertRulesException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorMediaPlanAlertRulesException():base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorMediaPlanAlertRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorMediaPlanAlertRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}