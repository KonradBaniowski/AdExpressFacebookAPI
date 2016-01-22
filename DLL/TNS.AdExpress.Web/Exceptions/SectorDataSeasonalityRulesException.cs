#region Informations
// Auteur: Y. R'kaina
// Date de création: 25/01/2007 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Rules : sector data seasonality
	/// </summary>
	public class SectorDataSeasonalityRulesException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDataSeasonalityRulesException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDataSeasonalityRulesException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorDataSeasonalityRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
