#region Informations
// Auteur: G. Facon 
// Date de création: 15/06/2004 
// Date de modification: 15/06/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe d'exception de la génération des détails insertion par insertion
	/// </summary>
	public class MediaInsertionsCreationsUIException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaInsertionsCreationsUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaInsertionsCreationsUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaInsertionsCreationsUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
