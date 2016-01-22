#region Informations
// Auteur: 
// Date de création: 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// DirectoryDataAccessException thrown whenever an error occured while accesing my adexpress directories
	/// </summary>
	public class DirectoryDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DirectoryDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public DirectoryDataAccessException(string message):base(message){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DirectoryDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
