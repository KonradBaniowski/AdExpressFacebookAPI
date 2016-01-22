using System;

namespace TNS.AdExpress.Hermes{
	/// <summary>
	/// Description of a Rule used to monitor the media transfer.
	/// </summary>
	///<author>G. Facon</author>
	///<since>12/02/07</since>
	[System.Serializable]
	public class Rule  {

		#region Variables
		///<summary>
		/// Rule Id
		///</summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected Int64 _id;
		///<summary>
		/// DataBase table name used by the plug-in to compute the indocator
		///</summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected string _tableName;
		///<summary>
		/// Media list Id used to retreive all the media to monitor
		///</summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected Int64 _mediaListId;
		///<summary>
		/// Define the beginning hour for the first spot which can be monitor
		///</summary>
		///<remarks>
		/// Not used for press vehicle
		///</remarks>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected DateTime _hourBegin;
		///<summary>
		/// Define the ending hour for the last spot which can be monitor
		///</summary>
		///<remarks>
		/// Not used for press vehicle
		///</remarks>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected DateTime _hourEnd;
		/// <summary>
		/// Diffusion Id
		/// </summary>
		protected Int64 _diffusionId;
		///<summary>
		/// Plug-in Id
		///</summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		protected Int64 _pluginId;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public Rule(){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">Rule Id</param>
		/// <param name="tableName">DataBase table name used by the plug-in to compute the indocator</param>
		/// <param name="mediaListId">media list Id used to retreive all the media to monitor</param>
		/// <param name="hourBegin">Define the beginning hour for the first spot which can be monitor</param>
		/// <param name="hourEnd">Define the ending hour for the last spot which can be monitor</param>
		/// <param name="diffusionId">Diffusion Id</param>
		/// <param name="pluginId">Plug-in Id</param>
		public Rule(Int64 id,string tableName,Int64 mediaListId,string hourBegin,string hourEnd,Int64 diffusionId,Int64 pluginId){
			_id = id;
			_tableName = tableName;
			_mediaListId = mediaListId;
			if(hourBegin.Length!=4)throw(new ArgumentException("the beginning hour length must be 4"));
			_hourBegin = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,int.Parse(hourBegin.Substring(0,2)),int.Parse(hourBegin.Substring(2,2)),0,0);
			if(hourEnd.Length!=4)throw(new ArgumentException("the ending hour length must be 4"));
			_hourEnd = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,int.Parse(hourEnd.Substring(0,2)),int.Parse(hourEnd.Substring(2,2)),0,0);
			_diffusionId=diffusionId;
			_pluginId=pluginId;
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Get Rule Id
		/// </summary>
		public Int64 Id{
			get{return(_id);}
		}

		/// <summary>
		/// Get TableName
		/// </summary>
		public string TableName{
			get{return(_tableName);}
		}

		/// <summary>
		/// Get Media list Id
		/// </summary>
		public Int64 MediaListId{
			get{return(_mediaListId);}
		}

		/// <summary>
		/// Get hour begin
		/// </summary>
		public DateTime HourBegin{
			get{return(_hourBegin);}
		}

		/// <summary>
		/// Get hour end
		/// </summary>
		public DateTime HourEnd{
			get{return(_hourEnd);}
		}

		/// <summary>
		/// Get Diffusion Id
		/// </summary>
		public Int64 DiffusionId{
			get{return(_diffusionId);}
		}

		/// <summary>
		/// Get plugin Id
		/// </summary>
		public Int64 PluginId{
			get{return(_pluginId);}
		}
		#endregion

	}
}
