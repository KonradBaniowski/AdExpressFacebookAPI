#region Informations
// Auteur: K. Shehzad
// Date de création: 04/05/2005 

#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// ExcelWebPageException thrown whenever an error occured in an excel web page
	/// </summary>
	public class ExcelWebPageException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ExcelWebPageException():base()		{
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">the exception message</param>
		public ExcelWebPageException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ExcelWebPageException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
