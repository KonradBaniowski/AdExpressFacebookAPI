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
	/// MediaPublicationDatesDataAccessException thrown while executing the query
	/// </summary>
	public class MediaPublicationDatesDataAccessException:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPublicationDatesDataAccessException():base()
		{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPublicationDatesDataAccessException(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPublicationDatesDataAccessException(string message,System.Exception innerException):base(message,innerException)
		{
		}
		#endregion
	}
}