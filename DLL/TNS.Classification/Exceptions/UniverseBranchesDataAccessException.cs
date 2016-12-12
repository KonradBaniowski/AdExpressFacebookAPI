using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.Classification.Exceptions {
	class UniverseBranchesDataAccessException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public UniverseBranchesDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public UniverseBranchesDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public UniverseBranchesDataAccessException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}
