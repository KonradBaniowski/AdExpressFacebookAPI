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
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static VehiclesInformation() {
		}
		#endregion

        #region Méthodes publiques

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

        #region Init
        /// <summary>
        /// Initialisation de la liste à partir du fichier XML
        /// </summary>
        /// <param name="source">Source de données</param>
        public static void Init(IDataSource source) {
            _listVehicleNames.Clear();
            _listDataBaseId.Clear();
            List<VehicleInformation> vehicles = VehiclesInformationXL.Load(source);
            try {
                foreach (VehicleInformation currentVehicle in vehicles) {
                    _listVehicleNames.Add(currentVehicle.Id, currentVehicle);
                    _listDataBaseId.Add(currentVehicle.DatabaseId, currentVehicle);
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
