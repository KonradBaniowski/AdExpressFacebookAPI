#region Informations
// Author: G. Facon
// Creation date: 06/03/2008
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.Domain.Exceptions{
	/// <summary>
    /// PluginConfiguration Object Exception
	/// </summary>
	public class PluginConfigurationException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public PluginConfigurationException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public PluginConfigurationException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public PluginConfigurationException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}
