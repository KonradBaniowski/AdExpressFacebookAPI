#region Informations
// Auteur: D. Mussuma 
// Date de création: 01/06/2006 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des données pour l'analyse plan média
	/// </summary>
	public class SloganDataAccessException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SloganDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SloganDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SloganDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
