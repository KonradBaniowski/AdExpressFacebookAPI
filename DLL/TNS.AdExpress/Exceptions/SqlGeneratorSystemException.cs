#region Informations
// Auteur: G. Facon 
// Date de création: 12/04/2005
// Date de modification: 12/04/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
	/// <summary>
	/// Description résumée de AdvertiserListException.
	/// </summary>
	public class SqlGeneratorSystemException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SqlGeneratorSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message"></param>
		public SqlGeneratorSystemException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SqlGeneratorSystemException(string message,System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}

