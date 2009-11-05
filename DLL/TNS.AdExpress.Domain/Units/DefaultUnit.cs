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

namespace TNS.AdExpress.Domain.Units {
    /// <summary>
    /// Default Unit
    /// </summary>
    public class DefaultUnit {

        #region Variables
        /// <summary>
        /// Default Unit Id
        /// </summary>
        private CustomerSessions.Unit _defaultUnit;
        /// <summary>
        /// Vehicle Name
        /// </summary>
        private Vehicles.names _vehicleName;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultUnit">defaultUnit</param>
        /// <param name="vehicleName">vehicleName</param>
        public DefaultUnit(CustomerSessions.Unit defaultUnit, Vehicles.names vehicleName) {
            _vehicleName = vehicleName;
            _defaultUnit = defaultUnit;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Default Unit
        /// </summary>
        public CustomerSessions.Unit Unit {
            get { return (_defaultUnit); }
        }

        /// <summary>
        /// Get Vehicle Name
        /// </summary>
        public Vehicles.names VehicleName {
            get { return (_vehicleName); }
        }
        #endregion

    }
}
