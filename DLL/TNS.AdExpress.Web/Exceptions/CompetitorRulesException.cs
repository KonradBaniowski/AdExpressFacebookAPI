#region Informations
// Auteur: G. Facon 
// Date de création: 16/09/2004 
// Date de modification: 16/09/2004
//		23/11/2004 G. Facon Changement de nom de classe
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'analyse plan média
	/// </summary>
	public class CompetitorRulesException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorRulesException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
