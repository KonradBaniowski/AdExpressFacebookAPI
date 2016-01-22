#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Analyse sectorielle: Indicateur
	/// </summary>
	public class IndicatorSeasonalityException : BaseException{
		
		#region Constructeurs

		/// <summary>
		/// constructeur par défaut
		/// </summary>
		public IndicatorSeasonalityException():base() {
		}
		/// <summary>
		/// constructeur avec message d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public IndicatorSeasonalityException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public IndicatorSeasonalityException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
