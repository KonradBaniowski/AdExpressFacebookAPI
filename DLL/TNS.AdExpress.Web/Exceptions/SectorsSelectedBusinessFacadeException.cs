#region Informations
// Auteur: K. Shehzad 
// Date de création: 22/04/2005 

#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// System: Sélection des familles
	/// </summary>
	public class SectorsSelectedBusinessFacadeException:BaseException{
	
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorsSelectedBusinessFacadeException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message</param>
		public SectorsSelectedBusinessFacadeException(string message):base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorsSelectedBusinessFacadeException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
