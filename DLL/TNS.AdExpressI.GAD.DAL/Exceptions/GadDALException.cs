#region Informations
// Auteur: B. Masson 
// Date de création: 27/09/2004 
// Date de modification: 27/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.GAD.DAL.Exceptions
{
	/// <summary>
	/// System: Gad
	/// </summary>
    public class GadDALException : BaseException
    {
		#region Constructeurs
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GadDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GadDALException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public GadDALException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
