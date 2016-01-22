#region Informations
// Auteur: Y. R'kaina 
// Date de création: 03/09/2007 
// Date de modification: 03/09/2007 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions {
    /// <summary>
    /// Classe d'exception de la génération des données pour la page des versions
    /// </summary>
    class CreativeRulesException : BaseException {

        #region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CreativeRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CreativeRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public CreativeRulesException(string message, System.Exception innerException) : base(message, innerException) {
		}
		#endregion

    }
}
