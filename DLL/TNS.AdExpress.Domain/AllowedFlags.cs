#region Informations
// Author: D. Mussuma
// Creation date: 23/03/2008 
// Modification date:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain {
	/// <summary>
	/// Flags loader
	/// </summary>
	public class AllowedFlags {

		#region Variables
		
		/// <summary>
		/// Flag list
		/// </summary>
		private static Dictionary<Int64, Flag> _flags = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		static AllowedFlags() {		
		}		
		#endregion

		#region Methods
		/// <summary>
		/// Initi flag list from xml file configuration
		/// </summary>
		/// <param name="source"></param>
		public static void Init(IDataSource source) {
			_flags = XmlLoader.FlagsXL.Load(source);
		}
		
		/// <summary>
		/// Check if contain flag
		/// </summary>
		/// <param name="id">Flag identifier</param>
		/// <returns>True if contain flag</returns>
		public static bool ContainFlag(Int64 id) {
			try {
				return _flags.ContainsKey(id);
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to retreive a flag Id", err));
			}
		}
		
		#endregion
	}
}
