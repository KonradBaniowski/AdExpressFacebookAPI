#region Informations
// Auteur: G. Facon 
// Date de création: 12/08/2005
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: Classe d'exception de la génération des données
	/// </summary>
	public class MarketShareUIException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MarketShareUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MarketShareUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MarketShareUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
