#region Informations
// Auteur: G. Facon
// Date de création: 21/11/2004 
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Exception lors de la Gestion des vehicle
	/// </summary>
	public class BannersFormatException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public BannersFormatException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public BannersFormatException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public BannersFormatException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion
	}
}
