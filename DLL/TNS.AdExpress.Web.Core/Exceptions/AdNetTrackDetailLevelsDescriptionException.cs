#region Informations
// Auteur: G. Facon
// Date de création:	22/03/2007 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// When AdExpress loads the AdNetTrack levels descriptions
	/// </summary>
	public class AdNetTrackDetailLevelsDescriptionException:BaseException{
		
		#region Constructeurs
		/// Basic constructor
		/// </summary>
		public AdNetTrackDetailLevelsDescriptionException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public AdNetTrackDetailLevelsDescriptionException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
		public AdNetTrackDetailLevelsDescriptionException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
