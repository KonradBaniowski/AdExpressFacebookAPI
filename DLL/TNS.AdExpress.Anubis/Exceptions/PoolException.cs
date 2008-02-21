#region Informations
// Auteur: G. Facon
// Date de création: 20/05/2005
// Date de modification: 20/05/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception de la liste des demandes de résultat
	/// </summary>
	public class PoolException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PoolException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public PoolException(string message):base(message){
				
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PoolException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}