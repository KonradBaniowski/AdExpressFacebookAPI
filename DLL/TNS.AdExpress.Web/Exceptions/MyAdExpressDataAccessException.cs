#region Informations
// Auteur: G. Facon 
// Date de création: 15/06/2004 
// Date de modification: 15/06/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données Mon AdExpress
	/// </summary>
	public class MyAdExpressDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MyAdExpressDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MyAdExpressDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MyAdExpressDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
