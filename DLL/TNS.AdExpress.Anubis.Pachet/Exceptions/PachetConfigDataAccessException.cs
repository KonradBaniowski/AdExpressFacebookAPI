#region Informations
// Auteur: D. V. Mussuma
// Date de création: 27/02/2012
// Date de modification:
#endregion


using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Pachet.Exceptions
{
	/// <summary>
	/// Description résumée de SobekConfigDataAccessException.
	/// </summary>
	public class PachetConfigDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PachetConfigDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PachetConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PachetConfigDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
