#region Informations
// Auteur: A.Dadouch
// Date de création: 25/07/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Périodicité du plan
	/// </summary>
	public class PeriodicityPlanDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PeriodicityPlanDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PeriodicityPlanDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PeriodicityPlanDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
