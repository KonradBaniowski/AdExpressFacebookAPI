#region Informations
// Auteur: Y. R'kaina
// Date de création: 25/01/2007 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Bases de données saisonnalité
	/// </summary>
	public class SectorDataSeasonalityDataAccessException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDataSeasonalityDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDataSeasonalityDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorDataSeasonalityDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
