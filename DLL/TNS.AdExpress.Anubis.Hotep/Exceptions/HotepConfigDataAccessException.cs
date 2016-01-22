#region Informations
// Auteur: Y. Rkaina
// Date de création: 10/07/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Hotep.Exceptions{
	/// <summary>
	/// Description résumée de HotepConfigDataAccessException.
	/// </summary>
	public class HotepConfigDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public HotepConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public HotepConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception relative</param>
		public HotepConfigDataAccessException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
