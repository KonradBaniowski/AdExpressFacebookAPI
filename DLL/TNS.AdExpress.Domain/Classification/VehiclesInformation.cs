#region Informations
// Auteur: Y. R'kaina
// Création: 06/08/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Vehicles descriptions
    /// </summary>
    public class VehiclesInformation {

        #region variables
        ///<summary>
        /// Vehicles description list
        /// </summary>
        private static Dictionary<Vehicles.names, VehicleInformation> _listVehicleNames = new Dictionary<Vehicles.names, VehicleInformation>();
        ///<summary>
        /// Vehicles description list
        /// </summary>
        private static Dictionary<Int64, VehicleInformation> _listDataBaseId = new Dictionary<Int64, VehicleInformation>();

        ///<summary>
        /// Media Agency Flag by Vehicles  names list
        /// </summary>
        private static Dictionary<Vehicles.names, Int64> _listMediaAgencyFlagByVehicleNames = new Dictionary<Vehicles.names, Int64>();

        ///<summary>
        /// Media Agency Flag by Vehicles Ids list
        /// </summary>
        private static Dictionary<Int64, Int64> _listMediaAgencyFlagByVehicleIds = new Dictionary<Int64, Int64>();
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static VehiclesInformation() {
		}
		#endregion

        #region Méthodes publiques
        /// <summary>
        /// Get Databasids
        /// </summary>
        /// <remarks>List like 1,2,3</remarks>
        /// <returns>Id list</returns>
        public static string GetDatabaseIds() {
            string list="";
            foreach(Int64 currentDatabaseId in _listDataBaseId.Keys) {
                list+=currentDatabaseId.ToString()+",";
            }
            if(list.Length>0) list=list.Substring(0,list.Length-1);
            return (list);
        }
        /// <summary>
        /// Get Media Agency Ids
        /// </summary>
        /// <remarks>List like 1,2,3</remarks>
        /// <returns>Id list</returns>
        public static string GetMediaAgencyIds()
        {
            string list = "";
            foreach (Int64 currentMediaAgencyId in _listMediaAgencyFlagByVehicleNames.Values)
            {
                list += currentMediaAgencyId.ToString() + ",";
            }
            if (list.Length > 0) list = list.Substring(0, list.Length - 1);
            return (list);
        }
        #region Convert

        #region Convert Enum Id to databaseId
        /// <summary>
        /// Get the Convert from Enum Id to databaseId
        /// </summary>
        /// <param name="id">Enum Id</param>
        /// <returns>databaseId</returns>
        public static Int64 EnumToDatabaseId(Vehicles.names id) {
            try {
                return (_listVehicleNames[id].DatabaseId);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        #endregion

        #region Convert databaseId Id to Enum
        /// <summary>
        /// Get the Convert from databaseId to Enum Id
        /// </summary>
        /// <param name="dataBaseVehicleId">databaseId</param>
        /// <returns>Enum Id</returns>
        public static Vehicles.names DatabaseIdToEnum(Int64 dataBaseVehicleId) {
            try {
                return (_listDataBaseId[dataBaseVehicleId].Id);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        #endregion

        #region Convert Enum List to databaseId list
        /// <summary>
        /// Get the Convert from Enum Id List to databaseId List
        /// </summary>
        /// <param name="vehicleList">Enum Id List</param>
        /// <returns>databaseId List</returns>
        public static List<Int64> EnumListToDatabaseIdList(List<Vehicles.names> vehicleList) {
            try {
                List<Int64> databaseIdList = new List<Int64>();
                foreach (Vehicles.names currentVehicle in vehicleList) {
                    databaseIdList.Add(VehiclesInformation.EnumToDatabaseId(currentVehicle));
                }
                return (databaseIdList);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        #endregion

        #region Convert databaseId list to Enum List
        /// <summary>
        /// Get the Convert from databaseId List to Enum Id List
        /// </summary>
        /// <param name="databaseIdList">databaseId List</param>
        /// <returns>Enum Id List</returns>
        public static List<Vehicles.names> DatabaseIdToEnumListList(List<Int64> databaseIdList) {
            try {
                List<Vehicles.names> vehicleList = new List<Vehicles.names>();
                foreach (Int64 currentDatabaseId in databaseIdList) {
                    vehicleList.Add(VehiclesInformation.DatabaseIdToEnum(currentDatabaseId));
                }
                return (vehicleList);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        #endregion

        #endregion

        #region Get Vehicle informations
        /// <summary>
        /// Get Vehicle informations
        /// </summary>
        public static VehicleInformation Get(Vehicles.names id) {
            try {
                return (_listVehicleNames[id]);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        /// <summary>
        /// Get Vehicle informations
        /// </summary>
        public static VehicleInformation Get(Int64 dataBaseVehicleId) {
            try {
                return (_listDataBaseId[dataBaseVehicleId]);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested vehicle", err));
            }
        }
        #endregion

        #region Get Media Agency Flag
        /// <summary>
        /// Get Media Agency Flag ID
        /// </summary>
        public static Int64 GetMediaAgencyFlag(Vehicles.names id)
        {
            try
            {
                return (_listMediaAgencyFlagByVehicleNames[id]);
            }
            catch (System.Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// Get Media Agency Flag ID
        /// </summary>
        public static Int64 GetMediaAgencyFlagId(Int64 dataBaseVehicleId)
        {
            try
            {
                return (_listMediaAgencyFlagByVehicleIds[dataBaseVehicleId]);
            }
            catch (System.Exception)
            {
                return long.MinValue;
            }
        }
        /// <summary>
        /// Get Media Agency Flag ID list
        /// </summary>
        public static List<Int64> GetMediaAgencyFlagIds(List<Int64> dataBaseVehicleIds)
        {
            List<Int64> ids = new List<long>();
            try
            {
                for (int i = 0; i < dataBaseVehicleIds.Count; i++)
                {
                    if(_listMediaAgencyFlagByVehicleIds.ContainsKey(dataBaseVehicleIds[i]))
                        ids.Add(_listMediaAgencyFlagByVehicleIds[dataBaseVehicleIds[i]]);
                }
                return (ids);
            }
            catch (System.Exception)
            {
                return new List<long>(); 
            }
        }

        /// <summary>
        /// Get All Media Agency Flag Ids
        /// </summary>
        /// <returns>Flag Ids</returns>
        public static List<Int64> GetAllMediaAgencyFlagIds(){
            List<Int64> ids = new List<long>();
            try
            {
                foreach (KeyValuePair<Int64,Int64> kpv in _listMediaAgencyFlagByVehicleIds)
                {
                    ids.Add(kpv.Value);
                }
                return (ids);
            }
            catch (System.Exception)
            {
                return new List<long>();
            }
        }
        #endregion

        #region Contains
        /// <summary>
		/// Verifiy if contains vehicle Id
		/// </summary>
		/// <param name="dataBaseVehicleId">Database vehicle Id</param>
		/// <returns></returns>
		public static bool Contains(Int64 dataBaseVehicleId) {
			try {
				return (_listDataBaseId.ContainsKey(dataBaseVehicleId));
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to reteive the requested vehicle", err));
			}
		}
		/// <summary>
		/// Verifiy if contains vehicle Id
		/// </summary>
		/// <param name="dataBaseVehicleId">Database vehicle Id</param>
		/// <returns></returns>
        public static bool Contains(Vehicles.names vehicle)
        {
			try {
				return (_listVehicleNames.ContainsKey(vehicle));
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to reteive the requested vehicle", err));
			}
		}
        /// <summary>
        /// Verifiy if contains Media Agency flag for current vehicle Id
        /// </summary>
        /// <param name="dataBaseVehicleId">Database vehicle Id</param>
        /// <returns></returns>
        public static bool ContainsMediaAgencyFlag(Int64 dataBaseVehicleId)
        {
            try
            {
                return (_listMediaAgencyFlagByVehicleIds.ContainsKey(dataBaseVehicleId));
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Verifiy if contains Media Agency flag for all vehicle Ids
        /// </summary>
        /// <param name="dataBaseVehicleIds">Database vehicle Id</param>
        /// <returns></returns>
        public static bool ContainsMediaAgencyFlag(List<Int64> dataBaseVehicleIds)
        {
            try
            {
                if (dataBaseVehicleIds == null || dataBaseVehicleIds.Count == 0) return false;
                for (int i = 0; i < dataBaseVehicleIds.Count; i++)
                {
                    if (!_listMediaAgencyFlagByVehicleIds.ContainsKey(dataBaseVehicleIds[i])) return false;
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        /// <summary>
        // Verifiy if contains Media Agency flag for current vehicle  Name
        /// </summary>
        /// <param name="dataBaseVehicleId">Database vehicle Id</param>
        /// <returns></returns>
        public static bool ContainsMediaAgencyFlag(Vehicles.names vehicle)
        {
            try
            {
                return (_listMediaAgencyFlagByVehicleNames.ContainsKey(vehicle));
            }
            catch (System.Exception)
            {
                return false;
            }
        }
		#endregion

		#region Get commun UnitInformation list
		/// <summary>
        /// Get commun UnitInformation list
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <returns>Unit Information List</returns>
        public static List<CustomerSessions.Unit> GetCommunUnitList(List<Vehicles.names> vehicleList) {

            List<CustomerSessions.Unit> unitList = new List<CustomerSessions.Unit>();
            List<CustomerSessions.Unit> communUnitList = new List<CustomerSessions.Unit>();
            foreach (CustomerSessions.Unit currentKey in UnitsInformation.List.Keys) {
                unitList.Add(currentKey);
                communUnitList.Add(currentKey);
            }

            if (vehicleList.Count == 1)
                return Get(vehicleList[0]).AllowedUnitEnumList;
            else {

                foreach (Vehicles.names currentVehicle in vehicleList) {
                    foreach (CustomerSessions.Unit currentUnit in unitList)
                        if (!Get(currentVehicle).AllowedBaseUnitInformationList.Contains(currentUnit))
                            communUnitList.Remove(currentUnit);
                }
            }

            return communUnitList;

        }
        /// <summary>
        /// Get commun UnitInformation list
        /// </summary>
        /// <param name="vehicleList">Vehicle Information List</param>
        /// <returns>Unit Information List</returns>
        /// <remarks>This method is used when we have a dataBaseId list</remarks>
        public static List<CustomerSessions.Unit> GetCommunUnitList(List<Int64> vehicleList) {

            List<CustomerSessions.Unit> unitList = new List<CustomerSessions.Unit>();
            List<CustomerSessions.Unit> communUnitList = new List<CustomerSessions.Unit>();
            foreach (CustomerSessions.Unit currentKey in UnitsInformation.List.Keys) {
                unitList.Add(currentKey);
                communUnitList.Add(currentKey);
            }

            if (vehicleList.Count == 1)
                return Get(vehicleList[0]).AllowedUnitEnumList;
            else {
             
                foreach (Int64 currentVehicle in vehicleList) {
                    foreach (CustomerSessions.Unit currentUnit in unitList)
                        if (!Get(currentVehicle).AllowedBaseUnitInformationList.Contains(currentUnit))
                            communUnitList.Remove(currentUnit);
                }

            }

            return communUnitList;

        }
        #endregion

        #region Get commun DetailLevelItemInformation list
        /// <summary>
        /// Get commun DetailLevelItemInformation list
        /// </summary>
        /// <param name="vehicleList">Vehicle Information List</param>
        /// <returns>Detail Level Information List</returns>
        public static List<DetailLevelItemInformation.Levels> GetCommunDetailLevelList(List<Vehicles.names> vehicleList) {

            List<DetailLevelItemInformation.Levels> communlevelList = new List<DetailLevelItemInformation.Levels>();
            if (vehicleList.Count > 0) {
                List<DetailLevelItemInformation.Levels> levelList = Get(vehicleList[0]).AllowedMediaLevelItemsEnumList;
                if (vehicleList.Count == 1)
                    return levelList;
                else {
                    foreach (DetailLevelItemInformation.Levels currentKey in levelList) {
                        communlevelList.Add(currentKey);
                    }
                    foreach (Vehicles.names currentVehicle in vehicleList) {
                        foreach (DetailLevelItemInformation.Levels currentLevel in levelList)
                            if (!Get(currentVehicle).AllowedMediaLevelItemsEnumList.Contains(currentLevel))
                                communlevelList.Remove(currentLevel);
                    }
                }
            }
            return communlevelList;
        }
        /// <summary>
        /// Get commun DetailLevelItemInformation list
        /// </summary>
        /// <param name="vehicleList">Vehicle Information List</param>
        /// <returns>Detail Level Information List</returns>
        /// <remarks>This method is used when we have a dataBaseId list</remarks>
        public static List<DetailLevelItemInformation.Levels> GetCommunDetailLevelList(List<Int64> vehicleList) {

            List<DetailLevelItemInformation.Levels> communlevelList = new List<DetailLevelItemInformation.Levels>();
            if (vehicleList.Count > 0) {
                List<DetailLevelItemInformation.Levels> levelList = Get(vehicleList[0]).AllowedMediaLevelItemsEnumList;
                if (vehicleList.Count == 1)
                    return levelList;
                else {
                    foreach (DetailLevelItemInformation.Levels currentKey in levelList) {
                        communlevelList.Add(currentKey);
                    }
                    foreach (Int64 currentVehicle in vehicleList) {
                        foreach (DetailLevelItemInformation.Levels currentLevel in levelList)
                            if (!Get(currentVehicle).AllowedMediaLevelItemsEnumList.Contains(currentLevel))
                                communlevelList.Remove(currentLevel);
                    }
                }
            }

            return communlevelList;

        }
        #endregion

        #region Get DetailLevelItemInformation list

		/// <summary>
		/// Get Selection DetailLevelItemInformation list
		/// </summary>
		/// <param name="vehicleList">Vehicle Information List</param>
		/// <returns>Detail Level Information List</returns>
		/// <remarks>This method is used when we have a dataBaseId list</remarks>
		public static List<DetailLevelItemInformation> GetSelectionDetailLevelList(List<Int64> vehicleList) {

			List<DetailLevelItemInformation> selectionlevelList = new List<DetailLevelItemInformation>();
			if (vehicleList.Count > 0) {
				List<DetailLevelItemInformation> levelList = Get(vehicleList[0]).AllowedMediaSelectionLevelItemsList;
				if (vehicleList.Count == 1)
					return levelList;
				else {
					foreach (DetailLevelItemInformation currentKey in levelList) {
						selectionlevelList.Add(currentKey);
					}
					foreach (Int64 currentVehicle in vehicleList) {
						foreach (DetailLevelItemInformation currentLevel in levelList)
							if (!Get(currentVehicle).AllowedMediaSelectionLevelItemsList.Contains(currentLevel))
								selectionlevelList.Remove(currentLevel);
					}
				}
			}

			return selectionlevelList;
		}
        #endregion

        #region Init
        /// <summary>
        /// Initialisation de la liste à partir du fichier XML
        /// </summary>
        /// <param name="source">Source de données</param>
        public static void Init(IDataSource source) {
            _listVehicleNames.Clear();
            _listDataBaseId.Clear();
            _listMediaAgencyFlagByVehicleNames.Clear();
            List<VehicleInformation> vehicles = VehiclesInformationXL.Load(source);
            try {
                foreach (VehicleInformation currentVehicle in vehicles) {
                    _listVehicleNames.Add(currentVehicle.Id, currentVehicle);
                    _listDataBaseId.Add(currentVehicle.DatabaseId, currentVehicle);
                    if (currentVehicle.MediaAgencyFlag >= 0)
                    {
                        _listMediaAgencyFlagByVehicleNames.Add(currentVehicle.Id, currentVehicle.MediaAgencyFlag);
                        _listMediaAgencyFlagByVehicleIds.Add(currentVehicle.DatabaseId, currentVehicle.MediaAgencyFlag);
                    }
                }
            }
            catch (System.Exception err) {
                throw (new VehicleException("Impossible to build the vehicle list", err));
            }
        }
        #endregion

        #endregion

    }
}
