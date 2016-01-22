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
		public string VehicleList{
			get{return(_vehicleItemsList);}
            set { _vehicleItemsList=value; }
		}

		/// <summary>
		/// Get the list of categories
		/// </summary>
		public string CategoryList{
			get{return(_categoryItemsList);}
            set { _categoryItemsList=value; }
		}

		/// <summary>
		/// Get the list of media
		/// </summary>
		public string MediaList{
			get{return(_mediaItemsList);}
            set { _mediaItemsList=value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Get vehicle long list
		/// </summary>
		/// <returns></returns>
		public List<long> GetVehicles() {
			List<long> vehicleList = null;
			if (_vehicleItemsList != null && _vehicleItemsList.Length>0) vehicleList = new List<Int64>(Array.ConvertAll<string, Int64>(_vehicleItemsList.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
			return vehicleList;
		}
		/// <summary>
		/// Get category long list
		/// </summary>
		/// <returns></returns>
		public List<long> GetCategories() {
			List<long> categoryList = null;
			if (_categoryItemsList != null && _categoryItemsList.Length > 0) categoryList = new List<Int64>(Array.ConvertAll<string, Int64>(_categoryItemsList.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
			return categoryList;
		}
		/// <summary>
		/// Get media long list
		/// </summary>
		/// <returns></returns>
		public List<long> GetMedias() {
			List<long> mediaList = null;
			if (_mediaItemsList != null && _mediaItemsList.Length > 0) mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(_mediaItemsList.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
			return mediaList;
		}
		/// <summary>
		/// Get vehicle list SQl condition
		/// </summary>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetVehicleListSQL(bool withAnd) {
			string sql = "";
			Table vehicle = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle);
			sql = GetVehicleListSQL(withAnd, vehicle.Prefix);
			return sql;
		}
		/// <summary>
		/// Get vehicle list SQl condition
		/// </summary>
		/// <param name="prefix">field prefix</param>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetVehicleListSQL(bool withAnd, string prefix) {
			string sql = "";
			Table vehicle = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle);
			if (_vehicleItemsList != null && _vehicleItemsList.Length > 0) {
				if (withAnd) sql = " And ";
				if (prefix != null && prefix.Length>0) sql += prefix +".";
				sql += "id_" + vehicle.Label + " in ( " + _vehicleItemsList + " ) ";
			}
			return sql;
		}
		/// <summary>
		/// Get category list SQl condition
		/// </summary>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetCategoryListSQL(bool withAnd) {
			string sql = "";
			Table category = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.category);
			sql = GetCategoryListSQL(withAnd,category.Prefix);
			return sql;
		}
		/// <summary>
		/// Get category list SQl condition
		/// </summary>
		/// <param name="prefix">field prefix</param>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetCategoryListSQL(bool withAnd, string prefix) {
			string sql = "";
			Table category = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.category);
			if (_categoryItemsList != null && _categoryItemsList.Length > 0) {
				if (withAnd) sql = " And ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + category.Label + " in ( " + _categoryItemsList + " ) ";
			}
			return sql;
		}
		/// <summary>
		/// Get media list SQl condition
		/// </summary>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetMediaListSQL(bool withAnd) {
			string sql = "";
			Table media = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.media);
			GetMediaListSQL(withAnd, media.Prefix);
			return sql;
		}
		/// <summary>
		/// Get media list SQl condition
		/// </summary>
		/// <param name="prefix">field prefix</param>
		/// <param name="withAnd">Determine if sql condition start with "and" </param>
		/// <returns>SQl condition</returns>
		public string GetMediaListSQL(bool withAnd, string prefix) {
			string sql = "";
			Table media = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.media);
			if (_mediaItemsList != null && _mediaItemsList.Length > 0) {
				if (withAnd) sql = " And ";
				if (prefix != null && prefix.Length > 0) sql += prefix + ".";
				sql += "id_" + media.Label + " in ( " + _mediaItemsList + " ) ";
			}
			return sql;
		}
		#endregion
	}
}
