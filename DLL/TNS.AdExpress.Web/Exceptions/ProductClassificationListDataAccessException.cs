#region Informations
// Auteur: G. Facon 
// Date de création: 26/08/2004 
// Date de modification: 26/08/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Liste de produits
	/// </summary>
	public class ProductClassificationListDataAccessException:BaseException{
		
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductClassificationListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductClassificationListDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ProductClassificationListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
