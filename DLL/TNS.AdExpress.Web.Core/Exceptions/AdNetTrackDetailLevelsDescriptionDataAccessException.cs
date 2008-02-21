#region Informations
// Auteur: G. Facon
// Date de création:	22/03/2007 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// When AdExpress loads the AdNetTrack levels descriptions in (DB)
	/// </summary>
	public class AdNetTrackDetailLevelsDescriptionDataAccessException:BaseException{
		
		#region Constructeurs
		/// Basic constructor
		/// </summary>
		public AdNetTrackDetailLevelsDescriptionDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public AdNetTrackDetailLevelsDescriptionDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
		public AdNetTrackDetailLevelsDescriptionDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
