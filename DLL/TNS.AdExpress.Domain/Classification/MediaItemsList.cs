#region Informations
// Author: G. Facon
// Creation Date: 27/08/2008 
// Modification Date: 
#endregion

using System;
using System.Collections.Generic;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DomainException=TNS.AdExpress.Domain.Exceptions;
using TNS.Baal.ExtractList;

namespace TNS.AdExpress.Domain.Classification {
	/// <summary>
	/// media Items list used to determine an AdExpress universe
	/// </summary>
    public class MediaItemsList:IBaalItemsList{

		#region Variables
		/// <summary>
		/// List of vehicles
		/// </summary>
		private string _vehicleItemsList="";
		/// <summary>
		/// List of categories
		/// </summary>
		private string _categoryItemsList="";
		/// <summary>
		/// List of media
		/// </summary>
		private string _mediaItemsList="";
		#endregion

		#region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		public MediaItemsList(){
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Init list from baal
        /// </summary>
        /// <param name="idList">Baal Id List</param>
        /// <param name="levels">Levels to load</param>
        public void InitFromBaal(int idList,List<TNS.Baal.ExtractList.Constantes.Levels> levels) {
            try {
                Liste liste = TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(idList);
                foreach(TNS.Baal.ExtractList.Constantes.Levels currentLevel in levels) {
                    switch(currentLevel) {
                        case TNS.Baal.ExtractList.Constantes.Levels.vehicle:
                            _vehicleItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.category:
                            _categoryItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.media:
                            _mediaItemsList=liste.GetLevelIds(currentLevel);
                            break;
                    }
                }
            }
            catch(System.Exception err) {
                throw (new DomainException.MediaListException("Error when loading list: "+idList,err));
            }
        }
        #endregion

		#region Accessors
		/// <summary>
		/// Get the list of vehicles
		/// </summary>
		public string GetVehicleItemsList{
			get{return(_vehicleItemsList);}
            set { _vehicleItemsList=value; }
		}

		/// <summary>
		/// Get the list of categories
		/// </summary>
		public string GetCategoryItemsList{
			get{return(_categoryItemsList);}
            set { _categoryItemsList=value; }
		}

		/// <summary>
		/// Get the list of media
		/// </summary>
		public string GetMediaItemsList{
			get{return(_mediaItemsList);}
            set { _mediaItemsList=value; }
		}
		#endregion
	}
}
