#region Informations
/*
 * Author : G Ragneau
 * Created on 30/06/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.ProductClassReports.Exceptions
{
	/// <summary>
	/// Thrown when user ask for a report not implemented
	/// </summary>
	public class NotImplementedReportException:ProductClassReportsException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public NotImplementedReportException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public NotImplementedReportException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public NotImplementedReportException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}
