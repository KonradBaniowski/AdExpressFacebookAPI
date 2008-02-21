#region Information
// Author: K. Shehzad
// Creation: 01/09/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions 
{
	/// <summary>
	/// ProductListDataAccessException thrown when database access error occurs while chargin dropdown list for synthesis : APPM
	/// </summary>
	public class ProductListDataAccessException:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductListDataAccessException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductListDataAccessException(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ProductListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

