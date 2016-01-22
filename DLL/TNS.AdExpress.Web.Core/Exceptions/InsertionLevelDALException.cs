#region Informations
// Auteur: d. Mussuma
// Date de création:
// Date de modification: 31/08/2011
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{

	/// <summary>
	/// Erreur lors de l'utilisation des listes
	/// </summary>
	public class InsertionLevelDALException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public InsertionLevelDALException():base(){
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public InsertionLevelDALException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public InsertionLevelDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}
