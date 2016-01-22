#region Informations
// Auteur: D. Mussuma 
// Date de création: 24/09/2004 
// Date de modification: 24/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Indicateurs
	/// </summary>
	public class SectorAnalysisIndicatorDataAccessException :BaseException{

		#region constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>		
		public SectorAnalysisIndicatorDataAccessException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorAnalysisIndicatorDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorAnalysisIndicatorDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion 
	}
}
