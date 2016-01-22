#region Informations
// Auteur: G. Facon 
// Date de création: 14/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Exception thrown when the size of the result table is too big to be displayed by the explorer
	/// </summary>
	public class ResultTableCapacityException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ResultTableCapacityException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
        public ResultTableCapacityException(string message)
            : base(message)
        {
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
        public ResultTableCapacityException(string message, Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}
