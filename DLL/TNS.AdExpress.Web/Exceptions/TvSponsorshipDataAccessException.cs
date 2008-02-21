#region Informations
// Auteur: D. Mussuma 
// Date de création: 01/12/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{


	/// <summary>
	/// Classe d'exception de la génération des données pour le parrainage télé
	/// </summary>
	public class TvSponsorshipDataAccessException:BaseException {
		
		#region Constructeur


		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TvSponsorshipDataAccessException( ):base() {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TvSponsorshipDataAccessException( string message ):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TvSponsorshipDataAccessException( string message, System.Exception innerException ):base(message,innerException) {
		}
		#endregion

	}
}
