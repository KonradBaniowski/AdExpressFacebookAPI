#region Information
// Author : G Facon
// Creation date : 29/03/2006
// Modification date :
//		
#endregion

using System;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// AdNetTrack product selection
	/// </summary>
	public class AdNetTrackProductSelection{

		#region Variables
		/// <summary>
		/// Selection type
		/// </summary>
		/// <example>
		/// <list type="string">
		/// <listheader>Selection Type</listheader>
		///	<item>Advertiser</item>
		///	<item>Product</item>
		///	<item>Hashcode</item>
		/// </list>
		/// </example>
		TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type _selectionType;
		/// <summary>
		/// Id
		/// </summary>
		Int64 _id=0;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="selectionType">Selection type</param>
		/// <param name="id">Id</param>
		public AdNetTrackProductSelection(TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type selectionType,Int64 id){
			_selectionType=selectionType;
			_id=id;
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Get Selection type
		/// </summary>
		public TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type SelectionType{
			get{return(_selectionType);}
		}
		
		/// <summary>
		/// Get Id selected
		/// </summary>
		public Int64 Id{
			get{return(_id);}
		}
		#endregion
	}
}
