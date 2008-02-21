#region Informations
// Auteur: Y. R'kaina
// Date de création: 07/03/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Description résumée de ReportingPluginBaseDataAccessException.
	/// </summary>
	public class ReportingPluginBaseDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ReportingPluginBaseDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ReportingPluginBaseDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ReportingPluginBaseDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
