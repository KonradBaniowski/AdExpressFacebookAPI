#region Informations
// Auteur: Y. R'kaina
// Date de création: 05/09/2007
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions {

    /// <summary>
    /// Erreur lors du résultat du détail media du portefeuille d'un support
    /// </summary>
    class PortofolioDetailMediaException : BaseException {

        #region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioDetailMediaException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PortofolioDetailMediaException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public PortofolioDetailMediaException(string message, System.Exception innerException) : base(message, innerException) {
		}
		#endregion

    }
}
