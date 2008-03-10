#region Informations
// Auteur: G. Facon
// Date de création: 06/03/08
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.DataAccess.Exceptions{
	/// <summary>
	/// Right Object Exception
	/// </summary>
	public class RightDALException:BaseException{

		#region Constructeurs

		/// <summary>
		/// Constructeur
		/// </summary>
		public RightDALException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RightDALException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public RightDALException(string message,System.Exception innerException)
            : base(message,innerException) {
		}

		#endregion
	}
}
