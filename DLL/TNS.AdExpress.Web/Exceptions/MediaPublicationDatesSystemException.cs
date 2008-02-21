
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
	/// MediaPublicationDatesSystemException thrown while initializing the list
	/// </summary>
	public class MediaPublicationDatesSystemException:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPublicationDatesSystemException():base()
		{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPublicationDatesSystemException(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPublicationDatesSystemException(string message,System.Exception innerException):base(message,innerException)
		{
		}
		#endregion
	}
}