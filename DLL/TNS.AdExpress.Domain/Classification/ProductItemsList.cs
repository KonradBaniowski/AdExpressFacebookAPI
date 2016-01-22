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
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

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

		#region Public methods
		/// <summary>
		/// Get Sql product to exclude in query
		/// </summary>
		/// <param name="startWithAnd">Detemine if sql string start with and</param>
		/// <param name="prefix">prefix</param>
		/// <returns></returns>
		public string GetExcludeItemsSql(bool startWithAnd, string prefix) {
			string sql = "";
			Table sector = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.sector);
			if (sector != null && _sectorItemsList != null && _sectorItemsList.Length > 0) {
				if (startWithAnd) sql += " and ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + sector.Label + " not in ( " + _sectorItemsList + " ) ";
			}
			Table subSector = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.subsector);
			if (sector!=null && _subSectorItemsList != null && _subSectorItemsList.Length > 0) {
				if (startWithAnd || sql.Length>0) sql += " and ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + subSector.Label + " not in ( " + _subSectorItemsList + " ) ";
			}
			Table group = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.group);
			if (group!=null && _groupItemsList != null && _groupItemsList.Length > 0) {
				if (startWithAnd || sql.Length > 0) sql += " and ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + group.Label + " not in ( " + _groupItemsList + " ) ";
			}
			Table segment = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.segment);
			if (segment!=null && _segmentItemsList != null && _segmentItemsList.Length > 0) {
				if (startWithAnd || sql.Length > 0) sql += " and ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + segment.Label + " not in ( " + _segmentItemsList + " ) ";
			}
			Table product = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product);
			if (product!=null && _productItemsList != null && _productItemsList.Length > 0) {
				if (startWithAnd || sql.Length > 0) sql += " and ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + product.Label + " not in ( " + _productItemsList + " ) ";
			}
			return sql;
		}
		
								
		#endregion
	}
}
