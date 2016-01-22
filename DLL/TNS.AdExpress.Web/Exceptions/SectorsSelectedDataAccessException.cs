#region Informations
// Auteur: K. Shehzad 
// Date de création: 22/04/2005 

#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: sélection des familles
	/// </summary>
	public class SectorsSelectedDataAccessException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorsSelectedDataAccessException():base(){		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorsSelectedDataAccessException(string message):base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorsSelectedDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
