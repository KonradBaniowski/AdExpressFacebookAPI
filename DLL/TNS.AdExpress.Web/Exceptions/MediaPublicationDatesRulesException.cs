#region Information
// Author: K. Shehzad
// Creation: 29/08/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions 
{
	/// <summary>
	/// MediaPublicationDatesRulesException thrown while formatting the data 
	/// </summary>
	public class MediaPublicationDatesRulesException:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPublicationDatesRulesException():base()
		{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPublicationDatesRulesException(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPublicationDatesRulesException(string message,System.Exception innerException):base(message,innerException)
		{
		}
		#endregion
	}
}
