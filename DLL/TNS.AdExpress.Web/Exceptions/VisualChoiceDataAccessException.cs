#region Informations
// Auteur: D. V. Mussuma
// Date de création:
// Date de modification: 28/09/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// VisualChoiceDataAccessException thrown whenever an error occured while accessing data for visual selection.
	/// </summary>
	public class VisualChoiceDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VisualChoiceDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public VisualChoiceDataAccessException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public VisualChoiceDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}