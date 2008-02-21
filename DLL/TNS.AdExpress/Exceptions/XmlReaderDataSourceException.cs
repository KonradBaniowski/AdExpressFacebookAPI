#region Informations
// Auteur: G. Facon
// Date de création: 05/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
	/// <summary>
	/// Erreur de la source de données
	/// </summary>
	public class XmlReaderDataSourceException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public XmlReaderDataSourceException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public XmlReaderDataSourceException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public XmlReaderDataSourceException(string message,System.Exception innerException):base(message,innerException){
	}
		#endregion

	}
}
