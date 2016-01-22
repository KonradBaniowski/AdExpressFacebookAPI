#region Informations
// Auteur: G. Ragneau
// Date de création: 24/08/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// GroupDataAccessException thrown whenever an error occured while accessing to data for groups management
	/// </summary>
	public class GroupDataAccessException:BaseException {

		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GroupDataAccessException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GroupDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public GroupDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
	
		
}
	
	

