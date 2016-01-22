#region Informations
// Author: Y. R'kaina
// Creation date: 21/06/2007 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
    /// <summary>
    /// Active Media management
    /// </summary>
    public class ActiveMediaListDataAccessException : BaseException{

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ActiveMediaListDataAccessException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ActiveMediaListDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public ActiveMediaListDataAccessException(string message, System.Exception innerException): base(message, innerException){
		}
		#endregion

    }
}
