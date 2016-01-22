#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 26/01/2007 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Description r�sum�e de SeasonalityPlanChartUIException.
	/// </summary>
	public class SeasonalityPlanChartUIException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SeasonalityPlanChartUIException():base(){			
		}	

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SeasonalityPlanChartUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SeasonalityPlanChartUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
