#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.MediaSchedule{


	///<summary>
	/// Data for media schedule report
	/// </summary>
	public class MediaScheduleData:ResultData {

		#region Variables
		///<summary>
		/// List of versions present in the result
		/// </summary>
		protected Dictionary<Int64, VersionItem> _versionsDetail = new Dictionary<Int64, VersionItem>();
        ///<summary>
        /// Media Schedule Header (usefull for pagination)
        /// </summary>
        protected string _headers = string.Empty;
        ///<summary>
        /// Number of periods (usefull for pagination)
        /// </summary>
        protected Int64 _periodNb = -1;
        #endregion

		#region Constructors
		///<summary>
		/// Constructor
		/// </summary>
        public MediaScheduleData()
            : base()
        {
		}
		#endregion

		#region Accesseurs
		///<summary>
		/// Get list of creatives presented in media schedule
		/// </summary>
        public Dictionary<Int64, VersionItem> VersionsDetail
        {
			get{return(_versionsDetail);}
		}
        ///<summary>
        /// Media Schedule Header (usefull for pagination)
        /// </summary>
        public string Headers{
			get{return(_headers);}
            set { _headers = value; }
        }
        ///<summary>
        /// Number of periods (usefull for pagination)
        /// </summary>
        public Int64 PeriodNb
        {
            get { return (_periodNb); }
            set { _periodNb = value; }
        }
		#endregion

		#region Méthodes public
		/// <summary>
		/// Get Ids of versions used in the media schedule
		/// </summary>
		/// <remarks>Format: 25001,15002,32555</remarks>
		/// <returns>List of versions</returns>
		public string GetVersionList(){
			StringBuilder list=new StringBuilder(1000);
			foreach(Int64 curentVersion in _versionsDetail.Keys){
                list.AppendFormat("{0},", curentVersion);
			}
			string listString=list.ToString();
			if(listString.Length>0)return(listString.Substring(0,listString.Length-1));
			return(listString);
		}
		#endregion

	}
}
