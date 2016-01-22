#region Informations
// Auteur: Y. R'kaina
// Date de création: 17/01/2007 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Rules : sector data synthèse
	/// </summary>
	public class SectorDataSynthesisRulesException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDataSynthesisRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDataSynthesisRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorDataSynthesisRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
