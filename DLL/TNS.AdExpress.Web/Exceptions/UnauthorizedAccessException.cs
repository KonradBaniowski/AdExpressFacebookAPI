#region Informations
/* 
 * Auteur       : G Ragneau
 * Date         : 21/08/2007
 * Modification :
 *      Auteur - Date - Description
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: Portefeuille
	/// </summary>
	public class UnauthorizedAccessException : BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public UnauthorizedAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public UnauthorizedAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public UnauthorizedAccessException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion

	}
}
