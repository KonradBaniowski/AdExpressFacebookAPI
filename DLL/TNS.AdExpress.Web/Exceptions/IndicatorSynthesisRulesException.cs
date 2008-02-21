#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation:
// Date de modification: 18/04/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Analyse sectorielle: Indicateur
	/// </summary>
	public class  IndicatorSynthesisRulesException : BaseException{
		
		#region Constructeurs

		/// <summary>
		/// constructeur par d�faut
		/// </summary>
		public  IndicatorSynthesisRulesException():base() {
		}
		/// <summary>
		/// constructeur avec message d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public  IndicatorSynthesisRulesException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public  IndicatorSynthesisRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
