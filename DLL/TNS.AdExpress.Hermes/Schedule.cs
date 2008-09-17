using System;
using System.Collections;

namespace TNS.AdExpress.Hermes {
	/// <summary>
	/// Description of a Schedule
	/// </summary>
	public class Schedule {
		
		#region Variables
		/// <summary>
		/// Days of a week.
		/// Contains all the rules
		/// </summary>
		/// <remarks>
		/// 0: Sunday
		/// </remarks>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		private ArrayList[] _days=new ArrayList[7];
		#endregion

		#region Accessors
		/// <summary>
		/// Get Days of a week
		/// </summary>
		public ArrayList this[DayOfWeek day]{
			get{return(_days[(int)day]);}
			set{_days[(int)day]=value;}
		}
		#endregion

	}
}
