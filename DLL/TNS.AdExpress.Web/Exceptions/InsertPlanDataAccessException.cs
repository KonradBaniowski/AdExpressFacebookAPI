#region Information
// Auteur: G. Ragneau
// Creation: 01/08/2005 
// Last modifications: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: APPM Insertion
	/// </summary>
	public class InsertPlanDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public InsertPlanDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public InsertPlanDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public InsertPlanDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}