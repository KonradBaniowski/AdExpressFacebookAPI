#region Informations
// Auteur: G. Facon 
// Date de création: 23/11/2004 
// Date de modification: 23/11/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'analyse plan média
	/// </summary>
	public class MarketShareRulesException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MarketShareRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MarketShareRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MarketShareRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
