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
	/// Product Items list used to determine an AdExpress universe
	/// </summary>
	public class ProductItemsList:IBaalItemsList{
		
		#region Variables
		/// <summary>
		/// List of sectors
		/// </summary>
		private string _sectorItemsList="";
		/// <summary>
		/// List of subsectors
		/// </summary>
		private string _subSectorItemsList="";
		/// <summary>
		/// List of groups
		/// </summary>
		private string _groupItemsList="";
		/// <summary>
		/// List of segments
		/// </summary>
		private string _segmentItemsList="";
		/// <summary>
		/// List ofproducts
		/// </summary>
		private string _productItemsList="";
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ProductItemsList(){
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
                        case TNS.Baal.ExtractList.Constantes.Levels.sector:
                            _sectorItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.subsector:
                            _subSectorItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.group_:
                            _groupItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.segment:
                            _segmentItemsList=liste.GetLevelIds(currentLevel);
                            break;
                        case TNS.Baal.ExtractList.Constantes.Levels.product:
                            _productItemsList=liste.GetLevelIds(currentLevel);
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
		/// Get list of sectors
		/// </summary>
		public string GetSectorItemsList{
			get{return(_sectorItemsList);}
		}

		/// <summary>
		/// Get list of subsectors
		/// </summary>
		public string GetSubSectorItemsList{
			get{return(_subSectorItemsList);}
		}

		/// <summary>
		/// Get list of groups
		/// </summary>
		public string GetGroupItemsList{
			get{return(_groupItemsList);}
		}

		/// <summary>
		/// Get list of segments
		/// </summary>
		public string GetSegmentItemsList{
			get{return(_segmentItemsList);}
		}

		/// <summary>
		/// Get list of products
		/// </summary>
		public string GetProductItemsList{
			get{return(_productItemsList);}
		}
		#endregion
	}
}
