#region Informations
// Auteur: Y. R'kaina
// Création: 06/08/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.WebService.Domain.Configuration {
    /// <summary>
    /// Vehicles descriptions
    /// </summary>
    public class VehiclesCreativesInformation {

        #region variables
        ///<summary>
        /// Vehicles description list
        /// </summary>
        private static Dictionary<Int64, VehicleCreativesInformation> _vehicleCreativesList = new Dictionary<Int64, VehicleCreativesInformation>();
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static VehiclesCreativesInformation() {
		}
		#endregion

        #region Méthodes publiques

        #region Init
        /// <summary>
        /// Initialize Vehicle Creatives List
		/// </summary>
        /// <param name="vehicleCreativesList">Vehicle Creatives List</param>
        public static void Init(Dictionary<Int64, VehicleCreativesInformation> vehicleCreativesList) {
            _vehicleCreativesList = vehicleCreativesList;
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
				return (_vehicleCreativesList.ContainsKey(dataBaseVehicleId));
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to reteive the requested vehicle", err));
			}
		}
		#endregion

		#region Get Vehicle Creatives Information
		/// <summary>
        /// Get Vehicle Creatives Information
        /// </summary>
        /// <param name="dataBaseVehicleId">Data Base Vehicle Id</param>
        /// <returns>Vehicle Creatives Information</returns>
        public static VehicleCreativesInformation GetVehicleCreativesInformation(Int64 dataBaseVehicleId) {
            if(Contains(dataBaseVehicleId))
                return _vehicleCreativesList[dataBaseVehicleId];
            else 
                return null;
        }
        #endregion

        #endregion

    }
}
