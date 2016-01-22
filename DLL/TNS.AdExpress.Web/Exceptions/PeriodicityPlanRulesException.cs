#region Informations
// Auteur: A.Dadouch
// Date de création: 25/07/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Rules: Périodicité du plan
	/// </summary>
	public class PeriodicityPlanRulesException:BaseException{
	
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PeriodicityPlanRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PeriodicityPlanRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PeriodicityPlanRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
