#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Domain.Units {
    /// <summary>
    /// Default Unit List
    /// </summary>
    public class DefaultUnitList {

        #region Variables
        /// <summary>
        /// Default Unit Id
        /// </summary>
        private CustomerSessions.Unit _defaultUnit;
        /// <summary>
        /// Default Unit List
        /// </summary>
        private Dictionary<Vehicles.names, DefaultUnit> _defaultUnitList;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="allowUnit">allowUnit</param>
        /// <param name="defaultUnit">default Unit is used if it is not defined in object allowUnit</param>
        public DefaultUnitList(Dictionary<Vehicles.names, DefaultUnit> defaultUnitList, CustomerSessions.Unit defaultUnit) {
            _defaultUnitList = defaultUnitList;
            _defaultUnit = defaultUnit;
        }
        #endregion

        #region Public Method

        #region GetDefaultUnit
        /// <summary>
        /// GetDefaultUnit
        /// </summary>
        /// <param name="vehicleName">Vehicle Name</param>
        /// <returns>default unit for current vehicle Name</returns>
        public CustomerSessions.Unit GetDefaultUnit(Vehicles.names vehicleName) {
            if (_defaultUnitList != null && _defaultUnitList.ContainsKey(vehicleName)) {
                return _defaultUnitList[vehicleName].Unit;
            }
            else {
                return _defaultUnit;
            }
        }
        /// <summary>
        /// GetDefaultUnit
        /// </summary>
        /// <param name="vehicleName">Vehicle Id</param>
        /// <returns>default unit for current vehicle Id</returns>
        public CustomerSessions.Unit GetDefaultUnit(Int64 vehicleId) {
            return GetDefaultUnit(VehiclesInformation.DatabaseIdToEnum(vehicleId));
        }
        #endregion

        #endregion

    }
}
