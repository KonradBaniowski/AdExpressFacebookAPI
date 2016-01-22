#region Informations
// Auteur: A. Obermeyer 
// Date de création: 07/02/2005 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{

	/// <summary>
	/// Classe d'exception de la génération des données pour le module Tendances
	/// </summary>
	public class TendenciesDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TendenciesDataAccessException():base(){	
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message</param>
		public TendenciesDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TendenciesDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
