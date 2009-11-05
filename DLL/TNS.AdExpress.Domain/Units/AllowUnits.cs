#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.Units {
    /// <summary>
    /// Allow Unit List
    /// </summary>
    public class AllowUnits {

        #region Variables
        /// <summary>
        /// Default Unit Id
        /// </summary>
        private CustomerSessions.Unit _defaultAllowUnit;
        /// <summary>
        /// Unit List
        /// </summary>
        private List<CustomerSessions.Unit> _allowUnitList = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        internal AllowUnits() {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public AllowUnits(CustomerSessions.Unit defaultAllowUnit, List<CustomerSessions.Unit> allowUnitList) {
            if (allowUnitList == null) throw new ArgumentNullException("allowUnitList parameters is null");
            if (allowUnitList.Count <= 0) throw new ArgumentException("allowUnitList parameters is invalid");
            if (!allowUnitList.Contains(defaultAllowUnit)) throw new ArgumentException("defaultAllowUnit parameters is not defined in allowUnitList parameters");
            _allowUnitList = allowUnitList;
            _defaultAllowUnit = defaultAllowUnit;
        }

        /// <summary>
        /// Constructor (default unit is the first in allowUnitList parameters)
        /// </summary>
        /// <param name="allowUnitList">Unit List</param>
        public AllowUnits(List<CustomerSessions.Unit> allowUnitList) {
            if (allowUnitList == null) throw new ArgumentNullException("allowUnitList parameters is null");
            if (allowUnitList.Count <= 0) throw new ArgumentException("allowUnitList parameters is invalid");
            _allowUnitList = allowUnitList;
            _defaultAllowUnit = allowUnitList[0];
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get default unit Id
        /// </summary>
        public CustomerSessions.Unit DefaultAllowUnit {
            get { return (_defaultAllowUnit); }
            set { _defaultAllowUnit = value; }
        }

        /// <summary>
        /// Get Unit List
        /// </summary>
        public List<CustomerSessions.Unit> AllowUnitList {
            get { return (_allowUnitList); }
            set { _allowUnitList = value; }
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Verify Object
        /// </summary>
        internal void Verify() {
            if (_allowUnitList == null) throw new ArgumentNullException("_allowUnitList is null");
            if (_allowUnitList.Count <= 0) throw new ArgumentException("_allowUnitList is invalid");
            if (!_allowUnitList.Contains(_defaultAllowUnit)) throw new ArgumentException("_defaultAllowUnit is not defined in allowUnitList");
        }
        #endregion

    }
}
