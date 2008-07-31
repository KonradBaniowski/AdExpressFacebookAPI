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
	public class Flags {

		#region Variables
		/// <summary>Instance of Flags</summary>		
		private static Flags _instance;
		/// <summary>
		/// Flag list
		/// </summary>
		private Dictionary<Int64, Flag> _flags;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		private Flags() {		
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private Flags(IDataSource source) {
			_flags = FlagsXL.Load(source);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get instance of Flags		
		public static Flags GetInstance(IDataSource source) {
			lock (typeof(Flags)) {
				if (_instance == null) {
					_instance = new Flags(source);
				}
			}
			return _instance;
		}
		/// <summary>
		/// Check
		/// </summary>
		/// <param name="id">Flag identifier</param>
		/// <returns>True if contain flag</returns>
		public static bool ContainFlag(Int64 id) {
			return (_instance != null && _instance._flags != null && _instance._flags.ContainsKey(id));
		}
		/// <summary>
		/// Check
		/// </summary>
		/// <param name="id">Flag identifier</param>
		/// <param name="source">source</param>
		/// <returns>True if contain flag</returns>
		public static bool ContainFlag(Int64 id, IDataSource source) {
			if (_instance == null) {
				lock (typeof(Flags)) {
						_instance = new Flags(source);
				}
			}		
			return (_instance != null && _instance._flags != null && _instance._flags.ContainsKey(id));			
		}
		#endregion
	}
}
