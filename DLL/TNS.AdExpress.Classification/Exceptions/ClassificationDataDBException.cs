#region Informations
// Auteur: G. Facon
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Classification.Exceptions{
	/// <summary>
	/// Erreur lors des traitements des données de la base de données de la nomenclature
	/// </summary>
	public class ClassificationDataDBException:BaseException{

		#region Constructeur

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ClassificationDataDBException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ClassificationDataDBException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ClassificationDataDBException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
