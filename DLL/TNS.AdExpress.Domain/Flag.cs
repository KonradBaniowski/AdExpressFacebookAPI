#region Informations
// Author: D. Mussuma
// Creation date: 23/03/2008 
// Modification date:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain {
	/// <summary>
	/// Description of a flag use to manage functionalities rights
	/// </summary>
	public class Flag {

		#region Variables
		/// <summary>
		/// Flag identifier
		/// </summary>
		private Int64 _id;
		/// <summary>
		/// Flag description
		/// </summary>
		private string _description = null;
		#endregion

		#region Accessors
		/// <summary>
		/// Get/set flag identifer
		/// </summary>
		public Int64 ID {
			get { return _id; }
			set { _id = value; }
		}
		/// <summary>
		/// Get/set flag description
		/// </summary>
		public string Description {
			get { return _description; }
			set { _description = value; }
		} 
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id"> Flag identifier</param>
		/// <param name="description">Flag description</param>
		public Flag(Int64 id, string description) {
			if (id == null) throw (new ArgumentException("Invalid identifier parameter"));
			if (description == null || description.Length == 0) throw (new ArgumentException("Invalid description parameter"));
		}
		#endregion
	}
}
