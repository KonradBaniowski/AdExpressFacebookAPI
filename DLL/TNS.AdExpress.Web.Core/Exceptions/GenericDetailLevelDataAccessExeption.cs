#region Informations
// Author: G. Facon
// Creation date: 25/04/2006 
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	///DataAccess : Detail Level management
	/// </summary>
	public class GenericDetailLevelDataAccessExeption:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericDetailLevelDataAccessExeption():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public GenericDetailLevelDataAccessExeption(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public GenericDetailLevelDataAccessExeption(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
