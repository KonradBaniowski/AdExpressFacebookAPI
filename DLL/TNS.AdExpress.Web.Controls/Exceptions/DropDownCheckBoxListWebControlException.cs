#region Informations
// Auteur: G. Facon 
// Date de création: 14/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Exception lancée lors de l'initialisation d'un composant
	/// </summary>
	public class DropDownCheckBoxListWebControlException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DropDownCheckBoxListWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
        public DropDownCheckBoxListWebControlException(string message)
            : base(message) {
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public DropDownCheckBoxListWebControlException(string message, Exception innerException)
            : base(message, innerException) {
        }
		#endregion
	}
}
