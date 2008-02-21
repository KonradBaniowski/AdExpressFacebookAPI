#region Informations
// Author: K. Shehzad
// Date of creation: 11/08/2005 
// Date of modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: APPM Graphique de la PDV du plan
	/// </summary>
	public class PDVPlanChartUIExcpetion:BaseException{

		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PDVPlanChartUIExcpetion():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PDVPlanChartUIExcpetion(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PDVPlanChartUIExcpetion(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}