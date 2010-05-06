#region Information
//Author : Y. Rkaina 
//Creation : 11/07/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Hotep.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class HotepPdfInitializationException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public HotepPdfInitializationException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public HotepPdfInitializationException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="err">Exception relative</param>
        public HotepPdfInitializationException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion
	}
}
