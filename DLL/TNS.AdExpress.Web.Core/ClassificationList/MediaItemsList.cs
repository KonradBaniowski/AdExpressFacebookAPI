#region Informations
// Author: G. Facon
// Creation Date: 29/10/2004 
// Modification Date: 29/10/2004 (G. Facon)
//				K. Shehzad		Modified for Including outdoor in the vehicle selection list
//	21/03/2005	D.V. Mussuma	Liste des pan euro
#endregion

using System;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebException=TNS.AdExpress.Web.Core.Exceptions;
// PZL
//using TNS.Baal.ExtractList;
namespace TNS.AdExpress.Web.Core.ClassificationList{
	/// <summary>
	/// media Items list used to determine an AdExpress universe
	/// </summary>
	public class MediaItemsList{

		#region Variables
		/// <summary>
		/// List of vehicles
		/// </summary>
		private string vehicleItemsList="";
		/// <summary>
		/// List of categories
		/// </summary>
		private string categoryItemsList="";
		/// <summary>
		/// List of media
		/// </summary>
		private string mediaItemsList="";
		#endregion

		#region constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="idVehicleItemsList">List Id</param>
		public MediaItemsList(int idVehicleItemsList){
			// PZL
            /*
			TNS.Baal.ExtractList.Liste liste = null;

			switch(idVehicleItemsList){
					#region ancienne version
//				case WebConstantes.AdExpressUniverse.ALERTE_MEDIA_LIST_ID:
//					vehicleItemsList="1,2,3,5,8,11";
//					break;
//				case WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID:
//					categoryItemsList="11,12,13,14,17,18,19,22,23,33,61,21,24,25,27,38,31,34,35,37,39,26,41,54,55,56,57,58,59,60,63,64,77,79,80,86,89,90,94,95,96,97,98,28,81,82,83,84,91";
//					break;
//				case WebConstantes.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID:
//					categoryItemsList="15,30";
//					break;
//				case WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID:
//					categoryItemsList="68";
//					break;
					#endregion

				case WebConstantes.AdExpressUniverse.MEDIA_DEFAULT_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.MEDIA_DEFAULT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.COMPETITIVE_ALERT_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.COMPETITIVE_ALERT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.COMPETITIVE_REPORTS_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.COMPETITIVE_REPORTS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ALERT_LIST_ID :
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ALERT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ANALYSIS_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ANALYSIS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.PROSPECTING_ALERT_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.PROSPECTING_ALERT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.PROSPECTING_REPORTS_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.PROSPECTING_REPORTS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;
				case WebConstantes.AdExpressUniverse.TREND_REPORTS_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.TREND_REPORTS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_ALERT_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_ALERT_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);
					break;

				case WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID);
					categoryItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.category);					
					break;

				case WebConstantes.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID);
					categoryItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.category);						
					break;

				case WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID);
					categoryItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.category);					
					break;

				case WebConstantes.AdExpressUniverse.TRENDS_LIST_ID:
					liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(WebConstantes.AdExpressUniverse.TRENDS_LIST_ID);
					vehicleItemsList = liste.GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.vehicle);					
					break;

				default:
					throw (new WebException.MediaListException("the methode doesn't contains treatement for the Id: "+idVehicleItemsList));
			}*/
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get the list of vehicles
		/// </summary>
		public string GetVehicleItemsList{
            // PZL
            get{return("");}
			//get{return(vehicleItemsList);}
		}

		/// <summary>
		/// Get the list of categories
		/// </summary>
		public string GetCategoryItemsList{
            // PZL
            get{return("");}
			//get{return(categoryItemsList);}
		}

		/// <summary>
		/// Get the list of media
		/// </summary>
		public string GetMediaItemsList{
            // PZL
            get{return("");}
			//get{return(mediaItemsList);}
		}
		#endregion
	}
}
