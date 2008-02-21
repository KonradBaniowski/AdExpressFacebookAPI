#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Rules: Analyse sectorielle Indicateurs
	/// </summary>
	public class IndicateurNoveltyRulesException : BaseException{
	
		#region Constructeurs

		/// <summary>
		/// constructeur par défaut
		/// </summary>
		public IndicateurNoveltyRulesException():base(){
		}
		/// <summary>
		/// constructeur avec message d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public IndicateurNoveltyRulesException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public IndicateurNoveltyRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
