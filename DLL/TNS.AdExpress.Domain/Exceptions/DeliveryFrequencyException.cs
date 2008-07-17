#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions
{
	/// <summary>
	/// DeliveryFrequencyException thrown whenever an error occured while a frequency delivery is unvalid
	/// </summary>
	public class DeliveryFrequencyException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DeliveryFrequencyException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DeliveryFrequencyException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DeliveryFrequencyException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
