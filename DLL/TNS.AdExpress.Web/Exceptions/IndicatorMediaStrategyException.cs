#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 04/11/2004 
// Date de modification: 04/11/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Analyse sectorielle: Indicateurs
	/// </summary>
	public class IndicatorMediaStrategyException : BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur par défaut
		/// </summary>
		public IndicatorMediaStrategyException():base(){			
		}
		/// <summary>
		/// Constructeur avec message
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public IndicatorMediaStrategyException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public IndicatorMediaStrategyException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
