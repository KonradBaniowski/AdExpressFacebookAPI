#region Informations
/*
 * Author : G Ragneau
 * Created On:
 * Modification:
 *      Author - Date - Description
 * 
 */
#endregion

using System;

namespace TNS.AdExpress.Domain.Results
{
	///<summary>
	/// HTML Code container
	/// </summary>
	public class ResultData{

		#region Variables
		///<summary>
		/// HTML Code of the resultCode HTML du calendrier d'action
		/// </summary>
		protected string _HTMLCode="";
		#endregion

		#region Constructos
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultData(){		
		}
		#endregion

		#region Accessors
		///<summary>
		/// Get / Set HTML Code
		/// </summary>
		public string HTMLCode {
			get{return(_HTMLCode);}
			set{_HTMLCode=value;}
		}
		#endregion
	}
}
