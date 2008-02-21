#region Informations
// Author: G. Facon
// Creation Date: 29/10/2004 
// Modification Date: 29/10/2004 (G. Facon)
//	21/03/2005	D.V. Mussuma   Add Pan Euro List
//	10/08/2006	Y. Rkaina		?
#endregion

using System;
using System.Collections;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebException=TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.ClassificationList{
	/// <summary>
	/// This classe contains media classification items Lists used in AdExpress web site
	/// </summary>
	/// <example>
	/// For exemple the tendency module computes results for Press, radio and Tv.
	/// As customers can have access to other media, it is necessary to restrict the accessible media. For this one uses a list media defined in this class.
	/// </example>
	public class Media{

		#region Variables
		/// <summary>
		/// Media classification universe list
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.ClassificationList.MediaItemsList</associates>
		protected static Hashtable _list=null;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialization of the lists
		/// </summary>
		public static void Init(){ 
			try{
				_list=new Hashtable();

				#region Alert and Reports modules
				// Default media list
				_list.Add(WebConstantes.AdExpressUniverse.MEDIA_DEFAULT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.MEDIA_DEFAULT_LIST_ID));
				// Alert media schedule media list
				_list.Add(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ALERT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ALERT_LIST_ID));
				// Media schedule media list
				_list.Add(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ANALYSIS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ANALYSIS_LIST_ID));
				// Prospecting alert media list
				_list.Add(WebConstantes.AdExpressUniverse.PROSPECTING_ALERT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.PROSPECTING_ALERT_LIST_ID));
				// Prospecting reports media list
				_list.Add(WebConstantes.AdExpressUniverse.PROSPECTING_REPORTS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.PROSPECTING_REPORTS_LIST_ID));
				// Trend reports media list
				_list.Add(WebConstantes.AdExpressUniverse.TREND_REPORTS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.TREND_REPORTS_LIST_ID));
				// comparative alert media schedule media list
				_list.Add(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID));
				// comparative media schedule media list
				_list.Add(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID));
				// competitive alert media list
				_list.Add(WebConstantes.AdExpressUniverse.COMPETITIVE_ALERT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.COMPETITIVE_ALERT_LIST_ID));
				// competitive media list
				_list.Add(WebConstantes.AdExpressUniverse.COMPETITIVE_REPORTS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.COMPETITIVE_REPORTS_LIST_ID));
				_list.Add(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_ALERT_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_ALERT_LIST_ID));
				_list.Add(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_LIST_ID));
				#endregion
				
				//Tendency media list
				_list.Add(WebConstantes.AdExpressUniverse.TRENDS_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.TRENDS_LIST_ID));

				//Category Analysis media list
				_list.Add(WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID));
				
				//PanEuro Reports media list
				_list.Add(WebConstantes.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID));
				
				//Television Sponsorship media list 
				_list.Add(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,new MediaItemsList(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID));
			}
			catch(System.Exception ex){
				throw (new WebException.MediaException("Impossible to initialize a media list",ex));
			}		
		}
		#endregion

		#region Data Access
		/// <summary>
		/// Get a media list
		/// </summary>
		/// <param name="idMediaItemsList">List ID</param>
		/// <returns>MediaItemsList object whitch contains the medialist</returns>
		public static MediaItemsList GetMediaItemsList(int idMediaItemsList){
			try{
				if(_list==null)
					Init();
				return((MediaItemsList)_list[idMediaItemsList]);
			}
			catch(System.Exception){
				throw (new WebException.MediaException("the Media list doesn't exists for Id: "+idMediaItemsList));
			}
		}
		#endregion

	}
}
