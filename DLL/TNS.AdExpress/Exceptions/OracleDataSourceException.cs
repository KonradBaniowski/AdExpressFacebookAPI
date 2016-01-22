#region Informations
// Auteur: G. Facon
// Date de création: 29/06/2005
// Date de modification: 29/06/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
	/// <summary>
	/// Erreur de la source de données
	/// </summary>
	public class OracleDataSourceException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public OracleDataSourceException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public OracleDataSourceException(string message):base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public OracleDataSourceException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
