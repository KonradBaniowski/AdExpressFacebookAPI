#region Informations
// Auteur: A.Dadouch
// Date de création: 01/08/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: Périodicité du plan
	/// </summary>
	public class PeriodicityPlanChartUIException:BaseException{

		#region Constructeurs		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PeriodicityPlanChartUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PeriodicityPlanChartUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PeriodicityPlanChartUIException(string message,System.Exception innerException):base(message,innerException){
	}
		#endregion

	}
}
