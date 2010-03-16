#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 14/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Exception lanc�e lors de l'initialisation d'un composant
	/// </summary>
	public class SectorWebControlException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
        public SectorWebControlException(string message)
            : base(message) {
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public SectorWebControlException(string message, Exception innerException)
            : base(message, innerException) {
        }
		#endregion
	}
}
