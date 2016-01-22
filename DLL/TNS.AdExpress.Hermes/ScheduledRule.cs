using System;
using System.Collections;

namespace TNS.AdExpress.Hermes {
	/// <summary>
	/// Description of a Schedule Rule
	/// </summary>
	public class ScheduledRule : Rule {

		#region Variables
		/// <summary>
		/// Hour to launch the rule
		/// </summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected DateTime _launchHour;
		/// <summary>
		/// Days of week to monitor
		/// </summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected ArrayList _daysOfWeek;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rule">Based Rule</param>
		/// <param name="launchHour">Hour to launch the rule</param>
		/// <param name="daysOfWeek">Days of week to monitor</param>
		public ScheduledRule(Rule rule,DateTime launchHour,ArrayList daysOfWeek){
			base._id = rule.Id;
			base._tableName = rule.TableName;
			base._mediaListId = rule.MediaListId;
			base._hourBegin = rule.HourBegin;
			base._hourEnd = rule.HourEnd;
			base._diffusionId = rule.DiffusionId;
			base._pluginId = rule.PluginId;
			_launchHour = launchHour;
			_daysOfWeek = daysOfWeek;
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Get Hour to launch the rule
		/// </summary>
		public DateTime LaunchHour{
			get{return(_launchHour);}
		}

		/// <summary>
		/// Get Days of week to monitor
		/// </summary>
		public ArrayList DaysOfWeek{
			get{return(_daysOfWeek);}
		}
		#endregion
	}
}
