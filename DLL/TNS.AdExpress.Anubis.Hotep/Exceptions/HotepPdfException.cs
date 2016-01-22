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
	public class HotepPdfException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public HotepPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public HotepPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="err">Exception relative</param>
		public HotepPdfException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
